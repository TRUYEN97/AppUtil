using System;
using System.IO;
using AppUtil.Service;
using Renci.SshNet.Messages;

namespace AppUtil.ErrorCode
{
    public class ErrorCodeMapper
    {
        private readonly static Lazy<ErrorCodeMapper> instance = new Lazy<ErrorCodeMapper>(() => new ErrorCodeMapper());
        private readonly ErrorCodeModel model;
        private readonly SpecialErrorCode specialError;
        private readonly ErrorCodeAnalysis errorCodeAnalysis;
        private ErrorCodeMapperConfig _config;
        private ErrorCodeMapper()
        {
            int maxLength = 6;
            model = new ErrorCodeModel(maxLength);
            specialError = new SpecialErrorCode();
            errorCodeAnalysis = new ErrorCodeAnalysis(model, specialError, maxLength);
            Config = LoadErrorCodeConfig.Instance.Config;
        }
        public static ErrorCodeMapper Instance => instance.Value;
        public ErrorCodeMapperConfig Config { get { return _config; } set { if (value != null) _config = value; } }
        public SftpConfig SftpConfig { get => _config.SftpConfig; set => _config.SftpConfig = value; }
        public string RemoteDir { get => _config.RemoteDir; set => _config.RemoteDir = value; }
        public string Product { get => _config.Product; set => _config.Product = value; }
        public string Station { get => _config.Station; set => _config.Station = value; }
        public SpecialErrorCode SpecialErrorCode => specialError;
        public string RemoteNewFilePath => Path.Combine(RemoteDir, Product, Station, "newErrorCodes.csv");
        public string RemoteFilePath => Path.Combine(RemoteDir, Product, Station, "ErrorCodes.csv");

        public bool LoadErrorcodeFromServer()
        {
            using (var sftp = new MySftp(Instance._config.SftpConfig))
            {
                if (!sftp.Connect())
                {
                    return false;
                }
                if (!sftp.TryReadAllLines(RemoteFilePath, out string[] lines))
                {
                    return false;
                }
                return AddErrorCode(lines);
            }
        }
        public bool LoadErrorcodeFromFile()
        {
            string filePath = Instance._config.LocalFilePath;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return false;
            }
            string[] lines = File.ReadAllLines(filePath);
            return AddErrorCode(lines);
        }
        public bool TryGetErrorcode(string logText, out string functionName, out string errorcode)
        {
            try
            {
                if (errorCodeAnalysis.TryGetErrorCode(logText, out functionName, out errorcode) && !string.IsNullOrWhiteSpace(errorcode))
                {
                    return true;
                }
                errorcode = errorCodeAnalysis.CreateNewErrorcode(functionName);
                UpdateNewErrorCode(functionName, errorcode);
                return !string.IsNullOrWhiteSpace(errorcode);
            }
            catch (Exception)
            {
                functionName = null;
                errorcode = null;
                return false;
            }
        }

        private void UpdateNewErrorCode(string functionName, string newErrorcode)
        {
            model.Set(functionName, newErrorcode);
            string line = $"{functionName};{newErrorcode}".ToUpper();
            UpdateToLocalFile(line);
            UpdateToSftpFile(line);
        }

        private void UpdateToSftpFile(string line)
        {
            using (var sftp = new MySftp(Instance._config.SftpConfig))
            {
                if (sftp.Connect())
                {
                    sftp.AppendLine(RemoteNewFilePath, line);
                }
            }
        }

        private void UpdateToLocalFile(string line)
        {
            string filePath = _config.LocalFilePath;
            if (!File.Exists(filePath))
            {
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            File.AppendAllText(filePath, line);
        }

        private bool AddErrorCode(string[] lines)
        {
            if (lines == null) return false;
            foreach (var line in lines)
            {
                string[] elems = line.Split(new char[] { ',', ';' });
                if (elems.Length >= 2)
                {
                    string funcName = elems[0];
                    string errorcode = elems[1];
                    model.Set(funcName, errorcode);
                }
            }
            return true;
        }
    }
}
