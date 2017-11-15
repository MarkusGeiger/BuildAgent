using Prism.Commands;
using Prism.Mvvm;
using System.Diagnostics;
using System.Windows.Input;
using System.ComponentModel;
using System;
using BuildAgent.Build.FullFramework;
using System.Collections.ObjectModel;

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
    private string _logText;

    public string LogText
    {
      get { return _logText; }
      set { SetProperty(ref _logText, value); }
    }



    public ViewModel()
    {
      if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
      {
        Duration = 17.46278;
        Status = "Design";
        LogText = "Very Looooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong logTeXt";
        SolutionFileList = new ObservableCollection<string>(new[] { "Solution1", "Solution2", "SOlution3" });
      }
      else
      {
        Status = "Wait";
        _build = new FullFramework.Build();
        SolutionFileList = new ObservableCollection<string>(_build.AvailableSolutionFiles);
      }
    }

    private ObservableCollection<string> _solutionFileList; 

    public ObservableCollection<string> SolutionFileList
    {
      get { return _solutionFileList; }
      set {SetProperty(ref _solutionFileList, value); }
    }


    public ICommand StartBuildCommand => _startBuildCommand ??
      (_startBuildCommand = new DelegateCommand(OnStartBuild,
        ()=> SolutionFileList != null && SolutionFileList.Count > 0 && !String.IsNullOrWhiteSpace(SelectedSolution)
        && !String.IsNullOrWhiteSpace(SelectedConfiguration) && !String.IsNullOrWhiteSpace(SelectedPlatform)));

    private string _selectedSolution;

    public string SelectedSolution
    {
      get { return _selectedSolution; }
      set {
        SetProperty(ref _selectedSolution, value);
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
      set {SetProperty(ref _selectedConfiguration, value); }
    }

    private string _selectedPlatform;

    public string SelectedPlatform
    {
      get { return _selectedPlatform; }
      set { SetProperty(ref _selectedPlatform, value); }
    }



    private void OnStartBuild()
    {
      Duration = 0;
      Status = "Init";
      LogText = String.Empty;
      _sw = new Stopwatch();
      _sw.Start();
      Status = "Pending";
      _build.RunNonBlocking(
        SelectedSolution,
        SelectedPlatform,
        SelectedConfiguration,
        message =>
        {
          if (NO_SUPRESSION || message.Contains(" -- ") )
          {
            LogText += message + Environment.NewLine;
          }
          Duration = _sw.Elapsed.TotalSeconds;
        },
        () =>
        {
          _sw.Stop();
          Duration = _sw.Elapsed.TotalSeconds;
          Status = "Done";
        }
      );
    }
  }
}
