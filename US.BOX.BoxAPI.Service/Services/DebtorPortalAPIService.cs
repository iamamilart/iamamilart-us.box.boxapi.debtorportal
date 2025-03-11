using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using US.BOX.BoxAPI.Auth.Utils;
using US.BOX.BoxAPI.Core.Interfaces;
using US.BOX.BoxAPI.Data;
using US.BOX.BoxAPI.Data.Actions;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Service.Utils;
using System;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Utils;
using Microsoft.AspNetCore.Http;
using US.BOX.BoxAPI.Data.Enums;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using System.Xml;
using System.Xml.Serialization;
using Transaction = US.BOX.BoxAPI.Data.Models.Transaction;
using Microsoft.Extensions.Options;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Reflection.Metadata;
using System.Collections.Generic;
using System.Net.Http;
using System.Diagnostics;
using System.Data;
using Microsoft.FeatureManagement;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Security.Cryptography;


namespace US.BOX.BoxAPI.Core.Services
{
    public class DebtorPortalAPIService : IDebtorPortalAPIService
    {
        private readonly IDataManager dataManager;
        //private readonly AppSettings appSettings;
        private readonly IConverter converter;
        private readonly IHttpClientFactory httpClient;
        private readonly IFeatureManager featureManager;
        public DebtorPortalAPIService(IDataManager _dataManager, IConverter _converter, IHttpClientFactory _httpClient, IFeatureManager _featureManager)
        {
            dataManager = _dataManager;
            converter = _converter;
            httpClient = _httpClient;
            featureManager = _featureManager;
        }
        public async Task<IDictionary<string, string>> GetDebtorPortalSettings(int caseNumber = 0, CancellationToken cancellationToken = default)
        {
            return await dataManager.GetDebtorPortalSettings(caseNumber, cancellationToken);
        }

        public async Task<PortalCase> GetPortalCase(int caseNumber, CancellationToken cancellationToken = default)
        {
            var portalcase = await dataManager.GetPortalCase(caseNumber, cancellationToken);
            var settings = await GetDebtorPortalSettings(0, cancellationToken);
            if (portalcase.IsPartPayment)
            {
                var payments = await dataManager.GetPartPayment(portalcase.BOXCaseNumber, cancellationToken);
                var dueAmount = payments.Where(P => P.Date < DateTime.Now).Sum(X => X.CapitalAmount);
                portalcase.DueBalance = portalcase.Balance; //installment payment is not avaiable yet
                portalcase.PartPayments = payments;
                var nextDueInstallment = payments.FirstOrDefault(p => p.Paid == false);
                portalcase.NextDueAmount = (nextDueInstallment != null) ? nextDueInstallment.InstallmentAmount : 0;
            }
            else
            {
                portalcase.PartPayments = new List<PartPayment>();
                portalcase.DueBalance = portalcase.Balance;
                portalcase.IsPartPaymentBreach = false;
            }

            portalcase.DebEmail = string.IsNullOrEmpty(portalcase.DebEmail) ? "" : AuthCommon.ProtectData(portalcase.DebEmail);
            portalcase.DebMobile = string.IsNullOrEmpty(portalcase.DebMobile) ? "" : AuthCommon.ProtectData(portalcase.DebMobile);
            portalcase.Signature = Common.GetDebtorSignature(portalcase.DueBalance, settings["SignatureKey"]);
            portalcase.PartPaymentSignature = Common.GetDebtorSignature(portalcase.NextDueAmount, settings["SignatureKey"]);
            portalcase.MinInstallmentAmount = Convert.ToDouble(settings["MinInstallmentAmount"]);

            return portalcase;
        }

        public async Task<List<Timeline>> GetTimelinedata(int caseNumber, CancellationToken cancellationToken = default)
        {
            return await dataManager.GetTimelinedata(caseNumber, cancellationToken);
        }

        public async Task<string> GetCaseStatus(int caseNumber, CancellationToken cancellationToken = default)
        {
            return await dataManager.GetCaseStatus(caseNumber, cancellationToken);
        }

        public async Task<bool> UpdateContact(int casenumber, Contact contact, CancellationToken cancellationToken = default)
        {

            return await dataManager.UpdateContact(casenumber, contact);

        }

        public async Task<List<SystemCategory>> GetSystemCategories(string category, int arNo, CancellationToken cancellationToken = default)
        {
            return await dataManager.GetSystemCategories(category, arNo);
        }

        public async Task<bool> PrivacyPolicyUpdate(int caseNo, bool state, CancellationToken cancellationToken = default)
        {
            return await dataManager.PrivacyPolicyUpdate(caseNo, state);
        }

        public async Task<string> openIdConfigurations(int caseNo, string companyCode, CancellationToken cancellationToken = default)
        {
            var settings = await GetDebtorPortalSettings(0, cancellationToken);

            string redirectUri = settings["EIDREDIRECTURL"];
            string contentUrl = settings["OpneIDConfigurationsURL"];
            string username = settings["EIDClientID"];
            bool vippsEnable = Convert.ToBoolean(settings["VippsEnable"]);
            bool netsEnable = Convert.ToBoolean(settings["NetsEnable"]);

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, contentUrl);
                var res = client.SendAsync(request).Result;

