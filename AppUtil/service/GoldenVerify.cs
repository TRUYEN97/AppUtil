using System;

namespace AppUtil.Service
{
    public class GoldenVerify
    {
        private const string LAST_TIME_KEY = "LAST_TIME_GOLDEN_TEST_KEY";
        private const string SPEC_KEY = "SPEC_KEY";
        private const string GOLDEN_MAC_KEY = "GOLDEN_MAC_KEY";
        private readonly RegistryUtil registry;
        private int _index;
        internal GoldenVerify(int index)
        {
            Index = index;
            registry = new RegistryUtil(@"Software\TestCondition\GoldenVerify");
            Enable = true;
            Spec = 12.0;
            GoldenMac = "";
        }
        public bool Enable { get; set; }
        public bool IsCanTest(string mac)
        {
            if (Enable && IsTimeOut && !IsGoldenMac(mac))
            {
                return false;
            }
            return true;
        }

        public bool IsGoldenMacResult(string mac, bool status)
        {
            if (IsGoldenMac(mac))
            {
                if (status)
                {
                    ResetLastTime();
                }
                return true;
            }
            return false;
        }

        public void ResetLastTime()
        {
            LastTime = DateTime.Now;
        }

        public bool IsGoldenMac(string mac)
        {
            return !string.IsNullOrEmpty(GoldenMac) && mac?.ToUpper() == GoldenMac;
        }
        public bool IsTimeOut => (DateTime.Now - LastTime).TotalHours >= Spec;
        public int Index { get => _index; private set => _index = value < 0 ? 0 : value; }
        public string GoldenMac
        {
            get
            {
                return registry.GetValue($"{GOLDEN_MAC_KEY}-{Index}", "")?.ToUpper().Trim();
            }
            set
            {
                registry.SaveStringValue($"{GOLDEN_MAC_KEY}-{Index}", value?.ToUpper().Trim() ?? "");
            }
        }
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
        public DateTime LastTime
        {
            get
            {
                string timeString = registry.GetValue($"{LAST_TIME_KEY}-{Index}", "");
                if (DateTime.TryParse(timeString, out var parsedTime))
                {
                    return parsedTime;
                }
                return default;
            }
            private set
            {
                registry.SaveStringValue($"{LAST_TIME_KEY}-{Index}", value.ToString("o"));
            }
        }
    }
}
