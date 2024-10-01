using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.TestHarness;

public class TestHarness
{
    private readonly IMessageSession _publisher;

    public TestHarness(IMessageSession publisher)
    {
        _publisher = publisher;
    }

    public async Task Run()
    {
        const long accountId = 1001;
        var key = ConsoleKey.Escape;

        while (key != ConsoleKey.X)
        {
            Console.Clear();
            Console.WriteLine("Test Options");
            Console.WriteLine("------------");
            Console.WriteLine("A - CreateAccountEvent");
            Console.WriteLine("B - ChangedAccountNameEvent");
            Console.WriteLine("C - ApplicationApprovedEvent");
            Console.WriteLine("D - TransferRequestApprovedEvent");
            Console.WriteLine("E - ApplicationFundingDeclinedEvent");
            Console.WriteLine("F - ApplicationCreatedEvent");
            Console.WriteLine("G - ApplicationApprovedEmailEvent");
            Console.WriteLine("H - ApplicationWithdrawnAfterAcceptanceEvent");
            Console.WriteLine("I - PledgeCreditedEvent");
            Console.WriteLine("X - Exit");
            Console.WriteLine("Press [Key] for Test Option");
            key = Console.ReadKey().Key;

            try
            {
                switch (key)
                {
                    case ConsoleKey.A:
                        await _publisher.Publish(new CreatedAccountEvent { AccountId = accountId, Created = DateTime.Now, HashedId = "HPRIV", PublicHashedId = "PUBH", Name = "My Test", UserName = "Tester", UserRef = Guid.NewGuid() });
                        Console.WriteLine();
                        Console.WriteLine($"Published CreatedAccountEvent");
                        break;
                    case ConsoleKey.B:
                        await _publisher.Publish(new ChangedAccountNameEvent { AccountId = accountId, Created = DateTime.Now, CurrentName = "My Test new", PreviousName = "My Test", HashedAccountId = "PUBH", UserName = "Tester", UserRef = Guid.NewGuid() });
                        Console.WriteLine();
                        Console.WriteLine($"Published ChangedAccountNameEvent");
                        break;
                    case ConsoleKey.C:
                        await _publisher.Publish(new ApplicationApprovedEvent(1, 1, DateTime.UtcNow, 10000, 1, 1));
                        Console.WriteLine();
                        Console.WriteLine($"Published ApplicationApprovedEvent");
                        break;
                    case ConsoleKey.D:
                        await _publisher.Publish(new TransferRequestApprovedEvent(1, 1, DateTime.UtcNow, new UserInfo(), 1, 300, 2017));
                        Console.WriteLine();
                        Console.WriteLine($"Published TransferRequestApprovedEvent");
                        break;
                    case ConsoleKey.E:
                        await _publisher.Publish(new ApplicationFundingDeclinedEvent(1, 1, DateTime.UtcNow, 10000));
                        Console.WriteLine();
                        Console.WriteLine("Published ApplicationFundingDeclinedEvent");
                        break;
                    case ConsoleKey.F:
                        await _publisher.Publish(new ApplicationCreatedEvent(1,2, DateTime.UtcNow, 3, 1));
                        Console.WriteLine();
                        Console.WriteLine($"Published ApplicationCreatedEvent");
                        break;
                    case ConsoleKey.G:
                        await _publisher.Publish(new ApplicationApprovedEvent(1, 1,  DateTime.UtcNow,  8194, 8194, 1));
                        Console.WriteLine();
                        Console.WriteLine($"Published ApplicationApprovedEmailEvent");
                        break;
                    case ConsoleKey.H:
                        await _publisher.Publish(new ApplicationWithdrawnAfterAcceptanceEvent(1, 3, 800));
                        Console.WriteLine();
                        Console.WriteLine($"Published ApplicationWithdrawnAfterAcceptanceEvent");
                        break;
                    case ConsoleKey.I:
                        await _publisher.Publish(new PledgeCreditedEvent(1, 8));
                        Console.WriteLine();
                        Console.WriteLine($"Published PledgeCreditedEvent");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine();
            }

            if (key == ConsoleKey.X) break;

            Console.WriteLine();
            Console.WriteLine("Press any key to return to menu");
            Console.ReadKey();
        }
    }
}