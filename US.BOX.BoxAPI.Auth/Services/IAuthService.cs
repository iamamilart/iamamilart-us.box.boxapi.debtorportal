using US.BOX.BoxAPI.Auth.Model;

namespace US.BOX.DebtorPortalAPI.Utils
{
    public interface IAuthService 
    {
       Task<AuthenticateResponse> Authenticate(string username , string password   , string company, CancellationToken cancellationToken = default);
    }
}
