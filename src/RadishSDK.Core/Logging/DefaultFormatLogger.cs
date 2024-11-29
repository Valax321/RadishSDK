using Cysharp.Text;

namespace Radish.Logging;

internal sealed class DefaultFormatLogger(string name) : ILogger
{
    public void Log(LogLevel level, string format, params ReadOnlySpan<object?> args)
    {
        using var builder = ZString.CreateStringBuilder();
        builder.AppendFormat("[{0}] ", name);
        
        if (args.Length > 0)
            builder.Append(string.Format(format, args));
        else
            builder.Append(format);
        
        foreach (var target in LogManager.Targets)
            target.Write(level, builder.AsSpan(), null);
    }
}