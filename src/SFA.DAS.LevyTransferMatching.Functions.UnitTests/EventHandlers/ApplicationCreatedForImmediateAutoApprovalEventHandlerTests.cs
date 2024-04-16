using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Events;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers
{
    [TestFixture]
    public class ApplicationCreatedForImmediateAutoApprovalEventHandlerTests
    {
        private ApplicationCreatedForImmediateAutoApprovalEventHandler _handler;
        private Mock<ILevyTransferMatchingApi> _api;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _api = new Mock<ILevyTransferMatchingApi>();

            _handler = new ApplicationCreatedForImmediateAutoApprovalEventHandler(_api.Object, Mock.Of<ILogger>());
        }

        [Test]
        public async Task Run_Invokes_TransferRequestApproved_Api_Endpoint()
        {
            var _event = _fixture.Create<ApplicationCreatedEvent>();
            await _handler.Handle(_event, Mock.Of<IMessageHandlerContext>());

            _api.Verify(x => x.ApplicationCreatedForImmediateAutoApproval(It.Is<ApplicationCreatedForImmediateAutoApprovalRequest>(r =>
                r.ApplicationId == _event.ApplicationId &&
                r.PledgeId == _event.PledgeId)));
        }
    }
}
