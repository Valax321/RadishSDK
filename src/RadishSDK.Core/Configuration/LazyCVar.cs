using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Radish.Configuration;

/// <summary>
/// Lazy wrapper for a <see cref="CVar{T}"/> that only finds the variable when accessed.
/// </summary>
/// <typeparam name="T">The type of variable.</typeparam>
[PublicAPI]
public sealed class LazyCVar<T>
    where T : IEquatable<T>, IParsable<T>
{
    /// <summary>
    /// Construct a new <see cref="LazyCVar{T}"/> instance.
    /// </summary>
    /// <param name="name">The CVar to reference.</param>
    public LazyCVar(string name)
    {
        Name = name;
    }
    
    /// <inheritdoc cref="ICVar.Name"/>
    public string Name { get; }
    
    /// <summary>
    /// Whether this variable's concrete value has been resolved.
    /// </summary>
    [MemberNotNullWhen(true, nameof(LazyValue))]
    public bool Resolved { get; private set; }
    
    /// <summary>
    /// The backing <see cref="CVar{T}"/>.
    /// </summary>
    public CVar<T>? LazyValue
    {
        get
        {
            if (_resolved is null)
            {
                _resolved = ConfigurationManager.FindCVar<T>(Name);
                Resolved = _resolved is not null;
            }

            return _resolved;
        }
    }
    
    private CVar<T>? _resolved;
    
    /// <summary>
    /// Converts a <see cref="LazyCVar{T}"/> to its concrete implementation.
    /// </summary>
    /// <param name="lCVar">Value to convert.</param>
    /// <returns>Converted variable.</returns>
    public static implicit operator CVar<T>?(LazyCVar<T> lCVar)
    {
        return lCVar.LazyValue;
    }
}