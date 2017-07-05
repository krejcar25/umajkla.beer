using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace umajkla.beer.Models.Shop
{
    public class Supply
    {
        public Guid SupplyId { get; set; }
        public Guid ItemId { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Notes { get; set; }
        public string ProcessedBy { get; set; }
        public string SQLResponse { get; set; }

        public Supply(Guid supplyId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                string cmdString = string.Format("SELECT * FROM dbo.supplies WHERE supplyId='{0}'", supplyId);
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    SupplyId = Guid.Parse(reader["supplyId"].ToString());
                    ItemId = Guid.Parse(reader["itemId"].ToString());
                    Amount = int.Parse(reader["amount"].ToString());
                    Price = int.Parse(reader["price"].ToString());
                    Created = DateTime.Parse(reader["created"].ToString());
                    Updated = DateTime.Parse(reader["updated"].ToString());
                    Notes = reader["notes"].ToString();
                    ProcessedBy = reader["processedBy"].ToString();
                }
            }
        }

        public Supply(string dataJson)
        {
            dynamic data = System.Web.Helpers.Json.Decode(dataJson);

            SupplyId = Guid.Parse(data.SupplyId.ToString());
            ItemId = Guid.Parse(data.ItemId.ToString());
            Amount = int.Parse(data.Amount.ToString());
            Price = int.Parse(data.Price.ToString());
            Created = DateTime.Parse(data.Created.ToString());
            Updated = DateTime.Parse(data.Updated.ToString());
            Notes = data.Notes.ToString();
            ProcessedBy = data.ProcessedBy.ToString();
        }

        public Supply()
        {

        }

        public List<Supply> List(Guid itemId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT supplyId FROM dbo.supplies WHERE itemId='{0}'", itemId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                List<Supply> supplies = new List<Supply>();
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Supply transaction = new Supply(Guid.Parse(list["supplyId"].ToString()));
                        supplies.Add(transaction);
                    }
                }

                return supplies;
            }
        }

        public Guid Create()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.supplies (itemId, amount, price, notes) " +
                "OUTPUT INSERTED.ID VALUES ('{0}', '{1}', '{2}', '{3}')",
                ItemId, Amount, Price, Notes);
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
                string cmdString = string.Format("UPDATE dbo.supplies SET " +
                    "itemId='{0}', amount='{1}', price='{2}', notes='{3}', updated='{4}' OUTPUT INSERTED.ID WHERE supplyId='{5}'",
                    ItemId, Amount, Price, Notes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SupplyId);
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