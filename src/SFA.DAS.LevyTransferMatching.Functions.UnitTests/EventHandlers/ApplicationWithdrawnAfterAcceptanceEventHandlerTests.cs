using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Events;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers
{
    [TestFixture]
    class ApplicationWithdrawnAfterAcceptanceEventHandlerTests
    {
        private ApplicationWithdrawnAfterAcceptanceEventHandler _handler;
        private Mock<ILevyTransferMatchingApi> _api;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _api = new Mock<ILevyTransferMatchingApi>();

            _handler = new ApplicationWithdrawnAfterAcceptanceEventHandler(_api.Object);
        }

        [Test]
        public async Task Run_Invokes_ApplicationWithdrawnAfterAcceptance_Api_Endpoint()
        {
            var _event = _fixture.Create<ApplicationWithdrawnAfterAcceptanceEvent>();
            await _handler.Run(_event, Mock.Of<ILogger>());

            _api.Verify(x => x.ApplicationWithdrawnAfterAcceptance(It.Is<ApplicationWithdrawnAfterAcceptanceRequest>(r =>
                r.ApplicationId == _event.ApplicationId &&
                r.PledgeId == _event.PledgeId &&
                r.Amount == _event.Amount)));
        }
    }
}
