using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Events;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers
{
    [TestFixture]
    public class ApplicationFundingAcceptedEventHandlerTests
    {
        private ApplicationFundingAcceptedEventHandler _handler;
        private ApplicationFundingAcceptedEvent _event;
        private Mock<ILevyTransferMatchingApi> _api;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _api = new Mock<ILevyTransferMatchingApi>();

            _event = _fixture.Create<ApplicationFundingAcceptedEvent>();

            _handler = new ApplicationFundingAcceptedEventHandler(_api.Object);
        }

        [Test]
        public async Task Run_Invokes_ApplicationFundingAccepted_Api_Endpoint_if_RejectApplications_IsTrue()
        {
            _event.RejectApplications = true; 

            await _handler.Run(_event, Mock.Of<ILogger>());

            _api.Verify(x => x.RejectPledgeApplications(It.Is<RejectPledgeApplicationsRequest>(r =>
                r.PledgeId == _event.PledgeId)));
        }

        [Test]
        public async Task Run_DoesNot_Invokes_ApplicationFundingAccepted_Api_Endpoint_if_RejectApplications_IsFalse()
        {
            _event.RejectApplications = false;

            await _handler.Run(_event, Mock.Of<ILogger>());

            _api.Verify(x => x.RejectPledgeApplications(It.Is<RejectPledgeApplicationsRequest>(r =>
                r.PledgeId == _event.PledgeId)), Times.Never());
        }
    }
}