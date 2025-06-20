

using System;
using System.Collections.Generic;

namespace AppUtil.ErrorCode
{
    public class SpecialErrorCode
    {
        private readonly Dictionary<string, Func<string, string>> _specialActions;
        public SpecialErrorCode()
        {
            _specialActions = new Dictionary<string, Func<string, string>>();
        }
        public void Clear()
        {
            _specialActions.Clear();
        }

        public void Remove(string funcName)
        {
            if (string.IsNullOrWhiteSpace(funcName))
            {
                return;
            }
            _specialActions.Remove(funcName);
        }
        public bool AddSpecialAction(string funcName, Func<string, string> specialAction)
        {
            if (string.IsNullOrWhiteSpace(funcName) || specialAction == null)
            {
                return false;
            }
            _specialActions[funcName] = specialAction;
            return true;
        }

        internal bool IsSpecial(string funcName, string logText, out string errorcode)
        {
            errorcode = null;
            if (string.IsNullOrWhiteSpace(funcName))
            {
                return false;
            }
            funcName = funcName.Trim().ToLower();
            if (_specialActions.TryGetValue(funcName, out var specialAction))
            {
                errorcode = specialAction?.Invoke(logText ?? "");
                return true;
            }
            return false;
        }
    }
}
