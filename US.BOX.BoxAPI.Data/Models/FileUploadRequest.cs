using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class FileUploadRequest
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public string NoteText { get; set; }
        public bool AddToHistory { get; set; }
        public string DocumentType { get; set; }
        public List<UploadDocument> UploadedDocuments { get; set; }
        public string? StorageType { get; set; } = "";
    }
}
