using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using US.BOX.BoxAPI.Auth.Utils;
using US.BOX.BoxAPI.Core.Interfaces;
using US.BOX.BoxAPI.Data;
using US.BOX.BoxAPI.Data.Models;

namespace US.BOX.BoxAPI.Core.Services
{
    public class DebtorPortalAPIService(IDataManager dataManager) : IDebtorPortalAPIService
    {
        public async Task<IDictionary<string, string>> GetDebtorPortalSettings(int caseNumber = 0, CancellationToken cancellationToken = default)
        {
           return await dataManager.GetDebtorPortalSettings(caseNumber, cancellationToken);
        }

        public async Task<PortalCase> GetPortalCase(int caseNumber, CancellationToken cancellationToken = default)
        {
            var portalCase = await dataManager.GetPortalCase(caseNumber, cancellationToken);
            var settings = await GetDebtorPortalSettings(0, cancellationToken);

            if (portalCase.IsPartPayment)
            {
                await ProcessPartPaymentAsync(portalCase, cancellationToken);
            }
            else
            {
                InitializeNonPartPaymentCase(portalCase);
            }

            SecureDebtorInfo(portalCase);
            ApplySignatures(portalCase, settings);
            portalCase.MinInstallmentAmount = Convert.ToDouble(settings["MinInstallmentAmount"]);

            return portalCase;
        }

        private async Task ProcessPartPaymentAsync(PortalCase portalCase, CancellationToken cancellationToken)
        {
            var payments = await dataManager.GetPartPayment(portalCase.BOXCaseNumber, cancellationToken);
            portalCase.PartPayments = payments;
            portalCase.DueBalance = portalCase.Balance; // Installment payment is not available yet

            var nextDueInstallment = payments.FirstOrDefault(p => !p.Paid);
            portalCase.NextDueAmount = nextDueInstallment?.InstallmentAmount ?? 0;
        }

        private void InitializeNonPartPaymentCase(PortalCase portalCase)
        {
            portalCase.PartPayments = new List<PartPayment>();
            portalCase.DueBalance = portalCase.Balance;
            portalCase.IsPartPaymentBreach = false;
        }

        private void SecureDebtorInfo(PortalCase portalCase)
        {
            portalCase.DebEmail = string.IsNullOrEmpty(portalCase.DebEmail) ? "" : AuthCommon.ProtectData(portalCase.DebEmail);
            portalCase.DebMobile = string.IsNullOrEmpty(portalCase.DebMobile) ? "" : AuthCommon.ProtectData(portalCase.DebMobile);
        }

        private void ApplySignatures(PortalCase portalCase, IDictionary<string, string> settings)
        {
            portalCase.Signature = Common.GetDebtorSignature(portalCase.DueBalance, settings["SignatureKey"]);
            portalCase.PartPaymentSignature = Common.GetDebtorSignature(portalCase.NextDueAmount, settings["SignatureKey"]);
        }


        public async Task<List<Timeline>> GetTimelinedata(int caseNumber, CancellationToken cancellationToken = default)
        {
            return await dataManager.GetTimelinedata(caseNumber, cancellationToken);
        }
    }
}
