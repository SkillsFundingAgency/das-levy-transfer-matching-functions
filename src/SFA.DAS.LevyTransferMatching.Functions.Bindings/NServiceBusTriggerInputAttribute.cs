using System;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;

// https://blog.maartenballiauw.be/post/2021/06/01/custom-bindings-with-azure-functions-dotnet-isolated-worker.html#:~:text=If%20you%27re%20building%20workloads,logic%20in%20your%20function%20instead

[assembly: ExtensionInformation("SFA.DAS.NServiceBus.AzureFunction", "17.0.49")]

namespace SFA.DAS.LevyTransferMatching.Functions.Bindings;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class NServiceBusTriggerInputAttribute : InputBindingAttribute
{
    public string Endpoint { get; set; }
}