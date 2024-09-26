using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ResourceShortageManager.Models

{
    public class ShortageKey
    {
        public string Title { get; }
        public Category Category { get; }

        public ShortageKey(string title, Category category)
        {
            Title = NormalizeString(title);
            Category = category;
        }

        public override bool Equals(object? obj)
        {
            if (obj is ShortageKey otherKey)
            {
                return Title == otherKey.Title && Category == otherKey.Category;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Category);
        }

        private string NormalizeString(string str)
        {
            return Regex.Replace(str.ToLower().Trim(), @"\s+", " ");
        }
    }
}
