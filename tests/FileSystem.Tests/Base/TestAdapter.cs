using Bogus;
using FluentAssertions;
using Maurosoft.FileSystem.Adapters;
using Maurosoft.FileSystem.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FileSystem.Tests.Base;

public abstract class TestAdapter<A> where A : Adapter
{
    public abstract string Prefix { get; }
    public abstract string RootPath { get; }

    protected A _adapter;

    private readonly Faker faker = new();

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

    public virtual void Connect_IfSuccess_Should_Return_Message_ConnectedSuccsefull()
    {
        //Arrange
        Log.CloseAndFlush();

        //Act
        _adapter.Connect();

        //Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage("{Adapter} - Connected succsefull")
            .Appearing()
            .Once();
    }

    public virtual void GetFile_IfSuccess_Should_ReturnFile()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);

        _adapter!.CreateDirectory(directory);
        _adapter!.WriteFile(directory + "/file1", System.Text.Encoding.UTF8.GetBytes("file1"));

        //Act
        var file = _adapter!.GetFile(directory + "/file1");

        //Assert
        Assert.IsNotNull(file);
    }

    public virtual void GetFile_IfFileNotExist_Should_Throw_FileNotFoundException()
    {
        //Assert
        var aggregateException = Assert.ThrowsException<AggregateException>(() => _adapter!.GetFile("test.txt"));
        Assert.AreEqual(aggregateException.InnerException.GetType(), typeof(FileNotFoundException));
    }

    public virtual async Task GetFileAsync_IfSuccess_Should_ReturnFile()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);

        _adapter!.CreateDirectory(directory);
        _adapter!.WriteFile(directory + "/file1", System.Text.Encoding.UTF8.GetBytes("file1"));

        //Act
        var file = await _adapter!.GetFileAsync(directory + "/file1", new System.Threading.CancellationToken());

        //Assert
        Assert.IsNotNull(file);
    }

    public virtual async Task GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException()
    {
        //Assert
        await Assert.ThrowsExceptionAsync<FileNotFoundException>(async () => await _adapter!.GetFileAsync("test.txt"));
    }

    public virtual void GetFiles_IfSuccess_Should_ReturnFiles()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);

        _adapter!.CreateDirectory(directory);
        _adapter!.WriteFile(directory + "/file1", System.Text.Encoding.UTF8.GetBytes("file1"));
        _adapter!.WriteFile(directory + "/file2", System.Text.Encoding.UTF8.GetBytes("file2"));
        _adapter!.WriteFile(directory + "/file3", System.Text.Encoding.UTF8.GetBytes("file3"));
        _adapter!.WriteFile(directory + "/file4", System.Text.Encoding.UTF8.GetBytes("file4"));
        _adapter!.WriteFile(directory + "/file5", System.Text.Encoding.UTF8.GetBytes("file5"));
        _adapter!.WriteFile(directory + "/file1", System.Text.Encoding.UTF8.GetBytes("file1file1"), true);

        //Act
        var files = _adapter!.GetFiles(directory);

        //Assert
        Assert.AreEqual(5, files.Count());
    }

    public virtual void GetFiles_IfDirectoryNotExist_Should_Throw_DirectoryNotFoundException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);

        //Assert
        var aggregateException = Assert.ThrowsException<AggregateException>(() => _adapter!.GetFiles(directory));
        Assert.AreEqual(aggregateException.InnerException.GetType(), typeof(DirectoryNotFoundException));
    }

    public virtual async Task GetFilesAsync_IfSuccess_Should_ReturnFiles()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);

        _adapter!.CreateDirectory(directory);
        _adapter!.WriteFile(directory + "/file1", System.Text.Encoding.UTF8.GetBytes("file1"));
        _adapter!.WriteFile(directory + "/file2", System.Text.Encoding.UTF8.GetBytes("file2"));
        _adapter!.WriteFile(directory + "/file3", System.Text.Encoding.UTF8.GetBytes("file3"));
        _adapter!.WriteFile(directory + "/file4", System.Text.Encoding.UTF8.GetBytes("file4"));
        _adapter!.WriteFile(directory + "/file5", System.Text.Encoding.UTF8.GetBytes("file5"));
        _adapter!.WriteFile(directory + "/file1", System.Text.Encoding.UTF8.GetBytes("file1file1"), true);

        //Act
        var files = await _adapter!.GetFilesAsync(directory, new System.Threading.CancellationToken());

        //Assert
        Assert.AreEqual(5, files.Count());
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

    public virtual async Task DeleteFileAsync()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);
        await _adapter!.CreateDirectoryAsync(directory, new System.Threading.CancellationToken());
        _adapter!.WriteFile(directory + "/file1", System.Text.Encoding.UTF8.GetBytes("file1"));

        //Act
        await _adapter!.DeleteFileAsync(directory + "/file1");

        //Act and Assert
        Assert.IsFalse(_adapter!.FileExists(directory + "/file1"));
    }

    public virtual async Task DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);

        //Act and Assert
        await Assert.ThrowsExceptionAsync<FileNotFoundException>(async () => await _adapter!.DeleteFileAsync(directory + "/file1"));
    }

    public virtual async Task DeleteDirectoryAsync()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);
        await _adapter!.CreateDirectoryAsync(directory, new System.Threading.CancellationToken());

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

    public virtual void ReadFile_IfSuccess_Should_ReturnLength()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);
        _adapter!.CreateDirectory(directory);
        var fileName = faker.Database.Random.AlphaNumeric(50);
        _adapter!.WriteFile(directory + "/" + fileName, "ReadFile");

        //Act
        var readFiles = _adapter!.ReadFile(directory + "/" + fileName);

        //Act and Assert
        Assert.AreEqual(8, readFiles.Length);
    }

    public virtual void ReadFile_IfFileNotExist_Should_ThrowFileNotFoundException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);
        _adapter!.CreateDirectory(directory);
        var fileName = faker.Database.Random.AlphaNumeric(50);

        //Act and Assert
        Assert.ThrowsException<FileNotFoundException>(() => _adapter!.ReadFile(directory + "/" + fileName));
    }

    public virtual async Task ReadFileAsync_IfSuccess_Should_ReturnLength()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);
        await _adapter!.CreateDirectoryAsync(directory);
        var fileName = faker.Database.Random.AlphaNumeric(50);
        await _adapter!.WriteFileAsync(directory + "/" + fileName, "ReadFile");

        //Act
        var readFiles = await _adapter!.ReadFileAsync(directory + "/" + fileName);

        //Act and Assert
        Assert.AreEqual(8, readFiles.Length);
    }

    public virtual async Task ReadFileAsync_IfFileNotExist_Should_ThrowFileNotFoundException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);
        await _adapter!.CreateDirectoryAsync(directory);
        var fileName = faker.Database.Random.AlphaNumeric(50);

        //Act and Assert
        await Assert.ThrowsExceptionAsync<FileNotFoundException>(async () => await _adapter!.ReadFileAsync(directory + "/" + fileName));
    }

    public virtual void ReadTextFile_IfSuccess_Should_ReturnLength()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);
        _adapter!.CreateDirectory(directory);
        var fileName = faker.Database.Random.AlphaNumeric(50);
        _adapter!.WriteFile(directory + "/" + fileName, "ReadFile");

        //Act
        var readFile = _adapter!.ReadTextFile(directory + "/" + fileName);

        //Act and Assert
        Assert.AreEqual("ReadFile", readFile);
    }

    public virtual void ReadTextFile_IfFileNotExist_Should_ThrowFileNotFoundException()
    {
        //Arrange
        var directory = faker.Database.Random.AlphaNumeric(30);
        _adapter!.CreateDirectory(directory);
        var fileName = faker.Database.Random.AlphaNumeric(50);

        //Act and Assert
        Assert.ThrowsException<FileNotFoundException>(() => _adapter!.ReadTextFile(directory + "/" + fileName));
    }
}
