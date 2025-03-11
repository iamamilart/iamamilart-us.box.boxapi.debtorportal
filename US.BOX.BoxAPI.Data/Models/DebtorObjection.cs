using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class DebtorObjection
    {
        public int CaseId { get; set; }
        public string CaseNote { get; set; }
        public int ObjectionReasonId { get; set; }
        public int CommunicationType { get; set; }
        public string SmsNumber { get; set; }
        public string Email { get; set; }
        public List<Invoice> Invoices { get; set; }
    }
}
