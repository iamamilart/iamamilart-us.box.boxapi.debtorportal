using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class PortalInvoice
    {
        /// <summary>
        /// Invoice Id
        /// </summary>
        public int InvoiceId { get; set; }
        /// <summary>
        /// Invoice Number
        /// </summary>
        public string InvoiceNumber { get; set; }
        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Invoice Date
        /// </summary>
        public DateTime? Date { get; set; }
        /// <summary>
        /// Invoice Text
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Document ID
        /// </summary>
        public string DocumentId { get; set; }
        public DateTime? DueDate { get; set; }
    }

}
