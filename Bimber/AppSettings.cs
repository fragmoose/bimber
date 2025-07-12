using Bimber.Properties;
using Microsoft.Win32;
using System;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
    public string ImageUploaderType { get; set; } = "ImageUploader";
    public bool SaveLocally { get; set; } = false;
    public string LocalSavePath { get; set; } = string.Empty;

    public void Save()
    {
        Settings.Default.ApiKey = ApiKey;
        Settings.Default.StartWithWindows = StartWithWindows;
        Settings.Default.Hotkey = Hotkey;
        Settings.Default.Language = Language;
        Settings.Default.ImageUploaderType = ImageUploaderType;
        Settings.Default.SaveLocally = SaveLocally;
        Settings.Default.LocalSavePath = LocalSavePath;
        Settings.Default.Save();
    }

    public void SetStartup(bool enable)
    {
        using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(
            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
        {
            if (rk == null) return;

            if (enable)
                rk.SetValue("Bimber", Application.ExecutablePath);
            else
                rk.DeleteValue("Bimber", false);
        }
    }

    // Add this static Load method
    public static AppSettings Load()
    {
        // Automatic settings upgrade for new versions
        if (Settings.Default.UpgradeRequired)
        {
            Settings.Default.Upgrade();
            Settings.Default.UpgradeRequired = false;
            Settings.Default.Save();
        }

        return new AppSettings
        {
            ApiKey = Settings.Default.ApiKey,
            StartWithWindows = Settings.Default.StartWithWindows,
            Hotkey = Settings.Default.Hotkey,
            Language = Settings.Default.Language,
            ImageUploaderType = Settings.Default.ImageUploaderType,
            SaveLocally = Settings.Default.SaveLocally,
            LocalSavePath = Settings.Default.LocalSavePath ?? string.Empty
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
