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

    [TestCleanup]
    public void Clean()
    {
        _adapter.Disconnect();
        _adapter.Dispose();
    }

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void AppendFile_ContentByte_IfSuccess_Should_ReturnStringAppend() => base.AppendFile_ContentByte_IfSuccess_Should_ReturnStringAppend();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void AppendFile_ContentByte_IfFileNotExist_Should_Throw_FileNotFoundException() => base.AppendFile_ContentByte_IfFileNotExist_Should_Throw_FileNotFoundException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void AppendFile_ContentByte_IfConnectionClose_Should_Throw_ConnectionException() => base.AppendFile_ContentByte_IfConnectionClose_Should_Throw_ConnectionException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void AppendFile_ContentString_IfSuccess_Should_ReturnStringAppend() => base.AppendFile_ContentString_IfSuccess_Should_ReturnStringAppend();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void AppendFile_ContentString_IfFileNotExist_Should_Throw_FileNotFoundException() => base.AppendFile_ContentString_IfFileNotExist_Should_Throw_FileNotFoundException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void AppendFile_ContentString_IfConnectionClose_Should_Throw_ConnectionException() => base.AppendFile_ContentString_IfConnectionClose_Should_Throw_ConnectionException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task AppendFileAsync_ContentByte_IfSuccess_Should_ReturnStringAppend() => await base.AppendFileAsync_ContentByte_IfSuccess_Should_ReturnStringAppend();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task AppendFileAsync_ContentByte_IfFileNotExist_Should_Throw_FileNotFoundException() => await base.AppendFileAsync_ContentByte_IfFileNotExist_Should_Throw_FileNotFoundException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task AppendFileAsync_ContentByte_IfConnectionClose_Should_Throw_ConnectionException() => await base.AppendFileAsync_ContentByte_IfConnectionClose_Should_Throw_ConnectionException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task AppendFileAsync_ContentString_IfSuccess_Should_ReturnStringAppend() => await base.AppendFileAsync_ContentString_IfSuccess_Should_ReturnStringAppend();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task AppendFileAsync_ContentString_IfFileNotExist_Should_Throw_FileNotFoundException() => await base.AppendFileAsync_ContentString_IfFileNotExist_Should_Throw_FileNotFoundException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task AppendFileAsync_ContentString_IfConnectionClose_Should_Throw_ConnectionException() => await base.AppendFileAsync_ContentString_IfConnectionClose_Should_Throw_ConnectionException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void Connect_IfSuccess_Should_Return_Message_ConnectedSuccessful() => base.Connect_IfSuccess_Should_Return_Message_ConnectedSuccessful();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task CreateDirectoryAsync_IfSuccess_Should_ReturnDirectoryExists() => await base.CreateDirectoryAsync_IfSuccess_Should_ReturnDirectoryExists();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException() => await base.CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task DeleteFileAsync_IfSuccess_Should_Return_FileNotExists() => await base.DeleteFileAsync_IfSuccess_Should_Return_FileNotExists();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException() => await base.DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task DeleteDirectoryAsync() => await base.DeleteDirectoryAsync();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task DeleteDirectoryAsync_IfNotExists_Should_ThrowFileNotFoundException() => await base.DeleteDirectoryAsync_IfNotExists_Should_ThrowFileNotFoundException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task GetDirectoryAsync_IfConnectionClose_Should_Throw_ConnectionException() => await base.GetDirectoryAsync_IfConnectionClose_Should_Throw_ConnectionException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task GetDirectoriesAsync_IfSuccess_Should_ReturnDirectories() => await base.GetDirectoriesAsync_IfSuccess_Should_ReturnDirectories();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task GetDirectoriesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException() => await base.GetDirectoriesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void GetFile_IfSuccess_Should_ReturnFile() => base.GetFile_IfSuccess_Should_ReturnFile();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void GetFile_IfFileNotExist_Should_Throw_FileNotFoundException() => base.GetFile_IfFileNotExist_Should_Throw_FileNotFoundException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task GetFileAsync_IfSuccess_Should_ReturnFile() => await base.GetFileAsync_IfSuccess_Should_ReturnFile();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException() => await base.GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task GetFileAsync_IfConnectionClose_Should_Throw_Exception() => await base.GetFileAsync_IfConnectionClose_Should_Throw_Exception();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void GetFiles_IfSuccess_Should_ReturnFiles() => base.GetFiles_IfSuccess_Should_ReturnFiles();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void GetFiles_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException() => base.GetFiles_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task GetFilesAsync_IfSuccess_Should_ReturnFiles() => await base.GetFilesAsync_IfSuccess_Should_ReturnFiles();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task GetFilesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException() => await base.GetFilesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void Instantiation_Prefix_Should_Return_Correct() => base.Instantiation_Prefix_Should_Return_Correct();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void Instantiation_RootPath_Should_Return_Correct() => base.Instantiation_RootPath_Should_Return_Correct();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void ReadFile_IfSuccess_Should_ReturnLength() => base.ReadFile_IfSuccess_Should_ReturnLength();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void ReadFile_IfFileNotExist_Should_ThrowFileNotFoundException() => base.ReadFile_IfFileNotExist_Should_ThrowFileNotFoundException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task ReadFileAsync_IfSuccess_Should_ReturnLength() => await base.ReadFileAsync_IfSuccess_Should_ReturnLength();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task ReadFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException() => await base.ReadFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task ReadFileAsync_IfConnectionClose_Should_Throw_Exception() => await base.ReadFileAsync_IfConnectionClose_Should_Throw_Exception();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task ReadTextFileAsync_IfSuccess_Should_ReturnLength() => await base.ReadTextFileAsync_IfSuccess_Should_ReturnLength();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override async Task ReadTextFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException() => await base.ReadTextFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void WriteFile_NoOverwrite_IfFileExist_Should_ThrowFileExistsException() => base.WriteFile_NoOverwrite_IfFileExist_Should_ThrowFileExistsException();

    [Fact]
    [TestCategory("IntegrationTest")]
    public override void WriteFile_OverwriteTrue_IfFileExist_Should_ReturnFileOvewrite() => base.WriteFile_OverwriteTrue_IfFileExist_Should_ReturnFileOvewrite();

}
