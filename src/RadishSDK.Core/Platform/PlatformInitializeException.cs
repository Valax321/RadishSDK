using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Radish.Platform;

/// <summary>
/// Exception thrown when an element of the native platform interface fails to be initialized.
/// </summary>
[PublicAPI]
public class PlatformInitializeException : Exception
{
    /// <summary>
    /// An error string explaining what happened.
    /// </summary>
    public string? Error { get; }
    
    /// <summary>
    /// If <see cref="condition"/> is false, throw a <see cref="PlatformInitializeException"/> explaining why.
    /// </summary>
    /// <param name="condition">The condition to test.</param>
    /// <param name="error">Message explaining the error.</param>
    /// <param name="message">Generated string representation of <see cref="condition"/>.</param>
    /// <exception cref="PlatformInitializeException">Exception thrown if <see cref="condition"/> is null.</exception>
    [StackTraceHidden]
    public static void ThrowIfFailed(bool condition, string? error = null, [CallerArgumentExpression("condition")] string? message = null)
    {
        if (!condition)
            throw new PlatformInitializeException(message, error);
    }

    internal PlatformInitializeException(string? conditionString, string? error) : base($"{error} ({conditionString})")
    {
        Error = error;
    }
}