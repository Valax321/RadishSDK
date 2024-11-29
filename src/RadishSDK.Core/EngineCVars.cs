using System.Reflection;
using JetBrains.Annotations;
using Radish.Configuration;

namespace Radish;

/// <summary>
/// <see cref="CVar{T}"/>s used by the engine.
/// </summary>
[PublicAPI]
public static class EngineCVars
{
    /// <summary>
    /// The directory where game data files are located. On some platforms (macOS for instance) this isn't the
    /// same location where the binaries are.
    /// </summary>
    public static readonly CVar<string> BaseDirectory = ConfigurationManager.CreateCVar(
        "fs_basedir",
        AppDomain.CurrentDomain.BaseDirectory,
        "Directory where the game data is located.",
        CVarFlags.ReadOnly);

    /// <summary>
    /// The directory where the persistent user data is located. This will be a platform-specific directory
    /// within the user's $HOME, most likely.
    /// </summary>
    public static readonly CVar<string> PersistentDataDirectory = ConfigurationManager.CreateCVar(
        "fs_persistdir",
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RadishSDK",
            Assembly.GetEntryAssembly()?.GetName().Name ?? "Common"),
        "Directory where local persistent game data is located.",
        CVarFlags.ReadOnly
    );
}