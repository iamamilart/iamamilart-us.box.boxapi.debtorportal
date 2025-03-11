using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class VippsUserInfo
    {


        public string sub { get; set; }

        public DateTime birthdate { get; set; }

        public string email { get; set; }

        public string nnin { get; set; }

        public string name { get; set; }

        public string phone_number { get; set; }

        public string given_name { get; set; }

        public string family_name { get; set; }

        public string iss { get; set; }

        public string aud { get; set; }

        public Int64 updated_at { get; set; }

        public string personNo { get; set; }

    }
}
