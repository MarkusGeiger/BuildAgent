using Newtonsoft.Json;
using System;
using System.IO;

namespace BuildAgent.Config
{
  public class Config
  {
    private static Config _instance;
    private string _configFilePath;
    private PersistentSettings _persistency;

    private Config()
    {
      _configFilePath = Path.Combine(Environment.CurrentDirectory, "persistency.json");
      if (File.Exists(_configFilePath))
      {
        _persistency = JsonConvert.DeserializeObject<PersistentSettings>(File.ReadAllText(_configFilePath));
      }
      else
      {
        _persistency = new PersistentSettings();
        File.WriteAllText(_configFilePath, JsonConvert.SerializeObject(_persistency));
      }
    }

    public string LastUsedSolutionFile
    {
      get => _persistency.SolutionFile;
      set
      {
        _persistency.SolutionFile = value;
        File.WriteAllText(_configFilePath, JsonConvert.SerializeObject(_persistency));
      }
    }

    public string LastUsedRepo
    {
      get => _persistency.Repository;
      set
      {
        _persistency.Repository = value;
        File.WriteAllText(_configFilePath, JsonConvert.SerializeObject(_persistency));
      }
    }

    public static Config Instance
    {
      get => _instance ?? (_instance = new Config());
    }
  }
}
