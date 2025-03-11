using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class DPObjectionRespond
    {
        /// <summary>
        /// true if debtor agree to responce 
        /// </summary>
        public bool IsAgree { get; set; }

        /// <summary>
        /// Comment by debtor
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Case Number
        /// </summary>
        public int CaseNumber { get; set; }

        /// <summary>
        /// Cast type
        /// </summary>
        public string CaseType { get; set; }


    }
}
