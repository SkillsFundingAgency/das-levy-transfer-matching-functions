using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure
{
    public static class QueueNames
    {
        public const string CreatedAccount = "SFA.DAS.LevyTransferMatching.CreatedAccount";
        public const string ChangedAccountName = "SFA.DAS.LevyTransferMatching.ChangedAccountName";
        public const string RunHealthCheck = "SFA.DAS.LevyTransferMatching.HealthCheck";
        public const string ApplicationApprovedEvent = "SFA.DAS.LevyTransferMatching.ApplicationApproved";
        public const string PledgeDebitFailed = "SFA.DAS.LevyTransferMatching.PledgeDebitFailed";
    }
}