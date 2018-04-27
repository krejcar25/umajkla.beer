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
using Newtonsoft.Json;
using beer.umajkla.ShopModel;

namespace beer.umajkla.web
{
    public class Verificator
    {
        /* ===========================
           |                         |
           |       UNUSED CODE       |
           |  This code is not used  |
           |   and will be removed   |
           |     a future commit     |
           |                         |
           =========================== */

        /*
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

        public class EventPermissions
        {
            public Guid EventId { get; set; }
            public EventPermissions Previous { get; set; }
            public PermissionsBundle Permissions { get; set; }
            public string UserId { get; set; }

            #region Definitions
            public class PermissionsBundle
            {
                public EventManagementPermissions EventManagement { get; set; }
                public CustomersPermissions Customers { get; set; }
                public GoodsPermissions Goods { get; set; }
                public PaymentsPermissions Payments { get; set; }
                public SuppliesPermissions Supplies { get; set; }
                public TransactionsPermissions Transactions { get; set; }
            }

            public class EventManagementPermissions
            {
                public bool EditInfo { get; set; } = false;
                public bool WriteNews { get; set; } = false;
                public bool EditNewsOwn { get; set; } = false;
                public bool EditNewsAll { get; set; } = false;
                public bool DeleteNewsOwn { get; set; } = false;
                public bool DeleteNewsAll { get; set; } = false;
            }

            public class CustomersPermissions
            {
                public bool List { get; set; } = false;
                public bool Create { get; set; } = false;
                public bool EditOwn { get; set; } = false;
                public bool EditAll { get; set; } = false;
                public bool DeleteOwn { get; set; } = false;
                public bool DeleteAll { get; set; } = false;
            }

            public class GoodsPermissions
            {
                public bool List { get; set; } = false;
                public bool Create { get; set; } = false;
                public bool EditOwn { get; set; } = false;
                public bool EditAll { get; set; } = false;
                public bool DeleteOwn { get; set; } = false;
                public bool DeleteAll { get; set; } = false;
            }

            public class PaymentsPermissions
            {
                public bool List { get; set; } = false;
                public bool Create { get; set; } = false;
                public bool EditOwn { get; set; } = false;
                public bool EditAll { get; set; } = false;
                public bool DeleteOwn { get; set; } = false;
                public bool DeleteAll { get; set; } = false;
            }

            public class SuppliesPermissions
            {
                public bool List { get; set; } = false;
                public bool Create { get; set; } = false;
                public bool EditOwn { get; set; } = false;
                public bool EditAll { get; set; } = false;
                public bool DeleteOwn { get; set; } = false;
                public bool DeleteAll { get; set; } = false;
            }

            public class TransactionsPermissions
            {
                public bool List { get; set; } = false;
                public bool Create { get; set; } = false;
                public bool EditOwn { get; set; } = false;
                public bool EditAll { get; set; } = false;
                public bool DeleteOwn { get; set; } = false;
                public bool DeleteAll { get; set; } = false;
            }
            #endregion

            public EventPermissions(string userId, Guid eventId)
            {
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    string cmdString = string.Format("SELECT * FROM dbo.ValidPermissions WHERE userId='{0}' AND eventId='{1}'", userId, eventId);
                    connection.Open();
                    SqlCommand command = new SqlCommand(cmdString, connection);
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        Permissions = JsonConvert.DeserializeObject<PermissionsBundle>(reader["permissions"].ToString());

                        if (reader["previousVersion"] != null)
                        {
                            Previous = new EventPermissions(new Guid(reader["previousVersion"].ToString()));
                        }
                    }
                    
                    EventId = eventId;
                    UserId = userId;
                }
            }

            public EventPermissions(Guid permissionsId)
            {
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaulfConnection"].ConnectionString))
                {
                    string cmdString = string.Format("SELECT * FROM dbo.permissions WHERE permissionsId='{0}'", permissionsId);
                    SqlCommand command = new SqlCommand(cmdString, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        Permissions = JsonConvert.DeserializeObject<PermissionsBundle>(reader["permissions"].ToString());

                        if (reader["previousVersion"] != null)
                        {
                            Previous = new EventPermissions(new Guid(reader["previousVersion"].ToString()));
                        }

                        EventId = new Guid(reader["eventId"].ToString());
                    }
                }
            }
            

            public bool VerifyPermission(string controller, string task, string id)
            {
                if (controller == "Events")
                {
                    if (task == "editInfo" && Permissions.EventManagement.EditInfo) { return true; }
                    else
                    if (task == "writeNews" && Permissions.EventManagement.WriteNews) { return true; }
                    else
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

                        if (task == "editNews" && isCreator && Permissions.EventManagement.EditNewsOwn) return true;
                        else
                        if (task == "editNews" && !isCreator && Permissions.EventManagement.EditNewsAll) return true;
                        else
                        if (task == "deleteNews" && isCreator && Permissions.EventManagement.DeleteNewsOwn) return true;
                        else
                        if (task == "deleteNews" && !isCreator && Permissions.EventManagement.DeleteNewsAll) return true; else return false;
                    }
                    else return false;
                }
                else if (controller == "Customers")
                {
                    if ((task == "list" || task == "show") && Permissions.Customers.List) return true;
                    else
                    if (task == "create" && Permissions.Customers.Create) { return true; }
                    else
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

                        if (task == "edit" && isCreator && Permissions.Customers.EditOwn) return true;
                        else
                        if (task == "edit" && !isCreator && Permissions.Customers.EditAll) return true;
                        else
                        if (task == "delete" && isCreator && Permissions.Customers.DeleteOwn) return true;
                        else
                        if (task == "delete" && !isCreator && Permissions.Customers.DeleteAll) return true; else return false;
                    }
                    else return false;
                }
                else if (controller == "Goods")
                {
                    if ((task == "list" || task == "show") && Permissions.Goods.List) return true;
                    else
                    if (task == "create" && Permissions.Goods.Create) { return true; }
                    else
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

                        if (task == "edit" && isCreator && Permissions.Goods.EditOwn) return true;
                        else
                        if (task == "edit" && !isCreator && Permissions.Goods.EditAll) return true;
                        else
                        if (task == "delete" && isCreator && Permissions.Goods.DeleteOwn) return true;
                        else
                        if (task == "delete" && !isCreator && Permissions.Goods.DeleteAll) return true; else return false;
                    }
                    else return false;
                }
                else if (controller == "Payments")
                {
                    if ((task == "list" || task == "show") && Permissions.Payments.List) return true;
                    else
                    if (task == "create" && Permissions.Payments.Create) { return true; }
                    else
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

                        if (task == "edit" && isCreator && Permissions.Payments.EditOwn) return true;
                        else
                        if (task == "edit" && !isCreator && Permissions.Payments.EditAll) return true;
                        else
                        if (task == "delete" && isCreator && Permissions.Payments.DeleteOwn) return true;
                        else
                        if (task == "delete" && !isCreator && Permissions.Payments.DeleteAll) return true; else return false;
                    }
                    else return false;
                }
                else if (controller == "Supplies")
                {
                    if ((task == "list" || task == "show") && Permissions.Supplies.List) return true;
                    else
                    if (task == "create" && Permissions.Supplies.Create) { return true; }
                    else
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

                        if (task == "edit" && isCreator && Permissions.Supplies.EditOwn) return true;
                        else
                        if (task == "edit" && !isCreator && Permissions.Supplies.EditAll) return true;
                        else
                        if (task == "delete" && isCreator && Permissions.Supplies.DeleteOwn) return true;
                        else
                        if (task == "delete" && !isCreator && Permissions.Supplies.DeleteAll) return true; else return false;
                    }
                    else return false;
                }
                else if (controller == "Transactions")
                {
                    if ((task == "list" || task == "show") && Permissions.Transactions.List) return true;
                    else
                    if (task == "create" && Permissions.Transactions.Create) { return true; }
                    else
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

                        if (task == "edit" && isCreator && Permissions.Transactions.EditOwn) return true;
                        else
                        if (task == "edit" && !isCreator && Permissions.Transactions.EditAll) return true;
                        else
                        if (task == "delete" && isCreator && Permissions.Transactions.DeleteOwn) return true;
                        else
                        if (task == "delete" && !isCreator && Permissions.Transactions.DeleteAll) return true; else return false;
                    }
                    else return false;
                }
                else return false;
            }
        }*/
    }
}