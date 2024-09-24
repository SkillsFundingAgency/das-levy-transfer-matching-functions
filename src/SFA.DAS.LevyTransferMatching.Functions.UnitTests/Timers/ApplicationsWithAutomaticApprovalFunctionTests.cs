using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Timers;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.Timers;

[TestFixture]
public class AutomaticApplicationApprovalFunctionTests
{
    private AutomaticApplicationApprovalFunction _handler;
    private Mock<ILevyTransferMatchingApi> _api;
    private Mock<ILogger<AutomaticApplicationApprovalFunction>> _logger;
    private GetApplicationsForAutomaticApprovalResponse _apiResponse;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture();

        _api = new Mock<ILevyTransferMatchingApi>();
        _logger = new Mock<ILogger<AutomaticApplicationApprovalFunction>>();

        _apiResponse = fixture.Create<GetApplicationsForAutomaticApprovalResponse>();
        _api.Setup(x => x.GetApplicationsForAutomaticApproval(It.IsAny<int?>())).ReturnsAsync(_apiResponse);
        _handler = new AutomaticApplicationApprovalFunction(_api.Object);
    }

    [Test]
    public async Task Run_Approves_Each_Application_Ready_For_Automatic_Approval()
    {
        // Act
        await _handler.Run(default, _logger.Object);

        // Assert
        _api.Verify(x => x.ApproveApplication(It.IsAny<ApproveApplicationRequest>()), Times.Exactly(_apiResponse.Applications.Count()));
    }

    [Test]
    public async Task HttpTrigger_Should_Return_OkResult()
    {
        // Arrange
        var httpRequestMock = new Mock<HttpRequest>();

        // Act
        var result = await _handler.HttpAutomaticApplicationApprovalFunction(httpRequestMock.Object, _logger.Object);

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
        (result as OkObjectResult)?.Value.Should().Be("ApplicationsWithAutomaticApproval successfully ran");
    }
}