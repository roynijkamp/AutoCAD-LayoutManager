; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "RN Layout Manager"
#define MyAppVersion "1.0.1031.1215"
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
OutputBaseFilename=BETA-RNLayoutManager-{#MyAppVersion}
SetupIconFile=K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution\logo.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"

[Files]
;DLL laden om te checken of AutoCAD draait
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution\psvince.dll"; Flags: dontcopy
;DLL opslaan om te kunnen gebruiken bij uninstall
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution\psvince.dll"; Destdir: "{app}\Contents"; Flags: ignoreversion
;Libraries and settings
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution\PlotPresets.json"; DestDir: "{app}\Contents"; Flags: ignoreversion
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution\PackageContents.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution\Handleiding.pdf"; DestDir: "{app}\Docs"; Flags: ignoreversion
;## CORE
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\Distribution\logo.ico"; DestDir: "{app}\Contents"; Flags: ignoreversion
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\bin\Debug\Microsoft.VisualBasic.PowerPacks.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\bin\Debug\RN-LayoutItems.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\bin\Debug\RN-attribute-listitem.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\bin\Debug\RN-LayoutManager.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\bin\Debug\RN-CustomAlerts.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\bin\Debug\Newtonsoft.Json.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-LayoutManager\RN-LayoutManager\bin\Debug\DropDownContainer.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion
;## update prog
Source: "K:\RoyN_doc\Documenten\Visual Studio 2015\Projects\RN-SmartUpdate\RN-SmartUpdate\bin\Debug\RN-SmartUpdate.exe"; DestDir: "{app}\Contents"; Flags: ignoreversion
; 
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
