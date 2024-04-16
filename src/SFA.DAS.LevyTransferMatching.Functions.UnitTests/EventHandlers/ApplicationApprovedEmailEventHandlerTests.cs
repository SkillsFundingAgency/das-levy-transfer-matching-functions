using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Events;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers
{
    [TestFixture]
    public class ApplicationApprovedEmailEventHandlerTests
    {
        private ApplicationApprovedEmailEventHandler _handler;
        private ApplicationApprovedEvent _event;
        private Mock<ILevyTransferMatchingApi> _levyTransferMatchingApi;
        private readonly Fixture _fixture = new();

        [SetUp]
        public void Setup()
        {
            _levyTransferMatchingApi = new Mock<ILevyTransferMatchingApi>();

            var encodingService = new Mock<IEncodingService>();

            _event = _fixture.Create<ApplicationApprovedEvent>();

            _handler = new ApplicationApprovedEmailEventHandler(_levyTransferMatchingApi.Object, encodingService.Object, Mock.Of<ILogger<ApplicationApprovedEmailEventHandler>>());
        }

        [Test]
        public async Task Run_Invokes_ApplicationApprovedEmail_Api_Endpoint()
        {
            await _handler.Handle(_event, Mock.Of<IMessageHandlerContext>());

            _levyTransferMatchingApi.Verify(x => x.ApplicationApprovedEmail(It.Is<ApplicationApprovedEmailRequest>(r =>
                r.ApplicationId == _event.ApplicationId &&
                r.PledgeId == _event.PledgeId &&
                r.ReceiverId == _event.ReceiverAccountId)));
        }
    }
}
