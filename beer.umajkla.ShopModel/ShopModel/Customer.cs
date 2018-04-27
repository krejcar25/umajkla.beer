using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace beer.umajkla.ShopModel
{
    public partial class Customer : IShopObject
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid EventId { get; set; }
        public string CreatedBy { get; set; }
        public string SQLResponse { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Payment> Payments { get; set; }
    }
}