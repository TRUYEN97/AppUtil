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
        private static readonly Lazy<CheckCondition> instance = new Lazy<CheckCondition>(() => new CheckCondition());



        private static CheckCondition Instance => instance.Value;

        public static bool IsOldMac(string mac)
        {
            if (string.IsNullOrWhiteSpace(Instance.Mac) || string.IsNullOrWhiteSpace(mac))
            {
                return false;
            }
            if (Instance.Mac.Equals(mac, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
        public static void SetMac(string mac)
        {
            Instance.Mac = mac;
        }

        public static bool IsFailedTimeOutOfSpec => Instance.Count >= Instance.Spec;
        public static int GetSpec => Instance.Spec;

        public static void SetSpec(int spec)
        {
            Instance.Spec = spec < 1 ? 1 : spec;
        }

        public static void SetFailed(string errorcode)
        {
            if (string.IsNullOrWhiteSpace(errorcode))
            {
                return;
            }
            if (Instance.ErrorCode != errorcode)
            {
                Instance.ErrorCode = errorcode;
                Instance.Count = 1;
            }
            else
            {
                Instance.Count++;
            }

        }

        public static void SetPass()
        {
            Instance.ErrorCode = "";
            Instance.Mac = "";
            Instance.Count = 0;
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
                key.SetValue(keyWord, value?? "");
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


        private string ErrorCode
        {
            get
            {
                return GetValue<string>(ERROR_KEY, null);
            }
            set
            {
                SaveStringValue(ERROR_KEY, value);
            }
        }
        private string Mac
        {
            get
            {
                return GetValue<string>(MAC_KEY, null);
            }
            set
            {
                SaveStringValue(MAC_KEY, value);
            }
        }

        private int Count
        {
            get
            {
                return GetValue(COUNT_KEY, 0);
            }
            set
            {
                SaveIntValue(COUNT_KEY, value);
            }
        }

        private int Spec
        {
            get
            {
                return GetValue(SPEC_KEY, 3);
            }
            set
            {
                SaveIntValue(SPEC_KEY, value);
            }
        }

    }
}
