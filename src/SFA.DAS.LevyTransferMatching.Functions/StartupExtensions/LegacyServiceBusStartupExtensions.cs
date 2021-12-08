using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;

namespace SFA.DAS.LevyTransferMatching.Functions.StartupExtensions
{
    public static class LegacyServiceBusStartupExtensions
    {
        public static IServiceCollection AddLegacyServiceBus(this IServiceCollection services, LevyTransferMatchingFunctions config)
        {
            services.AddTransient<ILegacyTopicMessagePublisher, LegacyTopicMessagePublisher>();
            return services;
        }
    }
}
