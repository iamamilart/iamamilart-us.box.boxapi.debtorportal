using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Enums;

namespace US.BOX.BoxAPI.Data.Models
{
    public class IntegrationSetting
    {
        public IntegrationType IntegrationCode { get; set; } = IntegrationType.Local;

        public Dictionary<string, string> IntegrationProperties { get; set; } = new Dictionary<string, string>();
    }
}
