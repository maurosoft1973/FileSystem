using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Common;
using Maurosoft.FileSystem.Exceptions;
using Maurosoft.FileSystem.Models;
using DirectoryNotFoundException = Maurosoft.FileSystem.Exceptions.DirectoryNotFoundException;
using FileNotFoundException = Maurosoft.FileSystem.Exceptions.FileNotFoundException;
using Renci.SshNet.Sftp;

namespace Maurosoft.FileSystem.Adapters.Sftp
{
    public class SftpAdapter : Adapter
    {
        private readonly ISftpClient client;

        public SftpAdapter(string prefix, string rootPath, ISftpClient client) : base(prefix, rootPath)
        {
            this.client = client;
        }

        public override async Task AppendFileAsync(string path, byte[] contents, CancellationToken cancellationToken = default)
        {
            await GetFileAsync(path, cancellationToken);

            try
            {
                var stringContents = Encoding.UTF8.GetString(contents, 0, contents.Length);

                var task = Task.Run(() =>
                {
                    try
                    {
                        client.AppendAllText(PathSftp(PrependRootPath(path)), stringContents);
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex, ex.Message);
                        throw;
                    }
                }, cancellationToken);

                await task;
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override void Connect()
        {
            try
            {
                if (!client.IsConnected)
                    client.Connect();

                Logger?.Information("{Adapter} - Connected successful", nameof(SftpAdapter));
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override async Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            if (await DirectoryExistsAsync(path, cancellationToken))
                throw new DirectoryExistsException(PathSftp(PrependRootPath(path)), Prefix);

            try
            {
                var task = Task.Run(() =>
                {
                    try
                    {
                        client.CreateDirectory(PathSftp(PrependRootPath(path)));
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex, ex.Message);
                        throw;
                    }
                });

                await task;
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override async Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            await GetDirectoryAsync(path, cancellationToken);

            try
            {
                var task = Task.Run(() =>
                {
                    try
                    {
                        client.DeleteDirectory(PathSftp(PrependRootPath(path)));
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex, ex.Message);
                        throw;
                    }
                }, cancellationToken);

                await task;
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override async Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
        {
            await GetFileAsync(path, cancellationToken);

            try
            {
                var task = Task.Run(() =>
                {
                    try
                    {
                        client.DeleteFile(PathSftp(PrependRootPath(path)));
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex, ex.Message);
                        throw;
                    }
                }, cancellationToken);

                await task;
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override void Disconnect()
        {
            client.Disconnect();
            Logger?.Information("{Adapter} - Disconnect succsefull", nameof(SftpAdapter));
        }

        public override void DisposeAdapter(bool disposing)
        {
            //client.Dispose()
        }

        public override async Task<IDirectory> GetDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            path = PathSftp(PrependRootPath(path));

            try
            {
                var task = Task.Run(() =>
                {
                    ISftpFile? directory = null;
                    try
                    {
                        directory = client.Get(path);
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex, ex.Message);
                        throw;
                    }

                    return directory;
                });

                await task;

                if (task.Result == null || !task.Result.IsDirectory)
                    throw new DirectoryNotFoundException(path, Prefix);

                return ModelFactory.CreateDirectory(task.Result);
            }
            catch (SftpPathNotFoundException)
            {
                throw new DirectoryNotFoundException(path, Prefix);
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override async Task<IEnumerable<IDirectory>> GetDirectoriesAsync(string path = "", CancellationToken cancellationToken = default)
        {
            await GetDirectoryAsync(path, cancellationToken);
            path = PathSftp(PrependRootPath(path));

            try
            {
                var directories = Enumerable.Empty<IDirectory>();

                var task = Task.Run(() =>
                {
                    try
                    {
                        directories = client.ListDirectory(path).Where(item => item.IsDirectory && item.Name != "." && item.Name != "..").Select(ModelFactory.CreateDirectory).ToList();
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex, ex.Message);
                        throw;
                    }

                    return directories;
                });

                await task;

                return task.Result;
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override async Task<IFile> GetFileAsync(string path, CancellationToken cancellationToken = default)
        {
            path = PathSftp(PrependRootPath(path));

            try
            {
                var task = Task.Run(() =>
                {
                    ISftpFile? directory = null;
                    try
                    {
                        directory = client.Get(path);
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex, ex.Message);
                        throw;
                    }

                    return directory;
                });

                await task;

                if (task.Result == null || task.Result.IsDirectory)
                    throw new FileNotFoundException(path, Prefix);

                return ModelFactory.CreateFile(task.Result);
            }
            catch (SftpPathNotFoundException)
            {
                throw new FileNotFoundException(path, Prefix);
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override async Task<IEnumerable<IFile>> GetFilesAsync(string path = "", CancellationToken cancellationToken = default)
        {
            await GetDirectoryAsync(path, cancellationToken);
            path = PathSftp(PrependRootPath(path));

            try
            {
                var files = Enumerable.Empty<IFile>();

                var task = Task.Run(() =>
                {
                    try
                    {
                        files = client.ListDirectory(path).Where(item => !item.IsDirectory).Select(ModelFactory.CreateFile).ToList();
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex, ex.Message);
                        throw;
                    }

                    return files;
                });

                await task;

                return task.Result;
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override async Task<byte[]> ReadFileAsync(string path, CancellationToken cancellationToken = default)
        {
            await GetFileAsync(path, cancellationToken);

            try
            {
                using var fileStream = client.OpenRead(PathSftp(PrependRootPath(path)));
                var fileContents = new byte[fileStream.Length];

                await fileStream.ReadAsync(fileContents, 0, (int)fileStream.Length, cancellationToken);

                return fileContents;
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override async Task<string> ReadTextFileAsync(string path, CancellationToken cancellationToken = default)
        {
            await GetFileAsync(path, cancellationToken);

            try
            {
                using var streamReader = new StreamReader(client.OpenRead(PathSftp(PrependRootPath(path))));

                return await streamReader.ReadToEndAsync();
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override async Task WriteFileAsync(string path, byte[] contents, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            if (!overwrite && await FileExistsAsync(path, cancellationToken))
                throw new FileExistsException(PathSftp(PrependRootPath(path)), Prefix);

            try
            {
                var task = Task.Run(() =>
                {
                    try
                    {
                        client.WriteAllBytes(PathSftp(PrependRootPath(path)), contents);
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex, ex.Message);
                        throw;
                    }
                }, cancellationToken);

                await task;
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        private static Exception Exception(Exception exception)
        {
            if (exception is SshConnectionException sshConnectionException)
                return new ConnectionException(sshConnectionException);

            if (exception is SocketException socketException)
                return new ConnectionException(socketException);

            if (exception is SshAuthenticationException sshAuthenticationException)
                return new ConnectionException(sshAuthenticationException);

            if (exception is ProxyException proxyException)
                return new ConnectionException(proxyException);

            return new AdapterRuntimeException(exception);
        }

        private string PathSftp(string path)
        {
            if (path.Substring(0, 1) == "/")
            {
                if (path.Length > 1)
                    return path.Substring(1);
                else
                    return String.Empty;
            }
            else
                return path;
        }
    }
}