using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Renci.SshNet.Sftp;
using Renci.SshNet;

namespace AppUtil.Service
{
    public class MySftp : IDisposable
    {
        private readonly SftpClient _client;
        public MySftp(string host, int port, string user, string password)
        {
            _client = new SftpClient(host, port, user, password);
            try
            {
                _client.Connect();
            }
            catch (Exception)
            {
            }
        }

        public MySftp(SftpConfig sftpConfig) : this(sftpConfig.Host, sftpConfig.Port, sftpConfig.User, sftpConfig.Password) { }

        public bool Connect()
        {
            try
            {
                if (_client.IsConnected)
                {
                    return true;
                }
                else
                {
                    _client.Connect();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool IsConnected { get { return _client.IsConnected; } }

        public string ReadAllText(string remotePath)
        {
            try
            {
                if (!Connect())
                {
                    return null;
                }
                if (!_client.Exists(remotePath))
                {
                    return null;
                }
                return _client.ReadAllText(remotePath);
            }
            catch (Exception)
            {
                return null;
            }

        }
        public string[] ReadAllLines(string remotePath)
        {
            try
            {
                if (!Connect())
                {
                    return null;
                }

                if (!_client.Exists(remotePath))
                {
                    return null;
                }
                return _client.ReadAllLines(remotePath);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public bool AppendAllText(string remotePath, string contents)
        {
            try
            {
                if (!Connect())
                {
                    return false;
                }
                if (!CreateDirectory(Path.GetDirectoryName(remotePath)))
                {
                    return false;
                }
                _client.AppendAllText(remotePath, contents);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AppendLine(string remotePath, string contents)
        {
            try
            {
                if (!Connect())
                {
                    return false;
                }
                if (!CreateDirectory(Path.GetDirectoryName(remotePath)))
                {
                    return false;
                }
                _client.AppendAllText(remotePath, $"{contents}\r\n");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool WriteAllText(string remotePath, string contents)
        {
            try
            {
                if (!Connect())
                {
                    return false;
                }
                if (!CreateDirectory(Path.GetDirectoryName(remotePath)))
                {
                    return false;
                }
                _client.WriteAllText(remotePath, contents);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public bool DownloadFile(string remotePath, string localPath)
        {
            int maxRetries = 3;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    if (!Connect())
                    {
                        return false;
                    }
                    if (!_client.Exists(remotePath))
                    {
                        return false;
                    }
                    string dir = Path.GetDirectoryName(localPath);
                    Directory.CreateDirectory(dir);
                    Task.Run(() =>
                   {
                       using (var fileStream = new FileStream(localPath, FileMode.Create))
                       {
                           _client.DownloadFile(remotePath, fileStream);
                       }
                   });
                    return true;
                }
                catch (Exception)
                {
                    if (attempt == maxRetries)
                        return false;
                    Task.Delay(10);
                }
            }
            return false;
        }

        public List<string> DownloadFolder(string remotePath, string localPath, bool isDowndloadAll = false)
        {
            try
            {
                if (!Connect())
                {
                    return null;
                }
                var fileNames = new List<string>();
                if (_client.Exists(remotePath))
                {
                    foreach (var file in _client.ListDirectory(remotePath))
                    {
                        if (isDowndloadAll && file.IsDirectory && !file.Name.StartsWith("."))
                        {
                            string subFolder = string.Format("{0}/{1}", localPath, file.Name);
                            fileNames.AddRange(DownloadFolder(file.FullName, subFolder, isDowndloadAll));
                        }
                        else if (!file.IsDirectory)
                        {
                            string subPath = file.FullName.Substring(remotePath.Length);
                            string filePath = string.Format("{0}{1}", localPath, subPath);
                            Directory.CreateDirectory(localPath);
                            Task.Run(() =>
                           {
                               using (var fileStreem = new FileStream(filePath, FileMode.Create))
                               {
                                   _client.DownloadFile(file.FullName, fileStreem);
                                   fileNames.Add(filePath);
                               }
                           });
                        }
                    }
                }
                return fileNames;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public bool UploadFile(string remotePath, string localPath)
        {
            int maxRetries = 3;
            if (!File.Exists(localPath))
            {
                return false;
            }
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    if (!Connect())
                    {
                        return false;
                    }
                    if (!CreateDirectory(Path.GetDirectoryName(remotePath)))
                    {
                        return false;
                    }
                    using (var fileStreem = new FileStream(localPath, FileMode.Open))
                    {
                        _client.UploadFile(fileStreem, remotePath);
                    }
                    return true;
                }
                catch (Exception)
                {
                    Task.Delay(10);
                }
            }
            return false;

        }
        public bool DeleteFile(string remotePath)
        {
            try
            {
                if (!Connect())
                {
                    return false;
                }
                if (_client.Exists(remotePath))
                {
                    _client.DeleteFile(remotePath);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CreateDirectory(string remotePath)
        {
            try
            {
                string[] parts = remotePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                string currentPath = "";

                if (remotePath.StartsWith("/"))
                {
                    currentPath = "/";
                }
                else if (remotePath.StartsWith("\\"))
                {
                    currentPath = "\\";
                }
                if (!Connect())
                {
                    return false;
                }
                foreach (var part in parts)
                {
                    currentPath = Path.Combine(currentPath, part).Replace('\\', '/');

                    if (!_client.Exists(currentPath))
                    {
                        _client.CreateDirectory(currentPath);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<string> ListDirectoryPath(string remotePath)
        {
            try
            {
                if (!Connect())
                {
                    return null;
                }
                List<string> listName = new List<string>();
                if (!_client.Exists(remotePath))
                {
                    return null;
                }
                foreach (var file in _client.ListDirectory(remotePath))
                {
                    if (file.IsDirectory && !file.Name.StartsWith("."))
                    {
                        listName.Add(file.FullName);
                    }
                }
                return listName;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<string> ListDirectoryName(string remotePath)
        {
            try
            {
                if (!Connect())
                {
                    return null;
                }
                List<string> listName = new List<string>();
                if (!_client.Exists(remotePath))
                {
                    return null;
                }
                foreach (var file in _client.ListDirectory(remotePath))
                {
                    if (file.IsDirectory && !file.Name.StartsWith("."))
                    {
                        listName.Add(file.Name);
                    }
                }
                return listName;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<string> ListFilePath(string remotePath, bool getAll)
        {
            try
            {
                if (!Connect())
                {
                    return null;
                }
                List<string> listName = new List<string>();
                if (!_client.Exists(remotePath))
                {
                    return null;
                }
                foreach (var file in _client.ListDirectory(remotePath))
                {
                    if (file.IsDirectory && getAll)
                    {
                        var temp = ListFilePath(remotePath, getAll);
                        if (temp != null)
                        {
                            listName.AddRange(temp);
                        }
                    }
                    else
                    {
                        listName.Add(file.FullName);
                    }
                }
                return listName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool DeleteFolder(string remotePath, bool deleteIfEmpty = false)
        {

            if (!Connect())
            {
                return false;
            }
            try
            {
                if (_client.Exists(remotePath))
                {
                    List<ISftpFile> list = new List<ISftpFile>(_client.ListDirectory(remotePath));
                    if (list.Count > 0 && !deleteIfEmpty)
                    {
                        foreach (var file in list)
                        {
                            if (!file.Name.StartsWith("."))
                            {
                                if (file.IsDirectory)
                                {
                                    DeleteFolder(file.FullName);
                                }
                                else
                                {
                                    _client.DeleteFile(file.FullName);
                                }
                            }
                        }
                    }
                    _client.DeleteDirectory(remotePath);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public void Dispose()
        {
            _client?.Dispose();
        }

        public bool Exists(string remotePath)
        {
            if (!Connect())
            {
                return false;
            }
            return _client.Exists(remotePath);
        }

        public bool TryReadAllLines(string remoteFilePath, out string[] lines)
        {
            lines = ReadAllLines(remoteFilePath);
            return lines == null || lines.Length == 0;
        }
    }
}
