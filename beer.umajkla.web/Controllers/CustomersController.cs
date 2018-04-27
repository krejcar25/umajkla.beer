using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using beer.umajkla.ShopModel;

namespace beer.umajkla.web.Controllers
{
    public class CustomersController : ApiController
    {
        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            VerifiedData<NoDataRequest> verifiedData;
            if (Request.Headers.Contains("auth"))
            {
                verifiedData = JsonConvert.DeserializeObject<VerifiedData<NoDataRequest>>(Request.Headers.GetValues("auth").ElementAt(0));
                GetResponseMessageAction<NoDataRequest> action = (VerifiedData<NoDataRequest> data, out HttpStatusCode statusCode) =>
                {
                    if (data.Data.ListBy == ControllerName.Events)
                    {
                        statusCode = HttpStatusCode.OK;
                        return JsonConvert.SerializeObject(Customer.List(data.Data.OfId));
                    }
                    else
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        return "cannot list by this type";
                    }
                };
                return verifiedData.GetResponseMessage(action, ControllerName.Customers, NoDataTask.Get);
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("no authentication header was provided", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]string json)
        {
            VerifiedData<Customer> verifiedData = JsonConvert.DeserializeObject<VerifiedData<Customer>>(json);
            return verifiedData.GetResponseMessage((VerifiedData<Customer> data, out HttpStatusCode statusCode) =>
            {
                Guid createdId;
                if (data.Data.Create(out createdId))
                {
                    statusCode = HttpStatusCode.OK;
                    return JsonConvert.SerializeObject(new Customer(createdId));
                }
                else
                {
                    statusCode = HttpStatusCode.InternalServerError;
                    return JsonConvert.SerializeObject(data.Data.SQLResponse);
                }
            });
        }

        // PUT api/<controller>
        public HttpResponseMessage Put([FromBody]string json)
        {
            VerifiedData<Customer> verifiedData = JsonConvert.DeserializeObject<VerifiedData<Customer>>(json);
            return verifiedData.GetResponseMessage((VerifiedData<Customer> data, out HttpStatusCode statusCode) =>
            {
                Guid updatedId;
                if (data.Data.Update(out updatedId))
                {
                    statusCode = HttpStatusCode.OK;
                    return JsonConvert.SerializeObject(new Customer(updatedId));
                }
                else
                {
                    statusCode = HttpStatusCode.InternalServerError;
                    return JsonConvert.SerializeObject(data.Data.SQLResponse);
                }
            });
        }

        // DELETE api/<controller>
        public HttpResponseMessage Delete([FromBody]string json)
        {
            VerifiedData<NoDataRequest> verifiedData = JsonConvert.DeserializeObject<VerifiedData<NoDataRequest>>(json);
            GetResponseMessageAction<NoDataRequest> action = (VerifiedData<NoDataRequest> data, out HttpStatusCode statusCode) =>
            {
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format("DELETE FROM dbo.customers WHERE customerId='{0}'", data.Data.OfId), connection);
                    connection.Open();
                    if (command.ExecuteNonQuery() == 1)
                    {
                        statusCode = HttpStatusCode.OK;
                        return "deleted";
                    }
                    else
                    {
                        statusCode = HttpStatusCode.NotFound;
                        return "customer could not be found";
                    }
                }
            };
            return verifiedData.GetResponseMessage(action, ControllerName.Customers, NoDataTask.Delete);
        }
    }
}