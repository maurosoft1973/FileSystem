using System;

namespace Maurosoft.FileSystem.Models
{
    public abstract class Model
    {
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
        public DateTime? LastModifiedDateTime {get; set; }
        public DateTime? CreatedDateTime {get; set; }
    }
}