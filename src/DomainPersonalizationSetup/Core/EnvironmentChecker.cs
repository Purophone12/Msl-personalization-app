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
using System.Management;
using Microsoft.Win32;

namespace DomainPersonalizationSetup.Core
{
    public static class EnvironmentChecker
    {
        private const string UserRegKeyPath = @"Software\DomainPersonalizationSetup";
        private const string SetupCompletedValueName = "SetupCompleted";

        public static bool IsDomainJoined()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT PartOfDomain FROM Win32_ComputerSystem");
                foreach (var obj in searcher.Get())
                {
                    return (bool)obj["PartOfDomain"];
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error checking domain join status", ex);
            }
            return false;
        }

        public static bool IsSetupComplete()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(UserRegKeyPath);
                var value = key?.GetValue(SetupCompletedValueName);
                return value != null && (int)value == 1;
            }
            catch (Exception ex)
            {
                Logger.Error("Error checking setup completion status", ex);
            }
            return false;
        }

        public static void MarkSetupComplete()
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(UserRegKeyPath);
                key.SetValue(SetupCompletedValueName, 1, RegistryValueKind.DWord);
                Logger.Info("Setup marked as complete in HKCU.");
            }
            catch (Exception ex)
            {
                Logger.Error("Error marking setup as complete in registry", ex);
            }
        }
    }
}
