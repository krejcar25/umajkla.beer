using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace beer.umajkla.ShopModel
{
    public partial interface IRequestValidityProvider<T> where T : IShopObject
    {
        ValidityCheckResult IsRequestValid();
        HttpResponseMessage GetResponseMessage(GetResponseMessageAction<T> action);
    }
}
