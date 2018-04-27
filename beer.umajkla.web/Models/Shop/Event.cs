using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace beer.umajkla.ShopModel
{
    public partial class Event : IShopObject
    {
        public Event(Guid eventId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                string cmdString = string.Format("SELECT * FROM dbo.events WHERE eventId='{0}'", eventId);
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    InitFromReader(reader);
                }
            }
        }

        public Event()
        {

        }

        private void InitFromReader(SqlDataReader reader)
        {
            EventId = Guid.Parse(reader["eventId"].ToString());
            Name = reader["name"].ToString();
            DateFrom = DateTime.Parse(reader["dateFrom"].ToString());
            DateTo = DateTime.Parse(reader["dateTo"].ToString());
            LocationId = Guid.Parse(reader["locationId"].ToString());
            CreatedBy = reader["createdBy"].ToString();
            Description = reader["description"].ToString();
            Created = DateTime.Parse(reader["created"].ToString());
            Updated = DateTime.Parse(reader["updated"].ToString());
        }

        public static List<IShopObject> List()
        {
            List<IShopObject> events = new List<IShopObject>();
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT * FROM dbo.events");
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Event _event = new Event();
                        _event.InitFromReader(list);
                        events.Add(_event);
                    }
                }
            }
            return events;
        }

        public bool Create(out Guid newId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.events (name, dateFrom, dateTo, description, locationId) " +
                "OUTPUT INSERTED.EVENTID VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                Name, DateFrom.ToString("yyyy-MM-dd HH:mm:ss"), DateTo.ToString("yyyy-MM-dd HH:mm:ss"), Description, LocationId);
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
                string cmdString = string.Format("UPDATE dbo.events SET " +
                    "name='{0}', dateFrom='{1}', dateTo='{2}', description='{3}', locationId='{4}', updated='{5}' OUTPUT INSERTED.EVENTID WHERE eventId='{6}'",
                    Name, DateFrom.ToString("yyyy-MM-dd HH:mm:ss"), DateTo.ToString("yyyy-MM-dd HH:mm:ss"), Description, LocationId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), EventId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                SQLResponse = command.ExecuteScalar().ToString();
                return Guid.TryParse(SQLResponse, out newId);
            }
        }
    }
}