using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace beer.umajkla.ShopModel
{
    public partial class Supply : IShopObject
    {
        public Guid SupplyId { get; set; }
        public Guid ItemId { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public string SQLResponse { get; set; }
        public Guid EventId { get; set; }
    }
}