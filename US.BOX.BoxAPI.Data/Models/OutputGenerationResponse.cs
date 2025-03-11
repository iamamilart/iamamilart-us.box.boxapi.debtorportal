using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class OutputGenerationResponse
    {
        public int QueueId { get; set; }
        public int StatusCode { get; set; }
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string TemplateCategory { get; set; }
        public List<string> RowFiles { get; set; }
        public string Message { get; set; }
        public string ErrorDescription { get; set; }
        public List<string> Messages { get; set; }
        public Dictionary<string, object> DataItems { get; set; }
        public string MessageType { get; set; }
        public string FileName { get; set; }
        public Dictionary<string, string> Settings { get; set; }
        public string DocumentDetails { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string AttachmentPath { get; set; }
    }
}
