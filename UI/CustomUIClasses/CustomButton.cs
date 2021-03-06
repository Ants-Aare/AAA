using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System.Collections;
using Lean.Transition;
using Sirenix.OdinInspector;

namespace AAA.UI.CustomUI
{
    public class CustomButton : Selectable, IBeginDragHandler, IDragHandler, IEndDragHandler, ISubmitHandler
    {
        #region Properties
        #if UNITY_EDITOR
        [SerializeField] [PropertyOrder(-5)] private bool showAdvancedSettings = false;
        #endif

        [TabGroup("Properties")][SerializeField]protected float dragThreshold = 10.0f;
        [TabGroup("Properties")][SerializeField]protected bool allowMultiplePointers;

        [Tooltip("This means the button can only trigger the OnClicked Event once. Use ResetIsClicked() to make it fire the event again.")]
        [TabGroup("Properties")][SerializeField][ShowIf("showAdvancedSettings")] private bool onlyAllowClickingOnce = false;
        [Tooltip("You can't click the button this many seconds after it has been pressed.")]
        [TabGroup("Properties")][SerializeField][ShowIf("showAdvancedSettings")] private float clickTimeOut = 0.1f;
        [Tooltip("Which Object to Select after Clicking the button. QOL thing. Defaults to itself when unassigned.")]
        [TabGroup("Properties")][SerializeField][ShowIf("showAdvancedSettings")] private GameObject firstSelectedWhenClicked = null;
        [TabGroup("Properties")][SerializeField][ShowIf("showAdvancedSettings")] private bool sendLogs = false;


        [TabGroup("Transitions")][SerializeField] protected LeanPlayer normalTransitions = new LeanPlayer();
        [TabGroup("Transitions")][SerializeField] protected LeanPlayer downTransitions = new LeanPlayer();
        [TabGroup("Transitions")][SerializeField] protected LeanPlayer clickTransitions = new LeanPlayer();
        [TabGroup("Transitions")][SerializeField] protected LeanPlayer selectedTransitions = new LeanPlayer();


        [TabGroup("Button Events")]
        [TabGroup("Button Events")][SerializeField] protected UnityEvent onButtonDown;
        [TabGroup("Button Events")][SerializeField] protected UnityEvent onButtonClicked;
        [TabGroup("Button Events")][SerializeField] protected UnityEvent onButtonSelected;
        [TabGroup("Button Events")][SerializeField] protected UnityEvent onButtonDeselected;


        [TabGroup("State")][ShowInInspector, ReadOnly] protected bool isSelected = false;
        [TabGroup("State")][ShowInInspector, ReadOnly] protected bool isDown = false;
        [TabGroup("State")][ShowInInspector, ReadOnly] protected bool isClicked = false;
        [TabGroup("State")][ShowInInspector, ReadOnly] protected Vector2 totalDelta;
        [TabGroup("State")][ShowInInspector, ReadOnly] protected List<int> downPointers = new List<int>();
        [TabGroup("State")][ShowInInspector, ReadOnly] protected ScrollRect parentScrollRect;

        #endregion

        #region Select
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsInteractable())
                return;

            StartSelect();
            eventData.selectedObject = gameObject;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!IsInteractable())
                return;

            Log($"Exiting");
            if (isDown)
            {
                if (dragThreshold == 0.0f)
                {
                    downPointers.Remove(eventData.pointerId);

                    if (downPointers.Count == 0)
                    {
                        isDown = false;
                        StopSelect();
                        DoNormal();
                    }
                }
            }
            else
            {
                StopSelect();
                DoNormal();
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if (!IsInteractable())
                return;

            StartSelect();
        }
        public override void OnDeselect(BaseEventData eventData)
        {
            if (!IsInteractable())
                return;

            isDown = false;
            StopSelect();
            DoNormal();
        }

        #endregion

        #region Submitting and Clicking
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable() || eventData.button != PointerEventData.InputButton.Left)
                return;

            if (navigation.mode != Navigation.Mode.None && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(gameObject, eventData);


            totalDelta = Vector2.zero;

            downPointers.Add(eventData.pointerId);

            if (allowMultiplePointers == true || downPointers.Count == 1)
            {
                DoDown();
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (downPointers.Remove(eventData.pointerId) == true)
            {
                isDown = false;
                DoClick();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!enabled || !IsInteractable())
                return;

            eventData.selectedObject = gameObject;
            DoClick();
        }

        #endregion

        #region Drag Behaviour
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(parentScrollRect == null)
                parentScrollRect = GetComponentInParent<ScrollRect>();

            parentScrollRect?.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!IsInteractable())
                return;

            if (downPointers.Contains(eventData.pointerId) == true)
            {
                totalDelta += eventData.delta;

                if (dragThreshold > 0.0f && totalDelta.magnitude > dragThreshold)
                {
                    downPointers.Remove(eventData.pointerId);

                    if (downPointers.Count == 0)
                    {
                        isDown = false;
                        StopSelect();
                        DoNormal();
                    }
                }
            }

            parentScrollRect?.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            parentScrollRect?.OnEndDrag(eventData);
        }

        #endregion

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            return;
        }

        private void StartSelect()
        {
            if (isSelected)
                return;

            Log("StartSelect");
            selectedTransitions?.Begin();
            onButtonSelected?.Invoke();
            isSelected = true;
        }

        private void StopSelect()
        {
            if (!isSelected)
                return;

            Log("StopSelect");
            onButtonDeselected?.Invoke();
            isSelected = false;
        }

        protected void DoNormal()
        {
            normalTransitions?.Begin();
        }

        protected void DoDown()
        {
            if (isDown)
                return;

            onButtonDown?.Invoke();
            downTransitions?.Begin();

            isDown = true;
        }

        protected virtual void DoClick()
        {
            if (isClicked)
                return;

            onButtonClicked?.Invoke();
            clickTransitions?.Begin();

            if (gameObject.activeInHierarchy)
            {
                isClicked = true;
                isSelected = false;
                StopAllCoroutines();
                StartCoroutine(ClickTimeout());
            }
        }


        public void ResetIsClicked()
        {
            isClicked = false;
        }

        public IEnumerator ClickTimeout()
        {
            yield return new WaitForSeconds(clickTimeOut);

            if (firstSelectedWhenClicked != null)
                EventSystem.current?.SetSelectedGameObject(firstSelectedWhenClicked);
            else
            {
                Log("Doing Selectiontransition");
                selectedTransitions?.Begin();
                isSelected = true;
            }

            if (!onlyAllowClickingOnce)
                ResetIsClicked();
        }
        #if UNITY_EDITOR
        [Button("Visualize Navigation")][TabGroup("Properties")][PropertyOrder(-1)]
        public void Visualize()
        {
            string s_ShowNavigationKey = "SelectableEditor.ShowNavigation";
            bool currentVisualization = EditorPrefs.GetBool(s_ShowNavigationKey);
            EditorPrefs.SetBool(s_ShowNavigationKey, !currentVisualization);
        }
        #endif
        public void Log(string message)
        {
            if(sendLogs)
                Debug.Log($"Button {gameObject.name}: {message}", gameObject);
        }
    }
}