using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using JetBrains.Annotations;
using SDL3;

namespace Radish.Platform.SDL3;

/// <summary>
/// <see cref="IPlatformManager"/> for the SDL3 backend.
/// Implements windowing and graphics via SDL_gpu.
/// </summary>
[PublicAPI]
public sealed class SDL3PlatformManager : IPlatformManager
{
    /// <summary>
    /// Delegate for responding to SDL events.
    /// </summary>
    public delegate void EventDelegate(in SDL.SDL_Event @event);
    
    /// <summary>
    /// Callback invoked when an SDL event is being processed.
    /// </summary>
    public event EventDelegate OnProcessEvent
    {
        add => _eventCallback += value;
        remove
        {
            if (_eventCallback is not null)
                _eventCallback -= value;
        }
    }
    
    private bool _initialized;
    private EventDelegate? _eventCallback;

    static SDL3PlatformManager()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), ResolveSDL3Assembly);
    }

    private static IntPtr ResolveSDL3Assembly(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        // It would be nice if SDL3-CS let us access the name const directly...
        if (libraryName == "SDL3")
        {
            if (OperatingSystem.IsMacOS())
            {
                //TODO: if in app bundle, go up to Frameworks and load it there!
                var loadDir = AppDomain.CurrentDomain.BaseDirectory;
                return NativeLibrary.Load(Path.Combine(loadDir, "SDL3.framework", "Versions", "Current", "SDL3"));
            }
        }
        
        return IntPtr.Zero;
    }

    /// <inheritdoc cref="IPlatformManager.TryInitialize"/>
    public bool TryInitialize([NotNullWhen(false)] out string? initError)
    {
        if (_initialized)
        {
            initError = "Already initialized";
            return false;
        }
        
        const SDL.SDL_InitFlags flags = SDL.SDL_InitFlags.SDL_INIT_TIMER | SDL.SDL_InitFlags.SDL_INIT_VIDEO |
                                        SDL.SDL_InitFlags.SDL_INIT_EVENTS | SDL.SDL_InitFlags.SDL_INIT_HAPTIC |
                                        SDL.SDL_InitFlags.SDL_INIT_SENSOR | SDL.SDL_InitFlags.SDL_INIT_GAMEPAD |
                                        SDL.SDL_InitFlags.SDL_INIT_JOYSTICK;

        var status = SDL.SDL_Init(flags);
        if (!status)
        {
            initError = SDL.SDL_GetError();
            return false;
        }

        // Change where we load data from (might be ../Resources on macOS for example)
        EngineCVars.BaseDirectory.Value = SDL.SDL_GetBasePath();

        // Change where we save data from
        var rootAssembly = Assembly.GetEntryAssembly();
        if (rootAssembly != null)
        {
            var company = rootAssembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            Debug.Assert(company is not null);
            
            var title = rootAssembly.GetCustomAttribute<AssemblyTitleAttribute>();
            Debug.Assert(title is not null);
            
            EngineCVars.PersistentDataDirectory.Value = SDL.SDL_GetPrefPath(company.Company, title.Title);
        }

        _initialized = true;
        initError = default;
        return true;
    }

    /// <inheritdoc cref="IPlatformManager.CreateWindow"/>
    public IWindow CreateWindow(WindowInitParams initParams)
    {
        return new SDL3Window(initParams, this);
    }

    /// <inheritdoc cref="IPlatformManager.PollEvents"/>
    public bool PollEvents(QuitRequestDelegate? quitCallback, bool waitForEvents)
    {
        var keepRunning = true;
        while (PollOrWaitForEvents(waitForEvents, out var ev))
        {
            if (ev.type == (uint)SDL.SDL_EventType.SDL_EVENT_QUIT)
            {
                var quit = true;
                quitCallback?.Invoke(ref quit);
                keepRunning = !quit;
            }
        }

        return keepRunning;
    }

    /// <inheritdoc cref="IPlatformManager.ShowMessageBox(Radish.Platform.MessageBoxType,string,string,Radish.Platform.IWindow?)"/>
    public void ShowMessageBox(MessageBoxType type, string title, string message, IWindow? parent)
    {
        var wnd = parent as SDL3Window;
        SDL.SDL_ShowSimpleMessageBox(type switch
        {
            MessageBoxType.Info => SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_INFORMATION,
            MessageBoxType.Warning => SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_WARNING,
            MessageBoxType.Error => SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        }, title, message, wnd?.WindowHandle ?? IntPtr.Zero);
    }

    /// <inheritdoc cref="IPlatformManager.ShowMessageBox(Radish.Platform.MessageBoxType,string,string,Radish.Platform.IWindow?,System.Collections.Generic.IReadOnlyList{Radish.Platform.MessageBoxButton})"/>
    public unsafe int ShowMessageBox(MessageBoxType type, string title, string message, IWindow? parent, IReadOnlyList<MessageBoxButton> buttons)
    {
        // Note: this being weird might be SDL3-CS not being finished yet,
        // the api will hopefully expose less native pointers like SDL2-CS does.
        
        var wnd = parent as SDL3Window;

        var titlePtr = Utf8StringMarshaller.ConvertToUnmanaged(title);
        var messagePtr = Utf8StringMarshaller.ConvertToUnmanaged(message);
        var buttonLabels = new byte*[buttons.Count];
        var buttonData = stackalloc SDL.SDL_MessageBoxButtonData[buttons.Count];
        
        for (var i = 0; i < buttons.Count; ++i)
        {
            var b = buttons[i];
            buttonLabels[i] = Utf8StringMarshaller.ConvertToUnmanaged(b.Text);
            buttonData[i] = new SDL.SDL_MessageBoxButtonData()
            {
                flags = b.IsDefault ? SDL.SDL_MessageBoxButtonFlags.SDL_MESSAGEBOX_BUTTON_RETURNKEY_DEFAULT : 0,
                buttonID = i,
                text = buttonLabels[i]
            };
        }
        
        var data = new SDL.SDL_MessageBoxData
        {
            flags = type switch
            {
                MessageBoxType.Info => SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_INFORMATION,
                MessageBoxType.Warning => SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_WARNING,
                MessageBoxType.Error => SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            },
            window = wnd?.WindowHandle ?? IntPtr.Zero,
            message = messagePtr,
            title = titlePtr,
            numbuttons = buttons.Count,
            buttons = buttonData
        };

        SDL.SDL_ShowMessageBox(ref data, out var button);
        
        Utf8StringMarshaller.Free(titlePtr);
        Utf8StringMarshaller.Free(messagePtr);
        foreach (var s in buttonLabels)
            Utf8StringMarshaller.Free(s);
        
        return button;
    }

    private static bool PollOrWaitForEvents(bool wait, out SDL.SDL_Event ev)
    {
        if (wait)
        {
            return SDL.SDL_WaitEvent(out ev);
        }
        else
        {
            return SDL.SDL_PollEvent(out ev);
        }
    }

    /// <inheritdoc cref="IPlatformManager.Dispose"/>
    public void Dispose()
    {
        if (_initialized)
            SDL.SDL_Quit();

        _initialized = false;
    }
}