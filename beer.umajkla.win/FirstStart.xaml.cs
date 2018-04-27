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
using System.Windows.Shapes;

namespace beer.umajkla.win
{
    /// <summary>
    /// Interakční logika pro FirstStart.xaml
    /// </summary>
    public partial class FirstStart : Window
    {
        public event EventHandler Selected;
        public string Value => address.Text;
        public FirstStart()
        {
            InitializeComponent();
        }

        private void submit(object sender, RoutedEventArgs e)
        {
            Close();
            Selected?.Invoke(this, new EventArgs());
        }
    }
}
