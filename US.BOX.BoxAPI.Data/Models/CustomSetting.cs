using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class CustomSetting
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public int FieldID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public int DataTypeID { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string DefaultValue { get; set; }

        public CustomSetting()
        {
            ID = 0;
            FieldID = 0;
            Name = string.Empty;
            DisplayName = string.Empty;
            DataTypeID = 0;
            Value = string.Empty;
            DefaultValue = string.Empty;
        }
    }
}
