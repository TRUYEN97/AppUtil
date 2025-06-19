using System.IO;
using AppUtil.Service;

namespace AppUtil.ErrorCode
{
    public class ErrorCodeMapperConfig
    {
        private string _station;
        private string _product;
        private string _remoteDir;

        public ErrorCodeMapperConfig()
        {
            SftpConfig = new SftpConfig();
            RemoteDir = null;
            Product = null;
            Station = null;
        }
        public SftpConfig _sftpConfig;
        public SftpConfig SftpConfig { get => _sftpConfig; set { if (value != null) _sftpConfig = value; } }

        public string RemoteDir { get => _remoteDir; set => _remoteDir = string.IsNullOrWhiteSpace(value) ? "ErrorCode" : value.Trim().ToUpper(); }
        public string Product { get => _product; set => _product = string.IsNullOrWhiteSpace(value) ? "PRODUCT" : value.Trim().ToUpper(); }
        public string Station { get => _station; set => _station = string.IsNullOrWhiteSpace(value) ? "STATION" : value.Trim().ToUpper(); }
        public string RemoteNewFilePath => Path.Combine(RemoteDir, Product, Station, "newErrorCodes.csv");
        public string RemoteFilePath => Path.Combine(RemoteDir, Product, Station, "ErrorCodes.csv");

        public string LocalFilePath { get; internal set; }
    }
}
