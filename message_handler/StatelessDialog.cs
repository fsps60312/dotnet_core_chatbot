using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace message_handler
{
    class StatelessDialog : DialogNode
    {
        public override void Run()
        {
            string minimized = Minimize(sender_msg);
            var candidates = gossip_data.Where(p => minimized == Minimize(p.Item1)).ToArray();
            if (candidates.Length != 0) { SendMsg(RandItem(candidates).Item2); EndDialog(Program.NextDialog); }
            if (sender_msg.Trim().Split('\n').Last().Trim() == "選一個")
            {
                var opts = sender_msg.Trim().Split('\n').ToList();
                opts.RemoveAt(opts.Count - 1);
                if (opts.Count == 0)
                {
                    SendMsg("選項呢？");
                    Sleep(1000);
                    SendMsg("範例：");
                    Sleep(2000);
                    SendMsg("黑桃\n紅心\n方塊\n梅花\n選一個");
                }
                else
                {
                    SendMsg("我選：");
                    Sleep(1000);
                    SendMsg(RandItem(opts));
                }
                EndDialog(Program.NextDialog);
            }
            if ((sender_msg.Trim().StartsWith("說") || sender_msg.Trim().StartsWith("講")) && sender_msg.Trim().Length > 1) { SendMsg(sender_msg.Trim().Substring(1)); EndDialog(Program.NextDialog); }
            if (sender_msg.Trim().All(c => c == '.')) { SendMsg(Bash.Cmd("bash", "fortune $(fortune -f 2>&1 | tail +2 | sed 's/^[ 0-9.]*% //g' | grep -v 'chinese\\|tang300\\|song100') | sed 's/\\x1b\\[[0-9;]*m//g'")); EndDialog(Program.NextDialog); }
            if (sender_msg.Trim().All(c => c == '…')) { SendMsg(Bash.Cmd("bash", "fortune-zh | opencc | sed 's/\\x1b\\[[0-9;]*m//g'")); EndDialog(Program.NextDialog); }
            if (new[] { "剪刀", "石頭", "布" }.Contains(sender_msg.Trim()))
            {
                var s = new List<string> { "剪刀", "石頭", "布" };
                (int you, int me) = (s.IndexOf(sender_msg.Trim()), RandInt(0, 2));
                SendMsg($"我出 {s[me]}");
                Sleep(500);
                switch ((me - you + 3) % 3)
                {
                    case 0: SendMsg("布！"); break;
                    case 1: SendMsg("我贏了！"); break;
                    case 2: SendMsg("嗚嗚"); break;
                    default: throw new Exception();
                }
                EndDialog(Program.NextDialog);
            }
            if(sender_msg.Trim()=="笑一個")
            {
                SendMsg(Bash.Cmd("bash", "curl facemood.grtimed.com 2>/dev/null | grep -A 2 '<div class=\"logo\">' | tail -1 | sed 's/<\\/div>//g' | sed -e 's/^[[:space:]]*//g' -e 's/[[:space:]]*$//g'"));
                EndDialog(Program.NextDialog);
            }
        }
        (string, string)[] gossip_data = new (string, string)[]//input must be lower case
        {
            ( "code風景區", "很棒的名字，不覺得嗎？XD\n然後，我的英文名字是「code scenic」哦，Google看看！\n總之，像欣賞風景一樣快樂的探索程式之美吧！"),
            //("Code風景區" ,"很棒的名字，不覺得嗎？XD\n然後，我的英文名字是「Code Scenic」哦，Google看看！\n總之，像欣賞風景一樣快樂的探索程式之美吧！"),
            ( "傳一則貼文的網址(?)","吼～不是真的要你說這句話啦！\n是你要傳一則貼文的網址給我～><" ),
            ("借我錢","我沒錢><" ),
            ("不要","好吧，你壞壞 :p" ),
            ("對","沒錯，就是這樣！😎" ),
            ("bot","嘿，什麼事？ ^_^" ),
            ("qq","好啦，乖（拍拍" ),
            ("電","謝謝"),
            ("你好電","你更電"),
            ("你好電哦","你更電哦"),
            ("雷","你才雷"),
            ("你好雷","你才雷，你全家都雷"),
            ("你好雷哦","你也很雷，別五十步笑百步www" ),
            ("hi","恩？" ),
            ("在嗎","不在～（不知道你要幹嘛怎麼決定我要不要在呢？XD）" ),
            ("ㄎㄎ","蝦？？\n不然我ㄎ回去好了\nㄎㄎ" ),
            ("掰掰","掰掰～歡迎隨時再傳訊息給我哦！>///<\n還是你只是說好玩的(?)" ),
            ("好吧","耶耶～～" ),
            ("小心回家不要被壞人抓走" ,"小心回家不要被洪水沖走"),
            ("所以你是誰","才不告訴你呢www" ),
            ("不好說","對呀，不好說(?)" ),
            ("你嗎","你猜呀～ ^_^" ),
            ("你","很棒 (y) (X)" ),
            ("好哦","\\(^o^)/（雖然不知道發生甚麼事XD）" ),
            ("不好說","真的不好說（咦？）" ),
            ("omg","喵(?)" ),
            ("這是自動回覆嗎","有可能是，也有可能不是(?)" ),
            ("github","https://github.com/fsps60312/dotnet_core_chatbot/tree/master/message_handler"),
            ("猜拳","好啊來！剪刀～石頭～布！")
        };
    }
}