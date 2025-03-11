using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class ActvityResponce
    {
        public string Result { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<string> Results { get; set; }
        public Dictionary<string, object> DataItems { get; set; }
        public int QueueId { get; set; }
        public string MessageType { get; set; }
        public string FileName { get; set; }
    }
}
