namespace message_handler
{
    class P2002954469974815 : DialogNode
    {
        public override void Run()
        {
            SendMsg("哼，證明題是吧？您傳錯篇網址囉！ :p \n請再仔細看那篇文～");
            EndDialog(new DialogEntry());
        }
    }
}
