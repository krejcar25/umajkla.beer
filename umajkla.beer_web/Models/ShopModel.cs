﻿using System;
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
        public class CustomerDetail
        {
            public Customer Customer { get; set; }
            public List<Transaction> Transactions { get; set; }
            public List<Payment> Payments { get; set; }

            public CustomerDetail(SqlDataReader reader)
            {
                Customer = new Customer(reader);
                Transactions = GetTransactionsForCustomer(Customer.Id);
                Payments = GetPaymentsForCustomer(Customer.Id);
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

            public Goods(SqlDataReader reader)
            {
                Id = int.Parse(reader["id"].ToString());
                Name = reader["name"].ToString();
                Price = int.Parse(reader["price"].ToString());
                Unit = reader["unit"].ToString();
                Notes = reader["notes"].ToString();
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
        #region Customers Methods
        public static List<CustomerDetail> GetCustomersList()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT * FROM dbo.customers");

                connection.Open();

                SqlCommand command = new SqlCommand(cmdString, connection);

                List<CustomerDetail> customers = new List<CustomerDetail>();

                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        CustomerDetail customer = new CustomerDetail(list);
                        for (int i = 0; i < customer.Transactions.Count; i++)
                        {
                            customer.Transactions[i].Customer = null;
                        }
                        for (int i = 0; i < customer.Payments.Count; i++)
                        {
                            customer.Payments[i].Customer = null;
                        }
                        customers.Add(customer);
                    }
                }

                return customers;
            }
        }

        public static CustomerDetail GetCustomerDetail(int id)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(string.Format("SELECT * FROM dbo.customers WHERE id='{0}'", id), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return new CustomerDetail(reader);
            }
        }

        public static CustomerDetail CreateCustomer (string details)
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

        public static CustomerDetail UpdateCustomer (string details)
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
        #region Goods Methods 
        public static List<Goods> GetGoodsList()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT * FROM dbo.goods");

                connection.Open();

                SqlCommand command = new SqlCommand(cmdString, connection);

                List<Goods> goodsList = new List<Goods>();

                using (SqlDataReader list = command.ExecuteReader())
                {
                    while (list.Read())
                    {
                        Goods goods = new Goods(list);
                        goodsList.Add(goods);
                    }
                }

                return goodsList;
            }
        }

        public static Goods GetGoodsDetail(int id)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(string.Format("SELECT * FROM dbo.goods WHERE id='{0}'", id), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return new Goods(reader);
            }
        }

        public static Goods CreateGoods(string details)
        {
            dynamic data = System.Web.Helpers.Json.Decode(details);
            int goodsId;
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("INSERT INTO dbo.goods (name, price, unit, notes) " +
                "OUTPUT INSERTED.ID VALUES ('{0}', '{1}', '{2}', '{3}')",
                data.Name, data.Price, data.Unit, data.Notes);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                goodsId = (int)command.ExecuteScalar();
            }
            return GetGoodsDetail(goodsId);
        }

        public static Goods UpdateGoods(string details)
        {
            dynamic data = System.Web.Helpers.Json.Decode(details);
            int goodsId;
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("UPDATE dbo.goods SET " +
                    "name='{0}', price='{1}', unit='{2}', notes='{3}', updated='{4}' OUTPUT INSERTED.ID WHERE id='{5}'",
                    data.Name, data.Price, data.Unit, data.Notes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data.Id);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                goodsId = (int)command.ExecuteScalar();
            }
            return GetGoodsDetail(goodsId);
        }
        #endregion
        // nutno prepsat update
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
        // nutno prepsat update
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
                string cmdString = string.Format("SELECT * FROM dbo.transactions WHERE customerId='{0}'", customerId);
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