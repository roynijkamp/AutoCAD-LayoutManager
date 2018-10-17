Public Class frmUpdate
    Shared sUserEmail As String
    Shared sComputername As String
    Shared sUserName As String
    Shared sRegDate As String
    Shared sUpdateOmschrijving As String
    Shared sReleaseNotes As String

    Public Property UserEmail As String
        Get
            Return sUserEmail
        End Get
        Set(value As String)
            sUserEmail = value
        End Set
    End Property

    Public Property Computername As String
        Get
            Return sComputername
        End Get
        Set(value As String)
            sComputername = value
        End Set
    End Property

    Public Property UserName As String
        Get
            Return sUserName
        End Get
        Set(value As String)
            sUserName = value
        End Set
    End Property

    Public Property RegDate As String
        Get
            Return sRegDate
        End Get
        Set(value As String)
            sRegDate = value
        End Set
    End Property

    Public Property UpdateOmschrijving As String
        Get
            Return sUpdateOmschrijving
        End Get
        Set(value As String)
            sUpdateOmschrijving = value
        End Set
    End Property

    Public Property ReleaseNotes As String
        Get
            Return sReleaseNotes
        End Get
        Set(value As String)
            sReleaseNotes = value
        End Set
    End Property

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub cmdUpdateNow_Click(sender As Object, e As EventArgs) Handles cmdUpdateNow.Click
        MsgBox("update wordt gestart")
        Me.DialogResult = Windows.Forms.DialogResult.Yes
    End Sub

    Private Sub frmUpdate_Load(sender As Object, e As EventArgs) Handles Me.Load
        webReleasenotes.DocumentText = sReleaseNotes
        webReleasenotes.Visible = True
        lblUserEmail.Text = sUserEmail
        lblUserName.Text = sUserName & " [" & sComputername & "]"
        lblRegDate.Text = sRegDate
        lblUpdate.Text = sUpdateOmschrijving
    End Sub
End Class