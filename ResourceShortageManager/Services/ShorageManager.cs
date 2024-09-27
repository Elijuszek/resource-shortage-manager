using ResourceShortageManager.Models;
using ResourceShortageManager.Utilities;

namespace ResourceShortageManager.Services;

class ShorageManager
{
    private readonly string _currentUser;
    private readonly string _pathJson;
    private Dictionary<ShortageKey, Shortage> _shortages;

    public ShorageManager(string username, string filePath)
    {
        _currentUser = username;
        _pathJson = filePath;
        _shortages = FileManager.DeserializeShortages(filePath);
    }

    public void Meniu()
    {
        Console.Clear();
        string? input = null;
        Status result = Status.None;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Resource Shortage Manager");

            // Check the result and print appropriate messages
            switch (result)
            {
                case Status.AddedSuccessfully:
                    PrintManager.PrintSuccess("Shortage added successfully.");
                    break;
                case Status.RemovedSuccessfully:
                    PrintManager.PrintSuccess("Shortage removed successfully.");
                    break;
                case Status.AlreadyExists:
                    PrintManager.PrintWarning("Shortage already exists with equal or higher priority!");
                    break;
                case Status.DoesNotExist:
                    PrintManager.PrintWarning("No such shortage exists.");
                    break;
                case Status.None:
                    break;
            }

            result = Status.None;

            Console.WriteLine("1. List Shortages");
            Console.WriteLine("2. Add Shortage");
            Console.WriteLine("3. Remove Shortage");
            Console.WriteLine("4. Exit");

            input = PrintManager.PromptInput("Enter your choice");

            switch (input)
            {
                case "1" or "list":
                    ListShortages();
                    break;
                case "2" or "add":
                    result = AddShortage();
                    break;
                case "3" or "remove":
                    result = RemoveShortage();
                    break;
                case "4" or "cancel":
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    break;
            }
        }
    }

    private Status AddShortage()
    {
        Console.Clear();
        Console.WriteLine("Add Shortage");

        // Get title and check if canceled
        string? title = PrintManager.PromptInput("Enter title ");
        if (string.IsNullOrWhiteSpace(title) || title.ToLower() == "cancel") return Status.None;

        // Get room and check if canceled
        Room room = PrintManager.PromptRoom("Enter room");
        if (room == Room.None) return Status.None;

        // Get category and check if canceled
        Category category = PrintManager.PromptCategory("Enter category");
        if (category == Category.None) return Status.None;

        // Get priority and check if canceled
        int priority = PrintManager.PromptInt("Enter priority");
        if (priority == -1) return Status.None;

        // Check if shortage with higher priority already exists
        ShortageKey key = new(title, room);
        if (_shortages.ContainsKey(key) && _shortages[key].Priority >= priority)
        {
            return Status.AlreadyExists;
        }

        // Add shortage
        Shortage shortage = new()
        {
            Title = title,
            Name = _currentUser,
            Room = room,
            Category = category,
            Priority = priority,
            CreatedOn = DateTime.Now
        };

        _shortages[key] = shortage;

        FileManager.SerializeShortages(_pathJson, _shortages);
        return Status.AddedSuccessfully;
    }

    private Status RemoveShortage()
    {
        Console.Clear();
        Console.WriteLine("Remove Shortage");

        // Get title and check if canceled
        string? title = PrintManager.PromptInput("Enter title ");
        if (string.IsNullOrWhiteSpace(title) || title.ToLower() == "cancel") return Status.None;

        // Get room and check if canceled
        Room room = PrintManager.PromptRoom("Enter room");
        if (room == Room.None) return Status.None;

        ShortageKey key = new(title, room);

        // Check if the shortage exists and currentUser can view it
        if (!_shortages.ContainsKey(key) || _shortages[key].Name != _currentUser
            && _currentUser.ToLower() != "admin")
        {
            return Status.DoesNotExist;
        }

        _shortages.Remove(key);
        FileManager.SerializeShortages(_pathJson, _shortages);
        return Status.RemovedSuccessfully;
    }

    public void ListShortages()
    {
        PrintManager.PrintShortagesList(_shortages);
    }
}
