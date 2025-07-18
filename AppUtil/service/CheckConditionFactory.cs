using System;
using System.Collections.Generic;

namespace AppUtil.Service
{
    public class CheckConditionFactory
    {
        private static readonly Dictionary<int, CheckTestFailedCondition> checkConditions = new Dictionary<int, CheckTestFailedCondition>();
        private static readonly Dictionary<int, GoldenVerify> goldenVerifys = new Dictionary<int, GoldenVerify>();
        public static CheckTestFailedCondition GetCheckTestFailedConditionInstanceOf(int index)
        {
            if (index < 0)
            {
                index = 0;
            }
            if (checkConditions.TryGetValue(index, out var checkCondition))
            {
                return checkCondition;
            }
            checkCondition = new CheckTestFailedCondition(index);
            checkConditions.Add(index, checkCondition);
            return checkCondition;
        }
        public static GoldenVerify GetGoldenVerifyInstanceOf(int index)
        {
            if (index < 0)
            {
                index = 0;
            }
            if (goldenVerifys.TryGetValue(index, out var goldenVerify))
            {
                return goldenVerify;
            }
            goldenVerify = new GoldenVerify(index);
            goldenVerifys.Add(index, goldenVerify);
            return goldenVerify;
        }
    }
}
