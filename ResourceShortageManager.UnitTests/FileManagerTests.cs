using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;
using ResourceShortageManager.Utilities;
using ResourceShortageManager.Models;
using ResourceShortageManager.Services;

namespace ResourceShortageManager.UnitTests
{
    public class FileManagerTests : IDisposable
    {
        private const string _filePath = "testFileManager.json";
        private readonly FileManager _fileManager;
        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter()
            },
            WriteIndented = true
        };
        private readonly TextReader _originalInput = Console.In;

        public FileManagerTests()
        {
            _fileManager = new FileManager();
        }

        [Fact]
        public void DeserializeShortages_FileDoesNotExist_ReturnsEmptyDictionary()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            var result = _fileManager.DeserializeShortages(_filePath);

            Assert.Empty(result);
        }

        [Fact]
        public void DeserializeShortages_ValidFile_ReturnsShortagesDictionary()
        {
            var shortages = new Dictionary<string, Shortage>
            {
                { "testshortageameetingroom", new Shortage()
                    {
                        Title = "TestShortageA",
                        Name = "TestNameA",
                        Room = Room.MeetingRoom,
                        Category = Category.Electronics,
                        Priority = 1,
                        CreatedOn = new DateTime(2021, 1, 1)
                    }
                },
                { "testshortagebbathroom", new Shortage()
                    {
                        Title = "TestShortageB",
                        Name = "TestNameB",
                        Room = Room.Bathroom,
                        Category = Category.Food,
                        Priority = 2,
                        CreatedOn = new DateTime(2021, 2, 1)
                    }
                }
            };

            string json = JsonSerializer.Serialize(shortages, _serializerOptions);

            // Write to file using FileStream
            using (var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.Write(json);
            }

            var result = _fileManager.DeserializeShortages(_filePath);

            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey("testshortageameetingroom"));
            Assert.True(result.ContainsKey("testshortagebbathroom"));
        }

        [Fact]
        public void SerializeShortages_EmptyDictionary_WritesEmptyObjectToFile()
        {
            // Arrange
            var shortages = new Dictionary<string, Shortage>();

            // Act
            _fileManager.SerializeShortages(_filePath, shortages);

            // Assert
            using (var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream))
            {
                var json = reader.ReadToEnd();
                Assert.Equal("{}", json);
            }
        }


        [Fact]
        public void SerializeShortages_FileOverwrite_ReplacesPreviousContent()
        {
            // Arrange
            var initialShortages = new Dictionary<string, Shortage>
            {
                { new string("InitialShortage" + Room.MeetingRoom), new Shortage
                    {
                        Title = "InitialShortage",
                        Name = "InitialName",
                        Room = Room.MeetingRoom,
                        Category = Category.Electronics,
                        Priority = 1,
                        CreatedOn = DateTime.Now
                    }
                }
            };
            _fileManager.SerializeShortages(_filePath, initialShortages);

            // Act
            var newShortages = new Dictionary<string, Shortage>
            {
                { new string("NewShortage" + Room.Bathroom), new Shortage
                    {
                        Title = "NewShortage",
                        Name = "NewName",
                        Room = Room.Bathroom,
                        Category = Category.Food,
                        Priority = 2,
                        CreatedOn = DateTime.Now
                    }
                }
            };
            _fileManager.SerializeShortages(_filePath, newShortages);

            // Assert
            using (var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream))
            {
                var json = reader.ReadToEnd();
                var deserializedShortages = JsonSerializer.Deserialize<Dictionary<string, Shortage>>(json, _serializerOptions);

                Assert.NotNull(deserializedShortages);
                Assert.Single(deserializedShortages);
                Assert.Contains(deserializedShortages, s => s.Value.Title == "NewShortage" && s.Value.Room == Room.Bathroom);
            }
        }

        [Fact]
        public void DeserializeShortages_CorruptFile_ThrowsJsonException()
        {
            // Arrange
            using (var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.Write("Invalid JSON Content");
            }

            // Act & Assert
            Assert.Throws<JsonException>(() => _fileManager.DeserializeShortages(_filePath));
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
}
