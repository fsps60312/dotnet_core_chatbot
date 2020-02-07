using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Http;
using Newtonsoft.Json;

namespace message_handler
{

    class WolframAlpha : DialogNode
    {
        class WolframQueryResult
        {
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
        }
        const string appid = "PRE8LA-29W6W87WAR";
        public override void Run()
        {
            if (sender_msg.StartsWith("Walfram"))
            {
                SendMsg($"取得Walfram Alpha計算結果...{sender_msg.Substring(7)}");
                Thread.Sleep(2000);
                SendMsg("你拼錯字啦，你好雷喔！ :P");
                SendMsg("是「Wolfram」啦！XD");
                EndDialog(Program.NextDialog);
            }
            else if (sender_msg.StartsWith("Wolfram"))
            {
                string q = sender_msg.Substring(7);
                SendMsg($"取得Wolfram Alpha計算結果...{q}");
                var client = new HttpClient();
                var url = $"http://api.wolframalpha.com/v2/query?input={System.Net.WebUtility.UrlEncode(q)}&format=image,plaintext,moutput&output=JSON&appid={appid}";
                //await context.PostAsync(url);
                var response = client.PostAsync(url, null).Result;
                using (HttpContent content = response.Content)
                {
                    string json = content.ReadAsStringAsync().Result;
                    var writer = new System.IO.StreamWriter("log.txt");
                    writer.Write(json);
                    writer.Close();
                    try
                    {
                        var obj = JsonConvert.DeserializeObject<WolframQueryResult>(json);
                        if (!obj.queryresult.success || obj.queryresult.error)
                        {
                            string err = "";
                            if (obj.queryresult.error != null) err = $"Error code: {obj.queryresult.error.code}<br/>Error message: {obj.queryresult.error.msg}";
                            SendMsg($"success: {obj.queryresult.success}<br/>{err}<br/>{json}");
                        }
                        else
                        {
                            foreach (var pod in obj.queryresult.pods)
                            {
                                if (!string.IsNullOrWhiteSpace(pod.title)) SendMsg($"{pod.title}：");
                                foreach (var subpod in pod.subpods)
                                {
                                    if (!string.IsNullOrWhiteSpace(subpod.title)) SendMsg($"{subpod.title}:");
                                    SendMsg(string.IsNullOrEmpty(subpod.moutput) ? subpod.plaintext : subpod.moutput);
                                    SendImage(subpod.img.src);
                                }
                            }
                        }
                    }
                    catch (Exception error) { SendMsg($"解析資料時發生問題：<br/>{error}<br/>原始資料：{json}"); }
                }
                EndDialog(Program.NextDialog);
            }
        }
    }
}
