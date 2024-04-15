// using System.Threading.Tasks;
// using AutoFixture;
// using Microsoft.Extensions.Logging;
// using Moq;
// using NUnit.Framework;
// using SFA.DAS.LevyTransferMatching.Functions.Api;
// using SFA.DAS.LevyTransferMatching.Functions.Events;
// using SFA.DAS.LevyTransferMatching.Messages.Events;
//
// namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers
// {
//     [TestFixture]
//     public class ApplicationFundingDeclinedEventHandlerTests
//     {
//         private ApplicationFundingDeclinedEventHandler _handler;
//         private ApplicationFundingDeclinedEvent _event;
//         private Mock<ILevyTransferMatchingApi> _api;
//         private readonly Fixture _fixture = new Fixture();
//
//         [SetUp]
//         public void Setup()
//         {
//             _api = new Mock<ILevyTransferMatchingApi>();
//
//             _event = _fixture.Create<ApplicationFundingDeclinedEvent>();
//
//             _handler = new ApplicationFundingDeclinedEventHandler(_api.Object);
//         }
//
//         [Test]
//         public async Task Run_Invokes_ApplicationFundingDeclined_Api_Endpoint()
//         {
//             await _handler.Run(_event, Mock.Of<ILogger>());
//
//             _api.Verify(x => x.ApplicationFundingDeclined(It.Is<ApplicationFundingDeclinedRequest>(r =>
//                 r.ApplicationId == _event.ApplicationId &&
//                 r.PledgeId == _event.PledgeId &&
//                 r.Amount == _event.Amount)));
//         }
//     }
// }