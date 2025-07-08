using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppUtil.Communicate.Interface;
using AppUtil.Timer.Interfcae;

namespace AppUtil.Communicate.InOutStream
{
    public class DynamicTextReader : IReceiver
    {
        protected readonly BlockingCollection<string> _collection;
        public DynamicTextReader()
        {
            _collection = new BlockingCollection<string>();
        }

        public void AddLine(string line)
        {
            _collection.Add(line);
        }

        public string ReadLine()
        {
            try
            {
                return _collection.Take();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string ReadToEnd()
        {
            StringBuilder sb = new StringBuilder();
            string line;
            while (!_collection.IsCompleted)
            {
                if ((line = ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
            }
            return sb.ToString();
        }

        public string ReadUntil(string keyword)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string line;
            while ((line = ReadLine()) != null)
            {
                stringBuilder.AppendLine(line);
                if (line.Contains(keyword))
                {
                    break;
                }
            }
            return stringBuilder.ToString();
        }

        public string ReadUntil(string keyword, IStopwatch timeOut) => ReadUntil(keyword, timeOut, null);

        public string ReadUntil(string keyword, IStopwatch timeOut, IStopwatch timeWait)
        {
            StringBuilder stringBuilder = new StringBuilder();
            timeOut?.Reset();
            timeWait?.Reset();
            string line;
            while (timeOut == null || timeOut.IsOntime())
            {
                line = ReadLine();
                if (line == null)
                {
                    Thread.Sleep(100);
                    continue;
                }
                stringBuilder.AppendLine(line);
                if (timeWait?.IsOutOfTime() == true || line.Contains(keyword))
                {
                    break;
                }
                timeWait?.Reset();
            }
            return stringBuilder.ToString();
        }

        public async Task<string> ReadLineAsync()
        {
            return await Task.Run(() => { return ReadLine(); });
        }

        public async Task<string> ReadUntilAsync(string keyword)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string line;
            while ((line = await ReadLineAsync()) != null)
            {
                stringBuilder.AppendLine(line);
                if (line.Contains(keyword))
                {
                    break;
                }
            }
            return stringBuilder.ToString();
        }

        public async Task<string> ReadUntilAsync(string keyword, IStopwatch timeOut) => await ReadUntilAsync(keyword, timeOut, null);

        public async Task<string> ReadUntilAsync(string keyword, IStopwatch timeOut, IStopwatch timeWait)
        {
            StringBuilder stringBuilder = new StringBuilder();
            timeOut?.Reset();
            timeWait?.Reset();
            string line;
            while (timeOut == null || timeOut.IsOntime())
            {
                line = await ReadLineAsync();
                if (line == null)
                {
                    Thread.Sleep(100);
                    continue;
                }
                stringBuilder.AppendLine(line);
                if (timeWait?.IsOutOfTime() == true || line.Contains(keyword))
                {
                    break;
                }
                timeWait?.Reset();
            }
            return stringBuilder.ToString();
        }

        public async Task<string> ReadToEndAsync() => await Task.Run(() => { return ReadToEnd(); });

        public void Complete()
        {
            _collection?.CompleteAdding();
        }

        public void Dispose()
        {
            _collection?.CompleteAdding();
            _collection?.Dispose();
        }
    }
}
