using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class ActivityRequest
    {
        public string ActivityCode { get; set; }
        public DateTime ExecutingDateTime { get; set; }
        public int CaseId { get; set; }
        public string EntityType { get; set; }
        public List<ActivityParameter> ParameterList { get; set; }
        public int uniqueId { get; set; }
        public string ActivityVersion { get; set; }
    }
}
