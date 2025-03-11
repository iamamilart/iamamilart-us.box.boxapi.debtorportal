using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Core.Interfaces;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Enums;
using US.BOX.DebtorPortalAPI.Controllers.v1;
using US.BOX.DebtorPortalAPI.Utils;

namespace US.BOX.DebtorPortalAPI.Tests.Controllers.v1
{
    [TestClass]
    public class DebtorPortalControllerTests
    {
        private readonly Mock<ILogger<DebtorPortalController>> _loggerMock;
        private readonly Mock<IDebtorPortalAPIService> _apiServiceMock;
        private DebtorPortalController? _controller;
        private const int TEST_CASE_NUMBER = 12345;

        public DebtorPortalControllerTests()
        {
            _loggerMock = new Mock<ILogger<DebtorPortalController>>();
            _apiServiceMock = new Mock<IDebtorPortalAPIService>();
        }

        [TestInitialize]
        public void Setup()
        {
            _controller = new DebtorPortalController(_loggerMock.Object, _apiServiceMock.Object);
            SetupUserClaims();
        }

        private void SetupUserClaims()
        {
            var claims = new List<Claim>
            {
                new Claim("username", TEST_CASE_NUMBER.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _controller!.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [TestMethod]
        public async Task GetCaseStatus_ValidInput_ReturnsOkResult()
        {
            // Arrange
            string expectedStatus = "Active";
            _apiServiceMock.Setup(x => x.GetCaseStatus(It.Is<int>(c => c == TEST_CASE_NUMBER), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(expectedStatus));

            // Act
            var result = await _controller!.GetCaseStatus(TEST_CASE_NUMBER, "TEST");

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedStatus, okResult.Value);
        }

        [TestMethod]
        public async Task UpdateContact_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var contact = new Contact 
            { 
                Email = "test@example.com",
                Mobile = "1234567890",
                Address = "Test Address"
            };
            
            _apiServiceMock.Setup(x => x.UpdateContact(It.Is<int>(c => c == TEST_CASE_NUMBER), It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(true));

            // Act
            var result = await _controller!.UpdateContact(contact);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsTrue(okResult.Value is bool value && value);
        }

        [TestMethod]
        public async Task GetSystemCategories_ValidInput_ReturnsOkResult()
        {
            // Arrange
            string categoryType = "Test";
            int arNo = 123;
            var expectedCategories = new List<SystemCategory>
            {
                new SystemCategory { Id = 1, Value = "Category1", EnText = "Category 1", NoText = "Kategori 1" }
            };

            _apiServiceMock.Setup(x => x.GetSystemCategories(It.Is<string>(c => c == categoryType), It.Is<int>(a => a == arNo), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(expectedCategories));

            // Act
            var result = await _controller!.GetSystemCategories(categoryType, arNo);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedCategories, okResult.Value);
        }

        [TestMethod]
        public async Task PrivacyPolicyUpdate_ValidInput_ReturnsOkResult()
        {
            // Arrange
            _apiServiceMock.Setup(x => x.PrivacyPolicyUpdate(It.Is<int>(c => c == TEST_CASE_NUMBER), It.Is<bool>(s => s == true), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(true));

            // Act
            var result = await _controller!.PrivacyPolicyUpdate(TEST_CASE_NUMBER);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsTrue(okResult.Value is bool value && value);
        }

        [TestMethod]
        public async Task OpenIdConfigurations_ValidInput_ReturnsOkResult()
        {
            // Arrange
            string companyCode = "TEST";
            string expectedConfig = "test-config";

            _apiServiceMock.Setup(x => x.openIdConfigurations(It.Is<int>(c => c == TEST_CASE_NUMBER), It.Is<string>(c => c == companyCode), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(expectedConfig));

            // Act
            var result = await _controller!.openIdConfigurations(companyCode);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedConfig, okResult.Value);
        }

        [TestMethod]
        public async Task BankIDProviders_ValidInput_ReturnsOkResult()
        {
            // Arrange
            string companyCode = "TEST";
            string expectedProviders = "test-providers";

            _apiServiceMock.Setup(x => x.bankIDProviders(It.Is<int>(c => c == TEST_CASE_NUMBER), It.Is<string>(c => c == companyCode), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(expectedProviders));

            // Act
            var result = await _controller!.bankIDProviders(companyCode);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedProviders, okResult.Value);
        }

        [TestMethod]
        public async Task ClaimOfPayment_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var claim = new ClaimOfPayment { Amount = 100 };

            _apiServiceMock.Setup(x => x.ClaimOfPayment(It.Is<int>(c => c == TEST_CASE_NUMBER), It.IsAny<ClaimOfPayment>(), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(true));

            // Act
            var result = await _controller!.ClaimOfPayment(claim);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsTrue(okResult.Value is bool value && value);
        }

        [TestMethod]
        public async Task PartPayment_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var request = new PartpaymentRequest 
            { 
                FirstInstallmentDate = DateTime.Now.AddDays(30),
                InstallmentCount = 3,
                Mobile = "1234567890",
                Email = "test@example.com",
                DebtorType = "Individual",
                MaxPossibleFee = 100,
                Confirmed = 1,
                CaseType = "Standard"
            };

            var expectedPayments = new List<PartPayment>
            {
                new PartPayment { InstallmentAmount = 100 }
            };

            _apiServiceMock.Setup(x => x.PartPayment(It.Is<int>(c => c == TEST_CASE_NUMBER), It.IsAny<PartpaymentRequest>(), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(expectedPayments));

            // Act
            var result = await _controller!.PartPayment(request);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            CollectionAssert.AreEqual(expectedPayments, okResult.Value as List<PartPayment>);
        }

        [TestMethod]
        public async Task BankIDLogin_ValidInput_ReturnsOkResult()
        {
            // Arrange
            string authCode = "test-auth-code";
            var expectedCases = new List<DebtorCase>
            {
                new DebtorCase { CaseNumber = TEST_CASE_NUMBER }
            };

            _apiServiceMock.Setup(x => x.BankIDLogin(It.Is<int>(c => c == TEST_CASE_NUMBER), It.Is<string>(a => a == authCode), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(expectedCases));

            // Act
            var result = await _controller!.BankIDLogin(authCode);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            CollectionAssert.AreEqual(expectedCases, okResult.Value as List<DebtorCase>);
        }

        [TestMethod]
        public async Task RegisterPortalPayment_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var payment = new CardPayment { Amount = 100 };
            string expectedPaymentRef = "test-payment-ref";

            _apiServiceMock.Setup(x => x.RegisterPortalPayment(It.Is<int>(c => c == TEST_CASE_NUMBER), It.IsAny<CardPayment>(), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(expectedPaymentRef));

            // Act
            var result = await _controller!.RegisterPortalPayment(payment);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedPaymentRef, okResult.Value);
        }

        [TestMethod]
        public async Task AuthorizePayment_ValidInput_ReturnsOkResult()
        {
            // Arrange
            string transRef = "test-trans-ref";

            _apiServiceMock.Setup(x => x.AuthorizePayment(It.Is<int>(c => c == TEST_CASE_NUMBER), It.Is<string>(t => t == transRef), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(true));

            // Act
            var result = await _controller!.AuthorizePayment(transRef);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsTrue(okResult.Value is bool value && value);
        }

        [TestMethod]
        public async Task CreateCaseNote_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var note = new DPCaseNote 
            { 
                ReferenceNumber = TEST_CASE_NUMBER,
                Type = "Case",
                Lan = Language.EN,
                NoteText = "Test note"
            };
            int expectedNoteId = 1;

            _apiServiceMock.Setup(x => x.CreateCaseNote(It.Is<int>(c => c == TEST_CASE_NUMBER), It.IsAny<DPCaseNote>(), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(expectedNoteId));

            // Act
            var result = await _controller!.CreateCaseNote(note);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedNoteId, okResult.Value);
        }

        [TestMethod]
        public async Task GetPortalDebtor_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var expectedContact = new Contact 
            { 
                Email = "test@example.com",
                Mobile = "1234567890"
            };

            _apiServiceMock.Setup(x => x.GetPortalDebtor(It.Is<int>(c => c == TEST_CASE_NUMBER), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(expectedContact));

            // Act
            var result = await _controller!.GetPortalDebtor();

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedContact, okResult.Value);
        }

        [TestMethod]
        public async Task GenerateGDPRDocument_ValidInput_ReturnsOkResult()
        {
            // Arrange
            int arNo = 123;
            string caseType = "test-type";
            string expectedDocument = "test-document";

            _apiServiceMock.Setup(x => x.GenerateGDPRDocument(It.Is<int>(c => c == TEST_CASE_NUMBER), It.Is<int>(a => a == arNo), It.Is<string>(t => t == caseType), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(expectedDocument));

            // Act
            var result = await _controller!.GenerateGDPRDocument(arNo, TEST_CASE_NUMBER, caseType);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedDocument, okResult.Value);
        }

        [TestMethod]
        public async Task GetDocument_ValidInput_ReturnsOkResult()
        {
            // Arrange
            string refNumber = "ref-123";
            string docType = "test-doc";
            string caseType = "test-case";
            int integration = 1;
            var expectedDocument = new byte[] { 1, 2, 3 };

            _apiServiceMock.Setup(x => x.GetDocument(It.Is<int>(c => c == TEST_CASE_NUMBER), It.Is<string>(r => r == refNumber), 
                                                   It.Is<string>(d => d == docType), It.Is<string>(c => c == caseType), 
                                                   It.Is<int>(i => i == integration), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(expectedDocument));

            // Act
            var result = await _controller!.GetDocument(refNumber, docType, caseType, integration);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            CollectionAssert.AreEqual(expectedDocument, okResult.Value as byte[]);
        }
    }
}
