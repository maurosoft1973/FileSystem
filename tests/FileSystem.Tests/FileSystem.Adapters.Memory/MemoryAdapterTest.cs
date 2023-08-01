using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maurosoft.FileSystem.Adapters.Memory;
using System.Threading.Tasks;
using Tests.Base;

namespace Tests.FileSystem.Adapters.Memory;

[TestClass]
public class MemoryAdapterTest : UnitTestAdapter<MemoryAdapter>
{
    public override string Prefix => "memory1";

    public override string RootPath => "/";

    [TestMethod("MemoryAdapter_Instantiation_Prefix_Should_Return_Correct")]
    public override void Instantiation_Prefix_Should_Return_Correct() => base.Instantiation_Prefix_Should_Return_Correct();

    [TestMethod("MemoryAdapter_Instantiation_RootPath_Should_Return_Correct")]
    public override void Instantiation_RootPath_Should_Return_Correct() => base.Instantiation_RootPath_Should_Return_Correct();

    [TestMethod("MemoryAdapter_GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException")]
    public override async Task GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException() => await base.GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException();

    [TestMethod("MemoryAdapter_GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles")]
    public override async Task GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles() => await base.GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles();

    [TestMethod("MemoryAdapter_CreateDirectoryAsync_Should_Exists")]
    public override async Task CreateDirectoryAsync_Should_Exists() => await base.CreateDirectoryAsync_Should_Exists();

    [TestMethod("MemoryAdapter_CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException")]
    public override async Task CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException() => await base.CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException();

    [TestMethod("MemoryAdapter_DeleteFileAsync")]
    public override async Task DeleteFileAsync() => await base.DeleteFileAsync();

    [TestMethod("MemoryAdapter_DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException")]
    public override async Task DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException() => await base.DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException();
}
