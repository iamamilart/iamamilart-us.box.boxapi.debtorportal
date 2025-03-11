using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    /// <summary>
    /// claim of payment that comming form debtor portal
    /// </summary>
    public class ClaimOfPayment
    {
        /// <summary>
        /// payment date
        /// </summary>
        public DateTime Date { get; set; }

        //paid amount
        public decimal Amount { get; set; }

        /// <summary>
        /// name of payer
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// comma separated list of invoices
        /// </summary>
        public string Invoices { get; set; }


        /// <summary>
        /// payment method
        /// </summary>
        public string Method { get; set; }


        /// <summary>
        /// paid from account number
        /// </summary>
        public string FromAccount { get; set; }


        /// <summary>
        /// paid to account
        /// </summary>
        public string ToAccount { get; set; }

        /// <summary>
        /// Last 4 number of card
        /// </summary>
        public string Cardnumber { get; set; }

        /// <summary>
        /// payment kid that issued by the bank
        /// </summary>
        public string Kid { get; set; }

        /// <summary>
        /// note to creditor
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// customer telephone number
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// Customer email
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
        /// <summary>
        /// true if objection is for creditor
        /// </summary>
        public bool IsPrincipal { get; set; }
        /// <summary>
        /// Selected language
        /// </summary>
        public string SelectedLang { get; set; }
        /// <summary>
        /// UploadedReceipts
        /// </summary>
        public List<string> UploadedReceipts { get; set; }
    }
}
