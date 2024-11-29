using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Radish.Platform;

/// <summary>
/// Interface to a platform API.
/// </summary>
[PublicAPI]
public interface IPlatformManager : IDisposable
{
    /// <summary>
    /// Tries to initialize the platform API.
    /// </summary>
    /// <param name="initError">An error message that must be set if returning false.</param>
    /// <returns>True if initialization was successful, otherwise false.</returns>
    bool TryInitialize([NotNullWhen(false)] out string? initError);
    
    /// <summary>
    /// Creates a new window.
    /// </summary>
    /// <remarks>
    /// It is recommended to use <see cref="TryCreateWindow"/> instead since you can better handle errors.
    /// </remarks>
    /// <exception cref="PlatformInitializeException">Thrown if the window could not be created.</exception>
    /// <param name="initParams">Parameters for initializing the window.</param>
    /// <returns>Window instance.</returns>
    IWindow CreateWindow(WindowInitParams initParams);

    /// <summary>
    /// Explicit error handling version of <see cref="CreateWindow"/>.
    /// </summary>
    /// <param name="initParams">Parameters for initializing the window.</param>
    /// <param name="window">Created window instance. Not null if this function returns true.</param>
    /// <param name="errorMessage">An error message describing what went wrong if this function returns false.</param>
    /// <returns>True if the window was created successfully, otherwise false.</returns>
    bool TryCreateWindow(WindowInitParams initParams, 
        [NotNullWhen(true)] out IWindow? window, 
        out string? errorMessage
        )
    {
        errorMessage = default;
        
        try
        {
            window = CreateWindow(initParams);
            return true;
        }
        catch (PlatformInitializeException ex)
        {
            window = default;
            errorMessage = ex.Error;
            return false;
        }
    }
    
    /// <summary>
    /// Polls for platform events.
    /// </summary>
    /// <param name="quitRequestedCallback">Callback to invoke when the user wants to quit.
    /// The parameter should be used to override the request if the app still needs to run.</param>
    /// <param name="waitForEvents">If true we can sleep the thread until an event occurs.</param>
    /// <returns>True if we should quit, otherwise false.</returns>
    bool PollEvents(QuitRequestDelegate? quitRequestedCallback, bool waitForEvents = false);

    /// <summary>
    /// Shows a system message box with the specified contents.
    /// </summary>
    /// <param name="type">Type of message box to show.</param>
    /// <param name="title">Title of the message box.</param>
    /// <param name="message">String contents of the message box.</param>
    /// <param name="parent">Parent window for the message box.</param>
    void ShowMessageBox(MessageBoxType type, string title, string message, IWindow? parent);

    /// <summary>
    /// Shows a system message box with custom buttons.
    /// </summary>
    /// <param name="type">Type of message box to show.</param>
    /// <param name="title">Title of the message box.</param>
    /// <param name="message">String contents of the message box.</param>
    /// <param name="parent">Parent window for the message box.</param>
    /// <param name="buttons">Button definitions for the message box.</param>
    int ShowMessageBox(MessageBoxType type, string title, string message, IWindow? parent,
        IReadOnlyList<MessageBoxButton> buttons);
}