using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace beer.umajkla.web.Models.Shop
{
    public class Event
    {
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public Guid LocationId { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string SQLResponse { get; set; }

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
                    EventId = Guid.Parse(reader["eventId"].ToString());
                    Name = reader["name"].ToString();
                    DateFrom = DateTime.Parse(reader["dateFrom"].ToString());
                    DateTo = DateTime.Parse(reader["dateTo"].ToString());
                    string locationId = reader["locationId"].ToString();
                    LocationId = Guid.Parse(locationId);
                    CreatedBy = reader["createdBy"].ToString();
                    Description = reader["description"].ToString();
                    Created = DateTime.Parse(reader["created"].ToString());
                    Updated = DateTime.Parse(reader["updated"].ToString());
                }
            }
        }

        /*public Event(string dataJson)
        {
            dynamic data = System.Web.Helpers.Json.Decode(dataJson);

            EventId = Guid.Parse(data.EventId.ToString());
            Name = data.Name.ToString();
            DateFrom = DateTime.Parse(data.DateFrom.ToString());
            DateTo = DateTime.Parse(data.DateTo.ToString());
            LocationId = Guid.Parse(data.LocationId.ToString());
            CreatedBy = data.CreatedBy.ToString();
            Description = data.Description.ToString();
            Created = DateTime.Parse(data.Created.ToString());
            Updated = DateTime.Parse(data.Updated.ToString());
        }*/

        public Event()
        {

        }

        public List<Event> List()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT eventId FROM dbo.events");
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                List<Event> events = new List<Event>();
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Event _event = new Event(Guid.Parse(list["eventId"].ToString()));
                        events.Add(_event);
                    }
                }

                return events;
            }
        }

        public Guid Create()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.events (name, dateFrom, dateTo, description, locationId) " +
                "OUTPUT INSERTED.EVENTID VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                Name, DateFrom.ToString("yyyy-MM-dd HH:mm:ss"), DateTo.ToString("yyyy-MM-dd HH:mm:ss"), Description, LocationId);
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
                string cmdString = string.Format("UPDATE dbo.events SET " +
                    "name='{0}', dateFrom='{1}', dateTo='{2}', description='{3}', locationId='{4}', updated='{5}' OUTPUT INSERTED.EVENTID WHERE eventId='{6}'",
                    Name, DateFrom.ToString("yyyy-MM-dd HH:mm:ss"), DateTo.ToString("yyyy-MM-dd HH:mm:ss"), Description, LocationId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), EventId);
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