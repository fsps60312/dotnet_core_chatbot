using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace message_handler
{
    [Serializable]
    class 幾A幾B : DialogNode
    {
        enum Stage { Run, Stage1 };
        Stage stage = Stage.Run;
        bool is_valid(string s)
        {
            return s.Length == 4 && s.All(c => '0' <= c && c <= '9') && s.Distinct().Count() == 4;
        }
        string gen()
        {
            return string.Join("", Shuffled(Range(9)).GetRange(0, 4));
        }
        (int, int) hint(string ans, string guess)
        {
            if (!is_valid(ans) || !is_valid(guess)) throw new Exception();
            int a = ans.Zip(guess).Count(v => v.First == v.Second);
            return (a, (8 - (ans + guess).Distinct().Count()) - a);
        }
        string answer;
        int num_guess = 0;
        DateTime start_time;
        void Stage1()
        {
            Func<string> give_example = () =>
              {
                  string ans = gen(), guess = gen();
                  (int a, int b) = hint(ans, guess);
                  return $"當答案為{ans}，你猜{guess}時，我會提示{a}A{b}B。";
              };
            if (sender_msg == "規則")
            {
                SendMsg("一個人設定一組四碼的數字作為謎底，另一方猜。每猜一個數，出數者就要根據這個數字給出提示，提示以XAYB形式呈現，直到猜中為止。其中X表示位置正確的數的個數，而Y表示數字正確而位置不對的數的個數。");
                Sleep(5000);
                SendMsg("例如：");
                Sleep(3000);
                SendMsg(give_example());
                Sleep(3000);
                SendMsg(give_example());
                Sleep(3000);
                SendMsg("更多介紹可參考：https://zh.wikipedia.org/wiki/猜数字");
                EndDialog(this);
            }
            else if (sender_msg == "退出")
            {
                SendMsg("掰掰~");
                EndDialog(new DialogEntry());
            }
            else if (sender_msg == "範例")
            {
                SendMsg(give_example());
                Sleep(3000);
                SendMsg(give_example());
                EndDialog(this);
            }
            else
            {
                if (!is_valid(sender_msg))
                {
                    SendMsg($"請輸入4位不重複數字，例如{gen()}");
                    EndDialog(this);
                }
                num_guess++;
                if (num_guess == 1) start_time = DateTime.Now;
                string guess = sender_msg;
                if (guess == answer)
                {
                    SendMsg($"恭喜答對！沒錯！答案就是{answer}！🎉🎉🎉");
                    string play_time_describe = "";
                    var ts = DateTime.Now - start_time;
                    if (ts.Days > 0) play_time_describe += $"{ts.Days}天";
                    if (ts.Hours > 0) play_time_describe += $"{ts.Hours}小時";
                    if (ts.Minutes > 0) play_time_describe += $"{ts.Minutes}分";
                    if (ts.Seconds > 0) play_time_describe += $"{ts.Seconds}秒";
                    Sleep(3000);
                    SendMsg($"你總共猜了{num_guess}次，耗時{play_time_describe}，趕快跟朋友炫耀吧！");
                    Sleep(3000);
                    SendButtons("意猶未盡？", "輸入幾A幾B再挑戰一次！", new[] { "幾A幾B" });
                    EndDialog(new DialogEntry());
                }
                else
                {
                    (int a, int b) = hint(answer, guess);
                    SendMsg($"{a}A{b}B");
                    if (num_guess >= 7)
                    {
                        SendMsg($"失敗，答案是{answer}");
                        Sleep(3000);
                        SendButtons("不甘心嗎？", "輸入幾A幾B再玩一次！", new[] { "幾A幾B" });
                        EndDialog(new DialogEntry());
                    }
                    EndDialog(this);
                }
            }
        }
        public override void Run()
        {
            switch (stage)
            {
                case Stage.Run:
                    if (sender_msg.Trim().ToUpper() != "幾A幾B" && sender_msg.Trim() != "猜數字") return;
                    answer = gen();
                    SendButtons("請輸入4位數開始遊戲", $"謎底是不重複的4位數，例如{gen()}", new[] { "規則", "範例", "退出" });
                    stage = Stage.Stage1;
                    break;
                case Stage.Stage1: Stage1(); break;
                default: Bug(stage.ToString()); return;
            }
            EndDialog(this);
        }
    }
}
