using AutoFixture;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Commands;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.Commands
{
    [TestFixture]
    public class PledgeOptionsEmailsCommandHandlerTests
    {
        private PledgeOptionsEmailsCommandHandler _handler;
        private Mock<ILevyTransferMatchingApi> _api;
        private Mock<IEncodingService> _encodingService;
        private Mock<ILogger> _logger;
        private EmailNotificationsConfiguration _config;
        private readonly Fixture _fixture = new Fixture();

        private GetPledgeOptionsEmailDataResponse _apiResponse;

        [SetUp]
        public void Setup()
        {
            _api = new Mock<ILevyTransferMatchingApi>();
            _encodingService = new Mock<IEncodingService>();
            _logger = new Mock<ILogger>();
            _config = _fixture.Create<EmailNotificationsConfiguration>();

            _apiResponse = _fixture.Create<GetPledgeOptionsEmailDataResponse>();
            _api.Setup(x => x.GetPledgeOptionsEmailData()).ReturnsAsync(_apiResponse);

            _handler = new PledgeOptionsEmailsCommandHandler(_api.Object, _encodingService.Object, _config);
        }

        [Test]
        public void Handle_Runs_Successfully()
        {
            Assert.DoesNotThrowAsync(() => _handler.Run(default(TimerInfo), _logger.Object));
        }
    }
}
