using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class ActivityExecutionAPI
    {
        public string APIURL { get; set; }
        public string EncriptionKey { get; set; }
        public string CaseObjectionActivityCode { get; set; }
        public string SubCaseObjectionActivityCode { get; set; }
        public string InvoiceActivityCode { get; set; }
        public string GenerateGDPRActivityCode { get; set; }
        public string SubCasePostPoneActivityCode { get; set; }
        public string CasePostPoneActivityCode { get; set; }
        public string User { get; set; }
        public string UserCode { get; set; }
    }
}
