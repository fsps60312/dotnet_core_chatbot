using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace message_handler
{
    class Bash:DialogNode
    {
        public static string RunCommand(string command)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/ssh",
                    Arguments = "restricted@localhost -p 60313 -- bash",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.StandardInput.WriteLine(command);
            process.StandardInput.Close();
            DateTime start_time = DateTime.Now;
            while (true)
            {
                System.Threading.Thread.Sleep(100);
                if ((DateTime.Now - start_time).TotalSeconds > 3)
                {
                    //SendMsg($"{process.Id} {process.SessionId}");
                    process.Kill(true);
                    return "超過3秒囉，卡";
                }
                if (process.HasExited)
                {
                    string res = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
                    return string.IsNullOrWhiteSpace(res) ? "Oops, no output." : res;
                }
            }
        }
        public override void Run()
        {
            if (sender_msg.ToLower().StartsWith("bash"))
            {
                string res = RunCommand(sender_msg.Substring(4).Trim());
                SendMsg(res);
                //res = RunCommand("killall -9 -u restricted");
                //Console.WriteLine(res);
                EndDialog(Program.NextDialog);
            }
        }
    }
}
