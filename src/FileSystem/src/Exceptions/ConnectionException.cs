using System;

namespace Maurosoft.FileSystem.Exceptions
{
    public class ConnectionException : FileSystemException
    {
        public ConnectionException(Exception innerException) : base("A connection exception occured. See the inner exception for more details.", innerException)
        {
        }
    }
}