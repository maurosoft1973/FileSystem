using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maurosoft.FileSystem.Adapters;
using Tests.Base;

namespace Tests.FileSystem.Adapters.Local;

[TestClass]
public class LocalAdapterTest : UnitTestAdapter<LocalAdapter>
{
    public override string Prefix => "local1";
    public override string RootPath => AppDomain.CurrentDomain.BaseDirectory;

    public override Task AppendFileAsync_ContentByte_IfConnectionClose_Should_Throw_ConnectionException() => Task.CompletedTask;

    public override Task AppendFileAsync_ContentString_IfConnectionClose_Should_Throw_ConnectionException() => Task.CompletedTask;

    public override void Connect_IfSuccess_Should_Return_Message_ConnectedSuccessful() => Assert.IsTrue(true);
}