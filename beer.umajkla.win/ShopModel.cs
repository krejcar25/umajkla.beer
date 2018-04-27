using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beer.umajkla.win
{
    public class Customer
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
    }

    public class Event
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

    public class Item
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }
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


        public override string ToString()
        {
            return Name;
        }
    }

    public class Location
    {
        public Guid LocationId { get; set; }
        public string Name { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string CreatedBy { get; set; }
        public string SQLResponse { get; set; }
    }

    public class Payment
    {
        public Guid PaymentId { get; set; }
        public long Amount { get; set; }
        public Guid CustomerId { get; set; }
        public string Notes { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Created { get; set; }
        public string SQLResponse { get; set; }
        public Guid EventId { get; set; }
    }

    public class Supply
    {
        public Guid SupplyId { get; set; }
        public Guid ItemId { get; set; }
        public long Amount { get; set; }
        public long Price { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Notes { get; set; }
        public string ProcessedBy { get; set; }
        public string SQLResponse { get; set; }
        public Guid EventId { get; set; }
    }

    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ItemId { get; set; }
        public long Amount { get; set; }
        public long Multiplier { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Notes { get; set; }
        public string ProcessedBy { get; set; }
        public string SQLResponse { get; set; }
        public Guid EventId { get; set; }
    }
}
