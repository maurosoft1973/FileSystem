using System;

namespace Maurosoft.FileSystem.Exceptions
{
    public abstract class FileSystemException : Exception
    {
        protected FileSystemException(string message) : base(message)
        {
        }

        protected FileSystemException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}