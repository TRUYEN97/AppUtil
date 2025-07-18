using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AppUtil.Service
{
    public class GoldenVerify
    {
        private const string LAST_TIME_GOOD_GOLDEN_TEST = "LAST_TIME_GOOD_GOLDEN_TEST_KEY";
        private const string LAST_TIME_BAD_GOLDEN_TEST = "LAST_TIME_BAD_GOLDEN_TEST_KEY";
        private const string SPEC_KEY = "SPEC_KEY";
        public readonly HashSet<string> GoodGoldens;
        public readonly HashSet<string> BadGoldens;
        private readonly RegistryUtil registry;
        private int _index;
        internal GoldenVerify(int index)
        {
            Index = index;
            GoodGoldens = new HashSet<string>();
            BadGoldens = new HashSet<string>();
            registry = new RegistryUtil(@"Software\TestCondition\GoldenVerify");
            GoodGoldenEnable = true;
            BadGoldenEnable = true;
            Spec = 12.0;
        }
        public bool GoodGoldenEnable { get; set; }
        public bool BadGoldenEnable { get; set; }
        public bool IsCanTest(string mac)
        {
            if (IsTimeOutTestGoodGolden && IsTimeOutTestBadGolden)
            {
                return (!GoodGoldenEnable && !BadGoldenEnable) 
                    || IsGoodGoldenMac(mac)  
                    || IsBadGoldenMac(mac);
            }
            if (GoodGoldenEnable && IsTimeOutTestGoodGolden)
            {
                return IsGoodGoldenMac(mac);
            }
            if (BadGoldenEnable && IsTimeOutTestBadGolden)
            {
                return IsBadGoldenMac(mac);
            }
            return true;
        }

        public void CopyLog(string fileSource, string fileTaget)
        {
            FileUtil.CopyFile(fileSource, fileTaget);
        }
        public void SaveLog(string filePath, ISerializable text)
        {
            FileUtil.WriteAllText(filePath, text);
        }

        public bool IsGoldenMacResult(string mac, bool status)
        {
            if (IsGoodGoldenMac(mac))
            {
                if (status)
                {
                    ResetLastTimeTestGoodGolden();
                }
                return true;
            }
            if (IsBadGoldenMac(mac))
            {
                if (!status)
                {
                    ResetLastTimeTestBadGolden();
                }
                return true;
            }
            return false;
        }

        public void ResetLastTimeTestBadGolden()
        {
            LastTimeTestBadGolden = DateTime.Now;
        }
        public void ResetLastTimeTestGoodGolden()
        {
            LastTimeTestGoodGolden = DateTime.Now;
        }
        public bool IsGoodGoldenMac(string mac)
        {
            return GoodGoldens.Contains(mac);
        }
        public bool IsBadGoldenMac(string mac)
        {
            return BadGoldens.Contains(mac);
        }

        public bool IsTimeOutTestGoodGolden => (DateTime.Now - LastTimeTestGoodGolden).TotalHours >= Spec;
        public bool IsTimeOutTestBadGolden => (DateTime.Now - LastTimeTestBadGolden).TotalHours >= Spec;
        public int Index { get => _index; private set => _index = value < 0 ? 0 : value; }

        public double Spec
        {
            get
            {
                return registry.GetValue<double>($"{SPEC_KEY}-{Index}", 12.0);
            }
            set
            {
                registry.SaveDoubleValue($"{SPEC_KEY}-{Index}", value);
            }
        }
        public DateTime LastTimeTestBadGolden
        {
            get
            {
                string timeString = registry.GetValue($"{LAST_TIME_BAD_GOLDEN_TEST}-{Index}", "");
                if (DateTime.TryParse(timeString, out var parsedTime))
                {
                    return parsedTime;
                }
                return default;
            }
            private set
            {
                registry.SaveStringValue($"{LAST_TIME_BAD_GOLDEN_TEST}-{Index}", value.ToString("o"));
            }
        }
        public DateTime LastTimeTestGoodGolden
        {
            get
            {
                string timeString = registry.GetValue($"{LAST_TIME_GOOD_GOLDEN_TEST}-{Index}", "");
                if (DateTime.TryParse(timeString, out var parsedTime))
                {
                    return parsedTime;
                }
                return default;
            }
            private set
            {
                registry.SaveStringValue($"{LAST_TIME_GOOD_GOLDEN_TEST}-{Index}", value.ToString("o"));
            }
        }
    }
}
