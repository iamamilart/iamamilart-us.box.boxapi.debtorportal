using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class UploadDocumentResponse
    {
        public int DocumentId { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
