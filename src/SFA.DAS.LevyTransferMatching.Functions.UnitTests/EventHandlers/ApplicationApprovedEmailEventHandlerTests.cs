// using System.Threading.Tasks;
// using AutoFixture;
// using Microsoft.Extensions.Logging;
// using Moq;
// using NUnit.Framework;
// using SFA.DAS.Encoding;
// using SFA.DAS.LevyTransferMatching.Functions.Api;
// using SFA.DAS.LevyTransferMatching.Functions.Commands;
// using SFA.DAS.LevyTransferMatching.Functions.Events;
// using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
// using SFA.DAS.LevyTransferMatching.Messages.Events;
//
// namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers
// {
//     [TestFixture]
//     public class ApplicationApprovedEmailEventHandlerTests
//     {
//         private ApplicationApprovedEmailEventHandler _handler;
//         private ApplicationApprovedEvent _event;
//         private Mock<ILevyTransferMatchingApi> _levyTransferMatchingApi;
//         private readonly Fixture _fixture = new Fixture();
//
//         [SetUp]
//         public void Setup()
//         {
//             _levyTransferMatchingApi = new Mock<ILevyTransferMatchingApi>();
//
//             Mock<IEncodingService> encodingService = new Mock<IEncodingService>();
//
//             _event = _fixture.Create<ApplicationApprovedEvent>();
//
//             _handler = new ApplicationApprovedEmailEventHandler(_levyTransferMatchingApi.Object, encodingService.Object, Mock.Of<ILogger>());
//         }
//
//         [Test]
//         public async Task Run_Invokes_ApplicationApprovedEmail_Api_Endpoint()
//         {
//             //await _handler.Run(_event, Mock.Of<ILogger>());
//             await _handler.Handle(_event);
//
//             _levyTransferMatchingApi.Verify(x => x.ApplicationApprovedEmail(It.Is<ApplicationApprovedEmailRequest>(r =>
//                 r.ApplicationId == _event.ApplicationId &&
//                 r.PledgeId == _event.PledgeId &&
//                 r.ReceiverId == _event.ReceiverAccountId)));
//         }
//     }
// }
