using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RestEase.HttpClientFactory;
using SFA.DAS.Encoding;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.MessageHandlers;
using SFA.DAS.LevyTransferMatching.Functions;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.StartupExtensions;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SFA.DAS.LevyTransferMatching.Functions;

// Read before updating packages:
// v3 Azure functions are NOT compatible at time of writing with v5 versions of the Microsoft.Extensions.* libraries
// https://github.com/Azure/azure-functions-core-tools/issues/2304#issuecomment-735454326
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddDasLogging();

        var serviceProvider = builder.Services.BuildServiceProvider();
        var configuration = serviceProvider.GetConfiguration();

        var config = configuration.BuildDasConfiguration();
            
        builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config));
        builder.Services.AddOptions();

        ConfigureServices(builder, config, serviceProvider);
    }

    private void ConfigureServices(IFunctionsHostBuilder builder, IConfiguration configuration, ServiceProvider serviceProvider)
    {
        var config = configuration.GetSection(ConfigurationKeys.LevyTransferMatchingFunctions).Get<LevyTransferMatchingFunctions>();

        var logger = serviceProvider.GetLogger(GetType().AssemblyQualifiedName);

        builder.Services
            .AddSingleton(config)
            .AddNServiceBus(config, logger)
            .AddLegacyServiceBus(config)
            .AddCache(config)
            .AddDasDataProtection(config);

        var apiConfig = configuration.GetSection(ConfigurationKeys.LevyTransferMatchingApi).Get<LevyTransferMatchingApiConfiguration>();
        var emailNotificationsConfig = configuration.GetSection(ConfigurationKeys.EmailNotifications).Get<EmailNotificationsConfiguration>();

        builder.Services.Configure<EncodingConfig>(configuration.GetSection(ConfigurationKeys.EncodingService));
        builder.Services.AddSingleton(cfg => cfg.GetService<IOptions<EncodingConfig>>().Value);

        builder.Services.AddSingleton(apiConfig);
        builder.Services.AddSingleton(emailNotificationsConfig);
        builder.Services.AddSingleton<IApimClientConfiguration>(x => x.GetRequiredService<LevyTransferMatchingApiConfiguration>());
        builder.Services.AddSingleton<IEncodingService, EncodingService>();
        builder.Services.AddTransient<DefaultHeadersHandler>();
        builder.Services.AddTransient<LoggingMessageHandler>();
        builder.Services.AddTransient<ApimHeadersHandler>();

        builder.Services.AddRestEaseClient<ILevyTransferMatchingApi>(apiConfig.ApiBaseUrl)
            .AddHttpMessageHandler<DefaultHeadersHandler>()
            .AddHttpMessageHandler<ApimHeadersHandler>()
            .AddHttpMessageHandler<LoggingMessageHandler>();
    }
}