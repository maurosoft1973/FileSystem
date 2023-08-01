using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maurosoft.FileSystem.Adapters;
using System;
using System.Threading.Tasks;
using FileSystem.Tests.Base;

namespace Tests.Base;

[TestClass]
public abstract class UnitTestAdapter<A> : TestAdapter<A> where A : Adapter
{
    public override string Prefix => typeof(A).FullName;
    public override string RootPath => "/";

    [TestInitialize]
    public void Init()
    {
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
    public override async Task CreateDirectoryAsync_Should_Exists() => await base.CreateDirectoryAsync_Should_Exists();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException() => await base.CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task DeleteFileAsync() => await base.DeleteFileAsync();

    [TestMethod]
    [TestCategory("UnitTest")]
    public override async Task DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException() => await base.DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException();

}
