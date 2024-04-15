// using AutoFixture;
// using Microsoft.Extensions.Logging;
// using Moq;
// using NUnit.Framework;
// using SFA.DAS.CommitmentsV2.Messages.Events;
// using SFA.DAS.LevyTransferMatching.Functions.Api;
// using SFA.DAS.LevyTransferMatching.Functions.Events;
// using System;
// using System.Threading.Tasks;
//
// namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers
// {
//     [TestFixture]
//     public class TransferRequestApprovedEventHandlerTests
//     {
//         private TransferRequestApprovedEventHandler _handler;
//         private Mock<ILevyTransferMatchingApi> _api;
//         private readonly Fixture _fixture = new Fixture();
//
//         [SetUp]
//         public void Setup()
//         {
//             _api = new Mock<ILevyTransferMatchingApi>();
//
//             _handler = new TransferRequestApprovedEventHandler(_api.Object);
//         }
//
//         [Test]
//         public async Task Run_Invokes_TransferRequestApproved_Api_Endpoint()
//         {
//             var _event = _fixture.Create<TransferRequestApprovedEvent>();
//             await _handler.Run(_event, Mock.Of<ILogger>());
//
//             _api.Verify(x => x.DebitApplication(It.Is<TransferRequestApprovedRequest>(r =>
//                 r.ApplicationId == _event.PledgeApplicationId &&
//                 r.NumberOfApprentices == _event.NumberOfApprentices &&
//                 r.Amount == _event.FundingCap)));
//         }
//
//         [Test]
//         public async Task Run_Ignores_Events_With_Null_PledgeApplicationId()
//         {
//             var _event = new TransferRequestApprovedEvent(1, 1, DateTime.UtcNow, new CommitmentsV2.Types.UserInfo(), 1, 300, null);
//             await _handler.Run(_event, Mock.Of<ILogger>());
//
//             _api.Verify(x => x.DebitApplication(It.Is<TransferRequestApprovedRequest>(r =>
//                 r.ApplicationId == _event.PledgeApplicationId &&
//                 r.NumberOfApprentices == _event.NumberOfApprentices &&
//                 r.Amount == _event.FundingCap)), Times.Never);
//         }
//     }
// }
