using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace SFA.DAS.LevyTransferMatching.Functions.StartupExtensions;

public static class ConfigureNServiceBusExtension
{
    private const string EndpointName = "SFA.DAS.LevyTransferMatching.Functions";
    private const string ErrorEndpointName = $"{EndpointName}-error";

    public static IHostBuilder ConfigureNServiceBus(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseNServiceBus((config, endpointConfiguration) =>
        {
            endpointConfiguration.AdvancedConfiguration.EnableInstallers();
            endpointConfiguration.AdvancedConfiguration.SendFailedMessagesTo(ErrorEndpointName);
            endpointConfiguration.AdvancedConfiguration.Conventions()
                .DefiningCommandsAs(IsCommand)
                .DefiningMessagesAs(IsMessage)
                .DefiningEventsAs(IsEvent);

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

    private static bool IsMessage(Type t) => t is IMessage || IsDasMessage(t, "Messages");

    private static bool IsEvent(Type t) => t is IEvent || IsDasMessage(t, "Messages.Events");

    private static bool IsCommand(Type t) => t is ICommand || IsDasMessage(t, "Messages.Commands");

    private static bool IsDasMessage(Type t, string namespaceSuffix)
        => t.Namespace != null &&
           t.Namespace.StartsWith("SFA.DAS") &&
           t.Namespace.EndsWith(namespaceSuffix);
}