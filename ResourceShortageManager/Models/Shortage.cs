using System.Text.RegularExpressions;

namespace ResourceShortageManager.Models

{
    public class Shortage
    {
        public required string Title { get; set; }
        public required string Name { get; set; }
        public required Room Room { get; set; }
        public required Category Category { get; set; }
        public required int Priority { get; set; }
        public required DateTime CreatedOn { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return string.Format("| {0, -25} | {1, -10} | {2, -15} | {3, -12} | {4, -8} | {5, -25} |",
                Title, Name, Room, Category, Priority, CreatedOn.ToString("yyyy-MM-dd h:mm:ss tt"));
        }

        public string MakeKey()
        {
            return NormalizeString(Title) + Room.ToString().ToLower();
        }
        public static string MakeKey(string title, Room room)
        {
            return new string(NormalizeString(title) + room.ToString().ToLower());
        }
        public static string NormalizeString(string str)
        {
            return Regex.Replace(str.ToLower().Trim(), @"\s+", "");
        }
    }
}
