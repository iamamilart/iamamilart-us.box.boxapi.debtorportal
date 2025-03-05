using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using US.BOX.BoxAPI.Core.Interfaces;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.DebtorPortalAPI.Utils;

namespace US.BOX.DebtorPortalAPI.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/api/debtorportal")]
    [Authorize]
    public class DebtorPortalController : ControllerBase
    {
        private readonly ILogger<DebtorPortalController> _logger;
        private readonly IDebtorPortalAPIService _apiService;

        /// <summary>
        /// Customized V2 APIs
        /// </summary>
        public DebtorPortalController(ILogger<DebtorPortalController> logger , IDebtorPortalAPIService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        [Route("timeline_v2")]
        [HttpGet]
        [ProducesResponseType(typeof(List<Timeline>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTimeline()
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.GetTimelinedata(caseNumber);
            return Ok(result);
        }

    }
}
