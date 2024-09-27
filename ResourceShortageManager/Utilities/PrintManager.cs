using ResourceShortageManager.Models;
using System.Globalization;

namespace ResourceShortageManager.Utilities
{
    public static class PrintManager
    {
        public static string PromptInput(string message = "")
        {
            string? input;
            do
            {
                Console.WriteLine($"{message} (or type 'exit' or 'cancel' to cancel):");
                input = Console.ReadLine();
                if (input?.ToLower() is "exit" or "cancel" or null)
                    return "cancel";

            } while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        public static Category PromptCategory(string message)
        {
            string? input;
            HashSet<string> validInputs = new HashSet<string>
            {
                "1", "2", "3",
                "Electronics", "Food", "Other",
                "cancel"
            };

            do
            {
                Console.Clear();
                Console.WriteLine(message);
                Console.WriteLine("1. Electronics");
                Console.WriteLine("2. Food");
                Console.WriteLine("3. Other");
                input = PromptInput();

            } while (!validInputs.Contains(input));

            return input.ToLower() switch
            {
                "cancel" => Category.None,
                "1" or "electronics" => Category.Electronics,
                "2" or "food" => Category.Food,
                _ => Category.Other
            };
        }


        public static Room PromptRoom(string message)
        {
            string input;
            HashSet<string> validInputs = new HashSet<string>
            {
                "1", "2", "3",
                "Meeting Room", "Bathroom", "Kitchen",
                "cancel"
            };

            do
            {
                Console.Clear();
                Console.WriteLine(message);
                Console.WriteLine("1. Meeting Room");
                Console.WriteLine("2. Bathroom");
                Console.WriteLine("3. Kitchen");
                input = PromptInput();

            } while (!validInputs.Contains(input));

            return input.ToLower() switch
            {
                "cancel" => Room.None,
                "1" or "meeting room" => Room.MeetingRoom,
                "2" or "bathroom" => Room.Bathroom,
                _ => Room.Kitchen
            };
        }

        public static int PromptInt(string message, int? min = null, int? max = null)
        {
            string? input;
            int number;

            do
            {
                Console.Clear();
                Console.WriteLine(message);
                input = PromptInput();

                if (input is null || input.ToLower() == "cancel")
                    return -1;

            } while (!int.TryParse(input, out number) || (min.HasValue && number < min) || (max.HasValue && number > max));

            return number;
        }

        public static DateTime? PromptDateTime(string message = "")
        {
            string? input;
            DateTime parsedDateTime;

            do
            {
                Console.WriteLine($"{message} (or type 'exit' or 'cancel' to cancel):");
                input = Console.ReadLine();

                if (input?.ToLower() is "exit" or "cancel" or null)
                    return null;

            } while (string.IsNullOrWhiteSpace(input) || !TryParseDateTime(input, out parsedDateTime));

            return parsedDateTime;
        }

        private static bool TryParseDateTime(string input, out DateTime parsedDateTime)
        {
            string[] formats =
            {
                "yyyy",
                "yyyy-MM",
                "yyyy-MM-dd",
                "yyyy-MM-dd-HH",
                "yyyy-MM-dd-HH-mm"
            };

            return DateTime.TryParseExact(input, formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out parsedDateTime);
        }
        public static void PrintTitles()
        {
            string header = string.Format("| {0, -25} | {1, -10} | {2, -15} | {3, -12} | {4, -8} | {5, -25} |",
                "Title", "Name", "Room", "Category", "Priority", "Created On");

            string separator = new string('-', header.Length);
            Console.WriteLine(separator);
            Console.WriteLine(header);
            Console.WriteLine(separator);
        }
        public static void PrintShortagesList(Dictionary<ShortageKey, Shortage> shortages)
        {
            Console.WriteLine("Registered shortages:");

            if (shortages.Count == 0)
            {
                PrintWarning("No shortages found.");
                return;
            }
            PrintTitles();
            foreach (Shortage shortage in shortages.Values)
            {
                Console.WriteLine(shortage.ToString());
            }
            Console.WriteLine();
        }

        public static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void PrintWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
