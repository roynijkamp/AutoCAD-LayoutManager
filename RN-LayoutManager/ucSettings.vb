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
    Dim sLayoutTemplate As String = ""
    Dim sLayoutTemplates As List(Of String)
    Dim sCurrVersion As String = Assembly.GetExecutingAssembly().GetName().Version.ToString
    Dim sDefaultPlottingDevice As String = "AutoCAD PDF (General Documentation).PC3"
    Dim bTrashDSD As Boolean = True
    Dim dtPlotPresets As System.Data.DataTable
    Dim bIsLoading As Boolean = False
    Dim bAutoLoad As Boolean = True
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
                'versie updaten
                iniFile.WriteString("appsettings", "version", sCurrVersion)
                sPDFuserFolder = iniFile.GetString("publishsettings", "outputfolder", sPDFuserFolder)
                txtUserSaveFolder.Text = sPDFuserFolder
                sLayoutTemplate = iniFile.GetString("template", "layout", sLayoutTemplate)
                'layout templates laden
                Dim temp As String = iniFile.GetString("template", "layouts", "")
                sLayoutTemplates = New List(Of String)(temp.Split(","c))
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
                bAutoLoad = iniFile.GetBoolean("appsettings", "autoload", bAutoLoad)
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
                Catch ex As System.Exception
                    MsgBox("Fout bij het lezen van de licentie!" & vbCrLf & ex.Message)
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
        Dim sSettingsFile As String = clsFunctions.getCoreDir() & sPlotPreferences
        clsFunctions.loadPlotPresets(sSettingsFile, dtPlotPresets, iSelectedIndex, sDefaultPlottingDevice)
        If Not dtPlotPresets.Rows.Count = 0 Then
            cmbPlottingDevice.DataSource = dtPlotPresets
            cmbPlottingDevice.DisplayMember = "preset"
            cmbPlottingDevice.ValueMember = "id"
            cmbPlottingDevice.SelectedIndex = iSelectedIndex
        Else
        End If
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
            If fldrDia.ShowDialog() = DialogResult.OK Then
                'eerst check of map wel bestaat
                If My.Computer.FileSystem.DirectoryExists(sIniDir) = False Then
                    My.Computer.FileSystem.CreateDirectory(sIniDir)
                End If
                'sLayoutTemplate = fldrDia.FileName
                If Not sLayoutTemplates.Contains(fldrDia.FileName) Then
                    sLayoutTemplates.Add(fldrDia.FileName)
                    chkListboxTemplates.Items.Add(fldrDia.FileName, False)
                End If

                'bestand aanmaken
                'settings schrijven
                iniFile = New clsINI(sIniDir & sIniFile)
                If sLayoutTemplates.Count > 0 Then
                    iniFile.WriteString("template", "layouts", String.Join(",", sLayoutTemplates.ToArray()))
                Else
                    iniFile.WriteString("template", "layouts", "")
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
        'If chkListboxTemplates.CheckedItems.Count = 0 Then
        '    If chkListboxTemplates.Items.Count > 0 Then
        '        'geen item geselecteerd, 1e item selecteren
        '        chkListboxTemplates.SetItemChecked(0, True)
        '        sLayoutTemplate = chkListboxTemplates.Items(0)
        '    End If
        'End If
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

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        'Dim msgBox As New frmCustomAlert
        'msgBox.WindowTitle = "Test Alerting"
        'msgBox.LabelTekst = "Test Label"

        'Dim dlgRes As DialogResult = msgBox.ShowDialog()

        'If dlgRes = DialogResult.Yes Then
        '    TextBox1.AppendText("YES Clicked" & vbCrLf)
        '    If msgBox.applytoall = True Then
        '        TextBox1.AppendText("APPLY TO ALL" & vbCrLf)
        '    End If
        'ElseIf dlgRes = DialogResult.No Then
        '    TextBox1.AppendText("NO Clicked" & vbCrLf)
        '    If msgBox.applytoall = True Then
        '        TextBox1.AppendText("APPLY TO ALL" & vbCrLf)
        '    End If
        'ElseIf dlgRes = DialogResult.Cancel Then
        '    TextBox1.AppendText("CANCEL Clicked" & vbCrLf)
        'End If
        'Dim RNmsgBox As RN_CustomAlerts.frmAlert = New RN_CustomAlerts.frmAlert
        'RNmsgBox.WindowTitle = "Wijzigingen opslaan?"
        'RNmsgBox.LabelTekst = "Wilt u de niet opgeslagen wijzigingen opslaan?"
        'Dim dlgRes As DialogResult = RNmsgBox.ShowDialog()

        'MsgBox(dlgRes.ToString & " -- Applyt to all" & RNmsgBox.applytoall.ToString)

    End Sub
End Class
