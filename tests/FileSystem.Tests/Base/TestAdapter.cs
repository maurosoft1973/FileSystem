using Bogus;
using FluentAssertions;
using Maurosoft.FileSystem.Adapters;
using Maurosoft.FileSystem.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DirectoryNotFoundException = Maurosoft.FileSystem.Exceptions.DirectoryNotFoundException;
using FileNotFoundException = Maurosoft.FileSystem.Exceptions.FileNotFoundException;

namespace FileSystem.Tests.Base;

public abstract class TestAdapter<A> where A : Adapter
{
    public abstract string Prefix { get; }
    public abstract string RootPath { get; }

    protected A _adapter;

    protected readonly Faker faker = new();

    public virtual void AppendFile_ContentByte_IfSuccess_Should_ReturnStringAppend()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        //Act
        _adapter!.AppendFile(directory + "/" + filename, System.Text.Encoding.UTF8.GetBytes("AppendTextFile"));

        //Assert
        var content = _adapter!.ReadFile(directory + "/" + filename);
        Assert.AreEqual("HelloWorldAppendTextFile", System.Text.Encoding.UTF8.GetString(content));
    }

    public virtual void AppendFile_ContentByte_IfFileNotExist_Should_Throw_FileNotFoundException()
    {
        //Assert
        Assert.ThrowsException<FileNotFoundException>(() => _adapter!.AppendFile("/test.txt", System.Text.Encoding.UTF8.GetBytes("AppendTextFile")));
    }

    public virtual void AppendFile_ContentByte_IfConnectionClose_Should_Throw_ConnectionException()
    {
        //Arrange
        _adapter.Disconnect();

        //Assert
        Assert.ThrowsException<ConnectionException>(() => _adapter!.AppendFile("/test.txt", System.Text.Encoding.UTF8.GetBytes("AppendTextFile")));
    }

    public virtual void AppendFile_ContentString_IfSuccess_Should_ReturnStringAppend()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        //Act
        _adapter!.AppendFile(directory + "/" + filename, "AppendTextFile");

        //Assert
        var content = _adapter!.ReadFile(directory + "/" + filename);
        Assert.AreEqual("HelloWorldAppendTextFile", System.Text.Encoding.UTF8.GetString(content));
    }

    public virtual void AppendFile_ContentString_IfFileNotExist_Should_Throw_FileNotFoundException()
    {
        //Assert
        Assert.ThrowsException<FileNotFoundException>(() => _adapter!.AppendFile("/test.txt", "AppendTextFile"));
    }

    public virtual void AppendFile_ContentString_IfConnectionClose_Should_Throw_ConnectionException()
    {
        //Act
        _adapter.Disconnect();

        //Assert
        Assert.ThrowsException<ConnectionException>(() => _adapter!.AppendFile("/test.txt", "AppendTextFile"));
    }

    public virtual async Task AppendFileAsync_ContentByte_IfSuccess_Should_ReturnStringAppend()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        //Act
        await _adapter!.AppendFileAsync(directory + "/" + filename, System.Text.Encoding.UTF8.GetBytes("AppendTextFile"));
        var content = _adapter!.ReadFile(directory + "/" + filename);

        //Assert
        Assert.AreEqual("HelloWorldAppendTextFile", System.Text.Encoding.UTF8.GetString(content));
    }

    public virtual async Task AppendFileAsync_ContentByte_IfFileNotExist_Should_Throw_FileNotFoundException()
    {
        //Act e Assert
        await Assert.ThrowsExceptionAsync<FileNotFoundException>(async () => await _adapter!.AppendFileAsync("/test.txt", System.Text.Encoding.UTF8.GetBytes("AppendTextFile")));
    }

    public virtual async Task AppendFileAsync_ContentByte_IfConnectionClose_Should_Throw_ConnectionException()
    {
        //Act
        _adapter.Disconnect();

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.AppendFileAsync("/test.txt", System.Text.Encoding.UTF8.GetBytes("AppendTextFile")));
    }

    public virtual async Task AppendFileAsync_ContentString_IfSuccess_Should_ReturnStringAppend()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        //Act
        await _adapter!.AppendFileAsync(directory + "/" + filename, "AppendTextFile");
        var content = _adapter!.ReadFile(directory + "/" + filename);

        //Assert
        Assert.AreEqual("HelloWorldAppendTextFile", System.Text.Encoding.UTF8.GetString(content));
    }

    public virtual async Task AppendFileAsync_ContentString_IfFileNotExist_Should_Throw_FileNotFoundException()
    {
        //Assert
        await Assert.ThrowsExceptionAsync<FileNotFoundException>(async () => await _adapter!.AppendFileAsync("/test.txt", "AppendTextFile"));
    }

    public virtual async Task AppendFileAsync_ContentString_IfConnectionClose_Should_Throw_ConnectionException()
    {
        //Act
        _adapter.Disconnect();

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.AppendFileAsync("/test.txt", "AppendTextFile"));
    }

    public virtual void Connect_IfSuccess_Should_Return_Message_ConnectedSuccessful()
    {
        //Arrange
        Log.CloseAndFlush();
        _adapter.Disconnect();

        //Act
        _adapter.Connect();

        //Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage("{Adapter} - Connected successful")
            .Appearing();
    }

    public virtual async Task CreateDirectoryAsync_IfSuccess_Should_ReturnDirectoryExists()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);

        //Act
        await _adapter!.CreateDirectoryAsync(directory, new System.Threading.CancellationToken());

        //Assert
        Assert.IsTrue(_adapter!.DirectoryExists(directory));
    }

    public virtual async Task CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);
        await _adapter!.CreateDirectoryAsync(directory, new System.Threading.CancellationToken());

        //Act and Assert
        await Assert.ThrowsExceptionAsync<DirectoryExistsException>(async () => await _adapter!.CreateDirectoryAsync(directory, new System.Threading.CancellationToken()));
    }

    public virtual async Task DeleteDirectoryAsync()
    {
        //Arrange
        var (directory, _, _, _) = CreateFile(false);

        //Act
        await _adapter!.DeleteDirectoryAsync(directory);

        //Act and Assert
        Assert.IsFalse(_adapter!.DirectoryExists(directory));
    }

    public virtual async Task DeleteDirectoryAsync_IfNotExists_Should_ThrowFileNotFoundException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);

        //Act and Assert
        await Assert.ThrowsExceptionAsync<DirectoryNotFoundException>(async () => await _adapter!.DeleteDirectoryAsync(directory));
    }

    public virtual async Task DeleteFileAsync_IfSuccess_Should_Return_FileNotExists()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        //Act
        await _adapter!.DeleteFileAsync(directory + "/" + filename);

        //Act and Assert
        Assert.IsFalse(_adapter!.FileExists(directory + "/" + filename));
    }

    public virtual async Task DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);

        //Act and Assert
        await Assert.ThrowsExceptionAsync<FileNotFoundException>(async () => await _adapter!.DeleteFileAsync(directory + "/file1"));
    }

    public virtual void GetFile_IfSuccess_Should_ReturnFile()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        //Act
        var file = _adapter!.GetFile(directory + "/" + filename);

        //Assert
        Assert.IsNotNull(file);
    }

    public virtual void GetFile_IfFileNotExist_Should_Throw_FileNotFoundException()
    {
        //Assert
        Assert.ThrowsException<FileNotFoundException>(() => _adapter!.GetFile("/test.txt"));
    }

    public virtual async Task GetFileAsync_IfSuccess_Should_ReturnFile()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        //Act
        var file = await _adapter!.GetFileAsync(directory + "/" + filename, new System.Threading.CancellationToken());

        //Assert
        Assert.IsNotNull(file);
    }

    public virtual async Task GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException()
    {
        //Assert
        await Assert.ThrowsExceptionAsync<FileNotFoundException>(async () => await _adapter!.GetFileAsync("/test.txt"));
    }

    public virtual async Task GetFileAsync_IfConnectionClose_Should_Throw_Exception()
    {
        //Act
        _adapter.Disconnect();

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.GetFileAsync("/test.txt"));
    }

    public virtual void GetFiles_IfSuccess_Should_ReturnFiles()
    {
        //Arrange
        var (directory, filenames, _) = CreateFiles();

        //Act
        var files = _adapter!.GetFiles(directory);

        //Assert
        Assert.AreEqual(filenames.Length, files.Count());
    }

    public virtual void GetFiles_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);

        //Assert
        Assert.ThrowsException<DirectoryNotFoundException>(() => _adapter!.GetFiles(directory));
    }

    public virtual async Task GetFilesAsync_IfSuccess_Should_ReturnFiles()
    {
        //Arrange
        var (directory, filenames, _) = CreateFiles();

        //Act
        var files = await _adapter!.GetFilesAsync(directory, new System.Threading.CancellationToken());

        //Assert
        Assert.AreEqual(filenames.Length, files.Count());
    }

    public virtual async Task GetFilesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);

        //Assert
        await Assert.ThrowsExceptionAsync<DirectoryNotFoundException>(async () => await _adapter!.GetFilesAsync(directory));
    }

    public virtual async Task GetDirectoriesAsync_IfSuccess_Should_ReturnDirectories()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);
        var directory1 = faker.Database.Random.AlphaNumeric(30);
        var directory2 = faker.Database.Random.AlphaNumeric(30);
        var directory3 = faker.Database.Random.AlphaNumeric(30);
        var directory4 = faker.Database.Random.AlphaNumeric(30);
        var directory5 = faker.Database.Random.AlphaNumeric(30);

        _adapter!.CreateDirectory(directory);
        _adapter!.CreateDirectory(directory + "/" + directory1);
        _adapter!.CreateDirectory(directory + "/" + directory2);
        _adapter!.CreateDirectory(directory + "/" + directory3);
        _adapter!.CreateDirectory(directory + "/" + directory4);
        _adapter!.CreateDirectory(directory + "/" + directory5);

        //Act
        var directories = await _adapter!.GetDirectoriesAsync(directory, new System.Threading.CancellationToken());

        //Assert
        Assert.AreEqual(5, directories.Count());
    }

    public virtual async Task GetDirectoriesAsync_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);

        //Assert
        await Assert.ThrowsExceptionAsync<DirectoryNotFoundException>(async () => await _adapter!.GetDirectoriesAsync(directory));
    }

    public virtual void Instantiation_Prefix_Should_Return_Correct()
    {
        //Assert
        Assert.AreEqual(Prefix, _adapter!.Prefix);
    }

    public virtual void Instantiation_RootPath_Should_Return_Correct()
    {
        //Assert
        Assert.AreEqual(RootPath, _adapter!.RootPath);
    }

    public virtual void ReadFile_IfSuccess_Should_ReturnLength()
    {
        //Arrange
        var (directory, filename, _, content) = CreateFile();

        //Act
        var readFiles = _adapter!.ReadFile(directory + "/" + filename);

        //Assert
        Assert.AreEqual(content.Length, readFiles.Length);
    }

    public virtual void ReadFile_IfFileNotExist_Should_ThrowFileNotFoundException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile(false);

        //Act and Assert
        Assert.ThrowsException<FileNotFoundException>(() => _adapter!.ReadFile(directory + "/" + filename));
    }

    public virtual async Task ReadFileAsync_IfSuccess_Should_ReturnLength()
    {
        //Arrange
        var (directory, filename, _, content) = CreateFile();

        //Act
        var readFiles = await _adapter!.ReadFileAsync(directory + "/" + filename);

        //Act and Assert
        Assert.AreEqual(content.Length, readFiles.Length);
    }

    public virtual async Task ReadFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile(false);

        //Act and Assert
        await Assert.ThrowsExceptionAsync<FileNotFoundException>(async () => await _adapter!.ReadFileAsync(directory + "/" + filename));
    }

    public virtual async Task ReadFileAsync_IfConnectionClose_Should_Throw_Exception()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile();

        //Act
        _adapter.Disconnect();

        //Assert
        await Assert.ThrowsExceptionAsync<ConnectionException>(async () => await _adapter!.ReadFileAsync(directory + "/" + filename));
    }

    public virtual void ReadTextFile_IfSuccess_Should_ReturnLength()
    {
        //Arrange
        var (directory, filename, _, content) = CreateFile();

        //Act
        var readFile = _adapter!.ReadTextFile(directory + "/" + filename);

        //Act and Assert
        Assert.AreEqual(content, readFile);
    }

    public virtual void ReadTextFile_IfFileNotExist_Should_ThrowFileNotFoundException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile(false);

        //Act and Assert
        Assert.ThrowsException<FileNotFoundException>(() => _adapter!.ReadTextFile(directory + "/" + filename));
    }

    public virtual async Task ReadTextFileAsync_IfSuccess_Should_ReturnLength()
    {
        //Arrange
        var (directory, filename, _, content) = CreateFile();

        //Act
        var readFile = await _adapter!.ReadTextFileAsync(directory + "/" + filename);

        //Act and Assert
        Assert.AreEqual(content, readFile);
    }

    public virtual async Task ReadTextFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException()
    {
        //Arrange
        var (directory, filename, _, _) = CreateFile(false);

        //Act and Assert
        await Assert.ThrowsExceptionAsync<FileNotFoundException>(async () => await _adapter!.ReadTextFileAsync(directory + "/" + filename));
    }

    protected (string directory, string filename, bool writefile, string content) CreateFile(bool writefile = true, string content = "HelloWorld")
    {
        var directory = faker.Database.Random.String2(30, 50);
        _adapter.CreateDirectory(directory);

        var filename = faker.Database.Random.String2(30, 50);
        if (writefile)
            _adapter.WriteFile(directory + "/" + filename, content);

        return new(directory, filename, writefile, content);
    }

    protected (string directory, string[] filesname, string content) CreateFiles(int numFiles = 5, string content = "HelloWorld")
    {
        var directory = faker.Database.Random.String2(30);
        _adapter.CreateDirectory(directory);

        var filenames = new string[numFiles];

        for (var i = 0; i < numFiles; i++)
        {
            filenames[i] = faker.Database.Random.String2(30);
            _adapter.WriteFile(directory + "/" + filenames[i], content);
        }

        return new(directory, filenames, content);
    }
}
