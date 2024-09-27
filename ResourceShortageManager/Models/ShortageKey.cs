using System.Text.RegularExpressions;

namespace ResourceShortageManager.Models

{
    public class ShortageKey
    {
        public string Title { get; }
        public Room Room { get; }

        public ShortageKey(string title, Room category)
        {
            Title = NormalizeString(title);
            Room = category;
        }

        public override bool Equals(object? obj)
        {
            if (obj is ShortageKey otherKey)
            {
                return Title == otherKey.Title && Room == otherKey.Room;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Room);
        }

        private string NormalizeString(string str)
        {
            return Regex.Replace(str.ToLower().Trim(), @"\s+", " ");
        }
    }
}
