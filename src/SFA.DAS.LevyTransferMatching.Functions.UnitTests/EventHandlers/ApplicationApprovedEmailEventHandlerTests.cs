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
    public class ApplicationApprovedEmailEventHandlerTests
    {
        private ApplicationApprovedEmailEventHandler _handler;
        private ApplicationApprovedEmailEvent _event;
        private Mock<ILevyTransferMatchingApi> _levyTransferMatchingApi;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _levyTransferMatchingApi = new Mock<ILevyTransferMatchingApi>();

            Mock<IEncodingService> encodingService = new Mock<IEncodingService>();

            EmailNotificationsConfiguration config = new EmailNotificationsConfiguration { ViewTransfersBaseUrl = "www.testurl.com" };

            _event = _fixture.Create<ApplicationApprovedEmailEvent>();

            _handler = new ApplicationApprovedEmailEventHandler(_levyTransferMatchingApi.Object, encodingService.Object, config);
        }

        [Test]
        public async Task Run_Invokes_ApplicationApprovedEmail_Api_Endpoint()
        {
            await _handler.Run(_event, Mock.Of<ILogger>());

            _levyTransferMatchingApi.Verify(x => x.ApplicationApprovedEmail(It.Is<ApplicationApprovedEmailRequest>(r =>
                r.ApplicationId == _event.ApplicationId &&
                r.PledgeId == _event.PledgeId &&
                r.ReceiverId == _event.TransferReceiverId)));
        }
    }
}
