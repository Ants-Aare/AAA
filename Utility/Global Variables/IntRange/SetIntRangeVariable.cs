using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using AAA.Utility.DataTypes;

namespace AAA.Utility.GlobalVariables
{
    public class SetIntRangeVariable : SetGlobalVariable<IntRangeValue, IntRangeVariable>
    {
        public void Increase(int amount)
        {
            variable.Value.Value += amount;
        }
        public void Decrease(int amount)
        {
            variable.Value.Value -= amount;
        }
        
        public void Increment()
        {
            variable.Value.Value++;
        }
        public void Decrement()
        {
            variable.Value.Value--;
        }

        public void SetVariableValue(int newValue)
        {
            variable.Value.Value = newValue;
            if (saveVariable)
                variable.Save();
        }
        public void SetVariableMinValue(int newValue)
        {
            variable.Value.MinValue = newValue;
            if (saveVariable)
                variable.Save();
        }
        public void SetVariableMaxValue(int newValue)
        {
            variable.Value.MaxValue = newValue;
            if (saveVariable)
                variable.Save();
        }
        public void SetVariableProgress(float newValue)
        {
            variable.Value.SetProgress(newValue);
            if (saveVariable)
                variable.Save();
        }
        [Button]
        public void SetRandomProgress()
        {
            SetVariableProgress(Random.Range(0f, 1f));
            if (saveVariable)
                variable.Save();
        }
    }
}