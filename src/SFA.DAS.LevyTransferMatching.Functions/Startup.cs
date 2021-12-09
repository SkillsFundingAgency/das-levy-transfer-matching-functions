using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RestEase.HttpClientFactory;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Http.Configuration;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.StartupExtensions;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

[assembly: FunctionsStartup(typeof(SFA.DAS.LevyTransferMatching.Functions.Startup))]
namespace SFA.DAS.LevyTransferMatching.Functions
{
    // Read before updating packages:
    // v3 Azure functions are NOT compatible at time of writing with v5 versions of the Microsoft.Extensions.* libraries
    // https://github.com/Azure/azure-functions-core-tools/issues/2304#issuecomment-735454326
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddNLog();

            var serviceProvider = builder.Services.BuildServiceProvider();
            var configuration = serviceProvider.GetConfiguration();

            var configBuilder = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

#if DEBUG
            configBuilder.AddJsonFile("local.settings.json", optional: true);
#endif
            configBuilder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            });

            var config = configBuilder.Build();
            
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config));
            builder.Services.AddOptions();

            ConfigureServices(builder, config, serviceProvider);
        }

        private void ConfigureServices(IFunctionsHostBuilder builder, IConfiguration configuration, ServiceProvider serviceProvider)
        {
            var config = configuration.GetSection("LevyTransferMatchingFunctions").Get<LevyTransferMatchingFunctions>();

            var logger = serviceProvider.GetLogger(GetType().AssemblyQualifiedName);

            builder.Services
                .AddSingleton(config)
                .AddNServiceBus(config, logger)
                .AddLegacyServiceBus(config)
                .AddCache(config)
                .AddDasDataProtection(config);

            var apiConfig = configuration.GetSection("LevyTransferMatchingApi").Get<LevyTransferMatchingApiConfiguration>();

            builder.Services.AddSingleton(apiConfig);

            builder.Services.AddSingleton<IApimClientConfiguration>(x => x.GetRequiredService<LevyTransferMatchingApiConfiguration>());
            builder.Services.AddTransient<Http.MessageHandlers.DefaultHeadersHandler>();
            builder.Services.AddTransient<Http.MessageHandlers.LoggingMessageHandler>();
            builder.Services.AddTransient<Http.MessageHandlers.ApimHeadersHandler>();

            builder.Services.AddRestEaseClient<ILevyTransferMatchingApi>(apiConfig.ApiBaseUrl)
                .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.ApimHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();
        }
    }
}
