using System;
using System.Threading.Tasks;

namespace AppUtil.Communicate.Interface
{
    public interface IWriter: IDisposable
    {
        bool Write(string mess);
        bool WriteLine(string mess);
        Task<bool> WriteAsync(string mess);
        Task<bool> WriteLineAsync(string mess);
    }
}
