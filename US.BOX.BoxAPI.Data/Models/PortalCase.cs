
using US.BOX.BoxAPI.Data.Models;

namespace US.BOX.BoxAPI.Data.Models
{
    public class PortalCase
    {
        /// <summary>
        /// Case Status
        /// </summary>
        public int Status { get; set; }
        public int ArNo { get; set; }
        /// <summary>
        /// Creditor Name
        /// </summary>
        public string CreditorName { get; set; }
        /// <summary>
        /// Creditor ID
        /// </summary>
        public string CreditorID { get; set; }
        /// <summary>
        /// Case number used in external communication(used in letters, SMSs, Portals etc.)
        /// </summary>
        public int ExternalCaseNumber { get; set; }
        /// <summary>
        /// UnicornBOX internal case number
        /// </summary>
        public int BOXCaseNumber { get; set; }
        /// <summary>
        /// Case Type
        /// </summary>
        public string CaseType { get; set; }
        /// <summary>
        /// Debtor Name
        /// </summary>
        public string DebtorName { get; set; }
        /// <summary>
        /// Debtor First Name
        /// </summary>
        public string DebtorFirstName { get; set; }
        /// <summary>
        /// Debtor Last Name
        /// </summary>
        public string DebtorLastName { get; set; }
        /// <summary>
        /// Debtor Number
        /// </summary>
        public string DebtorNo { get; set; }
        /// <summary>
        /// Debtor Social Security Number
        /// </summary>
        public string DebtorSSNO { get; set; }
        /// <summary>
        /// Debtor Type
        /// </summary>
        public string DebType { get; set; }
        /// <summary>
        /// KID
        /// </summary>
        public string CaseKID { get; set; }
        /// <summary>
        /// Client Account
        /// </summary>
        public string ClientAccount { get; set; }
        /// <summary>
        /// Main Amount
        /// </summary>
        public decimal MainAmount { get; set; }
        /// <summary>
        /// Interest
        /// </summary>
        public decimal Interest { get; set; }
        /// <summary>
        /// Cost
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// Paid Ammount
        /// </summary>
        public decimal Paid { get; set; }
        /// <summary>
        /// current Balance
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// balance after 2 weeks
        /// </summary>
        public decimal Balance2 { get; set; }
        /// <summary>
        /// balance after 4 weeks
        /// </summary>
        public decimal Balance4 { get; set; }
        /// <summary>
        /// balance after 6 weeks
        /// </summary>
        public decimal Balance6 { get; set; }
        /// <summary>
        /// Signature
        /// </summary>
        public string Signature { get; set; }
        /// <summary>
        /// Signature
        /// </summary>
        public string PartPaymentSignature { get; set; }
        /// <summary>
        /// Delayed Till Date
        /// </summary>
        public DateTime? DelayedTill { get; set; }
        /// <summary>
        /// Debtor will delay the case till this date
        /// </summary>
        public DateTime? DelayUpto { get; set; }
        /// <summary>
        /// Max number of Installment
        /// </summary>
        public int MaxInstallment { get; set; }
        /// <summary>
        /// Min number of Installment
        /// </summary>
        public int MinInstallment { get; set; }
        /// <summary>
        /// Maximum Possible fee
        /// </summary>
        public double MaxPossibleFee { get; set; }
        /// <summary>
        /// Min Installment Amount
        /// </summary>
        public double MinInstallmentAmount { get; set; }
        /// <summary>
        /// Invoice Infomation
        /// </summary>
        public List<PortalInvoice> InvoiceInfo { get; set; }
        /// <summary>
        /// Activity History
        /// </summary>
        public List<Activity> History { get; set; }
        /// <summary>
        /// Debtor Email
        /// </summary>
        public string DebEmail { get; set; }
        /// <summary>
        /// Debtor Mobile Number
        /// </summary>
        public string DebMobile { get; set; }
        /// <summary>
        /// If payment is part payment it should `true`
        /// </summary>
        public bool IsPartPayment { get; set; }
        /// <summary>
        /// 'true' if Part payment has brech 
        /// </summary>
        public bool IsPartPaymentBreach { get; set; }

        /// <summary>
        /// If case is allowed to delay it should `true`
        /// </summary>
        public bool IsAllowedToDelay { get; set; }
        /// <summary>
        /// If case is allowed to do partpayments it should `true`
        /// </summary>
        public bool IsPartPaymentPossible { get; set; }
        /// <summary>
        /// Case workflow status 
        /// </summary>
        public string WorkflowStatus { get; set; }
        /// <summary>
        /// Case workflow status 
        /// </summary>
        public string StateWorkflow { get; set; }
        /// <summary>
        /// Part payment list
        /// </summary>
        public List<PartPayment> PartPayments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DueBalance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal NextDueAmount { get; set; }
        /// <summary>
        /// debtor's contact details
        /// </summary>
        public Contact DebtorContact { get; set; }
        public DateTime? DebtorBirthDate { get; set; }

        /// <summary>
        /// Case Exceeded Amount
        /// </summary>
        public decimal ExceededAmount { get; set; }

        public bool HasOpenObjection { get; set; }
        public DateTime? ObjectionDate { get; set; }
        public DateTime? DelayDueDate { get; set; }

        public string InvoiceNumber { get; set; }

        public decimal DelayFee { get; set; }
        public decimal CaseCost { get; set; }


        // new columns added for creditor comment and objection reason
        public string DebtorObjectionNote { get; set; }
        public string LastCreditorPortalNote { get; set; }
        // new column for privacypolicy read
        public bool IsPrivacyPolicyRead { get; set; }

        //public override string ToString()
        //{
        //    var obj = new JavaScriptSerializer().Serialize(this);
        //    return obj;
        //}
    }
}

