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

    public virtual void Connect_ClientExist_Should_Return_Message_ConnectedSuccsefull()
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

    public virtual async Task GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException()
    {
        //Assert
        await Assert.ThrowsExceptionAsync<FileNotFoundException>(async () => await _adapter!.GetFileAsync("test.txt"));
    }

    public virtual async Task GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles()
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

    public virtual async Task CreateDirectoryAsync_Should_Exists()
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

    public virtual void ReadFile()
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

    public virtual async Task ReadFileAsync()
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
}
