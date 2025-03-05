using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using US.BOX.BoxAPI.Auth.Model;
using US.BOX.BoxAPI.Data;

namespace US.BOX.DebtorPortalAPI.Utils
{
    public partial class AuthService : IAuthService
    {
        private readonly AuthSettings _authSettings;
        private readonly IDataManager _dataManager;

        public AuthService(IOptions<AuthSettings> authSettings , IDataManager dataManager )
        {
            _authSettings = authSettings.Value;
            _dataManager = dataManager;
        }

        public async Task<AuthenticateResponse> Authenticate(string username, string password, string company, CancellationToken cancellationToken = default)
        {
            int status = 0;
            AuthenticateResponse response = new AuthenticateResponse();

            if (NumericOnlyRegex().IsMatch(username))
            {
                status = await _dataManager.CheckPortalUser(username, password, cancellationToken);
            }
            else {
                throw new SecurityTokenException("Provided username and password is incorrect");
            }

            if (status > 0)
            {
                response.userName = status.ToString();
                response.roles = response.userName == "admin" ? "debtoradmin" : "debtor";

            }
            else if (status == -1)
            {
                throw new SecurityTokenException("Failed login for expired URL.");
            }
            else if (status == -2)
            {
                throw new SecurityTokenException("Failed login for exceeded attempts.");
            }
            else
            {
                throw new SecurityTokenException("Failed login for exceeded attempts.");

            }


            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authSettings.Secret);
            var tokenExpiry = DateTime.UtcNow.AddDays(1);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserName",  status.ToString()),         // Add UserName claim
                    new Claim("ExCaseNo",  username),         // Add ExCaseNo claim
                    new Claim("CompanyCode", company),         // Add CompanyCode claim
                    new Claim(ClaimTypes.Role, response.roles),         // Add username claim

                    new Claim(JwtRegisteredClaimNames.Aud, _authSettings.ValidAudience), // Set the audience claim
                    new Claim(JwtRegisteredClaimNames.Iss, _authSettings.ValidIssuer) // Set the audience claim

                }),
                Expires = tokenExpiry,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            response.expires_in = (int)(tokenExpiry - DateTime.UtcNow).TotalSeconds;

            var token = tokenHandler.CreateToken(tokenDescriptor);
            response.access_token = tokenHandler.WriteToken(token);
            return response;

        }


        [GeneratedRegex(@"^\d+$", RegexOptions.None)]
        private static partial Regex NumericOnlyRegex();
    }
}
