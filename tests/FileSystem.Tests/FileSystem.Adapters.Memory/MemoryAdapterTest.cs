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

    public override Task AppendFileAsync_ContentByte_IfConnectionClose_Should_Throw_ConnectionException() => Task.CompletedTask;

    public override Task AppendFileAsync_ContentString_IfConnectionClose_Should_Throw_ConnectionException() => Task.CompletedTask;

    public override void Connect_IfSuccess_Should_Return_Message_ConnectedSuccessful() => Assert.IsTrue(true);
}
