using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class SubCase
    {
        public int SubCaseNumber { get; set; }
        public string InvoiceRef { get; set; }
        public string ExCaseNo { get; set; }
        public decimal MainAmount { get; set; }
        public decimal Balance { get; set; }
        public string KID { get; set; }
    }
}
