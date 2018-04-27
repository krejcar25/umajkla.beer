using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace beer.umajkla.ShopModel
{
	public partial class Item : IShopObject
    {
        public Item(Guid itemId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                string cmdString = string.Format("SELECT * FROM dbo.ValidItems WHERE itemId='{0}'", itemId);
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    InitFromReader(reader);
                    AmountInPack = -1;
                }
            }
        }

        public List<Item> GetPackItems()
        {
            List<Item> items = new List<Item>();
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                string cmdString = string.Format("SELECT * FROM dbo.ValidPackItems WHERE itemSetId='{0}'", ItemSetId);
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Item item = new Item();
                        item.InitFromReader(reader);
                        AmountInPack = double.Parse(reader["amountInPack"].ToString());
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public Item()
        {

        }

        private void InitFromReader(SqlDataReader reader)
        {
            ItemId = Guid.Parse(reader["itemId"].ToString());
            Name = reader["name"].ToString();
            Price = int.Parse(reader["price"].ToString());
            Unit = reader["unit"].ToString();
            Created = DateTime.Parse(reader["created"].ToString());
            Updated = DateTime.Parse(reader["updated"].ToString());
            Notes = reader["notes"].ToString();
            EventId = Guid.Parse(reader["eventId"].ToString());
            CreatedBy = reader["createdBy"].ToString();
            DisplayMultiplier = double.Parse(reader["displayMultiplier"].ToString());
            DefaultSize = double.Parse(reader["defaultSize"].ToString());
            Size1 = double.Parse(reader["size1"].ToString());
            Size2 = double.Parse(reader["size2"].ToString());
            Size1Label = reader["size1label"].ToString();
            Size2Label = reader["size2label"].ToString();
            PreviousVersion = (reader["previousVersion"] != null) ? new Item(Guid.Parse(reader["previousVersion"].ToString())) : null;
            ItemSetId = Guid.Parse(reader["itemSetId"].ToString());
            PackItems = (bool.Parse(reader["isPack"].ToString())) ? GetPackItems() : null;
            Supplies = Supply.ListByItem(ItemId);
            Transactions = Transaction.ListByItem(ItemId);
        }

        public static List<Item> List(Guid eventId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT * FROM dbo.ValidItems WHERE eventId='{0}'", eventId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                List<Item> items = new List<Item>();
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Item item = new Item();
                        item.InitFromReader(list);
                        items.Add(item);
                    }
                }
                return items;
            }
        }

        public bool Create(out Guid newId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.ValidItems (name, price, unit, notes, eventId, displayMultiplier, defaultSize, size1, size2, size1label, size2label) " +
                "OUTPUT INSERTED.ITEMID VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}')",
                Name, Price, Unit, Notes, EventId, DisplayMultiplier, DefaultSize, Size1, Size2, Size1Label, Size2Label);
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
                string cmdString = string.Format("UPDATE dbo.ValidItems SET " +
                    "name='{0}', price='{1}', unit='{2}', notes='{3}', updated='{4}', eventId='{5}', displayMultiplier='{6}', defaultSize='{7}', size1='{8}', size2='{9}', size1label='{10}', size2label='{11}' OUTPUT INSERTED.ITEMID WHERE itemId='{12}'",
                    Name, Price, Unit, Notes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), EventId, DisplayMultiplier, DefaultSize, Size1, Size2, Size1Label, Size2Label, ItemId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                SQLResponse = command.ExecuteScalar().ToString();
                return Guid.TryParse(SQLResponse, out newId);
            }
        }
    }
}