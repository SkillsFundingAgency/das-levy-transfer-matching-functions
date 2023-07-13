using AutoFixture;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Timers;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.Commands
{
    [TestFixture]
    public class ApplicationAutomaticApprovalCommandHandlerTests
    {
        private AutomaticApplicationApprovalFunction _handler;
        private Mock<ILevyTransferMatchingApi> _api;
        private Mock<ILogger> _logger;
        private readonly Fixture _fixture = new Fixture();

        private GetApplicationsForAutomaticApprovalResponse _apiGetApplicationsForsResponse;

        [SetUp]
        public void Setup()
        {
            _api = new Mock<ILevyTransferMatchingApi>();
            _logger = new Mock<ILogger>();

            _apiGetApplicationsForsResponse = _fixture.Create<GetApplicationsForAutomaticApprovalResponse>();

            _api.Setup(x => x.GetApplicationsForAutomaticApproval(new GetApplicationsForAutomaticApprovalRequest())).ReturnsAsync(_apiGetApplicationsForsResponse);

            _handler = new AutomaticApplicationApprovalFunction(_api.Object);
        }

        [Test]
        public async Task Run_WithValidApplications_CallsApiToApproveApplications()
        {
            // Act
            await _handler.Run(default(TimerInfo), _logger.Object);

            // Assert
            _api.Verify(x => x.ApproveApplication(It.IsAny<ApproveApplicationRequest>()), Times.Exactly(_apiGetApplicationsForsResponse.Applications.Count()));
        }

    }
}
