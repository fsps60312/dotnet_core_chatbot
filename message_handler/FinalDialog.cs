using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace message_handler
{
    [Serializable]
    class FinalDialog:DialogEntry
    {
        public static FinalDialog Read()
        {
            if (!File.Exists("final_dialog.txt")) return new FinalDialog();
            Stream stream = new FileStream("final_dialog.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
            IFormatter formatter = new BinaryFormatter();
            var dialog_content = (FinalDialog)formatter.Deserialize(stream);
            stream.Close();
            return dialog_content;
        }
        void Write()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("final_dialog.txt", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();
        }
        public override void Run()
        {
            var messageText = Minimize(sender_msg);
            var messageRepeatCount = GetRepeatCount(Program.SenderPsid, messageText);
            //SendMsg($"repeat={messageRepeatCount}");
            if (messageText == Minimize("我要說甚麼"))
            {
                var commands = Constants.Commands.ListCommands();
                SendMsg($"你可以說說看：{RandItem(commands)}");
            }
            else if (messageText == Minimize(Constants.Commands.C1))
            {
                if (ganTalkLeaderBoard == null) ganTalkLeaderBoard = new GanTalkLeaderBoard();
                var content = string.Join("\n", ganTalkLeaderBoard.GetBoard().Select(v => $"{v.Item2}人說了：{v.Item1}"));
                if (content == "") content = "目前沒有資料TwT";
                SendMsg("\\幹話排行榜/ <(_ _)>\n2人以上才會上榜哦！\n" + content);
            }
            else if (messageText == Minimize(Constants.Commands.C2))
            {
                switch (messageRepeatCount)
                {
                    case 0: SendMsg("不好說，這真的不好說"); break;
                    case 1: SendMsg("走遠了......"); break;
                    case 2: SendMsg("剩下的就不要再問了"); break;
                    case 3: SendMsg("就跟你說不要再問了！"); break;
                    case 4: SendMsg("就說不要再問了你是沒聽到嗎？"); break;
                    case 5: SendMsg("好啦好啦，我說我說"); break;
                    case 6: SendMsg("好啦我真的要說了，但是要幫我保密哦"); break;
                    case 7: SendMsg("真的要幫我保密哦！(勾小拇指"); break;
                    default:
                        {
                            switch ((messageRepeatCount - 8) % 10)
                            {
                                case 0: SendMsg("就是呢......"); break;
                                case 1: SendMsg("我覺得......"); break;
                                case 2: SendMsg("那個Spec......"); break;
                                case 3: SendMsg("應該要一開始就寫清楚，而且......"); break;
                                case 4: SendMsg("不要一直改啦，這樣......"); break;
                                case 5: SendMsg("壞透了，真的壞透了！><"); break;
                                case 6: SendMsg("生氣氣啦><"); break;
                                case 7: SendMsg("沒了，你還要我說甚麼？"); break;
                                case 8: SendMsg("好啦其實助教人也滿厲害的，甚麼問題都可以很快回答得出來～"); break;
                                case 9: SendMsg("而且作業也是很好玩、可以學到很多東西！只是呢......"); break;
                                default: Console.WriteLine($"messageRepeatCount: {messageRepeatCount}"); break;
                            }
                        }
                        break;
                }
            }
            else if (messageText == Minimize(Constants.Commands.C3))
            {
                SendMsg($"↓我知道你的資訊有這麼多↓\nId: {Program.SenderPsid}\n" + $"Name: {"TODO"}\n" + $"Properties: {"TODO"}");
            }
            else if (messageText == Minimize(Constants.Commands.C4))
            {
                switch (messageRepeatCount)
                {
                    case 0: SendMsg("這是秘密>///<"); break;
                    case 1: SendMsg("不要再問了，這是秘密！"); break;
                    case 2: SendMsg("就說這是秘密了，再問下去我崩潰給你看哦><"); break;
                    case 3: throw new NotImplementedException();
                }
            }
            else
            {
                //await context.PostAsync("\u4f60\u8aaa\u4e86\u300c"/*你說了「*/ + message.Text + "\u300d"/*」*/);
                string msg = sender_msg;
                if (ganTalkLeaderBoard == null) ganTalkLeaderBoard = new GanTalkLeaderBoard();
                ganTalkLeaderBoard.Update(Program.SenderPsid, msg);
                switch (RandInt(0, 5))
                {
                    case 0: break;
                    case 1: msg = "你說了「" + msg + "」"; break;
                    case 2: msg = "好啦，" + msg; break;
                    case 3: msg = msg + " XDD"; break;
                    case 4: msg = msg + " www"; break;
                    case 5: msg = msg + " ^_^"; break;
                }
                SendMsg(msg);
            }
            SetLastUserMessage(Program.SenderPsid, messageText);
            //SendMsg($"repeat={GetRepeatCount(Program.SenderPsid, messageText)}");
            Write();
            EndDialog(Program.NextDialog);
        }
        [Serializable]
        class GanTalkLeaderBoard
        {
            public const int BoardSize = 10;
            Dictionary<string, HashSet<string>> data = new Dictionary<string, HashSet<string>>();
            List<Tuple<int, string>> board = new List<Tuple<int, string>>();
            private void UpdateBoard(string msg, int cnt)
            {
                if (cnt <= 1) return;
                bool found = false;
                for (int i = 0; i < board.Count; i++) if (board[i].Item2 == msg)
                    {
                        board[i] = new Tuple<int, string>(-cnt, msg);
                        found = true;
                        break;
                    }
                if (!found) board.Add(new Tuple<int, string>(-cnt, msg));
                board.Sort();
                if (board.Count > BoardSize) board.RemoveRange(BoardSize, board.Count - BoardSize);
            }
            public void Update(string userId, string msg)
            {
                if (!data.ContainsKey(msg)) data.Add(msg, new HashSet<string>());
                if (data[msg].Add(userId)) UpdateBoard(msg, data[msg].Count);
            }
            public List<Tuple<string, int>> GetBoard()
            {
                return board.Select(v => new Tuple<string, int>(v.Item2, -v.Item1)).ToList();
            }
        }
        // Azure page: https://portal.azure.com/#blade/WebsitesExtension/BotsIFrameBlade/id/%2Fsubscriptions%2Fed3b27fa-21db-4e94-8061-2d654c6b87d5%2FresourceGroups%2Ffsps60312botservicetest%2Fproviders%2FMicrosoft.Web%2Fsites%2Ffsps60312botservicetest
        // Unicode convert: https://www.ifreesite.com/unicode-ascii-ansi.htm
        [Serializable]
        class LastUserMessageData
        {
            public string message;
            public int repeat;
        }
        Dictionary<string, LastUserMessageData> LastUserMessage = new Dictionary<string, LastUserMessageData>();
        GanTalkLeaderBoard ganTalkLeaderBoard = new GanTalkLeaderBoard();
        int GetRepeatCount(string userId, string message)
        {
            var lastUserMessageData = GetLastUserMessage(userId);
            if (lastUserMessageData == null || lastUserMessageData.message != message) return 0;
            else return lastUserMessageData.repeat;
        }
        LastUserMessageData GetLastUserMessage(string userId)
        {
            if (LastUserMessage.ContainsKey(userId)) return LastUserMessage[userId];
            return null;
        }
        void SetLastUserMessage(string userId, string message)
        {
            if (message == null) return;
            if (message.Length > 100) message = message.Remove(100);
            if (!LastUserMessage.ContainsKey(userId)) LastUserMessage.Add(userId, new LastUserMessageData { message = message, repeat = 1 });
            else if (LastUserMessage[userId].message != message) LastUserMessage[userId] = new LastUserMessageData { message = message, repeat = 1 };
            else LastUserMessage[userId].repeat++;
        }
    }
}
