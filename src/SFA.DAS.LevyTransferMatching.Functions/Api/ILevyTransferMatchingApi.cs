using System.Threading.Tasks;
using RestEase;

namespace SFA.DAS.LevyTransferMatching.Functions.Api
{
    public interface ILevyTransferMatchingApi
    {
        [Post("functions/application-approved")]
        Task ApplicationApproved(ApplicationApprovedRequest request);
    }
}
