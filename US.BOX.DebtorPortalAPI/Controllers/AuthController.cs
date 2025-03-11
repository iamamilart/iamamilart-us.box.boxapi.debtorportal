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
            //using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);

            //var body = await reader.ReadToEndAsync();
            //// Reset the request body stream position
            //Request.Body.Position = 0;

            //// Parse the form data
            //var formData = System.Web.HttpUtility.ParseQueryString(body);
            //var username = formData["username"];
            //var password = formData["password"];
            //var company = formData["company"];

            //ArgumentNullException.ThrowIfNull(username);
            var username = Request.Form["username"];
            var password = Request.Form["password"];
            var company = Request.Form["company"];
            ArgumentNullException.ThrowIfNull(username);
            ArgumentNullException.ThrowIfNull(password);

                var response = await _authService.Authenticate(username , password  , company , cancellationToken);

                return Ok(response);
        }

 
    }
}
