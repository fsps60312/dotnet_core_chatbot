using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace message_handler
{
    public abstract class MyDialog<T>
    {
        public static async Task PrintBug(IDialogContext context, string bug)
        {
            await context.PostAsync("吼～崩潰啦，都是你害的><");
            await Task.Delay(2000);
            await context.PostAsync("為了報復你，我要叫你幫我debug哇哈哈哈OwO");
            await Task.Delay(3000);
            await context.PostAsync(bug);
        }
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedPrivateAsync);
        }
        protected abstract Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument);
        private async Task MessageReceivedPrivateAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            try
            {
                await MessageReceivedAsync(context, argument);
            }
            catch (Exception error)
            {
                await PrintBug(context, error.ToString());
            }
        }
    }
}
