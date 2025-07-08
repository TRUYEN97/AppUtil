using System;
using System.Threading.Tasks;
using AppUtil.Communicate.Interface;
using AppUtil.Timer.Interfcae;

namespace AppUtil.Communicate
{
    public abstract class BaseInOutStream: IDisposable, IWriter, IReceiver
    {

        protected IReceiver OutPutReader;
        protected IWriter InputWriter;

        protected abstract void Close();

        public virtual bool Write(string mess)
        {
            return InputWriter?.Write(mess) == true;
        }

        public virtual bool WriteLine(string mess)
        {
            return InputWriter?.WriteLine(mess) == true;
        }

        public virtual async Task<bool> WriteAsync(string mess)
        {
            return await InputWriter?.WriteAsync(mess);
        }

        public virtual async Task<bool> WriteLineAsync(string mess)
        {
            return await InputWriter?.WriteLineAsync(mess);
        }

        public virtual string ReadLine()
        {
            return this.OutPutReader?.ReadLine();
        }

        public virtual string ReadUntil(string keyword)
        {
            return this.OutPutReader?.ReadUntil(keyword);
        }

        public virtual string ReadUntil(string keyword, IStopwatch timeOut)
        {
            return OutPutReader?.ReadUntil(keyword, timeOut);
        }

        public virtual string ReadUntil(string keyword, IStopwatch timeOut, IStopwatch timeWait)
        {
            return this.OutPutReader?.ReadUntil(keyword, timeOut, timeWait);
        }

        public virtual string ReadToEnd()
        {
            return this.OutPutReader?.ReadToEnd();
        }

        public virtual Task<string> ReadLineAsync()
        {
            return this.OutPutReader?.ReadLineAsync();
        }

        public virtual Task<string> ReadUntilAsync(string keyword)
        {
            return this.OutPutReader?.ReadUntilAsync(keyword);
        }

        public virtual Task<string> ReadUntilAsync(string keyword, IStopwatch timeOut)
        {
            return this.OutPutReader?.ReadUntilAsync(keyword, timeOut);
        }

        public virtual Task<string> ReadUntilAsync(string keyword, IStopwatch timeOut, IStopwatch timeWait)
        {
            return this.OutPutReader?.ReadUntilAsync(keyword, timeOut, timeWait);
        }

        public virtual Task<string> ReadToEndAsync()
        {
            return this.OutPutReader?.ReadToEndAsync();
        }
        public void Dispose()
        {
            try
            {
                Close();
            }
            finally
            {
                this.OutPutReader?.Dispose();
                this.InputWriter?.Dispose();
            }
        }
    }
}
