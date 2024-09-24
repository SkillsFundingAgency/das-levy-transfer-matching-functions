using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NServiceBus;
using RestEase.HttpClientFactory;
using SFA.DAS.Encoding;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.MessageHandlers;
using SFA.DAS.LevyTransferMatching.Functions;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.StartupExtensions;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

[assembly: NServiceBusTriggerFunction("SFA.DAS.LevyTransferMatching.MessageHandlers")]

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((hostBuilderContext, builder) => { builder.BuildDasConfiguration(hostBuilderContext.Configuration); })
    .ConfigureNServiceBus()
    .ConfigureServices((context, services) =>
    {
        services.AddDasLogging();

        var configuration = context.Configuration;

        services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), configuration));
        services.AddOptions();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var functionsConfig = configuration.GetSection(ConfigurationKeys.LevyTransferMatchingFunctions).Get<LevyTransferMatchingFunctions>();

        services.AddSingleton(configuration);
        services.AddCache(functionsConfig);
        services.AddDasDataProtection(functionsConfig);

        var apiConfig = configuration.GetSection(ConfigurationKeys.LevyTransferMatchingApi).Get<LevyTransferMatchingApiConfiguration>();
        var emailNotificationsConfig = configuration.GetSection(ConfigurationKeys.EmailNotifications).Get<EmailNotificationsConfiguration>();

        // MI isn't currently supported by NSB in isolation process so NServiceBusConnectionString will need to be a SharedAccessKey in Azure.
        // When NSB SB triggers work with MI, AzureWebJobsServiceBus needs replacing with AzureWebJobsServiceBus:fullyQualifiedNamespace env variable in azure as per
        // https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/servicebus/Microsoft.Azure.WebJobs.Extensions.ServiceBus/README.md#managed-identity-authentication
        Environment.SetEnvironmentVariable("NSERVICEBUS_LICENSE", functionsConfig.NServiceBusLicense);

        services.Configure<EncodingConfig>(configuration.GetSection(ConfigurationKeys.EncodingService));
        services.AddSingleton(cfg => cfg.GetService<IOptions<EncodingConfig>>().Value);

        services.AddSingleton(apiConfig);
        services.AddSingleton(emailNotificationsConfig);
        services.AddSingleton<IApimClientConfiguration>(x => x.GetRequiredService<LevyTransferMatchingApiConfiguration>());
        services.AddSingleton<IEncodingService, EncodingService>();
        services.AddTransient<DefaultHeadersHandler>();
        services.AddTransient<LoggingMessageHandler>();
        services.AddTransient<ApimHeadersHandler>();

        services.AddRestEaseClient<ILevyTransferMatchingApi>(apiConfig.ApiBaseUrl)
            .AddHttpMessageHandler<DefaultHeadersHandler>()
            .AddHttpMessageHandler<ApimHeadersHandler>()
            .AddHttpMessageHandler<LoggingMessageHandler>();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();