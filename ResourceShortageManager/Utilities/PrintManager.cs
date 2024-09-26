using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceShortageManager.Utilities
{
    public static class PrintManager
    {
        public static string? PromptUser(string message)
        {
            string? input;
            do
            {
                Console.WriteLine($"{message} (or type 'exit' or 'cancel' to cancel):");
                input = Console.ReadLine();
                if (input?.ToLower() is "exit" or "cancel")
                    return null;

            } while (string.IsNullOrWhiteSpace(input));

            return input;
        }
    }
}
