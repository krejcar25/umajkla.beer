using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Controllers;
using beer.umajkla.ShopModel;

namespace beer.umajkla.web.Controllers
{
    public class AppSecurityController : ApiController
    {
        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Not all headers are present", System.Text.Encoding.UTF8, "text/plain") };
            if (Request.Headers.Contains("username") && Request.Headers.Contains("token"))
            {
                string username = Request.Headers.GetValues("username").First();
                string token = Request.Headers.GetValues("token").First();

                string connectionString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                LoginData loginData = new LoginData()
                {
                    ReauthToken = new APIkey(),
                    ReauthSecret = new APIkey(),
                    ClientValidThru = DateTime.Now.AddDays(1),
                    ReauthValidThru = DateTime.Now.AddDays(7)
                };

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string cmdString = string.Format("EXECUTE [dbo].[InvalidateAPIKey] '{0}', '{1}', '{2}', '{3}', '{4}', '{5}'",
                    username, token, loginData.ClientValidThru.ToString("yyyy-MM-dd HH:mm:ss"), loginData.ReauthToken, loginData.ReauthSecret, loginData.ReauthValidThru.ToString("yyyy-MM-dd HH:mm:ss"));
                    SqlCommand command = new SqlCommand(cmdString, connection);
                    connection.Open();
                    string cmdResponse = command.ExecuteScalar().ToString();
                    Guid clientId = Guid.Empty;
                    if (Guid.TryParse(cmdResponse, out clientId))
                    {
                        loginData.ClientId = clientId;
                        response.StatusCode = HttpStatusCode.OK;
                        response.Content = new StringContent(JsonConvert.SerializeObject(loginData), System.Text.Encoding.UTF8, "application/json");
                    }
                    else
                    {
                        response.Content = new StringContent("token not found", System.Text.Encoding.UTF8, "text/plain");
                    }
                }
            }

            return response;
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(string id)
        {
            return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]string value)
        {
            return new HttpResponseMessage(HttpStatusCode.NotImplemented);
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(int id, [FromBody]string value)
        {
            return new HttpResponseMessage(HttpStatusCode.NotImplemented);
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(int id)
        {
            return new HttpResponseMessage(HttpStatusCode.NotImplemented);
        }
    }
}