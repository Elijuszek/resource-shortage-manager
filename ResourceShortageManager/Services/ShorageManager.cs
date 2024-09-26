using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourceShortageManager.Models;
using ResourceShortageManager.Utilities;

namespace ResourceShortageManager
{
    class ShorageManager
    {
        private readonly string _currentUser;
        private Dictionary<ShortageKey, Shortage> _shortages;

        public ShorageManager(string username, string filePath)
        {
            _currentUser = username;
            _shortages = FileManager.DeserializeShortages(filePath);
        }
    }
}
