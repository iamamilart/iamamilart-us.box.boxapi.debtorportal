using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Enums;

namespace US.BOX.BoxAPI.Data.Models
{
    public class DPCaseNote
    {
        /// <summary>
        /// AR, Case or SubCase Number
        /// </summary>
        public int ReferenceNumber { get; set; }
        /// <summary>
        /// `AR`, `Case` or `SubCase`
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Debtor portal note type
        /// </summary>
        public Language Lan { get; set; }
        /// <summary>
        /// Case Note Text
        /// </summary>
        public string NoteText { get; set; }
        /// <summary>
        /// Created User
        /// </summary>
       // [JsonIgnore]
        public string CreatedUser { get; set; }
        /// <summary>
        /// Activity Name
        /// </summary>
        //[JsonIgnore]
        public string ActivityName { get; set; }

    }
}

