using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Policy;
using System.Security.Permissions;
using System.Reflection;
using System.Runtime.Remoting;
using Newtonsoft.Json;
using IronPython;
using IronPython.Hosting;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace message_handler
{
    class Python:DialogNode
    {
        public static string RunCode(string python_code)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/fakechroot",
                    Arguments = "-- chroot /tmp2/b05902083/tmp /bin/bash",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.StandardInput.WriteLine("export PYTHONHOME=pythonhome");
            process.StandardInput.WriteLine("export PYTHONPATH=pythonhome");
            process.StandardInput.WriteLine("/bin/python3");
            process.StandardInput.WriteLine(python_code);
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
            string python_code = null;
            if (sender_msg.StartsWith("幫我算")) python_code = sender_msg.Substring(3).TrimStart();
            else if (sender_msg.StartsWith("幫算")) python_code = sender_msg.Substring(2).TrimStart();
            else if (sender_msg.ToLower().StartsWith("py")) python_code = sender_msg.Substring(2).TrimStart();
            else if (sender_msg.ToLower().StartsWith("python")) python_code = sender_msg.Substring(6).TrimStart();
            if (!string.IsNullOrWhiteSpace(python_code))
            {
                string res = RunCode(python_code);
                SendMsg(res);
                EndDialog(Program.NextDialog);
            }
        }
    }
}
