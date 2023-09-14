using System.Threading.Tasks;
using Xunit.Abstractions;
using FileSystem.Tests.Base;
using Maurosoft.FileSystem.Adapters.Sftp;
using Renci.SshNet;
using FileSystem.Tests.FileSystem.Adapters.Sftp;
using Serilog;
using Serilog.Sinks.InMemory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Maurosoft.FileSystem.Exceptions;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Renci.SshNet.Common;
using System.Net.Sockets;

namespace Tests.FileSystem.Adapters.Sftp;

[TestClass]
public class SftpAdapterIntegrationTest : IntegrationTestAdapter<SftpAdapter, SftpClient>, IClassFixture<SftpFixture>, IAsyncLifetime
{
    public override string Prefix => "sftpadapter1";
    public override string RootPath => "/";

    private SftpFixture Fixture { get; }
    private ISftpClient sftpClient;
    private readonly ISftpClient sftpClientMock = Mock.Of<ISftpClient>(MockBehavior.Strict);
    public SftpAdapterIntegrationTest(ITestOutputHelper outputHelper, SftpFixture sftpFixture)
    {
        Fixture = sftpFixture;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync()
    {
        Log.CloseAndFlush();

        Log.Logger = new LoggerConfiguration()
                     .WriteTo.InMemory()
                     .CreateLogger();

        sftpClient = new SftpClient(Fixture.GetHostname(), Fixture.GetPort(), Fixture.UserName, Fixture.Password);

        Mock.Get(sftpClientMock)
            .Setup((c) => c.AppendAllText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string path, string contents) => sftpClient.AppendAllText(path, contents));

        Mock.Get(sftpClientMock)
            .Setup((c) => c.Connect())
            .Callback(() => sftpClient.Connect());

        Mock.Get(sftpClientMock)
            .Setup((c) => c.CreateDirectory(It.IsAny<string>()))
            .Callback((string path) => sftpClient.CreateDirectory(path));

        Mock.Get(sftpClientMock)
            .Setup((c) => c.DeleteDirectory(It.IsAny<string>()))
            .Callback((string path) => sftpClient.DeleteDirectory(path));

        Mock.Get(sftpClientMock)
            .Setup((c) => c.DeleteFile(It.IsAny<string>()))
            .Callback((string path) => sftpClient.DeleteFile(path));

        Mock.Get(sftpClientMock)
            .Setup((c) => c.Disconnect())
            .Callback(() => sftpClient.Disconnect());

        Mock.Get(sftpClientMock)
            .Setup((c) => c.Get(It.IsAny<string>()))
            .Returns((string path) => sftpClient.Get(path));

        Mock.Get(sftpClientMock)
            .SetupGet((c) => c.IsConnected)
            .Returns(sftpClient.IsConnected);

        Mock.Get(sftpClientMock)
            .Setup((c) => c.ListDirectory(It.IsAny<string>(), It.IsAny<Action<int>>()))
            .Returns((string path, Action<int> listCallback) => sftpClient.ListDirectory(path, listCallback));

        Mock.Get(sftpClientMock)
            .Setup((c) => c.OpenRead(It.IsAny<string>()))
            .Returns((string path) => sftpClient.OpenRead(path));

        Mock.Get(sftpClientMock)
            .Setup((c) => c.WriteAllBytes(It.IsAny<string>(), It.IsAny<byte[]>()))
            .Callback((string path, byte[] bytes) => sftpClient.WriteAllBytes(path, bytes));

        _adapter = new SftpAdapter(Prefix, RootPath, sftpClientMock);
        _adapter.Connect();
        return Task.CompletedTask;
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task AppendFileAsync_ContentByte_IfExceptionOccurred_Should_Throw_AdapterRuntimeException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(sftpClientMock)
            .Setup((c) => c.AppendAllText(It.IsAny<string>(), It.IsAny<string>()))
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

        Mock.Get(sftpClientMock)
            .Setup((c) => c.AppendAllText(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new Exception("Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<AdapterRuntimeException>(async () => await _adapter!.AppendFileAsync(directory + "/" + filename, "AppendTextFile"));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task AppendFileAsync_ContentByte_IfSshConnectionException_Should_Throw_ConnectionException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(sftpClientMock)
            .Setup((c) => c.AppendAllText(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new SshConnectionException("Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.AppendFileAsync(directory + "/" + filename, System.Text.Encoding.UTF8.GetBytes("AppendTextFile")));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task AppendFileAsync_ContentString_IfSshConnectionException_Should_Throw_ConnectionException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(sftpClientMock)
            .Setup((c) => c.AppendAllText(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new SshConnectionException("Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.AppendFileAsync(directory + "/" + filename, "AppendTextFile"));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task CreateDirectoryAsync_IfSshConnectionException_Should_Throw_ConnectionException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);

        Mock.Get(sftpClientMock)
            .Setup((c) => c.CreateDirectory(It.IsAny<string>()))
            .Throws(new SshConnectionException("Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.CreateDirectoryAsync(directory));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public void Connect_IfThrowException_Should_Throw_AdapterRuntimeException()
    {
        //Arrange
        _adapter.Disconnect();

        Mock.Get(sftpClientMock)
            .Setup((c) => c.Connect())
            .Throws(new Exception("Errore"));

        //Assert
        Assert.ThrowsException<AdapterRuntimeException>(() => _adapter!.Connect());
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task DeleteDirectoryAsync_IfSshConnectionException_Should_Throw_ConnectionException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);
        _adapter.CreateDirectory(directory);

        Mock.Get(sftpClientMock)
            .Setup((c) => c.DeleteDirectory(It.IsAny<string>()))
            .Throws(new SshConnectionException("Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.DeleteDirectoryAsync(directory));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task DeleteFileAsync_IfSshConnectionException_Should_Throw_ConnectionException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(sftpClientMock)
            .Setup((c) => c.DeleteFile(It.IsAny<string>()))
            .Throws(new SshConnectionException("Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.DeleteFileAsync(directory + "/" + filename));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task GetDirectoriesAsync_IfSshConnectionException_Should_Throw_ConnectionException()
    {
        //Arrange
        Mock.Get(sftpClientMock)
            .Setup((c) => c.ListDirectory(It.IsAny<string>(), It.IsAny<Action<int>>()))
            .Throws(new SshAuthenticationException("Errore"));

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.GetDirectoriesAsync("/"));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task GetDirectoryAsync_IfSocketException_Should_Throw_ConnectionException()
    {
        //Arrange
        Mock.Get(sftpClientMock)
            .Setup((c) => c.Get(It.IsAny<string>()))
            .Throws(new SocketException());

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.GetDirectoryAsync("/"));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task GetFileAsync_IfProxyException_Should_Throw_ConnectionException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(sftpClientMock)
            .Setup((c) => c.Get(It.IsAny<string>()))
            .Throws(new ProxyException());

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.GetFileAsync(directory + "/" + filename));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task GetFilesAsync_IfProxyException_Should_Throw_ConnectionException()
    {
        //Arrange
        var (directory, _, _, _) = CreateFile();

        Mock.Get(sftpClientMock)
            .Setup((c) => c.ListDirectory(It.IsAny<string>(), It.IsAny<Action<int>>()))
            .Throws(new ProxyException());

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.GetFilesAsync(directory));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task ReadFileAsync_IfSshConnectionException_Should_Throw_ConnectionException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(sftpClientMock)
            .Setup((c) => c.OpenRead(It.IsAny<string>()))
            .Throws(new SshConnectionException());

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.ReadFileAsync(directory + "/" + filename));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task ReadTextFileAsync_IfSshConnectionException_Should_Throw_ConnectionException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        Mock.Get(sftpClientMock)
            .Setup((c) => c.OpenRead(It.IsAny<string>()))
            .Throws(new SshConnectionException());

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.ReadTextFileAsync(directory + "/" + filename));
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public async Task WriteFileAsync_IfSshConnectionException_Should_Throw_ConnectionException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);
        var filename = faker.Database.Random.AlphaNumeric(30);
        var content = System.Text.Encoding.UTF8.GetBytes("WriteTextFile");

        Mock.Get(sftpClientMock)
            .Setup((c) => c.WriteAllBytes(It.IsAny<string>(), It.IsAny<byte[]>()))
            .Throws(new SshConnectionException());

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.WriteFileAsync(directory + "/" + filename, content));
    }
}