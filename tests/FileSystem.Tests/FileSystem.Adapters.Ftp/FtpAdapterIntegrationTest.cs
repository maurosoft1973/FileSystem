using System.Threading.Tasks;
using Xunit;
using FileSystem.Tests.FileSystem.Adapters.Ftp;
using Xunit.Abstractions;
using FluentFTP;
using Maurosoft.FileSystem.Adapters.Ftp;
using FileSystem.Tests.Base;
using Serilog;
using Serilog.Sinks.InMemory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Renci.SshNet;
using Maurosoft.FileSystem.Exceptions;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using FluentFTP.Exceptions;
using System.Net.Sockets;
using Renci.SshNet.Common;

namespace Tests.FileSystem.Adapters.Ftp;

[TestClass]
public class FtpAdapterIntegrationTest : IntegrationTestAdapter<FtpAdapter, FtpClient>, IClassFixture<FtpFixture>, IAsyncLifetime
{
    public override string Prefix => "ftpadapter1";
    public override string RootPath => "/";

    private FtpFixture Fixture { get; }
    private IFtpClient ftpClient;
    private readonly IFtpClient ftpClientMock = Mock.Of<IFtpClient>(MockBehavior.Strict);

    public FtpAdapterIntegrationTest(ITestOutputHelper outputHelper, FtpFixture ftpFixture)
    {
        Fixture = ftpFixture;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync()
    {
        Log.CloseAndFlush();

        Log.Logger = new LoggerConfiguration()
                     .WriteTo.File("serilog.log")
                     .WriteTo.InMemory()
                     .CreateLogger();

        ftpClient = new FtpClient(Fixture.GetHostname(), Fixture.UserName, Fixture.Password, Fixture.GetPort());

        Mock.Get(ftpClientMock)
            .Setup((c) => c.OpenAppend(It.IsAny<string>(), It.IsAny<FtpDataType>(), It.IsAny<bool>()))
            .Returns((string path, FtpDataType ftpDataType, bool checkIfFileExists) => ftpClient.OpenAppend(path));

        Mock.Get(ftpClientMock)
            .Setup((c) => c.Connect())
            .Callback(() => ftpClient.Connect());

        Mock.Get(ftpClientMock)
            .Setup((c) => c.CreateDirectory(It.IsAny<string>()))
            .Returns((string path) => ftpClient.CreateDirectory(path));

        Mock.Get(ftpClientMock)
            .Setup((c) => c.CreateDirectory(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns((string path, bool force) => ftpClient.CreateDirectory(path, force));

        Mock.Get(ftpClientMock)
            .Setup((c) => c.DeleteDirectory(It.IsAny<string>()))
            .Callback((string path) => ftpClient.DeleteDirectory(path));

        Mock.Get(ftpClientMock)
            .Setup((c) => c.DeleteFile(It.IsAny<string>()))
            .Callback((string path) => ftpClient.DeleteFile(path));

        Mock.Get(ftpClientMock)
            .Setup((c) => c.Disconnect())
            .Callback(() => ftpClient.Disconnect());

        Mock.Get(ftpClientMock)
            .Setup((c) => c.GetObjectInfo(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns((string path, bool dateModified) => ftpClient.GetObjectInfo(path));

        Mock.Get(ftpClientMock)
            .SetupGet((c) => c.IsConnected)
            .Returns(() => ftpClient.IsConnected);

        Mock.Get(ftpClientMock)
            .Setup((c) => c.GetListing(It.IsAny<string>()))
            .Returns((string path) => ftpClient.GetListing(path));

        Mock.Get(ftpClientMock)
            .Setup((c) => c.OpenRead(It.IsAny<string>(), It.IsAny<FtpDataType>(), It.IsAny<long>(), It.IsAny<bool>()))
            .Returns((string path, FtpDataType type, long restart, bool checkIfFileExists) => ftpClient.OpenRead(path));

        Mock.Get(ftpClientMock)
            .Setup((c) => c.UploadBytes(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<FtpRemoteExists>(), It.IsAny<bool>(), It.IsAny<Action<FtpProgress>>()))
            .Returns((byte[] bytes, string path, FtpRemoteExists existsMode, bool createRemoteDir, Action<FtpProgress> progress) => ftpClient.UploadBytes(bytes, path));

        Mock.Get(ftpClientMock)
            .Setup((c) => c.IsAuthenticated)
            .Returns(() => ftpClient.IsAuthenticated);

        Mock.Get(ftpClientMock)
            .Setup((c) => c.Config)
            .Returns(() => ftpClient.Config);

        Mock.Get(ftpClientMock)
            .Setup((c) => c.AutoConnect())
            .Returns(() => ftpClient.AutoConnect());

        _adapter = new FtpAdapter(Prefix, RootPath, ftpClientMock);
        _adapter.Connect();

        return Task.CompletedTask;
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task AppendFileAsync_ContentByte_IfExceptionOccurred_Should_Throw_AdapterRuntimeException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(ftpClientMock)
            .Setup((c) => c.OpenAppend(It.IsAny<string>(), It.IsAny<FtpDataType>(), It.IsAny<bool>()))
            .Throws(new Exception("Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<AdapterRuntimeException>(async () => await _adapter!.AppendFileAsync(directory + "/" + filename, System.Text.Encoding.UTF8.GetBytes("AppendTextFile")));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task AppendFileAsync_ContentString_IfExceptionOccurred_Should_Throw_AdapterRuntimeException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(ftpClientMock)
            .Setup((c) => c.OpenAppend(It.IsAny<string>(), It.IsAny<FtpDataType>(), It.IsAny<bool>()))
            .Throws(new Exception("Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<AdapterRuntimeException>(async () => await _adapter!.AppendFileAsync(directory + "/" + filename, "AppendTextFile"));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task AppendFileAsync_ContentByte_IfFtpAuthenticationException_Should_Throw_ConnectionException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(ftpClientMock)
            .Setup((c) => c.OpenAppend(It.IsAny<string>(), It.IsAny<FtpDataType>(), It.IsAny<bool>()))
            .Throws(new FtpAuthenticationException("999", "Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.AppendFileAsync(directory + "/" + filename, System.Text.Encoding.UTF8.GetBytes("AppendTextFile")));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task AppendFileAsync_ContentString_IfFtpAuthenticationException_Should_Throw_ConnectionException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(ftpClientMock)
            .Setup((c) => c.OpenAppend(It.IsAny<string>(), It.IsAny<FtpDataType>(), It.IsAny<bool>()))
            .Throws(new FtpAuthenticationException("999", "Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.AppendFileAsync(directory + "/" + filename, "AppendTextFile"));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task CreateDirectoryAsync_IfFtpAuthenticationException_Should_Throw_ConnectionException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);

        Mock.Get(ftpClientMock)
            .Setup((c) => c.CreateDirectory(It.IsAny<string>(), It.IsAny<bool>()))
            .Throws(new FtpAuthenticationException("999", "Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.CreateDirectoryAsync(directory));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task DeleteDirectoryAsync_IfFtpAuthenticationException_Should_Throw_ConnectionException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);
        _adapter.CreateDirectory(directory);

        Mock.Get(ftpClientMock)
            .Setup((c) => c.DeleteDirectory(It.IsAny<string>()))
            .Throws(new FtpAuthenticationException("999", "Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.DeleteDirectoryAsync(directory));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task DeleteFileAsync_IfFtpAuthenticationException_Should_Throw_ConnectionException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(ftpClientMock)
            .Setup((c) => c.DeleteFile(It.IsAny<string>()))
            .Throws(new FtpAuthenticationException("999", "Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.DeleteFileAsync(directory + "/" + filename));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task GetDirectoriesAsync_IfFtpAuthenticationException_Should_Throw_ConnectionException()
    {
        //Arrange
        Mock.Get(ftpClientMock)
            .Setup((c) => c.GetListing(It.IsAny<string>()))
            .Throws(new FtpAuthenticationException("999", "Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.GetDirectoriesAsync("/"));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task GetDirectoryAsync_IfSocketException_Should_Throw_ConnectionException()
    {
        //Arrange
        Mock.Get(ftpClientMock)
            .Setup((c) => c.GetObjectInfo(It.IsAny<string>(), It.IsAny<bool>()))
            .Throws(new SocketException());

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.GetDirectoryAsync("/"));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task GetFileAsync_IfSocketException_Should_Throw_ConnectionException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(ftpClientMock)
            .Setup((c) => c.GetObjectInfo(It.IsAny<string>(), It.IsAny<bool>()))
            .Throws(new SocketException());

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.GetFileAsync(directory + "/" + filename));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task GetFilesAsync_IfSocketException_Should_Throw_ConnectionException()
    {
        //Arrange
        var (directory, _, _, _) = CreateFile();

        Mock.Get(ftpClientMock)
            .Setup((c) => c.GetListing(It.IsAny<string>()))
            .Throws(new SocketException());

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.GetFilesAsync(directory));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task ReadFileAsync_IfFtpAuthenticationException_Should_Throw_ConnectionException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(ftpClientMock)
            .Setup((c) => c.OpenRead(It.IsAny<string>(), It.IsAny<FtpDataType>(), It.IsAny<long>(), It.IsAny<bool>()))
            .Throws(new FtpAuthenticationException("999", "Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.ReadFileAsync(directory + "/" + filename));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task ReadTextFileAsync_IfFtpAuthenticationException_Should_Throw_ConnectionException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(ftpClientMock)
            .Setup((c) => c.OpenRead(It.IsAny<string>(), It.IsAny<FtpDataType>(), It.IsAny<long>(), It.IsAny<bool>()))
            .Throws(new FtpAuthenticationException("999", "Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.ReadTextFileAsync(directory + "/" + filename));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task WriteFileAsync_IfFtpAuthenticationException_Should_Throw_ConnectionException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);
        var filename = faker.Database.Random.AlphaNumeric(30);
        var content = System.Text.Encoding.UTF8.GetBytes("WriteTextFile");

        Mock.Get(ftpClientMock)
            .Setup((c) => c.UploadBytes(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<FtpRemoteExists>(), It.IsAny<bool>(), It.IsAny<Action<FtpProgress>>()))
            .Throws(new FtpAuthenticationException("999", "Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.WriteFileAsync(directory + "/" + filename, content));
    }
}