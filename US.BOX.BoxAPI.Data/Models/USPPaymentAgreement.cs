using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class USPPaymentAgreement
    {
        public DateTime PayDate { get; set; }


        public decimal MainAmount { get; set; }


        public decimal CollectionFee { get; set; }


        public decimal CostFee { get; set; }


        public decimal CourtFee { get; set; }


        public decimal OtherFee { get; set; }


        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal Balance { get; set; }

        public int ID { get; set; }

        public decimal InterestMainAmount { get; set; }

        public decimal InterestCollectionAmount { get; set; }

        public decimal InterestCostAmount { get; set; }

        public decimal InterestCourtAmount { get; set; }

        public decimal InterestOtherAmount { get; set; }

        public decimal MainAmountBalance { get; set; }

        public decimal InterestFeeBalance { get; set; }

        public decimal CollectionFeeBalance { get; set; }

        public decimal CostFeeBalance { get; set; }

        public decimal CourtFeeBalance { get; set; }

        public decimal OtherFeeBalance { get; set; }

        public decimal TotalBalance { get; set; }

        public int CaseNo { get; set; }

        public string AgreementType { get; set; }

        public decimal MinCaseBalance { get; set; }

        public string UserName { get; set; }

        public string ModifiedUser { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ToleranceDays { get; set; }

        public bool IsSendSMSNotification { get; set; }
        public DateTime InterestDate { get; set; }
        public decimal InterestMainBalance { get; set; }
        public decimal InterestCostBalance { get; set; }


    }
}
