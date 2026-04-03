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

namespace DomainPersonalizationSetup.Core
{
    public static class Logger
    {
        private static readonly string LogDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DomainPersonalizationSetup");

        private static readonly string LogPath = Path.Combine(LogDir, "logs.txt");

        static Logger()
        {
            try
            {
                if (!Directory.Exists(LogDir))
                {
                    Directory.CreateDirectory(LogDir);
                }
            }
            catch { }
        }

        public static void Log(string message, string level = "INFO")
        {
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {level}: {message}{Environment.NewLine}";
                File.AppendAllText(LogPath, logEntry);
            }
            catch { }
        }

        public static void Info(string message) => Log(message, "INFO");
        public static void Error(string message, Exception? ex = null)
        {
            string fullMessage = ex != null ? $"{message} | Exception: {ex.Message}" : message;
            Log(fullMessage, "ERROR");
        }
    }
}
