using Microsoft.Extensions.Azure;
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

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .UseNServiceBus()
    .ConfigureServices(services =>
    {
        services.AddDasLogging();

        var serviceProvider = services.BuildServiceProvider();
        
        var configuration = serviceProvider
            .GetConfiguration()
            .BuildDasConfiguration();

        services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), configuration));
        services.AddOptions();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var functionsConfig = configuration.GetSection(ConfigurationKeys.LevyTransferMatchingFunctions).Get<LevyTransferMatchingFunctions>();

        var logger = serviceProvider.GetLogger(nameof(Program));

        services.AddSingleton(configuration);
        //services.AddNServiceBus(functionsConfig, logger);
        services.AddLegacyServiceBus(functionsConfig);
        services.AddCache(functionsConfig);
        services.AddDasDataProtection(functionsConfig);

        var apiConfig = configuration.GetSection(ConfigurationKeys.LevyTransferMatchingApi).Get<LevyTransferMatchingApiConfiguration>();
        var emailNotificationsConfig = configuration.GetSection(ConfigurationKeys.EmailNotifications).Get<EmailNotificationsConfiguration>();
        
        Environment.SetEnvironmentVariable("NServiceBusConnectionString", functionsConfig.NServiceBusConnectionString);

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
    })
    // .UseNServiceBus(config =>
    // {
    //     config.AddServiceBusClient<servi>(options =>
    //     {
    //         options.EndpointConfiguration = endpoint =>
    //         {
    //             endpoint.UseTransport<LearningTransport>().StorageDirectory(
    //                 Path.Combine(
    //                     Directory.GetCurrentDirectory()
    //                         .Substring(0, Directory.GetCurrentDirectory().IndexOf("src")),
    //                     @"src\.learningtransport"));
    //                 
    //             return endpoint;
    //         };
    //     });
    //     if (_apiConfig.NServiceBusConnectionString.Equals("UseDevelopmentStorage=true", StringComparison.CurrentCultureIgnoreCase))
    //     {
    //
    //         services.AddNServiceBus(logger, (options) =>
    //         {
    //             options.EndpointConfiguration = (endpoint) =>
    //             {
    //                 endpoint.UseTransport<LearningTransport>().StorageDirectory(
    //                     Path.Combine(
    //                         Directory.GetCurrentDirectory()
    //                             .Substring(0, Directory.GetCurrentDirectory().IndexOf("src")),
    //                         @"src\.learningtransport"));
    //                 
    //                 return endpoint;
    //             };
    //         });
    //     }
    //     else
    //     {
    //         Environment.SetEnvironmentVariable("NServiceBusConnectionString", config.NServiceBusConnectionString);
    //         services.AddNServiceBus(logger);
    //     }
    //     
    //     config.Transport
    // })
    .Build();

host.Run();