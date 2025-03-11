using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace US.BOX.BoxAPI.Data.Models
{
    public class AzureBlobSettings
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public string DirPath { get; set; }
    }
}
