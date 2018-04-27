using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace beer.umajkla.win.Order
{
    /// <summary>
    /// Interakční logika pro Basket.xaml
    /// </summary>

    public delegate void FinaliseOrderEventHandler(object sender, FinaliseOrderEventArgs e);

    public class FinaliseOrderEventArgs : EventArgs
    {
        public List<Transaction> CurrentOrder { get; set; }

        public FinaliseOrderEventArgs(List<Transaction> transactions)
        {
            CurrentOrder = transactions;
        }
    }

    public partial class Basket : Page
    {
        public List<Transaction> CurrentOrder { get; set; }
        public Dictionary<Guid,Item> Items { get; set; }
        public long TotalPrice { get; set; }
        public Customer CurrentCustomer { get; set; }

        public event FinaliseOrderEventHandler FinaliseOrder;

        public Basket(Dictionary<Guid, Item> items)
        {
            InitializeComponent();
            TotalPrice = 0;
            Items = items;
            CurrentOrder = new List<Transaction>();
            ListItems();
        }

        public void Add(Transaction transaction, Dictionary<Guid,Item> items)
        {
            CurrentOrder.Add(transaction);
            Items = items;
            TotalPrice += (transaction.Amount * Items[transaction.ItemId].Price * transaction.Multiplier) / 100000L;
            ListItems();
        }

        private void finishOrder_Click(object sender, RoutedEventArgs e)
        {
            FinaliseOrder?.Invoke(this, new FinaliseOrderEventArgs(CurrentOrder));
        }

        private void ListItems()
        {
            basketList.Children.RemoveRange(0, basketList.Children.Count);
            foreach (Transaction transaction in CurrentOrder)
            {
                TextBlock block = new TextBlock();
                int id = basketList.Children.Count;

                Run itemName = new Run(Items[transaction.ItemId].Name + " - " + (id + 1));
                itemName.FontWeight = FontWeights.Bold;
                itemName.FontSize = 20;

                Run volume = new Run(string.Format("     {0}{1}", transaction.Amount / 1000, Items[transaction.ItemId].Unit));
                volume.FontSize = 16;

                long price = (transaction.Amount * Items[transaction.ItemId].Price * transaction.Multiplier) / 100000L;
                Run priceText = new Run(string.Format("     {0} Kč", price / 100));
                priceText.FontSize = 16;

                block.Inlines.Add(itemName);
                block.Inlines.Add(new LineBreak());
                block.Inlines.Add(volume);
                block.Inlines.Add(new LineBreak());
                block.Inlines.Add(priceText);

                block.Tag = id;
                block.MouseRightButtonDown += list_Remove;

                basketList.Children.Add(block);
            }
            total.Text = string.Format("{0} Kč", Math.Round(TotalPrice / 100d, 0));

            string orders = JsonConvert.SerializeObject(CurrentOrder);
            File.WriteAllText("CurrentOrder.json", orders);
        }

        private void list_Remove(object sender, MouseButtonEventArgs e)
        {
            int id = int.Parse(((TextBlock)sender).Tag.ToString());
            if (MessageBox.Show(string.Format("Skutečně odebrat položku číslo {0}?", id + 1), "Ověření",
                MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                TotalPrice -= (CurrentOrder[id].Amount * Items[CurrentOrder[id].ItemId].Price * CurrentOrder[id].Multiplier) / 100000L;
                CurrentOrder.RemoveAt(id);
            }
            ListItems();
        }

        public void PayByAccount(Customer customer)
        {
            foreach (Transaction transaction in CurrentOrder)
            {
                transaction.CustomerId = customer.CustomerId;
                transaction.EventId = Guid.Parse("99662CCC-A951-4EB9-9FBF-73EB9992C4AE");
                Client.Run("transactions", "POST", "", string.Format("={0}", JsonConvert.SerializeObject(transaction)));
            }
        }

        public void PayNow()
        {
            Payment payment = new Payment()
            {
                Amount = int.Parse((Math.Round(TotalPrice / 100d, 0) * 100).ToString()),
                CustomerId = Guid.Empty,
                EventId = Guid.Parse("99662CCC-A951-4EB9-9FBF-73EB9992C4AE")
            };
            payment = JsonConvert.DeserializeObject<Payment>(Client.Run("payments", "POST", "", string.Format("={0}", JsonConvert.SerializeObject(payment))));

            string notes = "položky: ";
            foreach (Transaction transaction in CurrentOrder)
            {
                transaction.Notes = string.Format("platba: {0}", payment.PaymentId);
                transaction.EventId = Guid.Parse("99662CCC-A951-4EB9-9FBF-73EB9992C4AE");
                Transaction stored = JsonConvert.DeserializeObject<Transaction>(Client.Run("transactions", "POST", "", string.Format("={0}", JsonConvert.SerializeObject(transaction))));
                notes += stored.TransactionId + "\r\n";
            }

            payment.Notes = notes;
            Client.Run("payments", "PUT", "", string.Format("={0}", JsonConvert.SerializeObject(payment)));
        }

        public Basket Store(Customer customer)
        {
            CurrentCustomer = customer;
            return this;
        }
    }
}
