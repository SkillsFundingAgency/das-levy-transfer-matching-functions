using RestEase;

namespace SFA.DAS.LevyTransferMatching.Functions.Api;

public interface ILevyTransferMatchingApi
{
    [Post("functions/application-approved")]
    Task ApplicationApproved([Body]ApplicationApprovedRequest request);

    [Post("functions/application-approved-receiver-notification")]
    Task ApplicationApprovedEmail([Body] ApplicationApprovedEmailRequest request);

    [Post("functions/pledge-debit-failed")]
    Task PledgeDebitFailed([Body] PledgeDebitFailedRequest request);

    [Post("functions/debit-application")]
    Task DebitApplication([Body] TransferRequestApprovedRequest request);

        [Post("functions/application-funding-declined")]
        Task ApplicationFundingDeclined([Body] ApplicationFundingDeclinedRequest request);  
        
        [Post("functions/reject-pledge-applications")]
        Task RejectPledgeApplications([Body] RejectPledgeApplicationsRequest request);

        [Get("functions/get-pending-application-email-data")]
        Task<GetPendingApplicationEmailDataResponse> GetPendingApplicationEmailData();

    [Post("functions/send-emails")]
    Task SendEmails([Body] SendEmailsRequest request);

    [Post("functions/recalculate-application-cost-projections")]
    Task RecalculateApplicationCostProjections();

    [Get("functions/get-pledge-options-email-data")]
    Task<GetPledgeOptionsEmailDataResponse> GetPledgeOptionsEmailData();

    [Post("functions/application-withdrawn-after-acceptance")]
    Task ApplicationWithdrawnAfterAcceptance([Body] ApplicationWithdrawnAfterAcceptanceRequest request);

    [Get("functions/applications-for-auto-approval")]
    Task<GetApplicationsForAutomaticApprovalResponse> GetApplicationsForAutomaticApproval([Query] int? pledgeId = null);

    [Get("functions/applications-for-auto-rejection")]
    Task<GetApplicationsForAutomaticRejectionResponse> GetApplicationsForAutomaticRejection();

    [Post("functions/approve-application")]
    Task ApproveApplication([Body] ApproveApplicationRequest request);

    [Post("functions/reject-application")]
    Task RejectApplication([Body] RejectApplicationRequest request);

    [Post("functions/application-created-immediate-auto-approval")]
    Task ApplicationCreatedForImmediateAutoApproval([Body] ApplicationCreatedForImmediateAutoApprovalRequest request);

    [Post("functions/application-created-receiver-notification")]
    Task ApplicationCreatedEmail([Body] ApplicationCreatedEmailRequest request);

    [Post("functions/application-rejected-receiver-notification")]
    Task ApplicationRejectedEmail([Body] ApplicationRejectedEmailRequest request);
}