using System;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;

[assembly: ExtensionInformation("SFA.DAS.NServiceBus.AzureFunction", "17.0.49")]

// https://blog.maartenballiauw.be/post/2021/06/01/custom-bindings-with-azure-functions-dotnet-isolated-worker.html#:~:text=If%20you%27re%20building%20workloads,logic%20in%20your%20function%20instead.
namespace SFA.DAS.LevyTransferMatching.Functions.Bindings
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class NServiceBusTriggerOutputAttribute : OutputBindingAttribute
    {
        public string Endpoint { get; set; }
        public string Connection { get; set; }
        public string LearningTransportStorageDirectory { get; set; }
    }
}