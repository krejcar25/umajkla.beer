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
    public class PaymentsController : ApiController
    {
        // GET api/<controller>/5
        public HttpResponseMessage Get(string id)
        {
            var resp = new HttpResponseMessage();
            string[] idExp = id.Split(new char[] { '=' });
            try
            {
                Guid guid = Guid.Parse(idExp[1]);
                resp.StatusCode = HttpStatusCode.OK;
                if (idExp[0] == "customer") resp.Content = new StringContent(JsonConvert.SerializeObject(new Payment().ListByCustomer(guid)), System.Text.Encoding.UTF8, "application/json");
                if (idExp[0] == "event") resp.Content = new StringContent(JsonConvert.SerializeObject(new Payment().ListByEvent(guid)), System.Text.Encoding.UTF8, "application/json");
                return resp;
            }
            catch (FormatException)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent("customer/event guid has incorrect format", System.Text.Encoding.UTF8, "application/json");
                return resp;
            }
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]string json)
        {
            var resp = new HttpResponseMessage();
            Payment payment = JsonConvert.DeserializeObject<Payment>(json);
            Guid createdId = payment.Create();
            if (createdId == Guid.Empty)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent(JsonConvert.SerializeObject(payment.SQLResponse), System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                resp.StatusCode = HttpStatusCode.Created;
                resp.Content = new StringContent(JsonConvert.SerializeObject(new Payment(createdId)), System.Text.Encoding.UTF8, "application/json");
            }
            return resp;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put([FromBody]string json)
        {
            var resp = new HttpResponseMessage();
            Payment payment = JsonConvert.DeserializeObject<Payment>(json);
            Guid updatedId = payment.Update();
            if (updatedId == Guid.Empty)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent(JsonConvert.SerializeObject(payment.SQLResponse), System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                resp.StatusCode = HttpStatusCode.OK;
                resp.Content = new StringContent(JsonConvert.SerializeObject(new Payment(updatedId)), System.Text.Encoding.UTF8, "application/json");
            }
            return resp;
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(string id)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            Guid paymentId;
            try
            {
                paymentId = Guid.Parse(id);
            }
            catch (FormatException)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent("payment guid has incorrect format", System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }

            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(string.Format("DELETE FROM dbo.payments WHERE paymentId='{0}'", paymentId), connection);
                connection.Open();
                if (command.ExecuteNonQuery() == 1)
                {
                    resp.StatusCode = HttpStatusCode.OK;
                    resp.Content = new StringContent("deleted", System.Text.Encoding.UTF8, "text/plain");
                }
                else
                {
                    resp.StatusCode = HttpStatusCode.NotFound;
                    resp.Content = new StringContent("payment could not be found", System.Text.Encoding.UTF8, "text/plain");
                }
            }
            return resp;
        }
    }
}