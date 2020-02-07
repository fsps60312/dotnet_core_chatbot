using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace message_handler.Posts
{
    public class P2002954469974815 : MyDialog<IMessageActivity>
    {
        protected override async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            await context.PostAsync("哼，證明題是吧？您傳錯篇網址囉！ :p <br/>請再仔細看那篇文～");
            context.Done(message = null);
        }
    }
}
