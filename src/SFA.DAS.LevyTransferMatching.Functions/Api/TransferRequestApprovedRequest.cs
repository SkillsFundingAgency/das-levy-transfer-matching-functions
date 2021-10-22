using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Functions.Api
{
    public class TransferRequestApprovedRequest
    {
        public int ApplicationId { get; set; }
        public int NumberOfApprentices { get; set; }
        public int Amount { get; set; }
    }
}
