using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace umajkla.beer.Models.Shop
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ItemId { get; set; }
        public int Amount { get; set; }
        public int Multiplier { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Notes { get; set; }
        public string ProcessedBy { get; set; }
        public string SQLResponse { get; set; }
        public Guid EventId { get; set; }

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

                    TransactionId = Guid.Parse(reader["transactionId"].ToString());
                    CustomerId = Guid.Parse(reader["customerId"].ToString());
                    ItemId = Guid.Parse(reader["itemId"].ToString());
                    Amount = int.Parse(reader["amount"].ToString());
                    Multiplier = int.Parse(reader["multiplier"].ToString());
                    Created = DateTime.Parse(reader["created"].ToString());
                    Updated = DateTime.Parse(reader["updated"].ToString());
                    Notes = reader["notes"].ToString();
                    ProcessedBy = reader["processedBy"].ToString();
                    EventId = Guid.Parse(reader["eventId"].ToString());
                }
            }
        }

        /*public Transaction(string dataJson)
        {
            dynamic data = System.Web.Helpers.Json.Decode(dataJson);

            if (!string.IsNullOrEmpty(data.TransactionId.ToString())) TransactionId = Guid.Parse(data.TransactionId.ToString());
            CustomerId = Guid.Parse(data.CustomerId.ToString());
            ItemId = Guid.Parse(data.ItemId.ToString());
            Amount = int.Parse(data.Amount.ToString());
            Multiplier = int.Parse(data.Multiplier.ToString());
            if (!string.IsNullOrEmpty(data.Created.ToString())) Created = DateTime.Parse(data.Created.ToString());
            if (!string.IsNullOrEmpty(data.Updated.ToString())) Updated = DateTime.Parse(data.Updated.ToString());
            Notes = data.Notes.ToString();
            ProcessedBy = data.ProcessedBy.ToString();
        }*/

        public Transaction()
        {

        }

        public List<Transaction> ListByCustomer(Guid customerId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT transactionId FROM dbo.transactions WHERE customerId='{0}' ORDER BY Created DESC", customerId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                List<Transaction> transactions = new List<Transaction>();
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Transaction transaction = new Transaction(Guid.Parse(list["transactionId"].ToString()));
                        transactions.Add(transaction);
                    }
                }

                return transactions;
            }
        }

        public List<Transaction> ListByItem(Guid itemId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT transactionId FROM dbo.transactions WHERE itemId='{0}' ORDER BY Created DESC", itemId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                List<Transaction> transactions = new List<Transaction>();
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        string idString = list["transactionId"].ToString();
                        Guid id = Guid.Parse(idString);
                        Transaction transaction = new Transaction(id);
                        transactions.Add(transaction);
                    }
                }

                return transactions;
            }
        }

        public List<Transaction> ListByEvent(Guid eventId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT transactionId FROM dbo.transactions WHERE eventId='{0}' ORDER BY Created DESC", eventId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                List<Transaction> transactions = new List<Transaction>();
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        string idString = list["transactionId"].ToString();
                        Guid id = Guid.Parse(idString);
                        Transaction transaction = new Transaction(id);
                        transactions.Add(transaction);
                    }
                }

                return transactions;
            }
        }

        public Guid Create()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.transactions (customerId, itemId, amount, multiplier, notes, eventId) " +
                "OUTPUT INSERTED.TRANSACTIONID VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                CustomerId, ItemId, Amount, Multiplier, Notes, EventId);
                connection.Open();
                try
                {
                    SqlCommand command = new SqlCommand(cmdString, connection);
                    SQLResponse = command.ExecuteScalar().ToString();
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
                string cmdString = string.Format("UPDATE dbo.transactions SET " +
                    "customerId='{0}', itemId='{1}', amount='{2}', multiplier='{3}', notes='{4}', updated='{5}' OUTPUT INSERTED.TRANSACTIONID WHERE transactionId='{6}'",
                    CustomerId, ItemId, Amount, Multiplier, Notes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), TransactionId);
                connection.Open();
                try
                {
                    SqlCommand command = new SqlCommand(cmdString, connection);
                    SQLResponse = command.ExecuteScalar().ToString();
                    return Guid.Parse(SQLResponse);
                }
                catch (Exception)
                {
                    return Guid.Empty;
                }
            }
        }
    }
}