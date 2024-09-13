using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Events;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers;

[TestFixture]
public class ApplicationRejectedEmailEventHandlerTests
{
    private ApplicationRejectedEmailEventHandler _handler;
    private ApplicationRejectedEvent _event;
    private Mock<ILevyTransferMatchingApi> _levyTransferMatchingApi;
    private readonly Fixture _fixture = new();
    private EmailNotificationsConfiguration _config;

    [SetUp]
    public void Setup()
    {
        _levyTransferMatchingApi = new Mock<ILevyTransferMatchingApi>();
        _config = _fixture.Create<EmailNotificationsConfiguration>();

        var encodingService = new Mock<IEncodingService>();

        _event = _fixture.Create<ApplicationRejectedEvent>();

        _handler = new ApplicationRejectedEmailEventHandler(_levyTransferMatchingApi.Object, encodingService.Object, _config, Mock.Of<ILogger>());
    }

    [Test]
    public async Task Run_Invokes_ApplicationRejectedEmail_Api_Endpoint()
    {
        await _handler.Handle(_event, Mock.Of<IMessageHandlerContext>());

        _levyTransferMatchingApi.Verify(x => x.ApplicationRejectedEmail(It.Is<ApplicationRejectedEmailRequest>(r =>
            r.ApplicationId == _event.ApplicationId &&
            r.PledgeId == _event.PledgeId &&
            r.ReceiverId == _event.ReceiverAccountId)));
    }
}