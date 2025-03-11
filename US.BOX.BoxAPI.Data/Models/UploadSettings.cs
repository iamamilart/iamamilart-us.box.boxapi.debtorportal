using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class UploadSettings
    {
        public int MaxFileUploadSize { get; set; }
        public int MaxFileUploadCount { get; set; }
        public string StorageType { get; set; }
        public string BlobContainerName { get; set; }
        public string AllowedFileTypes { get; set; }
        public string ErrorMessage { get; set; }
    }
}
