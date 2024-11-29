namespace Radish.Platform;

/// <summary>
/// A button for <see cref="Application.ShowMessageBox(Radish.Platform.MessageBoxType,Radish.Platform.IWindow?,string,string,IReadOnlyList{MessageBoxButton})"/>.
/// </summary>
/// <param name="Text">The text label for the button.</param>
/// <param name="IsDefault">Is this button the one selected by default? This should only be true for one button.</param>
public record struct MessageBoxButton(string Text, bool IsDefault = false);
