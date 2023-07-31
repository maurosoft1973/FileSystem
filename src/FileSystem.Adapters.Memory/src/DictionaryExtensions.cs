using Maurosoft.FileSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Maurosoft.FileSystem.Adapters.Memory
{
    public static class MemoryDirectoryExtensions
    {
        public static void Remove(this IDictionary<string, MemoryDirectory> dictionary, string key, bool recursive = false)
        {
            if (recursive)
                dictionary?.Where(e => e.Key.StartsWith(key)).ToList().ForEach(e => dictionary.Remove(e.Key));
        }

        public static void GetFiles(this IDictionary<string, MemoryFile> dictionary, string key, bool recursive = false)
        {
            if (recursive)
                dictionary?.Where(e => e.Key.StartsWith(key)).ToList().ForEach(e => dictionary.Remove(e.Key));
        }
    }
}
