using System;

namespace odatatest
{
    public class SDKSample
    {
        static void Main(string[] args)
        {
            CommonProxy client = new CommonProxy();
            string key = DateTime.UtcNow.Ticks.ToString();
            client.CreateAccounts(2, key);
            //int countAct = 2;
            //int countCont = 2;
            //client.CreateAccountsWithContacts(countAct, countCont, key);
            //Console.WriteLine("Created {0} Accounts each having {1} contacts", countAct, countCont);
            Console.WriteLine("Press enter to exit"); 
            Console.ReadLine();
        }
    }
}
