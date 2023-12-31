using System;

namespace Maurosoft.FileSystem.Models
{
    public interface IFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public long? Length { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public byte[]? Content { get; set; }
    }
}