using System;
using System.Collections.Generic;

namespace AppUtil.Service
{
    public class CheckConditionFactory
    {
        private static readonly Dictionary<int, CheckCondition> checkConditions = new Dictionary<int, CheckCondition>();
        public static CheckCondition GetInstanceOf(int index)
        {
            if (index < 0)
            {
                index = 0;
            }
            if (checkConditions.TryGetValue(index, out var checkCondition))
            {
                return checkCondition;
            }
            checkCondition = new CheckCondition(index);
            checkConditions.Add(index, checkCondition);
            return checkCondition;
        }
    }
}
