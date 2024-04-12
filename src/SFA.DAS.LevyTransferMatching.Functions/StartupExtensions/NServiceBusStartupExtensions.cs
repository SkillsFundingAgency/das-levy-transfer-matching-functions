// using Microsoft.Extensions.DependencyInjection;
// using NServiceBus;
// using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
//
// namespace SFA.DAS.LevyTransferMatching.Functions.StartupExtensions;
//
// public static class NServiceBusStartupExtensions
// {
//     public static IServiceCollection AddNServiceBus(this IServiceCollection services, LevyTransferMatchingFunctions config, ILogger logger)
//     {
//         if (config.NServiceBusConnectionString.Equals("UseDevelopmentStorage=true", StringComparison.CurrentCultureIgnoreCase))
//         {
//
//             services.AddNServiceBus(logger, (options) =>
//             {
//                 options.EndpointConfiguration = (endpoint) =>
//                 {
//                     endpoint.UseTransport<LearningTransport>().StorageDirectory(
//                         Path.Combine(
//                             Directory.GetCurrentDirectory()
//                                 .Substring(0, Directory.GetCurrentDirectory().IndexOf("src")),
//                             @"src\.learningtransport"));
//                     
//                     return endpoint;
//                 };
//             });
//         }
//         else
//         {
//             Environment.SetEnvironmentVariable("NServiceBusConnectionString", config.NServiceBusConnectionString);
//             services.AddNServiceBus(logger);
//         }
//
//         return services;
//     }
// }