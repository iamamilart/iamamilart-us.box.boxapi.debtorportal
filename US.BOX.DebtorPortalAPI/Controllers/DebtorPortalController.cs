using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using US.BOX.BoxAPI.Core.Interfaces;
using US.BOX.DebtorPortalAPI.Utils;
using US.BOX.BoxAPI.Data.Models;

namespace US.BOX.DebtorPortalAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/api/debtorportal")]
    [Authorize]
    public class DebtorPortalController : ControllerBase
    {
        private readonly ILogger<DebtorPortalController> _logger;
        private readonly IDebtorPortalAPIService _apiService;

        /// <summary>
        /// Common APIs that are exposed across all versions should be implemented in a shared controller. For any customizations, 
        /// add the attribute [MapToApiVersion("1.0")] to the method and implement the changes in the version 2 controller.
        ///</summary>
        /// <param name="logger"></param>
        /// <param name="apiService"></param>
        public DebtorPortalController(ILogger<DebtorPortalController> logger, IDebtorPortalAPIService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        /// <summary>
        /// Get case details for the current user
        /// </summary>
        /// <remarks>
        /// Retrieves the case details for the currently authenticated debtor user. 
        /// In the Debtor Portal, each user (with a username issued by UnicornBOX) is associated with a single case.
        /// 
        /// **Response Examples:**
        /// - **200 OK**: Returns the case details for the debtor.
        /// - **400 Bad Request**: The request is invalid or malformed.
        /// - **401 Unauthorized**: Authentication failed, or the user is not authorized.
        /// - **500 Internal Server Error**: An error occurred on the server while fetching the case details.
        /// </remarks>
        /// <response code="200">Returns the case details for the current user</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Authorization failure</response>
        /// <response code="500">Internal Server Error</response>
        [Route("cases")]
        [HttpGet]
        [ProducesResponseType(typeof(PortalCase), StatusCodes.Status200OK)]
        public async Task<IActionResult> Cases(CancellationToken cancellationToken)
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.GetPortalCase(caseNumber, cancellationToken);
            return Ok(result);
        }


        /// <summary>
        /// Fetch the debtor timeline data
        /// </summary>
        /// <returns>A list of timeline events related to the debtor case</returns>
        /// <remarks>
        /// This endpoint retrieves the debtor's timeline data based on the case number retrieved from the user token.
        /// 
        /// **Response Examples:**
        /// - **200 OK**: Returns a list of timeline events related to the debtor.
        /// - **400 Bad Request**: The request is malformed or missing required parameters.
        /// - **401 Unauthorized**: The request is missing a valid authentication token.
        /// - **500 Internal Server Error**: A server-side error occurred while processing the request.
        /// </remarks>
        /// <response code="200">Returns a list of timeline events</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Authorization failure</response>
        /// <response code="500">Internal Server Error</response>
        [Route("timeline")]
        [HttpGet]
        [ProducesResponseType(typeof(List<Timeline>), StatusCodes.Status200OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetTimeline()
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.GetTimelinedata(caseNumber);
            return Ok(result);
        }




    }
}
