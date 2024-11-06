#define MyAppName   "Comfy MAUI"
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
Source: "*"; Excludes: "output,*.pdb,*.iss,win7-x86"; DestDir: "{app}"  ;Flags: ignoreversion recursesubdirs createallsubdirs


[Run]
Filename: {app}\{#MyApp}; Description: {cm:LaunchProgram, {#MyAppName}}; Flags: postinstall skipifsilent nowait

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
    Result := True;
  end;
end;
