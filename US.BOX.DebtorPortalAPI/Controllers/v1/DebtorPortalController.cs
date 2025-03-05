using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using US.BOX.BoxAPI.Core.Interfaces;
using US.BOX.DebtorPortalAPI.Utils;

namespace US.BOX.DebtorPortalAPI.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/api/debtorportal")]
    [Authorize]
    public class DebtorPortalController : ControllerBase
    {
        private readonly ILogger<DebtorPortalController> _logger;
        private readonly IDebtorPortalAPIService _apiService;

        /// <summary>
        /// Customized V1 APIs
        /// </summary>
        public DebtorPortalController(ILogger<DebtorPortalController> logger , IDebtorPortalAPIService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }






    }
}
