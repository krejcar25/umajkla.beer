using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace beer.umajkla.ShopModel
{
    public partial class Location : IShopObject
    {
        public Guid LocationId { get; set; }
        public string Name { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string CreatedBy { get; set; }
        public string SQLResponse { get; set; }

        #region All Countries List Generator
        public static Dictionary<string, string> GetAllCountries()
        {
            Dictionary<string, string> objDicUnsorted = new Dictionary<string, string>();

            foreach (CultureInfo ObjCultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo objRegionInfo = new RegionInfo(ObjCultureInfo.Name);
                Console.WriteLine("Adding " + objRegionInfo.Name);
                if (!objDicUnsorted.ContainsKey(objRegionInfo.TwoLetterISORegionName))
                {
                    if (objRegionInfo.NativeName != objRegionInfo.EnglishName)
                    {
                        objDicUnsorted.Add(objRegionInfo.TwoLetterISORegionName, objRegionInfo.NativeName + " (" + objRegionInfo.EnglishName + ")");
                    }
                    else
                    {
                        objDicUnsorted.Add(objRegionInfo.TwoLetterISORegionName, objRegionInfo.NativeName);
                    }
                }
            }

            Dictionary<string, string> objDic = new Dictionary<string, string>();
            string[] keys = objDicUnsorted.Keys.ToArray();
            Array.Sort(keys);
            foreach (var item in keys)
            {
                objDic.Add(item, objDicUnsorted[item]);
            }

            return objDic;
        }
        #endregion
    }
}