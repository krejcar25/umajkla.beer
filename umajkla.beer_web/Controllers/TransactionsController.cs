using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Web.Configuration;
using Newtonsoft.Json;
using System.Web.Helpers;
using umajkla.beer.Models.Shop;

namespace umajkla.beer.Controllers
{
    public class TransactionsController : ApiController
    {
        // GET api/<controller>/5
        public HttpResponseMessage Get(string id)
        {
            var resp = new HttpResponseMessage();
            string[] idExp = id.Split(new char[]{ '='});
            try
            {
                Guid guid = Guid.Parse(idExp[1]);
                resp.StatusCode = HttpStatusCode.OK;
                if (idExp[0] == "customer") resp.Content = new StringContent(JsonConvert.SerializeObject(new Transaction().ListByCustomer(guid)), System.Text.Encoding.UTF8, "application/json");
                if (idExp[0] == "item") resp.Content = new StringContent(JsonConvert.SerializeObject(new Transaction().ListByItem(guid)), System.Text.Encoding.UTF8, "application/json");
                return resp;
            }
            catch (FormatException)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent("customer/item guid has incorrect format", System.Text.Encoding.UTF8, "application/json");
                return resp;
            }
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]string json)
        {
            var resp = new HttpResponseMessage();
            Transaction transaction = JsonConvert.DeserializeObject<Transaction>(json);
            Guid createdId = transaction.Create();
            if (createdId == Guid.Empty)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent(JsonConvert.SerializeObject(transaction.SQLResponse), System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                resp.StatusCode = HttpStatusCode.Created;
                resp.Content = new StringContent(JsonConvert.SerializeObject(new Transaction(createdId)), System.Text.Encoding.UTF8, "application/json");
            }
            return resp;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put([FromBody]string json)
        {
            var resp = new HttpResponseMessage();
            Transaction transaction = JsonConvert.DeserializeObject<Transaction>(json);
            Guid updatedId = transaction.Update();
            if (updatedId == Guid.Empty)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent(JsonConvert.SerializeObject(transaction.SQLResponse), System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                resp.StatusCode = HttpStatusCode.OK;
                resp.Content = new StringContent(JsonConvert.SerializeObject(new Transaction(updatedId)), System.Text.Encoding.UTF8, "application/json");
            }
            return resp;
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(string id)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            Guid transactionId;
            try
            {
                transactionId = Guid.Parse(id);
            }
            catch (FormatException)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent("transaction guid has incorrect format", System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }

            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(string.Format("DELETE FROM dbo.transactions WHERE transactionId='{0}'", transactionId), connection);
                connection.Open();
                if (command.ExecuteNonQuery() == 1)
                {
                    resp.StatusCode = HttpStatusCode.OK;
                    resp.Content = new StringContent("deleted", System.Text.Encoding.UTF8, "text/plain");
                }
                else
                {
                    resp.StatusCode = HttpStatusCode.NotFound;
                    resp.Content = new StringContent("transaction could not be found", System.Text.Encoding.UTF8, "text/plain");
                }
            }
            return resp;
        }
    }
}