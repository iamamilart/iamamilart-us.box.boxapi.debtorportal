using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    [DataContract]
    public class PartPayment
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public decimal Interest { get; set; }

        [DataMember]
        public decimal CapitalAmount { get; set; }

        [DataMember]
        public decimal InstallmentFee { get; set; }

        [DataMember]
        public decimal InstallmentAmount { get; set; }

        [DataMember]
        public decimal Outstanding { get; set; }
        [DataMember]
        public bool Paid { get; set; }
       
    }
}
