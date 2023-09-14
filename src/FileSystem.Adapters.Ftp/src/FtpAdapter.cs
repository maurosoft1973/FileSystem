using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Maurosoft.FileSystem.Exceptions;
using Maurosoft.FileSystem.Models;
using DirectoryNotFoundException = Maurosoft.FileSystem.Exceptions.DirectoryNotFoundException;
using FileNotFoundException = Maurosoft.FileSystem.Exceptions.FileNotFoundException;
using FluentFTP;
using FluentFTP.Exceptions;
using Polly;

namespace Maurosoft.FileSystem.Adapters.Ftp
{
    public class FtpAdapter : Adapter
    {
        private readonly IFtpClient client;

        public FtpAdapter(string prefix, string rootPath, IFtpClient client) : base(prefix, rootPath)
        {
            this.client = client;
        }

        public override async Task AppendFileAsync(string path, byte[] contents, CancellationToken cancellationToken = default)
        {
            await GetFileAsync(path, cancellationToken);

            try
            {
                var task = Task.Run(() =>
                {
                    try
                    {
                        var file = client.OpenAppend(PrependRootPath(path));
                        file.Write(contents, 0, contents.Length);
                        file.Flush();
                        file.Close();
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

                Logger?.Information("{Adapter} - Connected successful", nameof(FtpAdapter));
            }
            catch (Exception exception)
            {
                Logger?.Error("{Adapter} - Connect error: {ConnectErrorMessage}", nameof(FtpAdapter), exception.Message);
                throw Exception(exception);
            }
        }

        public override async Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            if (await DirectoryExistsAsync(path, cancellationToken))
                throw new DirectoryExistsException(PrependRootPath(path), Prefix);

            try
            {
                var task = Task.Run(() =>
                {
                    try
                    {
                        client.CreateDirectory(PrependRootPath(path), true);
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
            Logger?.Information("{Adapter} - Disconnect succsefull", nameof(FtpAdapter));
        }

        public override void DisposeAdapter(bool disposing)
        {
            client.Dispose();
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
                        client.DeleteDirectory(PrependRootPath(path));
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
                        client.DeleteFile(PrependRootPath(path));
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

        public override async Task<IDirectory> GetDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            path = PrependRootPath(path);

            try
            {
                if (!client.IsConnected)
                    throw new FtpAuthenticationException("999", "Client is not Connected to Server");

                var task = Task.Run(() =>
                {
                    FtpListItem? directory = null;
                    try
                    {
                        directory = client.GetObjectInfo(path);
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex, ex.Message);
                        throw;
                    }

                    return directory;
                });

                await task;

                if (task.Result == null || task.Result.Type != FtpObjectType.Directory)
                    throw new DirectoryNotFoundException(path, Prefix);

                return ModelFactory.CreateDirectory(task.Result);
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override async Task<IEnumerable<IDirectory>> GetDirectoriesAsync(string path = "", CancellationToken cancellationToken = default)
        {
            await GetDirectoryAsync(path, cancellationToken);
            path = PrependRootPath(path);

            try
            {
                return await Task.Run(
                    () => client.GetListing(path).Where(item => item.Type == FtpObjectType.Directory).Select(ModelFactory.CreateDirectory).ToList(),
                    cancellationToken
                );
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override async Task<IFile> GetFileAsync(string path, CancellationToken cancellationToken = default)
        {
            path = PrependRootPath(path);

            try
            {
                await Task.Delay(1);

                if (!client.IsConnected)
                    throw new FtpAuthenticationException("999", "Client is not Connected to Server");

                FtpListItem? file = null;

                var retry = 0;
                var policy = Policy
                    .HandleResult<FtpListItem>((file) => file == null)
                    .WaitAndRetry(3, i => TimeSpan.FromMilliseconds(10),
                    onRetry: (exception, sleepDuration, attemptNumber, context) =>
                    {
                        retry++;
                    });

                file = policy.Execute(() => client.GetObjectInfo(path));

                if (file == null || file.Type == FtpObjectType.Directory)
                    throw new FileNotFoundException(path, Prefix);

                return ModelFactory.CreateFile(file);
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override async Task<IEnumerable<IFile>> GetFilesAsync(string path = "", CancellationToken cancellationToken = default)
        {
            await GetDirectoryAsync(path, cancellationToken);
            path = PrependRootPath(path);

            try
            {
                var task = Task.Run(() =>
                {
                    IEnumerable<IFile>? file = null;
                    try
                    {
                        file = client.GetListing(path).Where(item => item.Type != FtpObjectType.Directory).Select(ModelFactory.CreateFile).ToList();
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex, ex.Message);
                        throw;
                    }

                    return file;
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
                var task = Task.Run(() =>
                {
                    try
                    {
                        using var fileStream = client.OpenRead(PrependRootPath(path));
                        var fileContents = new byte[fileStream.Length];
                        fileStream.Read(fileContents, 0, (int)fileStream.Length);
                        return fileContents;
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error(ex, ex.Message);
                        throw;
                    }
                });

                return await task;
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
                using var streamReader = new StreamReader(client.OpenRead(PrependRootPath(path)));

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
                throw new FileExistsException(PrependRootPath(path), Prefix);

            try
            {
                var task = Task.Run(() =>
                {
                    try
                    {
                        client.UploadBytes(contents, PrependRootPath(path));
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

        private static Exception Exception(Exception exception)
        {
            if (exception is DirectoryExistsException)
                return exception;

            if (exception is DirectoryNotFoundException)
                return exception;

            if (exception is FileExistsException)
                return exception;

            if (exception is FileNotFoundException)
                return exception;

            if (exception is SocketException socketException)
                return new ConnectionException(socketException);

            if (exception is FtpAuthenticationException ftpAuthenticationException)
                return new ConnectionException(ftpAuthenticationException);

            if (exception is FtpSecurityNotAvailableException ftpSecurityNotAvailableException)
                return new ConnectionException(ftpSecurityNotAvailableException);

            return new AdapterRuntimeException(exception);
        }
    }
}