using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet.Common;
using Maurosoft.FileSystem.Exceptions;
using Maurosoft.FileSystem.Models;
using DirectoryNotFoundException = Maurosoft.FileSystem.Exceptions.DirectoryNotFoundException;
using FileNotFoundException = Maurosoft.FileSystem.Exceptions.FileNotFoundException;
using FluentFTP;
using Serilog;

namespace Maurosoft.FileSystem.Adapters.Ftp
{
    public class FtpAdapter : Adapter
    {
        private readonly FtpClient client;

        public FtpAdapter(string prefix, string rootPath, FtpClient client, ILogger logger) : base(prefix, rootPath)
        {
            this.client = client;
            this.Logger = logger;
        }

        public override void Dispose()
        {
            client.Dispose();
        }

        public override void Connect()
        {
            try
            {
                if (client.IsConnected)
                    return;

                client.Connect();
                Logger?.Information("{Adapter} - Connected succsefull", nameof(FtpAdapter));
            }
            catch (Exception exception)
            {
                Logger?.Error("{Adapter} - Connect error: {ConnectErrorMessage}", nameof(FtpAdapter), exception.Message);
                throw Exception(exception);
            }
        }

        public override async Task<IFile> GetFileAsync(string path, CancellationToken cancellationToken = default)
        {
            path = PrependRootPath(path);

            try
            {
                var file = await Task.Run(() => client.GetObjectInfo(path), cancellationToken);

                if (file == null || file.Type == FtpObjectType.Directory)
                    throw new FileNotFoundException(path, Prefix);

                return ModelFactory.CreateFile(file);
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

        public override async Task<IDirectory> GetDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            path = PrependRootPath(path);

            try
            {
                var directory = await Task.Run(() => client.GetObjectInfo(path), cancellationToken);

                if (directory == null || directory.Type != FtpObjectType.Directory)
                    throw new DirectoryNotFoundException(path, Prefix);

                return ModelFactory.CreateDirectory(directory);
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

        public override async Task<IEnumerable<IFile>> GetFilesAsync(string path = "", CancellationToken cancellationToken = default)
        {
            await GetDirectoryAsync(path, cancellationToken);
            path = PrependRootPath(path);

            try
            {
                return await Task.Run(
                    () => client.GetListing(path).Where(item => item.Type != FtpObjectType.Directory).Select(ModelFactory.CreateFile).ToList(),
                    cancellationToken
                );
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

        public override async Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            if (await DirectoryExistsAsync(path, cancellationToken))
                throw new DirectoryExistsException(PrependRootPath(path), Prefix);

            try
            {
                await Task.Run(() => client.CreateDirectory(PrependRootPath(path), true), cancellationToken);
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
                await Task.Run(() => client.DeleteFile(PrependRootPath(path)), cancellationToken);
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
                await Task.Run(() => client.DeleteDirectory(PrependRootPath(path)), cancellationToken);
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
                using var fileStream = client.OpenRead(PrependRootPath(path));
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
                await Task.Run(() => client.UploadBytes(contents, PrependRootPath(path)), cancellationToken);
            }
            catch (Exception exception)
            {
                throw Exception(exception);
            }
        }

        public override async Task AppendFileAsync(string path, byte[] contents, CancellationToken cancellationToken = default)
        {
            await GetFileAsync(path, cancellationToken);

            try
            {
                await Task.Run(() =>
                {
                    var file = client.OpenWrite(PrependRootPath(path));
                    file.Write(contents, 0, contents.Length);
                    file.Flush();
                }, cancellationToken);
            }
            catch (SshConnectionException exception)
            {
                throw new ConnectionException(exception);
            }
            catch (Exception exception)
            {
                throw new AdapterRuntimeException(exception);
            }
        }

        private static Exception Exception(Exception exception)
        {
            if (exception is FileSystemException)
                return exception;

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
    }
}