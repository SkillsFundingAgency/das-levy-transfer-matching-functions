namespace SFA.DAS.LevyTransferMatching.Functions.Api
{
    public class ApplicationRejectedEmailRequest
    {
        public int ApplicationId { get; set; }
        public int PledgeId { get; set; }
        public long ReceiverId { get; set; }
        public string BaseUrl { get; set; }
        public string EncodedApplicationId { get; set; }
    }
}
