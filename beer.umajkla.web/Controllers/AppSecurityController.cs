using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace beer.umajkla.web.Controllers
{
    public class AppSecurityController : ApiController
    {
        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            if (Request.Headers.Contains("username") && Request.Headers.Contains("token") && Request.Headers.Contains("publicKey"))
            {
                string username = Request.Headers.GetValues("username").First();
                string token = Request.Headers.GetValues("token").First();
                string publicKey = Request.Headers.GetValues("publicKey").First();

                LoginData? data = Login(username, token);

                if (data == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Login failed") };
                }
                else
                {
                    string json = JsonConvert.SerializeObject(data);
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(Crypto.Encrypt(publicKey, json)) };
                }
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Not all headers are present") };
            }
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(string id)
        {
            return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
        }

        private LoginData? Login(string username, string token)
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            // Invalidate API token
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmdString = string.Format("UPDATE dbo.ValidAPIkeys SET used=1 WHERE username='{0}' AND token='{1}'",
                    username, token);
                SqlCommand command = new SqlCommand(cmdString, connection);
                connection.Open();
                if (command.ExecuteNonQuery() != 1) return null;
            }

            Crypto.KeyPair serverKeyPair = Crypto.CreateKeyPair();

            LoginData data = new LoginData();

            DateTime validThru = DateTime.Now.AddDays(1);
            data.ValidThru = validThru;

            // Register client
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.AuthedClients (userId, serverPriv) OUTPUT INSERTED.CLIENTID " +
                    "VALUES ((SELECT id FROM dbo.AspNetUsers WHERE UserName='{0}'), '{1}')", username, serverKeyPair.PrivateKey);
                SqlCommand command = new SqlCommand(cmdString, connection);
                Guid keySetId = Guid.Empty;
                connection.Open();
                Guid.TryParse(command.ExecuteScalar().ToString(), out keySetId);
                data.KeySetId = keySetId;
                data.PublicKey = serverKeyPair.PublicKey;
            }

            // Create ReauthToken
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string reauthToken = Crypto.RandomAPIKey();
                string cmdString = string.Format("INSERT INTO dbo.ValidAPIkeys(username, token) VALUES('{0}', '{1}')", username, reauthToken);
                SqlCommand command = new SqlCommand(cmdString, connection);
                connection.Open();
                bool success = command.ExecuteNonQuery() == 2;
                data.ReauthToken = (success) ? reauthToken : "PerformManual";
            }

            // Get user permissions
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmdString = string.Format("SELECT permissions FROM dbo.ValidPermissions WHERE UserName='{0}'", username);
                SqlCommand command = new SqlCommand(cmdString, connection);
                connection.Open();
                string permissions = command.ExecuteScalar().ToString();
                data.Permissions = permissions;
            }

            return data;
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

        private struct LoginData
        {
            public Guid KeySetId { get; set; }
            public string Permissions { get; set; }
            public DateTime ValidThru { get; set; }
            public string ReauthToken { get; set; }
            public string PublicKey { get; set; }
        }
    }
}