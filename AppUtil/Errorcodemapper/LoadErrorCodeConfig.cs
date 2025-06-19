using System;
using System.IO;
using AppUtil.ErrorCodeMapper;
using Newtonsoft.Json;

namespace AppUtil.Errorcodemapper
{
    public class LoadErrorCodeConfig
    {
        private static Lazy<LoadErrorCodeConfig> instance = new Lazy<LoadErrorCodeConfig>(() => new LoadErrorCodeConfig());
        private ErrorCodeMapperConfig _config;
        private static readonly string cfPath = "./ErrorCodeConfig.json";

        public static LoadErrorCodeConfig Instance { get { return instance.Value; } }
        public ErrorCodeMapperConfig Config => _config;

        private LoadErrorCodeConfig() {
            if (!Init(cfPath))
            {
                _config = new ErrorCodeMapperConfig();
            }
            UpdateCf();
        }

        private bool Init(string cfPath)
        {
            try
            {
                if (!File.Exists(cfPath))
                {
                    return false;
                }
                string configText = File.ReadAllText(cfPath);
                _config = JsonConvert.DeserializeObject<ErrorCodeMapperConfig>(configText);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool UpdateConfig()
        {
            return Instance.UpdateCf();
        }

        public bool UpdateCf()
        {
            try
            {
                string cfJson = JsonConvert.SerializeObject(_config, Formatting.Indented);
                File.WriteAllText(cfPath, cfJson);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
