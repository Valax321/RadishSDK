using JetBrains.Annotations;

namespace Radish.Configuration;

/// <summary>
/// Runtime configurable global variable.
/// </summary>
/// <typeparam name="T">The type of the CVar.</typeparam>
[PublicAPI]
public sealed class CVar<T> : ICVar 
    where T : IEquatable<T>, IParsable<T>
{
    /// <summary>
    /// Delegate for the change in value of a CVar.
    /// </summary>
    public delegate void ChangedDelegate(T oldValue, T newValue);
    
    /// <inheritdoc cref="ICVar.Name"/>
    public string Name { get; }
    
    /// <summary>
    /// The current value of this variable.
    /// </summary>
    public T Value
    {
        get => _value;
        set
        {
            if (Flags.HasFlag(CVarFlags.Constant))
                return;
            
            if (_value.Equals(value)) 
                return;
            
            NotifyChanged(_value, value);
            _value = value;
        }
    }

    /// <summary>
    /// The default value of this variable assigned at creation.
    /// </summary>
    public T DefaultValue { get; }
    
    /// <summary>
    /// Callback invoked when the <see cref="Value"/> of this variable is changed.
    /// </summary>
    public event ChangedDelegate OnChanged
    {
        add => _changedCallback += value;
        remove
        {
            if (_changedCallback != null)
                _changedCallback -= value;
        }
    }
    
    /// <inheritdoc cref="ICVar.HelpText"/>
    public string HelpText { get; }

    /// <inheritdoc cref="ICVar.Flags"/>
    public CVarFlags Flags { get; internal set; }
    
    private T _value;
    private ChangedDelegate? _changedCallback;

    internal CVar(string name, T defaultValue, string? helpText, CVarFlags flags)
    {
        Name = name;
        DefaultValue = defaultValue;
        _value = defaultValue;
        HelpText = helpText ?? string.Empty;
        Flags = flags;
    }

    private void NotifyChanged(T oldValue, T newValue)
    {
        _changedCallback?.Invoke(oldValue, newValue);
    }
    
    /// <inheritdoc cref="ICVar.GetValueString"/>
    public string GetValueString()
    {
        return Value.ToString() ?? "null";
    }

    /// <inheritdoc cref="ICVar.TryParseAndSet"/>
    public bool TryParseAndSet(string value)
    {
        if (Flags.HasFlag(CVarFlags.ReadOnly))
            return false;
        
        //TODO: check for cheats (where?)
        
        if (T.TryParse(value, null, out var v))
        {
            Value = v;
            return true;
        }

        return false;
    }
}