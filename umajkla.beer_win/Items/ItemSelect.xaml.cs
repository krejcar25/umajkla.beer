using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Collections.ObjectModel;

namespace umajkla.beer_win.Items
{
    /// <summary>
    /// Interakční logika pro ItemSelect.xaml
    /// </summary>
    /// 

    public delegate void ItemSelectedEventHandler(object sender, ItemSelectedEventArgs e);
    
    public class ItemSelectedEventArgs : EventArgs
    {
        public Item Item { get; set; }

        public ItemSelectedEventArgs(Item item)
        {
            Item = item;
        }
    }

    public partial class ItemSelect : Page
    {
        public event ItemSelectedEventHandler ItemSelected;
        public Dictionary<Guid,Item> Items { get; set; }

        public ItemSelect(Dictionary<Guid,Item> items, bool showEmpty = false, bool onlyInStock = true) 
        { 
            InitializeComponent();

            Items = items;

            int column = 1;

            if (showEmpty)
            {
                Button button = new Button();
                Viewbox box = new Viewbox();
                TextBlock block = new TextBlock();

                block.Text = "Nové zboží";
                block.FontWeight = FontWeights.Bold;

                box.Stretch = Stretch.Uniform;
                box.Child = block;

                button.Content = box;
                button.Tag = Guid.Empty;
                button.Click += itemButton_Click;
                button.Height = 80;
                button.Margin = new Thickness(10);

                list1.Children.Add(button);
                column = 2;
            }

            foreach (KeyValuePair<Guid,Item> item in items)
            {
                long amount = 0;
                if (onlyInStock)
                {
                    List<Transaction> transactions = JsonConvert.DeserializeObject<List<Transaction>>(Client.Run("transactions", "GET", string.Format("item={0}", item.Value.ItemId)));
                    List<Supply> supplies = JsonConvert.DeserializeObject<List<Supply>>(Client.Run("supplies", "GET", string.Format("item={0}", item.Value.ItemId)));

                    foreach (Transaction transaction in transactions)
                    {
                        amount -= transaction.Amount;
                    }

                    foreach (Supply supply in supplies)
                    {
                        amount += supply.Amount;
                    }
                }

                Button button = new Button();
                Viewbox box = new Viewbox();
                TextBlock block = new TextBlock();

                block.Text = item.Value.Name;
                block.FontWeight = FontWeights.Bold;

                box.Stretch = Stretch.Uniform;
                box.Child = block;

                button.Content = box;
                button.Tag = item.Value.ItemId;
                button.Click += itemButton_Click;
                button.Height = 80;
                button.Margin = new Thickness(10);

                if (onlyInStock && amount <= 0)
                {
                    return;
                }

                if (column == 1)
                {
                    list1.Children.Add(button);
                    column = 2;
                }
                else if (column == 2)
                {
                    list2.Children.Add(button);
                    column = 3;
                }
                else
                {
                    list3.Children.Add(button);
                    column = 1;
                }
            }


        }

        private void itemButton_Click(object sender, RoutedEventArgs e)
        {
            Guid id = Guid.Parse(((Button)sender).Tag.ToString());
            if (id == Guid.Empty)
            {
                ItemSelected?.Invoke(this, new ItemSelectedEventArgs(new Item()));
            }
            else
            {
                ItemSelected?.Invoke(this, new ItemSelectedEventArgs(Items[id]));
            }
        }
    }
}
