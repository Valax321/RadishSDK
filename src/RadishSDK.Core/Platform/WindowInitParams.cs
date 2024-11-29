using System.Drawing;

namespace Radish.Platform;

/// <summary>
/// Parameters for initializing a <see cref="IWindow"/>.
/// </summary>
/// <param name="Title">The initial title of the window. <seealso cref="IWindow.Title"/></param>
/// <param name="Size">The initial size of the window. <seealso cref="IWindow.Size"/></param>
/// <param name="MinimumSize">The initial minimum size of the window. <seealso cref="IWindow.MinimumSize"/></param>
/// <param name="MaximumSize">The initial maximum size of the window. <seealso cref="IWindow.MaximumSize"/></param>
/// <param name="StartFullscreen">Should the app start in fullscreen?</param>
public record struct WindowInitParams(
    string? Title, 
    Size Size, 
    Size? MinimumSize = null, 
    Size? MaximumSize = null, 
    bool StartFullscreen = false
);
