using Bimber.Properties;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Bimber
{
    public class AppSettings
    {
       
        private string _pixvidApiKey;
        private string _fivemanageApiKey;

        public string ApiKey
        {
            get => _pixvidApiKey;
            set => _pixvidApiKey = value;
        }

        public string ApiKey2
        {
            get => _fivemanageApiKey;
            set => _fivemanageApiKey = value;
        }

        
        public bool StartWithWindows { get; set; }
        public string Hotkey { get; set; } = "";
        public string Language { get; set; } = "en";
        public string ImageUploaderType { get; set; } = "ImageUploader";
        public bool SaveLocally { get; set; } = false;
        public string LocalSavePath { get; set; } = string.Empty;

        public void Save()
        {
            Settings.Default.ApiKey = _pixvidApiKey;
            Settings.Default.ApiKey2 = _fivemanageApiKey;
            Settings.Default.StartWithWindows = StartWithWindows;
            Settings.Default.Hotkey = Hotkey;
            Settings.Default.Language = Language;
            Settings.Default.ImageUploaderType = ImageUploaderType;
            Settings.Default.SaveLocally = SaveLocally;
            Settings.Default.LocalSavePath = LocalSavePath;
            Settings.Default.Save();
        }

        public static AppSettings Load()
        {
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                if (string.IsNullOrEmpty(Settings.Default.ApiKey2))
        {
                    Settings.Default.ApiKey2 = Settings.Default.ApiKey;
                }
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            return new AppSettings
            {
                _pixvidApiKey = Settings.Default.ApiKey,
                _fivemanageApiKey = Settings.Default.ApiKey2,
                StartWithWindows = Settings.Default.StartWithWindows,
                Hotkey = Settings.Default.Hotkey,
                Language = Settings.Default.Language,
                ImageUploaderType = Settings.Default.ImageUploaderType,
                SaveLocally = Settings.Default.SaveLocally,
                LocalSavePath = Settings.Default.LocalSavePath ?? string.Empty
            };
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
    }
}