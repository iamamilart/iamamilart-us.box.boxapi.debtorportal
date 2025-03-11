using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class ObjectionProfile
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public List<ObjectionCategory> ObjectionCategoryList { get; set; }

        public ObjectionProfile()
        {
            ObjectionCategoryList = new List<ObjectionCategory>();
        }
    }
}
