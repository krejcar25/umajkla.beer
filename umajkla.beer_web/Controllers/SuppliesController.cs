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
    public class SuppliesController : ApiController
    {
        // GET api/<controller>/5
        public HttpResponseMessage Get(string id)
        {
            var resp = new HttpResponseMessage();
            try
            {
                Guid itemId = Guid.Parse(id);
                resp.StatusCode = HttpStatusCode.OK;
                resp.Content = new StringContent(JsonConvert.SerializeObject(new Supply().List(itemId)), System.Text.Encoding.UTF8, "application/json");
                return resp;
            }
            catch (FormatException)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent("item guid has incorrect format", System.Text.Encoding.UTF8, "application/json");
                return resp;
            }
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]string json)
        {
            var resp = new HttpResponseMessage();
            Supply supply = new Supply(json);
            Guid createdId = supply.Create();
            if (createdId == Guid.Empty)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent(JsonConvert.SerializeObject(supply.SQLResponse), System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                resp.StatusCode = HttpStatusCode.Created;
                resp.Content = new StringContent(JsonConvert.SerializeObject(new Supply(createdId)), System.Text.Encoding.UTF8, "application/json");
            }
            return resp;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put([FromBody]string json)
        {
            var resp = new HttpResponseMessage();
            Supply supply = new Supply(json);
            Guid updatedId = supply.Update();
            if (updatedId == Guid.Empty)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent(JsonConvert.SerializeObject(supply.SQLResponse), System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                resp.StatusCode = HttpStatusCode.OK;
                resp.Content = new StringContent(JsonConvert.SerializeObject(new Supply(updatedId)), System.Text.Encoding.UTF8, "application/json");
            }
            return resp;
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(string id)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            Guid supplyId;
            try
            {
                supplyId = Guid.Parse(id);
            }
            catch (FormatException)
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent("supply guid has incorrect format", System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }

            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(string.Format("DELETE FROM dbo.supplies WHERE supplyId='{0}'", supplyId), connection);
                connection.Open();
                if (command.ExecuteNonQuery() == 1)
                {
                    resp.StatusCode = HttpStatusCode.OK;
                    resp.Content = new StringContent("deleted", System.Text.Encoding.UTF8, "text/plain");
                }
                else
                {
                    resp.StatusCode = HttpStatusCode.NotFound;
                    resp.Content = new StringContent("supply could not be found", System.Text.Encoding.UTF8, "text/plain");
                }
            }
            return resp;
        }
    }
}