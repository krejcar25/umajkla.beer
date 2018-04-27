using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using beer.umajkla.ShopModel;

namespace beer.umajkla.web.Controllers
{
    public class POSController : Controller
    {
        // GET: POS
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Order(string id)
        {
            Guid eventId;
            if (Guid.TryParse(id, out eventId))
            {
                ViewData["canOrder"] = true;
                List<Item> items = Item.List(eventId);
                List<Item> displayed = new List<Item>();
                foreach (Item item in items)
                {
                    List<Transaction> transactions = Transaction.ListByItem(item.ItemId);
                    List<Supply> supplies = Supply.ListByItem(item.ItemId);
                    long inStock = supplies.Sum(supply => supply.Amount) - transactions.Sum(transaction => transaction.Amount);
                    if (inStock > 0) displayed.Add(item);
                }
                ViewData["items"] = displayed;
            }
            else
            {
                ViewData["canOrder"] = false;
            }

            return View();
        }
    }
}