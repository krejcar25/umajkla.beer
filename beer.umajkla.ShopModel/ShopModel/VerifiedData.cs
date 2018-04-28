using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace beer.umajkla.ShopModel
{
    public partial class VerifiedData<TData> where TData : IShopObject
    {
        public TData Data { get; set; }
        public string ClientId { get; set; }
        public string Hash { get; set; }
        public DateTime DateTime { get; set; }

        public void Sign(APIkey secret)
        {
            Hash = ComputeHash(secret, true);
        }

        private string ComputeHash(APIkey secret, bool signing = false)
        {
            string data = JsonConvert.SerializeObject(Data);
            if (signing) DateTime = DateTime.UtcNow;
            byte[] toHash = Encoding.UTF8.GetBytes(string.Format("{0}{1}{2}{0}{1}{0}{3}{1}{0}", secret, ClientId, data, DateTime));
            byte[] hash;
            using (SHA512 shaM = new SHA512Managed())
            {
                hash = shaM.ComputeHash(toHash);
            }
            return hash.ToString();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static explicit operator TData(VerifiedData<TData> data) => data.Data;
    }
    public enum ValidityCheckResult
    {
        [Description("OK")]
        OK,
        [Description("Expired")]
        Expired,
        [Description("KeyNotFound")]
        KeyNotFound,
        [Description("InvalidHash")]
        InvalidHash,
        [Description("RequestTooEarly")]
        RequestTooEarly
    };

    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }
    }
}