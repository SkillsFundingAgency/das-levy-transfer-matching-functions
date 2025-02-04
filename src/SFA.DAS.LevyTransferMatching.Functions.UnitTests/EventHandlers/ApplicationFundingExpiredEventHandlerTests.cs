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
public class ApplicationFundingExpiredEventHandlerTests
{
    private ApplicationFundingExpiredEventHandler _handler;
    private ApplicationFundingExpiredEvent _event;
    private Mock<ILevyTransferMatchingApi> _levyTransferMatchingApi;
    private readonly Fixture _fixture = new();

    [SetUp]
    public void Setup()
    {
        _levyTransferMatchingApi = new Mock<ILevyTransferMatchingApi>();
        _event = _fixture.Create<ApplicationFundingExpiredEvent>();

        _handler = new ApplicationFundingExpiredEventHandler(_levyTransferMatchingApi.Object, Mock.Of<ILogger<ApplicationFundingExpiredEventHandler>>());
    }

    [Test]
    public async Task Run_Invokes_ApplicationFundingExpired_Api_Endpoint()
    {
        await _handler.Handle(_event, Mock.Of<IMessageHandlerContext>());

        _levyTransferMatchingApi.Verify(x => x.ApplicationFundingExpired(It.Is<ApplicationFundingExpiredRequest>(r =>
            r.ApplicationId == _event.ApplicationId &&
            r.PledgeId == _event.PledgeId &&
            r.Amount == _event.Amount)));
    }
}