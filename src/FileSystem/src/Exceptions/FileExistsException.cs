namespace Maurosoft.FileSystem.Exceptions
{
    public class FileExistsException : FileSystemException
    {
        public string Path { get; }
        public string Prefix { get; }

        public FileExistsException(string path, string prefix) : base($"File at path '{path}' already exists in adapter with prefix '{prefix}'.")
        {
            Path = path;
            Prefix = prefix;
        }
    }
}