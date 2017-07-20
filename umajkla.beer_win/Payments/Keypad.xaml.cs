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

namespace umajkla.beer_win.Payments
{
    /// <summary>
    /// Interakční logika pro Keypad.xaml
    /// </summary>
    /// 
    public partial class Keypad : Page
    {
        public event EventHandler Typed;
        public event EventHandler Cancelled;

        public Keypad(string label)
        {
            InitializeComponent();
            this.label.Text = label;
        }

        private void digit_Click(object sender, RoutedEventArgs e)
        {
            string tag = (string)((Button)sender).Tag;
            if (tag == "c")
            {
                if (display.Text.Length == 0)
                {
                    Cancelled?.Invoke(this, new EventArgs());
                }
                else
                {
                    display.Text = display.Text.Remove(display.Text.Length - 1, 1);
                }
            }
            else if (tag == "enter")
            {
                Typed?.Invoke(this, new EventArgs());
            }
            else
            {
                display.Text += tag;
            }
        }
    }
}
