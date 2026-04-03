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
using System.IO;
using System.Text.Json;
using Microsoft.Win32;
using DomainPersonalizationSetup.Models;

namespace DomainPersonalizationSetup.Core
{
    public static class ConfigManager
    {
        private const string RegistryKeyPath = @"Software\DomainPersonalizationSetup";
        private const string ConfigPathValueName = "ConfigPath";
        private const string DefaultConfigFileName = "config.json";

        public static AppConfig Current { get; private set; } = new AppConfig();

        public static void Load()
        {
            try
            {
                string? configPath = GetConfigFilePath();
                if (configPath != null && File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    var loaded = JsonSerializer.Deserialize<AppConfig>(json);
                    if (loaded != null)
                    {
                        Current = loaded;
                        Logger.Info($"Config loaded from: {configPath}");
                        return;
                    }
                }
                Logger.Info("No config file found or load failed, using defaults.");
            }
            catch (Exception ex)
            {
                Logger.Error("Error loading config", ex);
            }
        }

        private static string? GetConfigFilePath()
        {
            // 1. Path from HKLM registry (GPO-controlled)
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(RegistryKeyPath);
                string? gpoPath = key?.GetValue(ConfigPathValueName) as string;
                if (!string.IsNullOrEmpty(gpoPath)) return gpoPath;
            }
            catch { }

            // 2. Same directory as EXE
            string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultConfigFileName);
            if (File.Exists(localPath)) return localPath;

            // 3. Fallback: %ProgramData%\DomainPersonalizationSetup\config.json
            string progDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "DomainPersonalizationSetup",
                DefaultConfigFileName);
            if (File.Exists(progDataPath)) return progDataPath;

            return null;
        }
    }
}
