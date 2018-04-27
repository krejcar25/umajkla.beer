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

namespace beer.umajkla.win.Customers
{
    /// <summary>
    /// Interakční logika pro CustomerDetail.xaml
    /// </summary>
    public partial class CustomerDetail : Page
    {
        public event EventHandler ResetView;
        public event EventHandler KeypadRequest;

        public Dictionary<Guid,Item> Items { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Payment> Payments { get; set; }
        public Customer CurrentCustomer { get; set; }
        public long Balance { get; set; }
        public bool NeedsSave { get; set; }

        public CustomerDetail(Customer customer)
        {
            InitializeComponent();
            CurrentCustomer = customer;

            LoadDetails();
        }

        public void LoadDetails()
        {
            Items = new Dictionary<Guid, Item>();

            string json = Client.Run("items", "GET", Client.eventId.ToString());
            List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);

            foreach (Item item in items)
            {
                Items.Add(item.ItemId, item);
            }

            Transactions = JsonConvert.DeserializeObject<List<Transaction>>(Client.Run("transactions", "GET", string.Format("customer={0}", CurrentCustomer.CustomerId)));

            Payments = JsonConvert.DeserializeObject<List<Payment>>(Client.Run("payments", "GET", string.Format("customer={0}", CurrentCustomer.CustomerId)));

            long spent = 0;
            long paid = 0;

            foreach (Transaction transaction in Transactions)
            {
                long amount = transaction.Amount;
                long price = Items[transaction.ItemId].Price;
                long multiplier = transaction.Multiplier;
                spent += amount * price * multiplier / 100000;
            }
            foreach (Payment payment in Payments)
            {
                paid += payment.Amount;
            }
            Balance = paid - spent;

            

            nameBox.Text = CurrentCustomer.Name;
            emailBox.Text = CurrentCustomer.Email;
            phoneBox.Text = CurrentCustomer.Phone;
            addressBox.Text = CurrentCustomer.Address;
            notesBox.Text = CurrentCustomer.Notes;

            NeedsSave = false;
            saveLabel.Text = "Odejít";
            if (CurrentCustomer.CustomerId != Guid.Empty)
            {
                if (Balance < 0)
                {
                    Run run = new Run(string.Format("Dluh: {0} Kč", -Balance / 100));
                    run.Foreground = Brushes.Red;

                    balanceDisplay.Inlines.Clear();
                    balanceDisplay.Inlines.Add(run);

                    deleteButton.Visibility = Visibility.Collapsed;
                    PayAll.Visibility = Visibility.Visible;
                    PayAll.Tag = -Balance;
                    payAllButtonLabel.Text = string.Format("Zaplatit vše ({0} Kč)", -Balance / 100);
                }
                else
                {
                    Run run = new Run(string.Format("Kredit: {0} Kč", Balance / 100));
                    run.Foreground = Brushes.Black;

                    balanceDisplay.Inlines.Clear();
                    balanceDisplay.Inlines.Add(run);

                    deleteButton.Visibility = Visibility.Visible;
                    PayAll.Visibility = Visibility.Collapsed;
                }
                createdBox.Text = "Vytvořen: " + CurrentCustomer.Created.ToString("dd.MM.yyyy hh:mm:ss");
                updatedBox.Text = "Upraven: " + CurrentCustomer.Updated.ToString("dd.MM.yyyy hh:mm:ss");
                idBox.Text = CurrentCustomer.CustomerId.ToString();

                saveLabel.Text = "Odejít";

                transactionsGrid.ItemsSource = MainWindow.TransactionsDisplay.GenerateList(Transactions, Items);
                paymentsGrid.ItemsSource = MainWindow.PaymentsDisplay.GenerateList(Payments, Items);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!NeedsSave)
            {
                ResetView?.Invoke(this, new EventArgs());
            }
            else
            {
                SaveCurrent();
            }
        }

        private void SaveCurrent()
        {
            CurrentCustomer.Name = nameBox.Text;
            CurrentCustomer.Email = emailBox.Text;
            CurrentCustomer.Phone = phoneBox.Text;
            CurrentCustomer.Address = addressBox.Text;
            CurrentCustomer.Notes = notesBox.Text;
            CurrentCustomer.EventId = Client.eventId;

            if (CurrentCustomer.CustomerId == Guid.Empty)
            {
                Client.Run("customers", "POST", "", string.Format("={0}", JsonConvert.SerializeObject(CurrentCustomer)));
            }
            else
            {
                Client.Run("customers", "PUT", "", string.Format("={0}", JsonConvert.SerializeObject(CurrentCustomer)));
            }
            

            ResetView?.Invoke(this, new EventArgs());
        }

