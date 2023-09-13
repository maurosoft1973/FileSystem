namespace Maurosoft.FileSystem.Exceptions
{
    public class DirectoryExistsException : FileSystemException
    {
        public string Path { get; }
        public string Prefix { get; }

        public DirectoryExistsException(string path, string prefix) : base($"Directory at path '{path}' already exists in adapter with prefix '{prefix}'.")
        {
            Path = path;
            Prefix = prefix;
        }
    }
}