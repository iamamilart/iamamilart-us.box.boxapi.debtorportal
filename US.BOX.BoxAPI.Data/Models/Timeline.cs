using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class Timeline
    {
        public DateTime TimeLineDate { get; set; }
        public double Balance { get; set; }
        public string WorkflowState { get; set; }
    }
}
