using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Maurosoft.FileSystem.Models;

namespace Maurosoft.FileSystem.Adapters
{
    public interface IAdapter : IDisposable
    {
        string Prefix { get; }
        string RootPath { get; }
        void AppendFile(string path, byte[] contents);
        void AppendFile(string path, string contents);
        Task AppendFileAsync(string path, string contents, CancellationToken cancellationToken = default);
        Task AppendFileAsync(string path, byte[] contents, CancellationToken cancellationToken = default);
        public void Connect();
        void CreateDirectory(string path);
        Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);
        void DeleteDirectory(string path);
        Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken = default);
        void DeleteFile(string path);
        Task DeleteFileAsync(string path, CancellationToken cancellationToken = default);
        bool DirectoryExists(string path);
        Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default);
        public void Disconnect();
        bool FileExists(string path);
        Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default);
        IDirectory GetDirectory(string path);
        Task<IDirectory> GetDirectoryAsync(string path, CancellationToken cancellationToken = default);
        IEnumerable<IDirectory> GetDirectories(string path = "");
        Task<IEnumerable<IDirectory>> GetDirectoriesAsync(string path = "", CancellationToken cancellationToken = default);
        IFile GetFile(string path);
        Task<IFile> GetFileAsync(string path, CancellationToken cancellationToken = default);
        IEnumerable<IFile> GetFiles(string path = "");
        Task<IEnumerable<IFile>> GetFilesAsync(string path = "", CancellationToken cancellationToken = default);
        byte[] ReadFile(string path);
        Task<byte[]> ReadFileAsync(string path, CancellationToken cancellationToken = default);
        string ReadTextFile(string path);
        Task<string> ReadTextFileAsync(string path, CancellationToken cancellationToken = default);
        void WriteFile(string path, byte[] contents, bool overwrite = false);
        void WriteFile(string path, string contents, bool overwrite = false);
        Task WriteFileAsync(string path, string contents, bool overwrite = false, CancellationToken cancellationToken = default);
        Task WriteFileAsync(string path, byte[] contents, bool overwrite = false, CancellationToken cancellationToken = default);
    }
}