        private void madeEdit(object sender, TextChangedEventArgs e)
        {
            NeedsSave = true;
            saveLabel.Text = "Uložit a odejít";
        }

        private void transactionsGrid_RemoveTransaction(object sender, RoutedEventArgs e)
        {
            Guid id = Guid.Empty;
            try
            {
                id = (Guid)((Button)sender).Tag;
            }
            catch (NullReferenceException)
            {
                return;
            }

            if (MessageBox.Show("Chcete vybranou transakci opravdu smazat?", "Ověření", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Client.Run("transactions", "DELETE", id.ToString());
                LoadDetails();
            }
        }

        private void paymentsGrid_RemovePayment(object sender, RoutedEventArgs e)
        {
            Guid id = Guid.Empty;
            try
            {
                id = (Guid)((Button)sender).Tag;
            }
            catch (NullReferenceException)
            {
                return;
            }

            if (MessageBox.Show("Chcete vybranou platbu opravdu smazat?", "Ověření", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Client.Run("payments", "DELETE", id.ToString());
                LoadDetails();
            }
        }

        private void CreatePayment_Click(object sender, RoutedEventArgs e)
        {
            KeypadRequest?.Invoke(this, new EventArgs());
        }

        public void CreditKeypad_Typed(object sender, EventArgs e)
        {
            string notes = Microsoft.VisualBasic.Interaction.InputBox("Přidat poznámku?", "Nová platba");
            Payment payment = new Payment();
            payment.CustomerId = CurrentCustomer.CustomerId;
            payment.Amount = int.Parse(((Payments.Keypad)sender).display.Text) * 100;
            payment.Notes = notes;
            payment.EventId = Client.eventId;

            Client.Run("payments", "POST", "", string.Format("={0}", JsonConvert.SerializeObject(payment)));
            LoadDetails();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ResetView?.Invoke(this, new EventArgs());
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Opravdu chcete tohoto zákazníka odstranit?", "Ověření", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                List<Transaction> transactions = JsonConvert.DeserializeObject<List<Transaction>>(Client.Run("transactions", "GET", string.Format("customer={0}", CurrentCustomer.CustomerId)));
                List<Payment> payments = JsonConvert.DeserializeObject<List<Payment>>(Client.Run("payments", "GET", string.Format("customer={0}", CurrentCustomer.CustomerId)));

                long spent = 0;
                foreach (Transaction transaction in transactions)
                {
                    transaction.CustomerId = Guid.Empty;
                    transaction.Notes += string.Format("převedeno z účtu {0} při smazání {1}", CurrentCustomer.Name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    spent += (transaction.Amount * Items[transaction.ItemId].Price * transaction.Multiplier) / 100000;
                    Client.Run("transactions", "PUT", "", string.Format("={0}", JsonConvert.SerializeObject(transaction)));
                }

                Client.Run("payments", "POST", "", "=" + JsonConvert.SerializeObject(new Payment { CustomerId = Guid.Empty, Amount = spent, Notes = string.Format("vytvořena při mazání účtu {0} {1}", CurrentCustomer.Name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), EventId = Client.eventId }));

                long paid = 0;
                foreach (Payment payment in payments)
                {
                    paid += payment.Amount;
                    Client.Run("payments", "DELETE", payment.PaymentId.ToString());
                }

                Client.Run("customers", "DELETE", CurrentCustomer.CustomerId.ToString());

                if (paid - spent > 0) MessageBox.Show(string.Format("Zákazníkovi vrátit {0} Kč kreditu", (paid - spent) / 100d), "Smazání zákazníka", MessageBoxButton.OK, MessageBoxImage.Information);

                ResetView?.Invoke(this, new EventArgs());
            }
        }

        private void PayAll_Click(object sender, RoutedEventArgs e)
        {
            long toPay = (long)((Button)sender).Tag;
            if (MessageBox.Show(string.Format("Opravdu chcete zaplatit celý účet zákazníka? Jedná se o částku {0} Kč.", toPay/100),"Nová platba", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Payment payment = new Payment() { Amount = toPay, CustomerId = CurrentCustomer.CustomerId, EventId = Client.eventId };
                Client.Run("payments", "POST", json: "=" + JsonConvert.SerializeObject(payment));
                LoadDetails();
            }
        }
    }
}
