using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class InvoiceCopy
    {
        /// <summary>
        /// destination email address for invoice copy
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// invoice number to send
        /// </summary>
        public string InvoiceNumber { get; set; }
        /// <summary>
        /// base64 encoded email message in HTML format
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// email subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// source of the invoice 
        /// 1 = Default
        /// </summary>
        public int Integration { get; set; } = 1;

    }
}
