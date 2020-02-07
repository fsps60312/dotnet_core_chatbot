using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace message_handler.Posts
{
    public class P1995730270697235 : MyDialog<IMessageActivity>
    {
        [Serializable]
        class Graph
        {
            int N, M;
            List<HashSet<int>> ET;
            bool RemoveExtraNodesForPlanarIdentification()
            {
                for (int i = 0; i < N; i++)
                {
                    if (ET[i].Contains(i)) ET[i].Remove(i);
                    if (ET[i].Count == 1)
                    {
                        int a = i, b = ET[i].ElementAt(0);
                        ET[a].Remove(b);
                        ET[b].Remove(a);
                        return true;
                    }
                    else if (ET[i].Count == 2)
                    {
                        int a = ET[i].ElementAt(0), b = i, c = ET[i].ElementAt(1);
                        ET[a].Remove(b);
                        ET[b].Remove(a);
                        ET[b].Remove(c);
                        ET[c].Remove(b);
                        ET[a].Add(c);
                        ET[c].Add(a);
                        return true;
                    }
                }
                return false;
            }
            bool IsK55(List<int> s)
            {
                foreach (int a in s) foreach (int b in s) if (a < b && !ET[a].Contains(b)) return false;
                return true;
            }
            bool IsK33(List<int> aa, List<int> bb)
            {
                foreach (int a in aa) foreach (int b in bb) if (!ET[a].Contains(b)) return false;
                return true;
            }
            int __builtin_popcount(int v)
            {
                unchecked
                {
                    v = (v & (int)0x55555555) + ((v & (int)0xaaaaaaaa) >> 1);
                    v = (v & (int)0x33333333) + ((v & (int)0xcccccccc) >> 2);
                    v = (v & (int)0x0f0f0f0f) + ((v & (int)0xf0f0f0f0) >> 4);
                    v = (v & (int)0x00ff00ff) + ((v & (int)0xff00ff00) >> 8);
                    v = (v & (int)0x0000ffff) + ((v & (int)0xffff0000) >> 16);
                }
                return v;
            }
            bool IsK33(List<int> s)
            {
                for (int _ = 0; _ < (1 << 6); _++) if (__builtin_popcount(_) == 3)
                    {
                        List<int>[] ss = new List<int>[2] { new List<int>(), new List<int>() };
                        for (int i = 0; i < 6; i++) ss[(_ >> i) & 1].Add(s[i]);
                        if (IsK33(ss[0], ss[1])) return true;
                    }
                return false;
            }
            List<int> Trans(int s)
            {
                List<int> vs = new List<int>();
                for (int i = 0; i < N; i++) if ((s & (1 << i)) > 0) vs.Add(i);
                return vs;
            }
            bool IsK55(int s) { return IsK55(Trans(s)); }
            bool IsK33(int s) { return IsK33(Trans(s)); }
            public bool IsPlanar()
            {
                while (RemoveExtraNodesForPlanarIdentification()) ;
                for (int s = 0; s < (1 << N); s++)
                {
                    //		printf("s=%d, popcount=%d\n",s,__builtin_popcount(s));
                    if (__builtin_popcount(s) == 5 && IsK55(s)) return false;
                    if (__builtin_popcount(s) == 6 && IsK33(s)) return false;
                }
                return true;
            }
            List<int> colors;
            public List<int> Colors { get { return colors; } }
            bool IsColorsValid()
            {
                for (int a = 0; a < N; a++) foreach (int b in ET[a]) if (colors[a] == colors[b]) return false;
                return true;
            }
            public bool CanThreeColored()
            {
                int s_max = 1;
                for (int i = 0; i < N; i++) s_max *= 3;
                for (int s = 0; s < s_max; s++)
                {
                    colors = new List<int>();
                    int ss = s;
                    for (int i = 0; i < N; i++, ss /= 3) colors.Add(ss % 3);
                    if (IsColorsValid()) return true;
                }
                return false;
            }
            bool IsSubclique(List<int> s)
            {
                foreach (int a in s) foreach (int b in s) if (a < b && !ET[a].Contains(b)) return false;
                return true;
            }
            public List<int> GetMaxCliqueForPlanar()
            {
                for (int a = 0; a < N; a++) for (int b = a + 1; b < N; b++) for (int c = b + 1; c < N; c++) for (int d = c + 1; d < N; d++) if (IsSubclique(new List<int> { a, b, c, d })) return new List<int> { a, b, c, d };
                for (int a = 0; a < N; a++) for (int b = a + 1; b < N; b++) for (int c = b + 1; c < N; c++) if (IsSubclique(new List<int> { a, b, c })) return new List<int> { a, b, c };
                for (int a = 0; a < N; a++) for (int b = a + 1; b < N; b++) if (IsSubclique(new List<int> { a, b })) return new List<int> { a, b };
                for (int a = 0; a < N; a++) if (IsSubclique(new List<int> { a })) return new List<int> { a };
                return new List<int>();
            }
            public Graph(int _N, int _M, List<HashSet<int>> _ET)
            {
                N = _N; M = _M; ET = _ET;
            }
        }
        int N, M, EdgeRemain;
        Dictionary<string, HashSet<string>> ET = new Dictionary<string, HashSet<string>>();
        Graph BuildGraph()
        {
            Dictionary<string, int> idx = new Dictionary<string, int>();
            int i = 0;
            foreach (var p in ET) idx[p.Key] = i++;
            return new Graph(ET.Count, M, ET.Select(v1 => new HashSet<int>(v1.Value.Select(v2 => idx[v2]))).ToList());
        }
        bool IsPlanar() { return BuildGraph().IsPlanar(); }
        Dictionary<string, int> Colors = null;
        bool CanThreeColored()
        {
            var graph = BuildGraph();
            var ret = graph.CanThreeColored();
            Colors = new Dictionary<string, int>();
            for (int i = 0; i < graph.Colors.Count; i++) Colors[ET.ElementAt(i).Key] = graph.Colors[i];
            return ret;
        }
        List<string> GetMaxCliqueForPlanar() { return BuildGraph().GetMaxCliqueForPlanar().Select(v => ET.ElementAt(v).Key).ToList(); }
        public async Task Stage4(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            switch (message.Text.ToLower())
            {
                case "quit": await context.PostAsync("掰掰～小心回家不要被壞人抓走哦XDD"); break;
                case "這跟code有甚麼關係？":
                    {
                        var problemUrl = "https://ada-judge.csie.org/#!/problem/12";
                        var statusImage = "https://3.bp.blogspot.com/-odKLIepgfnk/Wi9w_GEsWHI/AAAAAAAAKDc/3_kGJIhSN_8PVUTIr23nIg7mBQH4-bRSACLcBGAs/s1600/Screenshot%2B%2528502%2529.png";
                        var rankingImage = "https://3.bp.blogspot.com/-xk_nfzqenUs/Wi9w_Pr2e2I/AAAAAAAAKDY/1VrYC2EUQUk1pikdKHZombO3FW5AaaxxwCLcBGAs/s1600/Screenshot%2B%2528503%2529.png";
                        var myDisprove = "https://2.bp.blogspot.com/-pBbGdM_p87w/Wi9yHDfetSI/AAAAAAAAKDo/GpQffboxOqY6FWd3rpo7zTXemOARDfaEgCPcBGAYYCw/s1600/DSC_0047.JPG";
                        await context.PostAsync($"題目網址：{problemUrl}");
                        await Main.PostImage(context, statusImage);
                        await context.PostAsync("其實呢這要從台大資工大二必修課作業開始講起，這堂課叫做ADA，第三次作業中有一題線上作業叫做「Metropolitan」");
                        await Task.Delay(10000);
                        await Main.PostImage(context, rankingImage);
                        await context.PostAsync("行健同學一時想不出好的解法，只好直接假設了「若平面圖無法3著色則最大團大小等於4」這個性質是好的，然後照這個想法寫了一份code，傳上去就AC了XDD 還在速度上得到第1名！<(\\_ \\_)>");
                        await Task.Delay(15000);
                        await context.PostAsync("於是小莫就想要幫他驗證這個性質是不是好的。殊不知，就不小心找到反例了呢！>///<");
                        await Task.Delay(9000);
                        await context.PostAsync("這是小莫當初找到的反例哦：");
                        await Main.PostImage(context, myDisprove);
                        await Task.Delay(5000);
                        await context.PostAsync("有沒有和你找到的反例一樣呢？ ;)");
                        await context.PostAsync("-----The End-----");
                        await Task.Delay(3000);
                        await context.PostAsync("歡迎再傳訊息給我哦！>w<");
                        break;
                    }
                default:
                    await context.PostAsync($"請輸入「這跟code有甚麼關係？」，您輸入的是「{message.Text}」");
                    return;
            }
            context.Done<IMessageActivity>(null);
        }
        public async Task Stage3(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            switch (message.Text.ToLower())
            {
                case "quit": await context.PostAsync("掰掰～歡迎隨時再傳訊息給我哦！>///<"); break;
                case "重新輸入":
                    {
                        ET.Clear();
                        EdgeRemain = M;
                        await context.PostAsync($"請重新輸入您的M={M}條邊：");
                        return;
                    }
                case "重新輸入n和m":
                    {
                        ET.Clear();
                        await context.PostAsync("請重新輸入您的N和M：");
                        context.Wait(Stage2);
                        return;
                    }
                default:
                    {
                        var data = message.Text.Split(new char[] { ' ', '\r', '\n' }).Where((v) => !string.IsNullOrWhiteSpace(v)).ToList();
                        if (data.Count % 2 == 1)
                        {
                            await context.PostAsync($"您輸入了奇數 ({data.Count}) 個點，但正常來講不管幾條邊都會有偶數個點耶（每條邊2個點），要不要再檢查看看您的輸入呢？><<br/>「重新輸入」來重新輸入這M={M}條邊<br/>「重新輸入N和M」來重新輸入N和M");
                            return;
                        }
                        if (data.Count / 2 > EdgeRemain)
                        {
                            await context.PostAsync($"您輸入太多邊了，之前您已經輸入了{M - EdgeRemain}條邊，因此只剩{EdgeRemain}條邊可以輸入哦！<br/>「重新輸入」來重新輸入這M={M}條邊<br/>「重新輸入N和M」來重新輸入N和M");
                            return;
                        }
                        {
                            var sb = new StringBuilder();
                            for (int i = 0; i < data.Count; i += 2) sb.Append($"{data[i]} ←→ {data[i + 1]}<br/>");
                            await context.PostAsync(sb.ToString());
                        }
                        {
                            int countAfterInserting = ET.Select(p => p.Key).Union(data).Count();
                            if (countAfterInserting > N)
                            {
                                await context.PostAsync($"若將您目前輸入的{data.Count / 2}條邊加進去，就會有{countAfterInserting}個點，但您一開始說總共有N={N}個點，是不是哪裡出錯了呢？<br/>「重新輸入」來重新輸入這M={M}條邊<br/>「重新輸入N和M」來重新輸入N和M");
                                return;
                            }
                        }
                        {
                            var selfLoop = new Func<string>(() =>
                            {
                                for (int i = 0; i < data.Count; i += 2) if (data[i] == data[i + 1]) return data[i];
                                return null;
                            })();
                            if (selfLoop != null)
                            {
                                await context.PostAsync($"不能有自環哦！自環就是兩邊都是同一個點的邊，這是您輸入中的其中一個自環：{selfLoop} ←→ {selfLoop}");
                                return;
                            }
                        }
                        EdgeRemain -= data.Count / 2;
                        for (int i = 0; i < data.Count; i += 2)
                        {
                            if (!ET.ContainsKey(data[i])) ET.Add(data[i], new HashSet<string>());
                            if (!ET.ContainsKey(data[i + 1])) ET.Add(data[i + 1], new HashSet<string>());
                            ET[data[i]].Add(data[i + 1]);
                            ET[data[i + 1]].Add(data[i]);
                        }
                        if (EdgeRemain > 0)
                        {
                            await context.PostAsync($"您這次輸入了{data.Count / 2}條邊，請繼續輸入剩下的{EdgeRemain}條邊：");
                            return;
                        }
                        else
                        {
                            await context.PostAsync($"您這次輸入了{data.Count / 2}條邊，輸入完成！");
                            if (ET.Count == N) await context.PostAsync($"您的輸入包含了{ET.Count}個點，和N一樣！");
                            else await context.PostAsync($"警告！您的輸入包含了{ET.Count}個點，可是N={N}，不一樣！");
                            await Task.Delay(1000);
                            await context.PostAsync("驗證您的答案中，請稍後......");
                            await Task.Delay(1000);
                            await context.PostAsync("驗證是否為平面圖......");
                            await Task.Delay(1000);
                            if (!IsPlanar())
                            {
                                await context.PostAsync("答案錯誤！您的反例不是平面圖哦，請再檢查～ ^\\_^");
                                break;
                            }
                            await context.PostAsync("驗證是否無法3著色......");
                            await Task.Delay(1000);
                            if (CanThreeColored())
                            {
                                await context.PostAsync("答案錯誤！您的反例其實可以3著色哦～");
                                await Task.Delay(3000);
                                var sb = new StringBuilder();
                                foreach (var p in Colors) sb.Append($"「{p.Key}」塗上「{new string[3] { "紅色", "藍色", "綠色" }[p.Value]}」<br/>");
                                await context.PostAsync(sb.ToString());
                                await Task.Delay(1000);
                                await context.PostAsync("啪搭～就是這樣～");
                                break;
                            }
                            await context.PostAsync("真的是一個無法3著色的平面圖耶！正在計算最大團大小，看看是不是真的<4......");
                            await Task.Delay(1000);
                            var clique = GetMaxCliqueForPlanar();
                            {
                                StringBuilder sb = new StringBuilder("其中一個最大團：<br/>");
                                foreach (var v in clique) sb.Append($" {v}");
                                await context.PostAsync(sb.ToString());
                            }
                            if (clique.Count < 4)
                            {
                                await context.PostAsync("AC！答對了！恭喜您！ ^\\_^");
                                await context.PostAsync("現在來問問看這跟code有甚麼關係吧！請輸入：「這跟code有甚麼關係？」");
                                context.Wait(Stage4);
                                return;
                            }
                            else
                            {
                                await context.PostAsync($"WA！答錯了！您的反例最大團大小是{clique.Count}哦～再接再勵，加油吧！");
                                break;
                            }
                        }
                    }
            }
            context.Done<IMessageActivity>(null);
        }
        public async Task Stage2(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            switch (message.Text.ToLower())
            {
                case "quit": await context.PostAsync("掰掰～歡迎隨時再傳訊息給我哦！>///<"); break;
                default:
                    {
                        var data = message.Text.Split(' ').Where(v => { int tmp; return int.TryParse(v, out tmp); }).ToList();
                        if (data.Count != 2)
                        {
                            await context.PostAsync($"請輸入2個數字，您輸入了{data.Count}個");
                            return;
                        }
                        else
                        {
                            var nums = data.Select((v) => int.Parse(v)).ToList();
                            N = nums[0]; M = nums[1];
                            await context.PostAsync($"您輸入了N={N}，M={M}");
                            if (N > 10 || M > 45)
                            {
                                if (N > 10 && M <= 45) await context.PostAsync("N太大了，N的上限是10哦！");
                                else if (N <= 10 && M > 45) await context.PostAsync("M太大了，M的上限是45哦！");
                                else await context.PostAsync("N和M都太大了，N的上限是10、M的上限是45哦！");
                                return;
                            }
                            else if (N <= 0)
                            {
                                await context.PostAsync("N不能為負數或0哦！");
                                return;
                            }
                            else if (M < 0)
                            {
                                await context.PostAsync("M不能為負數哦！");
                                return;
                            }
                            else if (M == 0)
                            {
                                await context.PostAsync("0條邊？！你在開玩笑吧？沒有邊的圖連「1著色」都可以了！你給我重新輸入！");
                                return;
                            }
                            else
                            {

                                await context.PostAsync($"現在請輸入M={M}條邊，每條邊由兩個點表示，例如：「A B」代表有一條邊從A連到B（也是從B連到A），「A B B C」代表有兩條邊AB和BC，現在，請輸入您反例中的M條邊：<br/>「重新輸入」來重新輸入這M={M}條邊<br/>「重新輸入N和M」來重新輸入N和M");
                                ET.Clear();
                                EdgeRemain = M;
                                context.Wait(Stage3);
                                return;
                            }
                        }
                    }
            }
            context.Done<IMessageActivity>(null);
        }
        public async Task AStage2(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            switch (message.Text.ToLower())
            {
                case "我已經放棄了，就給我後悔吧":
                    {
                        await context.PostAsync("好啦，您真沒志氣(X)，硬要看答案也只好跟您說囉～");
                        await Task.Delay(3000);
                        await context.PostAsync("這題是反證");
                        await Task.Delay(3000);
                        await context.PostAsync("認真想一下會發現有反例（屬於平面圖、無法3著色，但最大團大小≤3），例如：");
                        await Task.Delay(6000);
                        await Main.PostImage(context, "https://4.bp.blogspot.com/-p9UsT_8oO5U/WjM_cSm7CuI/AAAAAAAAKEU/tGA8Y4cI3BcwAfzJsCx8ZknsQ2Fd288PQCLcBGAs/s1600/Screenshot%2B%2528523%2529.png");
                        await context.PostAsync("這只是其中一種反例，掰掰，看您能不能找到更簡單的囉～");
                        break;
                    }
                case "既然你都這麼說了，那我還是想想看好了": await context.PostAsync("耶耶～很高興您終於想通了，加油！想到答案一定要記得告訴我哦！>///<"); break;
                default:
                    try { throw new NotImplementedException(); }
                    catch (Exception error) { await Main.PrintBug(context, error.ToString()); break; }
            }
            context.Done<IMessageActivity>(null);
        }
        public async Task _AStage2(IDialogContext context, IAwaitable<string> argument)
        {
            await AStage2(context, Awaitable.FromItem(new Activity { Text = await argument, From = new ChannelAccount() }));
        }
        public async Task AStage1(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            switch (message.Text.ToLower())
            {
                case "不管，我要看答案":
                    {
                        await context.PostAsync("真的不再想想嗎？看完答案不要後悔哦～XD<br/>建議先自己想想看哦，不會很難的！ ;)");
                        await Task.Delay(3000);
                        PromptDialog.Choice(context, _AStage2, new List<string> { "我已經放棄了，就給我後悔吧", "既然你都這麼說了，那我還是想想看好了" }, "請選擇：", "請輸入「我已經放棄了，就給我後悔吧」或「既然你都這麼說了，那我還是想想看好了」");
                        return;
                    }
                case "好吧，我再想想": await context.PostAsync("耶～很高興您做出了正確選擇，加油！想到答案要告訴我哦！>///<"); break;
                default:
                    try { throw new NotImplementedException(); }
                    catch (Exception error) { await Main.PrintBug(context, error.ToString()); break; }
            }
            context.Done<IMessageActivity>(null);
        }
        public async Task _AStage1(IDialogContext context, IAwaitable<string> argument)
        {
            await AStage1(context, Awaitable.FromItem(new Activity { Text = await argument, From = new ChannelAccount() }));
        }
        public async Task Stage1(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            switch (message.Text.ToLower())
            {
                case "quit": await context.PostAsync("掰掰～一定要記得下次告訴我這題的答案哦！>///<"); break;
                case "prove":
                    {
                        await context.PostAsync("恭喜你～");
                        await Task.Delay(2000);
                        await context.PostAsync("答錯了！！");
                        await Task.Delay(1000);
                        await context.PostAsync("請再想想吧～XD");
                    }
                    break;
                case "disprove":
                    {
                        await context.PostAsync("咦，想必您是想到反例囉？請給我看看您想到甚麼反例吧～");
                        await context.PostAsync("請輸入兩個數字N和M，N是反例中的點數，M是反例中的邊數，例如：「2 3」代表反例中有2個點、3條邊。現在，請照順序輸入您的N和M：");
                        context.Wait(Stage2);
                        return;
                    }
                case "我要看答案！":
                    {
                        await context.PostAsync("真的要看答案嗎？<br/>這題真的很好玩耶，要不要再想一下？><");
                        await Task.Delay(3000);
                        PromptDialog.Choice(context, _AStage1, new List<string> { "不管，我要看答案", "好吧，我再想想" }, "請選擇：", "請輸入「不管，我要看答案」或「好吧，我再想想」");
                        return;
                    }
                default:
                    {
                        await context.PostAsync($"請輸入「prove」或「disprove」，您輸入的是「{message.Text}」，任何時候輸入「quit」可以退出");
                        context.Wait(Stage1);
                        return;
                    }
            }
            context.Done<IMessageActivity>(null);
        }
        public async Task _Stage1(IDialogContext context, IAwaitable<string> argument)
        {
            await Stage1(context, Awaitable.FromItem(new Activity { Text = await argument, From = new ChannelAccount() }));
        }
        protected override async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            await context.PostAsync("想要對答案是吧？XD<br/>好，來！請輸入您的答案～<br/>任何時候輸入「quit」可以退出");
            PromptDialog.Choice(context, _Stage1, new List<string> { "Prove", "Disprove", "我要看答案！", "Quit" }, "請問您要prove還是disprove呢？", "請輸入「prove」或「disprove」，任何時候輸入「quit」可以退出");
        }
    }
}
