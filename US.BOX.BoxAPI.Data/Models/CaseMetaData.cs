using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class CaseMetaData
    {
        /// <summary>
        /// Unicornbox intrnal case number
        /// </summary>
        public int CaseNo { get; set; }
        /// <summary>
        /// Case tpye can be  Case or SubCase
        /// </summary>
        public string CaseType { get; set; }
    }
}
