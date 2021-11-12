namespace SFA.DAS.LevyTransferMatching.Infrastructure
{
    public static class QueueNames
    {
        public const string CreatedAccount = "SFA.DAS.LevyTransferMatching.CreatedAccount";
        public const string ChangedAccountName = "SFA.DAS.LevyTransferMatching.ChangedAccountName";
        public const string RunHealthCheck = "SFA.DAS.LevyTransferMatching.HealthCheck";
        public const string ApplicationApprovedEvent = "SFA.DAS.LevyTransferMatching.ApplicationApproved";
        public const string PledgeDebitFailed = "SFA.DAS.LevyTransferMatching.PledgeDebitFailed";
        public const string TransferRequestApprovedEvent = "SFA.DAS.LTM.TransferRequestApproved";
        public const string ApplicationFundingDeclined = "SFA.DAS.LTM.ApplicationFundingDeclined";
    }
}