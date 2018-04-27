using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace beer.umajkla.ShopModel
{
    public partial class Location : IShopObject
    {
        public Location(Guid locationId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                string cmdString = string.Format("SELECT * FROM dbo.locations WHERE locationId='{0}'", locationId);
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    InitFromReader(reader);
                }
            }
        }

        public Location()
        {
                
        }

        private void InitFromReader(SqlDataReader reader)
        {
            LocationId = Guid.Parse(reader["locationId"].ToString());
            Name = reader["name"].ToString();
            Street1 = reader["street1"].ToString();
            Street2 = reader["street2"].ToString();
            City = reader["city"].ToString();
            Postcode = reader["postcode"].ToString();
            CountryCode = reader["countrycode"].ToString();
            Latitude = float.Parse(reader["latitude"].ToString());
            Longitude = float.Parse(reader["longitude"].ToString());
            Created = DateTime.Parse(reader["created"].ToString());
            Updated = DateTime.Parse(reader["updated"].ToString());
            CreatedBy = reader["createdBy"].ToString();
        }

        public static List<Location> List()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT * FROM dbo.locations");
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                List<Location> locations = new List<Location>();
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Location location = new Location();
                        location.InitFromReader(list);
                        locations.Add(location);
                    }
                }
                return locations;
            }
        }

        public bool Create(out Guid newId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format(CultureInfo.GetCultureInfo("en-US"), "INSERT INTO dbo.locations (street1, street2, city, postcode, countrycode, latitude, longitude, name) " +
                "OUTPUT INSERTED.LOCATIONID VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5:0.0000000000}', '{6:0.0000000000}', '{7}')",
                Street1, Street2, City, Postcode, CountryCode, Latitude, Longitude, Name);
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
                string cmdString = string.Format("UPDATE dbo.locations SET street1 = '{0}', street2 = '{1}', city = '{2}', postcode = '{3}', countrycode = '{4}', updated = '{5}', latitude = '{6}', longitude = '{7}', name = '{8}'" +
                    "OUTPUT INSERTED.LOCATIONID WHERE locationId='{9}'",
                    Street1, Street2, City, Postcode, CountryCode, DateTime.Now, Latitude, Longitude, Name, LocationId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                SQLResponse = command.ExecuteScalar().ToString();
                return Guid.TryParse(SQLResponse, out newId);
            }
        }
    }
}