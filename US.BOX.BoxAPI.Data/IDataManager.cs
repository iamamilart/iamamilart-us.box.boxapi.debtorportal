
using US.BOX.BoxAPI.Data.Models;

namespace US.BOX.BoxAPI.Data
{
    public interface IDataManager
    {
        Task<int> CheckPortalUser(string username, string password , CancellationToken cancellationToken = default);
        Task<IDictionary<string, string>> GetDebtorPortalSettings(int caseNumber, CancellationToken cancellationToken = default);
        Task<PortalCase> GetPortalCase(int caseNumber, CancellationToken cancellationToken = default);
        Task<List<PartPayment>> GetPartPayment(int caseNumber, CancellationToken cancellationToken = default);
        Task<List<Timeline>> GetTimelinedata(int caseNumber, CancellationToken cancellationToken = default);

        

    }
}
