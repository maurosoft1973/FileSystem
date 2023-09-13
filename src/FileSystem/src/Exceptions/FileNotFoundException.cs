namespace Maurosoft.FileSystem.Exceptions
{
    public class FileNotFoundException : FileSystemException
    {
        public string Path { get; }
        public string Prefix { get; }

        public FileNotFoundException(string path, string prefix) : base($"File '{path}' not found in adapter with prefix '{prefix}'.")
        {
            Path = path;
            Prefix = prefix;
        }
    }
}