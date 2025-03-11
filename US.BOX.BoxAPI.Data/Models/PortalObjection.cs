using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class PortalObjection
    {
        /// <summary>
        /// comma separated list of invoice numbers
        /// </summary>
        public string InvoiceNumbers { get; set; }
        /// <summary>
        /// note for objection
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Mobile Number
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// Email Address
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Case Number
        /// </summary>
        public int CaseNumber { get; set; }
        /// <summary>
        /// Case Type
        /// </summary>
        public string CaseType { get; set; }

        public int ObjectionType { get; set; }

        //public override string ToString()
        //{
        //    var obj = new JavaScriptSerializer().Serialize(this);
        //    return obj;
        //}
    }
}
