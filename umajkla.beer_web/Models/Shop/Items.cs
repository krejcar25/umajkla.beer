using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace umajkla.beer.Models.Shop
{
	public class Item
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Unit { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Notes { get; set; }
        public Guid EventId { get; set; }
        public string CreatedBy { get; set; }
        public string SQLResponse { get; set; }

        public Item(Guid itemId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                string cmdString = string.Format("SELECT * FROM dbo.items WHERE itemId='{0}'", itemId);
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    ItemId = Guid.Parse(reader["itemId"].ToString());
                    Name = reader["name"].ToString();
                    Price = int.Parse(reader["price"].ToString());
                    Unit = reader["unit"].ToString();
                    Created = DateTime.Parse(reader["created"].ToString());
                    Updated = DateTime.Parse(reader["updated"].ToString());
                    Notes = reader["notes"].ToString();
                    CreatedBy = reader["createdBy"].ToString();
                }
            }
        }

        public Item(string dataJson)
        {
            dynamic data = System.Web.Helpers.Json.Decode(dataJson);

            if (!string.IsNullOrEmpty(data.ItemId.ToString())) ItemId = Guid.Parse(data.ItemId.ToString());
            Name = data.Name.ToString();
            Price = int.Parse(data.Price.ToString());
            Unit = data.Unit.ToString();
            if (!string.IsNullOrEmpty(data.Created.ToString())) Created = DateTime.Parse(data.Created.ToString());
            if (!string.IsNullOrEmpty(data.Updated.ToString())) Updated = DateTime.Parse(data.Updated.ToString());
            Notes = data.Notes.ToString();
            CreatedBy = data.CreatedBy.ToString();
        }

        public Item()
        {

        }

        public List<Item> List(Guid eventId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT itemId FROM dbo.items WHERE eventId='{0}'", eventId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                List<Item> items = new List<Item>();
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Item item = new Item(Guid.Parse(list["itemId"].ToString()));
                        items.Add(item);
                    }
                }
                return items;
            }
        }

        public Guid Create()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.goods (name, price, unit, notes, eventId) " +
                "OUTPUT INSERTED.ID VALUES ('{0}', '{1}', '{2}', '{3}, {4}')",
                Name, Price, Unit, Notes, EventId);
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
                string cmdString = string.Format("UPDATE dbo.goods SET " +
                    "name='{0}', price='{1}', unit='{2}', notes='{3}', updated='{4}', eventId='{5}' OUTPUT INSERTED.ID WHERE id='{6}'",
                    Name, Price, Unit, Notes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), EventId, ItemId);
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