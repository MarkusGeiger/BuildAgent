using Prism.Commands;
using Prism.Mvvm;
using System.Diagnostics;
using System.Windows.Input;
using System.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using System.Threading;
using System.Windows;

namespace BuildAgent.Build.WPF
{
  public class ViewModel : BindableBase
  {
    private const bool NO_SUPRESSION = true;
    private double _duration;
    private DelegateCommand _startBuildCommand;

    public double Duration
    {
      get { return _duration; }
      set { SetProperty(ref _duration, value); }
    }

    private string _status;

    public string Status
    {
      get { return _status; }
      set { SetProperty(ref _status, value); }
    }

    private FullFramework.Build _build;
    private Stopwatch _sw;
    private ObservableCollection<string> _logText;

    public ObservableCollection<string> LogText
    {
      get { return _logText; }
      set { SetProperty(ref _logText, value); }
    }



    public ViewModel()
    {
      _dispatcher = Application.Current.Dispatcher;
      _startupDirectory = Environment.CurrentDirectory;
      LogText = new ObservableCollection<string>();

      if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
      {
        Duration = 17.46278;
        Status = "Design";
        LogText.Add("Very");
        LogText.Add("Looooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong");
        LogText.Add("logTeXt");
        SolutionFileList = new ObservableCollection<string>(new[] { "Solution1", "Solution2", "SOlution3" });
      }
      else
      {
        Status = "Wait";
        _build = new FullFramework.Build(Config.Config.Instance.LastUsedRepo);
        SolutionFileList = new ObservableCollection<string>(_build.AvailableSolutionFiles);
      }

      SelectedSolution = SolutionFileList.First();
      SelectedConfiguration = FullFramework.Build.Configurations.First();
      SelectedPlatform = FullFramework.Build.Platforms.First();
    }

    private ObservableCollection<string> _solutionFileList;
    public ObservableCollection<string> SolutionFileList
    {
      get { return _solutionFileList; }
      set { SetProperty(ref _solutionFileList, value); }
    }
    
    public ICommand StartBuildCommand => _startBuildCommand ??
      (_startBuildCommand = new DelegateCommand(OnStartBuild,
        () => SolutionFileList != null && SolutionFileList.Count > 0 && !String.IsNullOrWhiteSpace(SelectedSolution)
        && !String.IsNullOrWhiteSpace(SelectedConfiguration) && !String.IsNullOrWhiteSpace(SelectedPlatform)));

    private string _selectedSolution;
    public string SelectedSolution
    {
      get { return _selectedSolution; }
      set
      {
        SetProperty(ref _selectedSolution, value);
        Config.Config.Instance.LastUsedSolutionFile = _selectedSolution;
        RaiseCanExecuteChanged();
      }
    }

    private void RaiseCanExecuteChanged()
    {
      _startBuildCommand?.RaiseCanExecuteChanged();

    }

    private string _selectedConfiguration;
    public string SelectedConfiguration
    {
      get { return _selectedConfiguration; }
      set
      {
        SetProperty(ref _selectedConfiguration, value);
        RaiseCanExecuteChanged();
      }
    }

    private string _selectedPlatform;
    private Dispatcher _dispatcher;
    private string _startupDirectory;

    public string SelectedPlatform
    {
      get { return _selectedPlatform; }
      set
      {
        SetProperty(ref _selectedPlatform, value);
        RaiseCanExecuteChanged();
      }
    }

    private void OnStartBuild()
    {
      Duration = 0;
      Status = "Init";
      LogText.Clear();
      _sw = new Stopwatch();
      _sw.Start();
      Status = "Pending";

      _build.RunNonBlocking(
        SelectedSolution,
        SelectedPlatform,
        SelectedConfiguration,
        OnLogMessage,
        OnFinished
      );

      void OnFinished(string message)
      {
        _sw.Stop();
        Duration = _sw.Elapsed.TotalSeconds;
        Status = $"Done - {message}";
      }

      void OnLogMessage(string message)
      {
        if (NO_SUPRESSION || message.Contains(" -- "))
        {
          //if (LogText.Count > 50) _dispatcher.Invoke(new Action(()=>LogText.RemoveAt(0)));
          _dispatcher.BeginInvoke(new Action<string>(LogText.Add), message);
        }
        Duration = _sw.Elapsed.TotalSeconds;
      }
    }
  }
}
