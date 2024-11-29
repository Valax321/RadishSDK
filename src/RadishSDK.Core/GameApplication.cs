using System.Drawing;
using JetBrains.Annotations;
using Radish.Platform;

namespace Radish;

/// <summary>
/// GameApplication is a specialisation of <see cref="Application"/> designed for single-window game applications.
/// </summary>
[PublicAPI]
public abstract class GameApplication : Application
{
    /// <summary>
    /// Global instance of the game.
    /// </summary>
    public new static GameApplication? Instance => Application.Instance as GameApplication;
    
    /// <summary>
    /// The main (and hopefully only) window for the game.
    /// </summary>
    public IWindow MainWindow { get; }

    /// <summary>
    /// Constructs a new <see cref="GameApplication"/> instance.
    /// </summary>
    /// <param name="platformManager">The <see cref="IPlatformManager"/>to use.</param>
    protected GameApplication(IPlatformManager platformManager) : base(new ApplicationInitParams(platformManager))
    {
        MainWindow = CreateWindow(new WindowInitParams(
            null,
            new Size(1280, 720), 
            new Size(640, 480))
        );

        MainWindow.AllowToggleFullscreenShortcut = true;
    }
}