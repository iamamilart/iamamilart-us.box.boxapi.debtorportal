using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class UploadDocument
    {
        public int DocumentId { get; set; }
        public bool IsUploaded { get; set; }
        public string DocumentPath { get; set; }
        public IFormFile Document { get; set; }
        public string NewFilePath { get; set; }
    }
}
