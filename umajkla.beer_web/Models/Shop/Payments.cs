﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace umajkla.beer.Models.Shop
{
    public class Payment
    {
        public Guid PaymentId { get; set; }
        public int Amount { get; set; }
        public Guid CustomerId { get; set; }
        public string Notes { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Created { get; set; }
        public string SQLResponse { get; set; }

        public Payment(Guid paymentId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                string cmdString = string.Format("SELECT * FROM dbo.payments WHERE paymentId='{0}'", paymentId);
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    PaymentId = Guid.Parse(reader["paymentId"].ToString());
                    Amount = int.Parse(reader["amount"].ToString());
                    CustomerId = Guid.Parse(reader["customerId"].ToString());
                    Notes = reader["notes"].ToString();
                    ProcessedBy = reader["processedBy"].ToString();
                    Updated = DateTime.Parse(reader["updated"].ToString());
                    Created = DateTime.Parse(reader["created"].ToString());
                }
            }
        }

        public Payment(string dataJson)
        {
            dynamic data = System.Web.Helpers.Json.Decode(dataJson);

            if (!string.IsNullOrEmpty(data.PaymentId.ToString())) PaymentId = Guid.Parse(data.PaymentId.ToString());
            Amount = int.Parse(data.Amount.ToString());
            CustomerId = Guid.Parse(data.CustomerId.ToString());
            Notes = data.Notes.ToString();
            ProcessedBy = data.ProcessedBy.ToString();
            if (!string.IsNullOrEmpty(data.Updated.ToString())) Updated = DateTime.Parse(data.Updated.ToString());
            if (!string.IsNullOrEmpty(data.Created.ToString())) Created = DateTime.Parse(data.Created.ToString());
        }

        public Payment()
        {

        }

        public List<Payment> List(Guid customerId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT paymentId FROM dbo.payments WHERE customerId='{0}'", customerId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                List<Payment> payments = new List<Payment>();
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Payment payment = new Payment(Guid.Parse(list["paymentId"].ToString()));
                        payments.Add(payment);
                    }
                }

                return payments;
            }
        }

        public Guid Create()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.payments (customerId, amount, notes) " +
                "OUTPUT INSERTED.ID VALUES ('{0}', '{1}', '{2}')",
                CustomerId, Amount, Notes);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                SQLResponse = command.ExecuteScalar().ToString();
                try
                {
                    return Guid.Parse(SQLResponse);
                }
                catch (FormatException)
                {
                    return Guid.Empty;
                }
            }
        }

        public Guid Update()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("UPDATE dbo.payments SET " +
                    "customerId='{0}', amount='{1}', notes='{2}', updated='{3}' OUTPUT INSERTED.ID WHERE id='{4}'",
                    CustomerId, Amount, Notes, DateTime.Now, PaymentId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                SQLResponse = command.ExecuteScalar().ToString();
                try
                {
                    return Guid.Parse(SQLResponse);
                }
                catch (FormatException)
                {
                    return Guid.Empty;
                }
            }
        }
    }
}