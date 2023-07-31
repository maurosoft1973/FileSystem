using Maurosoft.FileSystem.Exceptions;
using Maurosoft.FileSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Maurosoft.FileSystem.Adapters.Memory
{
    public class MemoryAdapter : Adapter
    {
        private readonly IDictionary<string, MemoryDirectory> _directories = new Dictionary<string, MemoryDirectory>();
        private readonly IDictionary<string, MemoryFile> _files = new Dictionary<string, MemoryFile>();

        public MemoryAdapter(string prefix, string rootPath) : base(prefix, rootPath)
        {
            _directories.Add(System.IO.Path.Combine(rootPath), new MemoryDirectory() { FullName = rootPath, Name = GetLastPathPart(rootPath) });
        }

        public override void Dispose()
        {
        }

        public override void Connect()
        {
        }

        public override async Task<IFile> GetFileAsync(string path, CancellationToken cancellationToken = default)
        {
            path = PrependRootPath(path);

            try
            {
                var found = await Task.Run(() => _files.ContainsKey(path), cancellationToken);

                if (!found)
                    throw new FileNotFoundException(path, Prefix);

                _files.TryGetValue(path, out MemoryFile memoryFile);

                return ModelFactory.CreateFile(memoryFile);
            }
            catch (FileSystemException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new AdapterRuntimeException(exception);
            }
        }

        public override async Task<IDirectory> GetDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            path = PrependRootPath(path);

            try
            {
                var found = await Task.Run(() => _directories.ContainsKey(path), cancellationToken);

                if (!found)
                    throw new DirectoryNotFoundException(path, Prefix);

                _directories.TryGetValue(path, out MemoryDirectory memoryDirectory);

                return ModelFactory.CreateDirectory(memoryDirectory);
            }
            catch (FileSystemException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new AdapterRuntimeException(exception);
            }
        }

        public override async Task<IEnumerable<IFile>> GetFilesAsync(string path = "", CancellationToken cancellationToken = default)
        {
            path = PrependRootPath(path);
            var found = await Task.Run(() => _directories.ContainsKey(path), cancellationToken);

            if (!found)
                throw new DirectoryNotFoundException(path, Prefix);

            _directories.TryGetValue(path, out MemoryDirectory memoryDirectory);

            try
            {
                return await Task.Run(() => _files.Where(p => p.Value.Directory == memoryDirectory.FullName).Select(item => ModelFactory.CreateFile(item.Value)).AsEnumerable(), cancellationToken);
            }
            catch (Exception exception)
            {
                throw new AdapterRuntimeException(exception);
            }
        }

        public override async Task<IEnumerable<IDirectory>> GetDirectoriesAsync(string path = "", CancellationToken cancellationToken = default)
        {
            path = PrependRootPath(path);
            var found = await Task.Run(() => _directories.ContainsKey(path), cancellationToken);

            if (!found)
                throw new DirectoryNotFoundException(path, Prefix);

            _directories.TryGetValue(path, out MemoryDirectory memoryDirectory);

            try
            {
                return await Task.Run(() => _directories.Where(p => p.Key.StartsWith(path)).Select(p => ModelFactory.CreateDirectory(p.Value)).AsEnumerable(), cancellationToken);
            }
            catch (Exception exception)
            {
                throw new AdapterRuntimeException(exception);
            }
        }

        public override async Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            if (await DirectoryExistsAsync(path, cancellationToken))
                throw new DirectoryExistsException(PrependRootPath(path), Prefix);

            try
            {
                await Task.Run(() => _directories.Add(PrependRootPath(path), new MemoryDirectory() { Name = System.IO.Path.GetDirectoryName(PrependRootPath(path)), FullName = PrependRootPath(path) }), cancellationToken);
            }
            catch (Exception exception)
            {
                throw new AdapterRuntimeException(exception);
            }
        }

        public override async Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
        {
            await GetFileAsync(path, cancellationToken);
            await Task.Run(() => _files.Remove(PrependRootPath(path)), cancellationToken);
        }

        public override async Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            await GetDirectoryAsync(path, cancellationToken);
            await Task.Run(() => _directories.Remove(PrependRootPath(path), true), cancellationToken);
        }

        public override async Task<byte[]> ReadFileAsync(string path, CancellationToken cancellationToken = default)
        {
            await GetFileAsync(path, cancellationToken);

            MemoryFile memoryFile = new MemoryFile();

            await Task.Run(() => _files.TryGetValue(path, out MemoryFile memoryFile));

            return memoryFile.Content;
        }

        public override async Task<string> ReadTextFileAsync(string path, CancellationToken cancellationToken = default)
        {
            await GetFileAsync(path, cancellationToken);

            MemoryFile memoryFile = new MemoryFile();

            await Task.Run(() => _files.TryGetValue(path, out MemoryFile memoryFile));

            return System.Text.Encoding.UTF8.GetString(memoryFile.Content);
        }

        public override async Task WriteFileAsync(string path, byte[] contents, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            if (!overwrite && await FileExistsAsync(path, cancellationToken))
                throw new FileExistsException(PrependRootPath(path), Prefix);

            var found = _files.TryGetValue(PrependRootPath(path), out MemoryFile memoryFile);

            memoryFile ??= new MemoryFile
            {
                FullName = PrependRootPath(path),
                Directory = PrependRootPath(System.IO.Path.GetDirectoryName(path)).Replace(@"\", "/"),
                Name = System.IO.Path.GetFileName(path),
            };

            memoryFile.Content = contents;

            if (!found)
            {
                await Task.Run(() => _files.Add(PrependRootPath(path), memoryFile));
            }
        }

        public override async Task AppendFileAsync(string path, byte[] contents, CancellationToken cancellationToken = default)
        {
            await GetFileAsync(path, cancellationToken);

            MemoryFile memoryFile = new MemoryFile();

            await Task.Run(() => _files.TryGetValue(path, out MemoryFile memoryFile));

            memoryFile.Content = memoryFile.Content.Concat(contents).ToArray();
        }
    }
}
