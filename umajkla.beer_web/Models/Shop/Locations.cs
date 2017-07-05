using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace umajkla.beer.Models.Shop
{
    public class Location
    {
        public Guid LocationId { get; set; }
        public string Name { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string CreatedBy { get; set; }
        public string SQLResponse { get; set; }

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
            }
        }

        public Location(string dataJson)
        {
            dynamic data = System.Web.Helpers.Json.Decode(dataJson);

            if (!string.IsNullOrEmpty(data.LocationId.ToString())) LocationId = Guid.Parse(data.LocationId.ToString());
            Name = data.Name.ToString();
            Street1 = data.Street1.ToString();
            Street2 = data.Street2.ToString();
            City = data.City.ToString();
            Postcode = data.Postcode.ToString();
            CountryCode = data.Countrycode.ToString();
            Latitude = float.Parse(data.lLatitude.ToString());
            Longitude = float.Parse(data.Longitude.ToString());
            if (!string.IsNullOrEmpty(data.Created.ToString())) Created = DateTime.Parse(data.Created.ToString());
            if (!string.IsNullOrEmpty(data.Updated.ToString())) Updated = DateTime.Parse(data.Updated.ToString());
            CreatedBy = data.CreatedBy.ToString();
        }

        public Location()
        {
                
        }

        public List<Location> List()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT locationId FROM dbo.locations");
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                List<Location> locations = new List<Location>();
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Location location = new Location(Guid.Parse(list["locationId"].ToString()));
                        locations.Add(location);
                    }
                }
                return locations;
            }
        }

        public Guid Create()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.locations (street1, street2, city, postcode, countrycode, latitude, longitude) " +
                "OUTPUT INSERTED.ID VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')",
                Street1, Street2, City, Postcode, CountryCode, Latitude, Longitude);
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
                string cmdString = string.Format("UPDATE dbo.locations SET street1 = {0}, street2 = {1}, city = {2}, postcode = {3}, countrycode = {4}, updated = {5}, latitude = {6}, longitude = {7}" +
                    "OUTPUT INSERTED.ID WHERE locationId = '{8}'",
                    Street1, Street2, City, Postcode, CountryCode, Latitude, Longitude, LocationId);
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

        #region All Countries List Generator
        public static Dictionary<string, string> GetAllCountries()
        {
            Dictionary<string, string> objDicUnsorted = new Dictionary<string, string>();

            foreach (CultureInfo ObjCultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo objRegionInfo = new RegionInfo(ObjCultureInfo.Name);
                Console.WriteLine("Adding " + objRegionInfo.Name);
                if (!objDicUnsorted.ContainsKey(objRegionInfo.TwoLetterISORegionName))
                {
                    if (objRegionInfo.NativeName != objRegionInfo.EnglishName)
                    {
                        objDicUnsorted.Add(objRegionInfo.TwoLetterISORegionName, objRegionInfo.NativeName + " (" + objRegionInfo.EnglishName + ")");
                    }
                    else
                    {
                        objDicUnsorted.Add(objRegionInfo.TwoLetterISORegionName, objRegionInfo.NativeName);
                    }
                }
            }

            Dictionary<string, string> objDic = new Dictionary<string, string>();
            string[] keys = objDicUnsorted.Keys.ToArray();
            Array.Sort(keys);
            foreach (var item in keys)
            {
                objDic.Add(item, objDicUnsorted[item]);
            }

            return objDic;
        }
        #endregion
    }
}