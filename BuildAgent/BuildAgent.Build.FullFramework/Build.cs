using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace BuildAgent.Build.FullFramework
{
  public class Build
  {
    public static readonly List<string> Platform = new List<string>(new[]{ "Any CPU" ,"x86","x64"});
    public static readonly List<string> Configuration = new List<string>(new[] { "Debug", "Release" });
    public Build()
    {
      AvailableSolutionFiles = Directory.GetFileSystemEntries(@"C:\Users\marku\source\repos", "*.sln", SearchOption.AllDirectories);
    }

    public string[] AvailableSolutionFiles { get; private set; }

    public void RunNonBlocking(string selectedSolution, string selectedPlatform, string selectedConfiguration, Action<string> buildLogAction, Action buildDoneAction)
    {
      ProjectCollection pc = new ProjectCollection();
      Dictionary<string, string> GlobalProperty = new Dictionary<string, string>();
      GlobalProperty.Add("Configuration", selectedConfiguration);
      GlobalProperty.Add("Platform", selectedPlatform);

      BuildRequestData BuildRequest = new BuildRequestData(selectedSolution, 
                                                           GlobalProperty, 
                                                           null, 
                                                           new string[] {"Clean", "Build" }, 
                                                           null);
      BuildLogger buildLogger = new BuildLogger();
      buildLogger.BuildEvent += buildLogAction;

      var forwardLogger = new ConfigurableForwardingLogger
      {
        BuildEventRedirector = buildLogger
      };

      BuildParameters buildParameters = new BuildParameters(pc);
      buildParameters.Loggers = new ILogger[]{ forwardLogger };
      BuildManager.DefaultBuildManager.BeginBuild(buildParameters);
      Console.WriteLine("Build setup complete. starting now");
      var submission = BuildManager.DefaultBuildManager.PendBuildRequest(BuildRequest);
      Console.WriteLine("Build sumitted!");
      submission.ExecuteAsync(callback,new Action<string>(result => { buildLogAction(result); buildDoneAction(); }));
    }

    private void callback(BuildSubmission submission)
    {
      if(submission.AsyncContext is Action<string> handler)
      {
        handler(Enum.GetName(typeof(BuildResultCode), submission.BuildResult.OverallResult));
      }
      BuildManager.DefaultBuildManager.EndBuild();
    }

    
  }
}
