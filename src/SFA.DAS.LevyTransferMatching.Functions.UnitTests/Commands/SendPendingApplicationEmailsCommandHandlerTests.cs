using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Commands;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.Commands;

[TestFixture]
public class SendPendingApplicationEmailsCommandHandlerTests
{
    private SendPendingApplicationEmailsCommandHandler _handler;
    private Mock<ILevyTransferMatchingApi> _api;
    private Mock<IEncodingService> _encodingService;
    private Mock<ILogger> _logger;
    private EmailNotificationsConfiguration _config;
    private readonly Fixture _fixture = new();

    private GetPendingApplicationEmailDataResponse _apiResponse;

    [SetUp]
    public void Setup()
    {
        _api = new Mock<ILevyTransferMatchingApi>();
        _encodingService = new Mock<IEncodingService>();
        _logger = new Mock<ILogger>();
        _config = _fixture.Create<EmailNotificationsConfiguration>();

        _apiResponse = _fixture.Create<GetPendingApplicationEmailDataResponse>();
        _api.Setup(x => x.GetPendingApplicationEmailData()).ReturnsAsync(_apiResponse);

        _handler = new SendPendingApplicationEmailsCommandHandler(_api.Object, _encodingService.Object, _config);
    }

    [Test]
    public void Handle_Runs_Successfully()
    {
        var action = () => _handler.Run(default, _logger.Object);
        action.Should().NotThrowAsync();
    }
}