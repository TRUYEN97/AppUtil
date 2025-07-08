
using AppUtil.Communicate.InOutStream;
using System;
using System.Diagnostics;

namespace AppUtil.Communicate
{
    public abstract class BaseProcess : BaseInOutStream
    {
        private readonly Process _process;
        protected BaseProcess(string fileName, string command, Action<string, bool> outputDataReceived)
        {
            _process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = fileName ?? "",
                    Arguments = command ?? "",
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };
            _process.Start();
            var dynamicTextStream = new DynamicTextReader();
            if (outputDataReceived != null)
            {
                _process.OutputDataReceived += (sender, args) => { if (args.Data != null) outputDataReceived?.Invoke(args.Data, true); };
                _process.ErrorDataReceived += (sender, args) => { if (args.Data != null) outputDataReceived?.Invoke(args.Data, false); };
                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();
                OutPutReader = null;
            }
            else
            {
                _process.OutputDataReceived += (sender, args) => { if (args.Data != null) dynamicTextStream?.AddLine(args.Data); else dynamicTextStream?.Complete(); };
                _process.ErrorDataReceived += (sender, args) => { if (args.Data != null) dynamicTextStream?.AddLine(args.Data); else dynamicTextStream?.Complete(); };
                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();
                OutPutReader = dynamicTextStream;
            }
            InputWriter = new StringWriter(_process.StandardInput);
        }

        public void WaitForExit()
        {
            _process?.WaitForExit();
        }

        public void Complete()
        {
            if (OutPutReader is DynamicTextReader reader)
            {
                reader.Complete();
            }
        }

        public virtual bool HasExited()
        {
            try
            {
                return _process == null || _process.HasExited;
            }
            catch (Exception)
            {
                return true;
            }
        }

        protected override void Close()
        {
            _process?.CancelErrorRead();
            _process?.CancelOutputRead();
            _process?.Close();
            _process?.Dispose();
        }
    }
}
