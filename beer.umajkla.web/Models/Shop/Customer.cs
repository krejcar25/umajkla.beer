using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using beer.umajkla.ShopModel;

namespace beer.umajkla.ShopModel
{
    public partial class Customer : IShopObject
    {
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
                    InitFromReader(reader);
                }
            }
        }

        public Customer()
        {

        }

        private void InitFromReader(SqlDataReader reader)
        {
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
            Transactions = Transaction.ListByCustomer(CustomerId);
            Payments = Payment.ListByCustomer(CustomerId);
        }

        public static List<Customer> List(Guid eventId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT * FROM dbo.customers WHERE eventId='{0}' ORDER BY Name ASC", eventId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                List<Customer> customers = new List<Customer>();
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Customer customer = new Customer();
                        customer.InitFromReader(list);
                        customers.Add(customer);
                    }
                }

                return customers;
            }
        }

        public bool Create(out Guid newId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.customers (name, address, phone, email, notes, eventId) " +
                "OUTPUT INSERTED.CUSTOMERID VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                Name, Address, Phone, Email, Notes, EventId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                SQLResponse = command.ExecuteScalar().ToString();
                return Guid.TryParse(SQLResponse, out newId);
            }
        }

        public bool Update(out Guid newId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("UPDATE dbo.customers SET " +
                    "name='{0}', address='{1}', phone='{2}', email='{3}', notes='{4}', updated='{5}', eventId='{6}' OUTPUT INSERTED.CUSTOMERID WHERE customerId='{7}'",
                    Name, Address, Phone, Email, Notes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), EventId, CustomerId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                SQLResponse = command.ExecuteScalar().ToString();
                return Guid.TryParse(SQLResponse, out newId);
            }
        }
    }
}