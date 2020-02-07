using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IronPython;

namespace message_handler.MathHelper
{
    public class MathHelper : MyDialog<IMessageActivity>
    {
        async Task ResumeAfterPython(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            context.Done(message);
        }
        async Task ResumeAfterWalframAlpha(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (message == null) context.Done(message);
            else await context.Forward(new Python(), ResumeAfterPython, message);
        }
        async Task ResumeAfterVectorNormalizer(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (message == null) context.Done(message);
            else await context.Forward(new WolframAlpha(), ResumeAfterWalframAlpha, message);
        }
        protected override async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            await context.Forward(new VectorNormalizer(), ResumeAfterVectorNormalizer, message);
        }
    }
}
