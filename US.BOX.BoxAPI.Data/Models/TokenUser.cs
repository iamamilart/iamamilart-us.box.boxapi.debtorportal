using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public sealed class TokenUser
    {
        public int ID { get; set; }

        public Guid TenantId { get; set; }

        public Guid AzureUserId { get; set; }

        public string TokenAudience { get; set; }

        public string TokenIssuer { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public bool MaskPrivateData { get; set; }

        public bool HasDebtCollectionLicense { get; set; }

        public bool ActiveState { get; set; }

        public string TenantServer { get; set; }

        public string TenantDatabase { get; set; }

        public string TenantCommonLogin { get; set; }

        public string TenantCommonPassword { get; set; }

        public string TenantUserLogin { get; set; }

        public string TenantUserPassword { get; set; }

        public string UserDBConnectionString { get; set; }

        public string UserBlobConnectionString { get; set; }

        public bool HasAccessToSubOperation { get; set; }

        public string Token { get; set; }

        public TokenType LoginTokenType { get; set; }

        public Guid ClientId { get; set; }

        public string ClientSecret { get; set; }

        public bool IsOnPremiseDB { get; set; }

        public string ManagedIdentityClientId { get; set; }

        public int AccessRank { get; set; }

        public string TokenVersion { get; set; }
    }

    public enum TokenType
    {
        B2C,
        AD
    }
}
