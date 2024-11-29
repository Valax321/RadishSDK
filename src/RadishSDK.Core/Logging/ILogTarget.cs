using JetBrains.Annotations;

namespace Radish.Logging;

[PublicAPI]
public interface ILogTarget
{
    void Write(LogLevel level, in ReadOnlySpan<char> text, Exception? ex);
}