using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Radish.Configuration;

/// <summary>
/// Class that manages cvars and commands.
/// </summary>
/// <remarks>
/// <para>Note: CVar names are not case-sensitive.</para>
/// <para>Note: because of how .NET loads assemblies as needed, some CVars may not
/// yet exist early during program execution if they are created in static constructors.</para>
/// </remarks>
[PublicAPI]
public static class ConfigurationManager
{
    /// <summary>
    /// Collection of all <see cref="ICVar"/>s that have been registered.
    /// </summary>
    public static IReadOnlyCollection<ICVar> CVars => _cvars.Values;
    
    private static Dictionary<string, ICVar> _cvars = new(Array.Empty<KeyValuePair<string, ICVar>>(), 
        StringComparer.InvariantCultureIgnoreCase);
    
    /// <summary>
    /// Creates a CVar with the given name.
    /// </summary>
    /// <seealso cref="FindCVar{T}"/>
    /// <seealso cref="TryFindCVar{T}"/>
    /// <param name="name">The name of the cvar.</param>
    /// <param name="defaultValue">The default value of the cvar.</param>
    /// <param name="helpText">Display string for cvar help text.</param>
    /// <param name="flags">Flags that inform behaviour of this cvar.</param>
    /// <typeparam name="T">The cvar value type.</typeparam>
    /// <returns>The CVar instance.</returns>
    /// <exception cref="DuplicateCVarException">Thrown if more than one CVar with the same <see cref="CVar{T}.Name"/> is registered.</exception>
    public static CVar<T> CreateCVar<T>(string name, T defaultValue, string? helpText = null,
        CVarFlags flags = CVarFlags.None) where T : IEquatable<T>, IParsable<T>
    {
        if (_cvars.ContainsKey(name))
        {
            throw new DuplicateCVarException(name);
        }
        
        var cvar = new CVar<T>(name, defaultValue, helpText, flags);
        return cvar;
    }

    /// <summary>
    /// Finds a CVar with the given name. The value types of the CVars must also match to get a valid result.
    /// </summary>
    /// <seealso cref="TryFindCVar{T}"/>
    /// <param name="name">The name of the CVar.</param>
    /// <typeparam name="T">The value type of the CVar.</typeparam>
    /// <returns>The CVar if found, otherwise null.</returns>
    public static CVar<T>? FindCVar<T>(string name) where T : IEquatable<T>, IParsable<T>
    {
        if (_cvars.TryGetValue(name, out var cvar))
        {
            // TODO: log if we find it but the type does not match?
            if (cvar is CVar<T> tCVar)
                return tCVar;
        }

        return null;
    }

    /// <summary>
    /// Finds a CVar with the given name. The value types of the CVars must also match to get a valid result.
    /// </summary>
    /// <param name="name">The name of the CVar.</param>
    /// <param name="cvar">The CVar itself, if found. Won't be null if this returns true.</param>
    /// <typeparam name="T">The value type of the CVar.</typeparam>
    /// <returns>True if the CVar was found, otherwise false.</returns>
    public static bool TryFindCVar<T>(string name, [NotNullWhen(true)] out CVar<T>? cvar) where T : IEquatable<T>, IParsable<T>
    {
        cvar = FindCVar<T>(name);
        return cvar != null;
    }
}