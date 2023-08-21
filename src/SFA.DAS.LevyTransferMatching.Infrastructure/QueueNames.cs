namespace SFA.DAS.LevyTransferMatching.Infrastructure
{
    public static class QueueNames
    {
        public const string CreatedAccount = "SFA.DAS.LevyTransferMatching.CreatedAccount";
        public const string ChangedAccountName = "SFA.DAS.LevyTransferMatching.ChangedAccountName";
        public const string RunHealthCheck = "SFA.DAS.LevyTransferMatching.HealthCheck";
        public const string ApplicationApprovedEvent = "SFA.DAS.LevyTransferMatching.ApplicationApproved";
        public const string ApplicationApprovedEmail = "SFA.DAS.LTM.ApplicationApprovedEmail";
        public const string PledgeDebitFailed = "SFA.DAS.LevyTransferMatching.PledgeDebitFailed";
        public const string TransferRequestApprovedEvent = "SFA.DAS.LTM.TransferRequestApproved";
        public const string ApplicationFundingDeclined = "SFA.DAS.LTM.ApplicationFundingDeclined";
        public const string ApplicationCreated = "SFA.DAS.LTM.ApplicationCreated";
        public const string ApplicationCreatedForImmediateAutoApproval = "SFA.DAS.LTM.ApplicationCreatedForImmediateAutoApproval";
        public const string ApplicationWithdrawn = "SFA.DAS.LTM.ApplicationWithdrawn";
        public const string ApplicationRejected = "SFA.DAS.LTM.ApplicationRejected";
        public const string ApplicationApproved = "SFA.DAS.LTM.ApplicationApproved";
        public const string ApplicationApprovedForLegacyTopicPublishing = "SFA.DAS.LTM.ApplicationApprovedLegacy";
        public const string SendPendingApplicationEmails = "SFA.DAS.LevyTransferMatching.SendPendingApplicationEmails";
        public const string ApplicationWithdrawnAfterAcceptance = "SFA.DAS.LTM.ApplicationWithdrawnAfterAcceptance";
        public const string PledgeCredited = "SFA.DAS.LTM.PledgeCredited";
    }
}