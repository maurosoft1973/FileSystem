﻿using System.Threading.Tasks;
using Xunit;
using FileSystem.Tests.FileSystem.Adapters.Ftp;
using Xunit.Abstractions;
using FluentFTP;
using Maurosoft.FileSystem.Adapters.Ftp;
using FileSystem.Tests.Base;

namespace Tests.FileSystem.Adapters.Ftp
{
    public class FtpAdapterIntegrationTest : IntegrationTestAdapter<FtpAdapter, FtpClient>, IClassFixture<FtpFixture>, IAsyncLifetime
    {
        public override string Prefix => "ftpadapter1";
        public override string RootPath => "/";

        private FtpFixture _fixture { get; }
        private FtpClient _ftpClient;

        private readonly ITestOutputHelper _outputHelper;

        public FtpAdapterIntegrationTest(ITestOutputHelper outputHelper, FtpFixture ftpFixture)
        {
            _outputHelper = outputHelper;
            _fixture = ftpFixture;
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public Task InitializeAsync()
        {
            _ftpClient = new FtpClient(_fixture.GetHostname(), _fixture.UserName, _fixture.Password, _fixture.GetPort());
            _ftpClient.Connect();
            _adapter = new FtpAdapter(Prefix, RootPath, _ftpClient);
            return Task.CompletedTask;
        }

        [Fact(DisplayName = "FtpAdapter_Instantiation_Prefix_Should_Return_Correct")]
        public override void Instantiation_Prefix_Should_Return_Correct() => base.Instantiation_Prefix_Should_Return_Correct();

        [Fact(DisplayName = "FtpAdapter_Instantiation_Prefix_Should_Return_Correct")]
        public override void Instantiation_RootPath_Should_Return_Correct() => base.Instantiation_RootPath_Should_Return_Correct();

        [Fact(DisplayName = "FtpAdapter_GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException")]
        public override async Task GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException() => await base.GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException();

        [Fact(DisplayName = "FtpAdapter_GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles")]
        public override async Task GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles() => await base.GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles();

        [Fact(DisplayName = "FtpAdapter_CreateDirectoryAsync_Should_Exists")]
        public override async Task CreateDirectoryAsync_Should_Exists() => await base.CreateDirectoryAsync_Should_Exists();

        [Fact(DisplayName = "FtpAdapter_CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException")]
        public override async Task CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException() => await base.CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException();

        [Fact(DisplayName = "FtpAdapter_DeleteFileAsync")]
        public override async Task DeleteFileAsync() => await base.DeleteFileAsync();

        [Fact(DisplayName = "FtpAdapter_DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException")]
        public override async Task DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException() => await base.DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException();
    }
}