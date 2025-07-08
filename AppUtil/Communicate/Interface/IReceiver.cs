
using System;
using System.Threading.Tasks;
using AppUtil.Timer.Interfcae;

namespace AppUtil.Communicate.Interface
{
    public interface IReceiver: IDisposable
    {
        string ReadLine();
        string ReadUntil(string keyword);
        string ReadUntil(string keyword, IStopwatch timeOut);
        string ReadUntil(string keyword, IStopwatch timeOut, IStopwatch timeWait);
        string ReadToEnd();
        Task<string> ReadLineAsync();
        Task<string> ReadUntilAsync(string keyword);
        Task<string> ReadUntilAsync(string keyword, IStopwatch timeOut);
        Task<string> ReadUntilAsync(string keyword, IStopwatch timeOut, IStopwatch timeWait);
        Task<string> ReadToEndAsync();
    }
}
