using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Events;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers;

[TestFixture]
public class ApplicationCreatedForImmediateAutoApprovalEventHandlerTests
{
    private ApplicationCreatedForImmediateAutoApprovalEventHandler _handler;
    private Mock<ILevyTransferMatchingApi> _api;
    private readonly Fixture _fixture = new();

    [SetUp]
    public void Setup()
    {
        _api = new Mock<ILevyTransferMatchingApi>();
        _handler = new ApplicationCreatedForImmediateAutoApprovalEventHandler(_api.Object, Mock.Of<ILogger>());
    }

    [Test]
    public async Task Run_Invokes_TransferRequestApproved_Api_Endpoint()
    {
        var createdEvent = _fixture.Create<ApplicationCreatedEvent>();
        await _handler.Handle(createdEvent, Mock.Of<IMessageHandlerContext>());

        _api.Verify(x => x.ApplicationCreatedForImmediateAutoApproval(It.Is<ApplicationCreatedForImmediateAutoApprovalRequest>(r =>
            r.ApplicationId == createdEvent.ApplicationId &&
            r.PledgeId == createdEvent.PledgeId)));
    }
}