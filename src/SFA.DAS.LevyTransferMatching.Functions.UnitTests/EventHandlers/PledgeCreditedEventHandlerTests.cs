using System;
using System.Linq;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.LevyTransferMatching.Functions.Events;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers
{
    [TestFixture]
    public class PledgeCreditedEventHandlerTests
    {
        private PledgeCreditedEventHandler _handler;
        private PledgeCreditedEvent _event;
        private Mock<ILevyTransferMatchingApi> _api;
        private Mock<ILogger> _logger;
        private GetApplicationsForAutomaticApprovalResponse _apiResponse;

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();

            _api = new Mock<ILevyTransferMatchingApi>();
            _logger = new Mock<ILogger>();

            _event = fixture.Create<PledgeCreditedEvent>();

            _apiResponse = fixture.Create<GetApplicationsForAutomaticApprovalResponse>();

            _api.Setup(x =>
                    x.GetApplicationsForAutomaticApproval(
                        It.Is<int?>(r => r == _event.PledgeId)))
                .ReturnsAsync(_apiResponse);

            _handler = new PledgeCreditedEventHandler(_api.Object);
        }

        [Test]
        public async Task Run_Approves_Each_Application_Ready_For_Automatic_Approval()
        {
            // Act
            await _handler.Run(_event, _logger.Object);

            // Assert
            _api.Verify(x => x.ApproveApplication(It.IsAny<ApproveApplicationRequest>()), Times.Exactly(_apiResponse.Applications.Count()));
        }
    }
}
