

using System;
using System.Collections.Generic;

namespace AppUtil.ErrorCode
{
    public class SpecialErrorCode
    {
        private readonly Dictionary<string, Func<string, string, string>> _specialActions;
        public int MaxLength { get; set; }
        public SpecialErrorCode()
        {
            _specialActions = new Dictionary<string, Func<string, string, string>>();
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
            funcName = funcName.Trim().ToUpper();
            _specialActions.Remove(funcName);
        }
        /// <summary>
        /// Add a specialAction that The custom errorcode function to SpecialErrorCode
        /// param1="logText", param2="BaseErrorCode", param3 = out 'a new custom errorcode'
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="specialAction">param1="logText", param2="BaseErrorCode", param3 = out 'a new custom errorcode' </param>
        /// <returns>true if the specialAction add success; otherwise false</returns>
        public bool AddSpecialAction(string funcName, Func<string, string, string> specialAction)
        {
            if (string.IsNullOrWhiteSpace(funcName) || specialAction == null)
            {
                return false;
            }
            funcName = funcName.Trim().ToUpper();
            _specialActions[funcName] = specialAction;
            return true;
        }

        /// <summary>
        /// Check and try to get a custom error by logtext and function name
        /// </summary>
        /// <param name="funcName">Failed function name</param>
        /// <param name="logText">The log test</param>
        /// <param name="defaultErrorcode">Default Errorcode, which can be an empty string</param>
        /// <param name="errorcode">Out a errorcode</param>
        /// <returns>true if this function contain a custom errorcode by function name; otherwise, false</returns>
        internal bool IsSpecial(string funcName, string logText, string defaultErrorcode, out string errorcode)
        {
            errorcode = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(funcName) || string.IsNullOrWhiteSpace(logText))
                {
                    return false;
                }
                funcName = funcName.Trim().ToUpper();
                if (_specialActions.TryGetValue(funcName, out var specialAction))
                {
                    defaultErrorcode = defaultErrorcode ?? string.Empty;
                    errorcode = specialAction?.Invoke(logText, defaultErrorcode);
                    return true;
                }
                return false;
            }
            finally
            {
                if (string.IsNullOrWhiteSpace(errorcode))
                {
                    errorcode = defaultErrorcode ?? string.Empty;
                }
                else if(errorcode.Length > MaxLength)
                {
                    errorcode = errorcode.Substring(0, MaxLength);
                }
            }
        }
    }
}
