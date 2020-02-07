namespace message_handler
{
    class UrlReactor:DialogNode
    {
        public override void Run()
        {
            switch (sender_msg)
            {
                case "https://codingsimplifylife.blogspot.tw/":
                    Program.SendMsg("code風景區！！！歡迎常來逛逛～～～你會發現，寫程式就像欣賞風景一樣快樂哦！");
                    EndDialog(Program.NextDialog); return;
                case "https://codingsimplifylife.blogspot.tw/2016/04/c.html":
                    Program.SendMsg("給新手的C++教學！！！號稱網路上對新手最友善的C++教學，歡迎推薦給親朋好友，或提供改善建議哦！");
                    EndDialog(Program.NextDialog); return;
                default:
                    {
                        switch (GetPostId(sender_msg))
                        {
                            case null:
                                {
                                    if (sender_msg.StartsWith("http://") || sender_msg.StartsWith("https://"))
                                    {
                                        if (sender_msg.StartsWith("https://codingsimplifylife.blogspot."))
                                        {
                                            Program.SendMsg("這是「code風景區」的連結，不是「Code風景區」的連結哦XDD");
                                        }
                                        else
                                        {
                                            Program.SendMsg("偷偷告訴你，以後傳某些連結（連結=網址）給我（特別是Code風景區某些文章的連結）會有特殊反應哦！敬請期待！>///<\n還有，你傳的網址根本不是Code風景區的網址，拒收！><");
                                        }
                                        EndDialog(Program.NextDialog);
                                    }
                                }
                                return;
                            case "1995730270697235":
                                new P1995730270697235().Run();
                                return;
                            //case "2002954469974815":
                            //    P2002954469974815.Run(sender_msg);
                            //    Environment.Exit(0); return;
                            //case "2003744179895844":
                            //    P2003744179895844.Run(sender_msg);
                            //    Environment.Exit(0); return;
                            default:
                                Program.SendMsg("Oops......這篇文沒有彩蛋哦～試試看別篇吧XD");
                                EndDialog(Program.NextDialog); return;
                        }
                    }
            }
        }
        private static string GetPostId(string url)
        {
            const string pcPre = "https://www.facebook.com/CodingSimplifyLife/posts/";
            if (url.StartsWith(pcPre)) return url.Substring(pcPre.Length);
            const string
                phonePre = "https://m.facebook.com/story.php?story_fbid=",
                phoneSuf = "&id=1848324468771150";
            if (url.StartsWith(phonePre) && url.EndsWith(phoneSuf)) return url.Substring(phonePre.Length, url.Length - phonePre.Length - phoneSuf.Length);
            return null;
        }
    }
}