                if (res.IsSuccessStatusCode)
                {
                    //var jsonResponse = res.Content.ReadAsStringAsync().Result.ToString();
                    OpenIDConfiguration openIDConfigurations = JsonConvert.DeserializeObject<OpenIDConfiguration>(res.Content.ReadAsStringAsync().Result);
                    openIDConfigurations.Client_ID = username;
                    openIDConfigurations.Redirect_URL = redirectUri;
                    openIDConfigurations.NetsEnable = netsEnable;
                    openIDConfigurations.VippsEnable = vippsEnable;
                    var jsonString = JsonConvert.SerializeObject(openIDConfigurations);
                    return jsonString;
                }
                else
                {
                    return "";
                }
            }

        }

        public async Task<string> bankIDProviders(int caseNo, string companyCode, CancellationToken cancellationToken = default)
        {
            var settings = await GetDebtorPortalSettings(0, cancellationToken);
            bool vippsEnable = Convert.ToBoolean(settings["VippsEnable"]);
            bool netsEnable = Convert.ToBoolean(settings["NetsEnable"]);
            OpenIDConfiguration openIDConfigurations = new OpenIDConfiguration();
            openIDConfigurations.NetsEnable = netsEnable;
            openIDConfigurations.VippsEnable = vippsEnable;
            var jsonString = JsonConvert.SerializeObject(openIDConfigurations);
            return jsonString;
        }

        public async Task<List<DebtorCase>> BankIDLogin(int caseNo, string authCode, CancellationToken cancellationToken = default)
        {
            List<DebtorCase> debtorCases = new List<DebtorCase>();

            //authCode = authCode.Replace(" ", "+");
            var settings = await GetDebtorPortalSettings(caseNo, cancellationToken);
            string url = settings["EIDURL"];
            string username = settings["EIDUser"];
            string password = settings["EIDPass"];
            string redirectUri = settings["EIDREDIRECTURL"];
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Authorization", "Basic " + encoded);


                Dictionary<string, string> reqDictionary = new Dictionary<string, string>
                    {
                        { "code",authCode },
                        { "grant_type","authorization_code" },
                        { "redirect_uri", redirectUri }
                    };
                request.Content = new FormUrlEncodedContent(reqDictionary);

                var res = client.SendAsync(request).Result;


                if (res.IsSuccessStatusCode)
                {
                    JObject tokenObject = JObject.Parse(res.Content.ReadAsStringAsync().Result);
                    JwtSecurityToken token = (JwtSecurityToken)ValidateBankToken(tokenObject["id_token"].ToString(), url, username);
                    string ssno = token.Payload["no_ssn"].ToString();
                    debtorCases = await dataManager.GetPortalCasesByDebtor(ssno, caseNo);

                }
                else
                {
                    //logger.Error(authCode);
                    //logger.Error(res.Content.ReadAsStringAsync().Result);
                    //return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "error on bankid"));
                }

            }
            return debtorCases;
        }
        //public async Task<string> GetDocumentPathByDocId(string documentId, CancellationToken cancellationToken = default)
        //{
        //    var settings = await GetDebtorPortalSettings(0, cancellationToken);
        //    var result = await GetDocumentPathByDocId(documentId);
        //    var key = "";// result.Key;
        //    var docPath = "";// result.Value;
        //    string fileContent = "";

        //    if (key == "EMAIL")
        //    {
        //         fileContent = File.ReadAllText(docPath);

        //    }
        //    else
        //    {
        //         fileContent = File.ReadAllText(docPath, Encoding.GetEncoding("iso-8859-1"));

        //    }
        //    return fileContent;
        //}

        //public async Task<DebtorCase> BankIDLogin((string authCode, int caseNumber, string openIDConfiguration, string companyCode, CancellationToken cancellationToken)
        //{
        //    List<DebtorCase> returnCases= new List<DebtorCase>();
        //    var settings = await GetDebtorPortalSettings(0, cancellationToken);
        //    authCode = authCode.Replace(" ", "+");
        //    string url = settings["EIDURL"];
        //    string username = settings["EIDUser"];
        //    string password = settings["EIDPass"];
        //    string redirectUri = settings["EIDREDIRECTURL"];

        //}
        //public async Task<bool> ObjectionRespond(int caseNo, PortalObjection objection, CancellationToken cancellationToken = default)
        //{
        //    bool isExcecutedSuccessfully = false;
        //    var settings = await GetDebtorPortalSettings(caseNo, cancellationToken);
        //    if (settings != null && settings.Count > 0)
        //    {
        //        List<string> invoiceNoList = new List<string>();
        //        var invoiceList = objection.InvoiceNumbers.Split(',').ToList();

        //        //CaseMetaData caseMetaData = new CaseMetaData();
        //        //caseMetaData.CaseType =await dataManager.GetCaseMetaData(caseNo, cancellationToken);

        //        var mobileEmailContact = new Contact()
        //        {
        //            Mobile = objection.Mobile,
        //            Email = objection.Email
        //        };
        //        await dataManager.UpdateContact(caseNo, mobileEmailContact, cancellationToken);

        //        ActivityRequest request = new ActivityRequest();
        //        request.ActivityCode = "4578";
        //        request.ActivityVersion = "V2";
        //        request.ExecutingDateTime = DateTime.Now;
        //        request.CaseId = caseNo;
        //        request.EntityType = objection.CaseType;
        //        ActivityParameter activityParameter1 = new ActivityParameter();
        //        activityParameter1.Name = "Caseno";
        //        activityParameter1.DataType = "int";
        //        activityParameter1.Value = caseNo.ToString();
        //        ActivityParameter activityParameter2 = new ActivityParameter();
        //        activityParameter2.Name = "Username";
        //        activityParameter2.DataType = "varchar";
        //        activityParameter2.Value = "DP-" + caseNo.ToString();
        //        request.ParameterList.Add(activityParameter1);
        //        request.ParameterList.Add(activityParameter2);

        //        if (objection.CaseType == "C")
        //        {

        //            isExcecutedSuccessfully = await ExecuteActivityAPI_V2(request);
        //        }
        //        else
        //        {
        //            isExcecutedSuccessfully = await ExecuteActivityAPI_V2(request);
        //        }

        //    }
        //    return isExcecutedSuccessfully;
        //}


        public async Task<bool> Objection(int caseNo, PortalObjection objection, CancellationToken cancellationToken = default)
        {
            bool isExcecutedSuccessfully = false;

            List<string> invoiceNoList = new List<string>();
            var invoiceList = objection.InvoiceNumbers.Split(',').ToList();

            var mobileEmailContact = new Contact()
            {
                Mobile = objection.Mobile,
                Email = objection.Email
            };
            await dataManager.UpdateContact(caseNo, mobileEmailContact, cancellationToken);

            //get objection profiles
            List<ObjectionProfile> objProfiles = new List<ObjectionProfile>();
            objProfiles = await dataManager.GetObjectionProfile(caseNo);
            //get subcase invoice details
            List<SubCase> subcases = new List<SubCase>();
            subcases = await dataManager.GetSubcasesByCaseNo(caseNo);

            List<Invoice> invList = subcases.Select(i => new Invoice
            {
                InvoiceNo = i.InvoiceRef,
                Kid = i.KID,
                Amount = i.MainAmount,
                Balance = i.Balance,
                PaidAmount = (i.MainAmount - i.Balance)
            }).ToList();

            //filter the selected invoice
            List<Invoice> filteredInvoices = invList.Where(inv => invoiceNoList.Contains(inv.InvoiceNo)).ToList();
            //map objects
            DebtorObjection requestObj = new DebtorObjection();
            requestObj.CaseId = caseNo;
            requestObj.SmsNumber = objection.Mobile;
            requestObj.Email = objection.Email;
            requestObj.Invoices = filteredInvoices;
            requestObj.CaseNote = objection.Note;
            //var reasonID = objProfiles.FirstOrDefault(obj => obj.ObjectionCategoryList.Any(o => o.CategoryId == objection.ObjectionType))
            //                              ?.ObjectionCategoryList.FirstOrDefault(o => o.CategoryId== objection.ObjectionType).ReasonList.FirstOrDefault(r=>r.ReasonId==1)?.ReasonId;
            //requestObj.ObjectionReasonId =Convert.ToInt32( reasonID);
            requestObj.ObjectionReasonId = objection.ObjectionType;
            //register debtor objection

            int ObjRegistered = await dataManager.RegisterDebtorObjection(requestObj);
            if (ObjRegistered > 0)
            {
                isExcecutedSuccessfully = true;
            }
            return isExcecutedSuccessfully;
        }
        public async Task<List<DebtorCase>> VippsBankIDLogin(string authCode, int caseNumber, string openIDConfiguration, string companyCode, CancellationToken cancellationToken = default)
        {
            List<DebtorCase> returnCases = new List<DebtorCase>();
            var settings = await GetDebtorPortalSettings(caseNumber, cancellationToken);
            
           authCode = authCode.Replace(" ", "+");

            OpenIDConfiguration OIDConfig = JsonConvert.DeserializeObject<OpenIDConfiguration>(openIDConfiguration);
            string authenticationURL = OIDConfig.Token_endpoint;
            string userInfoURL = OIDConfig.Userinfo_endpoint;
            string clientID = settings["EIDClientID"].ToString().Trim();
            string clinetSecret = settings["EIDClientSecret"].ToString().Trim();
            string redirectUri = settings["EIDRedirectURL"].ToString().Trim();

            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("UTF-8").GetBytes(clientID + ":" + clinetSecret));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, authenticationURL);
                request.Headers.Add("Authorization", "Basic " + encoded);

                Dictionary<string, string> reqDictionary = new Dictionary<string, string>
                    {
                        { "code",authCode },
                        { "grant_type","authorization_code" },
                        { "redirect_uri", redirectUri }
                    };
                request.Content = new FormUrlEncodedContent(reqDictionary);

                var res = new HttpResponseMessage();


                res = client.SendAsync(request).Result;
                // logger.Info("sucessfully sent authentication request for auth code- " + authCode);
                JObject tokenObject1 = JObject.Parse(res.Content.ReadAsStringAsync().Result);
                // logger.Info(tokenObject1);



                if (res.IsSuccessStatusCode)
                {
                    JObject tokenObject = JObject.Parse(res.Content.ReadAsStringAsync().Result);
                    // get user info
                    var requestUserInfo = new HttpRequestMessage(HttpMethod.Get, userInfoURL);
                    requestUserInfo.Headers.Add("Authorization", "Bearer " + tokenObject["access_token"].ToString());
                    var resUserInfo = new HttpResponseMessage();

                    resUserInfo = client.SendAsync(requestUserInfo).Result;
                     if (resUserInfo.IsSuccessStatusCode)
                    {
                        var stream = resUserInfo.Content.ReadAsStringAsync().Result;
                        var handler = new JwtSecurityTokenHandler();
                        var jsonToken = handler.ReadToken(stream);
                        var tokenS = jsonToken as JwtSecurityToken;
                        var payload = tokenS.Payload.SerializeToJson();
                        JObject payLoadJson = JObject.Parse(payload);
                        VippsUserInfo userInfo = new VippsUserInfo();
                        List<VippsUserInfo> responseList = new List<VippsUserInfo>();
                        userInfo = JsonConvert.DeserializeObject<VippsUserInfo>(payLoadJson.ToString());

                        string nnin = string.IsNullOrEmpty(userInfo.nnin) ? "" : userInfo.nnin.ToString().Trim();
                        string givenName = string.IsNullOrEmpty(userInfo.given_name) ? "" : userInfo.given_name.ToString().Trim();
                        string familyName = string.IsNullOrEmpty(userInfo.family_name) ? "" : userInfo.family_name.ToString().Trim();
                        string birthDate = string.IsNullOrEmpty(userInfo.birthdate.ToString()) ? "" : userInfo.birthdate.ToString().Trim();
                        string phoneNumber = string.IsNullOrEmpty(userInfo.phone_number) ? "" : userInfo.phone_number.ToString().Trim().Substring(2);
                        string email = string.IsNullOrEmpty(userInfo.email) ? "" : userInfo.email.ToString().Trim();

                        
                        if (await featureManager.IsEnabledAsync("VIPPSIFirstLookupEnabled") && await featureManager.IsEnabledAsync("VIPPSSecondLookupEnabled"))
                        {
                            if ((nnin != "" && (phoneNumber != "" && (birthDate != "" && birthDate != DateTime.MinValue.ToString()))))
                            {
                                var combinedResult = Lookups(phoneNumber, birthDate, nnin, companyCode);
                                if (combinedResult.Result.Count==0)
                                {
                                    throw new CustomInternalServerErrorException("1");
                                }
                                else if (combinedResult.Result.Count > 0)
                                {
                                    var portalCaselist = GetDistinctCasesByDebtors(combinedResult.Result, caseNumber, companyCode);
                                    if (portalCaselist.Result.Count==0)
                                    {
                                        throw new CustomInternalServerErrorException("1");
                                    }
                                    else if (portalCaselist.Result.Count > 0)
                                    {
                                        return portalCaselist.Result;
                                    }
                                    else
                                    {
                                        throw new CustomtException("3");
                                    }

                                }
                                else
                                {
                                    throw new CustomtException("2");

                                }
                            }
                            else
                            {
                                if (nnin != "")
                                {
                                    var debtorsBySSN = await dataManager.GetDebtorsBySSN(nnin);
                                    //var casesByDetails = SecondLookup(givenName, familyName, birthDate, nnin, caseNumber, companyCode);
                                    if (debtorsBySSN.Count==0)
                                    {
                                        throw new CustomInternalServerErrorException("1");
                                    }
                                    else if (debtorsBySSN.Count > 0)
                                    {
                                        var portalCaselist = GetDistinctCasesByDebtors(debtorsBySSN.ToList(), caseNumber, companyCode);
                                        if (portalCaselist.Result.Count==0)
                                        {
                                            throw new CustomInternalServerErrorException("1");
                                        }
                                        else if (portalCaselist.Result.Count > 0)
                                        {
                                            return portalCaselist.Result;
                                        }
                                        else
                                        {
                                            throw new CustomtException("3");
                                        }
                                    }
                                    else
                                    {
                                        throw new CustomtException("2");
                                    }
                                }
                                else if (phoneNumber != "" && (birthDate != "" && birthDate != DateTime.MinValue.ToString()))
                                {
                                    var debtorsByDebtorDetails = await dataManager.GetDebtorsByDebtorDetails(birthDate, phoneNumber);
                                    //var casesByDetails = SecondLookup(givenName, familyName, birthDate, nnin, caseNumber, companyCode);
                                    if (debtorsByDebtorDetails.Count==0)
                                    {
                                        throw new CustomInternalServerErrorException("1");
                                    }
                                    else if (debtorsByDebtorDetails.Count > 0)
                                    {
                                        var portalCaselist = GetDistinctCasesByDebtors(debtorsByDebtorDetails.ToList(), caseNumber, companyCode);
                                        if (portalCaselist.Result.Count==0)
                                        {
                                            throw new CustomInternalServerErrorException("1");
                                        }
                                        else if (portalCaselist.Result.Count > 0)
                                        {
                                            return portalCaselist.Result;
                                        }
                                        else
                                        {
                                            throw new CustomtException("");
                                        }
                                    }
                                    else
                                    {
                                        throw new CustomtException("2");
                                    }


                                }
                                else
                                {
                                    throw new CustomtException("4");
                                }
                            }
                        }
                        else
                        {
                            if (nnin != "")
                            {
                                if (await featureManager.IsEnabledAsync("VIPPSSecondLookupEnabled"))
                                {
                                    var debtorsBySSN = await dataManager.GetDebtorsBySSN(nnin);
                                    if (debtorsBySSN.Count==0)
                                    {
                                        throw new CustomInternalServerErrorException("1");
                                    }
                                    else if (debtorsBySSN.Count > 0)
                                    {
                                        var portalCaselist = GetDistinctCasesByDebtors(debtorsBySSN.ToList(), caseNumber, companyCode);
                                        if (portalCaselist.Result.Count==0)
                                        {
                                            throw new CustomInternalServerErrorException("1");
                                        }
                                        else if (portalCaselist.Result.Count > 0)
                                        {
                                            return portalCaselist.Result;
                                        }
                                        else
                                        {
                                            throw new CustomtException("3");
                                        }
                                    }
                                    else
                                    {
                                        throw new CustomInternalServerErrorException("3");
                                    }
                                }
                                else
                                {
                                    if (phoneNumber != "" && (birthDate != "" && birthDate != DateTime.MinValue.ToString()))
                                    {
                                        if (await featureManager.IsEnabledAsync("VIPPSSecondLookupEnabled"))
                                        {
                                            var debtorsByDebtorDetails = await dataManager.GetDebtorsByDebtorDetails(birthDate, phoneNumber);
                                            //var casesByDetails = SecondLookup(givenName, familyName, birthDate, nnin, caseNumber, companyCode);
                                            if (debtorsByDebtorDetails.Count==0)
                                            {
                                                //foreach (USBOXNotificationMessage message in debtorsByDebtorDetails.Notifications)
                                                //{

                                                //}
                                                throw new CustomInternalServerErrorException("1");
                                            }
                                            else if (debtorsByDebtorDetails.Count > 0)
                                            {
                                                var portalCaselist = GetDistinctCasesByDebtors(debtorsByDebtorDetails.ToList(), caseNumber, companyCode);
                                                if (portalCaselist.Result.Count==0)
                                                {
                                                    throw new CustomInternalServerErrorException("1");
                                                }
                                                else if (portalCaselist.Result.Count > 0)
                                                {
                                                    return portalCaselist.Result;
                                                }
                                                else
                                                {
                                                    throw new CustomInternalServerErrorException("3");
                                                }
                                            }
                                            else
                                            {
                                                throw new CustomInternalServerErrorException("2");
                                            }
                                        }
                                        else
                                        {
                                            throw new CustomInternalServerErrorException("5");
                                        }

                                    }
                                    else
                                    {
                                        throw new CustomInternalServerErrorException("4");
                                    }
                                }

                            }
                            else if (phoneNumber != "" && (birthDate != "" && birthDate != DateTime.MinValue.ToString()))
                            {
                                if (await featureManager.IsEnabledAsync("VIPPSSecondLookupEnabled"))
                                {
                                    var debtorsByDebtorDetails = await dataManager.GetDebtorsByDebtorDetails(birthDate, phoneNumber);
                                    //var casesByDetails = SecondLookup(givenName, familyName, birthDate, nnin, caseNumber, companyCode);
                                    if (debtorsByDebtorDetails.Count==0)
                                    {
                                        throw new CustomInternalServerErrorException("1");
                                    }
                                    else if (debtorsByDebtorDetails.Count > 0)
                                    {
                                        var portalCaselist = GetDistinctCasesByDebtors(debtorsByDebtorDetails.ToList(), caseNumber, companyCode);
                                        if (portalCaselist.Result.Count==0)
                                        {
                                            throw new CustomInternalServerErrorException("1");
                                        }
                                        else if (portalCaselist.Result.Count > 0)
                                        {
                                            return portalCaselist.Result;
                                        }
                                        else
                                        {
                                            throw new CustomInternalServerErrorException("3");
                                        }
                                    }
                                    else
                                    {
                                        throw new CustomInternalServerErrorException("2");
                                    }
                                }
                                else
                                {
                                    throw new CustomInternalServerErrorException("5");
                                }

                            }
                            else
                            {
                                throw new CustomInternalServerErrorException("4");
                            }

                        }

                    }
                    else
                    {
                        throw new CustomInternalServerErrorException("6");
                    }
                }
                else
                {
                    throw new CustomInternalServerErrorException("6");
                }
            }
        }


        public MiddlePartPaymentRequest MapPartPayment(MiddlePartPaymentRequest partPaymentRequest)
        {

            switch (partPaymentRequest.InstallmentInterval)
            {
                case nameof(InstallmentIntervals.Week):
                    partPaymentRequest.SelectedPeriod = 6;
                    partPaymentRequest.IncrementTimePeriod = 1;
                    break;
                case nameof(InstallmentIntervals.Biweek):
                    partPaymentRequest.SelectedPeriod = 6;
                    partPaymentRequest.IncrementTimePeriod = 2;
                    break;
                case nameof(InstallmentIntervals.Month):
                    partPaymentRequest.SelectedPeriod = 7;
                    partPaymentRequest.IncrementTimePeriod = 1;
                    break;
                case nameof(InstallmentIntervals.Quarter):
                    partPaymentRequest.SelectedPeriod = 7;
                    partPaymentRequest.IncrementTimePeriod = 3;
                    break;
                case nameof(InstallmentIntervals.Year):
                    partPaymentRequest.SelectedPeriod = 8;
                    partPaymentRequest.IncrementTimePeriod = 1;
                    break;
            }
            switch (partPaymentRequest.PartPaymentType)
            {
                case nameof(PartPaymentTypes.InstallmentAmount):
                    partPaymentRequest.TermAmountNoOfInstallments = partPaymentRequest.NumberOfInstallment;
                    partPaymentRequest.NumberOfInstallment = 0;
                    break;
                case nameof(PartPaymentTypes.NoOfInstallment):
                    partPaymentRequest.InstallmentAmount = 0;
                    break;
            }
            switch (partPaymentRequest.CommunicationMethod)
            {
                case nameof(CommunicationMethods.SMS):
                    partPaymentRequest.CommunicationProfile = 0;
                    break;
                case nameof(CommunicationMethods.Email):
                    partPaymentRequest.CommunicationProfile = 1;
                    break;
                case nameof(CommunicationMethods.Giro):
                    partPaymentRequest.CommunicationProfile = 2;
                    break;
                case nameof(CommunicationMethods.NonGiro):
                    partPaymentRequest.CommunicationProfile = 3;
                    break;
            }
            return partPaymentRequest;
        }
        public async Task<List<PartPayment>> CalculatePartPayment(int caseNo, PartpaymentRequest req, CancellationToken cancellationToken = default)
        {
            //CalculatePartPayment calculatePay = new CalculatePartPayment();
            List<PartPayment> partPaymentList = new List<PartPayment>();
            if (req.Confirmed == 0)
            {

                USPPaymentAgreement paymentAgreement = new USPPaymentAgreement();
                paymentAgreement.CaseNo = caseNo;
                paymentAgreement.UserName = "debtor portal - " + caseNo;
                paymentAgreement.PayDate = req.FirstInstallmentDate;
                paymentAgreement.CreatedDate = DateTime.Now;
                //paymentAgreement.TotalBalance=req.

                DataPartPaymentRequest dataPPReq = new DataPartPaymentRequest();
                dataPPReq.WriteToDB = req.Confirmed;
                dataPPReq.CaseId = caseNo.ToString();
                dataPPReq.SmsNumber = req.Mobile;
                dataPPReq.Email = req.Email;
                dataPPReq.FirstInstallmentDate = req.FirstInstallmentDate;
                dataPPReq.CaseNote = "Added thru dp api";
                dataPPReq.NumberOfInstallment = req.InstallmentCount;
                dataPPReq.PartPaymentDate = req.FirstInstallmentDate;
                dataPPReq.IncrementTimePeriod = 1;
                dataPPReq.SelectedPeriod = 7;
                if (req.DebtorType == "N")
                {
                    dataPPReq.IsCaseCostAdded = true;
                }
                else
                {
                    dataPPReq.ForwardCharge = req.MaxPossibleFee;
                }
                USPPaymentAgreement _paymentAgreement = new USPPaymentAgreement();


                int payAgreement = await dataManager.RegisterPaymentAgreement(_paymentAgreement, cancellationToken);
                CalculatePartPayment calculatePay = await dataManager.CalculatePartPayment(dataPPReq, cancellationToken);
                //var partPaymentItems = Payment.API.Case.CalculatePartPayment(partPaymentMetaData, userName, companyCode);
                if (payAgreement > 0)
                {
                    // logger.LogInformation("Payment agreement created for caseno -" + caseNo + "  Payment agreement ID - " + payAgreement);
                }
                else
                {
                    // logger.LogInformation("Payment agreement failed to create for caseno -" + caseNo + "  Payment agreement ID - " + payAgreement);
                }
                if (calculatePay != null && !calculatePay.IsErrorOccured)
                {
                    PartPayment partPayment = new PartPayment()
                    {
                        CapitalAmount = calculatePay.TotalAmount,
                        Date = Convert.ToDateTime(calculatePay.FirstInstallmentDate),
                        InstallmentFee = calculatePay.CaseCost,
                        InstallmentAmount = calculatePay.FirstInstallmentAmount,
                        Outstanding = calculatePay.ForwardCharge


                    };
                    partPaymentList.Add(partPayment);
                }
                //else
                //{
                //   // logger.LogInformation("PartPayment-CalculatePartPayment  Bussiness Validation Error: " + calculatePay.ErrorMessage);
                //    return new CustomtException("PartPayment-CalculatePartPayment  Bussiness Validation Error: " + calculatePay.ErrorMessage);
                //}
            }
            else
            {
                var payments = await dataManager.GetPartPayment(caseNo, cancellationToken);
                if (!payments.Any())
                {
                    partPaymentList.AddRange(payments);
                }
                //bool isExcecutedSuccessfully = false;

                //int entityType = -1;
                //CaseMetaData caseMetaData = new CaseMetaData();
                //caseMetaData = DataManager.GetCaseMetaData(casenumber, companyCode);

                //int arNo = -1;
                //int caseNo = -1;
                //int subCaseNo = -1;

                //switch (caseMetaData.CaseType)
                //{
                //    case "AR":
                //        arNo = casenumber;
                //        entityType = 0;
                //        break;
                //    case "C":
                //        caseNo = casenumber;
                //        entityType = 1;
                //        break;
                //    case "S":
                //        subCaseNo = casenumber;
                //        entityType = 2;
                //        break;
                //    default:
                //        throw new Exception("Invalid entity type");
                //}

                //if (!CcConfig)
                //{
                //    // Register push notification 
                //    DataManager.RegisterPushNotification(caseNo.ToString(), caseMetaData.CaseType == "C" ? "Case" : "SubCase"
                //        , "PartPayment"
                //        , companyCode);

                //    // Call activity
                //    if (req.Confirmed == 1)
                //    {
                //        if (caseMetaData.CaseType == "C")
                //        {
                //            isExcecutedSuccessfully = RoutineAndActivityAPI.ExecuteActivity(-1, "SendFirebaseCloudPushNotificationCase", caseNo, "Case", companyCode, -1, companyCode);
                //        }
                //        else
                //        {
                //            isExcecutedSuccessfully = RoutineAndActivityAPI.ExecuteActivity(-1, "SendFirebaseCloudPushNotificationSubCase", caseNo, "SubCase", companyCode, -1, companyCode);
                //        }
                //    }
                //}



            }
            return partPaymentList;
        }

        public async Task<List<PartPayment>> PartPayment(int caseNo, PartpaymentRequest partpaymentRequest, CancellationToken cancellationToken = default)
        {
            List<PartPayment> returnPP = new List<PartPayment>();
            //get settings
            var settings = await GetDebtorPortalSettings(0, cancellationToken);
            // mobile and email updating start
            var contact = new Contact()
            {
                Mobile = partpaymentRequest.Mobile,
                Email = partpaymentRequest.Email
            };
            var opResult = UpdateContact(caseNo, contact);

            // mobile and email updating end

            decimal MinInstallmentAmount = Convert.ToDecimal(settings["MinInstallmentAmount"]);
            int MaxNoOfInstallment = Convert.ToInt32(settings["MaxNoOfInstallment"]);
            if (MaxNoOfInstallment < partpaymentRequest.InstallmentCount)
            {
                throw new CustomtException("Incorrect installment count");

            }

            MiddlePartPaymentRequest mppRequest = new MiddlePartPaymentRequest();
            mppRequest.CaseId = caseNo.ToString();
            mppRequest.FirstInstallmentDate = partpaymentRequest.FirstInstallmentDate;
            mppRequest.NumberOfInstallment = partpaymentRequest.InstallmentCount;
            mppRequest.PartPaymentDate = partpaymentRequest.FirstInstallmentDate;
            mppRequest.IncrementTimePeriod = 1;
            mppRequest.SelectedPeriod = 7;
            //mppRequest.de = partpaymentRequest.DebtorType;
            mppRequest.SmsNumber = partpaymentRequest.Mobile;
            mppRequest.Email = partpaymentRequest.Email;
            mppRequest.InstallmentAmount = partpaymentRequest.MaxPossibleFee;
            mppRequest.WriteToDB = partpaymentRequest.Confirmed;
            mppRequest.CaseNote = "PP created thru DP";
            if (partpaymentRequest.DebtorType == "N")
            {
                mppRequest.IsCaseCostAdded = true;
            }
            else
            {
                mppRequest.ForwardCharge = partpaymentRequest.MaxPossibleFee;
            }
            MiddlePartPaymentRequest mpRequest = MapPartPayment(mppRequest);
            //var result = USBOXApiManage.CreatePartPayment(Convert.ToInt32(userInfo.Key), userInfo.Value, UserName, partpaymentRequest, MinInstallmentAmount, CcConfig);

            returnPP = await CalculatePartPayment(caseNo, partpaymentRequest, cancellationToken);


            //String UserName = "Debtor Portal";
            //bool CcConfig= true;

            //if (calculatePartPaymnent != null && !calculatePartPaymnent.IsErrorOccured)
            //{
            //    PartPayment partPayment = new PartPayment()
            //    {
            //        //Id = calculatePartPaymnent.,
            //        CapitalAmount = calculatePartPaymnent.TotalAmount,
            //        Date = calculatePartPaymnent.CreatedDate.Value,
            //       // InstallmentAmount = calculatePartPaymnent.,
            //        Interest = calculatePartPaymnent.FirstInstallmentAmount,
            //        InstallmentFee = calculatePartPaymnent.LastInstallmentAmount,
            //        Outstanding = calculatePartPaymnent.ForwardCharge,
            //        Paid = false,

            //    };
            //    returnPP.Add(partPayment);
            //}
            //else
            //{
            //    //log.LogInformation("PartPayment-CalculatePartPayment  Bussiness Validation Error: " + calculatePartPaymnent.ErrorMessage);
            //    throw new CustomtException("Bussiness Validation Error - "+ calculatePartPaymnent.ErrorMessage);
            //}



            if (returnPP.Count > 0)
            {
                if (partpaymentRequest.Confirmed == 1)
                {
                    DPCaseNote note = new DPCaseNote();
                    note.ReferenceNumber = Convert.ToInt32(caseNo);
                    note.CreatedUser = "DP - " + caseNo.ToString();

                    if (partpaymentRequest.CaseType == "C")
                    {
                        note.ActivityName = "DPPartPaymentCase";
                        note.Type = "Case";
                    }
                    else
                    {
                        note.ActivityName = "DPPartPaymentSubCase";
                        note.Type = "SubCase";
                    }
                    var noteResult = await dataManager.CreateDPortalCaseNote(note);

                }

            }

            return returnPP;
        }



        public async Task<string> RegisterPortalPayment(int caseNo, CardPayment paymentObj, CancellationToken cancellationToken = default)
        {
            var settings = await GetDebtorPortalSettings(0, cancellationToken);
            var contactUpdate = UpdateContact(caseNo, paymentObj.DebtorContact);
            string intAmount = ((int)(paymentObj.Amount * 100)).ToString();

            //check signature
            string generatedSignature = Common.GetDebtorSignature(paymentObj.Amount, settings["SignatureKey"]);

            string kid = await dataManager.GetNewPaymentKid(caseNo);
            if (paymentObj.Signature != generatedSignature)
            {
                throw new CustomtException("incorrect signature: " + generatedSignature);
            }
            var url = new UriBuilder(settings["NetsURL"]);
            url.Path = "Netaxept/Register.aspx";
            url.Query = $"merchantId={settings["MerchantID"]}" +
                $"&token={settings["MerchantToken"]}" +
                $"&orderNumber={kid}&amount={intAmount}&transactionReconRef={kid}" +
                $"&CurrencyCode=NOK&CardType={settings["CardType"]}" +
                $"&Language={paymentObj.Language}&redirectUrl={settings["RedirectUrl"]}";
            //url.Query = $"merchantId={settings.OperationReturnValue["MerchantID"]}&token={settings.OperationReturnValue["MerchantToken"]}&orderNumber={userInfo.Key}&amount={intAmount}&CurrencyCode=NOK&CardType={settings.OperationReturnValue["CardType"]}&Language={paymentObj.Language}&redirectUrl={settings.OperationReturnValue["RedirectUrl"]}";
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
            var iddoc = doc.GetElementsByTagName("TransactionId");


            if (iddoc.Count == 0)
            {

                throw new CustomtException("failed to register payment in nets:" + doc.GetElementsByTagName("Message")[0].InnerText);
            }
            string id = iddoc[0].InnerText;

            //register payment request in DB
            var updateResult = await dataManager.AddDPCardPayment(caseNo, kid, id, paymentObj.Amount, cancellationToken);

            //customer url
            var returnurl = new UriBuilder(settings["NetsURL"]);
            returnurl.Path = "Terminal/default.aspx";
            returnurl.Query = $"merchantId={settings["MerchantID"]}&transactionId={id}";



            return returnurl.ToString();

        }

        public async Task<bool> AuthorizePayment(int caseNumber, string transRef, CancellationToken cancellationToken = default)
        {
            //get settings
            var settings = await GetDebtorPortalSettings(caseNumber, cancellationToken);
            //register payment request in nets
            XmlDocument doc = US.BOX.BoxAPI.Data.Utils.Data.ProcessRequest(transRef, settings["NetsURL"], settings["MerchantToken"], settings["MerchantID"], "AUTH");
            var iddoc = doc.GetElementsByTagName("ResponseCode");

            if (iddoc.Count == 0 || iddoc[0].InnerText != "OK")
            {
                throw new CustomtException("failed to authorize payment in nets:");
                //logger.Error("failed to authorize payment in nets:" + doc.InnerXml);
                //return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "failed to authorize payment in nets:" + doc.InnerXml));
            }
            else
            {

                ProcessResponse response = null;
                XmlSerializer serializer = new XmlSerializer(typeof(ProcessResponse));
                using (XmlReader reader = new XmlNodeReader(doc))
                {
                    response = (ProcessResponse)serializer.Deserialize(reader);
                }

                //if Auth is success, call CAPTURE
                XmlDocument capDoc = US.BOX.BoxAPI.Data.Utils.Data.ProcessRequest(transRef, settings["NetsURL"], settings["MerchantToken"],
                    settings["MerchantID"], "CAPTURE");
                var resCode = capDoc.GetElementsByTagName("ResponseCode");
                if (resCode[0].InnerText != "OK")
                {
                    // logger.Error("failed to capture payment in nets: " + capDoc.InnerXml);

                    //update DB with AUTH response
                    await dataManager.AddDPCardPaymentDetails(0, response);
                    // return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "failed to capture payment in nets:" + capDoc.InnerXml));
                }

                var query = NetQuery(settings["MerchantID"], settings["MerchantToken"], transRef, settings["NetsURL"]);

                response.DetailResponce = query.InnerXml.ToString();

                var updateResult = await dataManager.AddDPCardPaymentDetails(1, response);

                string transID = response.TransactionId;
                Transaction importCPData = await dataManager.GetCardPaymentData(transID);

                List<Transaction> importCPDataList = new List<Transaction>();
                importCPDataList.Add(importCPData);

                // we are ignoring to import payment into box from DP now. this is specific for CC and fair already handle with OCR import and cardpayment
                // RegisterTransactions(importCPData, DomainCode);///if we want to register payment to box4 then find a way to import payment into box
                return updateResult;
            }
            
        }

        public async Task<List<string>> UploadClaimOfPaymentReceipts(int caseNo, List<IFormFile> files, CancellationToken cancellationToken = default)
        {
            List<string> filePathList = new List<string>();
            if (files == null || files.Count == 0)
            {

                throw new CustomtException("No file uploaded.");
            }
            else
            {

                var caseType = await dataManager.GetCaseMetaData(caseNo, cancellationToken);
                string caseDirectory = caseType == "s" ? "UploadClaimOfPaymentReceipts/SubCase/" + caseNo.ToString():
                                                          "UploadClaimOfPaymentReceipts/Case/" + caseNo.ToString();

                // var folderPath = Path.Combine(caseDirectory, caseNo.ToString(), "ClaimOfPaymentReceipts", DateTime.Now.ToString("yyyyMMddTHHmmss"));
                if (files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        var postedFile = file;
                        //string filePath = Path.Combine(folderPath, postedFile.FileName);
                        //if (await AzureBolbStorageCommon.FileExistsAsync(appSettings.AzureBlobStorage.ConnectionString, appSettings.AzureBlobStorage.ContainerName, folderPath))
                        //{
                        //    //await AzureBolbStorageCommon.DeleteFileAsync(postedFile.FileName, appSettings.AzureBlobStorage.ConnectionString, appSettings.AzureBlobStorage.ContainerName);
                        //}
                        string url = "";
                        url = await AzureBolbStorageCommon.UploadFileAsync(file, DPAPIConfig.AzureBlobStorageAccountName, DPAPIConfig.AzureBlobStorageContainerName, caseDirectory, postedFile.FileName,DPAPIConfig.ClientID);
                        if (url != "")
                        {
                            FileUploadRequest docuemntInfo = new FileUploadRequest();
                            docuemntInfo.EntityId = caseNo;
                            docuemntInfo.EntityType = caseType;
                            docuemntInfo.DocumentType = "ClaimOfPaymentReceipts";
                            docuemntInfo.AddToHistory = true;
                            docuemntInfo.NoteText = "Claim of payment receipt uploaded from debtor portal";
                            docuemntInfo.StorageType = "BlobStorage";
                            UploadDocument doc = new UploadDocument();
                            doc.DocumentPath = url;
                            doc.DocumentId = -1;
                            doc.IsUploaded = true;
                            List<UploadDocument> docs = new List<UploadDocument>();
                            docs.Add(doc);
                            docuemntInfo.UploadedDocuments = docs;

                            await dataManager.AddUploadedDocuments(docuemntInfo, "BlobStorage");
                        }
                        filePathList.Add(url);
                    }
                }




            }
            return filePathList;

        }

        public async Task<string> GetCPReciptPdf(string caseNo, string KID, string companyCode, CancellationToken cancellationToken = default)
        {
            string CPReciptTemplate = DPAPIConfig.CPReciptTemplate;

            // string body = File.ReadAllText(CPReciptTemplate);
            string body = await AzureBolbStorageCommon.DownloadFromBlobStorage("CPRecipt.html", CPReciptTemplate, DPAPIConfig.ClientID);
            var result = await dataManager.GetCPReciptData(caseNo, KID);

            body = body.Replace("[Creditor name]", result.CreditorName)
                    //.Replace("[Company ID]", result.OrgNo)
                    .Replace("[date and timestamp]", result.PaymentDate)
                    .Replace("[Debtor name]", result.DebtorName)
                    .Replace("[OrderID]", result.CaseNumber)
                    .Replace("[Voucher date]", result.InvoiceDate)
                    .Replace("[Main Amount]", result.PaidAmount.ToString("N2"))
                    .Replace("[Paid Amount]", result.PaidAmount.ToString("N2"))
                    .Replace("[Payment Ref]", result.PaymentRef)
                    .Replace("[orderlines]", GetOrderLinesHTML(result.ReciptPaymentDetail));

            string fileName = "document.html";
            string contentType = "text/html";
            string datetime = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            string reciptname = "CP_" + result.PaymentRef + ".html";
            string ReciptFolderpath = DPAPIConfig.ReciptFolderpath;
           // var folderPath = Path.Combine(  "ClaimOfPaymentReceipts", caseNo.ToString(), DateTime.Now.ToString("yyyyMMddTHHmmss"));
            var folderPath= "ClaimOfPaymentReceipts/"+ caseNo.ToString()+ "/" + DateTime.Now.ToString("yyyyMMddTHHmmss");
            IFormFile formFile = ConvertHtmlToIFormFile(body, reciptname, contentType);
            
            // byte[] filebytes = await PdfService.ConvertHtmlToPdfAsync(body);

            // string url = ReciptFolderpath + reciptname;
            //IFormFile file = ConvertByteArrayToIFormFile(filebytes, reciptname,"pdf");

            //string connectionString = DPAPIConfig.ConnectionString;
            //string containerName = DPAPIConfig.AzureBlobStorageContainerName+ folderPath;
            //string filePath = folderPath + "/" + reciptname;

            //string url = await AzureBolbStorageCommon.UploadFileAsync(formFile, appSettings.azureBlobSettings.ConnectionString, appSettings.azureBlobSettings.ContainerName);
            string url = await AzureBolbStorageCommon.UploadFileAsync(formFile, DPAPIConfig.AzureBlobStorageAccountName, DPAPIConfig.AzureBlobStorageContainerName, folderPath, reciptname, DPAPIConfig.ClientID);

            var caseType = await dataManager.GetCaseMetaData(Convert.ToInt32(caseNo), cancellationToken);

            if (url != "")
            {
                FileUploadRequest docuemntInfo = new FileUploadRequest();
                docuemntInfo.EntityId = Convert.ToInt32(caseNo);
                docuemntInfo.EntityType = caseType;
                docuemntInfo.DocumentType = "ClaimOfPaymentReceipts";
                docuemntInfo.AddToHistory = true;
                docuemntInfo.NoteText = "Payment receipt generated and uploaded from debtor portal";
                docuemntInfo.StorageType = "BlobStorage";
                UploadDocument doc = new UploadDocument();
                doc.DocumentPath = url;
                doc.DocumentId = -1;
                doc.IsUploaded = true;
                List<UploadDocument> docs = new List<UploadDocument>();
                docs.Add(doc);
                docuemntInfo.UploadedDocuments = docs;

                await dataManager.AddUploadedDocuments(docuemntInfo, "BlobStorage");
            }
                return url;
            //doc.Save(url);
        }

        public async Task<byte[]> ReadMessageDocument(int caseNo,string documentId, CancellationToken cancellationToken = default)
        {
            var caseType= await dataManager.GetCaseMetaData(caseNo);
            byte[] fileBytes = new byte[0];
            //var result = await dataManager.GetDocumentPathByDocId(documentId);
            List<string> docPathList = await dataManager.GetDocumentPathV4(Convert.ToInt32(documentId), "EntityDocument", caseType.ToString());
            //var key = result.Key;
            //var docPath = result.Value.ToString();
            if (docPathList.Count > 0)
            {
                string docPath = docPathList.FirstOrDefault();
                docPath = docPath.Replace("az:", "");
                // update local document info object
                DocumentInfo documentInfo = new DocumentInfo
                {
                    DocumentName = Path.GetFileName(docPath),
                    DocumentType = Path.GetExtension(docPath).Replace(".", ""),
                };

                if (docPath.StartsWith("email") || docPath.StartsWith("html") || docPath.StartsWith("ehf"))
                {
                    //var content = AzureDocumentHelper.GetDocumentContent(integrationSettings, documentPath);
                    // var content = AzureDocumentHelper.GetHtmlDocumentContent(integrationSettings, documentPath);
                    //Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    //var textBytes = encoding.GetBytes(content);
                    //documentInfo.Content = System.Convert.ToBase64String(textBytes);

                    //documentInfo.Content = Convert.ToBase64String(content);
                    // return documentInfo;


                }
                else
                {
                    string[] documentpathsegments = docPath.Split('/');
                    int segmentCount = documentpathsegments.Length;
                    if (segmentCount > 0)
                    {
                        string filePath = docPath;
                        string containerName = documentpathsegments[0];
                        if (containerName != "entitydocuments")
                        {
                            containerName = DPAPIConfig.AzureBlobStorageContainerName;
                            filePath = docPath.Replace(containerName, "");
                        }
                        else
                        {
                            filePath = docPath.Replace(documentpathsegments[0], "");
                        }
                        // get document string from blob storage 
                        // return DownloadFromBlobStorage(filePath, clientId, containerName);
                        fileBytes = await AzureBolbStorageCommon.DownloadFileAsync(filePath.ToString(), DPAPIConfig.AzureBlobStorageContainerName, DPAPIConfig.ClientID);
                    }
                    else
                    {
                        throw new Exception("Blobpath Docuemnt - documentPath is invalid - " + docPath);
                    }

                    // return documentInfo;
                }
            }
            //byte[] fileBytes = await AzureBolbStorageCommon.DownloadFileAsync(docPath.ToString(), appSettings.AzureBlobStorage.ConnectionString, appSettings.AzureBlobStorage.ContainerName);

            //if (key == "EMAIL")
            //{
            //    string fileContent = File.ReadAllText(docPath);
            //}
            //else
            //{
            //    string fileContent = File.ReadAllText(docPath, Encoding.GetEncoding("iso-8859-1"));



            //}
            return fileBytes;
            //return fileBytes;
            //return File(fileBytes, contentType, docPath);
        }

        public async Task<int> CreateCaseNote(int CaseNo, DPCaseNote casenote, CancellationToken cancellationToken = default)
        {
            if (casenote.Type == "Case")
            {
                casenote.ActivityName = "DPInquiryCase";
            }
            else
            {
                casenote.ActivityName = "DPInquirySubCase";
            }
            int casenoteResponse = await dataManager.CreateDPortalCaseNote(casenote);
            if (await featureManager.IsEnabledAsync("SendEmailInCreateCaseNote"))
            {
                bool emailSent = false;

                var settings = await dataManager.GetDebtorPortalSettings(CaseNo);

                string smtpUser = settings["SMTPUser"];
                string smtpPassword = settings["SMTPPassword"];
                string smtpServer = settings["SMTPServer"];
                string destEmails = settings["BureauEmails"];

                List<string> destEmailsList = destEmails.Split(',').ToList();
                emailSent = SendEMail.sendMail(smtpUser, destEmailsList, casenote.NoteText, smtpServer, "DP Inquiry - " + casenote.ReferenceNumber, smtpUser, smtpPassword);
            }
            return casenoteResponse;
        }

        public async Task<DateTime> PostponeCase(int CaseNo, Contact contact, CancellationToken cancellationToken = default)
        {
            var settings = await dataManager.GetDebtorPortalSettings(CaseNo);
            DateTime delayUntil = DateTime.Now.AddDays(Convert.ToInt32(settings["NoOfDaysToDelay"]));


            //call activity execution api and execute delay activity
            ActivityRequest request = new ActivityRequest();
            request.CaseId = CaseNo;
            request.ActivityVersion = "V2";
            request.ActivityCode = "10803";
            request.ExecutingDateTime = DateTime.Now;



            //var result = USBOXApiManage.PostponeCase(Convert.ToInt32(userInfo.Key), userInfo.Value, delayUntil, "Debtor Portal", addr.Mobile, addr.Email, addr.Note);
            DPCaseNote note = new DPCaseNote();
            note.ReferenceNumber = CaseNo;
            note.CreatedUser = CaseNo.ToString();

            if (contact.CaseType == "C")
            {
                note.ActivityName = "DPDelayCase";
                note.Type = "Case";
            }
            else
            {
                note.ActivityName = "DPDelayCase";
                note.Type = "SubCase";
            }
            var noteResult = await dataManager.CreateDPortalCaseNote(note);

            return delayUntil;
        }

        public async Task<Contact> GetPortalDebtor(int caseNo, CancellationToken cancellationToken = default)
        {
            return new Contact();
        }

        public async Task<bool> ClaimOfPayment(int caseNo, ClaimOfPayment claim, CancellationToken cancellationToken = default)
        {
            bool claimRegisteredResponse = false;
            var settings = await dataManager.GetDebtorPortalSettings(caseNo);
            //var userInfo = US.BOX.BoxAPI.Data.Utils.Data.GetDebtorPortalUserInfo((ClaimsIdentity)User.Identity);
            var contact = new Contact()
            {
                Mobile = claim.Mobile,
                Email = claim.Email
            };
            var opResult = await dataManager.UpdateContact(caseNo, contact, cancellationToken);

            Dictionary<string, string> trans = new Dictionary<string, string>()
                {
                    { "Claim of payment", "Notice of payment" },
                    { "Payment date", "Dato for betaling" },
                    { "Payer name", "Betalers navn" },
                    { "Amount paid", "Betalt beløp" },
                    { "Invoice", "Faktura"  },
                    { "Method of payment", "Betalingsmåte" },
                    { "Cash", "Bank eller nettbank" },
                    { "Card Payment", "Kortbetaling" },
                    { "From account", "Betalt fra" },
                    { "To account" , "Betalt til" },
                    { "Card number (Last 4 digits)", "Kortnummer (siste 4 siffer)" },
                    { "KID", "KID Number" },
                    { "Freezed", "Freezed" }
                };

            DPCaseNote noteClaimOfPayment = new DPCaseNote();
            var cardNumber = claim.Cardnumber != "" ? ("Card number (Last 4 digits): " + claim.Cardnumber) : "\n";
            var fromAccount = "From account: " + claim.FromAccount + "\n";
            var toAccount = "To account: " + claim.ToAccount + "\n";
            var freezed = DateTime.Today.Date.ToString("MM-dd-yyyy") + " to " + DateTime.Today.AddDays(3).Date.ToString("MM-dd-yyyy");
            noteClaimOfPayment.ReferenceNumber = caseNo;// Convert.ToInt32(userInfo.Key);
            noteClaimOfPayment.CreatedUser = caseNo.ToString();// userInfo.Value;
            noteClaimOfPayment.NoteText =
                //"**Payment date:**" + claim.Date.ToShortDateString() + "\n" +
                "Payer name: " + claim.Name + "\n" +
                "Method of payment: " + claim.Method + "\n" +
                //"\n **Amount paid:** kr" + claim.Amount + "\n" +
                fromAccount +
                toAccount +
                "Invoice: " + claim.Invoices + "\n" +
                cardNumber + "\n" +
                "KID: " + claim.Kid + "\n" +
                "Freezed : " + freezed;

            if (claim.SelectedLang == "nb")
            {
                foreach (var line in trans)
                {
                    noteClaimOfPayment.NoteText = noteClaimOfPayment.NoteText.Replace(line.Key, line.Value);
                }
            }

            claim.Note = noteClaimOfPayment.NoteText;

            //get subcase invoice details
            List<SubCase> subcases = new List<SubCase>();
            subcases = await dataManager.GetSubcasesByCaseNo(caseNo);

            List<Invoice> invList = subcases.Select(i => new Invoice
            {
                InvoiceNo = i.InvoiceRef,
                Kid = i.KID,
                Amount = i.MainAmount,
                Balance = i.Balance,
                PaidAmount = (i.MainAmount - i.Balance)
            }).ToList();

           // List<string> invoiceNoList = new List<string>();
            var invoiceNoList = claim.Invoices.Split(',').ToList();

            //filter the selected invoice
            List<Invoice> filteredInvoices = invList.Where(inv => invoiceNoList.Contains(inv.InvoiceNo)).ToList();

            DataClaimOfPayment dataClaimOfPayment = new DataClaimOfPayment();
            dataClaimOfPayment.CaseId = caseNo;
            dataClaimOfPayment.CaseNote = claim.Note;
            dataClaimOfPayment.Email = claim.Email;
            dataClaimOfPayment.SmsNumber = claim.Mobile;
            dataClaimOfPayment.CardNumber = claim.Cardnumber;
            dataClaimOfPayment.PaymentMethod = claim.Method;
            dataClaimOfPayment.PayerName = "Debtor " + claim.CaseNumber.ToString();
            dataClaimOfPayment.FromAccount = claim.FromAccount;
            dataClaimOfPayment.ToAccount = claim.ToAccount;
            dataClaimOfPayment.Kid = claim.Kid;
            dataClaimOfPayment.ToAccount = claim.ToAccount;
            dataClaimOfPayment.IsPrincipal = claim.IsPrincipal;
            dataClaimOfPayment.PaidAmount = claim.Amount;
            dataClaimOfPayment.PaidDate = claim.Date;
            dataClaimOfPayment.InvoiceList = filteredInvoices;

            int claimRegistered = await dataManager.RegisterClaimOfPayment(dataClaimOfPayment);
            if (claimRegistered > 0)
            {
                //add receipts
                claimRegisteredResponse = true;
            }
            else
            {
                throw new CustomtException("Failed to register the claim of payment");
            }
            DPCaseNote note = new DPCaseNote();
            note.ReferenceNumber = caseNo;

            note.NoteText = string.Empty;
            if (claim.CaseType == "C")
            {
                //note.ActivityName = "DPClaimOfPaymentCase";
                note.ActivityName = "DebtorClaimsToHavePayToCreditor";
                note.Type = "Case";
            }
            else
            {
                //note.ActivityName = "DPClaimOfPaymentSubCase";
                note.ActivityName = "SubCaseDebtorClaimsToHavePayToCreditor";
                note.Type = "SubCase";
            }
            var noteResult = await dataManager.CreateDPortalCaseNote(note);

            return claimRegisteredResponse;
        }

        public async Task<string> GenerateGDPRDocument(int caseNumber, int arNo, string caseType, CancellationToken cancellationToken = default)
        {
            //string entityType= await dataManager.GetCaseMetaData(caseNumber, cancellationToken);
            //string ActivityCode = DPAPIConfig.GenerateGDPRActivityCode;
            //string ActResponse= await CallActivityExecuteApiAsync(ActivityCode, caseType, caseNumber);

            //ActivityRequest Arequest = new ActivityRequest();
            //Arequest.ActivityCode = ActivityCode;
            //Arequest.ActivityVersion = "V1";
            //Arequest.ExecutingDateTime = DateTime.Now;
            //Arequest.CaseId = caseNumber;
            //Arequest.EntityType = caseType;
            //ActivityParameter activityParameter1 = new ActivityParameter();
            //activityParameter1.Name = "Caseno";
            //activityParameter1.DataType = "int";
            //activityParameter1.Value = caseNumber.ToString();
            //ActivityParameter activityParameter2 = new ActivityParameter();
            //activityParameter2.Name = "Username";
            //activityParameter2.DataType = "varchar";
            //activityParameter2.Value = "DP-" + caseNumber.ToString();
            //Arequest.ParameterList.Add(activityParameter1);
            //Arequest.ParameterList.Add(activityParameter2);
            ActivityRequest activity= new ActivityRequest();
            activity.ActivityCode= DPAPIConfig.GenerateGDPRActivityCode;
            activity.ExecutingDateTime = DateTime.Now;
            activity.CaseId = arNo;
            activity.EntityType = caseType;
            activity.uniqueId = -1;
            ActivityParameter para1 = new ActivityParameter();
            para1.Name = "username";
            para1.DataType = "varchar";
            para1.Value= "DPAPI:DP-" + caseNumber.ToString();
            List<ActivityParameter> listPara = new List<ActivityParameter>();
            listPara.Add(para1);
            activity.ParameterList= listPara; 

            ActivityServiceSetting activityserviceSettings = new ActivityServiceSetting();
            ActivityExecutionDetail activityDetails = await dataManager.GetActivity(DPAPIConfig.GenerateGDPRActivityCode, arNo, caseType);
            activity.ActivityVersion = activityDetails.ActivityVersion;
            activityserviceSettings = await dataManager.GetActivityServiceSettings(DPAPIConfig.GenerateGDPRActivityCode, caseType);
            if (string.IsNullOrEmpty(activityserviceSettings.ActivityServiceBasePath) || string.IsNullOrEmpty(activityserviceSettings.ActivityServiceBasePath))
            {
                throw new Exception("One or more activity service settings not found");
            }
            var user = activityserviceSettings.BureaueCode.Split("\\")[1];
            string activityApi = activityserviceSettings.ActivityServiceBasePath + "api/Activity?user=" + user + "&userCode=" + activityserviceSettings.BureaueCode;
            
            string ActResponse = await ExecuteActivity(activityApi, activity, activityserviceSettings);
            //no such activity found on cc uat/prod so comment this
            //DPCaseNote request = new DPCaseNote();
            //request.NoteText = "Personopplysninger er ferdig nedlastet";
            //request.CreatedUser = caseNumber.ToString();
            //request.ReferenceNumber = caseNumber;
            //if (caseType == "C")
            //{
            //    request.Type = "Case";
            //    request.ActivityName = "DPGDPRRequestCase";
            //}
            //else
            //{
            //    request.Type = "SubCase";
            //    request.ActivityName = "DPGDPRRequestSubCase";
            //}

            //var result1 = await dataManager.CreateDPortalCaseNote(request, cancellationToken);
            try
            {
                Byte[] bytes = File.ReadAllBytes(ActResponse.ToString());
                String file = Convert.ToBase64String(bytes);
                return file;
            }
            catch (Exception)
            {

                return ActResponse;
            }
           
        }


        public async Task<byte[]> GetDocument(int caseNumber, string refNumber, string docType, string caseType, int integration, CancellationToken cancellationToken = default)
        {
            byte[] doc = null;
            //get settings
            var settings = await dataManager.GetDebtorPortalSettings(caseNumber);
            IntegrationType integrationCode = IntegrationType.Azure;
            int ActivityCode =Convert.ToInt32( DPAPIConfig.InvoiceActivityCode); //6038;
            string documentPath = await dataManager.GetDocumentPath(caseNumber, refNumber, docType, ActivityCode);
            //testing para
            //119, "674432", "Invoice", ActivityCode
            //az:jpg/dpuploadclaims/77b14a5a-4448-4693-9b6e-9705e5b737c7_claim.jpg
            //az:pdf/20250217/6038_415_58_345345_36751440_V2.pdf
            string[] documentpathsegments = documentPath.Split('/');
            int segmentCount = documentpathsegments.Length;
            string filePath = documentPath;
            if (segmentCount > 0)
            {
               
                string containerName = documentpathsegments[0];
                if (containerName != "entitydocuments")
                {
                    containerName = DPAPIConfig.AzureBlobStorageContainerName;
                    filePath = documentPath.Replace(documentpathsegments[0], "");
                }
                else
                {
                    filePath = documentPath.Replace(documentpathsegments[0], "");
                }
            }
            else
            {
                throw new CustomtException("Document path is not valid");
            }
            int length = segmentCount - 1;
                string docName = documentpathsegments[length];
            if (documentPath.ToLower().Contains("https://") && documentPath.ToLower().Contains("stralfors"))
            {
                integrationCode = IntegrationType.Stralfors;
                // documentFactory = new StralforsDocumentFactory();
            }
            else if (documentPath.ToLower().Contains("https://") && documentPath.ToLower().Contains("fairdc"))
            {
                integrationCode = IntegrationType.Mercury;
                //documentFactory = new MercuryDocumentFactory();
            }
            else if (documentPath.ToLower().Contains("az:"))
            {
                integrationCode = IntegrationType.Azure;
                // documentFactory = new AzureDocumentFactory();
                doc = await AzureBolbStorageCommon.DownloadFileAsync(filePath,DPAPIConfig.AzureBlobStorageContainerName,DPAPIConfig.ClientID);
            }
            else
            {
                integrationCode = IntegrationType.Local;
                //documentFactory = new LocalDocumentFactory();
            }
            //create note
            DPCaseNote note = new DPCaseNote();
            note.ReferenceNumber = caseNumber;
            note.CreatedUser = caseNumber.ToString();
            note.NoteText = "Downloaded a invoice copy of case " + caseNumber.ToString();

            if (caseType == "C")
            {
                note.ActivityName = "DPInvoiceCopyRequestCase";
                note.Type = "Case";
            }
            else
            {
                note.ActivityName = "DPInvoiceCopyRequestSubCase";
                note.Type = "SubCase";
            }
            var noteResult = await dataManager.CreateDPortalCaseNote(note);
            return doc;
        }


        public async Task<bool> SendInvoiceCopy(int caseNo, InvoiceCopy invoiceDetails, CancellationToken cancellationToken = default)
        {
            byte[] fileBytes = new byte[0];
            var settings = await dataManager.GetDebtorPortalSettings(caseNo);
            string docPath = await dataManager.GetDocumentPath(caseNo, invoiceDetails.InvoiceNumber, "INVOICE", Convert.ToInt32(DPAPIConfig.InvoiceActivityCode));
            docPath = docPath.Replace("az:", "");
            string[] documentpathsegments = docPath.Split('/');
            int segmentCount = documentpathsegments.Length;
            if (segmentCount > 0)
            {
                string filePath = docPath;
                string containerName = documentpathsegments[0];
                //if (containerName != "entitydocuments")
                //{
                //    containerName = DPAPIConfig.AzureBlobStorageContainerName;
                    filePath = docPath.Replace(containerName, "");
                //}
                //else
                //{
                //    filePath = docPath.Replace(documentpathsegments[0], "");
                //}
                // get document string from blob storage 
                // return DownloadFromBlobStorage(filePath, clientId, containerName);
                fileBytes = await AzureBolbStorageCommon.DownloadFileAsync(filePath.ToString(), containerName, DPAPIConfig.ClientID);

               // invoiceDetails.Message = Encoding.UTF8.GetString(ProtectedData.Unprotect(Base64UrlEncoder.DecodeBytes(invoiceDetails.Message), null, DataProtectionScope.LocalMachine));

                Attachment att = new Attachment(new MemoryStream(fileBytes), invoiceDetails.InvoiceNumber + ".pdf");

                SmtpClient client = new SmtpClient();
                client.Port = Convert.ToInt32(settings["SMTPPort"]);
                client.Host = settings["SMTPServer"];
                //client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(settings["SMTPUser"], settings["SMTPPassword"]);

                MailMessage mm = new MailMessage(settings["SMTPUser"], invoiceDetails.Email, invoiceDetails.Subject, invoiceDetails.Message);
                mm.IsBodyHtml = true;
                mm.Attachments.Add(att);
                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.Send(mm);
                return true;
            }
            else
            { return false; }
             
        }

            #region private methods


            //public  async Task<string> CallActivityExecuteApiAsync(string ActivityCode,string EntityType,int caseNo)
            //{
            //    try
            //    {
            //        var client = httpClient.CreateClient();
            //        string baseUrl = DPAPIConfig.ActivityExecutionAPIURL;
            //        string user = DPAPIConfig.ActivityExecutionAPIUser;
            //        string userCode = DPAPIConfig.ActivityExecutionAPIUSerCode;
            //        Cryptography cryptography = new Cryptography();
            //        string encriptionKey = DPAPIConfig.EncriptionKey;
            //        string constring = DPAPIConfig.ConnectionString;

            //        string cypherConnectionString = cryptography.encrypt(constring, encriptionKey);

            //        string apiUrl = $"{baseUrl}?user={user}&userCode={userCode}&cypherdbConnection={cypherConnectionString}";
            //        // Create request body
            //        var requestBody = new
            //        {
            //            UniqueId = -1,
            //            ActivityCode = ActivityCode,
            //            ExecutingDateTime = DateTime.Now,
            //            CaseId = caseNo,
            //            EntityType = EntityType,
            //            ParameterList = new[]
            //            {
            //            new
            //            {
            //                Name = "username",
            //                DataType = "varchar",
            //                Value=DPAPIConfig.ActivityExecutionAPIUser
            //            }
            //        }
            //        };

            //        // Convert to JSON
            //        string jsonBody = System.Text.Json.JsonSerializer.Serialize(requestBody);
            //        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            //        // Read response
            //        var response = await client.PostAsync(apiUrl, content);
            //        var responseContent = await response.Content.ReadAsStringAsync();
            //        return responseContent.ToString();
            //        //if (response.IsSuccessStatusCode)
            //        //{
            //        //    //Console.WriteLine("Success! Response:");
            //        //    //Console.WriteLine(responseContent);
            //        //}
            //        //else
            //        //{
            //        //    //Console.WriteLine($"Error: {response.StatusCode}");
            //        //    //Console.WriteLine(responseContent);
            //        //}
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Exception: {ex.Message}");
            //        return "";
            //    }
            //}
            private async Task<List<int>> Lookups(string phoneNumber, string birthDate, string nnin, string companyCode)
        {
           var resultList = new List<int>();
            var returnResult = new List<int>();

            var resultSSN =  FirstLookup(nnin);
            returnResult.AddRange( resultSSN.Result);
            //if (!resultSSN.ErrorOccured)
            //{
            //    resultList.AddRange(resultSSN.OperationReturnValue);
            //}
            //else
            //{
            //    returnResult = resultSSN;
            //}


            var resultByDetails = SecondLookup(birthDate, phoneNumber, companyCode);
            returnResult.AddRange(resultByDetails.Result);
            //if (!resultByDetails.ErrorOccured)
            //{
            //    resultList.AddRange(resultByDetails.OperationReturnValue);
            //}
            //else
            //{
            //    returnResult = resultByDetails;
            //}

            returnResult = resultList.Distinct().ToList();
             //resultList.GroupBy(x => x.CaseNumber).Select(y => y.First()).ToList();
            return returnResult;
        }

        private async Task<List<int>> FirstLookup(string nnin)
        {
            if (await featureManager.IsEnabledAsync("VIPPSIFirstLookupEnabled"))
            {
                var casesBySSN = await dataManager.GetDebtorsBySSN(nnin);
                //if (casesBySSN.ErrorOccured)
                //{
                //    foreach (USBOXNotificationMessage message in casesBySSN.Notifications)
                //    {

                //        logger.Error("error occured in get debtors by ssn error- " + message.Message + "related to nnin - " + nnin);

                //    }
                //}
                return casesBySSN;
            }
            else
            {
                return new List<int>();
            }

        }
        
        private async Task<List<int>> SecondLookup(string birthDate, string phoneNumber, string companyCode)
        {
            if (await featureManager.IsEnabledAsync("VIPPSSecondLookupEnabled"))
            {
                var casesByDetails = await dataManager.GetDebtorsByDebtorDetails(birthDate, phoneNumber);
                //if (casesByDetails.ErrorOccured)
                //{
                //    //foreach (USBOXNotificationMessage message in casesByDetails.Notifications)
                //    //{

                //    //    //logger.Error("error occured in get debtors by debtor details error- " + message.Message + "related to birthdate - " + birthDate + ", phone number " + phoneNumber);

                //    //}
                //}
                return casesByDetails;
            }
            else
            {
                return new List<int>();
            }

        }

        private async Task<List<DebtorCase>> GetDistinctCasesByDebtors(List<int> entRoleIDList, int caseno, string companyCode)
        {
            var casesByDebtors = await dataManager.GetCasesByDebtors(entRoleIDList, caseno);
            casesByDebtors = casesByDebtors.GroupBy(x => x.CaseNumber).Select(y => y.First()).ToList();
            return casesByDebtors;
        }

        public async Task<string> ExecuteActivity(string activityApi,  ActivityRequest activity,  ActivityServiceSetting activityserviceSettings, CancellationToken cancellationToken = default)
        {
            try
            {
                ActivityExecutionDetail attachmentExecusion = null;

                //var activityDetails = await dataManager.GetActivity(activity.ActivityCode, activity.CaseId, activity.EntityType,cancellationToken);

                if (activityserviceSettings.AttachmentActivityCode > 0)
                {
                    attachmentExecusion = await dataManager.GetActivity(activityserviceSettings.AttachmentActivityCode.ToString(), activity.CaseId, activity.EntityType, cancellationToken);
                }

                if (activityserviceSettings.ActivityType == "SP")
                {
                    throw new CustomtException("SP activity not available in DP now");
                    //log.LogInformation("ActivityService-ExecuteActivity SP activity.");

                    //var parameters = new SPActivitySettings();
                    //foreach (var parameter in activity.ParameterList)
                    //{
                    //    parameters.ParameterList.Add(new SQLSPParameter()
                    //    {
                    //        ValueType = Payment.Core.DomainObjects.RutineAndActivities.ValueType.Parameter,
                    //        Value = parameter.Value,
                    //        Name = UpdateParameterName(parameter.Name),
                    //        DataType = parameter.DataType
                    //    });
                    //}

                    //var result = OrderSPActivity(-1, activity.ActivityCode, activityDetails, activity.ExecutingDateTime, parameters, activity.CaseId, authorizedRequest.AuthorizedUser.TenantUserLogin, activity.EntityType, authorizedRequest.AuthorizedUser.UserDBConnectionString, log);
                    //if (result.IsSuccess)
                    //{
                    //    return ResponseHandler.HandleOKResult(result.Message);
                    //}
                    //else
                    //{
                    //    return RequestHandler.HandleBadRequest(null, result.Message, log);
                    //}
                }
                else
                {
                    if (activity.ActivityVersion == "V2" && activityserviceSettings.ActivityType == "Message")
                    {
                        throw new CustomtException("Message activity V2 is not support by DP now");
                        // return ExecuteActivityAPI_V2(activityDetails, activityserviceSettings, authorizedRequest, activity, log, attachmentExecusion);
                    }
                    else
                    {
                        return await ExecuteActivityAPI_V1(activityApi, DPAPIConfig.ConnectionString, activity );
                    }
                }
            }

            catch (Exception ex)
            { 
                throw new BadHttpRequestException("Error in activity execution");
            }
        }
        private async Task<string> ExecuteActivityAPI_V1(string activityApi, string connectionString, ActivityRequest activity)
        {
            string StrResponse = "";
            string encriptionKey = DPAPIConfig.EncryptionKey;
            string cypherConnectionString = string.Empty;

            if (string.IsNullOrEmpty(encriptionKey))
            {
                throw new CustomtException("EncriptionKey is not found");
            }
            else
            {
                Cryptography cryptography = new Cryptography();
                cypherConnectionString = cryptography.encrypt(connectionString, encriptionKey);
            }
            var client =httpClient.CreateClient();
            var jsonData = JsonConvert.SerializeObject(activity);
            string activityapi = activityApi + "&cypherdbConnection=" + cypherConnectionString;
            var data = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(activityapi, data).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                //return ResponseHandler.HandleOKResult(result.Replace("\"", ""));
                StrResponse = result.Replace("\"", "");
            }
            else
            {
                string errorMessage = response.Content.ReadAsStringAsync().Result;
                // return RequestHandler.HandleBadRequest(null, errorMessage.Replace("\"", ""), log);
                StrResponse = errorMessage;
            }
            return StrResponse;
        }
        public async Task<string> ExecuteActivityAPI_V2(ActivityExecutionDetail activityExecutionDetail, ActivityServiceSetting activityserviceSettings, ActivityRequest activity, ActivityExecutionDetail attachmentActivity)
        {
            DateTime univeralExecutedTime = activity.ExecutingDateTime.ToUniversalTime();
            DateTime currentTime = DateTime.Now.ToUniversalTime();
            string param = "";

            if (activityserviceSettings.AttachmentActivityCode > 0)
            {
                var actvityResponce =await HandleAttachmentActivity(activity, attachmentActivity, activityserviceSettings, activityserviceSettings.AttachmentActivityCode);
                if (actvityResponce.StatusCode == 200)
                {
                    param = await HandleActivity(activityExecutionDetail, activityserviceSettings, activity, actvityResponce.Result);
                }
                else
                {
                    //return RequestHandler.HandleBadRequest(null, "error", log);
                }
            }
            else
            {
                param = await HandleActivity(activityExecutionDetail, activityserviceSettings, activity, string.Empty);
            }
            return param;
        }

        private async Task<string> HandleActivity(ActivityExecutionDetail activityExecutionDetail, ActivityServiceSetting activityserviceSettings, ActivityRequest activity, string attachmentpath)
        {
            var errormessage = string.Empty;
            string param = string.Empty;
            int queueID = -1;
            try
            {
                //add pending queue

                string templateName = string.Empty;
                string templateCategory = string.Empty;
                if (!string.IsNullOrEmpty(activityExecutionDetail.MessageType))
                {
                    string[] templateDetails = activityExecutionDetail.MessageType.Split('-');
                    if (templateDetails.Length > 1)
                    {
                        templateName = templateDetails[1];
                        templateCategory = templateDetails[0];
                    }
                }

                queueID = await dataManager.UpdateCommunicationQueue( activityExecutionDetail.CaseNo.ToString(), activityExecutionDetail.WorkFlowType, templateName, templateCategory, -1, 0, activityExecutionDetail.Id, activityExecutionDetail.Name, -1, activity.ActivityVersion);

                if (queueID > 0)
                {
                    //prepare the api client 
                    
                    HttpClient uscClient = httpClient.CreateClient("USBOXAPI");
                    string Token = "";
                    uscClient.DefaultRequestHeaders.Add("Authorization", $"Bearer "+Token);

                    var generationRequest = new GenerationRequest()
                    {
                        ActivityCode = Convert.ToInt32(activity.ActivityCode),
                        CategoryId = activityserviceSettings.TemplateCategoryId,
                        DataItem = queueID,
                        DataSource = activityserviceSettings.DataSource,
                        Attachments = new List<string>() { attachmentpath },
                        Group = activity.EntityType

                    };

                    var dataRequest = new StringContent(JsonConvert.SerializeObject(generationRequest), Encoding.UTF8, "application/json");
                    var response = uscClient.PostAsync("communicationservice/v1/output/manualgeneration", dataRequest);
                    Task.WaitAll(response);

                    if (response.Result.IsSuccessStatusCode)
                    {
                        var result = JsonConvert.DeserializeObject<OutputGenerationResponse>(response.Result.Content.ReadAsStringAsync().Result);
                        if (result.StatusCode == 200)
                        {
                            //todo get the param 
                            param = result.RowFiles[0];

                            //update execution status 
                            await dataManager.AddExecutedActivity(-1, queueID, activityExecutionDetail.Name, errormessage, activityExecutionDetail.CaseNo,string.Empty, activityExecutionDetail.EntityType,
                                             "DPAPI", param, DateTime.MinValue, -1, attachmentpath,   activity.ActivityVersion,true);

                            if (activityExecutionDetail.Type == "Message")
                            {
                                await dataManager.UpdateMainDocumentDate( activityExecutionDetail.CaseNo, activityExecutionDetail.Name, activityExecutionDetail.WorkFlowType);
                            }

                            //return ResponseHandler.HandleOKResult("success");
                        }
                        else
                        {
                            errormessage = result.Message;
                            await dataManager.AddExecutedActivity(-1, queueID, activityExecutionDetail.Name, errormessage, activityExecutionDetail.CaseNo, errormessage, activityExecutionDetail.EntityType,
                                             "DPAPI", param, DateTime.MinValue, -1, attachmentpath, activity.ActivityVersion, false);
                            //return RequestHandler.HandleBadRequest(null, "error", log);
                        }
                    }
                    else
                    {
                        errormessage = "Error in manual document generation";
                        await dataManager.AddExecutedActivity(-1, queueID,  activityExecutionDetail.Name, errormessage, activityExecutionDetail.CaseNo,errormessage, activityExecutionDetail.EntityType,
                                             "DPAPI", param, DateTime.MinValue, -1, attachmentpath, activity.ActivityVersion, false);
                       
                        //return RequestHandler.HandleBadRequest(null, "Error in document manual generation ", log);
                    }
                }
                else
                {
                    errormessage = "QueueId is not found";
                    await dataManager.AddExecutedActivity(-1, queueID, activityExecutionDetail.Name, errormessage, activityExecutionDetail.CaseNo, errormessage, activityExecutionDetail.EntityType,
                                              "DPAPI", param, DateTime.MinValue, -1, attachmentpath, activity.ActivityVersion, false);
                    //return new BadRequestObjectResult("QueueId is not found");
                }
            }
            catch (Exception ex)
            {
                errormessage = ex.Message;
                await dataManager.AddExecutedActivity(-1, queueID, activityExecutionDetail.Name, errormessage, activityExecutionDetail.CaseNo, errormessage, activityExecutionDetail.EntityType,
                                              "DPAPI", param, DateTime.MinValue, -1, attachmentpath, activity.ActivityVersion, false);
                
            }
            return param;
        }
        private async Task<ActvityResponce> HandleAttachmentActivity(ActivityRequest activityrequest, ActivityExecutionDetail activityExecutionDetail, ActivityServiceSetting activityserviceSettings, int activityCode)
        {
            var errormessage = string.Empty;
            var param = string.Empty;
            int queueID = -1;
            try
            {
                //add pending queue

                string templateName = string.Empty;
                string templateCategory = string.Empty;
                if (!string.IsNullOrEmpty(activityExecutionDetail.MessageType))
                {
                    string[] templateDetails = activityExecutionDetail.MessageType.Split('-');
                    if (templateDetails.Length > 1)
                    {
                        templateName = templateDetails[1];
                        templateCategory = templateDetails[0];
                    }
                }

                queueID = await dataManager.UpdateCommunicationQueue( activityExecutionDetail.CaseNo.ToString(), activityExecutionDetail.WorkFlowType, templateName, templateCategory, -1, 0, activityExecutionDetail.Id, activityExecutionDetail.Name, -1, activityrequest.ActivityVersion);

                if (queueID > 0)
                {
                    //prepare the api client 
                    HttpClient uscClient = httpClient.CreateClient("USBOXAPI");
                    string Token = "";
                    uscClient.DefaultRequestHeaders.Add("Authorization", $"Bearer "+ Token);

                    var generationRequest = new GenerationRequest()
                    {
                        ActivityCode = activityCode,
                        CategoryId = activityserviceSettings.AttachmentActivityCategoryId,
                        DataItem = queueID,
                        DataSource = activityserviceSettings.DataSource,
                        Group = activityrequest.EntityType
                    };

                    var dataRequest = new StringContent(JsonConvert.SerializeObject(generationRequest), Encoding.UTF8, "application/json");
                    var response = uscClient.PostAsync("communicationservice/v1/output/manualgeneration", dataRequest);
                    Task.WaitAll(response);

                    if (response.Result.IsSuccessStatusCode)
                    {
                        var result = JsonConvert.DeserializeObject<OutputGenerationResponse>(response.Result.Content.ReadAsStringAsync().Result);
                        if (result.StatusCode == 200)
                        {
                            //todo get the param 
                            param = result.RowFiles[0];

                            //update execution status 
                            await dataManager.AddExecutedActivity(-1, queueID,  activityExecutionDetail.Name, String.Empty, activityExecutionDetail.CaseNo,string.Empty, activityExecutionDetail.EntityType,
                                             "DPAPI", param, DateTime.MinValue, -1, string.Empty, activityrequest.ActivityVersion, true);
                            return new ActvityResponce()
                            {
                                StatusCode = result.StatusCode,
                                Result = param,
                                Message = "success"
                            };
                        }
                        else
                        {
                            errormessage = result.Message;

                            await dataManager.AddExecutedActivity(-1, queueID, activityExecutionDetail.Name, String.Empty, activityExecutionDetail.CaseNo, errormessage, activityExecutionDetail.EntityType,
                                            "DPAPI", param, DateTime.MinValue, -1, string.Empty, activityrequest.ActivityVersion, true);

                            return new ActvityResponce()
                            {
                                StatusCode = result.StatusCode,
                                Result = string.Empty,
                                Message = "error"
                            };
                        }
                    }
                    else
                    {
                        errormessage = "Error in manual document generation ";
                        await dataManager.AddExecutedActivity(-1, queueID, activityExecutionDetail.Name, String.Empty, activityExecutionDetail.CaseNo, errormessage, activityExecutionDetail.EntityType,
                                            "DPAPI", param, DateTime.MinValue, -1, string.Empty, activityrequest.ActivityVersion, false);
                        return new ActvityResponce()
                        {
                            StatusCode = 400,
                            Result = string.Empty,
                            Message = "Error in document manual generation "
                        };
                    }
                }
                else
                {
                    errormessage = "QueueId is not found";
                    await dataManager.AddExecutedActivity(-1, queueID, activityExecutionDetail.Name, String.Empty, activityExecutionDetail.CaseNo, errormessage, activityExecutionDetail.EntityType,
                                             "DPAPI", param, DateTime.MinValue, -1, string.Empty, activityrequest.ActivityVersion, false);
                    return new ActvityResponce()
                    {
                        StatusCode = 400,
                        Result = string.Empty,
                        Message = "QueueId is not found"
                    };
                }
            }
            catch (Exception ex)
            {
                errormessage = ex.Message;
                await dataManager.AddExecutedActivity(-1, queueID, activityExecutionDetail.Name, String.Empty, activityExecutionDetail.CaseNo, errormessage, activityExecutionDetail.EntityType,
                                             "DPAPI", param, DateTime.MinValue, -1, string.Empty, activityrequest.ActivityVersion, false);
                return new ActvityResponce()
                {
                    StatusCode = 500,
                    Result = string.Empty,
                    Message = ex.Message
                };
            }
        }
        private IFormFile ConvertHtmlToIFormFile(string htmlContent, string fileName, string contentType)
        {
            // Convert HTML content to byte array
            byte[] fileBytes = Encoding.UTF8.GetBytes(htmlContent);

            var stream = new MemoryStream(fileBytes);

            // Create an IFormFile from the memory stream
            IFormFile formFile = new FormFile(stream, 0, fileBytes.Length, fileName, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            // Create a memory stream from the byte array
            //using (var stream = new MemoryStream(fileBytes))
            //{
            //    // Create an IFormFile from the memory stream
            //    IFormFile formFile = new FormFile(stream, 0, fileBytes.Length, fileName, fileName)
            //    {
            //        Headers = new HeaderDictionary(),
            //        ContentType = contentType
            //    };

               return formFile;
            //}
        }

        private IFormFile ConvertByteArrayToIFormFile(byte[] fileBytes, string fileName, string contentType)
        {
            var stream = new MemoryStream(fileBytes);
            return new FormFile(stream, 0, fileBytes.Length, fileName, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }
        private string GetOrderLinesHTML(List<ReciptPaymentDetail> ReciptPaymentDetail)
        {
            StringBuilder res = new StringBuilder("");
            if (ReciptPaymentDetail.Any())
            {


                foreach (var item in ReciptPaymentDetail)
                {
                    res.Append("<tr><td style = \"width: 180px; \" >" + item.ItemText + "</td>");
                    res.Append("<td style=\"width: 10px; \">:</td>");
                    res.Append("<td style = \"font-weight: bold \" >" + item.ItemAmount.ToString("N2") + "</td></tr>");

                }

            }

            return res.ToString();
        }

        private static XmlDocument NetQuery(string merchantID, string merchantKey, string paymentID, string queryURL)
        {
            
                queryURL = queryURL + "/Netaxept/Query.aspx?";
                var url = new UriBuilder(queryURL);

                url.Query = $"merchantId={merchantID}&token={merchantKey}&transactionId={paymentID}";
                //logger.Info("url: " + url.ToString());
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url.ToString());
                httpWebRequest.CookieContainer = new CookieContainer();
                httpWebRequest.AllowAutoRedirect = false;
                httpWebRequest.Method = "GET";

                string result = "";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                //logger.Info(JsonConvert.SerializeObject(result));
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                return doc;
        }
        private SecurityToken ValidateBankToken(string token, string url, string username)
            {
                string configurl = url.Replace("token", ".well-known/openid-configuration"); // this is fixed in oid
                IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(configurl, new OpenIdConnectConfigurationRetriever());
                OpenIdConnectConfiguration openIdConfig = configurationManager.GetConfigurationAsync(CancellationToken.None).Result;
                TokenValidationParameters validationParameters =
                new TokenValidationParameters
                {
                    ValidIssuer = url.Replace("/token", ""),
                    ValidAudiences = new[] { username },
                    IssuerSigningKeys = openIdConfig.SigningKeys
                };
                SecurityToken validatedToken;
                var claims = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out validatedToken);
                return validatedToken;

            }

            
        

        #endregion
    }
    

}
