using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class DataClaimOfPayment
    {
        public int CaseId { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime? PaidDate { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public string Kid { get; set; }
        public string CaseNote { get; set; }
        public int CommunicationType { get; set; }
        public string CardNumber { get; set; }
        public string PayerName { get; set; }
        public string PaymentMethod { get; set; }
        public string SmsNumber { get; set; }
        public string Email { get; set; }
        public bool IsPrincipal { get; set; }
        public List<Invoice> InvoiceList { get; set; }
        public List<FileList> FileList { get; set; }
    }
}
