using System;

namespace StudentMessagingService 
{
    class Program
    {
        // Replace sender@example.com with your "From" address.
        // This address must be verified with Amazon SES.
        static readonly string senderAddress = "sender@example.com";

        static void Main(string[] args)
        {
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}