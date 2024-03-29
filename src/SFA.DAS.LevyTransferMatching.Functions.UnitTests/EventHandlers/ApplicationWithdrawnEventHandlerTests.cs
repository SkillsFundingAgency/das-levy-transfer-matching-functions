﻿using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Functions.Events;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.LevyTransferMatching.Messages.Legacy;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers
{
    [TestFixture]
    public class ApplicationWithdrawnEventHandlerTests
    {
        private ApplicationWithdrawnEventHandler _handler;
        private Mock<ILegacyTopicMessagePublisher> _legacyTopicMessagePublisher;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _legacyTopicMessagePublisher = new Mock<ILegacyTopicMessagePublisher>();
            _legacyTopicMessagePublisher.Setup(x => x.PublishAsync(It.IsAny<object>()))
                .Returns(() => Task.CompletedTask);

            _handler = new ApplicationWithdrawnEventHandler(_legacyTopicMessagePublisher.Object);
        }

        [Test]
        public async Task Run_Invokes_Legacy_Topic_Message_Publisher()
        {
            var sourceEvent = _fixture.Create<ApplicationWithdrawnEvent>();

            await _handler.Run(sourceEvent, Mock.Of<ILogger>());

            _legacyTopicMessagePublisher.Verify(x => x.PublishAsync(It.Is<PledgeApplicationWithdrawn>(r =>
                r.PledgeId == sourceEvent.PledgeId &&
                r.ApplicationId == sourceEvent.ApplicationId &&
                r.WithdrawnOn == sourceEvent.WithdrawnOn &&
                r.TransferSenderId == sourceEvent.TransferSenderId)));
        }
    }
}