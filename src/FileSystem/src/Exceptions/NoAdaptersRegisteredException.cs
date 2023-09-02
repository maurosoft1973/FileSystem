namespace Maurosoft.FileSystem.Exceptions
{
    public class NoAdaptersRegisteredException : FileSystemException
    {
        public NoAdaptersRegisteredException() : base("No adapters registered with the file system.")
        {
        }
    }
}