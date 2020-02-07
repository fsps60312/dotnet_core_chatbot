using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace message_handler
{
    public class BasicJudge : MyDialog<IMessageActivity>
    {
        protected override async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (message.Text == null)
            {
                await context.PostAsync("吼～都這樣，不說點話嗎？><");
                message = null;
            }
            else if (message.Text.Length > 250)
            {
                await context.PostAsync("???");
                message = null;
            }
            context.Done(message);
        }
    }
}
