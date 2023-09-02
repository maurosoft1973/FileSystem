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
}
