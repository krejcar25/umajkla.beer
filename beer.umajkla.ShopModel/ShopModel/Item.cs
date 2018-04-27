using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace beer.umajkla.ShopModel
{
	public partial class Item : IShopObject
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Unit { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Notes { get; set; }
        public Guid EventId { get; set; }
        public string CreatedBy { get; set; }
        public string SQLResponse { get; set; }
        public double DisplayMultiplier { get; set; }
        public double DefaultSize { get; set; }
        public double Size1 { get; set; }
        public double Size2 { get; set; }
        public string Size1Label { get; set; }
        public string Size2Label { get; set; }
        public Item PreviousVersion { get; set; }
        public Guid ItemSetId { get; set; }
        public List<Item> PackItems { get; set; }
        public double AmountInPack { get; set; }
        public List<Supply> Supplies { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}