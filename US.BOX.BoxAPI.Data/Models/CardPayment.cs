using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class CardPayment
    {
        /// <summary>
        /// payment amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// singature that issued with the case
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// UI language of payment window ISO...
        /// ex: en
        /// </summary>
        public string Language { get; set; }

        public Contact DebtorContact { get; set; }
    }
}
