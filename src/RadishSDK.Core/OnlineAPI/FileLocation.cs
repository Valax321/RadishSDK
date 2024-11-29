namespace Radish.OnlineAPI;

/// <summary>
/// Source to load saved files from.
/// </summary>
public enum FileLocation
{
    /// <summary>
    /// Synced files will be cloud saved and available elsewhere. Useful for non-system settings.
    /// </summary>
    Synced,
    
    /// <summary>
    /// Local files live with cloud saved ones, but are system-specific. Useful for graphics settings,
    /// hardware config etc. which is only relevant for the current device.
    /// </summary>
    Local
}