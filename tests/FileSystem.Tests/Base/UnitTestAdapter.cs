﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maurosoft.FileSystem.Adapters;
using System;
using System.Threading.Tasks;
using FileSystem.Tests.Base;
using Serilog;
using Serilog.Sinks.InMemory;
using Xunit;

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
    public override async Task AppendFileAsync_ContentByte_IfSuccess_Should_ReturnStringAppend() => await base.AppendFileAsync_ContentByte_IfSuccess_Should_ReturnStringAppend();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task AppendFileAsync_ContentByte_IfFileNotExist_Should_Throw_FileNotFoundException() => await base.AppendFileAsync_ContentByte_IfFileNotExist_Should_Throw_FileNotFoundException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task AppendFileAsync_ContentByte_IfConnectionClose_Should_Throw_ConnectionException() => await base.AppendFileAsync_ContentByte_IfConnectionClose_Should_Throw_ConnectionException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task AppendFileAsync_ContentString_IfSuccess_Should_ReturnStringAppend() => await base.AppendFileAsync_ContentString_IfSuccess_Should_ReturnStringAppend();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task AppendFileAsync_ContentString_IfFileNotExist_Should_Throw_FileNotFoundException() => await base.AppendFileAsync_ContentString_IfFileNotExist_Should_Throw_FileNotFoundException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task AppendFileAsync_ContentString_IfConnectionClose_Should_Throw_ConnectionException() => await base.AppendFileAsync_ContentString_IfConnectionClose_Should_Throw_ConnectionException();

    [Fact]
    [TestCategory("UnitTest")]
    public override void Connect_IfSuccess_Should_Return_Message_ConnectedSuccessful() => base.Connect_IfSuccess_Should_Return_Message_ConnectedSuccessful();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task CreateDirectoryAsync_IfSuccess_Should_ReturnDirectoryExists() => await base.CreateDirectoryAsync_IfSuccess_Should_ReturnDirectoryExists();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException() => await base.CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task DeleteFileAsync_IfSuccess_Should_Return_FileNotExists() => await base.DeleteFileAsync_IfSuccess_Should_Return_FileNotExists();

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
    public override async Task GetDirectoriesAsync_IfSuccess_Should_ReturnDirectories() => await base.GetDirectoriesAsync_IfSuccess_Should_ReturnDirectories();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task GetDirectoriesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException() => await base.GetDirectoriesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override void GetFile_IfSuccess_Should_ReturnFile() => base.GetFile_IfSuccess_Should_ReturnFile();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override void GetFile_IfFileNotExist_Should_Throw_FileNotFoundException() => base.GetFile_IfFileNotExist_Should_Throw_FileNotFoundException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task GetFileAsync_IfSuccess_Should_ReturnFile() => await base.GetFileAsync_IfSuccess_Should_ReturnFile();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException() => await base.GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override void GetFiles_IfSuccess_Should_ReturnFiles() => base.GetFiles_IfSuccess_Should_ReturnFiles();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override void GetFiles_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException() => base.GetFiles_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task GetFilesAsync_IfSuccess_Should_ReturnFiles() => await base.GetFilesAsync_IfSuccess_Should_ReturnFiles();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task GetFilesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException() => await base.GetFilesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override void Instantiation_Prefix_Should_Return_Correct() => base.Instantiation_Prefix_Should_Return_Correct();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override void Instantiation_RootPath_Should_Return_Correct() => base.Instantiation_RootPath_Should_Return_Correct();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override void ReadFile_IfSuccess_Should_ReturnLength() => base.ReadFile_IfSuccess_Should_ReturnLength();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override void ReadFile_IfFileNotExist_Should_ThrowFileNotFoundException() => base.ReadFile_IfFileNotExist_Should_ThrowFileNotFoundException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task ReadFileAsync_IfSuccess_Should_ReturnLength() => await base.ReadFileAsync_IfSuccess_Should_ReturnLength();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task ReadFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException() => await base.ReadFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task ReadTextFileAsync_IfSuccess_Should_ReturnLength() => await base.ReadTextFileAsync_IfSuccess_Should_ReturnLength();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task ReadTextFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException() => await base.ReadTextFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException();
}
