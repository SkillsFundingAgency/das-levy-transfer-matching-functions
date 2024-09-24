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
public class ApplicationApprovedEventHandlerTests
{
    private ApplicationApprovedEventHandler _handler;
    private ApplicationApprovedEvent _event;
    private Mock<ILevyTransferMatchingApi> _api;
    private readonly Fixture _fixture = new();

    [SetUp]
    public void Setup()
    {
        _api = new Mock<ILevyTransferMatchingApi>();
        _event = _fixture.Create<ApplicationApprovedEvent>();
        _handler = new ApplicationApprovedEventHandler(_api.Object, Mock.Of<ILogger<ApplicationApprovedEventHandler>>());
    }

    [Test]
    public async Task Run_Invokes_ApplicationApproved_Api_Endpoint()
    {
        await _handler.Handle(_event, Mock.Of<IMessageHandlerContext>());

        _api.Verify(x => x.ApplicationApproved(It.Is<ApplicationApprovedRequest>(r =>
            r.ApplicationId == _event.ApplicationId &&
            r.PledgeId == _event.PledgeId &&
            r.Amount == _event.Amount)));
    }
}