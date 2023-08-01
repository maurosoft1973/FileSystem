﻿using Maurosoft.FileSystem.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.InMemory;

namespace FileSystem.Tests.Base;

[TestClass]
public abstract class IntegrationTestAdapter<A, C> : TestAdapter<A> where A : Adapter
{
    public override string Prefix => typeof(A).FullName;
    public override string RootPath => "/";
    public C Client { get; set; }
    public ILogger Logger { get; protected set; }

    [TestInitialize]
    public void Init()
    {
        Log.CloseAndFlush();

        Logger = new LoggerConfiguration()
                     .WriteTo.InMemory()
                     .CreateLogger();

        _adapter = (A)Activator.CreateInstance(typeof(A), Prefix, RootPath, Client, Logger)!;
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public override void Instantiation_Prefix_Should_Return_Correct() => base.Instantiation_Prefix_Should_Return_Correct();

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public override void Instantiation_RootPath_Should_Return_Correct() => base.Instantiation_RootPath_Should_Return_Correct();

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public override async Task GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException() => await base.GetFileAsync_IfFileNotExist_Should_Throw_FileNotFoundException();

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public override async Task GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles() => await base.GetFilesAsync_IfWriteFileSameDirectory_Should_ReturnCorrectNumberOfFiles();

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public override async Task CreateDirectoryAsync_Should_Exists() => await base.CreateDirectoryAsync_Should_Exists();

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public override async Task CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException() => await base.CreateDirectoryAsync_IfExists_Should_ThrowDirectoryExistsException();

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public override async Task DeleteFileAsync() => await base.DeleteFileAsync();

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public override async Task DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException() => await base.DeleteFileAsync_IfNotExists_Should_ThrowFileNotFoundException();

}
