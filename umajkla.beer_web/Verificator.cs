using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace beer.umajkla.web
{
    public class Verificator
    {
        public struct VerifiedData
        {
            public string userId { get; set; }
            public string checksum { get; set; }
            public string encryptedData { get; set; }
        }

        public static string Encrypt(string controller, string method, string userId, object result, string publicKey)
        {
            string serialised = "";
            using (MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, result);
                serialised = Convert.ToBase64String(stream.ToArray());
            }
            return serialised;
        }
        public static string Encrypt(string json, string userId, Guid keyId)
        {
            DateTime verifyDate = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour);
            dynamic data = System.Web.Helpers.Json.Decode(json);
            dynamic newData = System.Web.Helpers.Json.Decode("");
            newData.data = data;
            string keyPhrase;

            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT keyPhrase FROM dbo.APIkeys WHERE userId = '{0}' and keyId = '{1}'", userId, keyId);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                keyPhrase = command.ExecuteScalar().ToString();
            }

            string raw = string.Format("I hereby send the json {0} to user {1}'s app {2} authenticated by code {3} as of the date {4}", json, userId, keyId, keyPhrase, verifyDate);
            byte[] rawEnc = new UTF8Encoding().GetBytes(raw);
            byte[] hashEnc = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(rawEnc);
            string hash = BitConverter.ToString(hashEnc).Replace("-", string.Empty).ToLower();

            newData.hash = hash;

            return System.Web.Helpers.Json.Encode(newData);
        }
        /// <summary>
        /// Verifies data validity before proceeding with SQL queries.
        /// 
        /// </summary>
        /// <param name="json">The JSON string from client</param>
        /// <returns>0 = OK
        /// 1 = Expired
        /// 2 = Invalid
        /// 3 = Key not found</returns>
        private static int Decrypt(string json, string userid, string date, string clientHash)
        {
            DateTime verifyDate = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour);
            dynamic data = System.Web.Helpers.Json.Decode(json);

            if (date != verifyDate.ToString("yyyy-MM-dd HH:mm:ss"))
            {
                return 1;
            }

            string dbKeyPhrase;

            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string cmdString = string.Format("SELECT keyPhrase FROM dbo.APIkeys WHERE userId = '{0}'", userid);
                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                if (command.ExecuteNonQuery() == 0) return 3;
                dbKeyPhrase = command.ExecuteScalar().ToString();
            }

            string raw = string.Format("I hereby send the json {0} as user {1} authenticated by code {2} as of the date {3}", json, userid, dbKeyPhrase, verifyDate);
            byte[] rawEnc = new UTF8Encoding().GetBytes(raw);
            byte[] hashEnc = ((HashAlgorithm)CryptoConfig.CreateFromName("SHA512")).ComputeHash(rawEnc);
            string hash = BitConverter.ToString(hashEnc).Replace("-", string.Empty).ToLower();

            if (hash != clientHash) return 2;

            return 0;
        }

        public static int IsAllowed(string details, string controller, string task, string id)
        {
            dynamic data = System.Web.Helpers.Json.Decode(details);
            int requestValid = 0;
            try
            {
                dynamic testdata = data.data;
                //requestValid = Decrypt(details);
            }
            catch (RuntimeBinderException)
            {
                data.data.controller = controller;
                data.data.task = task;
                data.data.id = id;
                //requestValid = Decrypt(System.Web.Helpers.Json.Encode(data));
            }

            if (requestValid == 0)
            {
                EventsPermissions perms = new EventsPermissions(data.security.userId, Guid.Parse(data.security.eventId));
                if (perms.VerifyPermission(controller, task, id)) return 0;
            }
            return requestValid;
        }

        public class EventsPermissions
        {
            public EventPermissions Event { get; set; }
            public CustomersPermissions Customers { get; set; }
            public GoodsPermissions Goods { get; set; }
            public PaymentsPermissions Payments { get; set; }
            public SuppliesPermissions Supplies { get; set; }
            public TransactionsPermissions Transactions { get; set; }
            public string UserId { get; set; }
            public Guid EventId { get; set; }

            #region Definitions
            public class EventPermissions
            {
                public bool EditInfo { get; set; } = false;
                public bool WriteNews { get; set; } = false;
                public bool EditNewsOwn { get; set; } = false;
                public bool EditNewsAll { get; set; } = false;
                public bool DeleteNewsOwn { get; set; } = false;
                public bool DeleteNewsAll { get; set; } = false;

                public EventPermissions(int level)
                {
                    if (level >= 32)
                    {
                        DeleteNewsAll = true;
                        level -= 32;
                    }

                    if (level >= 16)
                    {
                        DeleteNewsOwn = true;
                        level -= 16;
                    }

                    if (level >= 8)
                    {
                        EditNewsAll = true;
                        level -= 8;
                    }

                    if (level >= 4)
                    {
                        EditNewsOwn = true;
                        level -= 4;
                    }

                    if (level >= 2)
                    {
                        WriteNews = true;
                        level -= 2;
                    }

                    if (level >= 1)
                    {
                        EditInfo = true;
                        level -= 1;
                    }
                }
            }

            public class CustomersPermissions
            {
                public bool List { get; set; } = false;
                public bool Create { get; set; } = false;
                public bool EditOwn { get; set; } = false;
                public bool EditAll { get; set; } = false;
                public bool DeleteOwn { get; set; } = false;
                public bool DeleteAll { get; set; } = false;

                public CustomersPermissions(int level)
                {
                    if (level >= 32)
                    {
                        DeleteAll = true;
                        level -= 32;
                    }

                    if (level >= 16)
                    {
                        DeleteOwn = true;
                        level -= 16;
                    }

                    if (level >= 8)
                    {
                        EditAll = true;
                        level -= 8;
                    }

                    if (level >= 4)
                    {
                        EditOwn = true;
                        level -= 4;
                    }

                    if (level >= 2)
                    {
                        Create = true;
                        level -= 2;
                    }

                    if (level >= 1)
                    {
                        List = true;
                        level -= 1;
                    }
                }
            }

            public class GoodsPermissions
            {
                public bool List { get; set; } = false;
                public bool Create { get; set; } = false;
                public bool EditOwn { get; set; } = false;
                public bool EditAll { get; set; } = false;
                public bool DeleteOwn { get; set; } = false;
                public bool DeleteAll { get; set; } = false;

                public GoodsPermissions(int level)
                {
                    if (level >= 32)
                    {
                        DeleteAll = true;
                        level -= 32;
                    }

                    if (level >= 16)
                    {
                        DeleteOwn = true;
                        level -= 16;
                    }

                    if (level >= 8)
                    {
                        EditAll = true;
                        level -= 8;
                    }

                    if (level >= 4)
                    {
                        EditOwn = true;
                        level -= 4;
                    }

                    if (level >= 2)
                    {
                        Create = true;
                        level -= 2;
                    }

                    if (level >= 1)
                    {
                        List = true;
                        level -= 1;
                    }
                }
            }

            public class PaymentsPermissions
            {
                public bool List { get; set; } = false;
                public bool Create { get; set; } = false;
                public bool EditOwn { get; set; } = false;
                public bool EditAll { get; set; } = false;
                public bool DeleteOwn { get; set; } = false;
                public bool DeleteAll { get; set; } = false;

                public PaymentsPermissions(int level)
                {
                    if (level >= 32)
                    {
                        DeleteAll = true;
                        level -= 32;
                    }

                    if (level >= 16)
                    {
                        DeleteOwn = true;
                        level -= 16;
                    }

                    if (level >= 8)
                    {
                        EditAll = true;
                        level -= 8;
                    }

                    if (level >= 4)
                    {
                        EditOwn = true;
                        level -= 4;
                    }

                    if (level >= 2)
                    {
                        Create = true;
                        level -= 2;
                    }

                    if (level >= 1)
                    {
                        List = true;
                        level -= 1;
                    }
                }
            }

            public class SuppliesPermissions
            {
                public bool List { get; set; } = false;
                public bool Create { get; set; } = false;
                public bool EditOwn { get; set; } = false;
                public bool EditAll { get; set; } = false;
                public bool DeleteOwn { get; set; } = false;
                public bool DeleteAll { get; set; } = false;

                public SuppliesPermissions(int level)
                {
                    if (level >= 32)
                    {
                        DeleteAll = true;
                        level -= 32;
                    }

                    if (level >= 16)
                    {
                        DeleteOwn = true;
                        level -= 16;
                    }

                    if (level >= 8)
                    {
                        EditAll = true;
                        level -= 8;
                    }

                    if (level >= 4)
                    {
                        EditOwn = true;
                        level -= 4;
                    }

                    if (level >= 2)
                    {
                        Create = true;
                        level -= 2;
                    }

                    if (level >= 1)
                    {
                        List = true;
                        level -= 1;
                    }
                }
            }

            public class TransactionsPermissions
            {
                public bool List { get; set; } = false;
                public bool Create { get; set; } = false;
                public bool EditOwn { get; set; } = false;
                public bool EditAll { get; set; } = false;
                public bool DeleteOwn { get; set; } = false;
                public bool DeleteAll { get; set; } = false;

                public TransactionsPermissions(int level)
                {
                    if (level >= 32)
                    {
                        DeleteAll = true;
                        level -= 32;
                    }

                    if (level >= 16)
                    {
                        DeleteOwn = true;
                        level -= 16;
                    }

                    if (level >= 8)
                    {
                        EditAll = true;
                        level -= 8;
                    }

                    if (level >= 4)
                    {
                        EditOwn = true;
                        level -= 4;
                    }

                    if (level >= 2)
                    {
                        Create = true;
                        level -= 2;
                    }

                    if (level >= 1)
                    {
                        List = true;
                        level -= 1;
                    }
                }
            }
            #endregion

            public EventsPermissions(string userId, Guid eventId)
            {
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    string cmdString = string.Format("SELECT * FROM dbo.eventPermissions WHERE userId='{0}' AND eventId='{1}'", userId, eventId);
                    connection.Open();
                    SqlCommand command = new SqlCommand(cmdString, connection);
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        Event = new EventPermissions(int.Parse(reader["eventlevel"].ToString()));
                        Customers = new CustomersPermissions(int.Parse(reader["customerslevel"].ToString()));
                        Goods = new GoodsPermissions(int.Parse(reader["goodslevel"].ToString()));
                        Payments = new PaymentsPermissions(int.Parse(reader["paymentslevel"].ToString()));
                        Supplies = new SuppliesPermissions(int.Parse(reader["supplieslevel"].ToString()));
                        Transactions = new TransactionsPermissions(int.Parse(reader["transactionslevel"].ToString()));
                    }

                    UserId = userId;
                    EventId = eventId;
                }
            }

            public bool VerifyPermission(string controller, string task, string id)
            {
                if (controller == "Events")
                {
                    if (task == "editInfo" && Event.EditInfo) { return true; } else return false;
                    if (task == "writeNews" && Event.WriteNews) { return true; } else return false;
                    if (task == "editNews" || task == "deleteNews")
                    {
                        bool isCreator;
                        using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                        {
                            string cmdString = string.Format("SELECT * FROM dbo.eventNews WHERE newsId='{0}'", id);
                            connection.Open();
                            SqlCommand command = new SqlCommand(cmdString, connection);
                            SqlDataReader reader = command.ExecuteReader();
                            reader.Read();
                            if (reader["createdBy"].ToString() == UserId) isCreator = true; else isCreator = false;
                        }

                        if (task == "editNews" && isCreator && Event.EditNewsOwn) return true; else return false;
                        if (task == "editNews" && !isCreator && Event.EditNewsAll) return true; else return false;
                        if (task == "deleteNews" && isCreator && Event.DeleteNewsOwn) return true; else return false;
                        if (task == "deleteNews" && !isCreator && Event.DeleteNewsAll) return true; else return false;
                    }
                }
                else if (controller == "Customers")
                {
                    if ((task == "list" || task == "show") && Customers.List) return true; else return false;
                    if (task == "create" && Customers.Create) { return true; } else return false;
                    if (task == "edit" || task == "delete")
                    {
                        bool isCreator;
                        using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                        {
                            string cmdString = string.Format("SELECT * FROM dbo.customers WHERE customerId='{0}'", id);
                            connection.Open();
                            SqlCommand command = new SqlCommand(cmdString, connection);
                            SqlDataReader reader = command.ExecuteReader();
                            reader.Read();
                            if (reader["createdBy"].ToString() == UserId) isCreator = true; else isCreator = false;
                        }

                        if (task == "edit" && isCreator && Customers.EditOwn) return true; else return false;
                        if (task == "edit" && !isCreator && Customers.EditAll) return true; else return false;
                        if (task == "delete" && isCreator && Customers.DeleteOwn) return true; else return false;
                        if (task == "delete" && !isCreator && Customers.DeleteAll) return true; else return false;
                    }
                }
                else if (controller == "Goods")
                {
                    if ((task == "list" || task == "show") && Goods.List) return true; else return false;
                    if (task == "create" && Goods.Create) { return true; } else return false;
                    if (task == "edit" || task == "delete")
                    {
                        bool isCreator;
                        using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                        {
                            string cmdString = string.Format("SELECT * FROM dbo.goods WHERE goodsId='{0}'", id);
                            connection.Open();
                            SqlCommand command = new SqlCommand(cmdString, connection);
                            SqlDataReader reader = command.ExecuteReader();
                            reader.Read();
                            if (reader["createdBy"].ToString() == UserId) isCreator = true; else isCreator = false;
                        }

                        if (task == "edit" && isCreator && Goods.EditOwn) return true; else return false;
                        if (task == "edit" && !isCreator && Goods.EditAll) return true; else return false;
                        if (task == "delete" && isCreator && Goods.DeleteOwn) return true; else return false;
                        if (task == "delete" && !isCreator && Goods.DeleteAll) return true; else return false;
                    }
                }
                else if (controller == "Payments")
                {
                    if ((task == "list" || task == "show") && Payments.List) return true; else return false;
                    if (task == "create" && Payments.Create) { return true; } else return false;
                    if (task == "edit" || task == "delete")
                    {
                        bool isCreator;
                        using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                        {
                            string cmdString = string.Format("SELECT * FROM dbo.payments WHERE paymentId='{0}'", id);
                            connection.Open();
                            SqlCommand command = new SqlCommand(cmdString, connection);
                            SqlDataReader reader = command.ExecuteReader();
                            reader.Read();
                            if (reader["createdBy"].ToString() == UserId) isCreator = true; else isCreator = false;
                        }

                        if (task == "edit" && isCreator && Payments.EditOwn) return true; else return false;
                        if (task == "edit" && !isCreator && Payments.EditAll) return true; else return false;
                        if (task == "delete" && isCreator && Payments.DeleteOwn) return true; else return false;
                        if (task == "delete" && !isCreator && Payments.DeleteAll) return true; else return false;
                    }
                }
                else if (controller == "Supplies")
                {
                    if ((task == "list" || task == "show") && Supplies.List) return true; else return false;
                    if (task == "create" && Supplies.Create) { return true; } else return false;
                    if (task == "edit" || task == "delete")
                    {
                        bool isCreator;
                        using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                        {
                            string cmdString = string.Format("SELECT * FROM dbo.supplies WHERE supplyId='{0}'", id);
                            connection.Open();
                            SqlCommand command = new SqlCommand(cmdString, connection);
                            SqlDataReader reader = command.ExecuteReader();
                            reader.Read();
                            if (reader["createdBy"].ToString() == UserId) isCreator = true; else isCreator = false;
                        }

                        if (task == "edit" && isCreator && Supplies.EditOwn) return true; else return false;
                        if (task == "edit" && !isCreator && Supplies.EditAll) return true; else return false;
                        if (task == "delete" && isCreator && Supplies.DeleteOwn) return true; else return false;
                        if (task == "delete" && !isCreator && Supplies.DeleteAll) return true; else return false;
                    }
                }
                else if (controller == "Transactions")
                {
                    if ((task == "list" || task == "show") && Transactions.List) return true; else return false;
                    if (task == "create" && Transactions.Create) { return true; } else return false;
                    if (task == "edit" || task == "delete")
                    {
                        bool isCreator;
                        using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                        {
                            string cmdString = string.Format("SELECT * FROM dbo.transactions WHERE transactionId='{0}'", id);
                            connection.Open();
                            SqlCommand command = new SqlCommand(cmdString, connection);
                            SqlDataReader reader = command.ExecuteReader();
                            reader.Read();
                            if (reader["createdBy"].ToString() == UserId) isCreator = true; else isCreator = false;
                        }

                        if (task == "edit" && isCreator && Transactions.EditOwn) return true; else return false;
                        if (task == "edit" && !isCreator && Transactions.EditAll) return true; else return false;
                        if (task == "delete" && isCreator && Transactions.DeleteOwn) return true; else return false;
                        if (task == "delete" && !isCreator && Transactions.DeleteAll) return true; else return false;
                    }
                }
                else return false;
            }
        }
    }
}