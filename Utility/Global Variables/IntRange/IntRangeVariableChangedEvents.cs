using System;
using UnityEngine;
using UnityEngine.Events;
using AAA.Utility.CustomUnityEvents;
using Sirenix.OdinInspector;
using AAA.Utility.DataTypes;

namespace AAA.Utility.GlobalVariables
{
    public class IntRangeVariableChangedEvents : GlobalVariableChangedEvents<IntRangeValue, IntRangeVariable>
    {
        [TabGroup("Events")][SerializeField] private IntRangeValueUnityEvent onChanged;
        [TabGroup("Events")][SerializeField] private UnityEvent onIncreased, onDecreased, onReachedMin, onReachedMax;
        private int cachedValue = 0;

        protected override void OnEnable()
        {
            base.OnEnable();
            cachedValue = variable.Value.Value;
        }
        protected override void OnChanged()
        {
            onChanged?.Invoke(variable.Value);

            int outputValue = variable.Value.Value;

            if (outputValue > cachedValue)
            {
                onIncreased?.Invoke();
            }
            else if (outputValue < cachedValue)
            {
                onDecreased?.Invoke();
            }
            if(variable.Value.GetProgress() == 1)
            {
                onReachedMax?.Invoke();
            }
            else if(variable.Value.GetProgress() == 0)
            {
                onReachedMin?.Invoke();
            }
            cachedValue = outputValue;
        }
    }
}