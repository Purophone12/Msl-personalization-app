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

using System.Collections.Generic;

namespace DomainPersonalizationSetup.Models
{
    public class AppConfig
    {
        public BrandingConfig Branding { get; set; } = new BrandingConfig();
        public FeatureConfig Features { get; set; } = new FeatureConfig();
        public Dictionary<string, string> Defaults { get; set; } = new Dictionary<string, string>();
    }

    public class BrandingConfig
    {
        public string OrganizationName { get; set; } = "Our Organization";
        public string? LogoPath { get; set; }
        public string? AccentColor { get; set; }
    }

    public class FeatureConfig
    {
        public bool EnableWallpaper { get; set; } = true;
        public bool EnableTheme { get; set; } = true;
        public bool EnableTaskbar { get; set; } = true;
        public bool EnableDefaultApps { get; set; } = true;
        public bool EnableNickname { get; set; } = true;
    }
}
