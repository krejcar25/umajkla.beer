using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace umajkla.beer.Models
{
    public class ShopModel
    {
        #region Objects Definitions
        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Notes { get; set; }

            public Customer(int id)
            {
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    connection.Open();
                    string cmdString = string.Format("SELECT * FROM dbo.customers WHERE id='{0}'", id);
                    SqlCommand command = new SqlCommand(cmdString, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        Id = int.Parse(reader["id"].ToString());
                        Name = reader["name"].ToString();
                        Address = reader["address"].ToString();
                        Phone = reader["phone"].ToString();
                        Email = reader["email"].ToString();
                        Notes = reader["notes"].ToString();
                    }
                }
            }

            public Customer(SqlDataReader reader)
            {
                Id = int.Parse(reader["id"].ToString());
                Name = reader["name"].ToString();
                Address = reader["address"].ToString();
                Phone = reader["phone"].ToString();
                Email = reader["email"].ToString();
                Notes = reader["notes"].ToString();
            }
        }
        public class Payment
        {
            public int Id { get; set; }
            public int Amount { get; set; }
            public Customer Customer { get; set; }
            public DateTime Date { get; set; }
            public string Notes { get; set; }

            public Payment(SqlDataReader data)
            {
                Id = int.Parse(data["id"].ToString());
                Amount = int.Parse(data["amount"].ToString());
                Customer = new Customer(int.Parse(data["customerId"].ToString()));
                Date = DateTime.Parse(data["datetime"].ToString());
                Notes = data["notes"].ToString();
            }
        }
        public class Goods
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
            public string Unit { get; set; }
            public string Notes { get; set; }

            public Goods(int id)
            {
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    connection.Open();
                    string cmdString = string.Format("SELECT * FROM dbo.goods WHERE id='{0}'", id);
                    SqlCommand command = new SqlCommand(cmdString, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        Id = int.Parse(reader["id"].ToString());
                        Name = reader["name"].ToString();
                        Price = int.Parse(reader["price"].ToString());
                        Unit = reader["unit"].ToString();
                        Notes = reader["notes"].ToString();
                    }
                }
            }
        }
        public class Supply
        {
            public int Id { get; set; }
            public Goods Item { get; set; }
            public int Amount { get; set; }
            public DateTime Expiration { get; set; }
            public string Notes { get; set; }


        }
        public class Transaction
        {
            public int Id { get; set; }
            public Customer Customer { get; set; }
            public int Amount { get; set; }
            public Goods Item { get; set; }
            public int Multiplier { get; set; }
            public DateTime Date { get; set; }
            public string Notes { get; set; }

            public Transaction(SqlDataReader read)
            {
                Id = int.Parse(read["id"].ToString());
                Customer = new Customer(int.Parse(read["customerId"].ToString()));
                Item = new Goods(int.Parse(read["itemId"].ToString()));
                Amount = int.Parse(read["amount"].ToString());
                Multiplier = int.Parse(read["multiplier"].ToString());
                Date = DateTime.Parse(read["updated"].ToString());
                Notes = read["notes"].ToString();
            }
        }
        #endregion
        #region Payments Methods
        public static List<Payment> GetPaymentsList()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT * FROM dbo.payments");

                connection.Open();

                SqlCommand command = new SqlCommand(cmdString, connection);

                List<Payment> payments = new List<Payment>();

                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Payment payment = new Payment(list);
                        payments.Add(payment);
                    }
                }

                return payments;
            }
        }

        public static List<Payment> GetPaymentsForCustomer(int customerId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT * FROM dbo.payments WHERE customerId='{0}'", customerId);
                SqlCommand command = new SqlCommand(cmdString, connection);
                connection.Open();
                List<Payment> payments = new List<Payment>();
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while(list.Read())
                    {
                        Payment payment = new Payment(list);
                        payments.Add(payment);
                    }
                }
                return payments;
            }
        }

        public static Payment GetPaymentDetail(int id)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(string.Format("SELECT * FROM dbo.payments WHERE id='{0}'", id), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return new Payment(reader);
            }
        }

        public static Payment CreatePayment(string details)
        {
            dynamic data = System.Web.Helpers.Json.Decode(details);
            int paymentId;
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.payments (customerId, amount, notes) " +
                "OUTPUT INSERTED.ID VALUES ('{0}', '{1}', '{2}')",
                data.CustomerId, data.Amount, data.Notes);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                paymentId = (int)command.ExecuteScalar();
            }
            return GetPaymentDetail(paymentId);
        }

        public static Transaction UpdatePayment(string details)
        {
            dynamic data = System.Web.Helpers.Json.Decode(details);
            int paymentId;
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("UPDATE dbo.payments SET " +   
                    "customerId='{0}', amount='{1}', notes='{2}' OUTPUT INSERTED.ID WHERE id='{3}'",
                    data.CustomerId, data.Amount, data.Notes, data.Id);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                paymentId = (int)command.ExecuteScalar();
            }
            return GetTransactionDetail(paymentId);
        }
        #endregion
        #region Customers Methods
        public static List<Customer> GetCustomersList()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT * FROM dbo.customers");

                connection.Open();

                SqlCommand command = new SqlCommand(cmdString, connection);

                List<Customer> customers = new List<Customer>();

                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Customer customer = new Customer(list);
                        customers.Add(customer);
                    }
                }

                return customers;
            }
        }

        public static Customer GetCustomerDetail(int id)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(string.Format("SELECT * FROM dbo.customers WHERE id='{0}'", id), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return new Customer(reader);
            }
        }

        public static Customer CreateCustomer (string details)
        {
            dynamic data = System.Web.Helpers.Json.Decode(details);
            int customerId;
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.customers (name, address, phone, email, notes) " +
                "OUTPUT INSERTED.ID VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                data.Name, data.Address, data.Phone, data.Email, data.Notes);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                customerId = (int)command.ExecuteScalar();
            }
            return GetCustomerDetail(customerId);
        }

        public static Customer UpdateCustomer (string details)
        {
            dynamic data = System.Web.Helpers.Json.Decode(details);
            int customerId;
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("UPDATE dbo.customers SET " +
                    "name='{0}', address='{1}', phone='{2}', email='{3}', notes='{4}', updated='{5}' OUTPUT INSERTED.ID WHERE id='{6}'",
                    data.Name, data.Address, data.Phone, data.Email, data.Notes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data.Id);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                customerId = (int)command.ExecuteScalar();
            }
            return GetCustomerDetail(customerId);
        }
        #endregion
        #region Transaction Methods
        public static List<Transaction> GetTransactionsList()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT * FROM dbo.transactions");

                connection.Open();

                SqlCommand command = new SqlCommand(cmdString, connection);

                List<Transaction> transactions = new List<Transaction>();

                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Transaction transaction = new Transaction(list);
                        transactions.Add(transaction);
                    }
                }

                return transactions;
            }
        }

        public static List<Transaction> GetTransactionsForCustomer(int customerId)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT * FROM dbo.payments WHERE customerId='{0}'", customerId);
                SqlCommand command = new SqlCommand(cmdString, connection);
                connection.Open();
                List<Transaction> transactions = new List<Transaction>();
                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Transaction transaction = new Transaction(list);
                        transactions.Add(transaction);
                    }
                }
                return transactions;
            }
        }

        public static Transaction GetTransactionDetail(int id)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(string.Format("SELECT * FROM dbo.transactions WHERE id='{0}'", id), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return new Transaction(reader);
            }
        }

        public static Transaction CreateTransaction(string details)
        {
            dynamic data = System.Web.Helpers.Json.Decode(details);
            int transactionId;
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.transactions (customerId, itemId, amount, multiplier, notes) " +
                "OUTPUT INSERTED.ID VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                data.CustomerId, data.ItemId, data.Amount, data.Multiplier, data.Notes);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                transactionId = (int)command.ExecuteScalar();
            }
            return GetTransactionDetail(transactionId);
        }

        public static Transaction UpdateTransaction(string details)
        {
            dynamic data = System.Web.Helpers.Json.Decode(details);
            int transactionId;
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("UPDATE dbo.transactions SET " +
                    "customerId='{0}', itemId='{1}', amount='{2}', multiplier='{3}', notes='{4}', updated='{5}' OUTPUT INSERTED.ID WHERE id='{6}'",
                    data.CustomerId, data.ItemId, data.Amount, data.Multiplier, data.Notes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data.Id);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                transactionId = (int)command.ExecuteScalar();
            }
            return GetTransactionDetail(transactionId);
        }
        #endregion
    }
}