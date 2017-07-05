using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using umajkla.beer.Models.Shop;
namespace umajkla.beer.Controllers
{
    public class LocationsController : ApiController
    {
        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            var resp = new HttpResponseMessage();
            resp.StatusCode = HttpStatusCode.OK;
            resp.Content = new StringContent(JsonConvert.SerializeObject(new Location().List()), System.Text.Encoding.UTF8, "application/json");
            return resp;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]string json)
        {
            var resp = new HttpResponseMessage();
            Location location = JsonConvert.DeserializeObject<Location>(json);
            Guid createdId = location.Create();
            if (createdId == Guid.Empty)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent(JsonConvert.SerializeObject(location.SQLResponse), System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                resp.StatusCode = HttpStatusCode.Created;
                resp.Content = new StringContent(JsonConvert.SerializeObject(new Location(createdId)), System.Text.Encoding.UTF8, "application/json");
            }
            return resp;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put([FromBody]string json)
        {
            var resp = new HttpResponseMessage();
            Location location = JsonConvert.DeserializeObject<Location>(json);
            Guid updatedId = location.Update();
            if (updatedId == Guid.Empty)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent(JsonConvert.SerializeObject(location.SQLResponse), System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                resp.StatusCode = HttpStatusCode.OK;
                resp.Content = new StringContent(JsonConvert.SerializeObject(new Location(updatedId)), System.Text.Encoding.UTF8, "application/json");
            }
            return resp;
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(string id)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            Guid locationId;
            try
            {
                locationId = Guid.Parse(id);
            }
            catch (FormatException)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent("location guid has incorrect format", System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }

            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(string.Format("DELETE FROM dbo.locations WHERE locationId='{0}'", locationId), connection);
                connection.Open();
                if (command.ExecuteNonQuery() == 1)
                {
                    resp.StatusCode = HttpStatusCode.OK;
                    resp.Content = new StringContent("deleted", System.Text.Encoding.UTF8, "text/plain");
                }
                else
                {
                    resp.StatusCode = HttpStatusCode.NotFound;
                    resp.Content = new StringContent("location could not be found", System.Text.Encoding.UTF8, "text/plain");
                }
            }
            return resp;
        }
    }
}