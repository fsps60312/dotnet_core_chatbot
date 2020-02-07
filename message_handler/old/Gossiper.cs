using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace message_handler
{
    public class Gossiper : MyDialog<IMessageActivity>
    {
        protected override async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            var messageText = Main.GetConvertedMessageText(message);
            messageText = MapMessage(messageText);
            if (gossips.ContainsKey(messageText))
            {
                var list = gossips[messageText];
                await context.PostAsync(list[Main.Rand.Next(list.Count)]);
                message = null;
            }
            context.Done(message);
        }
        Dictionary<string, List<string>> __gossips__ = null;
        Dictionary<string, List<string>> gossips
        {
            get
            {
                if (__gossips__ == null)
                {
                    __gossips__ = new Dictionary<string, List<string>>();
                    for (int i = 0; i < gossipData.GetLength(0); i++)
                    {
                        if (!__gossips__.ContainsKey(gossipData[i, 0])) __gossips__.Add(gossipData[i, 0], new List<string>());
                        __gossips__[gossipData[i, 0]].Add(gossipData[i, 1]);
                    }
                }
                return __gossips__;
            }
        }
        string[,] gossipData = new string[23, 2]//input must be lower case
        {
            { "code風景區", "很棒的名字，不覺得嗎？XD<br/>然後，我的英文名字是「code scenic」哦，Google看看！<br/>總之，像欣賞風景一樣快樂的探索程式之美吧！"},
            //{"Code風景區" ,"很棒的名字，不覺得嗎？XD<br/>然後，我的英文名字是「Code Scenic」哦，Google看看！<br/>總之，像欣賞風景一樣快樂的探索程式之美吧！"},
            { "傳一則貼文的網址(?)","吼～不是真的要你說這句話啦！<br/>是你要傳一則貼文的網址給我～><" },
            {"說話","話" },
            {"借我錢","我沒錢><" },
            {"不要","好吧，你壞壞 :p" },
            {"對","沒錯，就是這樣！😎" },
            {"bot","嘿，什麼事？ ^_^" },
            {"qq","好啦，乖（拍拍" },
            {"你好雷哦","你也很雷，別五十步笑百步www" },
            {"hi","恩？" },
            {"在嗎","不在～（不知道你要幹嘛怎麼決定我要不要在呢？XD）" },
            {"ㄎㄎ","蝦？？<br/>不然我ㄎ回去好了<br/>ㄎㄎ" },
            {"掰掰","掰掰～歡迎隨時再傳訊息給我哦！>///<<br/>還是你只是說好玩的(?)" },
            {"好吧","耶耶～～" },
            {"小心回家不要被壞人抓走" ,"小心回家不要被洪水沖走"},
            {"所以你是誰","才不告訴你呢www" },
            {"不好說","對呀，不好說(?)" },
            {"你嗎","你猜呀～ ^_^" },
            {"你","很棒 (y) (X)" },
            {"好哦","\\(^o^)/（雖然不知道發生甚麼事XD）" },
            {"不好說","真的不好說（咦？）" },
            {"omg","喵(?)" },
            {"這是自動回覆嗎","有可能是，也有可能不是(?)" }
        };
        Dictionary<string, string> __mappings__ = null;
        Dictionary<string, string> mappings
        {
            get
            {
                if (__mappings__ == null)
                {
                    __mappings__ = new Dictionary<string, string>//input must be lower case
                    {
                        { "chat bot","bot" },
                        { "chatbot","bot" },
                        {"bye bye","掰掰" },
                        {"bye","掰掰" },
                        {"掰","掰掰" }
                    };
                }
                return __mappings__;
            }
        }
        private string MapMessage(string message)
        {
            message = message.ToLower().Trim(' ');
            while (mappings.ContainsKey(message)) message = mappings[message];
            return message;
        }
    }
}
