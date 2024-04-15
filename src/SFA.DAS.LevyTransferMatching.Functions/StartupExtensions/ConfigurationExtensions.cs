using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.LevyTransferMatching.Functions.StartupExtensions;

public static class ConfigurationExtensions
{
    public static IConfiguration BuildDasConfiguration(this IConfigurationBuilder configBuilder, IConfiguration configuration)
    {
        configBuilder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();

#if DEBUG
        configBuilder.AddJsonFile("local.settings.json", optional: true);
#endif
        configBuilder.AddAzureTableStorage(options =>
        {
// #if DEBUG
//             options.ConfigurationKeys = ["SFA.DAS.LevyTransferMatching.Functions", "SFA.DAS.Encoding:EncodingService"];
// #else
//             options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
// #endif
            options.ConfigurationKeys = ["SFA.DAS.LevyTransferMatching.Functions", "SFA.DAS.Encoding:EncodingService"];
            options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
            options.EnvironmentName = configuration["EnvironmentName"];
            options.PreFixConfigurationKeys = false;
        });
        
        return configBuilder.Build();
    }
}