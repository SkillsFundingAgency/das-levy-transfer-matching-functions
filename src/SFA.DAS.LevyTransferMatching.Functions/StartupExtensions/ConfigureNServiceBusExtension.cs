using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace SFA.DAS.LevyTransferMatching.Functions.StartupExtensions;

public static class ConfigureNServiceBusExtension
{
    private const string EndpointName = "SFA.DAS.LevyTransferMatching.MessageHandler";
    private const string ErrorEndpointName = $"{EndpointName}-error";

    public static IHostBuilder ConfigureNServiceBus(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseNServiceBus((config, endpointConfiguration) =>
        {
            // Function endpoints can create their own queues or other infrastructure in the Azure Service Bus namespace by using the configuration.AdvancedConfiguration.EnableInstallers() method.
            endpointConfiguration.AdvancedConfiguration.EnableInstallers();

            endpointConfiguration.AdvancedConfiguration.SendFailedMessagesTo(ErrorEndpointName);
            endpointConfiguration.AdvancedConfiguration.Conventions().DefiningEventsAs(type => type.Namespace == nameof(SFA.DAS.LevyTransferMatching.Messages.Events));
            endpointConfiguration.AdvancedConfiguration.Conventions().DefiningCommandsAs(type => type.Namespace == nameof(SFA.DAS.LevyTransferMatching.Messages.Commands));

            var decodedLicence = WebUtility.HtmlDecode(config["LevyTransferMatchingFunctions:NServiceBusLicense"]);
            endpointConfiguration.AdvancedConfiguration.License(decodedLicence);
        });

        return hostBuilder;
    }
}