using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using NServiceBus.Transport;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.Configuration;
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
                        .Substring(0, Directory.GetCurrentDirectory().IndexOf("src", StringComparison.OrdinalIgnoreCase)),
                    @"src\.learningtransport"));
            
            AddRouting(transport);
        }
        else
        {
            throw new NotImplementedException("Azure Service Bus transport is not implemented");
        }

        var endpoint = await Endpoint.Start(endpointConfiguration);
        var testHarness = new TestHarness(endpoint);

        await testHarness.Run();
        await endpoint.Stop();
    }

    private static void AddRouting<T>(TransportExtensions<T> transport) where T : TransportDefinition
    {
        transport.Routing().RouteToEndpoint(typeof(CreatedAccountEvent), "SFA.DAS.LevyTransferMatching.CreatedAccount");
        transport.Routing().RouteToEndpoint(typeof(ChangedAccountNameEvent), "SFA.DAS.LevyTransferMatching.ChangedAccountNameEvent");
        transport.Routing().RouteToEndpoint(typeof(ApplicationApprovedEvent), "SFA.DAS.LevyTransferMatching.ApplicationApprovedEvent");
        transport.Routing().RouteToEndpoint(typeof(TransferRequestApprovedEvent), "SFA.DAS.LevyTransferMatching.TransferRequestApprovedEvent");
        transport.Routing().RouteToEndpoint(typeof(ApplicationCreatedEvent), "SFA.DAS.LevyTransferMatching.ApplicationCreatedEvent");
    }
}