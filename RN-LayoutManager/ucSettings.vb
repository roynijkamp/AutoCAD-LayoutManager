Imports System.IO
Imports System.Reflection
Imports System.Security.AccessControl
Imports System.Windows.Forms
Imports System.Xml
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Newtonsoft.Json.Linq

Public Class ucSettings
    Dim sIniDir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\RNtools"
    Dim sIniFile As String = "\layoutmanager.ini"
    Dim sPlotPreferences As String = "PlotPresets.json"
    Dim sPackageContents As String = "PackageContents.xml"
    Dim sPackageContentsNew As String = "PackageContentsNew.xml"
    Dim iniFile As clsINI
    Dim sPDFuserFolder As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
    Dim sDefaultOutputLocation As String = ""
    Dim bUseDWGname As Boolean = False
    Dim sLayoutTemplate As String = ""
    Dim sLayoutTemplates As List(Of String)
    Dim sLayoutTemplateFolder As String = ""
    Dim sCurrVersion As String = Assembly.GetExecutingAssembly().GetName().Version.ToString
    Dim sDefaultPlottingDevice As String = "AutoCAD PDF (General Documentation).PC3"
    Dim bTrashDSD As Boolean = True
    Dim dtPlotPresets As System.Data.DataTable
    Dim bIsLoading As Boolean = False
    Dim bAutoLoad As Boolean = True
    Dim bUseBesteknr As Boolean = False
    Dim bUseBladnr As Boolean = False
    Dim bUseVersie As Boolean = False
    Dim bVersieInFront As Boolean = False
    'license vars
    Dim sUserName As String
    Dim sUserEmail As String
    Dim sRegDate As String
    Dim sUserID As String
    Dim sURL As String
    Dim sAppName As String
    Shared sComputerName As String = My.Computer.Name
    Dim sCoreDir As String = clsFunctions.getCoreDir()
    Dim tdes As New clsTripleDES("royNijkamp@My3Dkey")

    Private Sub radioPDFuserFolder_CheckedChanged(sender As Object, e As EventArgs) Handles radioPDFuserFolder.CheckedChanged
        If radioPDFuserFolder.Checked Then
            txtUserSaveFolder.Enabled = True
            cmdSelectUserSaveFolder.Enabled = True
            sDefaultOutputLocation = "userlocation"
            saveDefaultOutput()
        Else
            txtUserSaveFolder.Enabled = False
            cmdSelectUserSaveFolder.Enabled = False
        End If
    End Sub


    Private Sub cmdSelectUserSaveFolder_Click(sender As Object, e As EventArgs) Handles cmdSelectUserSaveFolder.Click
        Using fldrDia As FolderBrowserDialog = New FolderBrowserDialog
            If txtUserSaveFolder.Text.Length > 0 And Directory.Exists(txtUserSaveFolder.Text) Then
                fldrDia.SelectedPath = txtUserSaveFolder.Text
            End If
            If fldrDia.ShowDialog() = DialogResult.OK Then
                'eerst check of map wel bestaat
                If My.Computer.FileSystem.DirectoryExists(sIniDir) = False Then
                    My.Computer.FileSystem.CreateDirectory(sIniDir)
                End If
                sPDFuserFolder = fldrDia.SelectedPath
                txtUserSaveFolder.Text = sPDFuserFolder
                'bestand aanmaken
                'settings schrijven
                iniFile = New clsINI(sIniDir & sIniFile)
                iniFile.WriteString("publishsettings", "outputfolder", sPDFuserFolder)
            End If
        End Using
    End Sub

    Private Sub ucSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
        loadSettings()
    End Sub
    Public Sub loadSettings()
        bIsLoading = True
        Try
            'check of ini bestand bestaat, zo ja: instellingen laden
            If File.Exists(sIniDir & sIniFile) Then
                'bestand bestaat, instelingen laden
                iniFile = New clsINI(sIniDir & sIniFile)
                'MsgBox(sIniDir & sIniFile)
                'versie updaten
                iniFile.WriteString("appsettings", "version", sCurrVersion)
                sPDFuserFolder = iniFile.GetString("publishsettings", "outputfolder", sPDFuserFolder)
                txtUserSaveFolder.Text = sPDFuserFolder
                sLayoutTemplate = iniFile.GetString("template", "layout", sLayoutTemplate)
                'layout templates laden
                Dim temp As String = iniFile.GetString("template", "layouts", "")
                'MsgBox(temp.ToString)
                sLayoutTemplates = New List(Of String)(temp.Split(","c))
                'MsgBox(sLayoutTemplates.Count.ToString)
                chkListboxTemplates.Items.Clear()
                For Each sItem As String In sLayoutTemplates
                    If Not sItem = vbNullString Then
                        Dim bChecked As Boolean = False
                        If sItem = sLayoutTemplate Then
                            bChecked = True
                        End If
                        chkListboxTemplates.Items.Add(sItem, bChecked)
                    End If
                Next
                If chkListboxTemplates.CheckedItems.Count = 0 Then
                    'niets geselecteerd, eerste item selecteren
                    If chkListboxTemplates.Items.Count > 0 Then
                        sLayoutTemplate = chkListboxTemplates.Items(0).ToString
                        iniFile.WriteString("template", "layout", sLayoutTemplate)
                        chkListboxTemplates.SetSelected(0, True)
                    End If
                End If
                sLayoutTemplateFolder = iniFile.GetString("template", "templatefolder", "")
                cmdSetTPLFolder.Text = "Template map: " & sLayoutTemplateFolder

                sDefaultOutputLocation = iniFile.GetString("publishsettings", "defaultoutput", sDefaultOutputLocation)
                Select Case sDefaultOutputLocation
                    Case "drawingfolder"
                        radioPDFdrawingFolder.Checked = True
                    Case "userlocation"
                        radioPDFuserFolder.Checked = True
                    Case "askonplot"
                        radioPDFfolderAsk.Checked = True
                    Case Else
                        radioPDFdrawingFolder.Checked = True
                End Select
                sDefaultPlottingDevice = iniFile.GetString("publishsettings", "defaultplotter", sDefaultPlottingDevice)
                loadPlotConfigs()
                bUseDWGname = iniFile.GetBoolean("publishsettings", "usedwgname", bUseDWGname)
                bAutoLoad = iniFile.GetBoolean("appsettings", "autoload", bAutoLoad)

                'filename settings
                bUseBesteknr = iniFile.GetBoolean("publishsettings", "usebesteknummer", bUseBesteknr)
                bUseBladnr = iniFile.GetBoolean("publishsettings", "usebladnummer", bUseBladnr)
                bUseVersie = iniFile.GetBoolean("publishsettings", "useversie", bUseVersie)
                bVersieInFront = iniFile.GetBoolean("publishsettings", "versieinfront", bVersieInFront)
                setFileNameSettings()

            Else
                'eerst check of map wel bestaat
                If My.Computer.FileSystem.DirectoryExists(sIniDir) = False Then
                    My.Computer.FileSystem.CreateDirectory(sIniDir)
                    iniFile = New clsINI(sIniDir & sIniFile)
                    iniFile.WriteString("appsettings", "version", sCurrVersion)
                End If
            End If
            lblVersion.Text = sCurrVersion
            chkAutoLoad.Checked = bAutoLoad
            chkUseDWGname.Checked = bUseDWGname

            'load license details
            If IO.File.Exists(sCoreDir & "\RNLAYMAN.LCF") Then
                Dim sLic As String = My.Computer.FileSystem.ReadAllText(sCoreDir & "\RNLAYMAN.LCF")
                Try
                    Dim sLicenseDecrypt As String = tdes.Decrypt(sLic)
                    Dim myObject As JObject = JObject.Parse(sLicenseDecrypt)
                    Dim aUserDet As JArray = myObject("details")
                    Dim sComputername As String = "Computername: " & aUserDet(0).SelectToken("computername").ToString
                    sUserName = aUserDet(0).SelectToken("name").ToString
                    lblUserName.Text = sUserName & " [" & sComputername & "]"
                    sUserEmail = aUserDet(0).SelectToken("email").ToString
                    lblUserEmail.Text = sUserEmail
                    sRegDate = aUserDet(0).SelectToken("regdate").ToString
                    lblRegDate.Text = sRegDate
                    sUserID = aUserDet(0).SelectToken("userid").ToString
                    'licentie succesvol ingeladen, check voor update 1x per dag
                    'check for update tenzij het een debug versie is
                    If clsFunctions.isDebugMode() = False Then
                        checkForUpdate()
                    End If
                    'Dim sCurrDateTime As String = Date.Today()
                    'If checkForUpdate() = False Then
                    '    Exit Sub
                    'End If
                Catch ex As System.Exception
                    MsgBox("Fout bij het lezen van de licentie!" & vbCrLf & "Coredir: " & sCoreDir & vbCrLf & ex.Message)
                End Try
            End If

            bIsLoading = False
        Catch ex As Exception
            MsgBox("Fout bij het laden van de instellingen " & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub radioPDFfolderAsk_CheckedChanged(sender As Object, e As EventArgs) Handles radioPDFfolderAsk.CheckedChanged
        sDefaultOutputLocation = "askonplot"
        saveDefaultOutput()
    End Sub

    Private Sub radioPDFdrawingFolder_CheckedChanged(sender As Object, e As EventArgs) Handles radioPDFdrawingFolder.CheckedChanged
        sDefaultOutputLocation = "drawingfolder"
        saveDefaultOutput()
    End Sub
    Public Sub saveDefaultOutput()
        iniFile = New clsINI(sIniDir & sIniFile)
        iniFile.WriteString("publishsettings", "defaultoutput", sDefaultOutputLocation)
    End Sub

    Public Sub loadPlotConfigs()
        cmbPlottingDevice.DataSource = Nothing
        dtPlotPresets = New System.Data.DataTable
        Dim iSelectedIndex As Integer = 0
        Dim sSettingsFile As String = clsFunctions.getCoreDir() & "\" & sPlotPreferences
        clsFunctions.loadPlotPresets(sSettingsFile, dtPlotPresets, iSelectedIndex, sDefaultPlottingDevice)
        If Not dtPlotPresets.Rows.Count = 0 Then
            cmbPlottingDevice.DataSource = dtPlotPresets
            cmbPlottingDevice.DisplayMember = "preset"
            cmbPlottingDevice.ValueMember = "id"
            cmbPlottingDevice.SelectedIndex = iSelectedIndex
        Else
        End If
    End Sub

    Public Sub setFileNameSettings()
        Dim sFileNamePeview As String = ""
        chkUseBesteknummer.Checked = bUseBesteknr
        chkUseBladnummer.Checked = bUseBladnr
        chkUseVersienummer.Checked = bUseVersie
        chkVersieVooraan.Checked = bVersieInFront
        'saven die hap
        iniFile = New clsINI(sIniDir & sIniFile)
        iniFile.WriteBoolean("publishsettings", "usebesteknummer", bUseBesteknr)
        iniFile.WriteBoolean("publishsettings", "usebladnummer", bUseBladnr)
        iniFile.WriteBoolean("publishsettings", "useversie", bUseVersie)
        iniFile.WriteBoolean("publishsettings", "versieinfront", bVersieInFront)

        If bUseBesteknr Then
            sFileNamePeview = sFileNamePeview & "[#besteknr]-"
        End If
        If bUseBladnr Then
            sFileNamePeview = sFileNamePeview & "[#bladnummer] "
        End If
        sFileNamePeview = sFileNamePeview & "[#layoutnaam]"
        If bUseVersie Then
            'check versie voor of achteraan
            If bVersieInFront Then
                sFileNamePeview = "V[versienr]-" & sFileNamePeview
            Else
                sFileNamePeview = sFileNamePeview & " - V[versienr]"
            End If
        End If
        lblFilenamePreview.Text = sFileNamePeview
    End Sub

    Private Sub cmbPlottingDevice_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbPlottingDevice.SelectedIndexChanged
        iniFile = New clsINI(sIniDir & sIniFile)
        sDefaultPlottingDevice = cmbPlottingDevice.Text
        iniFile.WriteString("publishsettings", "defaultplotter", sDefaultPlottingDevice)
    End Sub


    Private Sub lblVersion_DoubleClick(sender As Object, e As EventArgs) Handles lblVersion.DoubleClick
        loadSettings()
    End Sub

    Private Sub chkTrashDSD_CheckedChanged(sender As Object, e As EventArgs) Handles chkTrashDSD.CheckedChanged
        iniFile = New clsINI(sIniDir & sIniFile)
        bTrashDSD = chkTrashDSD.Checked
        iniFile.WriteBoolean("debugoptions", "trashdsd", bTrashDSD)
    End Sub

    Private Sub cmdDebugOptions_Click(sender As Object, e As EventArgs) Handles cmdDebugOptions.Click
        grpDebugOptions.Enabled = Not grpDebugOptions.Enabled
        If grpDebugOptions.Enabled Then
            cmdDebugOptions.Text = "Disable debug options"
            iniFile = New clsINI(sIniDir & sIniFile)
            bTrashDSD = iniFile.GetBoolean("debugoptions", "trashdsd", bTrashDSD)
            chkTrashDSD.Checked = bTrashDSD
        Else
            cmdDebugOptions.Text = "Enable debug options"
        End If
    End Sub

    Private Sub cmdBrowseLayoutTemplate_Click(sender As Object, e As EventArgs) Handles cmdBrowseLayoutTemplate.Click
        Using fldrDia As OpenFileDialog = New OpenFileDialog
            fldrDia.InitialDirectory = sLayoutTemplateFolder
            If fldrDia.ShowDialog() = DialogResult.OK Then
                'eerst check of map wel bestaat
                If My.Computer.FileSystem.DirectoryExists(sIniDir) = False Then
                    My.Computer.FileSystem.CreateDirectory(sIniDir)
                End If
                If fldrDia.FileName.Contains(sLayoutTemplateFolder) Then
                    Dim sFile As String = fldrDia.SafeFileName
                    If Not sLayoutTemplates.Contains(sFile) Then
                        sLayoutTemplates.Add(sFile)
                        chkListboxTemplates.Items.Add(sFile, False)
                    End If
                    'If Not sLayoutTemplates.Contains(fldrDia.FileName) Then
                    '    sLayoutTemplates.Add(fldrDia.FileName)
                    '    chkListboxTemplates.Items.Add(fldrDia.FileName, False)
                    'End If
                    'bestand aanmaken
                    'settings schrijven
                    iniFile = New clsINI(sIniDir & sIniFile)
                    If sLayoutTemplates.Count > 0 Then
                        iniFile.WriteString("template", "layouts", String.Join(",", sLayoutTemplates.ToArray()))
                    Else
                        iniFile.WriteString("template", "layouts", "")
                    End If
                Else
                    MsgBox("Dit bestand staat niet in de Template map!" & vbCrLf & "Verplaats het bestand naar de template map")
                End If
            End If
        End Using
    End Sub

    Private Sub VerwijderenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VerwijderenToolStripMenuItem.Click
        If MsgBox("Weet u zeker dat u deze template wilt verwijderen uit de lijst?", MsgBoxStyle.Critical + MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            chkListboxTemplates.Items.Remove(chkListboxTemplates.SelectedItem)
            sLayoutTemplates = New List(Of String)
            For Each item In chkListboxTemplates.Items
                sLayoutTemplates.Add(item.ToString)
            Next
            If sLayoutTemplates.Count > 0 Then
                iniFile.WriteString("template", "layouts", String.Join(",", sLayoutTemplates.ToArray()))
            Else
                iniFile.WriteString("template", "layouts", "")
            End If
        End If
    End Sub


    Private Sub chkListboxTemplates_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles chkListboxTemplates.ItemCheck
        If bIsLoading = True Then
            Exit Sub
        End If
        If e.NewValue = CheckState.Checked Then
            For i As Integer = 0 To chkListboxTemplates.Items.Count - 1 Step 1
                If i <> e.Index Then
                    chkListboxTemplates.SetItemChecked(i, False)
                Else
                    sLayoutTemplate = chkListboxTemplates.Items(i)
                    'settings schrijven
                    iniFile = New clsINI(sIniDir & sIniFile)
                    iniFile.WriteString("template", "layout", sLayoutTemplate)
                End If
            Next i
        End If

    End Sub

    Private Sub chkListboxTemplates_MouseDown(sender As Object, e As MouseEventArgs) Handles chkListboxTemplates.MouseDown
        If e.Button = MouseButtons.Right Then
            chkListboxTemplates.SelectedIndex = chkListboxTemplates.IndexFromPoint(e.X, e.Y)
            editListTemplates.Show(chkListboxTemplates, e.Location)
        End If
    End Sub

    Private Sub chkAutoLoad_CheckedChanged(sender As Object, e As EventArgs) Handles chkAutoLoad.CheckedChanged
        bAutoLoad = chkAutoLoad.Checked
        iniFile = New clsINI(sIniDir & sIniFile)
        iniFile.WriteBoolean("appsettings", "autoload", bAutoLoad)
        If bIsLoading = False Then
            'wijzigingen alleen opslaan bij user action
            'updatePackageContents(bAutoLoad)
        End If
    End Sub

    Private Sub cmdGPStest_Click(sender As Object, e As EventArgs) Handles cmdGPStest.Click
        Dim testCoords As Double()
        Dim dLat As Double
        Dim dLon As Double
        dLat = CDbl(txtLat.Text)
        dLon = CDbl(txtLon.Text)
        'Try
        '    testCoords = clsGPSConverter.WGS84LatLonToUTM(dLat, dLon)

        'Catch ex As Exception
        '    MsgBox("fout 1" & vbCrLf & ex.Message)
        'End Try
        'txtConversion.Text = ""
        'txtConversion.Text = "Itms: " & testCoords.Length.ToString & " = "
        'Try
        '    For Each dItem As Double In testCoords
        '        txtConversion.AppendText(" | " & dItem.ToString)
        '    Next
        'Catch ex As Exception
        '    MsgBox("fout 2" & vbCrLf & ex.Message)
        'End Try
    End Sub



    Private Sub cmdUpdate_Click(sender As Object, e As EventArgs) Handles cmdUpdate.Click
        checkForUpdate()
    End Sub
    Public Function checkForUpdate()
        Dim updCrypt As String = clsRegister.checkUpdates(sUserEmail, "versioncheck", sUserID)
        If updCrypt.Contains("error:") Then
            MsgBox("Fout bij het uitvoeren van de Versie Check!" & vbCrLf & updCrypt)
            Return False
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

                    Dim sNewVersion As String = aUserDet(0).SelectToken("version").ToString
                    sAppName = "RNLayoutManager-" & sNewVersion & ".exe"
                    sURL = aUserDet(0).SelectToken("update_url").ToString
                    Dim sReleaseNotes As String = aUserDet(0).SelectToken("releasenotes").ToString
                    Dim RNmsgBox As RN_CustomAlerts.frmUpdate = New RN_CustomAlerts.frmUpdate
                    RNmsgBox.UserEmail = sUserEmail
                    RNmsgBox.UserName = sUserName
                    RNmsgBox.RegDate = sRegDate
                    RNmsgBox.UpdateOmschrijving = "Software update V: " & sNewVersion & " beschikbaar"
                    RNmsgBox.ReleaseNotes = sReleaseNotes
                    Dim dlgRes As System.Windows.Forms.DialogResult = RNmsgBox.ShowDialog()
                    'bij cancel result exit sub
                    If dlgRes = System.Windows.Forms.DialogResult.Cancel Then
                        'update geannuleerd, wel programma starten
                        Return True
                    ElseIf dlgRes = System.Windows.Forms.DialogResult.Yes Then
                        'update hier
                        If clsRegister.createUpdateFile(sUserEmail, "update", sUserID) Then
                            'meesturen update url, appname en core dir
                            clsRegister.startUpdate(sURL, sAppName, sCoreDir)
                        End If
                    End If
                End If
            Else
                MsgBox("computer name komt niet overeen!")
            End If
        Catch ex As System.Exception
            MsgBox("Fout bij het weergeven van de update!" & vbCrLf & ex.Message)
            Return True
        End Try
        Return True
    End Function

    Private Sub chkUseDWGname_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseDWGname.CheckedChanged
        iniFile = New clsINI(sIniDir & sIniFile)
        bUseDWGname = chkUseDWGname.Checked
        iniFile.WriteBoolean("publishsettings", "usedwgname", bUseDWGname)
    End Sub

    Private Sub cmdSetTPLFolder_Click(sender As Object, e As EventArgs) Handles cmdSetTPLFolder.Click
        Dim fldrBrwsr As FolderBrowserDialog = New FolderBrowserDialog
        If fldrBrwsr.ShowDialog = DialogResult.OK Then
            sLayoutTemplateFolder = fldrBrwsr.SelectedPath & "\"
            iniFile = New clsINI(sIniDir & sIniFile)
            iniFile.WriteString("template", "templatefolder", sLayoutTemplateFolder)
            cmdSetTPLFolder.Text = "Template map: " & sLayoutTemplateFolder
        End If
    End Sub

    Private Sub Label8_Click(sender As Object, e As EventArgs) Handles Label8.Click, lblFilenamePreview.Click

    End Sub

    Private Sub chkUseBesteknummer_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseBesteknummer.CheckedChanged
        bUseBesteknr = chkUseBesteknummer.Checked
        setFileNameSettings()
    End Sub

    Private Sub chkUseBladnummer_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseBladnummer.CheckedChanged
        bUseBladnr = chkUseBladnummer.Checked
        setFileNameSettings()
    End Sub

    Private Sub chkUseVersienummer_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseVersienummer.CheckedChanged
        bUseVersie = chkUseVersienummer.Checked
        setFileNameSettings()
    End Sub

    Private Sub chkVersieVooraan_CheckedChanged(sender As Object, e As EventArgs) Handles chkVersieVooraan.CheckedChanged
        bVersieInFront = chkVersieVooraan.Checked
        setFileNameSettings()
    End Sub
End Class
