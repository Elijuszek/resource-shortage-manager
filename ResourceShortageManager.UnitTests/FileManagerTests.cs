using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;
using ResourceShortageManager.Utilities;
using ResourceShortageManager.Models;
namespace ResourceShortageManager.UnitTests;

public class FileManagerTests
{

    private static JsonSerializerOptions serializerOptions = new()
    {
        Converters =
    {
        new JsonStringEnumConverter()
    },
        WriteIndented = true
    };

    private const string FilePath = "testShortages.json";

    [Fact]
    public void DeserializeShortages_FileDoesNotExist_ReturnsEmptyDictionary()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }

        var result = FileManager.DeserializeShortages(FilePath);

        Assert.Empty(result);
    }

    [Fact]
    public void DeserializeShortages_FileExistsButInvalidJson_ReturnsEmptyDictionary()
    {
        File.WriteAllText(FilePath, "invalid json");

        var result = FileManager.DeserializeShortages(FilePath);

        Assert.Empty(result);
    }

    [Fact]
    public void DeserializeShortages_ValidFile_ReturnsShortagesDictionary()
    {
        var shortagesList = new List<Shortage>
        {
            new Shortage()
            {
                Title = "Test Shortage A",
                Name = "Test Name A",
                Room = Room.MeetingRoom,
                Category = Category.Electronics,
                Priority = 1,
                CreatedOn = new DateTime(2021, 1, 1)
            },
            new Shortage()
            {
                Title = "Test Shortage B",
                Name = "Test Name B",
                Room = Room.Bathroom,
                Category = Category.Food,
                Priority = 2,
                CreatedOn = new DateTime(2021, 2, 1)
            }
        };

        string json = JsonSerializer.Serialize(shortagesList, new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = true
        });
        File.WriteAllText(FilePath, json);

        var result = FileManager.DeserializeShortages(FilePath);

        Assert.Equal(2, result.Count);
        Assert.True(result.ContainsKey(new ShortageKey("Test Shortage A", Category.Electronics)));
        Assert.True(result.ContainsKey(new ShortageKey("Test Shortage B", Category.Food)));
    }

    [Fact]
    public void SerializeShortages_ValidInput_WritesJsonToFile()
    {
        var shortages = new Dictionary<ShortageKey, Shortage>
    {
        { new ShortageKey("Test Shortage A", Category.Electronics), new Shortage()
            {
                Title = "Test Shortage A",
                Name = "Test Name A",
                Room = Room.MeetingRoom,
                Category = Category.Electronics,
                Priority = 1,
                CreatedOn = DateTime.Now
            }
        },
        { new ShortageKey("Test Shortage B", Category.Food), new Shortage()
            {
                Title = "Test Shortage B",
                Name = "Test Name B",
                Room = Room.Bathroom,
                Category = Category.Food,
                Priority = 2,
                CreatedOn = DateTime.Now
            }
        }
    };

        FileManager.SerializeShortages(FilePath, shortages);

        var json = File.ReadAllText(FilePath);

        var deserializedShortages = JsonSerializer.Deserialize<List<Shortage>>(json, serializerOptions);

        Assert.NotNull(deserializedShortages);
        Assert.Equal(2, deserializedShortages.Count);

        Assert.Contains(deserializedShortages, s => s.Title == "Test Shortage A" && s.Room == Room.MeetingRoom);
        Assert.Contains(deserializedShortages, s => s.Title == "Test Shortage B" && s.Room == Room.Bathroom);
    }


    public FileManagerTests()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
    }
}