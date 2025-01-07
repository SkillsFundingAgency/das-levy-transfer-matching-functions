using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Timers;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.Timers;

[TestFixture]
public class AutomaticApplicationDeclineFunctionTests
{
    private AutomaticApplicationDeclineFunction _handler;
    private Mock<ILevyTransferMatchingApi> _api;
    private Mock<ILogger<AutomaticApplicationDeclineFunction>> _logger;
    private GetApplicationsForAutomaticDeclineResponse _apiResponse;

    [SetUp]
    public void Setup()
    {
        var fixture = new Fixture();

        _api = new Mock<ILevyTransferMatchingApi>();
        _logger = new Mock<ILogger<AutomaticApplicationDeclineFunction>>();

        _apiResponse = fixture.Create<GetApplicationsForAutomaticDeclineResponse>();
        _api.Setup(x => x.GetApplicationsForAutomaticDecline()).ReturnsAsync(_apiResponse);
        _handler = new AutomaticApplicationDeclineFunction(_api.Object, _logger.Object);
    }

    [Test]
    public async Task Run_Declines_Each_Application_To_be_Auto_Declined()
    {
        // Act
        await _handler.Run(default);

        // Assert
        _api.Verify(x => x.DeclineAcceptedFunding(It.IsAny<DeclineAcceptedFundingRequest>()), Times.Exactly(_apiResponse.ApplicationIdsToDecline.Count()));
    }
}
