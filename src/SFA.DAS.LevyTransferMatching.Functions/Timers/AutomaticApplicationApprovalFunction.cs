using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Timers
{
    public class AutomaticApplicationApprovalFunction
    {
        private readonly ILevyTransferMatchingApi _api;

        public AutomaticApplicationApprovalFunction(ILevyTransferMatchingApi api)
        {
            _api = api;
        }

        [FunctionName("RunApplicationsWithAutomaticApprovalFunction")]
        public async Task Run([TimerTrigger("0 3 * * *")] TimerInfo timer, ILogger log)
        {
            log.LogInformation($"Executing RunApplicationsWithAutomaticApprovalFunction");

            try
            {
                var applications = await _api.GetApplicationsForAutomaticApproval(new GetApplicationsForAutomaticApprovalRequest());

                foreach (var app in applications.Applications)
                {
                    await _api.ApproveApplication(new ApproveApplicationRequest { ApplicationId = app.Id, PledgeId = app.PledgeId });
                }
            }
            catch (ApiException ex)
            {
                log.LogError(ex, $"Error executing RunApplicationsWithAutomaticApprovalFunction");
                throw;
            }
        }
    }
}
