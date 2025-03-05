using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class Contact
    {
        /// <summary>
        /// Mobile Number
        /// </summary>
        public string Mobile { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// note text if any
        /// </summary>
        public string Note { get; set; }

        public string Address { get; set; }

        public string PostalCode { get; set; }

        public string TelHome { get; set; }

        public string TelWork { get; set; }

        public string Fax { get; set; }
        public string PostalPlace { get; set; }
        public string CaseType { get; set; }

      
    }

}
