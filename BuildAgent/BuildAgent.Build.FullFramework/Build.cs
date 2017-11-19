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
    #region Constants
    public static readonly List<string> Platforms = new List<string> { Platform.AnyCPU, Platform.x86, Platform.x64 };
    public class Platform
    {
      public const string AnyCPU = "Any CPU";
      public const string x86 = "x86";
      public const string x64 = "x64";
    }

    public static readonly List<string> Configurations = new List<string> { Configuration.Debug, Configuration.Release };
    public class Configuration
    {
      public const string Debug = "Debug";
      public const string Release = "Release";
    }

    public static readonly List<string> BuildTargets = new List<string> { "Build", "Clean" };
    public class BuildTarget
    {
      public const string Build = "Build";
      public const string Clean = "Clean";
    }

    public const string SOLUTION_EXTENSION = "*.sln";

    #endregion Constants

    public Build(string path = null)
    {
      StartupDir = Environment.CurrentDirectory;
      var solutionDirectory = path ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source");
      AvailableSolutionFiles = Directory.GetFileSystemEntries(solutionDirectory, SOLUTION_EXTENSION, SearchOption.AllDirectories);
    }

    public string[] AvailableSolutionFiles { get; private set; }
    public string StartupDir { get; set; }

    public void RunNonBlocking(string selectedSolution, string selectedPlatform, string selectedConfiguration, Action<string> buildLogAction, Action<string> buildDoneAction)
    {
      BuildLogger buildLogger = new BuildLogger();
      buildLogger.BuildEvent += buildLogAction;

      var loggers = new ILogger[]
      {
        new ConfigurableForwardingLogger
        {
          BuildEventRedirector = buildLogger,
          Verbosity = LoggerVerbosity.Diagnostic
        },
        new FileLogger
        {
          Verbosity = LoggerVerbosity.Diagnostic,
          ShowSummary = true
        }
      };

      Dictionary<string, string> globalProperties = new Dictionary<string, string>();
      globalProperties.Add("Configuration", selectedConfiguration);
      globalProperties.Add("Platform", selectedPlatform);

      ProjectCollection pc = new ProjectCollection(ToolsetDefinitionLocations.Registry);
      
      BuildRequestData buildRequest = new BuildRequestData(selectedSolution, 
                                                           globalProperties, 
                                                           null, 
                                                           new string[] { BuildTarget.Build }, 
                                                           null);


      BuildParameters buildParameters = new BuildParameters(pc) { Loggers = loggers };
      BuildManager.DefaultBuildManager.BeginBuild(buildParameters);
      Console.WriteLine("Build setup complete. starting now");
      var submission = BuildManager.DefaultBuildManager.PendBuildRequest(buildRequest);
      Console.WriteLine("Build sumitted!");
      submission.ExecuteAsync(OnBuildComplete, (Action<string>)OnBuildCompleteCallbackAction);

      void OnBuildCompleteCallbackAction(string result)
      {
         buildLogAction(result); buildDoneAction(result);
      }
    }

    private void OnBuildComplete(BuildSubmission submission)
    {
      BuildManager.DefaultBuildManager.EndBuild();

      if (submission.AsyncContext is Action<string> handler)
      {
        handler(Enum.GetName(typeof(BuildResultCode), submission.BuildResult.OverallResult));
      }
    }
  }
}
