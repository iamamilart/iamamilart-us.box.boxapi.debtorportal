using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using US.BOX.BoxAPI.Core.Interfaces;
using US.BOX.DebtorPortalAPI.Utils;
using US.BOX.BoxAPI.Data.Models;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Microsoft.VisualBasic;


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

        #region cases

        [Route("caseStatus/{caseNumber}")]
        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCaseStatus(int caseNumber, string companyCode)
        {
            //var caseNumber = User.GetCaseNumber();
            var result = await _apiService.GetCaseStatus(caseNumber);
            return Ok(result);
        }

        //[Route("cases")]
        //[HttpGet]
        //[ProducesResponseType(typeof(List<PortalCase>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetPortalCase()
        //{
        //    var caseNumber = User.GetCaseNumber();
        //    var result = await _apiService.GetPortalCase(caseNumber);
        //    return Ok(result);
        //}

        [Route("contact")]
        [HttpPut]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateContact([FromBody] Contact contact)
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.UpdateContact(caseNumber, contact);
            return Ok(result);
        }

        [Route("SystemCategories/{categoryType}/{arNo}")]
       // [SwaggerResponse(HttpStatusCode.OK, "Successfully found System Categories", typeof(List<SystemCategory>))]
        [HttpGet]
        [ProducesResponseType(typeof(List<SystemCategory>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSystemCategories(string categoryType, int arNo)
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.GetSystemCategories(categoryType, arNo);
            return Ok(result);
        }

        [Route("privacy")]
        [HttpPut]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> PrivacyPolicyUpdate(int caseNo)
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.PrivacyPolicyUpdate(caseNo, true);
            return Ok(result);
        }
        
        [Route("openIdConfigurations")]
        [HttpGet]
        //[SwaggerResponse(HttpStatusCode.OK, "openIdConfigurations", typeof(string))]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> openIdConfigurations(string companyCode)
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.openIdConfigurations(caseNumber,companyCode);
            return Ok(result);
        }

        [Route("bankIDProviders")]
        [HttpGet]
        // [SwaggerResponse(HttpStatusCode.OK, "bankIDProviders", typeof(string))]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> bankIDProviders(string companyCode)
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.bankIDProviders(caseNumber, companyCode);
            return Ok(result);
        }

        [Route("cases/claimofpayment")]
        [HttpPut]
        [ProducesResponseType(typeof(List<Timeline>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ClaimOfPayment([FromBody] ClaimOfPayment claim)
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.ClaimOfPayment(caseNumber, claim);
            return Ok(result);
        }

        [Route("cases/UploadClaimOfPaymentReceipts")]
        [HttpPut]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadClaimOfPaymentReceipts(List<IFormFile> files)
        {
            var caseNumber = User.GetCaseNumber();

            var result = await _apiService.UploadClaimOfPaymentReceipts(caseNumber, files);
            return Ok(result);
        }

        [Route("invoices/send")]
        [HttpPut]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendInvoiceCopy([FromBody] InvoiceCopy invoiceDetails)
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.SendInvoiceCopy(caseNumber, invoiceDetails);
            return Ok(result);
        }


        [Route("cprecipt")]
        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCPReciptPdf(string caseNo,string KID,string companyCode)
        {
            var caseNumber = User.GetCaseNumber();

            var result = await _apiService.GetCPReciptPdf(caseNo, KID,companyCode);
            return Ok(result);
        }

        [Route("messageDocument")]
        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReadMessageDocument(string documentID)
        {
            var caseNumber = User.GetCaseNumber();

            var fileBytes = await _apiService.ReadMessageDocument(caseNumber,documentID);
            string contentType = "application/octet-stream"; // Generic MIME type
            string fileName = "";
            string fileExtension = Path.GetExtension(fileName.ToString()).ToLower();

            // Set specific content types based on file extension
            if (fileExtension == ".pdf") contentType = "application/pdf";
            if (fileExtension == ".jpg" || fileExtension == ".jpeg") contentType = "image/jpeg";
            if (fileExtension == ".png") contentType = "image/png";
            if (fileExtension == ".txt") contentType = "text/plain";
            if (fileExtension == ".docx") contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            return File(fileBytes, contentType, fileName);
            
        }

        [Route("VippsBankIDLogin")]
        [HttpGet]
        // [SwaggerResponse(HttpStatusCode.OK, "bankIDProviders", typeof(string))]
        [ProducesResponseType(typeof(List<DebtorCase>), StatusCodes.Status200OK)]
        public async Task<IActionResult> VippsBankIDLogin(string authCode, int caseNumber, string openIDConfiguration, string companyCode)
        {
            var caseNo = User.GetCaseNumber();
            var result = await _apiService.VippsBankIDLogin(authCode, caseNumber, openIDConfiguration, companyCode);
            return Ok(result);
        }

        //[Authorize(Roles = "debtor")]
        //[Route("objection/respond")]
        //[HttpPost]
        ////[SwaggerResponse(HttpStatusCode.OK, "Created Case Note Id", typeof(bool))]
        //[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        //public async Task<IActionResult> ObjectionRespond(DPObjectionRespond respond)
        //{
        //    var caseNumber = User.GetCaseNumber();
        //    var result = await _apiService.ObjectionRespond(caseNumber, respond);
        //    return Ok(result);
        //}

        [Authorize(Roles = "debtor")]
        [Route("cases/objection")]
        [HttpPut]
        //[SwaggerResponse(HttpStatusCode.OK, "Created Case Note Id", typeof(bool))]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> Objection([FromBody] PortalObjection objection)
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.Objection(caseNumber, objection);
            return Ok(result);
        }

        [Route("partpayments")]
        [HttpPost]
        // [SwaggerResponse(HttpStatusCode.OK, "bankIDProviders", typeof(string))]
        [ProducesResponseType(typeof(List<PartPayment>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PartPayment([FromBody] PartpaymentRequest partpaymentRequest)
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.PartPayment(caseNumber, partpaymentRequest);
            return Ok(result);
        }

        [Route("bankid")]
        [HttpGet]
        // [SwaggerResponse(HttpStatusCode.OK, "bankIDProviders", typeof(string))]
        [ProducesResponseType(typeof(List<DebtorCase>), StatusCodes.Status200OK)]
        public async Task<IActionResult> BankIDLogin(string authCode)
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.BankIDLogin( caseNumber, authCode);
            return Ok(result);
        }

        [Route("payments")]
        [HttpPost]
        // [SwaggerResponse(HttpStatusCode.OK, "Register", typeof(string))]
        [ProducesResponseType(typeof(List<DebtorCase>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterPortalPayment([FromBody] CardPayment paymentObj)
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.RegisterPortalPayment(caseNumber, paymentObj);
            return Ok(result);
        }

        [Route("payments/{transRef?}/autharize")]
        [HttpPut]
        // [SwaggerResponse(HttpStatusCode.OK, "Register", typeof(string))]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> AuthorizePayment(string transRef)
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.AuthorizePayment(caseNumber, transRef);
            return Ok(result);
        }       

        [Route("inquiry")]
        [HttpPost]
        // [SwaggerResponse(HttpStatusCode.OK, "Register", typeof(string))]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateCaseNote(DPCaseNote request)
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.CreateCaseNote(caseNumber, request);
            return Ok(result);
        }

        [Route("gdprdoc/{arNo}/{caseNumber}/{caseType}")]
        [HttpGet]
        // [SwaggerResponse(HttpStatusCode.OK, "Register", typeof(string))]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateGDPRDocument(int arNo, int caseNumber, string caseType)
        {
            var caseNo = User.GetCaseNumber();
            var result = await _apiService.GenerateGDPRDocument(caseNumber, arNo,caseType);
            return Ok(result);
        }

        [Route("invoices/{refNumber?}/{docType?}/{caseType?}/{integration?}")]
        [HttpPut]
        // [SwaggerResponse(HttpStatusCode.OK, "Register", typeof(string))]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDocument(string refNumber, string docType, string caseType, int integration)
        {
            var caseNo = User.GetCaseNumber();
            var result = await _apiService.GetDocument(caseNo, refNumber, docType, caseType, integration);
            return Ok(result);
        }

        //[Route("cases/postpone")]
        //[HttpPut]
        //// [SwaggerResponse(HttpStatusCode.OK, "Register", typeof(string))]
        //[ProducesResponseType(typeof(DateTime), StatusCodes.Status200OK)]
        //public async Task<IActionResult> PostponeCase([FromBody] Contact contact)
        //{
        //    var caseNumber = User.GetCaseNumber();
        //    var result = await _apiService.PostponeCase(caseNumber, contact);
        //    return Ok(result);
        //}

        [Route("debtor")]
        [HttpPut]
        // [SwaggerResponse(HttpStatusCode.OK, "Register", typeof(string))]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPortalDebtor()
        {
            var caseNumber = User.GetCaseNumber();
            var result = await _apiService.GetPortalDebtor(caseNumber);
            return Ok(result);
        }

        

        #endregion

    }
}
