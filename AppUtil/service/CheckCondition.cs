using System;
using Microsoft.Win32;

namespace AppUtil.Service
{
    public class CheckCondition
    {
        private const string SUB_KEY = @"Software\TestCondition";
        private const string ERROR_KEY = "ERROR_KEY";
        private const string COUNT_KEY = "COUNT_KEY";
        private const string SPEC_KEY = "SPEC_KEY";
        private const string MAC_KEY = "MAC_KEY";
        private int _index;

        public int Index { get => _index; private set => _index = value < 0 ? 0 : value; }

        internal CheckCondition(int index)
        {
            Index = index;
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
                return GetValue<string>($"{ERROR_KEY}-{Index}", null);
            }
            private set
            {
                SaveStringValue($"{ERROR_KEY}-{Index}", value ?? "");
            }
        }
        public string OldMac
        {
            get
            {
                return GetValue<string>($"{MAC_KEY}-{Index}", null);
            }
            set
            {
                SaveStringValue($"{MAC_KEY}-{Index}", value ?? "");
            }
        }

        public int Count
        {
            get
            {
                return GetValue($"{COUNT_KEY}-{Index}", 0);
            }
            private set
            {
                SaveIntValue($"{COUNT_KEY}-{Index}", value < 0 ? 0 : value);
            }
        }

        public int Spec
        {
            get
            {
                return GetValue($"{SPEC_KEY}-{Index}", 3);
            }
            set
            {
                SaveIntValue($"{SPEC_KEY}-{Index}", value < 1 ? 1 : value);
            }
        }


        private static void SaveIntValue(string keyWord, int value)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(SUB_KEY))
            {
                key.SetValue(keyWord, value);
            }
        }

        private static void SaveStringValue(string keyWord, string value)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(SUB_KEY))
            {
                key.SetValue(keyWord, value ?? "");
            }
        }
        private static T GetValue<T>(string keyWord, T def)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(SUB_KEY))
            {
                if (key == null)
                {
                    return def;
                }
                return (T)key.GetValue(keyWord, def);
            }
        }
    }
}
