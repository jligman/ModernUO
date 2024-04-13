using System;
using Server.Accounting;
using Server.Items;
using Server.Logging;
using System.IO;
using Org.BouncyCastle.Bcpg;
using Newtonsoft.Json;


namespace Server.Misc;

public static class AccountPrompt
{
    private static readonly ILogger logger = LogFactory.GetLogger(typeof(AccountPrompt));    

    public static void Initialize()
    {
        string configFile = Path.Combine("Configs", "account.cfg");
        try
        {
            if (!File.Exists(configFile)) { throw new Exception(); }
            string json = File.ReadAllText(configFile);
            AcctConfig acctConfig = JsonConvert.DeserializeObject<AcctConfig>(json);

            var a = new Account(acctConfig.User, acctConfig.Password, acctConfig.AccessLevel);

            logger.Information("Owner account created: {Username}", acctConfig.User);
            ServerAccess.AddProtectedAccount(a, true);
            File.Move(configFile, configFile + ".used");
        }
        catch
        {
            if (Accounts.Count == 0)
            {
                Console.WriteLine("This server has no accounts.");
                Console.Write("Do you want to create the owner account now? (y/n): ");

                var answer = ConsoleInputHandler.ReadLine();
                if (answer.InsensitiveEquals("y"))
                {
                    Console.WriteLine();

                    Console.Write("Username: ");
                    var username = ConsoleInputHandler.ReadLine();

                    Console.Write("Password: ");
                    var password = ConsoleInputHandler.ReadLine();

                    var a = new Account(username, password)
                    {
                        AccessLevel = AccessLevel.Owner
                    };

                    logger.Information("Owner account created: {Username}", username);
                    ServerAccess.AddProtectedAccount(a, true);
                }
                else
                {
                    logger.Warning("No owner account created.");
                }
            }            
        }
    }

    private class AcctConfig
    {
        public string User { get; set; }
        public string Password { get; set; }
        public int AccessLevel { get; set; }
    }
}
