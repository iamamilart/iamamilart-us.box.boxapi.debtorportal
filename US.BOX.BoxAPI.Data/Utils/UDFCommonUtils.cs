using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Enums;

namespace US.BOX.BoxAPI.Data.Utils
{
    public class UDFCommonUtils
    {
        private class UDFDateFormatter : IFormatProvider
        {
            public object GetFormat(Type formatType)
            {
                return "{0:MM/dd/yyyy HH:mm:ss}";
            }
        }

        

        public static string Serialize(object typeObj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeObj.GetType());
            MemoryStream memoryStream = new MemoryStream();
            xmlSerializer.Serialize(memoryStream, typeObj);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            byte[] array = new byte[memoryStream.Length];
            int num = memoryStream.Read(array, 0, 20);
            while (num < memoryStream.Length)
            {
                array[num++] = Convert.ToByte(memoryStream.ReadByte());
            }

            Encoding uTF = Encoding.UTF8;
            return uTF.GetString(array);
        }

        

        public static object Deserialize(Type type, string strXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(type);
            return xmlSerializer.Deserialize(new StringReader(strXml));
        }

        public static string FormatDate(string date)
        {
            try
            {
                return DateTime.Parse(date, new UDFDateFormatter()).ToShortDateString();
            }
            catch
            {
                return date;
            }
        }

        public static void SerializeObjectToXML(object obj, string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            TextWriter textWriter = new StreamWriter(path);
            try
            {
                xmlSerializer.Serialize(textWriter, obj);
                textWriter.Close();
                textWriter.Dispose();
                xmlSerializer = null;
            }
            catch
            {
                throw;
            }
            finally
            {
                xmlSerializer = null;
                textWriter.Close();
                textWriter.Dispose();
            }
        }

        public static object DeSerializeXMLToObject(object obj, string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            TextReader textReader = new StreamReader(path);
            try
            {
                object result = xmlSerializer.Deserialize(textReader);
                textReader.Close();
                textReader.Dispose();
                xmlSerializer = null;
                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                textReader.Close();
                textReader.Dispose();
                xmlSerializer = null;
            }
        }

        public static void SerializeWithXMLWriter(object source, string file)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.OmitXmlDeclaration = true;
            xmlWriterSettings.Indent = true;
            try
            {
                using XmlWriter xmlWriter = XmlWriter.Create(file, xmlWriterSettings);
                XmlSerializer xmlSerializer = new XmlSerializer(source.GetType());
                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add("xmlns", "\"http://tempuri.org/USDFValidation.Xsd\"");
                xmlSerializer.Serialize(xmlWriter, source, xmlSerializerNamespaces);
            }
            catch
            {
                throw;
            }
        }

        

        private static string FormatBirthDate(string p)
        {
            try
            {
                DateTime dateTime = DateTime.Parse(p);
                return dateTime.Day + "/" + dateTime.Month + "/" + dateTime.Year;
            }
            catch
            {
                return p;
            }
        }

        private static string GetAddrees3(string[] addressItemList)
        {
            string text = string.Empty;
            if (addressItemList.Length > 3)
            {
                int num = 1;
                foreach (string text2 in addressItemList)
                {
                    if (num < 3)
                    {
                        num++;
                    }
                    else
                    {
                        text += text2;
                    }
                }
            }
            else if (addressItemList.Length == 3)
            {
                text = addressItemList[2];
            }

            return text;
        }

        private static string GetAddress2(string[] addressItemList)
        {
            string result = string.Empty;
            if (addressItemList.Length > 1)
            {
                result = addressItemList[1];
            }

            return result;
        }

        private static string GetAddress1(string[] addressItemList)
        {
            string result = string.Empty;
            if (addressItemList.Length > 0)
            {
                result = addressItemList[0];
            }

            return result;
        }

       
        private static EnumClaimsImportStatus GetUDFClaimStatus(string claimStatus)
        {
            EnumClaimsImportStatus result = EnumClaimsImportStatus.Pending;
            switch (claimStatus)
            {
                case "Pending":
                case "Fail":
                case "Success":
                    switch (claimStatus)
                    {
                        case "Pending":
                            result = EnumClaimsImportStatus.Pending;
                            break;
                        case "Fail":
                            result = EnumClaimsImportStatus.Fail;
                            break;
                        case "Success":
                            result = EnumClaimsImportStatus.Success;
                            break;
                    }

                    break;
                default:
                    result = EnumClaimsImportStatus.Pending;
                    break;
            }

            return result;
        }

        private static string GetUSDFClaimStatus(EnumClaimsImportStatus claimStatus)
        {
            string result = string.Empty;
            switch (claimStatus)
            {
                case EnumClaimsImportStatus.Pending:
                    result = "Pending";
                    break;
                case EnumClaimsImportStatus.Fail:
                    result = "Fail";
                    break;
                case EnumClaimsImportStatus.Success:
                    result = "Success";
                    break;
            }

            return result;
        }
    }
}
