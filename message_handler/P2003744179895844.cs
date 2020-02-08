using System;
namespace message_handler
{
    [Serializable]
    class P2003744179895844 :DialogNode
    {
        void Stage1()
        {
            switch (sender_msg.ToLower())
            {
                case "我知道問題在哪了，真的好蠢www": SendMsg("對呀真的好蠢哈哈XD"); break;
                case "問題到底在哪裡？><":
                    SendMsg("C#的Dictionary在Add一個已存在的key時會跳Exception，然後小莫給Dictionary初始化用的Initializer List藏了兩個相同的key！！（仔細找，有兩個「不好說」！XXD）\n" +
      "所以在物件初始化的時候就crash了，然後在debug的時候完全不覺得new這個物件哪裡會有問題XD\n" +
      "總之，這個bug總算被我抓到啦～耶～"); break;
                default:
                    try { throw new NotImplementedException(); }
                    catch (Exception error) { Bug(error.ToString()); break; }
            }
            EndDialog(new DialogEntry());
        }
        void Stage0()
        {
            SendImage("https://1.bp.blogspot.com/-Pl5CWtj-KrQ/WjVLr97yhcI/AAAAAAAAKEk/u9QQIa998HEF4_f8WR15_eV8_6m7rsdZwCLcBGAs/s1600/Screenshot%2B%2528525%2529.png");
            SendMsg("其實是這一段code出問題啦，您找到問題在哪裡了嗎？\n當然要發現問題出在這一段code也是花了好一番功夫啦Orz\n然後其實圖片中的code是縮減版本，小莫在debug的時候這段code有25行那麼長～");
            SendButtons("請問您的狀況？", "請輸入「我知道問題在哪了，真的好蠢www」或「問題到底在哪裡？><」", new[] { "我知道問題在哪了，真的好蠢www", "問題到底在哪裡？><" });
            stage = Stage.Stage1;
            EndDialog(this);
        }
        enum Stage { Stage0,Stage1};
        Stage stage = Stage.Stage0;
        public override void Run()
        {
            switch (stage)
            {
                case Stage.Stage0:Stage0();return;
                case Stage.Stage1:Stage1();return;
                default:EndDialog(new DialogEntry());return;
            }
        }
    }
}
