using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using FileSystem.Tests.Base;
using Maurosoft.FileSystem.Adapters.Sftp;
using Renci.SshNet;
using FileSystem.Tests.FileSystem.Adapters.Sftp;
using Serilog;
using Serilog.Sinks.InMemory;

namespace Tests.FileSystem.Adapters.Sftp;

public class SftpAdapterIntegrationTest : IntegrationTestAdapter<SftpAdapter, SftpClient>, IClassFixture<SftpFixture>, IAsyncLifetime
{
    public override string Prefix => "sftpadapter1";
    public override string RootPath => "/";

    private SftpFixture Fixture { get; }
    private SftpClient sftpClient;

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
        _adapter = new SftpAdapter(Prefix, RootPath, sftpClient);
        _adapter.Connect();
        return Task.CompletedTask;
    }
}