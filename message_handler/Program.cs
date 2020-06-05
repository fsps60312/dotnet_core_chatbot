using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace message_handler
{
    class Program
    {
        public static DialogNode NextDialog { get; set; } = new DialogEntry();
        public static string SenderMsg { get { return sender_msg; } }
        public static string SenderPsid { get { return sender_psid; } }
        static string page_access_token, sender_psid, sender_msg;
        static HttpClient http_client = new HttpClient();
        public static void SendButtons(string title,string subtitle,string[] buttons)
        {
            if (buttons.Length > 3)
            {
                SendButtons(title, subtitle, buttons.Take(3).ToArray());
                SendButtons("More", "more", buttons.Skip(3).ToArray());
                return;
            }
            var response_obj = new
            {
                attachment = new
                {
                    type = "template",
                    payload = new
                    {
                        template_type = "generic",
                        elements = new[]
                        {
                            new
                            {
                                title=title,
                                subtitle=subtitle,
                                buttons = buttons.Select(b=>new{
                                    type="postback",
                                    title=b,
                                    payload=b
                                })
                            }
                        }
                    }
                }
            };
            string response = JsonConvert.SerializeObject(response_obj);
            Send(response);
        }
        public static void SendMsg(string msg)
        {
            const int msg_max_len = 2000;
            if(msg.Length> msg_max_len)
            {
                SendMsg($"Oops... 我最多只能傳送前面{msg_max_len}個字給你哦><\n原本是{msg.Length}個字！");
                msg = msg.Remove(msg_max_len);
            }
            string response = JsonConvert.SerializeObject(new { text = msg });
            Send(response);
        }
        public static void SendImage(string url)
        {
            //SendMsg(url);return;
            string attachment_id = UploadImage(url);
            var response_obj = new
            {
                attachment = new
                {
                    type = "template",
                    payload = new
                    {
                        template_type = "media",
                        elements = new[]
                        {
                            new
                            {
                                media_type="image",
                                attachment_id = attachment_id
                            }
                        }
                    }
                }
            };
            string response = JsonConvert.SerializeObject(response_obj);
            //Console.WriteLine(response);
            Send(response);
        }
        static string UploadImage(string url)
        {
            var response_obj = new
            {
                message = new
                {
                    attachment = new
                    {
                        type = "image",
                        payload = new
                        {
                            is_reusable = true,
                            url = url
                        }
                    }
                }
            };
            var content = new StringContent(JsonConvert.SerializeObject(response_obj), System.Text.Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://graph.facebook.com/v6.0/me/message_attachments?access_token=" + page_access_token);
            request.Content = content;
            var result = http_client.SendAsync(request).Result;
            if (!result.IsSuccessStatusCode) Console.WriteLine("failed to upload image: " + result.ReasonPhrase + "\n" + url);
            dynamic result_obj = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result);
            return result_obj.attachment_id;
        }
        static void Send(string response)
        {
            var content = new StringContent("{\"recipient\":{\"id\":" + sender_psid + "},\"message\":" + response + "}", System.Text.Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://graph.facebook.com/v6.0/me/messages?access_token="+ page_access_token);
            request.Content = content;
            var result = http_client.SendAsync(request).Result;
            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine("failed to send: " + result.ReasonPhrase);
                Console.WriteLine(string.Join(',',result.Headers.Select(v=>v.Key)));
                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                Console.WriteLine(response);

            }
            else Console.WriteLine("success");
        }
        static DialogNode ReadDialogNode()
        {
            if (!File.Exists(sender_psid + "_dialog_content.txt")) return new DialogEntry();
            try
            {
                Stream stream = new FileStream(sender_psid + "_dialog_content.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
                IFormatter formatter = new BinaryFormatter();
                var dialog_content = (DialogNode)formatter.Deserialize(stream);
                stream.Close();
                return dialog_content;
            }
            catch(Exception)
            {
                SendMsg("ReadDialogNode failed.");
                return new DialogEntry();
            }
            finally
            {
                File.Delete(sender_psid + "_dialog_content.txt");
            }
        }
        public static void WriteDialogNode(DialogNode dialog)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(sender_psid + "_dialog_content.txt", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, dialog);
            stream.Close();
        }
        static void Main(string[] args)
        {
            try
            {
                //Console.Write(args[1]);
                if (args.Length != 3)
                {
                    Console.Write($"Hello World!\n{string.Join("\n", args)}");
                    Environment.Exit(0);
                }
                page_access_token = args[0];
                sender_psid = args[1];
                sender_msg = args[2];
                var dialog_node = ReadDialogNode();
                dialog_node.Run();
            }
            catch(Exception error)
            {
                SendMsg("吼～崩潰啦，都是你害的><");
                System.Threading.Thread.Sleep(2000);
                SendMsg("為了報復你，我要叫你幫我debug哇哈哈哈OwO");
                System.Threading.Thread.Sleep(3000);
                SendMsg(error.ToString());
            }
        }
    }
}
