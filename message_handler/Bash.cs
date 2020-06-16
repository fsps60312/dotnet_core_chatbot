using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace message_handler
{
    class Bash : DialogNode
    {
        public static string Cmd(string cmd, string input)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/ssh",
                    Arguments = "restricted@localhost -p 60313 -- " + cmd,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.StandardInput.WriteLine(input);
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
                    return string.IsNullOrWhiteSpace(res) ? "👻" : res;
                }
            }
        }
        public override void Run()
        {
            foreach (var (prefixes, cmd) in new[]
            {
                (new[]{"bash", "sh"},"bash"),
                (new[]{"python", "py"},"python3"),
                (new[]{"幫我算", "幫算", "bc"},"\"export BC_LINE_LENGTH=0 && bc -l\""),
                (new[]{"factor"},"factor"),
                (new[]{ "cowsay"},"\"cowsay -f $(shuf -n 1 <(cowsay -l | tail +2 | sed 's/ /\\n/g'))\""),
                (new[]{ "tac"},"tac"),
                (new[]{"figlet"},"figlet"),
                (new[]{"toilet"},"toilet"),
                (new[]{"rev"},"rev"),
                (new[]{"moo"},"\"apt moo\""),
                (new[]{"rig"},"rig")
            })
            {
                string input = null;
                foreach (string prefix in prefixes)
                {
                    if (sender_msg.ToLower().StartsWith(prefix.ToLower()))
                    {
                        input = sender_msg.Substring(prefix.Length).Trim();
                        break;
                    }
                }
                if (input != null)
                {
                    string res = Cmd(cmd, input);
                    SendMsg(res);
                    EndDialog(Program.NextDialog);
                }
            }
        }
    }
}
