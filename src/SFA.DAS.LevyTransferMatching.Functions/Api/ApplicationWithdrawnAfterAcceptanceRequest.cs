﻿namespace SFA.DAS.LevyTransferMatching.Functions.Api;

public class ApplicationWithdrawnAfterAcceptanceRequest
{
    public int ApplicationId { get; set; }
    public int PledgeId { get; set; }
    public int Amount { get; set; }
}