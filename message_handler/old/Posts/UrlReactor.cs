using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace message_handler.Posts
{
    public class UrlReactor : MyDialog<IMessageActivity>
    {
        protected override async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            switch (message.Text)
            {
                case "https://codingsimplifylife.blogspot.tw/": await context.PostAsync("code風景區！！！歡迎常來逛逛～～～你會發現，寫程式就像欣賞風景一樣快樂哦！"); message = null; break;
                case "https://codingsimplifylife.blogspot.tw/2016/04/c.html": await context.PostAsync("給新手的C++教學！！！號稱網路上對新手最友善的C++教學，歡迎推薦給親朋好友，或提供改善建議哦！"); message = null; break;
                default:
                    {
                        switch (GetPostId(message.Text))
                        {
                            case null:
                                {
                                    if (message.Text.StartsWith("http://") || message.Text.StartsWith("https://"))
                                    {
                                        if (message.Text.StartsWith("https://codingsimplifylife.blogspot."))
                                        {
                                            await context.PostAsync("這是「code風景區」的連結，不是「Code風景區」的連結哦XDD");
                                        }
                                        else
                                        {
                                            await context.PostAsync("偷偷告訴你，以後傳某些連結（連結=網址）給我（特別是Code風景區某些文章的連結）會有特殊反應哦！敬請期待！>///<<br/>還有，你傳的網址根本不是Code風景區的網址，拒收！><");
                                        }
                                        message = null;
                                    }
                                }
                                break;
                            case "1995730270697235": await context.Forward(new P1995730270697235(), ResumeAfterAnything, message); return;
                            case "2002954469974815": await context.Forward(new P2002954469974815(), ResumeAfterAnything, message); return;
                            case "2003744179895844": await context.Forward(new P2003744179895844(), ResumeAfterAnything, message); return;
                            default: await context.PostAsync("Oops......這篇文沒有彩蛋哦～試試看別篇吧XD"); message = null; break;
                        }
                    }
                    break;
            }
            context.Done(message);
        }
        async Task ResumeAfterAnything(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            context.Done<IMessageActivity>(null);
            //Main.MarkContextCompleted(message);
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
