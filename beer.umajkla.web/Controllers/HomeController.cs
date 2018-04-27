using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using beer.umajkla.ShopModel;

namespace beer.umajkla.web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Guid kofola = Guid.Parse("4f203fea-746f-48eb-8a32-725aae12fbe5");
            Guid eventId = Guid.Parse("99662ccc-a951-4eb9-9fbf-73eb9992c4ae");
            ViewData["eventId"] = eventId;
            List<Transaction> transactions = Transaction.ListByItem(kofola);
            List<Supply> supplies = Supply.ListByItem(kofola);
            Item item = new Item(kofola);

            int supplyVolume = 0;
            foreach (Supply supply in supplies)
            {
                supplyVolume += supply.Amount;
            }

            int soldAmount = 0;
            foreach (Transaction transaction in transactions)
            {
                soldAmount += transaction.Amount;
            }

            ViewData["ItemName"] = item.Name;
            ViewData["Supplies"] = supplyVolume;
            ViewData["Sold"] = soldAmount;

            List<KeyValuePair<Customer, long>> customers = new List<KeyValuePair<Customer, long>>();
            List<Item> items = Item.List(eventId);
            List<Customer> customersList = new Customer().List(eventId);

            foreach (Customer customer in customersList)
            {
                List<Transaction> customerTransactions = Transaction.ListByCustomer(customer.CustomerId);
                List<Payment> payments = Payment.ListByCustomer(customer.CustomerId);

                long paid = payments.Sum(payment => payment.Amount);
                long spent = 0;
                foreach (Transaction transaction in customerTransactions)
                {
                    spent += transaction.Amount * transaction.Multiplier * items.Find(lookupItem => lookupItem.ItemId == transaction.ItemId).Price / 100000;
                }
                customers.Add(new KeyValuePair<Customer, long>(customer, (paid - spent) / 100));
            }

            ViewData["customers"] = customers;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}