using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class OpenIDConfiguration
    {
        public string Authorization_endpoint { get; set; }
        public string Token_endpoint { get; set; }
        public string Userinfo_endpoint { get; set; }
        public string Client_ID { get; set; }
        public string Redirect_URL { get; set; }
        public bool VippsEnable { get; set; }
        public bool NetsEnable { get; set; }
    }
}
