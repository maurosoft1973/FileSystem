using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maurosoft.FileSystem.Adapters;
using Maurosoft.FileSystem.Exceptions;
using Tests.Base;

namespace Tests.FileSystem.Adapters.Local
{
    [TestClass]
    public class LocalAdapterTest : UnitTestAdapter<LocalAdapter>
    {
        public override string Prefix => "local1";
        public override string RootPath => AppDomain.CurrentDomain.BaseDirectory;

        [TestMethod("LocalAdapter_Instantiation_Prefix_Should_Return_Correct")]
        public override void Instantiation_Prefix_Should_Return_Correct() => base.Instantiation_Prefix_Should_Return_Correct();

        [TestMethod("LocalAdapter_Instantiation_RootPath_Should_Return_Correct")]
        public override void Instantiation_RootPath_Should_Return_Correct() => base.Instantiation_RootPath_Should_Return_Correct();

        [TestMethod("LocalAdapter_GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException")]
        public override async Task GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException() => await base.GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException();

        [TestMethod("LocalAdapter_GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles")]
        public override async Task GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles() => await base.GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles();

        [TestMethod("LocalAdapter_CreateDirectoryAsync_Should_Exists")]
        public override async Task CreateDirectoryAsync_Should_Exists() => await base.CreateDirectoryAsync_Should_Exists();

        [TestMethod("LocalAdapter_CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException")]
        public override async Task CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException() => await base.CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException();

        [TestMethod("LocalAdapter_DeleteFileAsync")]
        public override async Task DeleteFileAsync() => await base.DeleteFileAsync();

        [TestMethod("LocalAdapter_DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException")]
        public override async Task DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException() => await base.DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException();

    }
}