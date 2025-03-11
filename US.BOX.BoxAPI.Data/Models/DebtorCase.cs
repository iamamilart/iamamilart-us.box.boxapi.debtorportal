using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class DebtorCase
    {
        public int CaseNumber { get; set; }

        public int ExtCaseNumber { get; set; }

        public string CreditorName { get; set; }

        public string creditorID { get; set; }

        public string DebtorName { get; set; }

        public decimal Balance { get; set; }

        public decimal MainAmount { get; set; }

        public string Status { get; set; }

        public string DebtorNumber { get; set; }

        public string Password { get; set; }
        public DateTime Date { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
