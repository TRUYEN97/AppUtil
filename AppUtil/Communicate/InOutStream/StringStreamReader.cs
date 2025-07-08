using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppUtil.Communicate.Interface;
using AppUtil.Timer.Interfcae;

namespace AppUtil.Communicate.InOutStream
{
    internal class StringStreamReader : IReceiver
    {
        protected TextReader reader;
        public StringStreamReader(TextReader reader)
        {
            this.reader = TextReader.Synchronized(reader);
        }
        public string ReadLine() => reader.ReadLine();
        public string ReadToEnd() => reader.ReadToEnd();

        public string ReadUntil(string keyword, IStopwatch timeOut, IStopwatch timeWait)
        {
            StringBuilder stringBuilder = new StringBuilder();
            timeOut?.Reset();
            timeWait?.Reset();
            string line;
            while (timeOut == null || timeOut.IsOntime())
            {
                line = reader.ReadLine();
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

        public string ReadUntil(string keyword)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string line;
            while ((line = reader.ReadLine()) != null)
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

        public async Task<string> ReadLineAsync() => await reader.ReadLineAsync();

        public async Task<string> ReadUntilAsync(string keyword)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
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
                line = await reader.ReadLineAsync();
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

        public async Task<string> ReadToEndAsync() => await reader.ReadToEndAsync();

        public void Dispose()
        {
            this.reader?.Dispose();
        }
    }
}
