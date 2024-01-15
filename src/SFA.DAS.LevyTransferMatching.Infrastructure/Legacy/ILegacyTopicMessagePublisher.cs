using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Legacy
{
    public interface ILegacyTopicMessagePublisher
    {
        Task PublishAsync<T>(T @event);
    }
}
