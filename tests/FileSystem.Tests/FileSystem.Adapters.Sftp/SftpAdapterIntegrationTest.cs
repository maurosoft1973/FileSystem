﻿using System.Threading.Tasks;
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

    private readonly ITestOutputHelper _outputHelper;

    public SftpAdapterIntegrationTest(ITestOutputHelper outputHelper, SftpFixture sftpFixture)
    {
        _outputHelper = outputHelper;
        Fixture = sftpFixture;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync()
    {
        Log.CloseAndFlush();

        Logger = new LoggerConfiguration()
                     .WriteTo.InMemory()
                     .CreateLogger();

        sftpClient = new SftpClient(Fixture.GetHostname(), Fixture.GetPort(), Fixture.UserName, Fixture.Password);
        _adapter = new SftpAdapter(Prefix, RootPath, sftpClient, Logger);
        _adapter.Connect();
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "SftpAdapter_Instantiation_Prefix_Should_Return_Correct")]
    public override void Instantiation_Prefix_Should_Return_Correct() => base.Instantiation_Prefix_Should_Return_Correct();

    [Fact(DisplayName = "SftpAdapter_Instantiation_Prefix_Should_Return_Correct")]
    public override void Instantiation_RootPath_Should_Return_Correct() => base.Instantiation_RootPath_Should_Return_Correct();

    [Fact(DisplayName = "SftpAdapter_GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException")]
    public override async Task GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException() => await base.GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException();

    [Fact(DisplayName = "SftpAdapter_GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles")]
    public override async Task GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles() => await base.GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles();

    [Fact(DisplayName = "SftpAdapter_CreateDirectoryAsync_Should_Exists")]
    public override async Task CreateDirectoryAsync_Should_Exists() => await base.CreateDirectoryAsync_Should_Exists();

    [Fact(DisplayName = "SftpAdapter_CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException")]
    public override async Task CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException() => await base.CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException();

    [Fact(DisplayName = "SftpAdapter_DeleteFileAsync")]
    public override async Task DeleteFileAsync() => await base.DeleteFileAsync();

    [Fact(DisplayName = "SftpAdapter_DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException")]
    public override async Task DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException() => await base.DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException();
}