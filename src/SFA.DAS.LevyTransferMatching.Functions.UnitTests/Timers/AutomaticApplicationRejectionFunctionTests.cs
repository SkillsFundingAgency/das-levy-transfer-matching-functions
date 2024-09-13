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
public class AutomaticApplicationRejectionFunctionTests
{
    private AutomaticApplicationRejectionFunction _handler;
    private Mock<ILevyTransferMatchingApi> _api;
    private Mock<ILogger> _logger;
    private GetApplicationsForAutomaticRejectionResponse _apiResponse;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture();

        _api = new Mock<ILevyTransferMatchingApi>();
        _logger = new Mock<ILogger>();

        _apiResponse = fixture.Create<GetApplicationsForAutomaticRejectionResponse>();

        _api.Setup(x => x.GetApplicationsForAutomaticRejection()).ReturnsAsync(_apiResponse);

        _handler = new AutomaticApplicationRejectionFunction(_api.Object);
    }

    [Test]
    public async Task Run_Rejects_Each_Application_Ready_For_Automatic_Rejection()
    {
        // Act
        await _handler.Run(default, _logger.Object);

        // Assert
        _api.Verify(x => x.RejectApplication(It.IsAny<RejectApplicationRequest>()), Times.Exactly(_apiResponse.Applications.Count()));
    }

    [Test]
    public async Task HttpTrigger_Should_Return_OkResult()
    {
        // Arrange
        var httpRequestMock = new Mock<HttpRequest>();

        // Act
        var result = await _handler.HttpAutomaticApplicationRejectionFunction(httpRequestMock.Object, _logger.Object);

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
        (result as OkObjectResult)?.Value.Should().Be("HttpAutomaticApplicationRejectionFunction successfully completed.");
    }
}