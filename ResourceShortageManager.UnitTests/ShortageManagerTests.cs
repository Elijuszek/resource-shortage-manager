using ResourceShortageManager.Models;
using ResourceShortageManager.Services;
using ResourceShortageManager.Utilities;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace ResourceShortageManager.UnitTests;

public class ShortageManagerTests : IDisposable
{
    private const string _currUser = "testUser";
    private const string _filePath = "testShortagesManager.json";
    private readonly FileManager _fileManager;
    private ShorageManager _shortageManager;
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        Converters =
        {
            new JsonStringEnumConverter()
        },
        WriteIndented = true
    };

    private readonly TextReader _originalInput = Console.In;

    public ShortageManagerTests()
    {
        _fileManager = new FileManager();
        _shortageManager = new ShorageManager(_currUser, _filePath);
    }

    [Fact]
    public void RegisterShortage_ValidInput_AddsShortage()
    {
        // Arrange
        var title = "Shortage1";
        var room = Room.MeetingRoom;
        var category = Category.Electronics;
        var priority = 1;

        // Simulate user input
        var input = new StringReader($"{title}\n{(int)room}\n{(int)category}\n{priority}\n");
        Console.SetIn(input);

        // Act
        var result = _shortageManager.AddShortage();

        // Assert
        Assert.Equal(Status.AddedSuccessfully, result);
        var shortages = _fileManager.DeserializeShortages(_filePath);
        var key = Shortage.MakeKey(title, room);
        Assert.True(shortages.ContainsKey(key));
        Assert.Equal(priority, shortages[key].Priority);
    }

    [Fact]
    public void DeleteShortage_ValidInput_DeletesShortage()
    {
        // Arrange
        var title = "Shortage2";
        var room = Room.Kitchen;
        var category = Category.Other;
        var priority = 9;

        var shortages = new Dictionary<string, Shortage>
        {
        { title.ToLower() + room.ToString().ToLower(), new Shortage()
            {
                Title = title,
                Name = _currUser,
                Room = room,
                Category = category,
                Priority = priority,
                CreatedOn = new DateTime(2021, 2, 1)
                }
            }
        };

        string json = JsonSerializer.Serialize(shortages, _serializerOptions);

        using (var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        using (var writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }
        _shortageManager = new ShorageManager(_currUser, _filePath);

        // Simulate user input
        var input = new StringReader($"{title}\n{(int)room}\n");
        Console.SetIn(input);

        // Act
        var result = _shortageManager.RemoveShortage();

        // Assert
        Assert.Equal(Status.RemovedSuccessfully, result);
        shortages = _fileManager.DeserializeShortages(_filePath);
        var key = Shortage.MakeKey(title, room);
        Assert.False(shortages.ContainsKey(key));
    }

    [Fact]
    public void RegisterShortage_EmptyTitle_ReturnsNone()
    {
        // Arrange
        var title = "";
        var room = Room.MeetingRoom;
        var category = Category.Electronics;
        var priority = 1;

        // Simulate user input
        var input = new StringReader($"{title}\n{(int)room}\n{(int)category}\n{priority}\n");
        Console.SetIn(input);

        // Act
        var result = _shortageManager.AddShortage();

        // Assert
        Assert.Equal(Status.None, result);
        var shortages = _fileManager.DeserializeShortages(_filePath);
        Assert.Empty(shortages);
    }

    [Fact]
    public void RegisterShortage_CancelTitle_ReturnsNone()
    {
        // Arrange
        var title = "cancel";
        var room = Room.MeetingRoom;
        var category = Category.Electronics;
        var priority = 1;

        // Simulate user input
        var input = new StringReader($"{title}\n{(int)room}\n{(int)category}\n{priority}\n");
        Console.SetIn(input);

        // Act
        var result = _shortageManager.AddShortage();

        // Assert
        Assert.Equal(Status.None, result);
        var shortages = _fileManager.DeserializeShortages(_filePath);
        Assert.Empty(shortages);
    }
    [Fact]
    public void RegisterShortage_InvalidRoom_ReturnsNone()
    {
        // Arrange
        var title = "Shortage3";
        var room = Room.None;
        var category = Category.Electronics;
        var priority = 1;

        // Simulate user input
        var input = new StringReader($"{title}\n{(int)room}\n{(int)category}\n{priority}\n");
        Console.SetIn(input);

        // Act
        var result = _shortageManager.AddShortage();

        // Assert
        Assert.Equal(Status.None, result);
        var shortages = _fileManager.DeserializeShortages(_filePath);
        Assert.Empty(shortages);
    }

    [Fact]
    public void RegisterShortage_InvalidCategory_ReturnsNone()
    {
        // Arrange
        var title = "Shortage4";
        var room = Room.MeetingRoom;
        var category = Category.None;
        var priority = 1;

        // Simulate user input
        var input = new StringReader($"{title}\n{(int)room}\n{(int)category}\n{priority}\n");
        Console.SetIn(input);

        // Act
        var result = _shortageManager.AddShortage();

        // Assert
        Assert.Equal(Status.None, result);
        var shortages = _fileManager.DeserializeShortages(_filePath);
        Assert.Empty(shortages);
    }

    [Fact]
    public void RegisterShortage_InvalidPriority_ReturnsNone()
    {
        // Arrange
        var title = "Shortage5";
        var room = Room.MeetingRoom;
        var category = Category.Electronics;
        var priority = -1;

        // Simulate user input
        var input = new StringReader($"{title}\n{(int)room}\n{(int)category}\n{priority}\n");
        Console.SetIn(input);

        // Act
        var result = _shortageManager.AddShortage();

        // Assert
        Assert.Equal(Status.None, result);
        var shortages = _fileManager.DeserializeShortages(_filePath);
        Assert.Empty(shortages);
    }

    [Fact]
    public void RegisterShortage_DuplicateShortageWithHigherOrEqualPriority_ReturnsAlreadyExists()
    {
        // Arrange
        var title = "Shortage6";
        var room = Room.MeetingRoom;
        var category = Category.Electronics;
        var existingPriority = 5;
        var newPriority = 5;

        var existingShortage = new Shortage
        {
            Title = title,
            Name = _currUser,
            Room = room,
            Category = category,
            Priority = existingPriority,
            CreatedOn = DateTime.Now
        };
        var shortages = new Dictionary<string, Shortage>
        {
            { existingShortage.MakeKey(), existingShortage }
        };
        _fileManager.SerializeShortages(_filePath, shortages);
        _shortageManager = new ShorageManager(_currUser, _filePath);

        // Simulate user input
        var input = new StringReader($"{title}\n{(int)room}\n{(int)category}\n{newPriority}\n");
        Console.SetIn(input);

        // Act
        var result = _shortageManager.AddShortage();

        // Assert
        Assert.Equal(Status.AlreadyExists, result);
        var updatedShortages = _fileManager.DeserializeShortages(_filePath);
        var key = Shortage.MakeKey(title, room);
        Assert.True(updatedShortages.ContainsKey(key));
        Assert.Equal(existingPriority, updatedShortages[key].Priority);
    }

    [Fact]
    public void RemoveShortage_NonExistingShortage_ReturnsDoesNotExist()
    {
        // Arrange
        var title = "NonExistingShortage";
        var room = Room.Kitchen;

        var shortages = _fileManager.DeserializeShortages(_filePath);
        var key = Shortage.MakeKey(title, room);
        Assert.False(shortages.ContainsKey(key));

        // Simulate user input
        var input = new StringReader($"{title}\n{(int)room}\n");
        Console.SetIn(input);

        // Act
        var result = _shortageManager.RemoveShortage();

        // Assert
        Assert.Equal(Status.DoesNotExist, result);
    }

    public void Dispose()
    {
        Console.SetIn(_originalInput);
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }
}
