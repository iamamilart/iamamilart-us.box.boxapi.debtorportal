using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class MiddlePartPaymentRequest
    {
        public string CaseId { get; set; }
        public decimal RemainingBalance { get; set; }
        public decimal InstallmentAmount { get; set; }
        public DateTime? PartPaymentDate { get; set; }
        public string PartPaymentType { get; set; }
        public int NumberOfInstallment { get; set; }
        public string InstallmentInterval { get; set; }
        public DateTime? FirstInstallmentDate { get; set; }
        public string CaseNote { get; set; }
        public decimal ForwardCharge { get; set; }
        public bool HasPaymentAgreement { get; set; }
        public bool IsCaseCostAdded { get; set; }
        public string CommunicationMethod { get; set; }
        public string Email { get; set; }
        public string SmsNumber { get; set; }
        public int SelectedPeriod { get; set; }
        public int IncrementTimePeriod { get; set; }
        public int TermAmountNoOfInstallments { get; set; }
        public int CommunicationProfile { get; set; }
        public int WriteToDB { get; set; }
    }
}
