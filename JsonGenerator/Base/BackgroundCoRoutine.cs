using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonGenerator.Base
{
    public class BackgroundCoRoutine : IResult
    {
        private readonly System.Action action;

        public BackgroundCoRoutine(System.Action action)
        {
            this.action = action;
        }

        public void Execute(ActionExecutionContext context)
        {

        }

        public void Execute(CoroutineExecutionContext context)
        {
            using (var backgroundWorker = new BackgroundWorker())
            {
                backgroundWorker.DoWork += (e, sender) => action();
                backgroundWorker.RunWorkerCompleted += (e, sender) => Completed(this, new ResultCompletionEventArgs());
                backgroundWorker.RunWorkerAsync();
            }
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

    }
}
