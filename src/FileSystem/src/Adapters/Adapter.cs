using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Maurosoft.FileSystem.Models;
using Serilog;
using DirectoryNotFoundException = Maurosoft.FileSystem.Exceptions.DirectoryNotFoundException;
using FileNotFoundException = Maurosoft.FileSystem.Exceptions.FileNotFoundException;

namespace Maurosoft.FileSystem.Adapters
{
    public abstract class Adapter : IAdapter
    {
        public string Prefix { get; }
        public string RootPath { get; }
        public ILogger? Logger { get; protected set; }

        protected Adapter(string prefix, string rootPath)
        {
            Prefix = prefix;
            RootPath = rootPath;
        }

        protected string PrependRootPath(string path) => Path.Combine(RootPath, path);

        protected string[] GetPathParts(string path) => path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

        protected string GetLastPathPart(string path) => GetPathParts(path).Length > 0 ? GetPathParts(path).Last() : String.Empty;

        protected string GetParentPathPart(string path)
        {
            var pathParts = GetPathParts(path);

            return string.Join("/", pathParts.Take(pathParts.Length - 1));
        }

        public IFile GetFile(string path) => GetFileAsync(path).Result;

        public IDirectory GetDirectory(string path) => GetDirectoryAsync(path).Result;

        public IEnumerable<IFile> GetFiles(string path = "") => GetFilesAsync(path).Result;

        public IEnumerable<IDirectory> GetDirectories(string path = "") => GetDirectoriesAsync(path).Result;

        public bool FileExists(string path) => FileExistsAsync(path).Result;

        public async Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                await GetFileAsync(path, cancellationToken);
            }
            catch (FileNotFoundException)
            {
                return false;
            }

            return true;
        }

        public bool DirectoryExists(string path) => DirectoryExistsAsync(path).Result;

        public async Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                await GetDirectoryAsync(path, cancellationToken);
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }

            return true;
        }

        public void CreateDirectory(string path) => CreateDirectoryAsync(path).Wait();

        public void DeleteDirectory(string path) => DeleteDirectoryAsync(path).Wait();

        public void DeleteFile(string path) => DeleteFileAsync(path).Wait();

        public byte[] ReadFile(string path)
        {
            try
            {
                return ReadFileAsync(path).Result;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public string ReadTextFile(string path)
        {
            try
            {
                return ReadTextFileAsync(path).Result;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public void WriteFile(string path, byte[] contents, bool overwrite = false) => WriteFileAsync(path, contents, overwrite).Wait();

        public void WriteFile(string path, string contents, bool overwrite = false) => WriteFileAsync(path, contents, overwrite).Wait();

        public async Task WriteFileAsync(string path, string contents, bool overwrite = false, CancellationToken cancellationToken = default) => await WriteFileAsync(path, Encoding.UTF8.GetBytes(contents), overwrite, cancellationToken);

        public void AppendFile(string path, byte[] contents) => AppendFileAsync(path, contents).Wait();

        public void AppendFile(string path, string contents) => AppendFileAsync(path, contents).Wait();

        public async Task AppendFileAsync(string path, string contents, CancellationToken cancellationToken = default) => await AppendFileAsync(path, Encoding.UTF8.GetBytes(contents), cancellationToken);

        public abstract void Dispose();
        public abstract void Connect();
        public abstract Task<IFile> GetFileAsync(string path, CancellationToken cancellationToken = default);
        public abstract Task<IDirectory> GetDirectoryAsync(string path, CancellationToken cancellationToken = default);
        public abstract Task<IEnumerable<IFile>> GetFilesAsync(string path = "", CancellationToken cancellationToken = default);
        public abstract Task<IEnumerable<IDirectory>> GetDirectoriesAsync(string path = "", CancellationToken cancellationToken = default);
        public abstract Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);
        public abstract Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken = default);
        public abstract Task DeleteFileAsync(string path, CancellationToken cancellationToken = default);
        public abstract Task<byte[]> ReadFileAsync(string path, CancellationToken cancellationToken = default);
        public abstract Task<string> ReadTextFileAsync(string path, CancellationToken cancellationToken = default);
        public abstract Task WriteFileAsync(string path, byte[] contents, bool overwrite = false, CancellationToken cancellationToken = default);
        public abstract Task AppendFileAsync(string path, byte[] contents, CancellationToken cancellationToken = default);
    }
}