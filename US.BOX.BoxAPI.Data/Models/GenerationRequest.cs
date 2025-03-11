using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class GenerationRequest
    {
        public int ActivityCode { get; set; }
        public string Group { get; set; }
        public int CategoryId { get; set; }
        public int DataItem { get; set; }
        public string DataSource { get; set; }
        [JsonIgnore]
        public string UserToken { get; set; }
        public List<string> Attachments { get; set; }
    }
}
