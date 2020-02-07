using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace message_handler.Posts
{
    class P2003744179895844
    {
        public async Task Stage1(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            switch (message.Text.ToLower())
            {
                case "我知道問題在哪了，真的好蠢www": await context.PostAsync("對呀真的好蠢哈哈XD"); break;
                case "問題到底在哪裡？><":
                    await context.PostAsync("C#的Dictionary在Add一個已存在的key時會跳Exception，然後小莫給Dictionary初始化用的Initializer List藏了兩個相同的key！！（仔細找，有兩個「不好說」！XXD）<br/>" +
      "所以在物件初始化的時候就crash了，然後在debug的時候完全不覺得new這個物件哪裡會有問題XD<br/>" +
      "總之，這個bug總算被我抓到啦～耶～"); break;
                default:
                    try { throw new NotImplementedException(); }
                    catch (Exception error) { await Main.PrintBug(context, error.ToString()); break; }
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
            await Main.PostImage(context, "https://1.bp.blogspot.com/-Pl5CWtj-KrQ/WjVLr97yhcI/AAAAAAAAKEk/u9QQIa998HEF4_f8WR15_eV8_6m7rsdZwCLcBGAs/s1600/Screenshot%2B%2528525%2529.png");
            await context.PostAsync("其實是這一段code出問題啦，您找到問題在哪裡了嗎？<br/>當然要發現問題出在這一段code也是花了好一番功夫啦Orz<br/>然後其實圖片中的code是縮減版本，小莫在debug的時候這段code有25行那麼長～");
            PromptDialog.Choice(context, _Stage1, new List<string> { "我知道問題在哪了，真的好蠢www", "問題到底在哪裡？><" }, "請問您的狀況？", "請輸入「我知道問題在哪了，真的好蠢www」或「問題到底在哪裡？><」");
        }
    }
}
