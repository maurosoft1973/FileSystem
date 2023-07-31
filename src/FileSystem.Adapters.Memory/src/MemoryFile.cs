using System;
using System.Collections.Generic;
using System.Text;

namespace Maurosoft.FileSystem.Adapters.Memory
{
    public class MemoryFile
    {
        public byte[] Content { get; set; }
        public string Name { get; set; } = String.Empty;
        public string FullName { get; set; } = String.Empty;
        public string Directory { get; set; } = String.Empty;

        public MemoryFile()
            : this(new byte[0])
        {
        }

        public MemoryFile(byte[] content)
        {
            Content = content;
        }
    }
}
