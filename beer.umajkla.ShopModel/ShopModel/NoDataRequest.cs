using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace beer.umajkla.ShopModel
{
    public partial class NoDataRequest : IShopObject
    {
        public ControllerName Controller { get; set; }
        public ControllerName ListBy { get; set; }
        public NoDataTask Task { get; set; }
        public Guid OfId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string CreatedBy { get; set; }
        public string SQLResponse { get; set; }
        
        public NoDataRequest(ControllerName controller, NoDataTask task, Guid ofId)
        {
            Controller = controller;
            Task = task;
            OfId = ofId;
        }
    }

    public enum ControllerName
    {
        [ApiAddress("customers")]
        Customers,
        [ApiAddress("events")]
        Events,
        [ApiAddress("items")]
        Items,
        [ApiAddress("locations")]
        Locations,
        [ApiAddress("payments")]
        Payments,
        [ApiAddress("supplies")]
        Supplies,
        [ApiAddress("transactions")]
        Transactions
    }

    public enum NoDataTask
    {
        Get, Delete
    }
}
