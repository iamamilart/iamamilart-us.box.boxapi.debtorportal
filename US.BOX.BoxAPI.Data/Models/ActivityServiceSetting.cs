using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class ActivityServiceSetting
    {
        public string ActivityServiceBasePath { get; set; }
        public string BureaueCode { get; set; }
        public string ActivityType { get; set; }
        public string ActivityVersion { get; set; }
        public bool CloudStorageUsed { get; set; }
        public int TemplateCategoryId { get; set; }
        public string DataSource { get; set; }
        public int AttachmentActivityCode { get; set; }
        public int AttachmentActivityCategoryId { get; set; }
        public string ActivityCode { get; set; }

    }
}
