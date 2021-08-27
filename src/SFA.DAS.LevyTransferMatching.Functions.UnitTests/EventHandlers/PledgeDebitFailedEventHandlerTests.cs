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
    public class PledgeDebitFailedEventHandlerTests
    {
        private PledgeDebitFailedEventHandler _handler;
        private PledgeDebitFailedEvent _event;
        private Mock<ILevyTransferMatchingApi> _api;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _api = new Mock<ILevyTransferMatchingApi>();

            _event = _fixture.Create<PledgeDebitFailedEvent>();

            _handler = new PledgeDebitFailedEventHandler(_api.Object);
        }

        [Test]
        public async Task Run_Invokes_PledgeDebitFailed_Api_Endpoint()
        {
            await _handler.Run(_event, Mock.Of<ILogger>());

            _api.Verify(x => x.PledgeDebitFailed(It.Is<PledgeDebitFailedRequest>(r =>
                r.ApplicationId == _event.ApplicationId &&
                r.PledgeId == _event.PledgeId &&
                r.Amount == _event.Amount)));
        }
    }
}
