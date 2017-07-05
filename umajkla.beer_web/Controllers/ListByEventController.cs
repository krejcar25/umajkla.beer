using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using umajkla.beer.Models.Shop;

namespace umajkla.beer.Controllers
{
    public class ListByEventController : Controller
    {
        // GET: ListByEvent
        public ActionResult Index()
        {
            return new EmptyResult();
        }

        public ActionResult Payments(string id)
        {
            Guid eventId;

            try
            {
                eventId = Guid.Parse(id);
            }
            catch (FormatException)
            {
                return Json("event guid has incorrect format");
            }

            List<Customer> customers = new Customer().List(eventId);
            List<Payment> payments = new List<Payment>();
            foreach (Customer customer in customers)
            {
                List<Payment> partial = new Payment().List(customer.CustomerId);
                payments.AddRange(partial);
            }

            return Json(payments);
        }

        public ActionResult Transactions(string id)
        {
            Guid eventId;

            try
            {
                eventId = Guid.Parse(id);
            }
            catch (FormatException)
            {
                return Json("event guid has incorrect format");
            }

            List<Customer> customers = new Customer().List(eventId);
            List<Transaction> transactions = new List<Transaction>();
            foreach (Customer customer in customers)
            {
                List<Transaction> partial = new Transaction().ListByCustomer(customer.CustomerId);
                transactions.AddRange(partial);
            }

            return Json(transactions);
        }

        public ActionResult Supplies(string id)
        {
            Guid itemId;

            try
            {
                itemId = Guid.Parse(id);
            }
            catch (FormatException)
            {
                return Json("item guid has incorrect format");
            }

            List<Item> items = new Item().List(itemId);
            List<Supply> payments = new List<Supply>();
            foreach (Item item in items)
            {
                List<Supply> partial = new Supply().List(item.ItemId);
                payments.AddRange(partial);
            }

            return Json(payments);
        }
    }
}