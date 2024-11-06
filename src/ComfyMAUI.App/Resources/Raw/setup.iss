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

[Components]
;Name: "main"; Description: "Main Files"; Types: full compact custom; Flags: fixed

[Run]
Filename: {app}\{#MyApp}; Description: {cm:LaunchProgram, {#MyAppName}}; Flags: postinstall skipifsilent nowait
Filename: "cmd"; Parameters: "/c del MicrosoftEdgeWebview2Setup.exe"; Flags: runhidden

[UninstallRun]
;Filename: {app}\{#MyApp};Parameters: {#MyAppUninstallParameter}; Flags: runhidden

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyApp}" 
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyApp}"; Tasks: desktopicon

[Code]
var
  DownloadPage: TDownloadWizardPage;

function OnDownloadProgress(const Url, FileName: String; const Progress, ProgressMax: Int64): Boolean;
begin
  if Progress = ProgressMax then
    Log(Format('Successfully downloaded file to {tmp}: %s', [FileName]));
  Result := True;
end;

procedure InitializeWizard;
begin
  DownloadPage := CreateDownloadPage(SetupMessage(msgWizardPreparing), SetupMessage(msgPreparingDesc), @OnDownloadProgress);
end;

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

function IsGitInstalled(): Boolean;
var
GitPath: String;
begin
  // 检查 Git 是否在 PATH 中
  if RegQueryStringValue(HKLM64, 'SOFTWARE\GitForWindows', 'InstallPath', GitPath) then
  begin
    Result := True;
  end
  else
  begin
    Result := False;
  end;
end;


function DownloadAndInstallGit(): Boolean;
var
  GitInstallerURL: String;
  GitInstallerPath: String;
  ResultCode: Integer;
begin
  Result := False;
  
  if MsgBox('Git 未安装。是否下载并安装 Git？', mbConfirmation, MB_YESNO) = IDYES then
  begin
    try
      GitInstallerURL := 'https://registry.npmmirror.com/-/binary/git-for-windows/v2.43.0.windows.1/Git-2.43.0-64-bit.exe';
      GitInstallerPath := ExpandConstant('{tmp}\GitInstaller.exe');
      
      // 使用内置下载页面下载 Git
      DownloadPage.Clear;
      DownloadPage.Add(GitInstallerURL, 'GitInstaller.exe', '');
      DownloadPage.Show;
      try
        try
          DownloadPage.Download;
          // 安装 Git（静默安装）
          if Exec(GitInstallerPath, '/VERYSILENT /NORESTART /NOCANCEL /SP- /CLOSEAPPLICATIONS /RESTARTAPPLICATIONS', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
          begin
            if ResultCode = 0 then
            begin
              Result := True;
              Log('Git 安装成功');
            end
            else
            begin
              MsgBox('Git 安装失败，错误代码: ' + IntToStr(ResultCode), mbError, MB_OK);
            end;
          end
          else
          begin
            MsgBox('无法执行 Git 安装程序。', mbError, MB_OK);
          end;
        except
          if DownloadPage.AbortedByUser then
            Log('下载被用户取消。');
            Result := False;
        end;
      finally
        DownloadPage.Hide;
      end;
    except
      MsgBox('安装 Git 时发生错误。', mbError, MB_OK);
    end;
  end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  
  // 在第一页点击"下一步"时检查 Git
  if CurPageID = wpReady then
  begin
    if not IsGitInstalled() then
    begin
      if not DownloadAndInstallGit() then
      begin
        Result := False;
        Exit;
      end;
    end;
  end;
end;

function InitializeSetup(): Boolean;
var
  V: Integer;
  ErrorCode: Integer;
  sUnInstallString: string;
begin
  if RegValueExists(HKEY_LOCAL_MACHINE,'Software\Microsoft\Windows\CurrentVersion\Uninstall\{CE845203-848B-4D07-98A6-1E3A5CA8CDCD}_is1', 'UninstallString') then
  begin
    V := MsgBox(ExpandConstant('已经发现一个旧版本, 请卸载后继续.'), mbInformation, MB_YESNO);
    if V = IDYES then
    begin
      sUnInstallString := GetUninstallString();
      sUnInstallString :=  RemoveQuotes(sUnInstallString);
      Exec(ExpandConstant(sUnInstallString), '/SILENT /NORESTART /SUPPRESSMSGBOXES', '', SW_HIDE, ewWaitUntilTerminated, ErrorCode);
      Result := True;
    end
    else
      Result := False;
  end
  else
  begin
    Result := True;
    
    // 只检查 WebView2
    if IsWebView2RuntimeNeeded() then
    begin
      if not DownloadAndInstallWebView2() then
      begin
        Result := False;
        Exit;
      end;
    end;
  end;
end;