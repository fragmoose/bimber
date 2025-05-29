using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Bimber
{
    internal class AppSettings
    {
    }
}
public class AppSettings
{
    public string ApiKey { get; set; } = "f2aee9480da2c75211f143f4a308bff0c83b2990b72cf21794f207591db93a39";
    public bool StartWithWindows { get; set; }
    public string Hotkey { get; set; } = "";
    public string Language { get; set; } = "en";

    public void Save()
    {
        Bimber.Properties.Settings.Default.ApiKey = ApiKey;
        Bimber.Properties.Settings.Default.StartWithWindows = StartWithWindows;
        Bimber.Properties.Settings.Default.Hotkey = Hotkey;
        Bimber.Properties.Settings.Default.Language = Language;
        Bimber.Properties.Settings.Default.Save();
    }
    public void SetStartup(bool enable)
    {
        RegistryKey rk = Registry.CurrentUser.OpenSubKey(
            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        if (enable)
        {
            rk.SetValue("BackgroundApp", Application.ExecutablePath);
        }
        else
        {
            rk.DeleteValue("BackgroundApp", false);
        }
    }

    public static AppSettings Load()
    {
        return new AppSettings
        {
            ApiKey = Bimber.Properties.Settings.Default.ApiKey,
            StartWithWindows = Bimber.Properties.Settings.Default.StartWithWindows,
            Hotkey = Bimber.Properties.Settings.Default.Hotkey,
            Language = Bimber.Properties.Settings.Default.Language
        };
    }
}
public class HotkeyManager : IDisposable
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private IntPtr _windowHandle;
    private int _hotkeyId;

    public HotkeyManager(IntPtr windowHandle, int hotkeyId)
    {
        _windowHandle = windowHandle;
        _hotkeyId = hotkeyId;
    }

    public bool RegisterHotkey(Keys key, KeyModifiers modifiers)
    {
        return RegisterHotKey(_windowHandle, _hotkeyId, (int)modifiers, (int)key);
    }

    public void UnregisterHotkey()
    {
        UnregisterHotKey(_windowHandle, _hotkeyId);
    }

    public void Dispose()
    {
        UnregisterHotkey();
    }

    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }
}
