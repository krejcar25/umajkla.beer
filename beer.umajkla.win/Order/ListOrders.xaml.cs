using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interakční logika pro ListOrders.xaml
    /// </summary>
    public partial class ListOrders : Page
    {
        public Button newOrder = new Button();
        public List<KeyValuePair<Customer, List<Transaction>>> Baskets { get; set; }
        public Dictionary<Guid, Item> Items { get; set; }
        public ListOrders(List<KeyValuePair<Customer, List<Transaction>>> baskets, Dictionary<Guid,Item> items)
        {
            InitializeComponent();

            Baskets = baskets;
            Items = items;

            LoadDisplay();
        }

        private void LoadDisplay()
        {
            newOrder.Content = new Viewbox() { Child = new TextBlock(new Run("Nová objednávka") { FontWeight = FontWeights.Bold }) };
            newOrder.Margin = new Thickness(10);
            newOrder.Height = 50;
            list1.Children.Clear();
            list2.Children.Clear();
            list3.Children.Clear();
            list1.Children.Add(newOrder);

            int column = 2;
            for (int i = 0; i < Baskets.Count; i++)
            {
                StackPanel panel = new StackPanel();
                panel.Margin = new Thickness(10);
                panel.Background = new SolidColorBrush(new Color() { A = 127, R = 160, G = 160, B = 160 });
                panel.Children.Add(new Viewbox() { Child = new TextBlock(new Run() { Text = Baskets[i].Key.Name, FontWeight = FontWeights.Bold }), Height = 30 });

                foreach (Transaction transaction in Baskets[i].Value)
                {
                    TextBlock block = new TextBlock();
                    block.Inlines.Add(new Run()
                    {
                        Text = Items[transaction.ItemId].Name, FontWeight = FontWeights.Bold
                    });
                    block.Inlines.Add(new Run()
                    {
                        Text = string.Format(": {0}{1}", transaction.Amount / 1000d, Items[transaction.ItemId].Unit)
                    });
                    BulletDecorator bullet = new BulletDecorator()
                    {
                        Bullet = new Ellipse() { Height = 10, Width = 10, Fill = Brushes.Black },
                        Child = block
                    };
                    panel.Children.Add(bullet);
                }

                Button complete = new Button();
                complete.Content = new Viewbox() { Child = new TextBlock(new Run("Vydat")) };
                complete.Margin = new Thickness(10);
                complete.Height = 30;
                complete.Click += Complete_Click;
                complete.Tag = i;

                panel.Children.Add(complete);

                panel.Tag = i;
                panel.MouseRightButtonDown += Panel_MouseRightButtonDown;

                if (column == 1)
                {
                    list1.Children.Add(panel);
                    column++;
                }
                else if (column == 2)
                {
                    list2.Children.Add(panel);
                    column++;
                }
                else if (column == 3)
                {
                    list3.Children.Add(panel);
                    column = 1;
                }
            }
        }

        private void Panel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Obravdu chcete zahodit tuto objednávku?", "Ověření", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Baskets.RemoveAt((int)((StackPanel)sender).Tag);
            LoadDisplay();
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            int index = (int)((Button)sender).Tag;
            Basket basket = new Basket(Items) { CurrentOrder = Baskets[index].Value };
            if (Baskets[index].Key.CustomerId == Guid.Empty)
            {
                if (MessageBox.Show(string.Format("Budou vytvořeny výdeje {0} zboží a celková platba ve výši {1} Kč", basket.CurrentOrder.Count, basket.TotalPrice / 100d), "Potvrzení", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    basket.PayNow();
                }
                else return;
            }
            else
            {
                if (MessageBox.Show(string.Format("Budou vytovřeny výdeje {0} zboží na účet {1}", basket.CurrentOrder.Count, Baskets[index].Key.Name), "Potvrzení", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    basket.PayByAccount(Baskets[index].Key);
                }
                else return;
            }
            File.WriteAllText("current_preorders.json", JsonConvert.SerializeObject(Baskets, Formatting.Indented));
            Baskets.RemoveAt(index);
            LoadDisplay();
        }
    }
}
