
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using US.BOX.BoxAPI.Data.Models;

namespace US.BOX.BoxAPI.Core.Interfaces
{
    public interface IDebtorPortalAPIService
    {
        Task<IDictionary<string, string>> GetDebtorPortalSettings(int caseNumber = 0, CancellationToken cancellationToken = default);
        Task<PortalCase> GetPortalCase(int caseNumber , CancellationToken cancellationToken = default);
        Task<List<Timeline>> GetTimelinedata(int caseNumber, CancellationToken cancellationToken = default);

        Task<string> GetCaseStatus(int caseNumber, CancellationToken cancellationToken = default);

        Task<bool> UpdateContact(int caseNo, Contact contact, CancellationToken cancellationToken = default);

        Task<List<SystemCategory>> GetSystemCategories(string category, int arNo, CancellationToken cancellationToken = default);

        Task<bool> PrivacyPolicyUpdate(int caseNo, bool state, CancellationToken cancellationToken = default);

        Task<string> openIdConfigurations(int caseNo, string companyCode, CancellationToken cancellationToken = default);

        Task<string> bankIDProviders(int caseNo, string companyCode, CancellationToken cancellationToken = default);

        //Task<KeyValuePair<string, string>> GetDocumentPathByDocId(string documentId, CancellationToken cancellationToken = default);
        Task<byte[]> ReadMessageDocument(int CaseNo,string documentId, CancellationToken cancellationToken = default);
        Task<List<DebtorCase>> VippsBankIDLogin(string authCode, int caseNumber, string openIDConfiguration, string companyCode, CancellationToken cancellationToken = default);

        //Task<bool> ObjectionRespond(int caseNo,DPObjectionRespond respond, CancellationToken cancellationToken = default);

        Task<bool> Objection(int caseNo, PortalObjection objection, CancellationToken cancellationToken = default);

        Task<List<PartPayment>> PartPayment(int caseNo, PartpaymentRequest partpaymentRequest, CancellationToken cancellationToken = default);

        //Task<CalculatePartPayment> CalculatePartPayment(int caseNo, DataPartPaymentRequest _partPayment, CancellationToken cancellationToken = default);
        //Task<int> RegisterPaymentAgreement(USPPaymentAgreement _paymentAgreement, CancellationToken cancellationToken = default);

        //Task<int> CreateDPortalCaseNote(DPCaseNote _request, CancellationToken cancellationToken = default);

        Task<List<DebtorCase>> BankIDLogin(int caseNo, string authCode,  CancellationToken cancellationToken = default);

        // Task<List<DebtorCase>> GetPortalCasesByDebtor(string ssno, int caseNumber, CancellationToken cancellationToken = default);

        Task<string> RegisterPortalPayment(int caseNo, CardPayment paymentObj, CancellationToken cancellationToken = default);

        Task<bool> AuthorizePayment( int caseNo, string transRef, CancellationToken cancellationToken = default);

        Task<List<string>> UploadClaimOfPaymentReceipts( int caseNo, List<IFormFile> file, CancellationToken cancellationToken = default);

        Task<string> GetCPReciptPdf(string caseNo, string KID, string companyCode, CancellationToken cancellationToken = default);

        Task<int> CreateCaseNote(int CaseNo, DPCaseNote casenote, CancellationToken cancellationToken = default);

        //Task<DateTime> PostponeCase(int CaseNo, Contact contact, CancellationToken cancellationToken = default);

        Task<Contact> GetPortalDebtor(int caseNo, CancellationToken cancellationToken = default);

        Task<bool> ClaimOfPayment(int caseNo, ClaimOfPayment claim, CancellationToken cancellationToken = default);

        Task<string> GenerateGDPRDocument(int caseNumber, int arNo, string caseType, CancellationToken cancellationToken = default);

        Task<byte[]> GetDocument(int caseNumber, string refNumber, string docType, string caseType, int integration, CancellationToken cancellationToken=default);

        //Task<List<string>> GetDocument(int lookupId, string system, string lookupType, CancellationToken cancellationToken = default);

        Task<bool> SendInvoiceCopy(int caseNo, InvoiceCopy invoiceDetails, CancellationToken cancellationToken = default);
    }
}
