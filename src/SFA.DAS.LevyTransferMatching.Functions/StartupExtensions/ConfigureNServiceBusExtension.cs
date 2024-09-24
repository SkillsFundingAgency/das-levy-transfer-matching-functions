using System.Text.RegularExpressions;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace SFA.DAS.LevyTransferMatching.Functions.StartupExtensions;

public static partial class ConfigureNServiceBusExtension
{
    [GeneratedRegex("Command(V\\d+)?$")]
    private static partial Regex CommandRegex();
    
    [GeneratedRegex("Event(V\\d+)?$")]
    private static partial Regex EventRegex();
    
    private const string EndpointName = "SFA.DAS.LevyTransferMatching.Functions";
    private const string ErrorEndpointName = $"{EndpointName}-error";

    public static IHostBuilder ConfigureNServiceBus(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseNServiceBus((config, endpointConfiguration) =>
        {
            endpointConfiguration.AdvancedConfiguration.EnableInstallers();
            endpointConfiguration.AdvancedConfiguration.SendFailedMessagesTo(ErrorEndpointName);
            endpointConfiguration.AdvancedConfiguration.Conventions()
                .DefiningCommandsAs(t => CommandRegex().IsMatch(t.Name))
                .DefiningEventsAs(t => EventRegex().IsMatch(t.Name));

            var decodedLicence = WebUtility.HtmlDecode(config["LevyTransferMatchingFunctions:NServiceBusLicense"]);
            endpointConfiguration.AdvancedConfiguration.License(decodedLicence);
            
#if DEBUG
            var transport = endpointConfiguration.AdvancedConfiguration.UseTransport<LearningTransport>();
            transport.StorageDirectory(Path.Combine(Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("src")),
                @"src\.learningtransport"));

#endif
        });

        return hostBuilder;
    }
}