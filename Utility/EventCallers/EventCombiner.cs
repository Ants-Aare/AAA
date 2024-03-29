using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;


namespace AAA.Utility.EventCallers
{
    public class EventCombiner : MonoBehaviour
    {
        [ShowNonSerializedField, ReadOnly] private bool hasBeenCalled = false;
        [SerializeField] private UnityEvent combinedEvent;

        public void CallEvent()
        {
            if(hasBeenCalled)
                return;
            hasBeenCalled = true;
            combinedEvent?.Invoke();

            StartCoroutine(ResetHasBeenCalled());
        }

        private IEnumerator ResetHasBeenCalled()
        {
            yield return new WaitForEndOfFrame();
            hasBeenCalled = false;
        }
    }
}