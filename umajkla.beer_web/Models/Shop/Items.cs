using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace beer.umajkla.web.Models.Shop
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
        public double DisplayMultiplier { get; set; }
        public double DefaultSize { get; set; }
        public double Size1 { get; set; }
        public double Size2 { get; set; }
        public string Size1Label { get; set; }
        public string Size2Label { get; set; }

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
                    EventId = Guid.Parse(reader["eventId"].ToString());
                    CreatedBy = reader["createdBy"].ToString();
                    DisplayMultiplier = double.Parse(reader["displayMultiplier"].ToString());
                    DefaultSize = double.Parse(reader["defaultSize"].ToString());
                    Size1 = double.Parse(reader["size1"].ToString());
                    Size2 = double.Parse(reader["size2"].ToString());
                    Size1Label = reader["size1label"].ToString();
                    Size2Label = reader["size2label"].ToString();
                }
            }
        }

        /*public Item(string dataJson)
        {
            dynamic data = System.Web.Helpers.Json.Decode(dataJson);

            if (!string.IsNullOrEmpty(data.ItemId.ToString())) ItemId = Guid.Parse(data.ItemId.ToString());
            Name = data.Name.ToString();
            Price = int.Parse(data.Price.ToString());
            Unit = data.Unit.ToString();
            if (!string.IsNullOrEmpty(data.Created.ToString())) Created = DateTime.Parse(data.Created.ToString());
            if (!string.IsNullOrEmpty(data.Updated.ToString())) Updated = DateTime.Parse(data.Updated.ToString());
            if (!string.IsNullOrEmpty(data.Notes.ToString())) Notes = data.Notes.ToString();
            CreatedBy = data.CreatedBy.ToString();
        }*/

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
                string cmdString = string.Format("INSERT INTO dbo.items (name, price, unit, notes, eventId, displayMultiplier, defaultSize, size1, size2, size1label, size2label) " +
                "OUTPUT INSERTED.ITEMID VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}')",
                Name, Price, Unit, Notes, EventId, DisplayMultiplier, DefaultSize, Size1, Size2, Size1Label, Size2Label);
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

        public Guid Update()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("UPDATE dbo.items SET " +
                    "name='{0}', price='{1}', unit='{2}', notes='{3}', updated='{4}', eventId='{5}', displayMultiplier='{6}', defaultSize='{7}', size1='{8}', size2='{9}', size1label='{10}', size2label='{11}' OUTPUT INSERTED.ITEMID WHERE itemId='{12}'",
                    Name, Price, Unit, Notes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), EventId, DisplayMultiplier, DefaultSize, Size1, Size2, Size1Label, Size2Label, ItemId);
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