using System;

namespace AppUtil.Service
{
    public class CheckTestFailedCondition
    {
        private const string ERROR_KEY = "ERROR_KEY";
        private const string COUNT_KEY = "COUNT_KEY";
        private const string SPEC_KEY = "SPEC_KEY";
        private const string MAC_KEY = "MAC_KEY";
        private readonly RegistryUtil registry;
        private int _index;

        public int Index { get => _index; private set => _index = value < 0 ? 0 : value; }

        internal CheckTestFailedCondition(int index)
        {
            Index = index;
            registry = new RegistryUtil(@"Software\TestCondition\CheckTestFailed");
        }

        public bool IsOldMac(string mac)
        {
            if (string.IsNullOrWhiteSpace(OldMac) || string.IsNullOrWhiteSpace(mac))
            {
                return false;
            }
            if (OldMac.Equals(mac, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public bool IsFailedTimeOutOfSpec => Count >= Spec;


        public void SetFailed(string errorcode)
        {
            if (string.IsNullOrWhiteSpace(errorcode))
            {
                return;
            }
            if (ErrorCode != errorcode)
            {
                ErrorCode = errorcode;
                Count = 1;
            }
            else
            {
                Count++;
            }

        }

        public void SetPass()
        {
            ErrorCode = "";
            OldMac = "";
            Count = 0;
        }


        public string ErrorCode
        {
            get
            {
                return registry.GetValue<string>($"{ERROR_KEY}-{Index}", null);
            }
            private set
            {
                registry.SaveStringValue($"{ERROR_KEY}-{Index}", value ?? "");
            }
        }
        public string OldMac
        {
            get
            {
                return registry.GetValue<string>($"{MAC_KEY}-{Index}", null);
            }
            set
            {
                registry.SaveStringValue($"{MAC_KEY}-{Index}", value ?? "");
            }
        }

        public int Count
        {
            get
            {
                return registry.GetValue($"{COUNT_KEY}-{Index}", 0);
            }
            private set
            {
                registry.SaveIntValue($"{COUNT_KEY}-{Index}", value < 0 ? 0 : value);
            }
        }

        public int Spec
        {
            get
            {
                return registry.GetValue($"{SPEC_KEY}-{Index}", 3);
            }
            set
            {
                registry.SaveIntValue($"{SPEC_KEY}-{Index}", value < 1 ? 1 : value);
            }
        }

    }
}
