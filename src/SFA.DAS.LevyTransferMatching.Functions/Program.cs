using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SFA.DAS.LevyTransferMatching.Functions.StartupExtensions;

var hostBuilder = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDasLogging();

        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetConfiguration();

        var config = configuration.BuildDasConfiguration();
        services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config));
        services.AddOptions();

        services.AddServiceRegistration(config, serviceProvider);

    });
   

var host = hostBuilder.Build();

host.Run();