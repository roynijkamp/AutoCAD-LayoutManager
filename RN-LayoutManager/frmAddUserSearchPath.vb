Imports System.IO
Imports System.Windows.Forms
Imports Autodesk.AutoCAD.Interop

Public Class frmAddUserSearchPath
    Dim sIniDir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\RNtools"
    Dim sIniFile As String = "\layoutmanager.ini"
    Dim iniFile As clsINI
    Dim sUserSearchPaths As String
    Dim lstUserSeachPaths As List(Of String) = New List(Of String)
    Dim bIsEdit As Boolean = False
    Dim sOldPath As String = ""
    Private Sub frmAddUserSearchPath_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        loadPaths()
    End Sub

    Private Function loadPaths()
        If File.Exists(sIniDir & sIniFile) Then
            lstUserPaths.Items.Clear()
            'bestand bestaat, instelingen laden
            iniFile = New clsINI(sIniDir & sIniFile)
            sUserSearchPaths = iniFile.GetString("autocadoptions", "support file search paths", "")
            If sUserSearchPaths.Length = 0 Then
                Return False
            End If
            lstUserSeachPaths = New List(Of String)(sUserSearchPaths.Split(";"))
            For Each sPath As String In lstUserSeachPaths
                lstUserPaths.Items.Add(sPath)
            Next
        End If
        Return True
    End Function

    Private Sub cmdSelectPath_Click(sender As Object, e As EventArgs) Handles cmdSelectPath.Click
        Dim dlgFolderBrowser As FolderBrowserDialog = New FolderBrowserDialog()
        If dlgFolderBrowser.ShowDialog = DialogResult.OK Then
            txtSelectedPath.Text = dlgFolderBrowser.SelectedPath
        End If
    End Sub

    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        saveItems()
    End Sub

    Public Sub saveItems()
        If Not bIsEdit Then
            lstUserSeachPaths.Add(txtSelectedPath.Text)
            lstUserPaths.Items.Add(txtSelectedPath.Text)
        Else
            'path edit
            lstUserSeachPaths = New List(Of String)
            For Each sPath As String In lstUserPaths.Items
                If sPath = sOldPath Then
                    lstUserSeachPaths.Add(sOldPath)
                Else
                    lstUserSeachPaths.Add(sPath)
                End If
            Next
        End If
        'save paths to ini
        iniFile = New clsINI(sIniDir & sIniFile)
        iniFile.WriteString("autocadoptions", "support file search paths", String.Join(";", lstUserSeachPaths))
        txtSelectedPath.Text = ""
        bIsEdit = False
    End Sub

    Private Sub lstUserPaths_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles lstUserPaths.MouseDoubleClick
        bIsEdit = True
        txtSelectedPath.Text = lstUserPaths.SelectedItem.ToString
        sOldPath = lstUserPaths.SelectedItem.ToString
    End Sub

    Private Sub BewerkenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BewerkenToolStripMenuItem.Click
        bIsEdit = True
        txtSelectedPath.Text = lstUserPaths.SelectedItem.ToString
        sOldPath = lstUserPaths.SelectedItem.ToString
    End Sub

    Private Sub cmdClose_Click(sender As Object, e As EventArgs) Handles cmdClose.Click
        Me.Close()
    End Sub

    Private Sub VerwijderenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VerwijderenToolStripMenuItem.Click
        If MsgBox("Weet u zeker dat u deze wilt verwijderen?", MsgBoxStyle.Question + MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            lstUserPaths.Items.Remove(lstUserPaths.SelectedItem)
            bIsEdit = True
            saveItems()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        UpdateProfile(False)

    End Sub

    Public Sub UpdateProfile(ByVal bWrite As Boolean)
        'Cast the current autocad application from .net to COM interop
        Dim oApp As Autodesk.AutoCAD.Interop.AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication
        'Get the user preferences on the current profile
        Dim profile As AcadPreferences = oApp.Preferences
        'Configure the default preferences
        Try

            Dim supportpaths() As String = Split(oApp.Preferences.Files.SupportPath, ";")
            Dim supportlist As New List(Of String)
            If bWrite = True Then
                For Each sPath As String In lstUserPaths.Items
                    supportlist.Add(sPath)
                Next
            End If
            For i As Integer = 0 To UBound(supportpaths)
                supportlist.Add(supportpaths(i))
            Next



            ''Format new support path string
            'Dim supportpath As String = Nothing
            'For Each item As String In supportlist
            '    supportpath = supportpath + item + ";"
            'Next
            If bWrite = True Then
                'Add new support
                oApp.Preferences.Files.SupportPath = String.Join(";", supportlist)
            Else
                MsgBox(String.Join("; ", supportpaths))
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        UpdateProfile(True)
    End Sub
End Class