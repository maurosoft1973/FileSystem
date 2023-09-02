using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maurosoft.FileSystem.Adapters;
using System;
using System.Threading.Tasks;
using FileSystem.Tests.Base;
using Serilog;
using Serilog.Sinks.InMemory;

namespace Tests.Base;

[TestClass]
public abstract class UnitTestAdapter<A> : TestAdapter<A> where A : Adapter
{
    public override string Prefix => typeof(A).FullName;
    public override string RootPath => "/";

    [TestInitialize]
    public void Init()
    {
        Log.CloseAndFlush();

        Log.Logger = new LoggerConfiguration()
                     .WriteTo.InMemory()
                     .CreateLogger();

        _adapter = (A)Activator.CreateInstance(typeof(A), Prefix, RootPath)!;
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public override void Instantiation_Prefix_Should_Return_Correct() => base.Instantiation_Prefix_Should_Return_Correct();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override void Instantiation_RootPath_Should_Return_Correct() => base.Instantiation_RootPath_Should_Return_Correct();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException() => await base.GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles() => await base.GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task CreateDirectoryAsync_IfSuccess_Should_ReturnDirectoryExists() => await base.CreateDirectoryAsync_IfSuccess_Should_ReturnDirectoryExists();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException() => await base.CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task DeleteFileAsync() => await base.DeleteFileAsync();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException() => await base.DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task DeleteDirectoryAsync() => await base.DeleteDirectoryAsync();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task DeleteDirectoryAsync_IfNotExists_Should_ThrowFileNotFoundException() => await base.DeleteDirectoryAsync_IfNotExists_Should_ThrowFileNotFoundException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override void ReadFile() => base.ReadFile();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override void ReadFile_IfFileNotExist_Should_ThrowFileNotFoundException() => base.ReadFile_IfFileNotExist_Should_ThrowFileNotFoundException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task ReadFileAsync() => await base.ReadFileAsync();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task ReadFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException() => await base.ReadFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException();
}
