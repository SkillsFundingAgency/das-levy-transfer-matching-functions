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

        [SetUp]
        public void Setup()
        {
            _levyTransferMatchingApi = new Mock<ILevyTransferMatchingApi>();

            Mock<IEncodingService> encodingService = new Mock<IEncodingService>();
            EmailNotificationsConfiguration config = new EmailNotificationsConfiguration { ViewOpportunitiesBaseUrl = "www.testurl.com" };

            _event = _fixture.Create<ApplicationRejectedEmailEvent>();

            _handler = new ApplicationRejectedEmailEventHandler(_levyTransferMatchingApi.Object, encodingService.Object, config);
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
