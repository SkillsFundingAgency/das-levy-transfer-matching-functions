using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Commands;
using SFA.DAS.LevyTransferMatching.Functions.Events;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers
{
    [TestFixture]
    public class ApplicationRejectedEmailEventHandlerTests
    {
        private ApplicationRejectedEmailEventHandler _handler;
        private ApplicationRejectedEmailEvent _event;
        private Mock<ILevyTransferMatchingApi> _levyTransferMatchingApi;
        private readonly Fixture _fixture = new Fixture();
        private EmailNotificationsConfiguration _config;

        [SetUp]
        public void Setup()
        {
            _levyTransferMatchingApi = new Mock<ILevyTransferMatchingApi>();
            _config = _fixture.Create<EmailNotificationsConfiguration>();

            Mock<IEncodingService> encodingService = new Mock<IEncodingService>();

            _event = _fixture.Create<ApplicationRejectedEmailEvent>();

            _handler = new ApplicationRejectedEmailEventHandler(_levyTransferMatchingApi.Object, encodingService.Object, _config);
        }

        [Test]
        public async Task Run_Invokes_ApplicationRejectedEmail_Api_Endpoint()
        {
            await _handler.Run(_event, Mock.Of<ILogger>());

            _levyTransferMatchingApi.Verify(x => x.ApplicationRejectedEmail(It.Is<ApplicationRejectedEmailRequest>(r =>
                r.ApplicationId == _event.ApplicationId &&
                r.PledgeId == _event.PledgeId &&
                r.ReceiverId == _event.ReceiverAccountId)));
        }
    }
}
