using Maurosoft.FileSystem.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.InMemory;
using Xunit;

namespace FileSystem.Tests.Base;

[TestClass]
public abstract class IntegrationTestAdapter<A, C> : TestAdapter<A> where A : Adapter
{
    public override string Prefix => typeof(A).FullName;
    public override string RootPath => "/";
    public C Client { get; set; }

    [TestInitialize]
    public void Init()
    {
        Log.CloseAndFlush();

        Log.Logger = new LoggerConfiguration()
                     .WriteTo.InMemory()
                     .CreateLogger();

        _adapter = (A)Activator.CreateInstance(typeof(A), Prefix, RootPath, Client)!;
    }

    [Fact(DisplayName = "Instantiation_Prefix_Should_Return_Correct")]
    [TestCategory("IntegrationTest")]
    public override void Instantiation_Prefix_Should_Return_Correct() => base.Instantiation_Prefix_Should_Return_Correct();

    [Fact(DisplayName = "Instantiation_RootPath_Should_Return_Correct")]
    [TestCategory("IntegrationTest")]
    public override void Instantiation_RootPath_Should_Return_Correct() => base.Instantiation_RootPath_Should_Return_Correct();

    [Fact(DisplayName = "GetFile_IfSuccess_Should_ReturnFile")]
    [TestCategory("IntegrationTest")]
    public override void GetFile_IfSuccess_Should_ReturnFile() => base.GetFile_IfSuccess_Should_ReturnFile();

    [Fact(DisplayName = "GetFile_IfFileNotExist_Should_Throw_FileNotFoundException")]
    [TestCategory("IntegrationTest")]
    public override void GetFile_IfFileNotExist_Should_Throw_FileNotFoundException() => base.GetFile_IfFileNotExist_Should_Throw_FileNotFoundException();

    [Fact(DisplayName = "GetFileAsync_IfSuccess_Should_ReturnFile")]
    [TestCategory("IntegrationTest")]
    public override async Task GetFileAsync_IfSuccess_Should_ReturnFile() => await base.GetFileAsync_IfSuccess_Should_ReturnFile();

    [Fact(DisplayName = "GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException")]
    [TestCategory("IntegrationTest")]
    public override async Task GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException() => await base.GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException();

    [Fact(DisplayName = "GetFiles_IfSuccess_Should_ReturnFiles")]
    [TestCategory("IntegrationTest")]
    public override void GetFiles_IfSuccess_Should_ReturnFiles() => base.GetFiles_IfSuccess_Should_ReturnFiles();

    [Fact(DisplayName = "GetFiles_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException")]
    [TestCategory("IntegrationTest")]
    public override void GetFiles_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException() => base.GetFiles_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException();

    [Fact(DisplayName = "GetFilesAsync_IfSuccess_Should_ReturnFiles")]
    [TestCategory("IntegrationTest")]
    public override async Task GetFilesAsync_IfSuccess_Should_ReturnFiles() => await base.GetFilesAsync_IfSuccess_Should_ReturnFiles();

    [Fact(DisplayName = "GetFilesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException")]
    [TestCategory("IntegrationTest")]
    public override async Task GetFilesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException() => await base.GetFilesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException();

    [Fact(DisplayName = "GetDirectoriesAsync_IfSuccess_Should_ReturnDirectories")]
    [TestCategory("IntegrationTest")]
    public override async Task GetDirectoriesAsync_IfSuccess_Should_ReturnDirectories() => await base.GetDirectoriesAsync_IfSuccess_Should_ReturnDirectories();

    [Fact(DisplayName = "GetDirectoriesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException")]
    [TestCategory("IntegrationTest")]
    public override async Task GetDirectoriesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException() => await base.GetDirectoriesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException();

    [Fact(DisplayName = "CreateDirectoryAsync_IfSuccess_Should_ReturnDirectoryExists")]
    [TestCategory("IntegrationTest")]
    public override async Task CreateDirectoryAsync_IfSuccess_Should_ReturnDirectoryExists() => await base.CreateDirectoryAsync_IfSuccess_Should_ReturnDirectoryExists();

    [Fact(DisplayName = "CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException")]
    [TestCategory("IntegrationTest")]
    public override async Task CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException() => await base.CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException();

    [Fact(DisplayName = "DeleteFileAsync")]
    [TestCategory("IntegrationTest")]
    public override async Task DeleteFileAsync() => await base.DeleteFileAsync();

    [Fact(DisplayName = "DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException")]
    [TestCategory("IntegrationTest")]
    public override async Task DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException() => await base.DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException();

    [Fact(DisplayName = "DeleteDirectoryAsync")]
    [TestCategory("IntegrationTest")]
    public override async Task DeleteDirectoryAsync() => await base.DeleteDirectoryAsync();

    [Fact(DisplayName = "DeleteDirectoryAsync_IfNotExists_Should_ThrowFileNotFoundException")]
    [TestCategory("IntegrationTest")]
    public override async Task DeleteDirectoryAsync_IfNotExists_Should_ThrowFileNotFoundException() => await base.DeleteDirectoryAsync_IfNotExists_Should_ThrowFileNotFoundException();

    [Fact(DisplayName = "ReadFile_IfSuccess_Should_ReturnLength")]
    [TestCategory("IntegrationTest")]
    public override void ReadFile_IfSuccess_Should_ReturnLength() => base.ReadFile_IfSuccess_Should_ReturnLength();

    [Fact(DisplayName = "ReadFile_IfFileNotExist_Should_ThrowFileNotFoundException")]
    [TestCategory("IntegrationTest")]
    public override void ReadFile_IfFileNotExist_Should_ThrowFileNotFoundException() => base.ReadFile_IfFileNotExist_Should_ThrowFileNotFoundException();

    [Fact(DisplayName = "ReadFileAsync_IfSuccess_Should_ReturnLength")]
    [TestCategory("IntegrationTest")]
    public override async Task ReadFileAsync_IfSuccess_Should_ReturnLength() => await base.ReadFileAsync_IfSuccess_Should_ReturnLength();

    [Fact(DisplayName = "ReadFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException")]
    [TestCategory("IntegrationTest")]
    public override async Task ReadFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException() => await base.ReadFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException();

    [Fact(DisplayName = "ReadTextFileAsync_IfSuccess_Should_ReturnLength")]
    [TestCategory("IntegrationTest")]
    public override async Task ReadTextFileAsync_IfSuccess_Should_ReturnLength() => await base.ReadTextFileAsync_IfSuccess_Should_ReturnLength();

    [Fact(DisplayName = "ReadTextFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException")]
    [TestCategory("IntegrationTest")]
    public override async Task ReadTextFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException() => await base.ReadTextFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException();

}
