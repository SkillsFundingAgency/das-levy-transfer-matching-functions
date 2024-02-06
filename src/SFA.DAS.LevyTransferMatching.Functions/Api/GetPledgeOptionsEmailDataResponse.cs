namespace SFA.DAS.LevyTransferMatching.Functions.Api;

public class GetPledgeOptionsEmailDataResponse
{
    public List<EmailData> EmailDataList { get; set; }

    public class EmailData
    {
        public string RecipientEmailAddress { get; set; }
        public string EmployerName { get; set; }
        public long AccountId { get; set; }
        public string FinancialYearStart { get; set; }
        public string FinancialYearEnd { get; set; }
    }
}