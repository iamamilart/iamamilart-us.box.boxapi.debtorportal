using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class TenentDetail
    {
        public int ID { get; set; }
        public string TenentServer { get; set; }
        public string TenentDatabase { get; set; }
        public string TenentCommonLogin { get; set; }
        public string TenentCommonPassword { get; set; }
        public bool IsOnPremiseDB { get; set; }
        public string BlobConnection { get; set; }
        public Guid ClientID { get; set; }
    }
}
