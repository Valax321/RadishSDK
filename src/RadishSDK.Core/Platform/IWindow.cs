using System.Drawing;
using JetBrains.Annotations;

namespace Radish.Platform;

/// <summary>
/// Wrapper for a platform native window.
/// Not all functions are guaranteed to be supported on all platforms.
/// <remarks>
/// All functions and parameters are liable to throw a <see cref="ObjectDisposedException"/> if the
/// window was not created properly or if it was already disposed. Use <see cref="IsValid"/> to ensure no exceptions
/// will occur if you want to make sure.
/// </remarks>
/// </summary>
[PublicAPI]
public interface IWindow : IDisposable
{
    /// <summary>
    /// The title of this window.
    /// </summary>
    string Title { get; set; }
    
    /// <summary>
    /// The size of the window, in display size-independent coordinates.
    /// <remarks>
    /// If you want the size in pixels, use <see cref="PixelSize"/>.
    /// </remarks>
    /// <seealso cref="PixelSize"/>
    /// <seealso cref="MinimumSize"/>
    /// <seealso cref="MaximumSize"/>
    /// </summary>
    Size Size { get; set; }
    
    /// <summary>
    /// The size of the window, in pixels.
    /// <seealso cref="Size"/>
    /// </summary>
    Size PixelSize { get; }
    
    /// <summary>
    /// The minimum allowed size of the window.
    /// </summary>
    Size MinimumSize { get; set; }
    
    /// <summary>
    /// The maximum allowed size of the window.
    /// </summary>
    Size MaximumSize { get; set; }
    
    /// <summary>
    /// The desktop position of the window.
    /// </summary>
    Point Position { get; set; }
    
    /// <summary>
    /// Is the window currently in fullscreen mode?
    /// </summary>
    bool IsFullscreen { get; set; }
    
    /// <summary>
    /// Is the window currently hidden?
    /// <remarks>
    /// All windows start hidden and must be manually shown before use.
    /// </remarks>
    /// <seealso cref="Show"/>
    /// <seealso cref="Hide"/>
    /// </summary>
    bool IsHidden { get; }
    
    /// <summary>
    /// Can this window be resized?
    /// </summary>
    bool Resizable { get; set; }
    
    /// <summary>
    /// Can the window be toggled in/out of fullscreen with alt+enter?
    /// </summary>
    bool AllowToggleFullscreenShortcut { get; set; }
    
    /// <summary>
    /// Shows this window.
    /// <seealso cref="IsHidden"/>
    /// </summary>
    void Show();
    
    /// <summary>
    /// Hides this window.
    /// <seealso cref="IsHidden"/>
    /// </summary>
    void Hide();
    
    /// <summary>
    /// Checks whether this window has been created successfully and has not been disposed.
    /// </summary>
    bool IsValid { get; }
    
    /// <summary>
    /// Callback for when the window is being disposed.
    /// Only used internally.
    /// </summary>
    internal event Action<IWindow> OnDisposed
    {
        add => DisposedCallback += value;
        remove
        {
            if (DisposedCallback is not null)
                DisposedCallback -= value;
        }
    }
    
    /// <summary>
    /// Only used internally for <see cref="Application"/> logic.
    /// This just needs to exist, no fancy behaviour needed.
    /// </summary>
    protected Action<IWindow>? DisposedCallback { get; set; }
}