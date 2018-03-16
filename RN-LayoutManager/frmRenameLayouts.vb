Public Class frmRenameLayouts
    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        'Me.Close()
    End Sub

    Private Sub cmdRename_Click(sender As Object, e As EventArgs) Handles cmdRename.Click
        Me.DialogResult = Windows.Forms.DialogResult.OK
        'Me.Close()
    End Sub

    Private Sub frmRenameLayouts_Load(sender As Object, e As EventArgs) Handles Me.Load
        'reset vars
        txtAutoNummer.Value = 0
        txtLayoutNaam.Text = ""
        radioLayoutNameExists.Checked = True
    End Sub

    Private Sub chkAutoNummer_CheckedChanged(sender As Object, e As EventArgs) Handles chkAutoNummer.CheckedChanged
        txtAutoNummer.Enabled = chkAutoNummer.Checked
    End Sub
End Class