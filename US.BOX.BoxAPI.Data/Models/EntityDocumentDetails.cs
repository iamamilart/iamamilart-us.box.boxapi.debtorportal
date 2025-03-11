using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class EntityDocumentDetails
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentCategory { get; set; }
        public string StorageType { get; set; }
        public DateTime UploadedDate { get; set; }
        public string UploadedBy { get; set; }
        public string DocumentType { get; set; }
        public string EntityType { get; set; }
    }
}
