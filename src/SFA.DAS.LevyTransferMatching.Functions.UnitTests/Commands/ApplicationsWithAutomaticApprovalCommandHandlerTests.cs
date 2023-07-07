using AutoFixture;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.Commands
{
    [TestFixture]
    public class ApplicationAutomaticApprovalCommandHandlerTests
    {
        private ApplicationAutomaticApprovalCommandHandler _handler;
        private Mock<ILevyTransferMatchingApi> _api;
        private Mock<ILogger> _logger;
        private readonly Fixture _fixture = new Fixture();

        private ApplicationAutomaticApprovalResponse _apiApplicationsResponse;

        [SetUp]
        public void Setup()
        {
            _api = new Mock<ILevyTransferMatchingApi>();
            _logger = new Mock<ILogger>();

            _apiApplicationsResponse = _fixture.Create<ApplicationAutomaticApprovalResponse>();

            _api.Setup(x => x.ApplicationsWithAutomaticApproval(new ApplicationAutomaticApprovalRequest())).ReturnsAsync(_apiApplicationsResponse);

            _handler = new ApplicationAutomaticApprovalCommandHandler(_api.Object);
        }

        [Test]
        public async Task Run_WithValidApplications_CallsApiToApproveApplications()
        {
            // Act
            await _handler.Run(default(TimerInfo), _logger.Object);

            // Assert
            _api.Verify(x => x.ApproveAutomaticApplication(It.IsAny<ApproveAutomaticApplicationRequest>()), Times.Exactly(_apiApplicationsResponse.Applications.Count()));
        }

    }
}
