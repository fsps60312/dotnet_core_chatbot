using System;
using System.Collections.Generic;
using System.Text;

namespace message_handler
{
    [Serializable]
    class P2652267661710156 : DialogNode
    {
        enum Stage { Run, Stage1 };
        Stage stage = Stage.Run;
        const string problem_code =
            "l=[]\n" +
            "for i in range(5):\n" +
            "    l.append(lambda x:x+i)\n" +
            "for f in l:\n" +
            "    print(f(10))";
        const string desired_answer = "10\n11\n12\n13\n14\n";
        int CalEditDistance(string a,string b)
        {
            var dp = new int[a.Length + 1, b.Length + 1];
            for(int i=0;i<dp.GetLength(0);i++)
            {
                for(int j=0;j<dp.GetLength(1);j++)
                {
                    dp[i, j] = int.MaxValue;
                }
            }
            dp[0, 0] = 0;
            var update = new Action<(int, int), int>((x, v) => dp[x.Item1, x.Item2] = Math.Min(dp[x.Item1, x.Item2], v));
            for(int i=0;i<a.Length;i++)
            {
                for(int j=0;j<b.Length;j++)
                {
                    if (dp[i, j] == int.MaxValue) continue;
                    update((i + 1, j), dp[i, j] + 1);
                    update((i, j + 1), dp[i, j] + 1);
                    update((i + 1, j + 1), dp[i, j] + (a[i] == b[j] ? 0 : 1));
                }
            }
            return dp[a.Length, b.Length];
        }
        void Stage1()
        {
            string python_code = sender_msg;
            SendMsg($"收到你的code：\n{python_code}");
            Sleep(2000);
            int edit_distance = CalEditDistance(problem_code, python_code);
            SendMsg($"你code的edit distance是{edit_distance}");
            Sleep(2000);
            if (edit_distance>4)
            {
                SendMsg("你的code改太多囉，edit distance最多是4！");
                EndDialog(new DialogEntry());
            }
            SendMsg("正在run你的code...");
            Sleep(5000);
            string res = Bash.Cmd("python3", python_code);
            SendMsg($"結果：\n{res}");
            Sleep(3000);
            if(res==desired_answer)
            {
                if (python_code.Contains('#'))
                {
                    SendMsg("靠北哦，偷用註解=^=");
                    Sleep(5000);
                    SendMsg("好啦，就算你對一半嘍！");
                    Sleep(3000);
                    SendMsg("下次告訴我正解以解鎖小故事(ゝ∀･)b");
                }
                else
                {
                    SendMsg("恭喜答對囉！^_^");
                    Sleep(5000);
                    SendMsg("上次小莫在train某個model");
                    Sleep(3000);
                    SendMsg("因為模型有多個output，所以偷懶用迴圈去設定每個output的loss function");
                    Sleep(3000);
                    SendMsg("model.fit(... , loss=[lambda yt,yp:my_loss(yt,yp)*i**2 for i in range(5)], ...)");
                    Sleep(2000);
                    SendMsg("按下run，漸漸發現好像哪裡怪怪的... 乾！！怎麼每個loss function都長得一樣 (／‵Д′)／~ ╧╧");
                    Sleep(4000);
                    SendMsg("所以就不能只有我被python雷啦，要大家一起被雷！⎝( OωO)⎠\n你覺得這題bug難度如何呢？歡迎分享你的感想哦！");
                }
            }
            else
            {
                SendMsg("ㄉㄟㄉㄟ～答錯了\n再想想看怎麼修這個bug吧XD");
                Sleep(3000);
                SendMsg($"你應該要讓輸出變成：\n{desired_answer}");
            }
            EndDialog(new DialogEntry());
        }
        public override void Run()
        {
            switch (stage)
            {
                case Stage.Run:
                    SendMsg("想要debug是吧？XD\n好，來！請輸入您的code～\n這個code拿去改，不要改太多哦...");
                    Sleep(2000);
                    SendMsg(problem_code);
                    stage = Stage.Stage1;
                    break;
                case Stage.Stage1: Stage1(); break;
                default: Bug(stage.ToString()); return;
            }
            EndDialog(this);
        }
    }
}
