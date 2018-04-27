using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace beer.umajkla.ShopModel
{
    public partial class VerifiedData<TData> : IRequestValidityProvider<TData> where TData : IShopObject
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
}