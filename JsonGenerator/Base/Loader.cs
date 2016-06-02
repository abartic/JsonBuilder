//using Caliburn.Micro;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using Xceed.Wpf.Toolkit;

//namespace JsonGenerator.Base
//{

//    public class Loader : IResult
//    {
        
//        readonly bool hide;
//        readonly IEventAggregator eventAggregator;

//        public Loader(IEventAggregator eventAggregator)
//        {
//            this.eventAggregator = eventAggregator;
//        }

//        public Loader(bool hide)
//        {
//            this.hide = hide;
//        }

//        public void Execute(CoroutineExecutionContext context)
//        {
//            var view = context.View as FrameworkElement;
//            while (view != null)
//            {
//                var busyIndicator = view as BusyIndicator;
//                if (busyIndicator != null)
//                {
//                    if (!string.IsNullOrEmpty(message))
//                        busyIndicator.BusyContent = message;
//                    busyIndicator.IsBusy = !hide;
//                    break;
//                }

//                view = view.Parent as FrameworkElement;
//            }

//            Completed(this, new ResultCompletionEventArgs());
//        }

//        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

//        public static IResult Show(string message = null)
//        {
//            return new Loader(message);
//        }

//        public static IResult Hide()
//        {
//            return new Loader(true);
//        }
//    }
//}
