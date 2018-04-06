; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "RN Layout Manager"
#define MyAppVersion "1.0.18096.1018"
#define MyAppPublisher "Roy Nijkamp"
#define MyAppURL "http://www.roynijkamp.nl"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{50FBDEB1-BFEE-4AF0-9CE6-9E826689CDDB}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
; AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName=C:\ProgramData\Autodesk\ApplicationPlugins\RNLayoutManager.bundle
DisableDirPage=yes
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution
OutputBaseFilename=RNLayoutManager-{#MyAppVersion}
SetupIconFile=K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution\logo.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"

[Files]
;DLL laden om te checken of AutoCAD draait
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution\psvince.dll"; Flags: dontcopy
;DLL opslaan om te kunnen gebruiken bij uninstall
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution\psvince.dll"; Destdir: "{app}\Contents"; Flags: ignoreversion recursesubdirs createallsubdirs
;Libraries and settings
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution\PlotPresets.json"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution\PackageContents.xml"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution\Handleiding.pdf"; DestDir: "{app}\Docs"; Flags: ignoreversion recursesubdirs createallsubdirs
;## CORE
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution\logo.ico"; DestDir: "{app}\Contents"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\bin\Release\Microsoft.VisualBasic.PowerPacks.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\bin\Release\RN-LayoutItems.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\bin\Release\RN-attribute-listitem.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\bin\Release\RN-LayoutManager.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Code]
// function IsModuleLoaded to call at install time
// added also setuponly flag
function IsModuleLoaded(modulename: String ):  Boolean;
external 'IsModuleLoaded@files:psvince.dll stdcall setuponly';

// function IsModuleLoadedU to call at uninstall time
// added also uninstallonly flag
function IsModuleLoadedU(modulename: String ):  Boolean;
external 'IsModuleLoaded@{app}\Contents\psvince.dll stdcall uninstallonly' ;


function InitializeSetup(): Boolean;
begin

  // check if autocad is running
  if IsModuleLoaded( 'acad.exe' ) then
  begin
    MsgBox( 'U heeft AutoCAD niet afgesloten! Sluit AutoCAD af en open de Setup opnieuw.',
             mbError, MB_OK );
    Result := false;
  end
  else Result := true;
end;

function InitializeUninstall(): Boolean;
begin

  // check if autocad is running
  if IsModuleLoadedU( 'acad.exe' ) then
  begin
    MsgBox( 'U heeft AutoCAD niet afgesloten! Sluit AutoCAD af en open de Setup opnieuw.',
             mbError, MB_OK );
    Result := false;
  end
  else Result := true;

  // Unload the DLL, otherwise the dll psvince is not deleted
  UnloadDLL(ExpandConstant('{app}\Contents\psvince.dll'));

end;
