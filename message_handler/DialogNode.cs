using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace message_handler
{
    [Serializable]
    abstract class DialogNode
    {
        static Random rand = new Random();
        protected int RandInt(int l,int r) { return rand.Next(l, r + 1); }
        protected T RandItem<T>(IEnumerable<T> list) { return list.ElementAt(RandInt(0, list.Count() - 1)); }
        protected string sender_msg { get { return Program.SenderMsg; } }
        protected void SendMsg(string msg) { Program.SendMsg(msg); }
        protected void SendButtons(string title, string subtitle, string[] buttons) { Program.SendButtons(title, subtitle, buttons); }
        protected void SendImage(string url) { Program.SendImage(url); }
        protected void EndDialog(DialogNode dialog_node) { Program.WriteDialogNode(dialog_node); Environment.Exit(0); }
        protected void Sleep(int miliseconds) { System.Threading.Thread.Sleep(miliseconds); }
        protected void Bug(string msg) { SendMsg("error:\n" + msg);EndDialog(new DialogEntry()); }
        bool IsChinese(char c) { return 0x4E00 <= c && c <= 0x9FFF; }
        protected string Minimize(string s)
        {
            s = new string(s.Where(c => char.IsLetterOrDigit(c) || IsChinese(c)).ToArray()).ToLower();
            s = s.Replace("什麼", "甚麼").Replace("神麼", "甚麼").Replace("啥", "甚麼").Replace('喔', '哦');
            return s;
        }
        public virtual void Run()
        {
            if (sender_msg == null)
            {
                SendMsg("吼～都這樣，不說點話嗎？><");
                EndDialog(this);
            }
            if (sender_msg.Length > 1000)
            {
                SendMsg("???");
                SendMsg($"傳了{sender_msg.Length}個字是在哈囉???");
                EndDialog(this);
            }
        }
    }
}
