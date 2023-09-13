namespace Maurosoft.FileSystem.Exceptions
{
    public class DirectoryNotFoundException : FileSystemException
    {
        public string Path { get; }
        public string Prefix { get; }

        public DirectoryNotFoundException(string path, string prefix) : base($"Directory '{path}' not found in adapter with prefix '{prefix}'.")
        {
            Path = path;
            Prefix = prefix;
        }
    }
}