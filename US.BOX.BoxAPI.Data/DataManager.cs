using Microsoft.Extensions.Configuration;
using System.Threading;
using US.BOX.BoxAPI.Data.Actions;
using US.BOX.BoxAPI.Data.Models;

namespace US.BOX.BoxAPI.Data
{
    public class DataManager : IDataManager
    {
        private readonly string _connectionString;
        public DataManager(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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
    }
}
