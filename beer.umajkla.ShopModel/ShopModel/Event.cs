using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace beer.umajkla.ShopModel
{
    public partial class Event : IShopObject
    {
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public Guid LocationId { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string SQLResponse { get; set; }
    }
}