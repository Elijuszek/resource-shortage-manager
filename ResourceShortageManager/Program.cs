using ResourceShortageManager.Models;
using ResourceShortageManager.Utilities;
using ResourceShortageManager.Services;

namespace ResourceShortageManager;

class Program
{
    static void Main()
    {
        string path = @"..\..\..\shortages.json";
        Console.Clear();
        Console.WriteLine("Started Resource Shortage Manager");

        string? username = PrintManager.PromptUser("Enter your username");

        if (username is null)
        {
            Console.WriteLine("Exiting...");
            return;
        }

        ShorageManager manager = new(username, path);

    }
}