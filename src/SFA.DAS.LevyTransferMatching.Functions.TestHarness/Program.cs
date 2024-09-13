using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;

namespace SFA.DAS.LevyTransferMatching.Functions.TestHarness;

internal class Program
{
    private const string EndpointName = "SFA.DAS.LevyTransferMatching.MessageHandlers";
    private const string ConfigName = "SFA.DAS.LevyTransferMatching.Functions";

    public static async Task Main()
    {
        var builder = new ConfigurationBuilder()
            .AddAzureTableStorage(ConfigName);

        var configuration = builder.Build();

        var endpointConfiguration = new EndpointConfiguration(EndpointName)
            .UseErrorQueue($"{EndpointName}-errors")
            .UseInstallers()
            .UseMessageConventions()
            .UseNewtonsoftJsonSerializer();

        var connString = configuration[$"{ConfigName}:LevyTransferMatchingFunctions:NServiceBusConnectionString"];

        if (connString == "UseDevelopmentStorage=true")
        {
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            transport.StorageDirectory(
                Path.Combine(
                    Directory.GetCurrentDirectory()
                        .Substring(0, Directory.GetCurrentDirectory().IndexOf("src")),
                    @"src\.learningtransport"));
            transport.Routing().RouteToEndpoint(typeof(CreatedAccountEvent), "SFA.DAS.LevyTransferMatching.CreatedAccount");
            transport.Routing().RouteToEndpoint(typeof(ChangedAccountNameEvent), "SFA.DAS.LevyTransferMatching.ChangedAccountNameEvent");
            transport.Routing().RouteToEndpoint(typeof(ApplicationApprovedEvent), "SFA.DAS.LevyTransferMatching.ApplicationApprovedEvent");
            transport.Routing().RouteToEndpoint(typeof(TransferRequestApprovedEvent), "SFA.DAS.LevyTransferMatching.TransferRequestApprovedEvent");
            transport.Routing().RouteToEndpoint(typeof(ApplicationCreatedEvent), "SFA.DAS.LevyTransferMatching.ApplicationCreatedEvent");
        }
        else
        {
            //var transport = endpointConfiguration();
            //transport.CustomTokenProvider(TokenProvider.CreateManagedServiceIdentityTokenProvider());
            //transport.Routing().RouteToEndpoint(typeof(CreatedAccountEvent), "SFA.DAS.LevyTransferMatching.CreatedAccount");
            //transport.Routing().RouteToEndpoint(typeof(ChangedAccountNameEvent), "SFA.DAS.LevyTransferMatching.ChangedAccountNameEvent");
            //transport.Routing().RouteToEndpoint(typeof(ApplicationApprovedEvent), "SFA.DAS.LevyTransferMatching.ApplicationApprovedEvent");
            //transport.Routing().RouteToEndpoint(typeof(TransferRequestApprovedEvent), "SFA.DAS.LevyTransferMatching.TransferRequestApprovedEvent");
            //transport.Routing().RouteToEndpoint(typeof(ApplicationCreatedEvent), "SFA.DAS.LevyTransferMatching.ApplicationCreatedEvent");
        }

        var endpoint = await Endpoint.Start(endpointConfiguration);
        var testHarness = new TestHarness(endpoint);

        await testHarness.Run();
        await endpoint.Stop();
    }
}