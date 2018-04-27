using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace beer.umajkla.ShopModel
{
    public partial class Transaction : IShopObject
    {
        public Transaction(Guid transactionId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                string cmdString = string.Format("SELECT * FROM dbo.transactions WHERE transactionId='{0}'", transactionId);
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    InitFromReader(reader);
                }
            }
        }

        public Transaction()
        {

        }

        private void InitFromReader(SqlDataReader reader)
        {
            TransactionId = Guid.Parse(reader["transactionId"].ToString());
            CustomerId = Guid.Parse(reader["customerId"].ToString());
            ItemId = Guid.Parse(reader["itemId"].ToString());
            Amount = int.Parse(reader["amount"].ToString());
            Multiplier = int.Parse(reader["multiplier"].ToString());
            Created = DateTime.Parse(reader["created"].ToString());
            Updated = DateTime.Parse(reader["updated"].ToString());
            Notes = reader["notes"].ToString();
            CreatedBy = reader["processedBy"].ToString();
            EventId = Guid.Parse(reader["eventId"].ToString());
        }

        private static List<Transaction> ListBy(Guid id, string byWhat)
        {
            List<Transaction> transactions = new List<Transaction>();
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT * FROM dbo.transactions WHERE {0}='{1}' ORDER BY Created DESC", byWhat, id);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Transaction transaction = new Transaction();
                        transaction.InitFromReader(list);
                        transactions.Add(transaction);
                    }
                }
            }
            return transactions;
        }

        public static List<Transaction> ListByCustomer(Guid customerId) => ListBy(customerId, "customerId");

        public static List<Transaction> ListByItem(Guid itemId) => ListBy(itemId, "itemId");

        public static List<Transaction> ListByEvent(Guid eventId) => ListBy(eventId, "eventId");

        public bool Create(out Guid newId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.transactions (customerId, itemId, amount, multiplier, notes, eventId) " +
                "OUTPUT INSERTED.TRANSACTIONID VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                CustomerId, ItemId, Amount, Multiplier, Notes, EventId);
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
                string cmdString = string.Format("UPDATE dbo.transactions SET " +
                    "customerId='{0}', itemId='{1}', amount='{2}', multiplier='{3}', notes='{4}', updated='{5}' OUTPUT INSERTED.TRANSACTIONID WHERE transactionId='{6}'",
                    CustomerId, ItemId, Amount, Multiplier, Notes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), TransactionId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                SQLResponse = command.ExecuteScalar().ToString();
                return Guid.TryParse(SQLResponse, out newId);
            }
        }
    }
}