using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JsonGenerator
{
    /// <summary>
    /// Interaction logic for JsonBuilderView.xaml
    /// </summary>
    public partial class JsonBuilderView : UserControl, IHandle<Object>
    {
        
        [ImportingConstructor]
        public JsonBuilderView()
        {
            InitializeComponent();

            
            this.DataContextChanged += JsonBuilderView_DataContextChanged;
        }

        private void JsonBuilderView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if (DataContext is JsonBuilderViewModel)
            //    grdcType.ItemsSource = (DataContext as JsonBuilderViewModel).ShellDependencies;
        }

        public void Handle(Object message)
        {
            ;
        }

        private void trwRelations_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            trwRelations.BringIntoView();
        }

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        //private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    txtShellCode.SelectAll();
        //}
    }
}
