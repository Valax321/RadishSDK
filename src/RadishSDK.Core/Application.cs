using System.Diagnostics;
using JetBrains.Annotations;
using Radish.Logging;
using Radish.Platform;

namespace Radish;

/// <summary>
/// Main application class.
/// Manages the run loop for the program.
/// </summary>
[PublicAPI]
public abstract class Application : IDisposable
{
    private static readonly ILogger Logger = LogManager.GetLogger<Application>();
    
    /// <summary>
    /// Global application instance.
    /// </summary>
    public static Application? Instance { get; private set; }
    
    /// <summary>
    /// Platform manager that handles all backend specifics for this application.
    /// </summary>
    public IPlatformManager PlatformManager { get; }
    
    /// <summary>
    /// List of all windows that currently exist in the app.
    /// </summary>
    public IReadOnlyList<IWindow> Windows => _windows;
    
    /// <summary>
    /// Is the game currently running?
    /// </summary>
    /// <remarks>
    /// If this is false during an update loop, the game is imminently about to close
    /// and anything that needs to be saved should be.
    /// </remarks>
    public bool IsRunning { get; private set; }
    
    /// <summary>
    /// Event raised when the user requests to close the app.
    /// You can cancel the quit request if it is not advisable to close right now.
    /// </summary>
    public event QuitRequestDelegate OnQuitRequested
    {
        add => _quitCallback += value;
        remove
        {
            if (_quitCallback != null)
                _quitCallback -= value;
        }
    }
    
    private List<IWindow> _windows = [];
    private QuitRequestDelegate? _quitCallback;
    private bool _waitForEvents;

    /// <summary>
    /// Creates a new application.
    /// </summary>
    /// <param name="initParams">Parameters for initializing the application.</param>
    public Application(ApplicationInitParams initParams)
    {
        if (Instance is not null)
        {
            throw new InvalidOperationException("Only one Application can exist at a time");
        }
        
        PlatformManager = initParams.PlatformManager;
        _waitForEvents = initParams.WaitForEvents;
        
        PlatformInitializeException.ThrowIfFailed(PlatformManager.TryInitialize(out var err), err);
        
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
    }

    [DebuggerDisableUserUnhandledExceptions]
    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (Debugger.IsAttached)
        {
            Debugger.BreakForUserUnhandledException((Exception)e.ExceptionObject);
            return;
        }
        
        var result = ShowMessageBox(
            MessageBoxType.Error, 
            null, 
            "Unhandled Exception", 
            e.ExceptionObject.ToString() ?? "null",
    [
                new MessageBoxButton("Abort", true),
                new MessageBoxButton("Debug")
            ]);

        Logger.Log(LogLevel.Debug, "Message box returned: {0}", result);
        
        switch (result)
        {
            case 0:
                Environment.Exit(1);
                break;
            case 1:
            {
                var attached =  Debugger.Launch();
                if (attached)
                    Debugger.BreakForUserUnhandledException((Exception)e.ExceptionObject);
                else
                    Environment.Exit(1);
            }
            break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, null);
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        
        foreach (var wnd in _windows)
        {
            wnd.Dispose();
        }
        
        _windows.Clear();
        
        PlatformManager.Dispose();

        AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
    }
    
    /// <summary>
    /// Creates a new window.
    /// </summary>
    /// <remarks>
    /// The window will be hidden by default. Use <see cref="IWindow.Show"/> to show it.
    /// </remarks>
    /// <exception cref="PlatformInitializeException">Thrown if the window could not be created.</exception>
    /// <param name="initParams">Parameters used to initialize the window.</param>
    /// <returns>Window interface instance.</returns>
    public IWindow CreateWindow(WindowInitParams initParams)
    {
        var wnd = PlatformManager.CreateWindow(initParams);
        wnd.OnDisposed += w => _windows.Remove(w);
        _windows.Add(wnd);
        return wnd;
    }

    /// <summary>
    /// Called when the application should initialize its components.
    /// All native systems are initialized at this point.
    /// </summary>
    protected virtual void Initialize()
    {
    }

    /// <summary>
    /// Called when the application is about to close.
    /// Nothing has been disposed yet.
    /// </summary>
    protected virtual void Quit()
    {
    }

    /// <summary>
    /// Called constantly, or whenever an event is received if <see cref="ApplicationInitParams.WaitForEvents"/> was set to true.
    /// </summary>
    protected virtual void Update()
    {
    }

    /// <summary>
    /// Runs the main loop. Will not return until the app
    /// is done running.
    /// </summary>
    public void Run()
    {
        IsRunning = true;
        Initialize();

        while (IsRunning)
        {
            IsRunning &= PlatformManager.PollEvents(_quitCallback, _waitForEvents);
            Update();
        }
        
        Quit();
    }

    /// <summary>
    /// Shows a message box attached to the specified window.
    /// </summary>
    /// <param name="type">The type of message box to show.</param>
    /// <param name="parent">The parent window of the dialog. Optional.</param>
    /// <param name="title">The title of the message box.</param>
    /// <param name="message">The contents of the message box.</param>
    public void ShowMessageBox(MessageBoxType type, IWindow? parent, string title, string message)
    {
        PlatformManager.ShowMessageBox(type, title, message, parent);
    }

    /// <summary>
    /// Shows a message box with customised buttons.
    /// </summary>
    /// <inheritdoc cref="ShowMessageBox(Radish.Platform.MessageBoxType,Radish.Platform.IWindow?,string,string)"/>
    /// <seealso cref="ShowMessageBox(Radish.Platform.MessageBoxType,Radish.Platform.IWindow?,string,string)"/>
    /// <param name="buttons">The buttons to add to the message box.</param>
    /// <returns>The index of the button selected by the user.</returns>
    public int ShowMessageBox(MessageBoxType type, IWindow? parent, string title, string message, IReadOnlyList<MessageBoxButton> buttons)
    {
        Debug.Assert(buttons.Count > 0);
        return PlatformManager.ShowMessageBox(type, title, message, parent, buttons);
    }
}
