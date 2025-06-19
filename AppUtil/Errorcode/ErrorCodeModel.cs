using System.Collections.Generic;

namespace AppUtil.ErrorCode
{
    internal class ErrorCodeModel
    {
        private readonly Dictionary<string, string> errorcodes;

        public ErrorCodeModel()
        {
            errorcodes = new Dictionary<string, string>();
        }

        public void Remove(string funcName)
        {
            if (string.IsNullOrWhiteSpace(funcName))
            {
                return;
            }
            funcName = funcName.Trim().ToUpper();
            errorcodes.Remove(funcName);
        }

        public bool Set(string funcName, string errorcode)
        {
            if (string.IsNullOrWhiteSpace(funcName) || string.IsNullOrWhiteSpace(errorcode))
            {
                return false;
            }
            funcName = funcName.Trim().ToUpper();
            errorcode = errorcode.Trim().ToUpper();
            errorcodes[funcName] = errorcode;
            return true;
        }

        public bool TryGet(string funcName, out string errorcode)
        {
            if (string.IsNullOrWhiteSpace(funcName))
            {
                errorcode = null;
                return false;
            }
            funcName = funcName.Trim().ToUpper();
            return errorcodes.TryGetValue(funcName, out errorcode);
        }
    }
}
