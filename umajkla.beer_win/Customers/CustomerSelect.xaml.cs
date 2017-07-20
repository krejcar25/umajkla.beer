using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace umajkla.beer_win.Customers
{
    /// <summary>
    /// Interakční logika pro CustomerSelect.xaml
    /// </summary>

    public delegate void CustomerSelectedEventhandler(object sender, Customer e);

    public class CustomerSelectedEventArgs : EventArgs
    {
        public Customer Customer { get; set; }

        public CustomerSelectedEventArgs(Customer customer)
        {
            Customer = customer;
        }
    }

    public partial class CustomerSelect : Page
    {
        public event CustomerSelectedEventhandler CustomerSelected;

        public Dictionary<Guid,Customer> Customers { get; set; }

        public CustomerSelect(bool showEmpty = false, string emptyMessage = "")
        {
            InitializeComponent();
            ListCustomers();

            int column = 1;

            if (showEmpty)
            {
                Button button = new Button();
                Viewbox box = new Viewbox();
                TextBlock block = new TextBlock();
                block.Text = emptyMessage;
                block.FontWeight = FontWeights.Bold;

                box.Stretch = Stretch.Uniform;
                box.Child = block;

                button.Content = box;
                button.Tag = Guid.Empty;
                button.Click += customerButton_Click;
                button.Height = 80;
                button.Margin = new Thickness(10);

                list1.Children.Add(button);
                column = 2;
            }

            foreach (KeyValuePair<Guid, Customer> customer in Customers)
            {
                Button button = new Button();
                Viewbox box = new Viewbox();
                TextBlock block = new TextBlock();

                block.Text = customer.Value.Name;
                block.FontWeight = FontWeights.Bold;

                box.Stretch = Stretch.Uniform;
                box.Child = block;

                button.Content = box;
                button.Tag = customer.Value.CustomerId;
                button.Click += customerButton_Click;
                button.Height = 80;
                button.Margin = new Thickness(10);

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

        private void customerButton_Click(object sender, RoutedEventArgs e)
        {
            Guid id = Guid.Parse(((Button)sender).Tag.ToString());
            if (id == Guid.Empty)
            {
                CustomerSelected?.Invoke(this, new Customer());
            }
            else
            {
                CustomerSelected?.Invoke(this, Customers[id]);
            }
        }

        private void ListCustomers()
        {
            Customers = new Dictionary<Guid, Customer>();

            string json = Client.Run("customers", "GET", Client.eventId.ToString());
            List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(json);

            foreach (Customer customer in customers)
            {
                Customers.Add(customer.CustomerId, customer);
            }
        }
    }
}
