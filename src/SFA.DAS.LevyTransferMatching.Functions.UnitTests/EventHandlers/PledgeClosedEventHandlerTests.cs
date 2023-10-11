using Moq;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.LevyTransferMatching.Functions.Events;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using AutoFixture.NUnit3;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers
{
    [TestFixture]
    public class PledgeClosedEventHandlerTests
    {
        [Test]
        [MoqAutoData]
        public async Task Run_Rejects_Applications_If_InSufficient_Funds(
            [Frozen] Mock<ILevyTransferMatchingApi> _api,
            ILogger _logger,
            PledgeClosedEvent _event,
            PledgeClosedEventHandler _handler)
        {
            _event.InsufficientFunds = true;
            // Act
            await _handler.Run(_event, _logger);

            // Assert
            _api.Verify(x => x.RejectPledgeApplications(
                It.Is<RejectPledgeApplicationsRequest>(request =>
                request.PledgeId.Equals(_event.PledgeId))),
                Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task Run_Doesnt_Reject_Applications_If_Sufficient_Funds(
            [Frozen] Mock<ILevyTransferMatchingApi> _api,
            ILogger _logger,
            PledgeClosedEvent _event,
            PledgeClosedEventHandler _handler)
        {
            _event.InsufficientFunds = false;
            // Act
            await _handler.Run(_event, _logger);

            // Assert
            _api.Verify(x => x.RejectPledgeApplications(
                           It.Is<RejectPledgeApplicationsRequest>(request =>
                           request.PledgeId.Equals(_event.PledgeId))),
                           Times.Never);
        }
    }   
}
