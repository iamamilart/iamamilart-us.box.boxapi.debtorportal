using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class AppSettings
    {
        public ConnectionStrings connectionStrings { get; set; }
        public string AllowedHosts { get; set; } = string.Empty;
        public AuthSettings authSetting { get; set; }
        public VIPPSLookup VIPPSLookUps { get; set; }
        public ActivityExecutionAPI ActivityExecutionAPI { get; set; }
        public string SendEmailInCreateCaseNote { get; set; }
        public AzureBlobSettings AzureBlobStorage { get; set; }
        public string CPReciptTemplate { get; set; }
        public string ReciptFolderpath { get; set; }

    }
}
