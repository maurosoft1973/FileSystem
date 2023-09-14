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
        protected readonly ILogger Logger = Log.ForContext(typeof(Adapter));

        protected Adapter(string prefix, string rootPath)
        {
            Prefix = prefix;
            RootPath = rootPath;
        }

        public void AppendFile(string path, byte[] contents)
        {
            try
            {
                var task = Task.Run(async delegate
                {
                    await AppendFileAsync(path, contents);
                });

                task.Wait();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public void AppendFile(string path, string contents)
        {
            try
            {
                var task = Task.Run(async delegate
                {
                    await AppendFileAsync(path, contents);
                });

                task.Wait();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public abstract Task AppendFileAsync(string path, byte[] contents, CancellationToken cancellationToken = default);

        public async Task AppendFileAsync(string path, string contents, CancellationToken cancellationToken = default) => await AppendFileAsync(path, Encoding.UTF8.GetBytes(contents), cancellationToken);

        public abstract void Connect();

        public void CreateDirectory(string path)
        {
            try
            {
                CreateDirectoryAsync(path).Wait();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public abstract Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);

        public bool DirectoryExists(string path)
        {
            try
            {
                return DirectoryExistsAsync(path).Result;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

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

        public abstract void Disconnect();

        public void DeleteDirectory(string path)
        {
            try
            {
                DeleteDirectoryAsync(path).Wait();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public abstract Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken = default);

        public void DeleteFile(string path)
        {
            try
            {
                DeleteFileAsync(path).Wait();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public abstract Task DeleteFileAsync(string path, CancellationToken cancellationToken = default);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
            DisposeAdapter(disposing);
        }

        public abstract void DisposeAdapter(bool disposing);

        public bool FileExists(string path)
        {
            try
            {
                return FileExistsAsync(path).Result;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

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

        public IDirectory GetDirectory(string path)
        {
            try
            {
                return GetDirectoryAsync(path).Result;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public abstract Task<IDirectory> GetDirectoryAsync(string path, CancellationToken cancellationToken = default);

        public IEnumerable<IDirectory> GetDirectories(string path = "")
        {
            try
            {
                return GetDirectoriesAsync(path).Result;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public abstract Task<IEnumerable<IDirectory>> GetDirectoriesAsync(string path = "", CancellationToken cancellationToken = default);

        public IFile GetFile(string path)
        {
            try
            {
                return GetFileAsync(path).Result;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public abstract Task<IFile> GetFileAsync(string path, CancellationToken cancellationToken = default);

        public IEnumerable<IFile> GetFiles(string path = "")
        {
            try
            {
                return GetFilesAsync(path).Result;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public abstract Task<IEnumerable<IFile>> GetFilesAsync(string path = "", CancellationToken cancellationToken = default);

        public byte[] ReadFile(string path)
        {
            try
            {
                var task = Task.Run(async delegate
                {
                    return await ReadFileAsync(path);
                });

                return task.Result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw ex.InnerException;
            }
        }

        public abstract Task<byte[]> ReadFileAsync(string path, CancellationToken cancellationToken = default);

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

        public abstract Task<string> ReadTextFileAsync(string path, CancellationToken cancellationToken = default);

        public void WriteFile(string path, byte[] contents, bool overwrite = false)
        {
            try
            {
                WriteFileAsync(path, contents, overwrite).Wait();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public void WriteFile(string path, string contents, bool overwrite = false)
        {
            try
            {
                WriteFileAsync(path, contents, overwrite).Wait();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public async Task WriteFileAsync(string path, string contents, bool overwrite = false, CancellationToken cancellationToken = default) => await WriteFileAsync(path, Encoding.UTF8.GetBytes(contents), overwrite, cancellationToken);

        public abstract Task WriteFileAsync(string path, byte[] contents, bool overwrite = false, CancellationToken cancellationToken = default);

        protected string PrependRootPath(string path) => Path.Combine(RootPath, path);

        protected string[] GetPathParts(string path) => path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

        protected string GetLastPathPart(string path)
        {
            var pathParts = GetPathParts(path);

            if (pathParts.Length > 0)
                return pathParts[pathParts.Length - 1];
            else
                return String.Empty;
        }

        protected string GetParentPathPart(string path)
        {
            var pathParts = GetPathParts(path);

            return string.Join("/", pathParts.Take(pathParts.Length - 1));
        }
    }
}