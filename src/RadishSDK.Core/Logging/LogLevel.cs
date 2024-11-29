namespace Radish.Logging;

/// <summary>
/// Logging levels for <see cref="ILogger"/>.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// A debug message. Not output in Release builds.
    /// </summary>
    Debug,
    
    /// <summary>
    /// An informational message.
    /// </summary>
    Info,
    
    /// <summary>
    /// A warning.
    /// </summary>
    Warning,
    
    /// <summary>
    /// An error.
    /// </summary>
    Error,
}