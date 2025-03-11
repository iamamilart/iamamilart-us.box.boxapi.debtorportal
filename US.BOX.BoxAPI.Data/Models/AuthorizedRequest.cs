using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public sealed class AuthorizedRequest
    {
        public ClaimsPrincipal Principal { get; private set; }

        public AuthorizationStatus Status { get; private set; }

        public Exception Exception { get; private set; }

        public TokenUser AuthorizedUser { get; set; }

        private AuthorizedRequest()
        {
        }

        public static AuthorizedRequest Success(ClaimsPrincipal principal)
        {
            return new AuthorizedRequest
            {
                Principal = principal,
                Status = AuthorizationStatus.Valid
            };
        }

        public static AuthorizedRequest Success(TokenUser tokenUser)
        {
            return new AuthorizedRequest
            {
                AuthorizedUser = tokenUser,
                Status = AuthorizationStatus.Valid
            };
        }

        public static AuthorizedRequest Expired()
        {
            return new AuthorizedRequest
            {
                Status = AuthorizationStatus.Expired
            };
        }

        public static AuthorizedRequest Error(Exception ex)
        {
            return new AuthorizedRequest
            {
                Status = AuthorizationStatus.Error,
                Exception = ex
            };
        }

        public static AuthorizedRequest NoToken()
        {
            return new AuthorizedRequest
            {
                Status = AuthorizationStatus.NoToken
            };
        }

        public static AuthorizedRequest NoTenantId()
        {
            return new AuthorizedRequest
            {
                Status = AuthorizationStatus.NoTenantId
            };
        }

        public static AuthorizedRequest Unauthorized()
        {
            return new AuthorizedRequest
            {
                Status = AuthorizationStatus.Unauthorized
            };
        }

        
    }

    public enum AuthorizationStatus
    {
        Valid,
        Expired,
        Error,
        NoToken,
        Unauthorized,
        NoTenantId
    }
}
