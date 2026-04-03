// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using DomainPersonalizationSetup.Core;

namespace DomainPersonalizationSetup.Services
{
    public class PersonalizationService
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

        private const int SPI_SETDESKWALLPAPER = 0x0014;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        private const uint WM_SETTINGCHANGE = 0x001A;
        private const uint SMTO_ABORTIFHUNG = 0x0002;
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        private const string UserRegKeyPath = @"Software\DomainPersonalizationSetup";

        public void NotifyShell()
        {
            try
            {
                SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, "Environment", SMTO_ABORTIFHUNG, 5000, out _);
                Logger.Info("Notified shell of settings change.");
            }
            catch (Exception ex)
            {
                Logger.Error("Error notifying shell", ex);
            }
        }

        public void SetWallpaper(string imagePath)
        {
            try
            {
                if (System.IO.File.Exists(imagePath))
                {
                    SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
                    Logger.Info($"Wallpaper set to: {imagePath}");
                }
                else
                {
                    Logger.Error($"Wallpaper file not found: {imagePath}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error setting wallpaper", ex);
            }
        }

        public void SetTheme(bool isDark)
        {
            try
            {
                const string themeKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
                using var key = Registry.CurrentUser.CreateSubKey(themeKey);
                int value = isDark ? 0 : 1;
                key.SetValue("AppsUseLightTheme", value, RegistryValueKind.DWord);
                key.SetValue("SystemUsesLightTheme", value, RegistryValueKind.DWord);
                Logger.Info($"Theme set to: {(isDark ? "Dark" : "Light")}");
                NotifyShell();
            }
            catch (Exception ex)
            {
                Logger.Error("Error setting theme", ex);
            }
        }

        public void SetTaskbarAlignment(bool center)
        {
            try
            {
                // 0 = Left, 1 = Center (Windows 11)
                const string taskbarKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
                using var key = Registry.CurrentUser.CreateSubKey(taskbarKey);
                key.SetValue("TaskbarAl", center ? 1 : 0, RegistryValueKind.DWord);
                Logger.Info($"Taskbar alignment set to: {(center ? "Center" : "Left")}");
                NotifyShell();
            }
            catch (Exception ex)
            {
                Logger.Error("Error setting taskbar alignment", ex);
            }
        }

        public void SetNickname(string nickname)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(UserRegKeyPath);
                key.SetValue("Nickname", nickname, RegistryValueKind.String);
                Logger.Info($"Nickname set to: {nickname}");
            }
            catch (Exception ex)
            {
                Logger.Error("Error setting nickname", ex);
            }
        }
    }
}
