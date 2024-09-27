using ResourceShortageManager.Models;
using ResourceShortageManager.Utilities;

namespace ResourceShortageManager.Services;

public class ShorageManager
{
    private readonly string _currentUser;
    private readonly string _pathJson;
    private Dictionary<ShortageKey, Shortage> _shortages;
    private FileManager _fileManager;

    public ShorageManager(string username, string filePath)
    {
        _currentUser = username;
        _pathJson = filePath;
        _fileManager = new FileManager();
        _shortages = _fileManager.DeserializeShortages(filePath);
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
                    Console.Clear();
                    ListShortages();
                    break;
                case "2" or "add":
                    Console.Clear();
                    result = AddShortage();
                    break;
                case "3" or "remove":
                    Console.Clear();
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

    public Status AddShortage()
    {
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

        _fileManager.SerializeShortages(_pathJson, _shortages);
        return Status.AddedSuccessfully;
    }

    public Status RemoveShortage()
    {
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
        _fileManager.SerializeShortages(_pathJson, _shortages);
        return Status.RemovedSuccessfully;
    }

    public void ListShortages()
    {
        string titleFilter = "";
        DateTime? startDateFilter = null;
        DateTime? endDateFilter = null;
        Room roomFilter = Room.None;
        Category categoryFilter = Category.None;

        var filteredShortages = _shortages.Where(s =>
                (s.Value.Name == _currentUser || _currentUser == "admin")
            ).OrderByDescending(s => s.Value.Priority).ToDictionary(s => s.Key, s => s.Value);

        while (true)
        {
            Console.Clear();
            PrintManager.PrintShortagesList(filteredShortages);

            Console.WriteLine("Choose a filter to apply:");
            Console.WriteLine("1. Title");
            Console.WriteLine("2. Start date");
            Console.WriteLine("3. End date");
            Console.WriteLine("4. Room");
            Console.WriteLine("5. Category");
            Console.WriteLine("6. Reset all filters");
            Console.WriteLine("7. Exit");

            string input = PrintManager.PromptInput("Enter your choice");

            switch (input)
            {
                case "1":
                    titleFilter = PrintManager.PromptInput("Enter filter title");
                    if (string.IsNullOrWhiteSpace(titleFilter) || titleFilter.ToLower() == "cancel")
                    {
                        continue;
                    }
                    break;
                case "2":
                    startDateFilter = PrintManager.PromptDateTime("Enter start date (year-month-day-hour-minutes)");
                    if (startDateFilter is null)
                    {
                        continue;
                    }
                    break;
                case "3":
                    endDateFilter = PrintManager.PromptDateTime("Enter end date (year-month-day-hour-minutes)");
                    if (endDateFilter is null)
                    {
                        continue;
                    }
                    break;
                case "4":
                    roomFilter = PrintManager.PromptRoom("Enter category");
                    if (roomFilter == Room.None)
                    {
                        continue;
                    }
                    break;
                case "5":
                    categoryFilter = PrintManager.PromptCategory("Enter category");
                    if (categoryFilter == Category.None)
                    {
                        continue;
                    }
                    break;
                case "6":
                    titleFilter = "";
                    startDateFilter = null;
                    endDateFilter = null;
                    roomFilter = Room.None;
                    categoryFilter = Category.None;
                    break;
                case "7" or "cancel":
                    return;
                default:
                    break;
            }

            filteredShortages = _shortages.Where(s =>
                (string.IsNullOrEmpty(titleFilter) || s.Value.Title.Contains(titleFilter, StringComparison.OrdinalIgnoreCase)) &&
                (categoryFilter == Category.None || s.Value.Category == categoryFilter) &&
                (roomFilter == Room.None || (s.Value.Room == roomFilter)) &&
                (!startDateFilter.HasValue || s.Value.CreatedOn >= startDateFilter.Value) &&
                (!endDateFilter.HasValue || s.Value.CreatedOn <= endDateFilter.Value) &&
                (s.Value.Name == _currentUser || _currentUser == "admin")
            ).OrderByDescending(s => s.Value.Priority).ToDictionary(s => s.Key, s => s.Value);
        }
    }
}
