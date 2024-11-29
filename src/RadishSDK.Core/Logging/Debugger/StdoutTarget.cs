using Cysharp.Text;

namespace Radish.Logging.Debugger;

internal sealed class StdoutTarget : ILogTarget
{
    public void Write(LogLevel level, in ReadOnlySpan<char> text, Exception? ex)
    {
        using var builder = ZString.CreateStringBuilder();
        
        #if !DEBUG
        if (level == LogLevel.Debug)
            return;
        #endif
        
        builder.AppendFormat("[{0:u}] ", DateTime.Now);
        
        builder.AppendFormat("[{0}] ", level switch
        {
            LogLevel.Debug => "DEBUG",
            LogLevel.Info => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "ERROR",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        });
        builder.Append(text);
        
        if (ex is not null)
        {
            builder.Append('\n');
            builder.Append(ex);
        }

        if (level is LogLevel.Info or LogLevel.Debug)
        {
            Console.Out.WriteLine(builder);
        }
        else
        {
            Console.Error.WriteLine(builder);
        }
    }
}