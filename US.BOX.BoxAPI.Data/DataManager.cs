using Azure.Core;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Threading;
using System.Transactions;
using US.BOX.BoxAPI.Data.Actions;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;
using Transaction = US.BOX.BoxAPI.Data.Models.Transaction;

namespace US.BOX.BoxAPI.Data
{
    public class DataManager : IDataManager
    {
        private  string _connectionString;
        private  string _CatalogDBconnectionString;

        // private readonly Dictionary<string,string> _activityExecutionAPI= new Dictionary<string, string>();
        public DataManager(IConfiguration configuration)
        {
           // _connectionString=DPAPIConfig.ConnectionString;
            // _CatalogDBconnectionString = configuration.GetConnectionString("Common:WorkstationDbConnection");
            _CatalogDBconnectionString = DPAPIConfig.CatalogDBConnectionString;

        }

        public async Task<TenentDetail> GetTenantDBCommonConnection(string TenantId, CancellationToken cancellationToken = default)
        {
            using var executor = new GetTenantDBCommonConnectionAction(TenantId);
            TenentDetail tenent = new TenentDetail();
            tenent=await executor.ExecuteAsync(_CatalogDBconnectionString, cancellationToken);
            //Data Source=sqldbsrv-unicornbox-dev.database.windows.net;Initial Catalog=sqldb-unicornbox-tenant3-dev;User ID=box4devsqluser; Password=Pl9960HTdnym41Ka;
            _connectionString = "Data Source="+ tenent.TenentServer + ";Initial Catalog="+ tenent.TenentDatabase + ";User ID="+ tenent.TenentCommonLogin + "; Password="+ tenent.TenentCommonPassword+";";
            DPAPIConfig.ConnectionString= _connectionString;
            DPAPIConfig.AzureBlobStorageConnectionString=tenent.BlobConnection;
            return tenent;
                

        }

