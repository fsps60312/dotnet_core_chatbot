using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace message_handler
{
    public class IAwaitable<T>:Task<T>
    {
    }
    public interface Foo1
    {
        string Id { get; }
        string Name { get; }
        Dictionary<string, Newtonsoft.Json.Linq.JToken> Properties { get; }
    }
    public interface IMessageActivity
    {
        string Text { get; }
        Foo1 From { get; }
    }
    public interface IDialogContext
    {
        Task PostAsync(string msg);
        void Wait(Func<IDialogContext,IAwaitable<IMessageActivity>,Task> task);
        void Done<T>(T msg);
        Task Forward<T>(MyDialog<T> a, Func<IDialogContext, IAwaitable<IMessageActivity>, Task> resume, IMessageActivity message);
    }
}
