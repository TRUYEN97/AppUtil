
using System.Globalization;
using System;
using Microsoft.Win32;

namespace AppUtil.Service
{
    public class RegistryUtil
    {
        private readonly string SubKey;
        public RegistryUtil(string key)
        {
            SubKey = key;
        }

        public bool SaveIntValue(string keyWord, int value)
        {
            return SetValue(keyWord, value);
        }

        public bool SaveLongValue(string keyWord, long value)
        {
            return SetValue(keyWord, value);
        }

        public bool SaveDoubleValue(string keyWord, double value)
        {
            return SetValue(keyWord, value);
        }

        public bool SaveStringValue(string keyWord, string value)
        {
            return SetValue(keyWord, value);
        }
        public T GetValue<T>(string keyWord, T def)
        {
            if (string.IsNullOrWhiteSpace(SubKey)) { return def; }
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(SubKey))
            {
                if (key == null)
                {
                    return def;
                }

                object value = key.GetValue(keyWord, def);

                try
                {
                    // Nếu kiểu cần đọc là Enum
                    if (typeof(T).IsEnum && value is string enumString)
                    {
                        return (T)Enum.Parse(typeof(T), enumString);
                    }

                    // Nếu kiểu là DateTime và đang lưu chuỗi ISO
                    if (typeof(T) == typeof(DateTime) && value is string timeStr)
                    {
                        if (DateTime.TryParseExact(timeStr, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime dt))
                        {
                            return (T)(object)dt;
                        }
                        return def;
                    }

                    // Nếu là double và lưu dưới dạng chuỗi
                    if (typeof(T) == typeof(double) && value is string doubleStr)
                    {
                        if (double.TryParse(doubleStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double dbl))
                        {
                            return (T)(object)dbl;
                        }
                        return def;
                    }

                    return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                }
                catch
                {
                    return def;
                }
            }
        }

        // Generic: Lưu giá trị vào Registry
        public bool SetValue<T>(string keyWord, T value)
        {
            if (string.IsNullOrWhiteSpace(SubKey)) { return false; }
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(SubKey))
            {
                object toSave;
                if (value == null)
                {
                    toSave = "";
                }
                else if (value is DateTime dt)
                {
                    toSave = dt.ToString("o", CultureInfo.InvariantCulture); // ISO format
                }
                else if (value is double d)
                {
                    toSave = d.ToString(CultureInfo.InvariantCulture);
                }
                else if (value is Enum)
                {
                    toSave = value.ToString();
                }
                else
                {
                    toSave = value;
                }
                key.SetValue(keyWord, toSave);
                return true;
            }
        }

        // Xoá một key
        public bool DeleteValue(string keyWord)
        {
            if (string.IsNullOrWhiteSpace(SubKey)) { return false; }
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(SubKey, writable: true))
            {
                key?.DeleteValue(keyWord, throwOnMissingValue: false);
                return true;
            }
        }
    }
}
