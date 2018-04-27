using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace beer.umajkla.ShopModel
{
    public partial class Payment : IShopObject
    {
        public Payment(Guid paymentId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                string cmdString = string.Format("SELECT * FROM dbo.payments WHERE paymentId='{0}'", paymentId);
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    InitFromReader(reader);
                }
            }
        }

        public Payment()
        {

        }

        private void InitFromReader(SqlDataReader reader)
        {
            PaymentId = Guid.Parse(reader["paymentId"].ToString());
            Amount = int.Parse(reader["amount"].ToString());
            CustomerId = Guid.Parse(reader["customerId"].ToString());
            Notes = reader["notes"].ToString();
            CreatedBy = reader["processedBy"].ToString();
            Updated = DateTime.Parse(reader["updated"].ToString());
            Created = DateTime.Parse(reader["created"].ToString());
            EventId = Guid.Parse(reader["eventId"].ToString());
        }

        private static List<Payment> ListBy(Guid id, string byWhat)
        {
            List<Payment> payments = new List<Payment>();
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT * FROM dbo.payments WHERE {0}='{1}' ORDER BY Created DESC", byWhat, id);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Payment payment = new Payment();
                        payment.InitFromReader(list);
                        payments.Add(payment);
                    }
                }
            }
            return payments;
        }

        public static List<Payment> ListByCustomer(Guid customerId) => ListBy(customerId, "customerId");

        public static List<Payment> ListByEvent(Guid eventId) => ListBy(eventId, "eventId");

        public bool Create(out Guid newId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.payments (customerId, amount, notes, eventId) " +
                "OUTPUT INSERTED.PAYMENTID VALUES ('{0}', '{1}', '{2}', '{3}')",
                CustomerId, Amount, Notes, EventId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                SQLResponse = command.ExecuteScalar().ToString();
                return Guid.TryParse(SQLResponse, out newId);
            }
        }

        public bool Update(out Guid newId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("UPDATE dbo.payments SET " +
                    "customerId='{0}', amount='{1}', notes='{2}', updated='{3}' OUTPUT INSERTED.PAYMENTID WHERE paymentId='{4}'",
                    CustomerId, Amount, Notes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), PaymentId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                SQLResponse = command.ExecuteScalar().ToString();
                return Guid.TryParse(SQLResponse, out newId);
            }
        }
    }
}
