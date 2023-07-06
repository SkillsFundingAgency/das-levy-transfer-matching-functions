using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands
{
    public class ApplicationAutomaticApprovalCommandHandler
    {
        private readonly ILevyTransferMatchingApi _api;

        public ApplicationAutomaticApprovalCommandHandler(ILevyTransferMatchingApi api)
        {
            _api = api;
        }

        [FunctionName("RunApplicationsWithAutomaticApprovalCommand")]
        public async Task Run([TimerTrigger("0 3 * * *")] TimerInfo timer, ILogger log)
        {
            log.LogInformation($"Handling ApplicationsWithAutomaticApprovalCommand handler");

            try
            {
                var applications = await _api.ApplicationsWithAutomaticApproval();

                foreach (var app in applications.Applications)
                {
                    await _api.ApproveAutomaticApplication(new ApproveAutomaticApplicationRequest { ApplicationId = app.Id, PledgeId = app.PledgeId });
                }
            }
            catch (ApiException ex)
            {
                log.LogError(ex, $"Error handling ApplicationsWithAutomaticApprovalCommand");
                throw;
            }
        }
    }
}
