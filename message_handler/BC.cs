using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace message_handler
{
    class BC:DialogNode
    {
        public override void Run()
        {
            if (sender_msg.ToLower().StartsWith("bc"))
            {
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/su",
                        Arguments = "- test -c bc",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };
                process.Start();
                process.StandardInput.WriteLine(sender_msg.Substring(3));
                process.StandardInput.Close();
                DateTime start_time = DateTime.Now;
                while (true)
                {
                    System.Threading.Thread.Sleep(100);
                    if ((DateTime.Now - start_time).TotalSeconds > 3)
                    {
                        //SendMsg($"{process.Id} {process.SessionId}");
                        process.Kill(true);
                        SendMsg("超過3秒囉，卡");
                        EndDialog(Program.NextDialog);
                    }
                    if (process.HasExited)
                    {
                        string res = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
                        SendMsg(string.IsNullOrWhiteSpace(res) ? "Oops, no output." : res);
                        EndDialog(Program.NextDialog);
                    }
                }
            }
        }
    }
}
