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
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

[assembly: NServiceBusTriggerFunction("SFA.DAS.LevyTransferMatching.Functions")]


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((hostBuilderContext, builder) =>
    {
        builder.BuildDasConfiguration(hostBuilderContext.Configuration);
    })
    .ConfigureServices(services =>
    {
        services.AddDasLogging();

        var serviceProvider = services.BuildServiceProvider();

        var hostConfig = serviceProvider.GetConfiguration();
            //.BuildDasConfiguration();

        services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), hostConfig));
        services.AddOptions();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var functionsConfig = hostConfig.GetSection(ConfigurationKeys.LevyTransferMatchingFunctions).Get<LevyTransferMatchingFunctions>();

        //var logger = serviceProvider.GetLogger(nameof(Program));

        services.AddSingleton(hostConfig);
        //services.AddNServiceBus(functionsConfig, logger);
        services.AddLegacyServiceBus(functionsConfig);
        services.AddCache(functionsConfig);
        services.AddDasDataProtection(functionsConfig);

        var apiConfig = hostConfig.GetSection(ConfigurationKeys.LevyTransferMatchingApi).Get<LevyTransferMatchingApiConfiguration>();
        var emailNotificationsConfig = hostConfig.GetSection(ConfigurationKeys.EmailNotifications).Get<EmailNotificationsConfiguration>();
        
        Environment.SetEnvironmentVariable("AzureWebJobsServiceBus", functionsConfig.NServiceBusConnectionString);
        Environment.SetEnvironmentVariable("ServiceBus:ConnectionString", functionsConfig.NServiceBusConnectionString);
        Environment.SetEnvironmentVariable("NSERVICEBUS_LICENSE", functionsConfig.NServiceBusLicense);

        services.Configure<EncodingConfig>(hostConfig.GetSection(ConfigurationKeys.EncodingService));
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
    })
    .UseNServiceBus((config, endpointConfiguration) =>
    {
        // Function endpoints can create their own queues or other infrastructure in the Azure Service Bus namespace by using the configuration.AdvancedConfiguration.EnableInstallers() method.
        endpointConfiguration.AdvancedConfiguration.EnableInstallers();

       // var functionsConfig = hostConfig.GetSection(ConfigurationKeys.LevyTransferMatchingFunctions).Get<LevyTransferMatchingFunctions>();

        // if (functionsConfig.NServiceBusConnectionString.Equals("UseDevelopmentStorage=true", StringComparison.CurrentCultureIgnoreCase))
        // {
        //     endpointConfiguration
        //         .UseTransport<LearningTransport>()
        //         .StorageDirectory(Path.Combine(Directory.GetCurrentDirectory()[..Directory.GetCurrentDirectory().IndexOf("src", StringComparison.Ordinal)], @"src\.learningtransport")
        //         );
        // }
        // else
        // {
        //     Environment.SetEnvironmentVariable("AzureWebJobsServiceBus", functionsConfig.NServiceBusConnectionString);
        // }
        //
        // Environment.SetEnvironmentVariable("NSERVICEBUS_LICENSE", functionsConfig.NServiceBusLicense);
    })
    .Build();

host.Run();