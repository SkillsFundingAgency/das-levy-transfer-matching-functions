using System.Threading.Tasks;
using RestEase;

namespace SFA.DAS.LevyTransferMatching.Functions.Api
{
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

        [Get("functions/get-pending-application-email-data")]
        Task<GetPendingApplicationEmailDataResponse> GetPendingApplicationEmailData();

        [Post("functions/send-emails")]
        Task SendEmails([Body] SendEmailsRequest request);

        [Post("functions/backfill-application-matching-criteria")]
        Task BackfillApplicationMatchingCriteria();
    }
}
