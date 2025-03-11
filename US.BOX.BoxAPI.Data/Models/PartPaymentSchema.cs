using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class PartPaymentSchema
    {
        public int PartPaymentNo { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal PartPayment { get; set; }
        public decimal Interest { get; set; }
        public decimal InstallmentFee { get; set; }
        public decimal InstallmentAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public decimal InstallmentOutstanding { get; set; }
        public bool IsPaid { get; set; }
        public int InstallmentId { get; set; }
    }
}
