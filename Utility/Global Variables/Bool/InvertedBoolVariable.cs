using UnityEngine;

namespace AAA.Utility.GlobalVariables
{
    [CreateAssetMenu(menuName = "Variable/Inverted Bool Variable")]
    public class InvertedBoolVariable : BoolVariable
    {
        [SerializeField] private BoolVariable sourceBoolVariable;

        protected override void OnEnable()
        {
            base.OnEnable();
            if(sourceBoolVariable != null)  
                sourceBoolVariable.OnChanged += OnSourceChanged;
        }
        protected override void OnDisable()
        {
            base.OnEnable();
            if(sourceBoolVariable != null)  
                sourceBoolVariable.OnChanged -= OnSourceChanged;
        }
        public void OnSourceChanged()
        {
            if(sourceBoolVariable != null)  
                Value = !sourceBoolVariable.Value;
        }

        public override void Save()
        {
        }
        public override void InitializeVariable()
        {
            base.InitializeVariable();

            isInitialized = true;
        }
    }
}