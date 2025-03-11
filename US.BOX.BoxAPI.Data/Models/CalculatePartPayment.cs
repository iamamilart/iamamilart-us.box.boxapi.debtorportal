using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class CalculatePartPayment
    {
        public decimal TotalAmount { get; set; }
        public DateTime? FirstInstallmentDate { get; set; }
        public DateTime? LastInstallmentDate { get; set; }
        public decimal CaseCost { get; set; }
        public decimal FirstInstallmentAmount { get; set; }
        public decimal LastInstallmentAmount { get; set; }
        public string CreatedUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal ForwardCharge { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsErrorOccured { get; set; }
        public List<PartPaymentSchema> SchemaList { get; set; }
        public bool IsDCFAdded { get; set; }

    }
}
