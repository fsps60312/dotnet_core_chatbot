using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace message_handler.MathHelper
{
    public class WolframAlpha : MyDialog<IMessageActivity>
    {
        class WolframQueryResult
        {
#pragma warning disable 0649
            public queryresultClass queryresult;
            public class queryresultClass
            {
                public bool success;
                public errorClass error;
                public class errorClass
                {
                    public string code, msg;
                    public static implicit operator errorClass(bool value)
                    {
                        System.Diagnostics.Trace.Assert(!value);
                        return null;
                    }
                    public static implicit operator bool(errorClass value)
                    {
                        // assuming, that 1 is true;
                        // somehow this method should deal with value == null case
                        return value != null && (value.code != null || value.msg != null);
                    }

                }
                public List<podsClass> pods;
                public class podsClass
                {
                    public string title;
                    public List<subpodsClass> subpods;
                    public class subpodsClass
                    {
                        public string title;
                        public imgClass img;
                        public class imgClass
                        {
                            public string src;
                        }
                        public string plaintext;
                        public string moutput;
                    }
                }
            }
#pragma warning restore 0649
        }
        const string appid = "PRE8LA-29W6W87WAR";
        protected override async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (message.Text.StartsWith("Walfram"))
            {
                await context.PostAsync($"取得Walfram Alpha計算結果...{message.Text.Substring(7)}");
                await Task.Delay(2000);
                await context.PostAsync("你拼錯字啦，你好雷喔！ :P");
                await context.PostAsync("是「Wolfram」啦！XD");
                message = null;
            }
            else if (message.Text.StartsWith("Wolfram"))
            {
                string q = message.Text.Substring(7);
                await context.PostAsync($"取得Wolfram Alpha計算結果...{q}");
                var client = new HttpClient();
                var url = $"http://api.wolframalpha.com/v2/query?input={System.Net.WebUtility.UrlEncode(q)}&format=image,moutput&output=JSON&appid={appid}";
                //await context.PostAsync(url);
                var response = await client.PostAsync(url, null);
                using (HttpContent content = response.Content)
                {
                    string json = await content.ReadAsStringAsync();
                    //await context.PostAsync(json);
                    try
                    {
                        var obj = JsonConvert.DeserializeObject<WolframQueryResult>(json);
                        if (!obj.queryresult.success || obj.queryresult.error)
                        {
                            string err = "";
                            if (obj.queryresult.error != null) err = $"Error code: {obj.queryresult.error.code}<br/>Error message: {obj.queryresult.error.msg}";
                            await context.PostAsync($"success: {obj.queryresult.success}<br/>{err}<br/>{json}");
                        }
                        else
                        {
                            foreach (var pod in obj.queryresult.pods)
                            {
                                if (!string.IsNullOrWhiteSpace(pod.title)) await context.PostAsync($"{pod.title}：");
                                foreach (var subpod in pod.subpods)
                                {
                                    if (!string.IsNullOrWhiteSpace(subpod.title)) await context.PostAsync($"{subpod.title}:");
                                    await context.PostAsync(string.IsNullOrEmpty(subpod.moutput) ? subpod.plaintext : subpod.moutput);
                                    await Main.PostImage(context, subpod.img.src);
                                }
                            }
                        }
                    }
                    catch (Exception error) { await context.PostAsync($"解析資料時發生問題：<br/>{error}<br/>原始資料：{json}"); }
                }
                message = null;
            }
            context.Done(message);
        }
    }
}
