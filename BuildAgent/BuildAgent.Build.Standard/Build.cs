using System;
using System.Collections.Generic;
using System.Text;

namespace BuildAgent.Build.Standard
{
  public class Build
  {
    public Build()
    {
      string projectFileName = @"...\ConsoleApplication3\ConsoleApplication3.sln";
      //ProjectCollection pc = new ProjectCollection();
      Dictionary<string, string> GlobalProperty = new Dictionary<string, string>();
      GlobalProperty.Add("Configuration", "Debug");
      GlobalProperty.Add("Platform", "x86");

      //BuildRequestData BuidlRequest = new BuildRequestData(projectFileName, GlobalProperty, null, new string[] { "Build" }, null);

      //BuildResult buildResult = BuildManager.DefaultBuildManager.Build(new BuildParameters(pc), BuidlRequest);
    }
  }
}
