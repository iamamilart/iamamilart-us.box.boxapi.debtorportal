using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace US.BOX.BoxAPI.Data.Models
{
    [XmlRoot(ElementName = "ProcessResponse")]
    public class ProcessResponse
    {
        [XmlElement(ElementName = "AuthorizationId")]
        public string AuthorizationId { get; set; }
        [XmlElement(ElementName = "BatchNumber")]
        public string BatchNumber { get; set; }
        [XmlElement(ElementName = "ExecutionTime")]
        public string ExecutionTime { get; set; }
        [XmlElement(ElementName = "MerchantId")]
        public string MerchantId { get; set; }
        [XmlElement(ElementName = "Operation")]
        public string Operation { get; set; }
        [XmlElement(ElementName = "ResponseCode")]
        public string ResponseCode { get; set; }
        [XmlElement(ElementName = "TransactionId")]
        public string TransactionId { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }
        [XmlElement(ElementName = "DetailResponce")]
        public string DetailResponce { get; set; }
    }
}
