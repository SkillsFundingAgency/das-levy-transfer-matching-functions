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

[assembly: NServiceBusTriggerFunction("SFA.DAS.LevyTransferMatching.Functions", Connection = "AzureWebJobsServiceBus")]

const string endpointName = nameof(SFA.DAS.LevyTransferMatching.Functions);
const string errorEndpointName = $"{endpointName}-error";

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((hostBuilderContext, builder) => { builder.BuildDasConfiguration(hostBuilderContext.Configuration); })
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
        if (context.HostingEnvironment.IsDevelopment())
        {
            Environment.SetEnvironmentVariable("AzureWebJobsServiceBus", functionsConfig.NServiceBusConnectionString);
        }
        else
        {
            Environment.SetEnvironmentVariable("AzureWebJobsServiceBus:fullyQualifiedNamespace", configuration["SharedServiceBusFqdn"]);
        }
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
    .UseNServiceBus((config, endpointConfiguration) =>
    {
        // Function endpoints can create their own queues or other infrastructure in the Azure Service Bus namespace by using the configuration.AdvancedConfiguration.EnableInstallers() method.
        endpointConfiguration.AdvancedConfiguration.EnableInstallers();
        endpointConfiguration.AdvancedConfiguration.SendFailedMessagesTo(errorEndpointName);
        endpointConfiguration.AdvancedConfiguration.Conventions().DefiningEventsAs(type => type.Namespace == nameof(SFA.DAS.LevyTransferMatching.Messages.Events));
        endpointConfiguration.AdvancedConfiguration.Conventions().DefiningCommandsAs(type => type.Namespace == nameof(SFA.DAS.LevyTransferMatching.Messages.Commands));
    })
    .Build();

host.Run();