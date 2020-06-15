using System;
using System.Collections.Generic;
using System.Text;

namespace message_handler
{
    [Serializable]
    class DialogEntry : DialogNode
    {
        public override void Run()
        {
            base.Run();
            new UrlReactor().Run();
            new WolframAlpha().Run();
            new Bash().Run();
            new 幾A幾B().Run();
            new StatelessDialog().Run();
            FinalDialog.Read().Run();
        }
    }
}
