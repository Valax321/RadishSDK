using JetBrains.Annotations;
using Radish.Logging.Debugger;

namespace Radish.Logging;

/// <summary>
/// Global manager for debug logging.
/// </summary>
[PublicAPI]
public static class LogManager
{
    internal static IReadOnlyList<ILogTarget> Targets => _targets;
    
    // ReSharper disable once InconsistentNaming
    private static readonly List<ILogTarget> _targets = 
    [
        new DebugLogTarget(),
        new StdoutTarget()
    ];

    /// <summary>
    /// Adds a new <see cref="ILogTarget"/> for outputting debug messages.
    /// </summary>
    /// <param name="target"></param>
    public static void AddLogTarget(ILogTarget target)
    {
        _targets.Add(target);
    }

    /// <summary>
    /// Gets a new logger with the given name.
    /// </summary>
    /// <param name="name">The name of the logger.</param>
    /// <returns>A new <see cref="ILogger"/> instance.</returns>
    public static ILogger GetLogger(string name) => new DefaultFormatLogger(name);
    
    /// <summary>
    /// Gets a new logger for the given type.
    /// </summary>
    /// <param name="type">The type of the logger.</param>
    /// <returns>A new <see cref="ILogger"/> instance.</returns>
    public static ILogger GetLogger(Type type) => GetLogger(type.FullName ?? type.Name);
    
    /// <summary>
    /// Gets a new logger for thr given type.
    /// </summary>
    /// <typeparam name="T">The type of the logger.</typeparam>
    /// <returns>A new <see cref="ILogger"/> instance.</returns>
    public static ILogger GetLogger<T>() => GetLogger(typeof(T));
}