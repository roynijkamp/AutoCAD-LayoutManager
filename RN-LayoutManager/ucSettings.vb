Imports System.IO
Imports System.Windows.Forms

Public Class ucSettings
    Private Sub radioPDFuserFolder_CheckedChanged(sender As Object, e As EventArgs) Handles radioPDFuserFolder.CheckedChanged
        If radioPDFuserFolder.Checked Then
            txtUserSaveFolder.Enabled = True
            cmdSelectUserSaveFolder.Enabled = True
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
                txtUserSaveFolder.Text = fldrDia.SelectedPath
            End If
        End Using
    End Sub
End Class
