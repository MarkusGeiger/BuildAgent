using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildAgent.Tests
{
  [TestClass]
  public class BuildTests
  {
    [TestMethod]
    public void ConstructBuildAndReturnPath()
    {
      var testString = @"C:\TestDirectory\TestPath";
      var build = new BuildAgent.Build.FullFramework.Build();
      Assert.AreEqual(testString, build.SolutionDirectory);
    }
  }
}
