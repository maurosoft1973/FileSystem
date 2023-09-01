using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maurosoft.FileSystem.Adapters;
using Maurosoft.FileSystem.Exceptions;
using Serilog;
using Maurosoft.FileSystem.Adapters.Memory;
using System.Text;
using System;
using System.Linq;

namespace Tests.FileSystem;

[TestClass]
public class FileSystemTest
{
    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_Instantiation_NoRegistredAdapter_Should_Return_Zero_Adapter()
    {
        var fileSystem1 = new Maurosoft.FileSystem.FileSystem();

        Assert.AreEqual(0, fileSystem1.Adapters.Count);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_Instantiation_RegisteredAdapter_Should_Return_Correct_Number()
    {
        var localAdapter1 = new LocalAdapter("local1", "/");
        var localAdapter2 = new LocalAdapter("local2", "/");
        var localAdapter3 = new LocalAdapter("local3", "/");

        var adapters = new List<IAdapter>
        {
            localAdapter1,
            localAdapter2,
            localAdapter3
        };

        var fileSystem = new Maurosoft.FileSystem.FileSystem(adapters);

        Assert.AreEqual(3, fileSystem.Adapters.Count);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_Adapters_Add_Should_Return_Adapter()
    {
        //Arrange
        var localAdapter1 = new LocalAdapter("local1", "/");
        var fileSystem = new Maurosoft.FileSystem.FileSystem();

        //Act
        fileSystem.Adapters.Add(localAdapter1);

        //Assert
        Assert.AreEqual(localAdapter1, fileSystem.GetAdapter("local1"));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_GetAdapter_NoRegistred_Should_ThrowException_NoAdaptersRegisteredException()
    {
        //Arrange
        var localAdapter1 = new LocalAdapter("prefix-1", "/");
        var fileSystem = new Maurosoft.FileSystem.FileSystem();

        //Act && Assert
        Assert.ThrowsException<NoAdaptersRegisteredException>(() => fileSystem.GetAdapter("prefix-1"));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_GetAdapter_RegistredWithSamePrefix_Should_ThrowException_DuplicateAdapterPrefixException()
    {
        //Arrange
        var localAdapter1 = new LocalAdapter("prefix-1", "/");
        var localAdapter2 = new LocalAdapter("prefix-1", "/");
        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(localAdapter1);
        fileSystem.Adapters.Add(localAdapter2);

        //Act && Assert
        Assert.ThrowsException<DuplicateAdapterPrefixException>(() => fileSystem.GetAdapter("prefix-1"));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_GetAdapter_WithPrefixNotRegistred_Should_ThrowException_AdapterNotFoundException()
    {
        //Arrange
        var localAdapter1 = new LocalAdapter("prefix-1", "/");
        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(localAdapter1);

        //Act && Assert
        Assert.ThrowsException<AdapterNotFoundException>(() => fileSystem.GetAdapter("prefix-2"));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_GetFile_IfFileExists_Should_ReturnFile()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.WriteFile("helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act
        var file = fileSystem.GetFile("memory-1://helloworld.txt");

        //Assert
        Assert.AreEqual("helloworld.txt", file.Name);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_GetFile_IfFileNotExists_Should_ThrowException_FileNotFoundException()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.WriteFile("helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act & Assert
        var aggregateException = Assert.ThrowsException<AggregateException>(() => fileSystem.GetFile("memory-1://helloworld1.txt"));
        Assert.AreEqual(aggregateException.InnerException.GetType(), typeof(FileNotFoundException));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    [DataRow("memory-1:/helloworld.txt", DisplayName = "")]
    [DataRow("memory-1:helloworld.txt", DisplayName = "")]
    [DataRow("memory-1helloworld.txt", DisplayName = "")]
    public void FileSystem_GetFile_IfPrefixIsInvalid_Should_ThrowException_PrefixNotFoundInPathException(string prefix)
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.WriteFile("helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act & Assert
        Assert.ThrowsException<PrefixNotFoundInPathException>(() => fileSystem.GetFile(prefix));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_GetDirectory_IfDirectoryExists_Should_ReturnDirectory()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.CreateDirectory("helloworld");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act
        var directory = fileSystem.GetDirectory("memory-1://helloworld");

        //Assert
        Assert.AreEqual("/helloworld", directory.Path);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_GetDirectory_IfDirectoryNotExists_Should_ThrowException_DirectoryNotFoundException()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.CreateDirectory("helloworld");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act & Assert
        var aggregateException = Assert.ThrowsException<AggregateException>(() => fileSystem.GetDirectory("memory-1://helloworld1"));
        Assert.AreEqual(aggregateException.InnerException.GetType(), typeof(DirectoryNotFoundException));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    [DataRow("memory-1:/helloworld", DisplayName = "")]
    [DataRow("memory-1:helloworld", DisplayName = "")]
    [DataRow("memory-1helloworld", DisplayName = "")]
    public void FileSystem_GetDirectory_IfPrefixIsInvalid_Should_ThrowException_PrefixNotFoundInPathException(string prefix)
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.CreateDirectory("helloworld");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act & Assert
        Assert.ThrowsException<PrefixNotFoundInPathException>(() => fileSystem.GetDirectory(prefix));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_GetFiles_IfFilesExists_Should_ReturnFiles()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.WriteFile("/home/helloworld1.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));
        memoryAdapter.WriteFile("/home/helloworld2.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act
        var files = fileSystem.GetFiles("memory-1://home").ToList();

        //Assert
        Assert.AreEqual(2, files.Count);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_GetFiles_IfPathNotExists_Should_ThrowException_DirectoryNotFoundException()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.WriteFile("/home/helloworld1.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));
        memoryAdapter.WriteFile("/home/helloworld2.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act & Assert
        var aggregateException = Assert.ThrowsException<AggregateException>(() => fileSystem.GetFiles("memory-1://home1"));
        Assert.AreEqual(aggregateException.InnerException.GetType(), typeof(DirectoryNotFoundException));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_GetDirectories_IfPathExists_Should_ReturnDirectories()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.CreateDirectory("home");
        memoryAdapter.CreateDirectory("home/home1");
        memoryAdapter.CreateDirectory("home/home2");
        memoryAdapter.CreateDirectory("home/home3");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act
        var directories = fileSystem.GetDirectories("memory-1://home").ToList();

        //Assert
        Assert.AreEqual(3, directories.Count);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_GetDirectories_IfPathNotExists_Should_ThrowException_DirectoryNotFoundException()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.CreateDirectory("home");
        memoryAdapter.CreateDirectory("home/home1");
        memoryAdapter.CreateDirectory("home/home2");
        memoryAdapter.CreateDirectory("home/home3");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act & Assert
        var aggregateException = Assert.ThrowsException<AggregateException>(() => fileSystem.GetDirectories("memory-1://home1"));
        Assert.AreEqual(aggregateException.InnerException.GetType(), typeof(DirectoryNotFoundException));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_FileExists_IfPathExists_Should_ReturnTrue()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.WriteFile("/home/helloworld1.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act
        var fileExists = fileSystem.FileExists("memory-1://home/helloworld1.txt");

        //Assert
        Assert.IsTrue(fileExists);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_FileExists_IfPathNotExists_Should_ReturnFalse()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.WriteFile("/home/helloworld1.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act
        var fileExists = fileSystem.FileExists("memory-1://home/helloworld2.txt");

        //Assert
        Assert.IsFalse(fileExists);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_DirectoryExists_IfPathExists_Should_ReturnTrue()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.CreateDirectory("home");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act
        var dirExists = fileSystem.DirectoryExists("memory-1://home");

        //Assert
        Assert.IsTrue(dirExists);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_DirectoryExists_IfPathNotExists_Should_ReturnFalse()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.CreateDirectory("home");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act
        var dirExists = fileSystem.DirectoryExists("memory-1://home1");

        //Assert
        Assert.IsFalse(dirExists);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_CreateDirectory_IfSuccess_Should_ReturnExistsTrue()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act
        fileSystem.CreateDirectory("memory-1://home");

        //Assert
        Assert.IsTrue(fileSystem.DirectoryExists("memory-1://home"));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_DeleteDirectory_IfSuccess_Should_ReturnExistsFalse()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);
        fileSystem.CreateDirectory("memory-1://home");

        //Act
        fileSystem.DeleteDirectory("memory-1://home");

        //Assert
        Assert.IsFalse(fileSystem.DirectoryExists("memory-1://home"));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_DeleteFile_IfSuccess_Should_ReturnExistsFalse()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.WriteFile("/home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act
        fileSystem.DeleteFile("memory-1://home/helloworld.txt");

        //Assert
        Assert.IsFalse(fileSystem.FileExists("memory-1://helloworld.txt"));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_ReadFile_IfExist_Should_ReturnContents()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.WriteFile("/home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act
        var contents = fileSystem.ReadFile("memory-1://home/helloworld.txt");

        //Assert
        Assert.AreEqual("HelloWorld", System.Text.Encoding.UTF8.GetString(contents));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_ReadTextFile_IfExist_Should_ReturnContents()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");
        memoryAdapter.WriteFile("/home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act
        var contents = fileSystem.ReadTextFile("memory-1://home/helloworld.txt");

        //Assert
        Assert.AreEqual("HelloWorld", contents);
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_WriteFile_IfSuccess_Should_ReturnFileExists()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act
        fileSystem.WriteFile("memory-1://home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        //Assert
        Assert.IsTrue(fileSystem.FileExists("memory-1://home/helloworld.txt"));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_WriteFile_IfOverwriteFile_Should_ReturnContentOverWrite()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);
        fileSystem.WriteFile("memory-1://home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        //Act
        fileSystem.WriteFile("memory-1://home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorldOverWrite"), true);
        var file = fileSystem.GetFile("memory-1://home/helloworld.txt");

        //Assert
        Assert.AreEqual("HelloWorldOverWrite", System.Text.Encoding.UTF8.GetString(file.Content));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_WriteFileString_IfSuccess_Should_ReturnFileExists()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);

        //Act
        fileSystem.WriteFile("memory-1://home/helloworld.txt", "HelloWorld");

        //Assert
        Assert.IsTrue(fileSystem.FileExists("memory-1://home/helloworld.txt"));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_CopyFile_IfSuccess_Should_ReturnExistsDestinationFile()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);
        fileSystem.WriteFile("memory-1://home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        //Act
        fileSystem.CopyFile("memory-1://home/helloworld.txt", "memory-1://home/helloworld-copy.txt");

        //Assert
        Assert.IsTrue(fileSystem.FileExists("memory-1://home/helloworld-copy.txt"));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_CopyFile_IfOverWriteFile_Should_ReturnContentSourceFile()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);
        fileSystem.WriteFile("memory-1://home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorldOverWrite"));
        fileSystem.WriteFile("memory-1://home/helloworld-copy.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorldCopy"));

        //Act
        fileSystem.CopyFile("memory-1://home/helloworld.txt", "memory-1://home/helloworld-copy.txt", true);
        var file = fileSystem.GetFile("memory-1://home/helloworld-copy.txt");

        //Assert
        Assert.AreEqual("HelloWorldOverWrite", System.Text.Encoding.UTF8.GetString(file.Content));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_CopyFile_AcrossAdapter_IfSuccess_Should_ReturnExistsFile()
    {
        //Arrange
        var memoryAdapter1 = new MemoryAdapter("memory-1", "/");
        var memoryAdapter2 = new MemoryAdapter("memory-2", "/");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter1);
        fileSystem.Adapters.Add(memoryAdapter2);
        fileSystem.WriteFile("memory-1://home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        //Act
        fileSystem.CopyFile("memory-1://home/helloworld.txt", "memory-2://home/helloworld.txt");

        //Assert
        Assert.IsTrue(fileSystem.FileExists("memory-2://home/helloworld.txt"));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_CopyFile_AcrossAdapter_IfOverWriteSuccess_Should_ReturnContentOverWrite()
    {
        //Arrange
        var memoryAdapter1 = new MemoryAdapter("memory-1", "/");
        var memoryAdapter2 = new MemoryAdapter("memory-2", "/");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter1);
        fileSystem.Adapters.Add(memoryAdapter2);
        fileSystem.WriteFile("memory-1://home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorldNew"));
        fileSystem.WriteFile("memory-2://home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorldOld"));

        //Act
        fileSystem.CopyFile("memory-1://home/helloworld.txt", "memory-2://home/helloworld.txt", true);
        var file = fileSystem.GetFile("memory-2://home/helloworld.txt");

        //Assert
        Assert.AreEqual("HelloWorldNew", System.Text.Encoding.UTF8.GetString(file.Content));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_AppendFileByte_IfSuccess_Should_ReturnNewContentFile()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);
        fileSystem.WriteFile("memory-1://home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        //Act
        fileSystem.AppendFile("memory-1://home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("AppendFile"));
        var file = fileSystem.GetFile("memory-1://home/helloworld.txt");

        //Assert
        Assert.AreEqual("HelloWorldAppendFile", System.Text.Encoding.UTF8.GetString(file.Content));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_AppendFileString_IfSuccess_Should_ReturnNewContentFile()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);
        fileSystem.WriteFile("memory-1://home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        //Act
        fileSystem.AppendFile("memory-1://home/helloworld.txt", "AppendFile");
        var file = fileSystem.GetFile("memory-1://home/helloworld.txt");

        //Assert
        Assert.AreEqual("HelloWorldAppendFile", System.Text.Encoding.UTF8.GetString(file.Content));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_MoveFile_IfSuccess_Should_ReturnFileSourceNotExist()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);
        fileSystem.WriteFile("memory-1://home/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld"));

        //Act
        fileSystem.MoveFile("memory-1://home/helloworld.txt", "memory-1://home1/helloworld.txt");

        //Assert
        Assert.IsFalse(fileSystem.FileExists("memory-1://home/helloworld.txt"));
    }

    [TestMethod]
    [TestCategory("UnitTest")]
    public void FileSystem_MoveFile_IfOverWriteDestinationFile_Should_ReturnFileSourceNotExist()
    {
        //Arrange
        var memoryAdapter = new MemoryAdapter("memory-1", "/");

        var fileSystem = new Maurosoft.FileSystem.FileSystem();
        fileSystem.Adapters.Add(memoryAdapter);
        fileSystem.WriteFile("memory-1://home1/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld1"));
        fileSystem.WriteFile("memory-1://home2/helloworld.txt", System.Text.Encoding.UTF8.GetBytes("HelloWorld2"));

        //Act
        fileSystem.MoveFile("memory-1://home1/helloworld.txt", "memory-1://home2/helloworld.txt", true);
        var file = fileSystem.GetFile("memory-1://home2/helloworld.txt");

        //Assert
        Assert.AreEqual("HelloWorld1", System.Text.Encoding.UTF8.GetString(file.Content));
    }
}