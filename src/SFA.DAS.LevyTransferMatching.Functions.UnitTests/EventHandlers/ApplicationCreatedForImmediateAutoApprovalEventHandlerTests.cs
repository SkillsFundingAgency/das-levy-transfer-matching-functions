using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Events;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using System;
using System.Threading.Tasks;

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

            _handler = new ApplicationCreatedForImmediateAutoApprovalEventHandler(_api.Object);
        }

        [Test]
        public async Task Run_Invokes_TransferRequestApproved_Api_Endpoint()
        {
            var _event = _fixture.Create<ApplicationCreatedEvent>();
            await _handler.Run(_event, Mock.Of<ILogger>());

            _api.Verify(x => x.ApplicationCreatedForImmediateAutoApproval(It.Is<ApplicationCreatedForImmediateAutoApprovalRequest>(r =>
                r.ApplicationId == _event.ApplicationId &&
                r.PledgeId == _event.PledgeId)));
        }

       
    }
}
