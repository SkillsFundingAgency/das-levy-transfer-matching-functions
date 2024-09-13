using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Functions.Events;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.LevyTransferMatching.Messages.Legacy;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers;

[TestFixture]
public class ApplicationApprovedForLegacyTopicPublishingEventHandlerTests
{
    private ApplicationApprovedForLegacyTopicPublishingEventHandler _handler;
    private Mock<ILegacyTopicMessagePublisher> _legacyTopicMessagePublisher;
    private readonly Fixture _fixture = new();

    [SetUp]
    public void Setup()
    {
        _legacyTopicMessagePublisher = new Mock<ILegacyTopicMessagePublisher>();
        _legacyTopicMessagePublisher.Setup(x => x.PublishAsync(It.IsAny<object>()))
            .Returns(() => Task.CompletedTask);

        _handler = new ApplicationApprovedForLegacyTopicPublishingEventHandler(_legacyTopicMessagePublisher.Object, Mock.Of<ILogger>());
    }

    [Test]
    public async Task Run_Invokes_Legacy_Topic_Message_Publisher()
    {
        var sourceEvent = _fixture.Create<ApplicationApprovedEvent>();

        await _handler.Handle(sourceEvent, Mock.Of<IMessageHandlerContext>());

        _legacyTopicMessagePublisher.Verify(x => x.PublishAsync(It.Is<PledgeApplicationApproved>(r =>
            r.PledgeId == sourceEvent.PledgeId &&
            r.ApplicationId == sourceEvent.ApplicationId &&
            r.ApprovedOn == sourceEvent.ApprovedOn &&
            r.Amount == sourceEvent.Amount &&
            r.TransferSenderId == sourceEvent.TransferSenderId)));
    }
}