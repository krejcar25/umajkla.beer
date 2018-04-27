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
        public Supply(Guid supplyId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                string cmdString = string.Format("SELECT * FROM dbo.supplies WHERE supplyId='{0}'", supplyId);
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    InitFromReader(reader);
                }
            }
        }
        
        public Supply()
        {

        }
        private void InitFromReader(SqlDataReader reader)
        {
            SupplyId = Guid.Parse(reader["supplyId"].ToString());
            ItemId = Guid.Parse(reader["itemId"].ToString());
            Amount = int.Parse(reader["amount"].ToString());
            Price = int.Parse(reader["price"].ToString());
            Created = DateTime.Parse(reader["created"].ToString());
            Updated = DateTime.Parse(reader["updated"].ToString());
            Notes = reader["notes"].ToString();
            CreatedBy = reader["processedBy"].ToString();
            EventId = Guid.Parse(reader["eventId"].ToString());
        }

        private static List<Supply> ListBy(Guid id, string byWhat)
        {
            List<Supply> supplies = new List<Supply>();
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT * FROM dbo.supplies WHERE itemId='{0}' ORDER BY Created DESC", itemId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Supply transaction = new Supply();
                        transaction.InitFromReader(list);
                        supplies.Add(transaction);
                    }
                }
            }
            return supplies;
        }

        public static List<Supply> ListByItem(Guid itemId) => ListBy(itemId, "itemId");

        public static List<Supply> ListByEvent(Guid eventId) => ListBy(eventId, "eventId");

        public bool Create(out Guid newId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.supplies (itemId, amount, price, notes, eventId) " +
                "OUTPUT INSERTED.SUPPLYID VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                ItemId, Amount, Price, Notes, EventId);
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
                string cmdString = string.Format("UPDATE dbo.supplies SET " +
                    "itemId='{0}', amount='{1}', price='{2}', notes='{3}', updated='{4}' OUTPUT INSERTED.SUPPLYID WHERE supplyId='{5}'",
                    ItemId, Amount, Price, Notes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SupplyId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                SQLResponse = command.ExecuteScalar().ToString();
                return Guid.TryParse(SQLResponse, out newId);
            }
        }
    }
}