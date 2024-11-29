using JetBrains.Annotations;

namespace Radish.Configuration;

/// <summary>
/// Exception thrown when more than one CVar with the same name is registered.
/// </summary>
/// <param name="cvarName">The name of the CVar.</param>
[PublicAPI]
public class DuplicateCVarException(string cvarName)
    : Exception($"More than one exception with name '{cvarName}' registered");