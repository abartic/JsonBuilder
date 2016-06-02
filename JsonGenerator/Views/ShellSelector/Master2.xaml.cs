using System;
using System.Collections.Generic;
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

namespace JsonGenerator.ShellSelector
{
    /// <summary>
    /// Interaction logic for ShellSelector.xaml
    /// </summary>
    public partial class Master2 : UserControl
    {
        public Master2()
        {
            InitializeComponent();
            
        }
     
        private void lstShells_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Canceled = true;
            
        }

        public Boolean Canceled { get; set; }
    }
}
