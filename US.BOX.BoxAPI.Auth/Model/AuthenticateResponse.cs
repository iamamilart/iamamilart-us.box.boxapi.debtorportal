using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Auth.Model
{
    public class AuthenticateResponse
    {
        public  string access_token { get; set; }
        public int expires_in { get; set; }

        public string roles { get; set; }

        public string userName { get; set; }

    }
}
