using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace message_handler.MathHelper
{
    public class VectorNormalizer : MyDialog<IMessageActivity>
    {
        protected override async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (message.Text.StartsWith("標準化"))
            {
                await context.PostAsync($"正在標準化...{message.Text.Substring(5)}");
                try
                {
                    var values = new List<double>(
                        await Task.WhenAll(message.Text.Substring(5).TrimStart('<').TrimEnd('>').Split(',')
                        .Select(async s => await Python.ExecutePython(Python.ProcessPythonCode(s, true)))
                        .Select(async s => double.Parse(await s)))
                        );
                    await context.PostAsync($"<{string.Join(", ", values)}>的標準化：");
                    double length = 0;
                    values.ForEach(v => length += v * v);
                    length = Math.Sqrt(length);
                    await context.PostAsync($"倍數：{length}<br/>標準化結果：<{string.Join(", ", values.Select(v => v / length))}>");
                }
                catch (TimeoutException) { await context.PostAsync("計算太久了，已中斷"); }
                catch (Exception error) { await context.PostAsync($"計算時發生問題：<br/>{error}"); }
                message = null;
            }
            context.Done(message);
        }
    }
}
