using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using US.BOX.BoxAPI.Data.Models;

namespace US.BOX.BoxAPI.Data.Utils
{
    public static class Data
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();

        public static KeyValuePair<string, string> GetUserInfo(ClaimsIdentity userIdentity)
        {
            var userIdenties = userIdentity.Claims.Where(a => a.Type == "UserName").ToList();
            var companyCodeIdenties = userIdentity.Claims.Where(a => a.Type == "CompanyCode").ToList();
            if (userIdenties.Count > 0 && companyCodeIdenties.Count > 0)
            {
                return new KeyValuePair<string, string>(userIdenties[0].Value, companyCodeIdenties[0].Value + "\\" + userIdenties[0].Value);
            }
            return new KeyValuePair<string, string>();
        }

        /// <summary>
        /// Responsible for returning user role
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static List<string> GetRoles(ClaimsIdentity identity)
        {
            return identity.Claims
                           .Where(c => c.Type == ClaimTypes.Role)
                           .Select(c => c.Value)
                           .ToList();
        }

        /// <summary>
        /// Responsible for fetching Get securiry identifire of the user
        /// For Creditor - Creditor ID
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetSecurityIdentifier(ClaimsIdentity identity)
        {
            return identity.Claims
                           .Where(c => c.Type == ClaimTypes.Sid)
                           .Select(c => c.Value)
                           .ToList()[0];
        }

        /// <summary>
        /// debttor portal send formated company code 
        /// companycode\username
        /// </summary>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string> GetDebtorPortalUserInfo(ClaimsIdentity userIdentity)
        {
            var userIdenties = userIdentity.Claims.Where(a => a.Type == "UserName").ToList();
            var companyCodeIdenties = userIdentity.Claims.Where(a => a.Type == "CompanyCode").ToList();
            if (userIdenties.Count > 0 && companyCodeIdenties.Count > 0)
            {
                return new KeyValuePair<string, string>(userIdenties[0].Value, companyCodeIdenties[0].Value);
            }
            return new KeyValuePair<string, string>();
        }

        //public static HeaderInfo GetHeaderInfo(int noOfCases, double invoicesum, string user, string PID)
        //{
        //    var usdfHeaderInfo = new HeaderInfo();
        //    usdfHeaderInfo.FileCreator = user;
        //    usdfHeaderInfo.MessageID = Guid.NewGuid().ToString();
        //    usdfHeaderInfo.FileType = "DP";
        //    usdfHeaderInfo.FileVersion = "1.0";
        //    usdfHeaderInfo.PID = PID;
        //    return usdfHeaderInfo;
        //}

        public static string GetUserFileHandler(ClaimsIdentity userIdentity)
        {
            var fileHandlerIdentities = userIdentity.Claims.Where(a => a.Type == "FileHandler").ToList();
            if (fileHandlerIdentities.Count > 0)
            {
                return fileHandlerIdentities[0].Value;
            }
            return string.Empty;
        }

        //SystemName

        public static string GetUserSystemName(ClaimsIdentity userIdentity)
        {
            var fileHandlerIdentities = userIdentity.Claims.Where(a => a.Type == "SystemName").ToList();
            if (fileHandlerIdentities.Count > 0)
            {
                return fileHandlerIdentities[0].Value;
            }
            return string.Empty;
        }

        /// <summary>
        /// call netaxept to make payment
        /// </summary>
        /// <param name="transRef">orderid or invoice number</param>
        /// <param name="netsurl">nets payment url</param>
        /// <param name="netstoken">token that issued by nets</param>
        /// <param name="netsmerchantid">merchantid in nets</param>
        /// <param name="opration">nets opration ,CAPTURE or AUTH</param>
        /// <returns>payment status object that returned by nets</returns>
        public static XmlDocument ProcessRequest(string transRef, string netsurl, string netstoken, string netsmerchantid, string opration)
        {
            var url = new UriBuilder(netsurl);
            url.Path = "Netaxept/Process.aspx";
            url.Query = $"merchantId={netsmerchantid}&token={netstoken}&transactionId={transRef}&operation={opration}";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url.ToString());
            httpWebRequest.Method = "GET";

            string result = "";
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            return doc;
        }

        /// <summary>
        /// genrate document url for FDC invoice
        /// </summary>
        /// <returns></returns>
        public static string GetDocumentURL(string host, string token, string secret, string documentID, int integration)
        {
           // logger.Info(documentID);
            if (integration == 1)
            {
                try
                {
                    var encodedDocId = WebUtility.UrlEncode(documentID);
                    string source = documentID + "!" + token + "!" + secret;
                    string signature = string.Empty;

                    StringContent _content = new StringContent(source);

                    if (_content != null)
                    {
                        var content = _content.ReadAsByteArrayAsync().Result;

                        MD5 md5 = new MD5CryptoServiceProvider();

                        signature = Base64UrlEncoder.Encode(md5.ComputeHash(content));

                    }

                    if (host.EndsWith("/"))
                    {
                        var url = host + encodedDocId + "/" + token + "/" + signature;
                        //logger.Info(url);
                        return url;
                    }
                    else
                    {
                        var url = host + "/" + encodedDocId + "/" + token + "/" + signature;
                        //logger.Info(url);
                        return url;
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }
            return "";
        }


        public static string GetBaseUrl(string absoluteUri, string routePrefix)
        {
            string baseUrl = absoluteUri.Split(new string[] { routePrefix }, StringSplitOptions.None).FirstOrDefault();  return baseUrl;
        }



    }
}
