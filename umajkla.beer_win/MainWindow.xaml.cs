using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using static beer.umajkla.win.AppConfig;

namespace beer.umajkla.win
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            LoadConfig();
            Client.RequestWindowClose += Close;

            InitializeComponent();
            charts.Visibility = Visibility.Collapsed;
            workingOverlay.Visibility = Visibility.Collapsed;
            keypadOverlay.Visibility = Visibility.Collapsed;
            displayOptionsOverlay.Visibility = Visibility.Collapsed;

            Client.WorkStarted += Client_WorkStarted;
            Client.WorkDone += Client_WorkDone;
            this.Refresh();


            LoadEvents();
        }

        private void AdminLoadWindow()
        {
            Title = "Pub U Majkla - ADMIN";
            this.Refresh();

            NewOrder();
            orderTab.Visibility = Visibility.Visible;
            CustomersInit();
            customers.Visibility = Visibility.Visible;
            InitPreorder();
            preorder.Visibility = Visibility.Visible;

            LoadItemEditor();
            items.Visibility = Visibility.Visible;
            orders_payments.Visibility = Visibility.Visible;
            supplies.Visibility = Visibility.Visible;
            cashflow.Visibility = Visibility.Visible;
            stats.Visibility = Visibility.Visible;

            orderTab.IsSelected = true;
        }

        private void UserLoadWindow()
        {
            Title = "Pub U Majkla";
            this.Refresh();

            NewOrder();
            orderTab.Visibility = Visibility.Visible;
            CustomersInit();
            customers.Visibility = Visibility.Visible;
            InitPreorder();
            preorder.Visibility = Visibility.Visible;

            orderTab.IsSelected = true;
        }

        private void DisplayLoadWindow()
        {
            Title = "Pub U Majkla - DISPLAY";
            this.Refresh();
            DisplayInit();
            tabs.Visibility = Visibility.Collapsed;
            charts.Visibility = Visibility.Visible;
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

        #region Výdej
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

        private void ItemSelect_ItemSelected(object sender, Item e)
        {
            CurrentTransaction = new Transaction();
            CurrentTransaction.ItemId = e.ItemId;

            SizeSelect = new Order.SizeSelect("Výběr velikosti", e.Name, (int)(e.DefaultSize * 1000), e.Size1Label, (int)(e.Size1 * 1000), e.Size2Label, (int)(e.Size2 * 1000));
            SizeSelect.SizeSelected += SizeSelect_SizeSelected;
            SizeSelect.Cancelled += NewItem;

            orderViewer.Content = SizeSelect;
        }

        private void SizeSelect_SizeSelected(object sender, Order.SizeSelectedEventArgs e)
        {
            CurrentTransaction.Amount = e.Size;

            Dictionary<Guid, Item> items = ItemsDic();
            SaleSelect = new Order.SizeSelect("Cena", CurrentTransaction.Amount / 1000d + items[CurrentTransaction.ItemId].Unit + " " + items[CurrentTransaction.ItemId], 100, "Plná cena", 100, "Sleva pro místní (60% ceny)", 60);
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
            CustomerSelectProp = new Customers.CustomerSelect(true, "Nový zákazník", ItemsDic());
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

        #region Objednat

        public Items.ItemSelect PreorderItemSelect { get; set; }
        public Order.SizeSelect PreorderSizeSelect { get; set; }
        public Order.SizeSelect PreorderSaleSelect { get; set; }
        public Order.Basket PreorderBasket { get; set; }
        public Customers.CustomerSelect PreorderCustomerSelectSale { get; set; }
        public Transaction PreorderCurrentTransaction { get; set; }
        public List<KeyValuePair<Customer,List<Transaction>>> PreorderList { get; set; }
        public Order.ListOrders ListOrders { get; set; }

        private void InitPreorder()
        {
            if (File.Exists("current_preorders.json"))
            {
                PreorderList = JsonConvert.DeserializeObject<List<KeyValuePair<Customer, List<Transaction>>>>(File.ReadAllText("current_preorders.json")) ?? new List<KeyValuePair<Customer, List<Transaction>>>();
            }
            else
            {
                PreorderList = new List<KeyValuePair<Customer, List<Transaction>>>();
            }

            NewPreorder();
        }

        private void NewPreorder()
        {
            
            PreorderBasket = new Order.Basket(ItemsDic());
            PreorderBasket.FinaliseOrder += PreorderBasket_FinaliseOrder;

            PreorderNewItem();

            preorderViewer.Content = PreorderItemSelect;
            preorderBasketViewer.Content = PreorderBasket;

            //if (!File.Exists("current_preorders.json")) File.Create("current_preorders.json");
            File.WriteAllText("current_preorders.json", JsonConvert.SerializeObject(PreorderList, new JsonSerializerSettings() {  Formatting = Formatting.Indented }));
        }

        private void PreorderNewItem(object sender = null, EventArgs e = null)
        {
            PreorderItemSelect = new Items.ItemSelect(ItemsDic(), showEmpty: true, emptyLabel: "Objednávky");
            PreorderItemSelect.ItemSelected += PreorderItemSelect_ItemSelected;

            preorderViewer.Content = PreorderItemSelect;
        }

        private void PreorderBasket_FinaliseOrder(object sender, Order.FinaliseOrderEventArgs e)
        {
            PreorderCustomerSelectSale = new Customers.CustomerSelect(true, "Zaplatit po vydání");
            PreorderCustomerSelectSale.CustomerSelected += PreorderCustomerSelect_CustomerSelected;

            preorderViewer.Content = PreorderCustomerSelectSale;
        }

        private void PreorderCustomerSelect_CustomerSelected(object sender, Customer e)
        {
            PreorderList.Add(new KeyValuePair<Customer, List<Transaction>>(e, PreorderBasket.CurrentOrder));
            NewPreorder();
        }

        private void PreorderItemSelect_ItemSelected(object sender, Item e)
        {
            if (e.ItemId == Guid.Empty)
            {
                ListOrders = new Order.ListOrders(PreorderList, ItemsDic());
                ListOrders.newOrder.Click += NewOrder_Click;
                preorderViewer.Content = ListOrders;
            }
            else
            {
                PreorderCurrentTransaction = new Transaction();
                PreorderCurrentTransaction.ItemId = e.ItemId;

                PreorderSizeSelect = new Order.SizeSelect("Výběr velikosti", e.Name, (int)(e.DefaultSize * 1000), e.Size1Label, (int)(e.Size1 * 1000), e.Size2Label, (int)(e.Size2 * 1000));
                PreorderSizeSelect.SizeSelected += PreorderSizeSelect_SizeSelected;
                PreorderSizeSelect.Cancelled += PreorderNewItem;

                preorderViewer.Content = PreorderSizeSelect;
            }
        }

        private void NewOrder_Click(object sender, RoutedEventArgs e)
        {
            NewPreorder();
        }

        private void PreorderSizeSelect_SizeSelected(object sender, Order.SizeSelectedEventArgs e)
        {
            PreorderCurrentTransaction.Amount = e.Size;
            
            Dictionary<Guid, Item> items = ItemsDic();
            PreorderSaleSelect = new Order.SizeSelect("Cena", CurrentTransaction.Amount / 1000d + items[CurrentTransaction.ItemId].Unit + " " + items[CurrentTransaction.ItemId], 100, "Plná cena", 100, "Sleva pro místní (60% ceny)", 60);
            PreorderSaleSelect.SizeSelected += PreorderSaleSelect_SizeSelected;
            PreorderSaleSelect.Cancelled += PreorderNewItem;

            preorderViewer.Content = PreorderSaleSelect;
        }

        private void PreorderSaleSelect_SizeSelected(object sender, Order.SizeSelectedEventArgs e)
        {
            PreorderCurrentTransaction.Multiplier = e.Size;

            PreorderBasket.Add(PreorderCurrentTransaction, ItemsDic());

            PreorderNewItem();
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
            items.IsSelected = true;
            Item item = (Item)((Button)sender).Tag;
            ItemSelectItems_ItemSelected(this, item);
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

        private void ItemSelectItems_ItemSelected(object sender, Item e)
        {
            ItemDetail = new Items.ItemDetail(e);
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
            Close();
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
                else if (customers.IsSelected)
                {
                    CustomersInit();
                }
                else if (preorder.IsSelected)
                {
                    PreorderNewItem();
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
                else if (items.IsSelected)
                {
                    LoadItemEditor();
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
                    LoadEvents();
                }
            }
        }

        private void LoadEvents()
        {
            string json = Client.Run("events", "GET");
            List<Event> events = JsonConvert.DeserializeObject<List<Event>>(json);
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

        private void SetEvent(object sender, RoutedEventArgs e)
        {
            CurrentConfig.EventId = (Guid)((Button)sender).Tag;
            events.Visibility = Visibility.Collapsed;
            string mode;
            if (AppArgs.TryGetValue("-mode", out mode) || AppArgs.TryGetValue("/mode", out mode)) 
            {
                if (mode == "admin")
                {
                    AdminLoadWindow();
                }
                else if (mode == "user")
                {
                    UserLoadWindow();
                }
                else if (mode == "display")
                {
                    DisplayLoadWindow();
                }
                else
                {
                    MessageBox.Show("Parametr mode nebylo možné rozpoznat! Prosím spusťte program s jedním z platných hodnot parametru mode: admin, user nebo display.", "Chyba spouštění programu", MessageBoxButton.OK, MessageBoxImage.Stop);
                    Close();
                }
            }
            else
            {
                /*MessageBox.Show("Parametr mode nebyl uveden! Prosím spusťte program s jedním z platných hodnot parametru mode: admin, user nebo display.", "Chyba spouštění programu", MessageBoxButton.OK, MessageBoxImage.Stop);
                Close();*/
                UserLoadWindow();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            CurrentConfig.Save();
        }

        #endregion

        #region Display

        BackgroundWorker displayTimer = new BackgroundWorker();

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (charts.Visibility == Visibility.Visible) 
            {
                if (e.Key == Key.Escape && displayOptionsOverlay.Visibility == Visibility.Collapsed)
                {
                    Close();
                }
                else if ((e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl) && displayOptionsOverlay.Visibility == Visibility.Visible) 
                {
                    displayOptionsOverlay.Visibility = Visibility.Collapsed;
                }
                else if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                {
                    displayOptionsOverlay.Visibility = Visibility.Visible;
                }
            }
        }

        private List<ItemChartDisplaySettings> ItemChartDisplaySettingList
        {
            get
            {
                List<ItemChartDisplaySettings> items = new List<ItemChartDisplaySettings>();
                foreach (KeyValuePair<Guid,ItemChartDisplaySettings> item in CurrentConfig.DisplayItems)
                {
                    if (item.Key != Guid.Empty) items.Add(item.Value);
                }
                return items;
            }
            set
            {
                for (int i = 0; i < value.Count; i++)
                {
                    CurrentConfig.DisplayItems[value[i].ItemId] = value[i];
                }
            }
        }

        private void DisplayInit()
        {
            Dictionary<Guid, Item> items = ItemsDic();
            Dictionary<Guid, ItemChartDisplaySettings> settings = CurrentConfig.DisplayItems;
            foreach (KeyValuePair<Guid,Item> item in items)
            {
                if (!CurrentConfig.DisplayItems.ContainsKey(item.Value.ItemId))
                {
                    settings.Add(item.Value.ItemId, new ItemChartDisplaySettings());
                    settings[item.Value.ItemId].ItemId = item.Value.ItemId;
                    settings[item.Value.ItemId].Name = item.Value.Name;
                    settings[item.Value.ItemId].Displayed = true;
                }
            }
            CurrentConfig.DisplayItems = settings;

            delay.Value = CurrentConfig.DisplayItems[Guid.Empty].Delay;
            soldLabelText.Text = CurrentConfig.DisplayItems[Guid.Empty].SoldLabel;
            remainsLabelText.Text = CurrentConfig.DisplayItems[Guid.Empty].RemainsLabel;
            soldColor.SelectedColor = CurrentConfig.DisplayItems[Guid.Empty].SoldColor;
            remainsColor.SelectedColor = CurrentConfig.DisplayItems[Guid.Empty].RemainsColor;
            timingColor.SelectedColor = CurrentConfig.DisplayItems[Guid.Empty].ProgressBarColor;

            displayOptionsList.ItemsSource = ItemChartDisplaySettingList;
            displayTimer.WorkerReportsProgress = true;
            displayTimer.WorkerSupportsCancellation = false;
            displayTimer.DoWork += new DoWorkEventHandler(DisplayTimer_Loop);
            displayTimer.ProgressChanged += new ProgressChangedEventHandler(DisplayTimer_Update);
            displayTimer.RunWorkerAsync();
        }

        private void Charts_DoubleTap(object sender, EventArgs e)
        {
            Close();
        }

        private void DisplayTimer_Loop(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Debug.WriteLine("Starting backgroundWorker");
            while (!worker.CancellationPending)
            {
                Debug.WriteLine("BackgroundWorker is running new cycle");
                foreach (KeyValuePair<Guid,ItemChartDisplaySettings> item in CurrentConfig.DisplayItems)
                {
                    Debug.WriteLine("BackgroundWorker hit item " + item.Value.Name + " and ID " + item.Key);
                    if (item.Key != Guid.Empty&&item.Value.Displayed)
                    {
                        worker.ReportProgress(0, item);
                        for (int i = 0; i < item.Value.Delay; i++)
                        {
                            Debug.WriteLine(string.Format("Will sleep for {0} s", item.Value.Delay - i));
                            worker.ReportProgress(0, i);
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
        }

        private void DisplayTimer_Update(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState.GetType()==typeof(int))
            {
                if ((int)e.UserState == 0)
                {
                    timingCountdown.Text = string.Format("{0} s", timing.Maximum);
                    timing.BeginAnimation(System.Windows.Controls.Primitives.RangeBase.ValueProperty, new DoubleAnimation(0, new TimeSpan(0, 0, 0)));
                }
                else
                {
                    timingCountdown.Text = string.Format("{0} s", timing.Maximum - (int)e.UserState + 1);
                    timing.BeginAnimation(System.Windows.Controls.Primitives.RangeBase.ValueProperty, new DoubleAnimation((int)e.UserState, new TimeSpan(0, 0, 1)));
                }
            }
            else
            {
                KeyValuePair<Guid, ItemChartDisplaySettings> item = (KeyValuePair<Guid, ItemChartDisplaySettings>)e.UserState;
                List<Transaction> transactions = JsonConvert.DeserializeObject<List<Transaction>>(Client.Run("transactions", "GET", "item=" + item.Key.ToString()));
                Debug.WriteLine("Transactions for query item=" + item.Key.ToString() + " were retrieved");
                List<Supply> supplies = JsonConvert.DeserializeObject<List<Supply>>(Client.Run("supplies", "GET", "item=" + item.Key.ToString()));
                Debug.WriteLine("Supplies for query item=" + item.Key.ToString() + " were retrieved");
                long inStock = supplies.Sum(supply => supply.Amount);
                long sold = transactions.Sum(transaction => transaction.Amount);
                double ratio = 0;
                if (inStock > 0) ratio = (double)sold / (double)inStock;
                if (inStock > 0 && ratio == 0) ratio = 0.00000001;
                Debug.WriteLine(string.Format("Sold: {0} of {1} ({2}%)", sold, inStock, ratio * 100));
                timing.Foreground = new SolidColorBrush(item.Value.ProgressBarColor);
                timing.Maximum = item.Value.Delay - 1;
                soldLegend.Text = item.Value.SoldLabel;
                remainsLegend.Text = item.Value.RemainsLabel;
                displayLabel.Text = item.Value.Name;

                SolidColorBrush consumedBrush = FindResource("consumedSliceForeground") as SolidColorBrush;
                SolidColorBrush remainsBrush = FindResource("remainsSliceForeground") as SolidColorBrush;
                ColorAnimation consumedAnim = new ColorAnimation();
                ColorAnimation remainsAnim = new ColorAnimation();
                consumedAnim.From = consumedBrush.Color;
                remainsAnim.From = remainsBrush.Color;
                consumedAnim.To = item.Value.SoldColor;
                remainsAnim.To = item.Value.RemainsColor;
                consumedAnim.Duration = new TimeSpan(0, 0, 1);
                remainsAnim.Duration = new TimeSpan(0, 0, 1);
                consumedBrush.BeginAnimation(SolidColorBrush.ColorProperty, consumedAnim);
                remainsBrush.BeginAnimation(SolidColorBrush.ColorProperty, remainsAnim);
                sliceConsumed.BeginAnimation(Xceed.Wpf.Toolkit.Pie.SliceProperty, new DoubleAnimation(ratio, new TimeSpan(0, 0, 1)));
                this.Refresh();
            }
        }

        private void setToSelected_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<Guid, ItemChartDisplaySettings> item in CurrentConfig.DisplayItems)
            {
                if (item.Value.BulkSelected)
                {
                    CurrentConfig.DisplayItems[item.Key].Delay = (int)delay.Value;
                    CurrentConfig.DisplayItems[item.Key].SoldLabel = soldLabelText.Text;
                    CurrentConfig.DisplayItems[item.Key].RemainsLabel = remainsLabelText.Text;
                    CurrentConfig.DisplayItems[item.Key].SoldColor = soldColor.SelectedColor.GetValueOrDefault();
                    CurrentConfig.DisplayItems[item.Key].RemainsColor = remainsColor.SelectedColor.GetValueOrDefault();
                    CurrentConfig.DisplayItems[item.Key].ProgressBarColor = timingColor.SelectedColor.GetValueOrDefault();
                }
            }
        }

        private void setToDofault_Click(object sender, RoutedEventArgs e)
        {
            CurrentConfig.DisplayItems[Guid.Empty].Delay = (int)delay.Value;
            CurrentConfig.DisplayItems[Guid.Empty].SoldLabel = soldLabelText.Text;
            CurrentConfig.DisplayItems[Guid.Empty].RemainsLabel = remainsLabelText.Text;
            CurrentConfig.DisplayItems[Guid.Empty].SoldColor = soldColor.SelectedColor.GetValueOrDefault();
            CurrentConfig.DisplayItems[Guid.Empty].RemainsColor = remainsColor.SelectedColor.GetValueOrDefault();
            CurrentConfig.DisplayItems[Guid.Empty].ProgressBarColor = timingColor.SelectedColor.GetValueOrDefault();
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (charts.Visibility == Visibility.Visible)
            {
                if (displayOptionsOverlay.Visibility == Visibility.Visible)
                {
                    displayOptionsOverlay.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Close();
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (charts.Visibility == Visibility.Visible)
            {
                if (displayOptionsOverlay.Visibility == Visibility.Collapsed)
                {
                    displayOptionsOverlay.Visibility = Visibility.Visible;
                }
            }
        }

        #endregion
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
