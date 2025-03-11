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
using US.BOX.DebtorPortalAPI.Controllers;
using US.BOX.DebtorPortalAPI.Utils;

namespace US.BOX.DebtorPortalAPI.Tests.Controllers
{
    [TestClass]
    public class DebtorPortalControllerTests
    {
        private readonly Mock<ILogger<DebtorPortalController>> _loggerMock = new();
        private readonly Mock<IDebtorPortalAPIService> _apiServiceMock = new();
        private DebtorPortalController? _controller;
        private const int TEST_CASE_NUMBER = 12345;

        [TestInitialize]
        public void Setup()
        {
            _controller = new DebtorPortalController(_loggerMock.Object, _apiServiceMock.Object);

            // Setup mock user claims
            var claims = new List<Claim>
            {
                new Claim("username", TEST_CASE_NUMBER.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [TestMethod]
        public async Task Cases_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var expectedCase = new PortalCase
            {
                BOXCaseNumber = TEST_CASE_NUMBER,
                DebtorName = "Test Debtor",
                CaseType = "Standard",
                Balance = 1000,
                Status = 1 // Active
            };

            _apiServiceMock.Setup(x => x.GetPortalCase(It.Is<int>(c => c == TEST_CASE_NUMBER), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(expectedCase));

            // Act
            var result = await _controller!.Cases(CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedCase, okResult.Value);
        }

        [TestMethod]
        public async Task Cases_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _apiServiceMock.Setup(x => x.GetPortalCase(It.Is<int>(c => c == TEST_CASE_NUMBER), It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller!.Cases(CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            var statusCodeResult = result as StatusCodeResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [TestMethod]
        public async Task GetTimeline_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var expectedTimeline = new List<Timeline>
            {
                new Timeline
                {
                    TimeLineDate = DateTime.Now,
                    Balance = 1000,
                    WorkflowState = "Payment Received"
                }
            };

            _apiServiceMock.Setup(x => x.GetTimelinedata(It.Is<int>(c => c == TEST_CASE_NUMBER), It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(expectedTimeline));

            // Act
            var result = await _controller!.GetTimeline();

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            CollectionAssert.AreEqual(expectedTimeline, okResult.Value as List<Timeline>);
        }

        [TestMethod]
        public async Task GetTimeline_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _apiServiceMock.Setup(x => x.GetTimelinedata(It.Is<int>(c => c == TEST_CASE_NUMBER), It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller!.GetTimeline();

            // Assert
            Assert.IsNotNull(result);
            var statusCodeResult = result as StatusCodeResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }
    }
}
