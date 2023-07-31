using Renci.SshNet.Sftp;
using Maurosoft.FileSystem.Models;

namespace Maurosoft.FileSystem.Adapters.Sftp
{
    public static class ModelFactory
    {
        public static IFile CreateFile(ISftpFile file)
        {
            return new FileModel
            {
                Name = file.Name,
                Path = file.FullName,
                Length = file.Length,
                LastModifiedDateTime = file.LastWriteTime
            };
        }

        public static DirectoryModel CreateDirectory(ISftpFile directory)
        {
            return new DirectoryModel
            {
                Name = directory.Name,
                Path = directory.FullName,
                LastModifiedDateTime = directory.LastWriteTime
            };
        }
    }
}