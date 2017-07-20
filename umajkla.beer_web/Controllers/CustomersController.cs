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
    public class CustomersController : ApiController
    {
        // GET api/<controller>/5
        public HttpResponseMessage Get(string id)
        {
            var resp = new HttpResponseMessage();
            try
            {
                Guid eventId = Guid.Parse(id);
                resp.StatusCode = HttpStatusCode.OK;
                resp.Content = new StringContent(JsonConvert.SerializeObject(new Customer().List(eventId)), System.Text.Encoding.UTF8, "application/json");
                return resp;
            }
            catch (FormatException)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent("event guid has incorrect format", System.Text.Encoding.UTF8, "application/json");
                return resp;
            }
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]string json)
        {
            var resp = new HttpResponseMessage();
            Customer customer = JsonConvert.DeserializeObject<Customer>(json);
            Guid createdId = customer.Create();
            if (createdId == Guid.Empty)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent(JsonConvert.SerializeObject(customer.SQLResponse), System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                resp.StatusCode = HttpStatusCode.Created;
                resp.Content = new StringContent(JsonConvert.SerializeObject(new Customer(createdId)), System.Text.Encoding.UTF8, "application/json");
            }
            return resp;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put([FromBody]string json)
        {
            var resp = new HttpResponseMessage();
            Customer customer = JsonConvert.DeserializeObject<Customer>(json);
            Guid updatedId = customer.Update();
            if (updatedId == Guid.Empty)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent(JsonConvert.SerializeObject(customer.SQLResponse), System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                resp.StatusCode = HttpStatusCode.OK;
                resp.Content = new StringContent(JsonConvert.SerializeObject(new Customer(updatedId)), System.Text.Encoding.UTF8, "application/json");
            }
            return resp;
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(string id)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            Guid customerId;
            try
            {
                customerId = Guid.Parse(id);
            }
            catch (FormatException)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent("customer guid has incorrect format", System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }

            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(string.Format("DELETE FROM dbo.customers WHERE customerId='{0}'", customerId), connection);
                connection.Open();
                if (command.ExecuteNonQuery() == 1)
                {
                    resp.StatusCode = HttpStatusCode.OK;
                    resp.Content = new StringContent("deleted", System.Text.Encoding.UTF8, "text/plain");
                }
                else
                {
                    resp.StatusCode = HttpStatusCode.NotFound;
                    resp.Content = new StringContent("customer could not be found", System.Text.Encoding.UTF8, "text/plain");
                }
            }
            return resp;
        }
    }
}