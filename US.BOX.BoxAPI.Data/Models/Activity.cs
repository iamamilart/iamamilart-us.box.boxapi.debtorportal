using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class Activity
    {
        /// <summary>
        /// Activity Code
        /// </summary>
        public int ActivityCode { get; set; }
        /// <summary>
        /// Execution Id
        /// </summary>
        public int ExecutionId { get; set; }
        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Sub Type
        /// </summary>
        public string SubType { get; set; }
        /// <summary>
        /// Note about activity
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Description about activity
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Message Id
        /// </summary>
        public int MessageId { get; set; }
        /// <summary>
        /// Exeuted Date
        /// </summary>
        public DateTime ExeutedDate { get; set; }

        /// <summary>
        /// external document id or link.
        /// if case note in ducumenttype, then put note text here
        /// </summary>
        /// <value>
        /// The document identifier.
        /// </value>
        public string DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the documenttype.
        /// - `1` for note text
        /// - `2` for external doc
        /// - `0` if no doc
        /// </summary>
        /// <value>
        /// The documenttype.
        /// </value>
        public int DocumentType { get; set; }

    }

}
