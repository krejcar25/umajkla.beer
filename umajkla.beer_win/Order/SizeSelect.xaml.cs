using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace beer.umajkla.win.Order
{
    /// <summary>
    /// Interakční logika pro SizeSelect.xaml
    /// </summary>
    /// 

    public delegate void SizeSelectedEventHandler(object sender, SizeSelectedEventArgs e);
   
    public class SizeSelectedEventArgs : EventArgs
    {
        public int Size { get; set; }

        public SizeSelectedEventArgs(int size)
        {
            Size = size;
        }
    }

    public partial class SizeSelect : Page
    {
        public event SizeSelectedEventHandler SizeSelected;
        public event EventHandler Cancelled;

        public SizeSelect(string todo, string name, int defaultValue, string featured1label, int featured1value, string featured2label, int featured2value)
        {
            InitializeComponent();
            
            Run header = new Run(string.Format("{0} - {1}", todo, name));
            header.FontSize = 18;
            item.Header = header;

            featured1text.Text = featured1label;
            featured1.Tag = featured1value;

            featured2text.Text = featured2label;
            featured2.Tag = featured2value;

            volume.Text = defaultValue.ToString();

        }

        private void featured1_Click(object sender, RoutedEventArgs e)
        {
            SizeSelected?.Invoke(this, new SizeSelectedEventArgs((int)featured1.Tag));
        }

        private void featured2_Click(object sender, RoutedEventArgs e)
        {
            SizeSelected?.Invoke(this, new SizeSelectedEventArgs((int)featured2.Tag));
        }

        private void editVolume_Click(object sender, RoutedEventArgs e)
        {
            int vol = int.Parse(volume.Text);

            switch (((RepeatButton)sender).Tag.ToString())
            {
                case "-1":
                    vol -= 1;
                    break;
                case "-10":
                    vol -= 10;
                    break;
                case "-100":
                    vol -= 100;
                    break;
                case "+100":
                    vol += 100;
                    break;
                case "+10":
                    vol += 10;
                    break;
                case "+1":
                    vol += 1;
                    break;
                default:
                    break;
            }

            volume.Text = vol.ToString();
        }

        private void confirm_Click(object sender, RoutedEventArgs e)
        {
            SizeSelected?.Invoke(this, new SizeSelectedEventArgs(int.Parse(volume.Text)));
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Cancelled?.Invoke(this, new EventArgs());
        }
    }
}
