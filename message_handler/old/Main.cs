using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace message_handler
{
    public partial class Main<T>: MyDialog<T>
    {
        async Task ResumeAfterFinalJudge(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            context.Wait(MessageReceivedAsync);//TODO
        }
        async Task ResumeAfterMathHelper(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (message == null) context.Wait(MessageReceivedAsync);
            else await context.Forward(new FinalJudge(), ResumeAfterFinalJudge, message);
        }
        async Task ResumeAfterGossiper(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            try
            {
                var message = await argument;
                if (message == null) context.Wait(MessageReceivedAsync);
                else await context.Forward(new MathHelper.MathHelper(), ResumeAfterMathHelper, message);
            }
            catch (Exception error) { await context.PostAsync(error.ToString()); }
        }
        async Task ResumeAfterUrlReactor(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            try
            {
                //await ResumeAfterGossiper(context, argument); return;
                var message = await argument;
                if (message == null) context.Wait(MessageReceivedAsync);
                else await context.Forward(new Gossiper(), ResumeAfterGossiper, message);
            }
            catch (Exception error) { await context.PostAsync(error.ToString()); }
        }
        async Task ResumeAfterBasicJudge(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            try
            {
                var message = await argument;
                if (message == null) context.Wait(MessageReceivedAsync);
                else await context.Forward(new Posts.UrlReactor(), ResumeAfterUrlReactor, message);
            }
            catch (Exception error) { await context.PostAsync(error.ToString()); }
        }
        protected override async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            //Set(message, Constants.ConvertedMessageText, ConvertMessageText(message.Text));
            await context.Forward(new BasicJudge(), ResumeAfterBasicJudge, message);
        }
    }
    public partial class Main
    {
        public static Random Rand = new Random();
        public static List<double> ReadDoubles(string s)
        {
            List<double> ans = new List<double>();
            int i = 0;
            while (i < s.Length)
            {
                for (; i < s.Length && !char.IsNumber(s[i]); i++) ;
                if (i >= s.Length) break;
                double v = 0;
                for (; i < s.Length && char.IsNumber(s[i]); i++) v = v * 10 + s[i] - '0';
                if (i < s.Length && s[i] == '.')
                {
                    i++;
                    for (double d = 0.1; i < s.Length && char.IsNumber(s[i]); d *= 0.1, i++) v += d * (s[i] - '0');
                }
                ans.Add(v);
            }
            return ans;
        }
        public static async Task PostImage(IDialogContext context, string url)
        {
            //await context.PostAsync("");
            var msg = context.MakeMessage();
            //new Microsoft.Bot.Connector.Attachment(Microsoft.Bot.Connector.ActionTypes.ShowImage, "https://lh3.googleusercontent.com/cd3ESRhwidl-flcOj_rF6nqX6NShAiH8S2T5gafsR_RxymqNGxReTiwxmjtnoDYDML2h4ISp49Frmg=w1626-h1620-no");
            var contentType = "image/png";
            if (url.ToLower().EndsWith(".jpg")) contentType = "image/jpeg";// Not "image/jpg"
            msg.Attachments.Add(new Attachment
            {
                ContentUrl = url,
                ContentType = contentType,
                Name = "圖示"
            });
            await context.PostAsync(msg);
            //AdaptiveCard card = new AdaptiveCard();
            //card.Body.Add(new TextBlock
            //{
            //    Text = "\u60f3\u8981\u5c0d\u7b54\u6848\u662f\u5427\uff1fXD<br/>\u597d\uff0c\u4f86\uff01\u8acb\u8f38\u5165\u60a8\u7684\u7b54\u6848\uff5e"/*想要對答案是吧？XD<br/>好，來！請輸入您的答案～*/,
            //    Size = TextSize.Medium,
            //    Weight = TextWeight.Normal
            //});
            //context.Activity.
            //card.Actions.Add(new SuggestedActions
            //{

            //})
            //throw new NotImplementedException();
        }
        public static bool IsContextCompleted(IMessageActivity message)
        {
            Newtonsoft.Json.Linq.JToken value;
            return message.From.Properties.TryGetValue(Constants.IsContextCompleted, out value);
        }
        public static void MarkContextCompleted(IMessageActivity message)
        {
            if (!IsContextCompleted(message)) message.From.Properties.Add(Constants.IsContextCompleted, default(Newtonsoft.Json.Linq.JToken));
        }
        public static string GetConvertedMessageText(IMessageActivity message)
        {
            //return Get(message, Constants.ConvertedMessageText);
            return ConvertMessageText(message.Text);
        }
        private static string Get(IMessageActivity message, string name)
        {
            return (string)message.From.Properties.GetValue(name);
        }
        private static void Set(IMessageActivity message, string name, string value)
        {
            message.From.Properties.Add(name, value);
        }
        public static string ConvertMessageText(string msg)
        {
            msg = msg.Replace("？", "?").Replace("什麼", "甚麼").Replace("神麼", "甚麼").Replace("啥", "甚麼").Replace('喔', '哦').Replace('\t', ' ').TrimEnd(new char[] { '?', ' ' }).TrimStart(' ');
            msg = RemoveDuplicatedSpaces(msg);
            return Mapping.Mapper.Map(msg);
        }
        public static string RemoveDuplicatedSpaces(string msg)
        {
            string ans = "";
            for (int i = 0; i < msg.Length; i++) if (!(i > 0 && msg[i] == ' ' && msg[i - 1] == ' ')) ans += msg[i];
            return ans;
        }
    }
}
