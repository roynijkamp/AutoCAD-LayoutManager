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
Imports System.Windows.Forms
Imports Newtonsoft.Json.Linq

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
            'hier controle licentie
            Dim sCoreDir As String = clsFunctions.getCoreDir()
            Dim bLicenseCheck As Boolean = False
            If IO.File.Exists(sCoreDir & "\RNLAYMAN.LCF") Then
                'licentie file gevonden, controle
                bLicenseCheck = clsRegister.CheckLicense(sCoreDir & "\RNLAYMAN.LCF")
            End If
            If bLicenseCheck = False Then
                '- geen licentie -> registratie form
                Dim frmRegistration As New frmRegister
                frmRegistration.cmdCancel.DialogResult = DialogResult.Cancel
                frmRegistration.cmdRegister.DialogResult = DialogResult.OK
                Dim myDialogResult As DialogResult = frmRegistration.ShowDialog
                If myDialogResult = DialogResult.OK Then
                    'Registratie was succesvol
                    MsgBox("Registratie succesvol")
                ElseIf myDialogResult = DialogResult.Abort Then
                    'registratie was niet succesvol
                    MsgBox("fout bij de registratie")
                    Exit Sub
                Else
                    'geannuleerd
                    Exit Sub
                End If
            End If

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
            RNmsgBox.applytoall = True
            Dim dlgRes As Windows.Forms.DialogResult = RNmsgBox.ShowDialog()
            Dim bApplyToAll As Boolean = RNmsgBox.applytoall
            'bij cancel result exit sub
            If dlgRes = Windows.Forms.DialogResult.Cancel Then
                Exit Sub
            End If

            Dim docs As DocumentCollection = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager



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
                    Dim iIsModified As Integer = System.Convert.ToInt32(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("DBMOD"))
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

        Public Sub checkForUpdate()
            Dim sUserName As String = ""
            Dim sUserEmail As String = ""
            Dim sRegDate As String = ""
            Dim sUserID As String = ""
            Dim sURL As String = ""
            Dim sAppName As String = ""
            Dim sComputerName As String = ""
            Dim sCoreDir As String = clsFunctions.getCoreDir()
            Dim tdes As New clsTripleDES("royNijkamp@My3Dkey")

            'load license details
            If IO.File.Exists(sCoreDir & "\RNLAYMAN.LCF") Then
                Dim sLic As String = My.Computer.FileSystem.ReadAllText(sCoreDir & "\RNLAYMAN.LCF")
                Try
                    Dim sLicenseDecrypt As String = tdes.Decrypt(sLic)
                    Dim myObject As JObject = JObject.Parse(sLicenseDecrypt)
                    Dim aUserDet As JArray = myObject("details")
                    sComputerName = "Computername: " & aUserDet(0).SelectToken("computername").ToString
                    sUserName = aUserDet(0).SelectToken("name").ToString
                    'lblUserName.Text = sUserName & " [" & sComputername & "]"
                    sUserEmail = aUserDet(0).SelectToken("email").ToString
                    'lblUserEmail.Text = sUserEmail
                    sRegDate = aUserDet(0).SelectToken("regdate").ToString
                    'lblRegDate.Text = sRegDate
                    sUserID = aUserDet(0).SelectToken("userid").ToString
                Catch ex As System.Exception
                    MsgBox("Fout bij het lezen van de licentie!" & vbCrLf & ex.Message)
                    Exit Sub
                End Try
            End If


            Dim updCrypt As String = clsRegister.checkUpdates(sUserEmail, "versioncheck", sUserID)
            If updCrypt.Contains("error:") Then
                'pcbUpdate.BackgroundImage = My.Resources.icon_stop
                'lblUpdate.Text = updCrypt
            End If
            Try
                Dim sUpdateDecrypt As String = tdes.Decrypt(updCrypt)
                'JSON doorlopen
                Dim bUpdate As Boolean = False
                Dim myObject As JObject = JObject.Parse(sUpdateDecrypt)
                Dim aUserDet As JArray = myObject("details")
                Dim sComputernameResp As String = aUserDet(0).SelectToken("computername").ToString
                If sComputerName = sComputernameResp Then
                    'response is geldig voor deze pc
                    bUpdate = CBool(aUserDet(0).SelectToken("updateready").ToString)
                    If bUpdate Then
                        'update beschikbaar, melden

                        'pcbUpdate.BackgroundImage = My.Resources.icon_warning
                        Dim sNewVersion As String = aUserDet(0).SelectToken("version").ToString
                        sAppName = "RNODM-" & sNewVersion & ".exe"
                        sURL = aUserDet(0).SelectToken("update_url").ToString
                        'lblUpdate.Text = "Software update V: " & sNewVersion & " beschikbaar"
                        Dim sReleaseNotes As String = aUserDet(0).SelectToken("releasenotes").ToString
                        'webReleasenotes.DocumentText = sReleaseNotes
                        'webReleasenotes.Visible = True
                        'cmdUpdateNow.Visible = True
                        Dim RNmsgBox As RN_CustomAlerts.frmUpdate = New RN_CustomAlerts.frmUpdate
                        RNmsgBox.UserEmail = sUserEmail
                        RNmsgBox.UserName = sUserName
                        RNmsgBox.RegDate = sRegDate
                        RNmsgBox.UpdateOmschrijving = "Software update V: " & sNewVersion & " beschikbaar"
                        RNmsgBox.ReleaseNotes = sReleaseNotes
                        Dim dlgRes As Windows.Forms.DialogResult = RNmsgBox.ShowDialog()
                        'bij cancel result exit sub
                        If dlgRes = Windows.Forms.DialogResult.Cancel Then
                            Exit Sub
                        Else
                            'update hier
                        End If
                    End If
                End If
            Catch ex As System.Exception
                MsgBox("Fout bij het weergeven van de update!" & vbCrLf & ex.Message)
            End Try
        End Sub

    End Class

End Namespace