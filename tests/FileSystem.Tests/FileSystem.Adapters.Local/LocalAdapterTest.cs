using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maurosoft.FileSystem.Adapters;
using Maurosoft.FileSystem.Exceptions;
using Tests.Base;
using Xunit;

namespace Tests.FileSystem.Adapters.Local;

[TestClass]
public class LocalAdapterTest : UnitTestAdapter<LocalAdapter>
{
    public override string Prefix => "local1";
    public override string RootPath => AppDomain.CurrentDomain.BaseDirectory;

}