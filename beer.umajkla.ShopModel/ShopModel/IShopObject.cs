using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beer.umajkla.ShopModel
{
    public partial interface IShopObject
    {
        string CreatedBy { get; set; }
        DateTime Created { get; set; }
        DateTime Updated { get; set; }
    }
}
