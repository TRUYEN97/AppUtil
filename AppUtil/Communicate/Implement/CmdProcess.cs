using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AppUtil.Communicate.Implement
{
    public class CmdProcess : BaseProcess
    {
        public CmdProcess(bool keepSeason, string command, Action<string,bool> outputDataReceived) :
            base("cmd.exe", $"{(keepSeason ? "/k" : "/c")} {(string.IsNullOrWhiteSpace(command) ? "" : ($"{command} 2>&1"))}",
                outputDataReceived){ }
        public CmdProcess(bool keepSeason, string command) : this(keepSeason, command, null) { }
        public CmdProcess(bool keepSeason) : this(keepSeason, "") { }
        public CmdProcess() : this(true) { }

        public override bool Write(string mess)
        {
            try
            {
                base.Write($"{mess} 2>&1");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool WriteLine(string mess)
        {
            try
            {
                base.WriteLine($"{mess} 2>&1");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> WriteAsync(string mess)
        {
            try
            {
                await base.WriteAsync($"{mess} 2>&1");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> WriteLineAsync(string mess)
        {
            try
            {
                await base.WriteAsync($"{mess} 2>&1");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
