#define MyAppName   "ComfyMAUI"
#define MyCompany   "AiShuoHua"
#define MyApp       "ComfyMAUI.App.exe"
[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{CE845203-848B-4D07-98A6-1E3A5CA8CDCD}
AppName={#MyAppName}
AppVersion=1.0
WizardStyle=modern
DefaultDirName={userappdata}\{#MyCompany}\{#MyAppName}
DefaultGroupName={#MyAppName}
UninstallDisplayIcon={app}\{#MyApp}
;SetupIconFile=Resources\AppIcon\ComfyUI.ico
Compression=lzma2/fast
SolidCompression=yes
OutputDir=./output
OutputBaseFilename={#MyAppName}.setup
ChangesAssociations = yes
; DisableDirPage=yes
; DisableProgramGroupPage=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "ChineseSimplified.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}";


[Files]
Source: "*"; Excludes: "output,*.pdb,*.iss,win7-x86,x86"; DestDir: "{app}"  ;Flags: ignoreversion recursesubdirs createallsubdirs


[Run]
Filename: {app}\{#MyApp}; Description: {cm:LaunchProgram, {#MyAppName}}; Flags: postinstall skipifsilent nowait
Filename: "cmd"; Parameters: "/c del MicrosoftEdgeWebview2Setup.exe"; Flags: runhidden

[UninstallRun]
;Filename: {app}\{#MyApp};Parameters: {#MyAppUninstallParameter}; Flags: runhidden

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyApp}" 
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyApp}"; Tasks: desktopicon

[Code]

function GetUninstallString: string;
var
  sUnInstPath: string;
  sUnInstallString: String;
begin
  Result := '';
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{{CE845203-848B-4D07-98A6-1E3A5CA8CDCD}_is1'); //Your App GUID/ID
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  Result := sUnInstallString;
end;

function DownloadAndInstallWebView2(): boolean;
var
  ErrorCode: Integer;
begin
  Result := false;
  if MsgBox('WebView2 Runtime is not installed. Do you want to download and install it now?', mbInformation, MB_YESNO) = IDYES then
  begin
    ExtractTemporaryFile('MicrosoftEdgeWebview2Setup.exe');
    if Exec(ExpandConstant('{tmp}\MicrosoftEdgeWebview2Setup.exe'), '', '', SW_SHOW, ewWaitUntilTerminated, ErrorCode) then
    begin
      if ErrorCode = 0 then
      begin
        Log('WebView2 Runtime installed successfully.');
        Result := true;
      end
      else
      begin
        Log('WebView2 Runtime installation failed with error code: ' + IntToStr(ErrorCode));
      end;
    end
    else
    begin
      Log('Failed to execute WebView2 installer.');
    end;
  end;
end;

function IsWebView2RuntimeNeeded(): boolean;
var
  Version: string;
  RuntimeNeeded: boolean;
  VerifyRuntime: boolean;
begin
  { See: https://learn.microsoft.com/en-us/microsoft-edge/webview2/concepts/distribution#detect-if-a-suitable-webview2-runtime-is-already-installed }

  RuntimeNeeded := true;
  VerifyRuntime := false;

  { Since we are using an elevated installer I am not checking HKCU }
  if (IsWin64) then
  begin
    { Test x64 }
    if (RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}', 'pv', Version)) then
    begin
      { We need to verify }
      VerifyRuntime := true;
    end;
  end
  else
  begin
    { Test x32 }
    if (RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}', 'pv', Version)) then
    begin
      { We need to verify }
      VerifyRuntime := true;
    end;
  end;

  { Verify the version information }
  if (VerifyRuntime) then
  begin
    if (Version <> '') and (Version <> '0.0.0.0') then
    begin
      Log('WebView2 Runtime is installed');
      RuntimeNeeded := false;
    end
    else
      Log('WebView2 Runtime needs to be downloaded and installed');
  end;

  Result := RuntimeNeeded;
end;

function InitializeSetup(): Boolean;
var
  V: Integer;
  ErrorCode: Integer;
  sUnInstallString: string;
begin
  if RegValueExists(HKEY_LOCAL_MACHINE,'Software\Microsoft\Windows\CurrentVersion\Uninstall\{CE845203-848B-4D07-98A6-1E3A5CA8CDCD}_is1', 'UninstallString') then  //Your App GUID/ID
  begin
    V := MsgBox(ExpandConstant('已经发现一个旧版本, 请卸载后继续.'), mbInformation, MB_YESNO); //Custom Message if App installed
    if V = IDYES then
    begin
      sUnInstallString := GetUninstallString();
      sUnInstallString :=  RemoveQuotes(sUnInstallString);
      Exec(ExpandConstant(sUnInstallString), '/SILENT /NORESTART /SUPPRESSMSGBOXES', '', SW_HIDE, ewWaitUntilTerminated, ErrorCode);
      Result := True; //if you want to proceed after uninstall
                //Exit; //if you want to quit after uninstall
    end
    else
      Result := False; //when older version present and not uninstalled
  end
  else
  begin
    if IsWebView2RuntimeNeeded() then
    begin
      Result := DownloadAndInstallWebView2();
    end
    else
    begin
      Result := True;
    end;
  end;
end;