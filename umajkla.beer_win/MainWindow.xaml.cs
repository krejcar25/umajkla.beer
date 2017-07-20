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
using System.Windows.Threading;

namespace umajkla.beer_win
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Client.WorkStarted += Client_WorkStarted;
            Client.WorkDone += Client_WorkDone;

            events.IsSelected = true;
        }

        private void LoadWindow()
        {
            this.Refresh();

            NewOrder();
            orderTab.Visibility = Visibility.Visible;
            CustomersInit();
            customers.Visibility = Visibility.Visible;
            LoadItemEditor();

            items.Visibility = Visibility.Visible;
            orders_payments.Visibility = Visibility.Visible;
            supplies.Visibility = Visibility.Visible;
            cashflow.Visibility = Visibility.Visible;
            stats.Visibility = Visibility.Visible;
            orderTab.IsSelected = true;
        }

        private void Client_WorkDone(object sender, EventArgs e)
        {
            workingOverlay.Visibility = Visibility.Collapsed;
            this.Refresh();
        }

        private void Client_WorkStarted(object sender, EventArgs e)
        {
            workingOverlay.Visibility = Visibility.Visible;
            this.Refresh();
        }

        #region Display Classes
        public class TransactionsDisplay
        {
            public Transaction Transaction { get; set; }
            public Customer Customer { get; set; }
            public Item Item { get; set; }
            
            public string Details => string.Format("{0}l × {1} × {2} Kč",
                        (double)Transaction.Amount / 1000,
                        (double)Transaction.Multiplier / 100,
                        (double)Item.Price / 100);
            public string Price => (Transaction.Amount * Transaction.Multiplier * Item.Price / 10000000L).ToString() + " Kč";
            public string Volume => (Transaction.Amount / 1000d).ToString() + Item.Unit;

            public static List<TransactionsDisplay> GenerateList(List<Transaction> transactions, Dictionary<Guid, Item> items, bool addCustomersName = false)
            {
                List<TransactionsDisplay> list = new List<TransactionsDisplay>();
                Dictionary<Guid, Customer> customers = new Dictionary<Guid, Customer>();
                if (addCustomersName)
                {
                    List<Customer> customersList;
                    string json = Client.Run("customers", "GET", Client.eventId.ToString());
                    customersList = JsonConvert.DeserializeObject<List<Customer>>(json);
                    foreach (Customer customer in customersList)
                    {
                        customers.Add(customer.CustomerId, customer);
                    }

                    Customer customerNull = new Customer();
                    customerNull.Name = "Pouliční prodej";
                    customers.Add(Guid.Empty, customerNull);
                }
                foreach (Transaction transaction in transactions)
                {
                    TransactionsDisplay display = new TransactionsDisplay();
                    display.Transaction = transaction;
                    display.Item = items[transaction.ItemId];
                    if (addCustomersName) display.Customer = customers[transaction.CustomerId];
                    list.Add(display);
                }
                return list;
            }

            public static List<TransactionsDisplay> GenerateList(List<Transaction> transactions, Item item)
            {
                List<TransactionsDisplay> list = new List<TransactionsDisplay>();
                Dictionary<Guid, Customer> customers = new Dictionary<Guid, Customer>();
                List<Customer> customersList;
                string json = Client.Run("customers", "GET", Client.eventId.ToString());
                customersList = JsonConvert.DeserializeObject<List<Customer>>(json);
                foreach (Customer customer in customersList)
                {
                    customers.Add(customer.CustomerId, customer);
                }

                Customer customerNull = new Customer();
                customerNull.Name = "Pouliční prodej";
                customers.Add(Guid.Empty, customerNull);
                foreach (Transaction transaction in transactions)
                {
                    TransactionsDisplay display = new TransactionsDisplay();
                    display.Transaction = transaction;
                    display.Item = item;
                    display.Customer = customers[transaction.CustomerId];
                    list.Add(display);
                }
                return list;
            }
        }
        public class PaymentsDisplay
        {
            public Payment Payment { get; set; }
            public Customer Customer { get; set; }
            
            public string Amount => string.Format("{0} Kč", (double)Payment.Amount / 100);

            public static List<PaymentsDisplay> GenerateList(List<Payment> payments, Dictionary<Guid, Item> items, bool addCustomersName = false)
            {
                List<PaymentsDisplay> list = new List<PaymentsDisplay>();
                Dictionary<Guid, Customer> customers = new Dictionary<Guid, Customer>();
                if (addCustomersName)
                {
                    List<Customer> customersList;
                    string json = Client.Run("customers", "GET", Client.eventId.ToString());
                    customersList = JsonConvert.DeserializeObject<List<Customer>>(json);
                    foreach (Customer customer in customersList)
                    {
                        customers.Add(customer.CustomerId, customer);
                    }

                    Customer customerNull = new Customer();
                    customerNull.Name = "";
                    customers.Add(Guid.Empty, customerNull);
                }
                foreach (Payment payment in payments)
                {
                    PaymentsDisplay display = new PaymentsDisplay();
                    display.Payment = payment;
                    if (addCustomersName) display.Customer = customers[payment.CustomerId];
                    list.Add(display);
                }
                return list;
            }
        }
        public class SuppliesDisplay
        {
            public Supply Supply { get; set; }
            public Item Item { get; set; }

            public string Volume => (Supply.Amount / 1000d).ToString() + Item.Unit;
            public string Price => Math.Round(Supply.Price / 100d,0) + " Kč";

            public static List<SuppliesDisplay> GenerateList(List<Supply> supplies, Dictionary<Guid, Item> items)
            {
                List<SuppliesDisplay> list = new List<SuppliesDisplay>();
                foreach (Supply supply in supplies)
                {
                    SuppliesDisplay display = new SuppliesDisplay();
                    display.Supply = supply;
                    display.Item = items[supply.ItemId];
                    list.Add(display);
                }
                return list;
            }
            
            public static List<SuppliesDisplay> GenerateList(List<Supply> supplies, Item item)
            {
                List<SuppliesDisplay> list = new List<SuppliesDisplay>();
                foreach (Supply supply in supplies)
                {
                    SuppliesDisplay display = new SuppliesDisplay();
                    display.Supply = supply;
                    display.Item = item;
                    list.Add(display);
                }
                return list;
            }
        }
        public class CustomerDebtDisplay
        {
            public string CustomerName { get; set; }
            public string Notes { get; set; }
            public Guid Id { get; set; }
            public string Amount { get; set; }

            public static List<CustomerDebtDisplay> GenerateList(Dictionary<Guid,Item> items)
            {
                List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(Client.Run("customers", "GET", Client.eventId.ToString()));
                List<CustomerDebtDisplay> list = new List<CustomerDebtDisplay>();
                foreach (Customer customer in customers)
                {
                    CustomerDebtDisplay display = new CustomerDebtDisplay();
                    display.CustomerName = customer.Name;
                    display.Notes = customer.Notes;
                    display.Id = customer.CustomerId;
                    
                    List<Transaction> transactions = JsonConvert.DeserializeObject<List<Transaction>>(Client.Run("transactions", "GET", string.Format("customer={0}", customer.CustomerId)));
                    List<Payment> payments = JsonConvert.DeserializeObject<List<Payment>>(Client.Run("payments", "GET", string.Format("customer={0}", customer.CustomerId)));

                    long spent = 0;
                    long paid = 0;

                    foreach (Transaction transaction in transactions)
                    {
                        spent += (transaction.Amount * transaction.Multiplier * items[transaction.ItemId].Price) / 100000L;
                    }

                    foreach (Payment payment in payments)
                    {
                        paid += payment.Amount;
                    }

                    long debt = spent - paid;
                    display.Amount = string.Format("{0} Kč", Math.Round(debt / 100d, 0));

                    if (debt > 0) list.Add(display);
                }
                return list;
            }
        }

        #endregion

        #region Listy
        private List<Item> ItemsList()
        {
            return JsonConvert.DeserializeObject<List<Item>>(Client.Run("items", "GET", Client.eventId.ToString()));
        }

        private Dictionary<Guid, Item> ItemsDic()
        {
            Dictionary<Guid, Item> dic = new Dictionary<Guid, Item>();

            foreach (Item item in ItemsList())
            {
                dic.Add(item.ItemId, item);
            }
            return dic;
        }

        #endregion

        #region Objednávky
        public Items.ItemSelect ItemSelect { get; set; }
        public Order.SizeSelect SizeSelect { get; set; }
        public Order.SizeSelect SaleSelect { get; set; }
        public Order.Basket Basket { get; set; }
        public Customers.CustomerSelect CustomerSelectSale { get; set; }
        public Transaction CurrentTransaction { get; set; }

        private void NewOrder()
        {
            Basket = new Order.Basket(ItemsDic());
            Basket.FinaliseOrder += Basket_FinaliseOrder;

            NewItem();

            orderViewer.Content = ItemSelect;
            basketViewer.Content = Basket;
        }

        private void NewItem(object sender = null, EventArgs e = null)
        {
            ItemSelect = new Items.ItemSelect(ItemsDic());
            ItemSelect.ItemSelected += ItemSelect_ItemSelected;

            orderViewer.Content = ItemSelect;
        }

        private void Basket_FinaliseOrder(object sender, Order.FinaliseOrderEventArgs e)
        {
            CustomerSelectSale = new Customers.CustomerSelect(true, "Zaplatit ihned");
            CustomerSelectSale.CustomerSelected += CustomerSelect_CustomerSelected;

            orderViewer.Content = CustomerSelectSale;
        }

        private void CustomerSelect_CustomerSelected(object sender, Customer e)
        {
            if (e.CustomerId == Guid.Empty)
            {
                Basket.PayNow();
            }
            else
            {
                Basket.PayByAccount(e);
            }
            if (CustomerDetail != null)
            {
                CustomerDetail.LoadDetails();
            }
            NewOrder();
        }

        private void ItemSelect_ItemSelected(object sender, Items.ItemSelectedEventArgs e)
        {
            CurrentTransaction = new Transaction();
            CurrentTransaction.ItemId = e.Item.ItemId;

            SizeSelect = new Order.SizeSelect("Výběr velikosti", e.Item.Name, 500, "Velké (500ml)", 500, "Malé (300ml)", 300);
            SizeSelect.SizeSelected += SizeSelect_SizeSelected;
            SizeSelect.Cancelled += NewItem;

            orderViewer.Content = SizeSelect;
        }

        private void SizeSelect_SizeSelected(object sender, Order.SizeSelectedEventArgs e)
        {
            CurrentTransaction.Amount = e.Size;

            SaleSelect = new Order.SizeSelect("Cena", CurrentTransaction.Amount + "ml " + ItemsDic()[CurrentTransaction.ItemId], 100, "Plná cena", 100, "Sleva pro místní (60% ceny)", 60);
            SaleSelect.SizeSelected += SaleSelect_SizeSelected;
            SaleSelect.Cancelled += NewItem;

            orderViewer.Content = SaleSelect;
        }

        private void SaleSelect_SizeSelected(object sender, Order.SizeSelectedEventArgs e)
        {
            CurrentTransaction.Multiplier = e.Size;

            Basket.Add(CurrentTransaction, ItemsDic());

            NewItem();
        }
        
        #endregion

        #region Zákazníci

        public Customers.CustomerSelect CustomerSelectProp { get; set; }
        public Customers.CustomerDetail CustomerDetail { get; set; }
        public Payments.Keypad CreditKeypad { get; set; }

        private void CustomersInit()
        {
            CustomerSelectProp = new Customers.CustomerSelect(true, "Nový zákazník");
            CustomerSelectProp.CustomerSelected += CustomerSelectProp_CustomerSelected;

            customersViever.Content = CustomerSelectProp;
        }

        private void CustomerSelectProp_CustomerSelected(object sender, Customer e)
        {
            CustomerDetail = new Customers.CustomerDetail(e);
            CustomerDetail.ResetView += CustomerDetail_ResetView;
            CustomerDetail.KeypadRequest += CustomerDetail_KeypadRequest;

            customersViever.Content = CustomerDetail;
        }

        private void CustomerDetail_KeypadRequest(object sender, EventArgs e)
        {
            CreditKeypad = new Payments.Keypad(string.Format("Platba pro {0}", CustomerDetail.CurrentCustomer.Name));
            CreditKeypad.Typed += CreditKeypad_Typed;
            CreditKeypad.Typed += CustomerDetail.CreditKeypad_Typed;
            CreditKeypad.Cancelled += keypad_Cancelled;

            keypadViewer.Content = CreditKeypad;
            keypadOverlay.Visibility = Visibility.Visible;
        }

        private void keypad_Cancelled(object sender, EventArgs e)
        {
            keypadOverlay.Visibility = Visibility.Collapsed;
        }

        private void CreditKeypad_Typed(object sender, EventArgs e)
        {
            keypadOverlay.Visibility = Visibility.Collapsed;
        }

        private void CustomerDetail_ResetView(object sender, EventArgs e)
        {
            CustomersInit();
        }

        #endregion
        
        #region Transakce/Platby

        public List<TransactionsDisplay> TransactionsDisplayList { get; set; }
        public List<PaymentsDisplay> PaymentsDisplayList { get; set; }

        private void LoadTransactions()
        {
            List<Transaction> list = JsonConvert.DeserializeObject<List<Transaction>>(Client.Run("transactions", "GET", string.Format("event={0}", Client.eventId)));

            TransactionsDisplayList = TransactionsDisplay.GenerateList(list, ItemsDic(), true);
            transactionsGrid.ItemsSource = TransactionsDisplayList;
        }

        private void LoadPayments()
        {
            List<Payment> payments = JsonConvert.DeserializeObject<List<Payment>>(Client.Run("payments", "GET", string.Format("event={0}", Client.eventId)));

            PaymentsDisplayList = PaymentsDisplay.GenerateList(payments, ItemsDic(), true);
            paymentsGrid.ItemsSource = PaymentsDisplayList;
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
            }
            LoadTransactions();
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
            }
            LoadPayments();
        }

        private void grid_selectNone(object sender, MouseButtonEventArgs e)
        {
            ((DataGrid)sender).SelectedItem = null;
        }

        private void grid_ShowCustomer(object sender, RoutedEventArgs e)
        {
            Customer customer = (Customer)((Button)sender).Tag;
            if (customer.CustomerId == Guid.Empty) return;
            CustomerSelectProp_CustomerSelected(this, customer);
            customers.IsSelected = true;
        }

        private void grid_ShowItem(object sender, RoutedEventArgs e)
        {
            ItemSelectItems_ItemSelected(this, new Items.ItemSelectedEventArgs((Item)((Button)sender).Tag));
            items.IsSelected = true;
        }

        #endregion

        #region Dodávky

        public List<SuppliesDisplay> SuppliesDisplayList { get; set; }

        private void LoadSupplies()
        {
            List<Supply> supplies = JsonConvert.DeserializeObject<List<Supply>>(Client.Run("supplies", "GET", string.Format("event={0}", Client.eventId)));

            SuppliesDisplayList = SuppliesDisplay.GenerateList(supplies, ItemsDic());
            suppliesGrid.ItemsSource = SuppliesDisplayList;
        }

        private void suppliesGrid_RemoveSupply(object sender, RoutedEventArgs e)
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
            }
            LoadSupplies();
        }

        #endregion

        #region Zboží

        public Items.ItemSelect ItemSelectItems { get; set; }
        public Items.ItemDetail ItemDetail { get; set; }
        public Payments.Keypad SupplyKeypad { get; set; }

        public void LoadItemEditor()
        {
            ItemSelectItems = new Items.ItemSelect(ItemsDic(), true, false);
            ItemSelectItems.ItemSelected += ItemSelectItems_ItemSelected;

            itemsViewer.Content = ItemSelectItems;
        }

        private void ItemSelectItems_ItemSelected(object sender, Items.ItemSelectedEventArgs e)
        {
            ItemDetail = new Items.ItemDetail(e.Item);
            ItemDetail.ResetView += ItemDetail_ResetView;
            ItemDetail.KeypadRequest += ItemDetail_KeypadRequest;

            itemsViewer.Content = ItemDetail;
        }

        private void ItemDetail_KeypadRequest(object sender, Items.KeypadRequestEventArgs e)
        {
            if (e.Stage == 0)
            {
                SupplyKeypad = new Payments.Keypad(e.Label);
                SupplyKeypad.Typed += ItemDetail.AmountKeypad_Typed;
                SupplyKeypad.Cancelled += keypad_Cancelled;

                keypadViewer.Content = SupplyKeypad;
                keypadOverlay.Visibility = Visibility.Visible;
            }
            else if (e.Stage == 1)
            {
                SupplyKeypad = new Payments.Keypad(e.Label);
                SupplyKeypad.Typed += SupplyKeypad_Typed;
                SupplyKeypad.Typed += ItemDetail.PriceKeypad_Typed;
                SupplyKeypad.Cancelled += keypad_Cancelled;

                keypadViewer.Content = SupplyKeypad;
                keypadOverlay.Visibility = Visibility.Visible;
            }
        }

        private void SupplyKeypad_Typed(object sender, EventArgs e)
        {
            keypadOverlay.Visibility = Visibility.Collapsed;
        }

        private void ItemDetail_ResetView(object sender, EventArgs e)
        {
            LoadItemEditor();
        }

        #endregion

        #region Cashflow
        private void LoadCashflow()
        {
            Dictionary<Guid, Item> items = ItemsDic();
            List<Payment> payments = JsonConvert.DeserializeObject<List<Payment>>(Client.Run("payments", "GET", string.Format("event={0}", Client.eventId)));
            List<Supply> supplies = JsonConvert.DeserializeObject<List<Supply>>(Client.Run("supplies", "GET", string.Format("event={0}", Client.eventId)));
            List<Transaction> transactions = JsonConvert.DeserializeObject<List<Transaction>>(Client.Run("transactions", "GET", string.Format("event={0}", Client.eventId)));

            long paid = 0;
            long suppliesWorth = 0;
            long transactionsWorth = 0;

            foreach (Payment payment in payments)
            {
                paid += payment.Amount;
            }
            foreach (Supply supply in supplies)
            {
                suppliesWorth += supply.Price;
            }
            foreach (Transaction transaction in transactions)
            {
                transactionsWorth += (transaction.Amount * transaction.Multiplier * items[transaction.ItemId].Price) / 100000;
            }

            double suppliesValue = Math.Round(suppliesWorth / 100d, 0);
            double paymentsValue = Math.Round(paid / 100d, 0);
            double cash = Math.Round((paid - suppliesWorth) / 100d, 0);
            double toPay = Math.Round((transactionsWorth - paid) / 100d, 0);
            double total = Math.Round((transactionsWorth - suppliesWorth) / 100d, 0);

            suppliesValueDisplay.Text = suppliesValue.ToString() + " Kč";
            paymentsValueDisplay.Text = paymentsValue.ToString() + " Kč";
            cashDisplay.Text = cash.ToString() + " Kč";
            toPayDisplay.Text = toPay.ToString() + " Kč";
            totalDisplay.Text = total.ToString() + " Kč";

            if (cash < 0)
            {
                cashDisplay.Foreground = Brushes.Red;
            }
            else
            {
                cashDisplay.Foreground = Brushes.Black;
            }

            if (toPay < 0)
            {
                toPayDisplay.Foreground = Brushes.Red;
            }
            else
            {
                toPayDisplay.Foreground = Brushes.Black;
            }

            if (total < 0)
            {
                totalDisplay.Foreground = Brushes.Red;
            }
            else
            {
                totalDisplay.Foreground = Brushes.Black;
            }
        }

        private void customersToPayGrid_showCustomer(object sender, RoutedEventArgs e)
        {
            List<Customer> customersList = JsonConvert.DeserializeObject<List<Customer>>(Client.Run("customers", "GET", Client.eventId.ToString()));
            foreach (Customer cust in customersList)
            {
                if (cust.CustomerId == (Guid)((Button)sender).Tag)
                {
                    CustomerSelectProp_CustomerSelected(this, cust);
                    customers.IsSelected = true;
                    break;
                }
            }
        }

        #endregion

        #region Konec
        private void CloseGo_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("CurrentOrder.json"))
            {
                string file = File.ReadAllText("CurrentOrder.json");
                if (!string.IsNullOrEmpty(file)&&file!="[]")
                {
                    MessageBoxResult result = MessageBox.Show("Chcete uložit vaši aktuální objednávku?\r\n" +
                        "Kliknutím na Ano uložíte svou objednávku a ukončíte program.\r\n" +
                        "Kliknutím na Ne zavřete program bez uložení.\r\n" +
                        "Kliknutím na Zrušit zrušíte zavření programu.", "Ověření", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                    if (result == MessageBoxResult.No)
                    {
                        File.Delete("CurrentOrder.json");
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
                else
                {
                    File.Delete("CurrentOrder.json");
                }
            }
            Environment.Exit(0);
        }

        private void CloseCancel_Click(object sender, RoutedEventArgs e)
        {
            orderTab.IsSelected = true;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (orderTab.IsSelected)
                {
                    NewItem();
                }
                else if (orders_payments.IsSelected)
                {
                    LoadTransactions();
                    LoadPayments();
                }
                else if (supplies.IsSelected)
                {
                    LoadSupplies();
                }
                else if (cashflow.IsSelected)
                {
                    LoadCashflow();
                }
                else if (stats.IsSelected)
                {
                    customersToPay.ItemsSource = CustomerDebtDisplay.GenerateList(ItemsDic());
                }
                else if (events.IsSelected)
                {
                    List<Event> events = JsonConvert.DeserializeObject<List<Event>>(Client.Run("events", "GET"));
                    foreach (Event _event in events)
                    {
                        if (_event.EventId == Guid.Empty)
                        {
                            events.Remove(_event);
                            break;
                        }
                    }
                    eventsGrid.ItemsSource = events;
                }
            }
        }

        #endregion

        private void SetEvent(object sender, RoutedEventArgs e)
        {
            Client.eventId = (Guid)((Button)sender).Tag;
            LoadWindow();
        }
    }

    public static class ExtensionMethods
    {

        private static Action EmptyDelegate = delegate () { };


        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
