using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;

namespace beer.umajkla.ShopModel
{
    public partial class VerifiedData<TData> where TData : IShopObject
    {
        public ValidityCheckResult IsRequestValid()
        {
            APIkey secret = null;
            DateTime allowedTimeMin = DateTime.UtcNow - TimeSpan.FromHours(1);
            DateTime allowedTimeMax = DateTime.UtcNow;
            if (DateTime < allowedTimeMin) return ValidityCheckResult.Expired;
            else if (DateTime > allowedTimeMax) return ValidityCheckResult.RequestTooEarly;
            else
            {
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    string cmdString = string.Format("SELECT secret FROM dbo.ValidAuthedClients WHERE clientId = '{0}'", ClientId);
                    connection.Open();
                    SqlCommand command = new SqlCommand(cmdString, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            secret = APIkey.Parse(reader["secret"].ToString());
                        }
                    }
                }
                if (secret == null) return ValidityCheckResult.KeyNotFound;
                else if (Hash == ComputeHash(secret)) return ValidityCheckResult.OK;
                else return ValidityCheckResult.InvalidHash;
            }
        }

        public HttpResponseMessage GetResponseMessage(GetResponseMessageAction<TData> action)
        {
            ValidityCheckResult result = IsRequestValid();

            if (result == ValidityCheckResult.OK)
            {
                HttpStatusCode statusCode;
                string content = action(this, out statusCode);
                return new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                };
            }
            else if (result == ValidityCheckResult.InvalidHash) return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("InvalidHash", System.Text.Encoding.UTF8, "text/plain")
            };
            else return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(result.GetDescription(), System.Text.Encoding.UTF8, "text/plain")
            };
        }

        public HttpResponseMessage GetResponseMessage(GetResponseMessageAction<TData> action, ControllerName controller, NoDataTask task)
        {
            ValidityCheckResult result = IsRequestValid();

            if (result == ValidityCheckResult.OK)
            {
                if (Data is NoDataRequest)
                {
                    NoDataRequest data = Data as NoDataRequest;
                    if (data.Controller == ControllerName.Customers && data.Task == NoDataTask.Get)
                    {
                        HttpStatusCode statusCode;
                        string content = action(this, out statusCode);
                        return new HttpResponseMessage()
                        {
                            StatusCode = statusCode,
                            Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else return new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StringContent("incorrect controller name or task", System.Text.Encoding.UTF8, "text/plain")
                    };
                }
                else throw new ArgumentException("TData must be NoDataRequest");
            }
            else if (result == ValidityCheckResult.InvalidHash) return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("InvalidHash", System.Text.Encoding.UTF8, "text/plain")
            };
            else return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(result.GetDescription(), System.Text.Encoding.UTF8, "text/plain")
            };
        }
    }

    public delegate string GetResponseMessageAction<T>(VerifiedData<T> provider, out HttpStatusCode statusCode) where T : IShopObject;
}