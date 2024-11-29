using System.Diagnostics;
using Cysharp.Text;

namespace Radish.Logging.Debugger;

internal sealed class DebugLogTarget : ILogTarget
{
    public void Write(LogLevel level, in ReadOnlySpan<char> text, Exception? ex)
    {
        using var builder = ZString.CreateStringBuilder();
        builder.Append(text);
        
        if (ex is not null)
        {
            builder.Append('\n');
            builder.Append(ex);
        }

        Debug.WriteLine(builder, level switch
        {
            LogLevel.Debug => "DEBUG",
            LogLevel.Info => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "ERROR",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        });
    }
}