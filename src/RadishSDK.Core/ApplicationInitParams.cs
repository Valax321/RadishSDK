using Radish.Platform;

namespace Radish;

/// <summary>
/// Initialization parameters for an <see cref="Application"/>.
/// </summary>
/// <param name="PlatformManager">Platform manager to use.
/// By default, will select one appropriate for this platform.</param>
/// <param name="WaitForEvents">If this is set, the main loop will not run constantly but only when the app
/// receives events. This might be useful for desktop apps to save CPU/GPU time when not being interacted with.</param>
public record struct ApplicationInitParams(
    IPlatformManager PlatformManager,
    bool WaitForEvents = false
);