namespace Radish.Configuration;

/// <summary>
/// Interface for CVars.
/// </summary>
public interface ICVar
{
    /// <summary>
    /// The console name for the variable. Not case-sensitive for comparison purposes.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Description and help information for this variable.
    /// </summary>
    string HelpText { get; }
    
    /// <summary>
    /// Configuration flags for this variable.
    /// </summary>
    CVarFlags Flags { get; }
    
    /// <summary>
    /// String representation of the variable's value.
    /// </summary>
    /// <returns>String representation of the value.</returns>
    string GetValueString();
    
    /// <summary>
    /// Try and set the variable's value by parsing the given string.
    /// </summary>
    /// <param name="value">String value to try and parse.</param>
    /// <returns>True if the value was parsed and set successfully, otherwise false.</returns>
    bool TryParseAndSet(string value);
}