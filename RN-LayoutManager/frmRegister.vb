Imports System.Reflection

Public Class frmRegister
    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        Me.Close()

    End Sub

    Private Sub cmdRegister_Click(sender As Object, e As EventArgs) Handles cmdRegister.Click
        Dim sEmail As String = txtEmailadres.Text
        Dim sRequestType As String = "activation"
        Dim sUserID As String = ""

        If clsRegister.createLicense(sEmail, sRequestType, sUserID) = True Then
            'OK, scherm sluiten
            cmdRegister.DialogResult = System.Windows.Forms.DialogResult.OK
        Else
            cmdRegister.DialogResult = System.Windows.Forms.DialogResult.Abort
        End If
    End Sub
End Class