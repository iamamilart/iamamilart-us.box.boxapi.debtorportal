using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class ObjectionCategory
    {
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public List<ObjectionReason> ReasonList { get; set; }

        public ObjectionCategory()
        {
            ReasonList = new List<ObjectionReason>();
        }
    }
}
