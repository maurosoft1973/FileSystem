using Maurosoft.FileSystem.Adapters.Memory;
using Maurosoft.FileSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Maurosoft.FileSystem.Adapters.Memory
{
    public static class ModelFactory
    {
        public static FileModel CreateFile(MemoryFile file)
        {
            return new FileModel
            {
                Name = file.Name,
                Path = file.FullName,
                Length = file.Content.Length,
                LastModifiedDateTime = DateTime.Now,
            };
        }

        public static DirectoryModel CreateDirectory(MemoryDirectory directory)
        {
            return new DirectoryModel
            {
                Name = directory.Name,
                Path = directory.FullName,
                LastModifiedDateTime = DateTime.Now
            };
        }
    }
}
