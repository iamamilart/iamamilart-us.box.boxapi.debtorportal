using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class PartpaymentRequest
    {
        /// <summary>
        /// date of first installment, required filed
        /// </summary>

        public DateTime FirstInstallmentDate { get; set; }

        /// <summary>
        /// number of installment , required,
        /// </summary>
        public int InstallmentCount { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }
        /// <summary>
        /// Debtor Type
        /// </summary>
        public string DebtorType { get; set; }
        /// <summary>
        /// Maximum Possible fee
        /// </summary>
        public decimal MaxPossibleFee { get; set; }
        /// <summary>
        /// 0 for calculate partpayment and not save
        /// 1 for calculate and save
        /// </summary>
        public int Confirmed { get; set; } = 0;
        /// <summary>
        /// Case type
        /// </summary>
        public string CaseType { get; set; }


        //public override string ToString()
        //{
        //    var obj = new JavaScriptSerializer().Serialize(this);
        //    return obj;
        //}
    }
}
