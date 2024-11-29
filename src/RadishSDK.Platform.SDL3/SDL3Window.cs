using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using Radish.Logging;
using SDL3;

namespace Radish.Platform.SDL3;

internal sealed class SDL3Window : IWindow
{
    private static readonly ILogger Logger = LogManager.GetLogger<SDL3Window>();
    
    internal IntPtr WindowHandle { get; private set; }
    
    public bool IsValid => WindowHandle != IntPtr.Zero;

    private SDL3PlatformManager _owner;

    public uint WindowID
    {
        get
        {
            EnsureValid(WindowHandle);
            return SDL.SDL_GetWindowID(WindowHandle);
        }
    }

    public string Title
    {
        get
        {
            EnsureValid(WindowHandle);
            return SDL.SDL_GetWindowTitle(WindowHandle);
        }
        set
        {
            EnsureValid(WindowHandle);
            LogSDLErrorIfFailed(SDL.SDL_SetWindowTitle(WindowHandle, value));
        }
    }
    
    public Point Position
    {
        get
        {
            EnsureValid(WindowHandle);
            SDL.SDL_GetWindowPosition(WindowHandle, out var x, out var y);
            return new Point(x, y);
        }
        set
        {
            EnsureValid(WindowHandle);
            LogSDLErrorIfFailed(SDL.SDL_SetWindowPosition(WindowHandle, value.X, value.Y));
        }
    }

    public bool IsFullscreen
    {
        get
        {
            EnsureValid(WindowHandle);
            return (SDL.SDL_GetWindowFlags(WindowHandle) & SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0;
        }
        set
        {
            EnsureValid(WindowHandle);
            LogSDLErrorIfFailed(SDL.SDL_SetWindowFullscreen(WindowHandle, value));
        }
    }

    public bool IsHidden
    {
        get
        {
            EnsureValid(WindowHandle);
            return (SDL.SDL_GetWindowFlags(WindowHandle) & SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN) != 0;
        }
    }

    public bool Resizable
    {
        get
        {
            EnsureValid(WindowHandle);
            return (SDL.SDL_GetWindowFlags(WindowHandle) & SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE) != 0;
        }
        set
        {
            EnsureValid(WindowHandle);
            LogSDLErrorIfFailed(SDL.SDL_SetWindowResizable(WindowHandle, value));
        }
    }

    public bool AllowToggleFullscreenShortcut { get; set; }

    public Action<IWindow>? DisposedCallback { get; set; }

    public Size Size
    {
        get
        {
            EnsureValid(WindowHandle);
            SDL.SDL_GetWindowSize(WindowHandle, out var w, out var h);
            return new Size(w, h);
        }
        set
        {
            EnsureValid(WindowHandle);
            LogSDLErrorIfFailed(SDL.SDL_SetWindowSize(WindowHandle, value.Width, value.Height));
        }
    }
    
    public Size PixelSize
    {
        get
        {
            EnsureValid(WindowHandle);
            SDL.SDL_GetWindowSizeInPixels(WindowHandle, out var w, out var h);
            return new Size(w, h);
        }
    }

    public Size MinimumSize
    {
        get
        {
            EnsureValid(WindowHandle);
            SDL.SDL_GetWindowMinimumSize(WindowHandle, out var w, out var h);
            return new Size(w, h);
        }
        set
        {
            EnsureValid(WindowHandle);
            LogSDLErrorIfFailed(SDL.SDL_SetWindowMinimumSize(WindowHandle, value.Width, value.Height));
        }
    }

    public Size MaximumSize
    {
        get
        {
            EnsureValid(WindowHandle);
            SDL.SDL_GetWindowMaximumSize(WindowHandle, out var w, out var h);
            return new Size(w, h);
        }
        set
        {
            EnsureValid(WindowHandle);
            LogSDLErrorIfFailed(SDL.SDL_SetWindowMaximumSize(WindowHandle, value.Width, value.Height));
        }
    }

    public SDL3Window(WindowInitParams initParams, SDL3PlatformManager owner)
    {
        _owner = owner;
        
        var flags = SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN | SDL.SDL_WindowFlags.SDL_WINDOW_HIGH_PIXEL_DENSITY;
        if (initParams.StartFullscreen)
        {
            flags |= SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
        }

        WindowHandle =
            SDL.SDL_CreateWindow(initParams.Title ?? Assembly.GetEntryAssembly()?.GetName().Name ?? "Radish SDK Game",
                initParams.Size.Width, initParams.Size.Height, flags);
        PlatformInitializeException.ThrowIfFailed(WindowHandle != IntPtr.Zero, SDL.SDL_GetError());

        if (initParams.MinimumSize.HasValue)
        {
            MinimumSize = initParams.MinimumSize.Value;
        }

        if (initParams.MaximumSize.HasValue)
        {
            MaximumSize = initParams.MaximumSize.Value;
        }
        
        _owner.OnProcessEvent += OnSDLEvent;
    }

    private void OnSDLEvent(in SDL.SDL_Event @event)
    {
        switch ((SDL.SDL_EventType)@event.type)
        {
            case SDL.SDL_EventType.SDL_EVENT_KEY_DOWN:
            {
                if (AllowToggleFullscreenShortcut
                    && @event.key.windowID == WindowID 
                    && (@event.key.mod & SDL.SDL_Keymod.SDL_KMOD_ALT) != 0 
                    && @event.key.key == (uint)SDL.SDL_Keycode.SDLK_RETURN)
                {
                    IsFullscreen = !IsFullscreen;
                }
            }
            break;
        }
    }

    public void Dispose()
    {
        _owner.OnProcessEvent -= OnSDLEvent;
        
        if (WindowHandle != IntPtr.Zero)
        {
            SDL.SDL_DestroyWindow(WindowHandle);
            WindowHandle = IntPtr.Zero;
        }
    }
    
    public void Show()
    {
        EnsureValid(WindowHandle);
        LogSDLErrorIfFailed( SDL.SDL_ShowWindow(WindowHandle));
    }

    public void Hide()
    {
        EnsureValid(WindowHandle);
        LogSDLErrorIfFailed(SDL.SDL_HideWindow(WindowHandle));
    }

    [StackTraceHidden]
    private static void EnsureValid(IntPtr handle, [CallerArgumentExpression("handle")] string? message = null)
    {
        if (handle == IntPtr.Zero)
            throw new ObjectDisposedException(message, "Native resource pointer was null");
    }

    private static void LogSDLErrorIfFailed(bool condition, [CallerArgumentExpression("condition")] string? message = null)
    {
        if (!condition)
            Logger.Log(LogLevel.Warning, "{0} failed: {1}", message, SDL.SDL_GetError());
    }
}