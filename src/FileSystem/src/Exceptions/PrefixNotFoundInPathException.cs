namespace Maurosoft.FileSystem.Exceptions
{
    public class PrefixNotFoundInPathException : FileSystemException
    {
        public string Path { get; }

        public PrefixNotFoundInPathException(string path) : base($"No prefix found in path '{path}'.")
        {
            Path = path;
        }
    }
}