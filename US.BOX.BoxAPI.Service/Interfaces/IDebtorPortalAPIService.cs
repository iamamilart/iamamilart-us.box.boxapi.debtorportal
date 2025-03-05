
using US.BOX.BoxAPI.Data.Models;

namespace US.BOX.BoxAPI.Core.Interfaces
{
    public interface IDebtorPortalAPIService
    {
        Task<IDictionary<string, string>> GetDebtorPortalSettings(int caseNumber = 0, CancellationToken cancellationToken = default);
        Task<PortalCase> GetPortalCase(int caseNumber , CancellationToken cancellationToken = default);
        Task<List<Timeline>> GetTimelinedata(int caseNumber, CancellationToken cancellationToken = default);

    }
}
