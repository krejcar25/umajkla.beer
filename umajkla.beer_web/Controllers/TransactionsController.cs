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

namespace umajkla.beer.Controllers
{
    public class TransactionsController : ApiController
    {
        // GET api/<controller>/5
        public HttpResponseMessage Get(string task, int id = 0)
        {
            var resp = new HttpResponseMessage();
            if (task == "list")
            {
                resp.StatusCode = HttpStatusCode.OK;
                resp.Content = new StringContent(JsonConvert.SerializeObject(Models.ShopModel.GetTransactionsList()), System.Text.Encoding.UTF8, "application/json");
            }
            else if (task == "show")
            {
                if (id==0)
                {
                    resp.StatusCode = HttpStatusCode.Ambiguous;
                    resp.Content = new StringContent("information missing", System.Text.Encoding.UTF8, "text/plain");
                }
                else
                {
                    resp.StatusCode = HttpStatusCode.OK;
                    resp.Content = new StringContent(JsonConvert.SerializeObject(Models.ShopModel.GetTransactionDetail(id)), System.Text.Encoding.UTF8, "application/json");
                }
            } else
            {
                resp.StatusCode = HttpStatusCode.NotImplemented;
                resp.Content = new StringContent("requested feature could not be understood", System.Text.Encoding.UTF8, "text/plain");
            }
            return resp;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]string details)
        {
            var resp = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent(JsonConvert.SerializeObject(Models.ShopModel.CreateTransaction(details)), System.Text.Encoding.UTF8, "application/json")
            };
            return resp;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put([FromBody]string details)
        {
            var resp = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(Models.ShopModel.UpdateTransaction(details)), System.Text.Encoding.UTF8, "application/json")
            };
            return resp;
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(string task)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(string.Format("DELETE FROM dbo.transactions WHERE id='{0}'", task), connection);
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