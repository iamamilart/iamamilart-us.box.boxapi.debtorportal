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

        public  async Task<PortalCase> GetPortalCase(int caseNumber, CancellationToken cancellationToken = default)
        {
            var portalcase = await dataManager.GetPortalCase(caseNumber, cancellationToken);
            var settings = await GetDebtorPortalSettings(0,cancellationToken);
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
    }
}
