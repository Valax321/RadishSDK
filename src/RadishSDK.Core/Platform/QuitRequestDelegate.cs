namespace Radish.Platform;

/// <summary>
/// Delegate for cancelling a user quit request.
/// <param name="shouldQuit">Set this to false in order to cancel the quit.</param>
/// </summary>
public delegate void QuitRequestDelegate(ref bool shouldQuit);
