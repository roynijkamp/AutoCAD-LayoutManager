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
Imports Autodesk.AutoCAD.Interop
Imports CodeTech.Control

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


        <CommandMethod("closeallopen", CommandFlags.Modal + CommandFlags.Session)>
        Public Sub closeallopen()
            Dim RNmsgBox As RN_CustomAlerts.frmAlert = New RN_CustomAlerts.frmAlert
            RNmsgBox.WindowTitle = "Wijzigingen opslaan?"
            RNmsgBox.LabelTekst = "Wilt u de niet opgeslagen wijzigingen opslaan?"
            Dim dlgRes As Windows.Forms.DialogResult = RNmsgBox.ShowDialog()
            Dim bApplyToAll As Boolean = RNmsgBox.applytoall
            'bij cancel result exit sub
            If dlgRes = Windows.Forms.DialogResult.Cancel Then
                Exit Sub
            End If

            Dim docs As DocumentCollection = Application.DocumentManager



            For Each doc As Document In docs
                'cancel actiefe commando's
                'If Not doc.CommandInProgress = "" And Not doc.CommandInProgress = "closeallopen" Then
                '    'Dim oDoc As AcadDocument = doc.GetAcadDocument
                '    Dim oDoc As AcadDocument = DirectCast(doc.GetAcadDocument(), AcadDocument)
                '    oDoc.SendCommand(ChrW(3) & ChrW(3))
                'End If

                If doc.IsReadOnly Then
                    doc.CloseAndDiscard()
                Else
                    'maak document actief
                    If Not docs.MdiActiveDocument = doc Then
                        docs.MdiActiveDocument = doc
                    End If
                    Dim iIsModified As Integer = System.Convert.ToInt32(Application.GetSystemVariable("DBMOD"))
                    If iIsModified = 0 Then
                        'niet gewijzigd, dus sluiten
                        doc.CloseAndDiscard()
                    Else
                        'gewijzigd, kijken wat de keuze is
                        If dlgRes = Windows.Forms.DialogResult.Yes Then
                            'saven
                            If bApplyToAll Then
                                'keuze geld voor alle documenten
                                doc.CloseAndSave(doc.Name)
                            Else
                                'keuze moet per document gemaakt worden
                                If MsgBox("Wilt u de wijzigingen in dit document opslaan?", MsgBoxStyle.Question + MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                                    doc.CloseAndSave(doc.Name)
                                Else
                                    doc.CloseAndDiscard()
                                End If
                            End If
                        Else
                            'niet saven
                            If bApplyToAll Then
                                'keuze geld voor alle documenten
                                doc.CloseAndDiscard()
                            Else
                                'keuze moet per document gemaakt worden
                                If MsgBox("Wilt u de wijzigingen in dit document opslaan?", MsgBoxStyle.Question + MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                                    doc.CloseAndSave(doc.Name)
                                Else
                                    doc.CloseAndDiscard()
                                End If
                            End If
                        End If
                    End If
                End If
            Next
        End Sub

    End Class

End Namespace