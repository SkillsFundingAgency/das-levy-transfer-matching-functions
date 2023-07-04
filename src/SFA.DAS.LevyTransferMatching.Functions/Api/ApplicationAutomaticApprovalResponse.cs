using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Functions.Api
{
    public  class ApplicationAutomaticApprovalResponse
    {
        public IEnumerable<Application> Applications { get; set; }

        public class Application
        {
            public int Id { get; set; }
            public long EmployerAccountId { get; set; }
            public int PledgeId { get; set; }
            public int MatchPercentage { get; set; }
            public string AutoApproval { get; set; }
            public int TotalAmount { get; set; }
            public DateTime CreatedOn { get; set; }
            public string Status { get; set; }

        }
    }
}
