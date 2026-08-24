using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RifeOS.SDK.Context;
using RifeOS.SDK.Enums;
using RifeOS.SDK.Services;

namespace RifeOS.Apps.Settings.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    private readonly IRifeAppContext _context;

    public IUserProfile User => _context.CurrentUser;
    public IThemeContext Theme => _context.Theme;

    public string SystemVersion => "1.0.0-alpha";
    public string RuntimeArchitecture => ".NET 8 / C# 12 / WPF Microkernel";
    public string Author => "Renly";

    public SettingsViewModel(IRifeAppContext context)
    {
        _context = context;
    }

    [RelayCommand]
    public void SetTheme(string modeName)
    {
        if (Enum.TryParse<ThemeMode>(modeName, true, out var mode))
        {
            _context.Theme.SetTheme(mode);
        }
    }
}
