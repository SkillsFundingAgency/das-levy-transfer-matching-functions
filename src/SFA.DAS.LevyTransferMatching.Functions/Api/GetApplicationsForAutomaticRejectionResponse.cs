using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Functions.Api
{
    public class GetApplicationsForAutomaticRejectionResponse
    {
        public IEnumerable<Application> Applications { get; set; }

        public class Application
        {
            public int Id { get; set; }
            public int PledgeId { get; set; }
        }
    }
}
