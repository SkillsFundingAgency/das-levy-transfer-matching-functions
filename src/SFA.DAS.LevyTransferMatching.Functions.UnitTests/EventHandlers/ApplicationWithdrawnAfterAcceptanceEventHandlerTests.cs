using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Events;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers;

[TestFixture]
public class ApplicationWithdrawnAfterAcceptanceEventHandlerTests
{
    private ApplicationWithdrawnAfterAcceptanceEventHandler _handler;
    private Mock<ILevyTransferMatchingApi> _api;
    private readonly Fixture _fixture = new();

    [SetUp]
    public void Setup()
    {
        _api = new Mock<ILevyTransferMatchingApi>();
        _handler = new ApplicationWithdrawnAfterAcceptanceEventHandler(_api.Object, Mock.Of<ILogger<ApplicationWithdrawnAfterAcceptanceEventHandler>>());
    }

    [Test]
    public async Task Run_Invokes_ApplicationWithdrawnAfterAcceptance_Api_Endpoint()
    {
        var acceptanceEvent = _fixture.Create<ApplicationWithdrawnAfterAcceptanceEvent>();
        await _handler.Handle(acceptanceEvent, Mock.Of<IMessageHandlerContext>());

        _api.Verify(x => x.ApplicationWithdrawnAfterAcceptance(It.Is<ApplicationWithdrawnAfterAcceptanceRequest>(r =>
            r.ApplicationId == acceptanceEvent.ApplicationId &&
            r.PledgeId == acceptanceEvent.PledgeId &&
            r.Amount == acceptanceEvent.Amount)));
    }
}