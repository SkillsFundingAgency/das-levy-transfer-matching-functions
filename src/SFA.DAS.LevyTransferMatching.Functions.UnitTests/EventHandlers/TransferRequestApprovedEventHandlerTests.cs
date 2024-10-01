using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Events;
using System;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers;

[TestFixture]
public class TransferRequestApprovedEventHandlerTests
{
    private TransferRequestApprovedEventHandler _handler;
    private Mock<ILevyTransferMatchingApi> _api;
    private readonly Fixture _fixture = new();

    [SetUp]
    public void Setup()
    {
        _api = new Mock<ILevyTransferMatchingApi>();
        _handler = new TransferRequestApprovedEventHandler(_api.Object, Mock.Of<ILogger<TransferRequestApprovedEventHandler>>());
    }

    [Test]
    public async Task Run_Invokes_TransferRequestApproved_Api_Endpoint()
    {
        var approvedEvent = _fixture.Create<TransferRequestApprovedEvent>();
        await _handler.Handle(approvedEvent, Mock.Of<IMessageHandlerContext>());

        _api.Verify(x => x.DebitApplication(It.Is<TransferRequestApprovedRequest>(r =>
            r.ApplicationId == approvedEvent.PledgeApplicationId &&
            r.NumberOfApprentices == approvedEvent.NumberOfApprentices &&
            r.Amount == approvedEvent.FundingCap)));
    }

    [Test]
    public async Task Run_Ignores_Events_With_Null_PledgeApplicationId()
    {
        var approvedEvent = new TransferRequestApprovedEvent(1, 1, DateTime.UtcNow, new CommitmentsV2.Types.UserInfo(), 1, 300, null);
        await _handler.Handle(approvedEvent,Mock.Of<IMessageHandlerContext>());

        _api.Verify(x => x.DebitApplication(It.Is<TransferRequestApprovedRequest>(r =>
            r.ApplicationId == approvedEvent.PledgeApplicationId &&
            r.NumberOfApprentices == approvedEvent.NumberOfApprentices &&
            r.Amount == approvedEvent.FundingCap)), Times.Never);
    }
}