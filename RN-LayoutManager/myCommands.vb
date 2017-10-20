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

' This line is not mandatory, but improves loading performances
<Assembly: CommandClass(GetType(RN_LayoutManager.MyCommands))>
Namespace RN_LayoutManager

    Public Class MyCommands
        Friend Shared m_palette As Autodesk.AutoCAD.Windows.PaletteSet = Nothing

        ' Application Session Command with localized name
        <CommandMethod("layoutman", CommandFlags.Modal + CommandFlags.Session)>
        Public Sub layoutman()
            If m_palette Is Nothing Then
                'palette nog niet geopend
                m_palette = New Autodesk.AutoCAD.Windows.PaletteSet("RN-LayoutManager", New Guid("{fad7b1e9-625e-4c10-9ba4-c94681a982cf}"))
                m_palette.Style = PaletteSetStyles.ShowPropertiesMenu + PaletteSetStyles.ShowAutoHideButton + PaletteSetStyles.ShowCloseButton
                Dim palette_overlayman As ucLayoutManager = New ucLayoutManager()
                m_palette.Add("RN-LayoutManager", palette_overlayman)

            End If
            'm_palette.Icon = GetEmbeddedIcon("aitoolsicon.ico")
            'palette aanzetten
            m_palette.Visible = True
        End Sub


    End Class

End Namespace