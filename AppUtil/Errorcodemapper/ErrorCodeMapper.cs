using System;
using System.IO;
using AppUtil.Errorcodemapper;
using AppUtil.Service;

namespace AppUtil.ErrorCodeMapper
{
    public class ErrorCodeMapper
    {
        private readonly static Lazy<ErrorCodeMapper> instance = new Lazy<ErrorCodeMapper>(() => new ErrorCodeMapper());
        private readonly ErrorCodeMapperConfig config;
        private readonly ErrorCodeModel model;
        private readonly ErrorCodeAnalysis errorCodeAnalysis;
        private ErrorCodeMapper()
        {
            config = new ErrorCodeMapperConfig();
            model = new ErrorCodeModel();
            errorCodeAnalysis = new ErrorCodeAnalysis(model, 6);
        }
        private static ErrorCodeMapper Instance => instance.Value;
        public static SftpConfig SftpConfig { get => Instance.config.SftpConfig; set => Instance.config.SftpConfig = value; }
        public static string RemoteDir { get => Instance.config.RemoteDir; set => Instance.config.RemoteDir = value; }
        public static string Product { get => Instance.config.Product; set => Instance.config.Product = value; }
        public static string Station { get => Instance.config.Station; set => Instance.config.Station = value; }

        public static bool LoadErrorcodeFromServer()
        {
            using (var sftp = new MySftp(Instance.config.SftpConfig))
            {
                if (!sftp.Connect())
                {
                    return false;
                }
                if (!sftp.TryReadAllLines(Instance.config.RemoteFilePath, out string[] lines))
                {
                    return false;
                }
                return Instance.AddErrorCode(lines);
            }
        }
        public static bool LoadErrorcodeFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return false;
            string[] lines = File.ReadAllLines(filePath);
            return Instance.AddErrorCode(lines);
        }

        public static string GetErrorcode(string logText)
        {
            if (TryGetErrorcode(logText, out string functionName, out string errorcode))
            {
                return errorcode;
            }
            string newErrorcode = Instance.errorCodeAnalysis.CreateNewErrorcode(functionName);
            Instance.model.Set(functionName, newErrorcode);
            using (var sftp = new MySftp(Instance.config.SftpConfig))
            {
                if (sftp.Connect())
                {
                    string remotePath = Instance.config.RemoteNewFilePath;
                    string line = $"{functionName};{newErrorcode}".ToUpper();
                    sftp.AppendLine(remotePath, line);
                }
            }
            return newErrorcode;
        }

        public static bool TryGetErrorcode(string logText, out string functionName, out string errorcode)
        {
            return Instance.errorCodeAnalysis.TryGetErrorCode(logText, out functionName, out errorcode);
        }

        private bool AddErrorCode(string[] lines)
        {
            if (lines == null) return false;
            foreach (var line in lines)
            {
                string[] elems = line.Split(new char[] { ',', ';' });
                if (elems.Length >= 2)
                {
                    model.Set(elems[0], elems[1]);
                }
            }
            return true;
        }
    }
}
