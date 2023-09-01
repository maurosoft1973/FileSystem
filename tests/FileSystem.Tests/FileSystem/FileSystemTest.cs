using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maurosoft.FileSystem.Adapters;
using Maurosoft.FileSystem.Exceptions;
using Serilog;

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
#pragma warning disable S1481 // Unused local variables should be removed
        var localAdapter1 = new LocalAdapter("prefix-1", "/");
#pragma warning restore S1481 // Unused local variables should be removed
        var fileSystem = new Maurosoft.FileSystem.FileSystem();

        //Act && Assert
        Assert.ThrowsException<NoAdaptersRegisteredException>(() => fileSystem.GetAdapter("prefix-1"));
    }
}