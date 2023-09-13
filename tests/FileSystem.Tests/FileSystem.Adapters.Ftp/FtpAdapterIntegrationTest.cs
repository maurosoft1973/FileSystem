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

namespace Tests.FileSystem.Adapters.Ftp;

[TestClass]
public class FtpAdapterIntegrationTest : IntegrationTestAdapter<FtpAdapter, FtpClient>, IClassFixture<FtpFixture>, IAsyncLifetime
{
    public override string Prefix => "ftpadapter1";
    public override string RootPath => "/";

    private FtpFixture Fixture { get; }
    private FtpClient ftpClient;

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
        ftpClient.Connect();
        _adapter = new FtpAdapter(Prefix, RootPath, ftpClient);
        return Task.CompletedTask;
    }
}