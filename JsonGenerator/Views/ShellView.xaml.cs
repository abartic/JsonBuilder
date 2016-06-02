using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Odbc;
using System.Data;
using Npgsql;
using Caliburn.Micro;

namespace JsonGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellView : UserControl //, IHandle<Object>
    {
        

        public ShellView()
        {
            InitializeComponent();
            //IoC.Get<IEventAggregator>().Subscribe(this);
        }


        //public void Handle(Object message)
        //{
        //    ;
        //}



        //public Boolean IsLoadingShowed
        //{
        //    get { return (Boolean)GetValue(IsLoadingShowedProperty); }
        //    set { SetValue(IsLoadingShowedProperty, value); }
        //}

        //public static readonly DependencyProperty IsLoadingShowedProperty =
        //    DependencyProperty.Register("IsLoadingShowed", typeof(Boolean), typeof(ShellView), new PropertyMetadata(false, new PropertyChangedCallback(OnIsLoadingShowedPropertyChanged)));

        //private static void OnIsLoadingShowedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        //{
        //    ;
        //}
    }
    
   
}
