using JetBrains.Annotations;

namespace Radish.Logging;

/// <summary>
/// Debug logger interface.
/// </summary>
[PublicAPI]
public interface ILogger
{
    /// <summary>
    /// Logs a message to all registered log targets.
    /// </summary>
    /// <param name="level">The log level to output to.</param>
    /// <param name="format">Format string for the log message.</param>
    /// <param name="args">List of parameters to insert into the format string.</param>
    [StringFormatMethod(nameof(format))]
    void Log(LogLevel level, string format, params ReadOnlySpan<object?> args);
}