using System;
using System.Collections.Generic;
using System.Text;

namespace message_handler.Mapping
{
    public static class Mapper
    {
        static Dictionary<string, string> mapping = new Dictionary<string, string>//input might be upper case
        {
            {"借我錢錢","借我錢" },
            {"我要說啥","我要說甚麼" },
            {"help","我要說甚麼" },
            {"我要怎麼說點話","我要說甚麼" },
            {"我應該要說甚麼","我要說甚麼" },
            {"SP 助教怎麼樣","SP助教怎麼樣" },
            {"嗨","hi" },
            {"你很雷耶","你好雷哦"},
            {"QAQ","QQ" }
        };
        public static string Map(string message)
        {
            while (mapping.ContainsKey(message)) message = mapping[message];
            return message;
        }
    }
}
