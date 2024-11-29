using JetBrains.Annotations;

namespace Radish.OnlineAPI;

/// <summary>
/// Interface to access cloud-saved data for different game platforms (Steam, EOS, etc.)
/// </summary>
[PublicAPI]
public interface ISaveData
{
    /// <summary>
    /// Opens a saved file. Will create the file if <see cref="access"/> is <see cref="FileAccess.Write"/> and
    /// it does not already exist.
    /// </summary>
    /// <param name="file">Path to the file to open.</param>
    /// <param name="location">Storage source for the file.</param>
    /// <param name="access">Read/write mode for the file.</param>
    /// <returns>Stream to the file if it could be opened, otherwise false.</returns>
    Stream? OpenFile(string file, FileLocation location, FileAccess access);
    
    /// <summary>
    /// Gets a list of files matching the given pattern.
    /// </summary>
    /// <param name="location">Storage source for the file.</param>
    /// <param name="searchPattern">Glob pattern of files to search for. Use null to search for all files at the location.</param>
    /// <returns>Enumeration of the files found.</returns>
    IEnumerable<string> GetFiles(FileLocation location, string? searchPattern = null);
    
    /// <summary>
    /// Checks whether the given file exists.
    /// </summary>
    /// <param name="location">Storage source for the file.</param>
    /// <param name="file">Path to the file to check for.</param>
    /// <returns>True if the file exists, otherwise false.</returns>
    bool Exists(FileLocation location, string file);
}