using Maurosoft.FileSystem.Models;
using FluentFTP;

namespace Maurosoft.FileSystem.Adapters.Ftp
{
    public static class ModelFactory
    {
        public static IFile CreateFile(FtpListItem file)
        {
            return new FileModel
            {
                Name = file.Name,
                Path = file.FullName,
                Length = file.Size,
                LastModifiedDateTime = file.Modified
            };
        }

        public static DirectoryModel CreateDirectory(FtpListItem directory)
        {
            return new DirectoryModel
            {
                Name = directory.Name,
                Path = directory.FullName,
                LastModifiedDateTime = directory.Modified
            };
        }
    }
}