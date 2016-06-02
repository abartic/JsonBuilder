using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonGenerator.Base
{
    class BaseCoRoutine : IResult
    {
        private readonly System.Action action;

        public BaseCoRoutine(System.Action action)
        {
            this.action = action;
        }

        public void Execute(ActionExecutionContext context)
        {

        }

        public void Execute(CoroutineExecutionContext context)
        {
            action();
            Completed(this, new ResultCompletionEventArgs());
        }
        

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}
