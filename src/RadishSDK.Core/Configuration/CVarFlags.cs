namespace Radish.Configuration;

/// <summary>
/// Options for controlling <see cref="CVar{T}"/> behaviour.
/// </summary>
[Flags]
public enum CVarFlags
{
    /// <summary>
    /// No special behaviour.
    /// </summary>
    None = 1 << 0,
    
    /// <summary>
    /// Can only be changed from default if cheats are enabled.
    /// </summary>
    Cheat = 1 << 1,
    
    /// <summary>
    /// Save this variable to the local user config.
    /// </summary>
    ArchiveSystem = 1 << 2,
    
    /// <summary>
    /// Disallow modifying this variable from the console and configs.
    /// Game code can still set it.
    /// <seealso cref="Constant"/>
    /// </summary>
    ReadOnly = 1 << 3,
    
    /// <summary>
    /// Save this variable to the cloud synced user config.
    /// </summary>
    ArchiveUser = 1 << 4,
    
    /// <summary>
    /// Disallow modifying this variable entirely.
    /// <seealso cref="ReadOnly"/>
    /// </summary>
    Constant,
}