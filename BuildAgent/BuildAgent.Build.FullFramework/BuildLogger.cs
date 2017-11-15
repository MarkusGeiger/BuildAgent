using Microsoft.Build.Framework;
using System;

namespace BuildAgent.Build.FullFramework
{
  public class BuildLogger : IEventRedirector
  {
    public void ForwardEvent(BuildEventArgs buildEvent)
    {
      BuildEvent?.Invoke(buildEvent.Message);
    }

    public event Action<string> BuildEvent;
  }
}
