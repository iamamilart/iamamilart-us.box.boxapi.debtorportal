using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using US.BOX.DebtorPortalAPI.Controllers.v1;
using US.BOX.DebtorPortalAPI.Utils;

namespace US.BOX.DebtorPortalAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<DebtorPortalController> _logger;

        public AuthController(ILogger<DebtorPortalController> logger ,IAuthService AuthService)
        {
            _authService = AuthService;
            _logger = logger;
        }

        [HttpPost("token")]
      //  [MapToApiVersion("1.0")]
        public async Task<IActionResult> Authenticate(CancellationToken cancellationToken)
        {
                Request.EnableBuffering();

                var username = Request.Form["username"];
                var password = Request.Form["password"];
                var company = Request.Form["company"];


            if (username == "")
                return null; 
             

                var response = await _authService.Authenticate(username , password  , company , cancellationToken);

                return Ok(response);
        }

 
    }
}
