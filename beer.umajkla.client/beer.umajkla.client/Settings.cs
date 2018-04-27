using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace beer.umajkla.client
{
    public class Settings
    {
        public static Settings Current { get; set; }

        public string UserName { get; set; }
        public Guid KeySetId { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public DateTime KeySetExpiry { get; set; }
        public string ServerAddress { get; set; }

        public Settings()
        {
            UserName = GetSettingValue("UserName");
            KeySetId = GetSettingValue<Guid>("KeySetId");
            PrivateKey = GetSettingValue("PrivateKey");
            PublicKey = GetSettingValue("PublicKey");
            KeySetExpiry = GetSettingValue<DateTime>("KeySetExpiry");
            ServerAddress = GetSettingValue("ServerAddress");
        }

        public void Save()
        {
            Application.Current.Properties.

        }

        private T GetSettingValue<T>(string settingName) where T : struct
        {
            if (Application.Current.Properties.ContainsKey(settingName))
            {
                return (T)Application.Current.Properties[settingName];
            }
            else return new T();
        }

        private string GetSettingValue(string settingName)
        {
            return (string)Application.Current.Properties[settingName];
        }

        private void SaveValue(string key, object value)
        {
            if (Application.Current.Properties.ContainsKey(key))
                Application.Current.Properties.Remove(key);
            Application.Current.Properties.Add(key, value);
        }

        private void SaveValue(object toSave)
        {
            foreach (PropertyInfo property in toSave.GetType().GetRuntimeProperties())
            {
                //SaveValue(property.Name,property.GetMethod)
                throw new NotImplementedException();
            }
        }
    }
}
