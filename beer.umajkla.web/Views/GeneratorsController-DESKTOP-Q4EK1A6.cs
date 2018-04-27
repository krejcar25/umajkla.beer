using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using beer.umajkla.ShopModel;
using Newtonsoft.Json;

namespace beer.umajkla.web.Views
{
    public class GeneratorsController : Controller
    {
        // GET: Generators
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DrankBeer(string itemGuid)
        {
            List<Transaction> transactions = Transaction.ListByItem(Guid.Parse(itemGuid));
            List<Supply> supplies = Supply.ListByItem(Guid.Parse(itemGuid));
            Item item = new Item(Guid.Parse(itemGuid));

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

            return View();
        }
    }
}