using Radish.Logging;
using Radish.Platform.SDL3;

namespace Radish;

public sealed class ExampleApplication() : GameApplication(new SDL3PlatformManager())
{
    public new static ExampleApplication? Instance => GameApplication.Instance as ExampleApplication;
    
    private static readonly ILogger Logger = LogManager.GetLogger<ExampleApplication>();
    
    protected override void Initialize()
    {
        MainWindow.Show();
        MainWindow.Title = "My Example Game";
        MainWindow.Resizable = true;
        
        Logger.Log(LogLevel.Info, "Hello!");
    }

    protected override void Quit()
    {
        MainWindow.Hide();
    }
}