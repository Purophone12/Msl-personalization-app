# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

import json
import os
import argparse

# NSIS Template
NSIS_TEMPLATE = """
!include "MUI2.nsh"

; General
Name "{AppName}"
OutFile "{OutFile}"
InstallDir "{DefaultInstallDir}\\{AppName}"
InstallDirRegKey HKCU "Software\\{AppName}" "Install_Dir"
RequestExecutionLevel {ExecutionLevel}

; VIProductVersion
VIProductVersion "{Version}"
VIAddVersionKey "ProductName" "{AppName}"
VIAddVersionKey "CompanyName" "{OrganizationName}"
VIAddVersionKey "FileVersion" "{Version}"
VIAddVersionKey "FileDescription" "{AppName} Installer"
VIAddVersionKey "LegalCopyright" "Copyright (c) 2026 {OrganizationName}"

; UI Settings
!define MUI_ABORTWARNING
!define MUI_WELCOMEPAGE_TITLE "Welcome to {AppName} Setup"
!define MUI_FINISHPAGE_RUN "$INSTDIR\\{ExecutableName}"

; Pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

; Languages
!insertmacro MUI_LANGUAGE "English"

Section "Install"
  SetOutPath "$INSTDIR"

  ; Application Files
  File /r "publish\\*.*"
  File "LICENSE"
  File "NOTICE"
  File "config.json.example"

  ; Registry for installation info
  WriteRegStr HKCU "Software\\{AppName}" "Install_Dir" "$INSTDIR"

  ; Uninstaller
  WriteUninstaller "$INSTDIR\\uninstall.exe"

  ; Start Menu Shortcut
  CreateDirectory "$SMPROGRAMS\\{AppName}"
  CreateShortcut "$SMPROGRAMS\\{AppName}\\{AppName}.lnk" "$INSTDIR\\{ExecutableName}"
  CreateShortcut "$SMPROGRAMS\\{AppName}\\Uninstall.lnk" "$INSTDIR\\uninstall.exe"

  {DesktopShortcut}

SectionEnd

Section "Uninstall"
  Delete "$INSTDIR\\uninstall.exe"
  RMDir /r "$INSTDIR"

  Delete "$SMPROGRAMS\\{AppName}\\{AppName}.lnk"
  Delete "$SMPROGRAMS\\{AppName}\\Uninstall.lnk"
  RMDir "$SMPROGRAMS\\{AppName}"

  DeleteRegKey /ifempty HKCU "Software\\{AppName}"
SectionEnd
"""

# WiX Template (Simplified for generating an MSI)
WIX_TEMPLATE = """<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
    <Product Id="*" Name="{AppName}" Language="1033" Version="{Version}" Manufacturer="{OrganizationName}" UpgradeCode="{UpgradeCode}">
        <Package InstallerVersion="200" Compressed="yes" InstallScope="{InstallScope}" />

        <MajorUpgrade DowngradeErrorMessage="A newer version of [ProductName] is already installed." />
        <MediaTemplate EmbedCab="yes" />

        <Feature Id="ProductFeature" Title="{AppName}" Level="1">
            <ComponentGroupRef Id="ProductComponents" />
        </Feature>
    </Product>

    <Fragment>
        <Directory Id="TARGETDIR" Name="SourceDir">
            <Directory Id="{ProgramFilesDir}">
                <Directory Id="INSTALLFOLDER" Name="{AppName}" />
            </Directory>
        </Directory>
    </Fragment>

    <Fragment>
        <ComponentGroup Id="ProductComponents" Directory="INSTALLFOLDER">
            <!-- Simplified: In a real tool, we would list all files from publish/ -->
            <Component Id="MainExecutable" Guid="*">
                <File Id="AppEXE" Source="publish\\{ExecutableName}" KeyPath="yes" />
            </Component>
            <Component Id="LicenseFile" Guid="*">
                <File Id="AppLicense" Source="LICENSE" />
            </Component>
        </ComponentGroup>
    </Fragment>
</Wix>
"""

def generate_nsis_script(config):
    desktop_shortcut = ""
    if config.get("CreateDesktopShortcut", True):
        desktop_shortcut = f'CreateShortcut "$DESKTOP\\\\{config["AppName"]}.lnk" "$INSTDIR\\\\{config["ExecutableName"]}"'

    execution_level = "user" if config.get("PerUser", True) else "admin"
    default_install_dir = "$LOCALAPPDATA" if config.get("PerUser", True) else "$PROGRAMFILES"

    # Ensure version is in X.X.X.X format for NSIS VIProductVersion
    ver = config.get("Version", "1.0.0")
    if ver.count('.') == 2: ver += ".0"

    nsis_content = NSIS_TEMPLATE.format(
        AppName=config["AppName"],
        OutFile=config.get("OutFile", config["AppName"] + "_Installer.exe"),
        ExecutableName=config["ExecutableName"],
        DefaultInstallDir=default_install_dir,
        ExecutionLevel=execution_level,
        DesktopShortcut=desktop_shortcut,
        Version=ver,
        OrganizationName=config.get("OrganizationName", "Our Organization")
    )

    with open("installer.nsi", "w") as f:
        f.write(nsis_content)
    print("NSIS script 'installer.nsi' (EXE) generated successfully.")

def generate_wix_script(config):
    # Simplified WiX generation for demonstration
    ver = config.get("Version", "1.0.0")
    install_scope = "perUser" if config.get("PerUser", True) else "perMachine"
    program_files_dir = "LocalAppDataFolder" if config.get("PerUser", True) else "ProgramFilesFolder"
    upgrade_code = "7a4e69b2-0941-4c12-875c-15a995e80d47" # Should be unique

    wix_content = WIX_TEMPLATE.format(
        AppName=config["AppName"],
        Version=ver,
        OrganizationName=config.get("OrganizationName", "Our Organization"),
        UpgradeCode=upgrade_code,
        InstallScope=install_scope,
        ProgramFilesDir=program_files_dir,
        ExecutableName=config["ExecutableName"]
    )

    with open("installer.wxs", "w") as f:
        f.write(wix_content)
    print("WiX script 'installer.wxs' (MSI source) generated successfully.")

def main():
    parser = argparse.ArgumentParser(description="Generate installer scripts for Domain Personalization Setup.")
    parser.add_argument("--config", default="installer_config.json", help="Path to the installer configuration file.")
    parser.add_argument("--type", choices=["exe", "msi", "both"], default="exe", help="Type of installer to generate.")
    args = parser.parse_args()

    if not os.path.exists(args.config):
        default_config = {
            "AppName": "DomainPersonalizationSetup",
            "ExecutableName": "DomainPersonalizationSetup.exe",
            "OrganizationName": "Contoso Corp",
            "Version": "1.0.0",
            "PerUser": True,
            "CreateDesktopShortcut": True
        }
        with open(args.config, "w") as f:
            json.dump(default_config, f, indent=4)
        print(f"Created default config at {args.config}")

    with open(args.config, "r") as f:
        config = json.load(f)

    if args.type in ["exe", "both"]:
        generate_nsis_script(config)
    if args.type in ["msi", "both"]:
        generate_wix_script(config)

if __name__ == "__main__":
    main()
