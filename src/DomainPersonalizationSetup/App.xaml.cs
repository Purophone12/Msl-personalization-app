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

using System.Configuration;
using System.Data;
using System.Windows;
using DomainPersonalizationSetup.Core;

namespace DomainPersonalizationSetup
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Initialize Logging
            Logger.Info("Application starting...");

            // 2. Load Config
            ConfigManager.Load();

            // 3. Environment Checks
            if (!EnvironmentChecker.IsDomainJoined())
            {
                Logger.Info("Not domain joined. Exiting.");
                MessageBox.Show("This setup tool is intended for domain-joined devices.", "Incompatible Device", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
                return;
            }

            if (EnvironmentChecker.IsSetupComplete())
            {
                Logger.Info("Setup already completed for this user. Exiting.");
                Shutdown();
                return;
            }

            Logger.Info("Starting UI...");
        }
    }
}
