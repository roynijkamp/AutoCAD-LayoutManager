' (C) Copyright 2017 by  Roy Nijkamp
'
Imports System
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Windows
Imports Autodesk.AutoCAD.Windows.ToolPalette
Imports System.IO

' This line is not mandatory, but improves loading performances
<Assembly: CommandClass(GetType(RN_LayoutManager.MyCommands))>
Namespace RN_LayoutManager

    Public Class MyCommands
        'auto-enable toolpalette
        Implements Autodesk.AutoCAD.Runtime.IExtensionApplication
        Friend Shared m_palette As Autodesk.AutoCAD.Windows.PaletteSet = Nothing

        Dim sIniDir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\RNtools"
        Dim sIniFile As String = "\layoutmanager.ini"
        Dim iniFile As clsINI
        Dim bAutoload As Boolean = True

        Public Sub Initialize() Implements IExtensionApplication.Initialize


        End Sub

        Public Sub Terminate() Implements IExtensionApplication.Terminate
            'afsluiten
        End Sub

        <CommandMethod("layoutman", CommandFlags.Modal + CommandFlags.Session)>
        Public Sub layoutman()
            If m_palette Is Nothing Then
                'palette nog niet geopend
                m_palette = New Autodesk.AutoCAD.Windows.PaletteSet("RN-LayoutManager", New Guid("{fad7b1e9-625e-4c10-9ba4-c94681a982cf}"))
                m_palette.Style = PaletteSetStyles.ShowPropertiesMenu + PaletteSetStyles.ShowAutoHideButton + PaletteSetStyles.ShowCloseButton
                Dim palette_overlayman As ucLayoutManager = New ucLayoutManager()
                m_palette.Add("LayoutManager", palette_overlayman)
                Dim palette_settings As ucSettings = New ucSettings()
                m_palette.Add("Instellingen", palette_settings)

            End If
            'm_palette.Icon = GetEmbeddedIcon("aitoolsicon.ico")
            'palette aanzetten
            m_palette.Visible = True
            m_palette.Activate(0)
        End Sub

        <CommandMethod("autoloadLayoutMan")>
        Public Sub autoloadLayoutMan()
            'MsgBox("Init Layoutman")
            If File.Exists(sIniDir & sIniFile) Then
                'bestand bestaat, instelingen laden
                iniFile = New clsINI(sIniDir & sIniFile)
                bAutoload = iniFile.GetBoolean("appsettings", "autoload", bAutoload)
            End If
            If bAutoload = True Then
                layoutman() 'start program
            End If
        End Sub

    End Class

End Namespace