        public async Task<int> CheckPortalUser( string username, string password, CancellationToken cancellationToken = default)
        {
            using var executor = new CheckPortalUserAction( username,  password);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<IDictionary<string, string>> GetDebtorPortalSettings(int caseNumber, CancellationToken cancellationToken = default)
        {
            using var executor = new GetDebtorPortalSettingAction(caseNumber);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }


        public async Task<PortalCase> GetPortalCase(int caseNumber , CancellationToken cancellationToken = default)
        {
            using var executor = new GetPortalCaseAction(caseNumber);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<List<PartPayment>> GetPartPayment(int caseNumber, CancellationToken cancellationToken = default)
        {
            using var executor = new GetPartPaymentAction(caseNumber);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<List<Timeline>> GetTimelinedata(int caseNumber, CancellationToken cancellationToken = default)
        {
            using var executor = new GetTimelinedataAction(caseNumber);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<string> GetCaseStatus(int caseNumber, CancellationToken cancellationToken = default)
        {
            using var executor = new GetCaseStatusAction(caseNumber);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<bool> UpdateContact(int caseNo, Contact contact, CancellationToken cancellationToken = default)
        {
            using var executor = new UpdateContactAction(caseNo, contact);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<List<SystemCategory>> GetSystemCategories(string category, int arNo, CancellationToken cancellationToken = default)
        {
            using var executor = new GetSystemCategoriesAction(category, arNo);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<bool> PrivacyPolicyUpdate(int caseNo, bool state, CancellationToken cancellationToken = default)
        {
            using var executor = new PrivacyPolicyUpdateAction(caseNo, state);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<KeyValuePair<string, string>> GetDocumentPathByDocId(string documentId, CancellationToken cancellationToken = default)
        {
            using var executor = new GetDocumentPathByDocIdAction(documentId);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }


        public async Task<CalculatePartPayment> CalculatePartPayment(DataPartPaymentRequest _partPayment,  CancellationToken cancellationToken = default)
        {
                using var PPexecutor = new CalculatePartPaymentAction(_partPayment);
                return await PPexecutor.ExecuteAsync(_connectionString, cancellationToken);  

        }

        public async Task<int> RegisterPaymentAgreement(USPPaymentAgreement _paymentAgreement, CancellationToken cancellationToken = default)
        {
            using var Agreemntexecutor = new RegisterPaymentAgreementAction(_paymentAgreement);
            return await Agreemntexecutor.ExecuteAsync(_connectionString, cancellationToken);

        }

        public async Task<int> CreateDPortalCaseNote(DPCaseNote _request, CancellationToken cancellationToken = default)
        {
            using var executor = new CreateDPortalCaseNoteAction(_request);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<List<DebtorCase>> GetPortalCasesByDebtor(string ssno, int caseNumber, CancellationToken cancellationToken = default)
        {
            using var executor = new GetPortalCasesByDebtorAction(ssno,caseNumber);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<string> GetNewPaymentKid(int caseno, CancellationToken cancellationToken = default)
        {
            using var executor = new GetNewPaymentKidAction(caseno);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<bool> AddDPCardPayment(int caseno, string kid, string paymentRef, decimal amount, CancellationToken cancellationToken = default)
        {
            using var executor = new AddDPCardPaymentAction(caseno,kid,paymentRef,amount);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<bool> AddDPCardPaymentDetails(int status, ProcessResponse obj, CancellationToken cancellationToken = default)
        {
            using var executor = new AddDPCardPaymentDetailsAction(status,obj);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<Transaction> GetCardPaymentData(String transID, CancellationToken cancellationToken = default)
        {
            using var executor = new GetCardPaymentDataAction(transID);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<string> GetCaseMetaData(int caseNo, CancellationToken cancellationToken = default)
        {
            using var executor = new GetCaseMetaDataAction(caseNo);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<List<ObjectionProfile>> GetObjectionProfile(int caseNo, CancellationToken cancellationToken = default)
        {
            using var executor = new GetObjectionProfileAction(caseNo);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<List<SubCase>> GetSubcasesByCaseNo(int CaseNo, CancellationToken cancellationToken = default)
        {
            using var executor = new GetSubcasesByCaseNoAction(CaseNo);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<int> RegisterDebtorObjection(DebtorObjection _debtorObjection, CancellationToken cancellationToken = default)
        {
            using var executor = new RegisterDebtorObjectionAction(_debtorObjection);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<int> RegisterClaimOfPayment(DataClaimOfPayment _claimOfPayment, CancellationToken cancellationToken = default)
        {
            using var executor = new RegisterClaimOfPaymentAction(_claimOfPayment);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<UploadSettings> GetDocumentUploadSettings(int entityId, string entityType, CancellationToken cancellationToken = default)
        {
            using var executor = new GetDocumentUploadSettingsAction(entityId,entityType);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<CardPaymentReceipt> GetCPReciptData(string caseNo, string KID, CancellationToken cancellationToken = default)
        {
            using var executor = new GetCPReciptDataAction(caseNo,KID);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<string> GetDocumentPath(int CaseNo, string RefNo, string DocType, int ActivityCode, CancellationToken cancellationToken = default)
        {
            using var executor = new GetDocumentPathAction(CaseNo,RefNo,DocType,ActivityCode);
            return await  executor.ExecuteAsync(_connectionString, cancellationToken);          
        }

        public async Task<int> UpdateCommunicationQueue(string entityId, string entityType, string templateName,
                                        string templateCategory, int pendingexecId, int isPreview, int activityId, string activity, int invGrpNo, string activityversion, CancellationToken cancellationToken = default)
        {
            using var executor = new UpdateCommunicationQueueAction( entityId,  entityType,  templateName,
                                         templateCategory,  pendingexecId,  isPreview,  activityId,  activity,  invGrpNo,  activityversion);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<int> AddExecutedActivity(int rowId, int queueId, string activitiyName, string routineName, int entityId,
                                 string errorText,
                                 string entityType, string user, string param, DateTime dueDate, int invGrpNo, string attachmentPath, string activityversion, bool isSuccess, CancellationToken cancellationToken = default)
        {
            using var executor = new AddExecutedActivityAction(rowId, queueId, activitiyName, routineName, entityId,
                                 errorText,
                                 entityType, user, param, dueDate, invGrpNo, attachmentPath, activityversion, isSuccess);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }
        //Task<List<EntityDocumentDetails>> GetEntityDocumentDetails(int entityId, string entityType, bool addedToHistory, CancellationToken cancellationToken = default)
        //{
        //    using var executor = new GetEntityDocumentDetailsAction(entityId, entityType,addedToHistory);
        //    return await executor.ExecuteAsync(_connectionString, cancellationToken);
        //}
        public async Task<bool> UpdateMainDocumentDate(int entityNo, string activityName, string workflowType, CancellationToken cancellation = default)
        {
            using var executor = new UpdateMainDocumentDateAction(entityNo, activityName, workflowType);
            return await executor.ExecuteAsync(_connectionString, cancellation);
        }

        public async Task<ActivityExecutionDetail> GetActivity(string activityCode, int entityId, string entityType, CancellationToken cancellation = default)
        {
            using var executor = new GetActivityAction(activityCode, entityId, entityType);
            return await executor.ExecuteAsync(_connectionString, cancellation);
        }

        public async Task<ActivityServiceSetting> GetActivityServiceSettings(string activityCode, string entityType, CancellationToken cancellation = default)
        {
            using var executor = new GetActivityServiceSettingsAction(activityCode, entityType);
            return await executor.ExecuteAsync(_connectionString, cancellation);
        }

        public async Task<List<UploadDocumentResponse>> AddUploadedDocuments(FileUploadRequest fileUploadRequest, string storageType, CancellationToken cancellation = default)
        {
            using var executor = new AddUploadedDocumentsAction(fileUploadRequest, storageType);
            return await executor.ExecuteAsync(_connectionString, cancellation);
        }

        public async Task<List<string>> GetDocumentPathV4(int lookupId, string system, string lookupType, CancellationToken cancellationToken = default)
        {
            using var executor = new GetDocumentPathV4Action(lookupId, system, lookupType);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<List<DebtorCase>> GetCasesByDebtors(List<int> EntRoleidList, int caseno, CancellationToken cancellationToken = default)
        {
            using var executor = new GetCasesByDebtorsAction(EntRoleidList, caseno);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<List<int>> GetDebtorsByDebtorDetails(string dateOfBirth, string phoneNumber, CancellationToken cancellationToken = default)
        {
            using var executor = new GetDebtorsByDebtorDetailsAction(dateOfBirth, phoneNumber);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

        public async Task<List<int>> GetDebtorsBySSN(string personNo, CancellationToken cancellationToken = default)
        {
            using var executor = new GetDebtorsBySSNAction(personNo);
            return await executor.ExecuteAsync(_connectionString, cancellationToken);
        }

    }
}
