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
    public partial class TreeShellsView : UserControl 
    {
        

        public TreeShellsView()
        {
            InitializeComponent();
            
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
    
   
}
