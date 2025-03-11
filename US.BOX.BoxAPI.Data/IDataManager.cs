
using System.Transactions;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;
using Transaction = US.BOX.BoxAPI.Data.Models.Transaction;

namespace US.BOX.BoxAPI.Data
{
    public interface IDataManager
    {
        Task<TenentDetail> GetTenantDBCommonConnection(string TenantId, CancellationToken cancellationToken = default);
        Task<int> CheckPortalUser(string username, string password, CancellationToken cancellationToken = default);
        Task<IDictionary<string, string>> GetDebtorPortalSettings(int caseNumber, CancellationToken cancellationToken = default);
        Task<PortalCase> GetPortalCase(int caseNumber, CancellationToken cancellationToken = default);
        Task<List<PartPayment>> GetPartPayment(int caseNumber, CancellationToken cancellationToken = default);
        Task<List<Timeline>> GetTimelinedata(int caseNumber, CancellationToken cancellationToken = default);

        Task<string> GetCaseStatus(int caseNumber, CancellationToken cancellationToken = default);
        Task<bool> UpdateContact(int caseNo, Contact contact, CancellationToken cancellationToken = default);

        Task<List<SystemCategory>> GetSystemCategories(string category, int arNo, CancellationToken cancellationToken = default);

        Task<bool> PrivacyPolicyUpdate(int caseNo, bool state, CancellationToken cancellationToken = default);

        Task<KeyValuePair<string, string>> GetDocumentPathByDocId(string documentId, CancellationToken cancellationToken = default);

        Task<CalculatePartPayment> CalculatePartPayment(DataPartPaymentRequest _partPayment, CancellationToken cancellationToken = default);
        Task<int> RegisterPaymentAgreement(USPPaymentAgreement _paymentAgreement, CancellationToken cancellationToken = default);

        Task<int> CreateDPortalCaseNote(DPCaseNote _request, CancellationToken cancellationToken = default);
        Task<List<DebtorCase>> GetPortalCasesByDebtor(string ssno, int caseNumber, CancellationToken cancellationToken = default);
        Task<string> GetNewPaymentKid(int caseno, CancellationToken cancellationToken = default);
        Task<bool> AddDPCardPayment(int caseno, string kid, string paymentRef, decimal amount, CancellationToken cancellationToken = default);

        Task<bool> AddDPCardPaymentDetails(int status, ProcessResponse obj, CancellationToken cancellationToken = default);

        Task<Transaction> GetCardPaymentData(String transID, CancellationToken cancellationToken = default);

        Task<string> GetCaseMetaData(int caseNo, CancellationToken cancellationToken = default);

        Task<List<ObjectionProfile>> GetObjectionProfile(int caseNo, CancellationToken cancellationToken = default);

        Task<List<SubCase>> GetSubcasesByCaseNo(int CaseNo, CancellationToken cancellationToken = default);

        Task<int> RegisterDebtorObjection(DebtorObjection _debtorObjection, CancellationToken cancellationToken = default);

        Task<int> RegisterClaimOfPayment(DataClaimOfPayment _claimOfPayment, CancellationToken cancellationToken = default);

        Task<UploadSettings> GetDocumentUploadSettings(int entityId, string entityType, CancellationToken cancellationToken = default);
        Task<CardPaymentReceipt> GetCPReciptData(string caseNo, string KID, CancellationToken cancellationToken = default);

        Task<string> GetDocumentPath(int CaseNo, string RefNo, string DocType, int ActivityCode, CancellationToken cancellationToken = default);
        // Task<List<EntityDocumentDetails>> GetEntityDocumentDetails(int entityId, string entityType, bool addedToHistory, CancellationToken cancellationToken = default);

        Task<int> UpdateCommunicationQueue(string entityId, string entityType, string templateName,
                                         string templateCategory, int pendingexecId, int isPreview, int activityId, string activity, int invGrpNo, string activityversion, CancellationToken cancellationToken = default);
        Task<int> AddExecutedActivity(int rowId, int queueId, string activitiyName, string routineName, int entityId,
                                 string errorText,
                                 string entityType, string user, string param, DateTime dueDate, int invGrpNo, string attachmentPath, string activityversion, bool isSuccess, CancellationToken cancellationToken = default);
        Task<bool> UpdateMainDocumentDate(int entityNo, string activityName, string workflowType, CancellationToken cancellation = default);

        Task<ActivityExecutionDetail> GetActivity(string activityCode, int entityId, string entityType, CancellationToken cancellation = default);

        Task<ActivityServiceSetting> GetActivityServiceSettings(string activityCode, string entityType, CancellationToken cancellation = default);
        Task<List<UploadDocumentResponse>> AddUploadedDocuments(FileUploadRequest fileUploadRequest, string storageType, CancellationToken cancellation = default);
        Task<List<string>> GetDocumentPathV4(int lookupId, string system, string lookupType, CancellationToken cancellation = default);
        Task<List<DebtorCase>> GetCasesByDebtors(List<int> EntRoleidList, int caseno, CancellationToken cancellation = default);
        Task<List<int>> GetDebtorsByDebtorDetails(string dateOfBirth, string phoneNumber, CancellationToken cancellation = default);

        Task <List<int>> GetDebtorsBySSN(string personNo, CancellationToken cancellation = default);
    }
}
