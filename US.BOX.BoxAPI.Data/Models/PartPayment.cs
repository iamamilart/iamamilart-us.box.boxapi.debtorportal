using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class PartPayment
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }

        public decimal Interest { get; set; }

        public decimal CapitalAmount { get; set; }

        public decimal InstallmentFee { get; set; }

        public decimal InstallmentAmount { get; set; }

        public decimal Outstanding { get; set; }
        public bool Paid { get; set; }
       
    }
}
