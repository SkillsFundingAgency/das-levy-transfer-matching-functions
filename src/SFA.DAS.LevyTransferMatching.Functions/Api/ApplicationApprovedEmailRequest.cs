﻿namespace SFA.DAS.LevyTransferMatching.Functions.Api;

public class ApplicationApprovedEmailRequest
{
    public int ApplicationId { get; set; }
    public int PledgeId { get; set; }
    public long ReceiverId { get; set; }
    public string EncodedAccountId { get; set; }
    public string EncodedApplicationId { get; set; }
    public string BaseUrl { get; set; }
    public string UnsubscribeNotificationsUrl { get; set; }
}