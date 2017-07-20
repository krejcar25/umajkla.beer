using Newtonsoft.Json;
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

namespace umajkla.beer_win.Items
{
    /// <summary>
    /// Interakční logika pro ItemDetail.xaml
    /// </summary>
    /// 

    public delegate void KeypadRequestEventHandler(object sender, KeypadRequestEventArgs e);
    public class KeypadRequestEventArgs : EventArgs
    {
        public string Label { get; set; }
        public int Stage { get; set; }

        public KeypadRequestEventArgs(string label, int stage)
        {   
            Label = label;
            Stage = stage;
        }
    }
    public partial class ItemDetail : Page
    {
        public event EventHandler ResetView;
        public event KeypadRequestEventHandler KeypadRequest;

        public Dictionary<Guid, Customer> Customers { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Supply> Supplies { get; set; }
        public Item CurrentItem { get; set; }
        public long Balance { get; set; }
        public long Volume { get; set; }
        public bool NeedsSave { get; set; }
        public Supply NewSupply { get; set; }

        public ItemDetail(Item item)
        {
            InitializeComponent();
            CurrentItem = item;

            LoadDetails();
        }

        public void LoadDetails()
        {
            Customers = new Dictionary<Guid, Customer>();

            string json = Client.Run("customers", "GET", Client.eventId.ToString());
            List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(json);

            foreach (Customer customer in customers)
            {
                Customers.Add(customer.CustomerId, customer);
            }

            Transactions = JsonConvert.DeserializeObject<List<Transaction>>(Client.Run("transactions", "GET", string.Format("item={0}", CurrentItem.ItemId)));

            Supplies = JsonConvert.DeserializeObject<List<Supply>>(Client.Run("supplies", "GET", string.Format("item={0}", CurrentItem.ItemId)));

            long sold = 0;
            long soldFor = 0;
            long bought = 0;
            long boughtFor = 0;

            foreach (Transaction transaction in Transactions)
            {
                long amount = transaction.Amount;
                sold += amount;
                long price = CurrentItem.Price;
                long multiplier = transaction.Multiplier;
                soldFor += amount * price * multiplier / 100000;
            }
            foreach (Supply supply in Supplies)
            {
                bought += supply.Amount;
                boughtFor += supply.Price;
            }
            Volume = bought - sold;
            Balance = soldFor - boughtFor;



            nameBox.Text = CurrentItem.Name;
            priceBox.Text = (CurrentItem.Price / 100d).ToString();
            unitBox.Text = CurrentItem.Unit;
            notesBox.Text = CurrentItem.Notes;

            NeedsSave = false;
            saveLabel.Text = "Odejít";
            if (CurrentItem.ItemId != Guid.Empty)
            {
                if (Balance < 0)
                {
                    Run run = new Run(string.Format("Ztráta: {0} Kč", -Balance / 100L));
                    run.Foreground = Brushes.Red;

                    balanceDisplay.Inlines.Clear();
                    balanceDisplay.Inlines.Add(run);
                }
                else
                {
                    Run run = new Run(string.Format("Zisk: {0} Kč", Balance / 100L));
                    run.Foreground = Brushes.Black;

                    balanceDisplay.Inlines.Clear();
                    balanceDisplay.Inlines.Add(run);
                }

                if (Volume < 0)
                {
                    Run run = new Run(string.Format("Chybí: {0}{1}", -Volume / 1000d, CurrentItem.Unit));
                    run.Foreground = Brushes.Red;

                    volumeDisplay.Inlines.Clear();
                    volumeDisplay.Inlines.Add(run);
                }
                else
                {
                    Run run = new Run(string.Format("Zbývá: {0}{1}", Volume / 1000d, CurrentItem.Unit));
                    run.Foreground = Brushes.Black;

                    volumeDisplay.Inlines.Clear();
                    volumeDisplay.Inlines.Add(run);
                }

                createdBox.Text = "Vytvořen: " + CurrentItem.Created.ToString("dd.MM.yyyy hh:mm:ss");
                updatedBox.Text = "Upraven: " + CurrentItem.Updated.ToString("dd.MM.yyyy hh:mm:ss");
                idBox.Text = CurrentItem.ItemId.ToString();

                saveLabel.Text = "Odejít";

                transactionsGrid.ItemsSource = MainWindow.TransactionsDisplay.GenerateList(Transactions, CurrentItem);
                suppliesGrid.ItemsSource = MainWindow.SuppliesDisplay.GenerateList(Supplies,CurrentItem);
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
            CurrentItem.Name = nameBox.Text;
            CurrentItem.Price = int.Parse(priceBox.Text) * 100;
            CurrentItem.Unit = unitBox.Text;
            CurrentItem.Notes = notesBox.Text;
            CurrentItem.EventId = Client.eventId;

            if (CurrentItem.ItemId == Guid.Empty)
            {
                Client.Run("items", "POST", "", string.Format("={0}", JsonConvert.SerializeObject(CurrentItem)));
            }
            else
            {
                Client.Run("items", "PUT", "", string.Format("={0}", JsonConvert.SerializeObject(CurrentItem)));
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

        private void suppliesGrid_removeSupply(object sender, RoutedEventArgs e)
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

            if (MessageBox.Show("Chcete vybranou dodávku opravdu smazat?", "Ověření", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Client.Run("supplies", "DELETE", id.ToString());
                LoadDetails();
            }
        }

        private void CreateSupply_Click(object sender, RoutedEventArgs e)
        {
            KeypadRequest?.Invoke(this, new KeypadRequestEventArgs(string.Format("Množství dodávky pro {0} v {1}", CurrentItem.Name, CurrentItem.Unit), 0));
        }

        public void AmountKeypad_Typed(object sender, EventArgs e)
        {
            NewSupply = new Supply();
            NewSupply.ItemId = CurrentItem.ItemId;
            NewSupply.Amount = int.Parse(((Payments.Keypad)sender).display.Text) * 1000;

            KeypadRequest?.Invoke(this, new KeypadRequestEventArgs(string.Format("Cena dodávky pro {0} v Kč", CurrentItem.Name), 1));
        }

        public void PriceKeypad_Typed(object sender, EventArgs e)
        {
            string notes = Microsoft.VisualBasic.Interaction.InputBox("Přidat poznámku?", "Nová dodávka");
            NewSupply.Price = int.Parse(((Payments.Keypad)sender).display.Text) * 100;
            NewSupply.Notes = notes;
            NewSupply.EventId = Client.eventId;

            Client.Run("supplies", "POST", "", string.Format("={0}", JsonConvert.SerializeObject(NewSupply)));
            LoadDetails();
        }
    }
}
