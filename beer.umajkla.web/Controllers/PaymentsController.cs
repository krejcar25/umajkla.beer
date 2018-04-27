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
    public class PaymentsController : ApiController
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
                    if (data.Data.ListBy == ControllerName.Customers)
                    {
                        statusCode = HttpStatusCode.OK;
                        return JsonConvert.SerializeObject(Payment.ListByCustomer(data.Data.OfId));
                    }
                    else if (data.Data.ListBy == ControllerName.Events)
                    {
                        statusCode = HttpStatusCode.OK;
                        return JsonConvert.SerializeObject(Payment.ListByEvent(data.Data.OfId));
                    }
                    else
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        return "cannot list by this type";
                    }
                };
                return verifiedData.GetResponseMessage(action, ControllerName.Payments, NoDataTask.Get);
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
            VerifiedData<Payment> verifiedData = JsonConvert.DeserializeObject<VerifiedData<Payment>>(json);
            return verifiedData.GetResponseMessage((VerifiedData<Payment> data, out HttpStatusCode statusCode) =>
            {
                Guid createdId;
                if (data.Data.Create(out createdId))
                {
                    statusCode = HttpStatusCode.OK;
                    return JsonConvert.SerializeObject(new Payment(createdId));
                }
                else
                {
                    statusCode = HttpStatusCode.InternalServerError;
                    return JsonConvert.SerializeObject(data.Data.SQLResponse);
                }
            });
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put([FromBody]string json)
        {
            VerifiedData<Payment> verifiedData = JsonConvert.DeserializeObject<VerifiedData<Payment>>(json);
            return verifiedData.GetResponseMessage((VerifiedData<Payment> data, out HttpStatusCode statusCode) =>
            {
                Guid updatedId;
                if (data.Data.Update(out updatedId))
                {
                    statusCode = HttpStatusCode.OK;
                    return JsonConvert.SerializeObject(new Payment(updatedId));
                }
                else
                {
                    statusCode = HttpStatusCode.InternalServerError;
                    return JsonConvert.SerializeObject(data.Data.SQLResponse);
                }
            });
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete([FromBody]string json)
        {
            VerifiedData<NoDataRequest> verifiedData = JsonConvert.DeserializeObject<VerifiedData<NoDataRequest>>(json);
            GetResponseMessageAction<NoDataRequest> action = (VerifiedData<NoDataRequest> data, out HttpStatusCode statusCode) =>
            {
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format("DELETE FROM dbo.payments WHERE paymentId='{0}'", data.Data.OfId), connection);
                    connection.Open();
                    if (command.ExecuteNonQuery() == 1)
                    {
                        statusCode = HttpStatusCode.OK;
                        return "deleted";
                    }
                    else
                    {
                        statusCode = HttpStatusCode.NotFound;
                        return "payment could not be found";
                    }
                }
            };
            return verifiedData.GetResponseMessage(action, ControllerName.Payments, NoDataTask.Delete);
        }
    }
}