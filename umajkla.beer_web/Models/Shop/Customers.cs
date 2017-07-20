using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace umajkla.beer.Models.Shop
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid EventId { get; set; }
        public string CreatedBy { get; set; }
        public string SQLResponse { get; set; }

        public Customer(Guid customerId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                string cmdString = string.Format("SELECT * FROM dbo.customers WHERE customerId='{0}'", customerId);
                SqlCommand command = new SqlCommand(cmdString, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    CustomerId = Guid.Parse(reader["customerId"].ToString());
                    Name = reader["name"].ToString();
                    Address = reader["address"].ToString();
                    Phone = reader["phone"].ToString();
                    Email = reader["email"].ToString();
                    Notes = reader["notes"].ToString();
                    Created = DateTime.Parse(reader["created"].ToString());
                    Updated = DateTime.Parse(reader["updated"].ToString());
                    EventId = Guid.Parse(reader["eventId"].ToString());
                    CreatedBy = reader["createdBy"].ToString();
                }
            }
        }

        /*public Customer(string dataJson)
        {
            dynamic data = System.Web.Helpers.Json.Decode(dataJson);

            if (!string.IsNullOrEmpty(data.CustomerId.ToString())) CustomerId = Guid.Parse(data.CustomerId.ToString());
            Name = data.Name.ToString();
            Address = data.Address.ToString();
            Phone = data.Phone.ToString();
            Email = data.Email.ToString();
            Notes = data.Notes.ToString();
            if (!string.IsNullOrEmpty(data.Created.ToString())) Created = DateTime.Parse(data.Created.ToString());
            if (!string.IsNullOrEmpty(data.Updated.ToString())) Updated = DateTime.Parse(data.Updated.ToString());
            EventId = Guid.Parse(data.EventId.ToString());
            CreatedBy = data.CreatedBy.ToString();
        }*/

        public Customer()
        {

        }

        public List<Customer> List(Guid eventId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT customerId FROM dbo.customers WHERE eventId='{0}' ORDER BY Name ASC", eventId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                List<Customer> customers = new List<Customer>();
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Customer customer = new Customer(Guid.Parse(list["customerId"].ToString()));
                        customers.Add(customer);
                    }
                }

                return customers;
            }
        }

        public Guid Create()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.customers (name, address, phone, email, notes, eventId) " +
                "OUTPUT INSERTED.CUSTOMERID VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                Name, Address, Phone, Email, Notes, EventId);
                connection.Open();
                try
                {
                    SqlCommand command = new SqlCommand(cmdString, connection);
                    SQLResponse = command.ExecuteScalar().ToString();
                    return Guid.Parse(SQLResponse);
                }
                catch (Exception)
                {
                    return Guid.Empty;
                }
            }
        }

        public Guid Update()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("UPDATE dbo.customers SET " +
                    "name='{0}', address='{1}', phone='{2}', email='{3}', notes='{4}', updated='{5}', eventId='{6}' OUTPUT INSERTED.CUSTOMERID WHERE customerId='{7}'",
                    Name, Address, Phone, Email, Notes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), EventId, CustomerId);
                connection.Open();
                try
                {
                    SqlCommand command = new SqlCommand(cmdString, connection);
                    SQLResponse = command.ExecuteScalar().ToString();
                    return Guid.Parse(SQLResponse);
                }
                catch (Exception)
                {
                    return Guid.Empty;
                }
            }
        }
    }
}