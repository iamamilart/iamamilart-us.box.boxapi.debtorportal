using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class CardPaymentReceipt
    {
        public string CreditorName { get; set; }

        public string InvoiceDate { get; set; }

        public string DebtorName { get; set; }

        public string CaseNumber { get; set; }

        public Decimal PaidAmount { get; set; }

        public string PaymentRef { get; set; }

        public string PaymentDate { get; set; }

        public string PaymentCheck { get; set; }

        public List<ReciptPaymentDetail> ReciptPaymentDetail { get; set; }

        public Decimal TotalAmount { get; set; }
    }
}
