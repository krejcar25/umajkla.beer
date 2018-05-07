using System;
using System.Collections.Generic;
using System.Text;

namespace beer.umajkla.ShopModel
{
    public class LoginData
    {
        public DateTime ClientValidThru { get; set; }
        public DateTime ReauthValidThru { get; set; }
        public APIkey ReauthToken { get; set; }
        public APIkey ReauthSecret { get; set; }
        public Guid ClientId { get; set; }
    }
}
