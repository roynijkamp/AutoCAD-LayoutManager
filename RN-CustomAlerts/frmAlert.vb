Public Class frmAlert
    Shared bApplytoall As Boolean = False
    Shared sLabelTekst As String = "Wilt u de wijzigingen aan de geopende documenten opslaan?"
    Shared sWindowTitle As String = "RN Custom Alert Dialog"
    Public Property applytoall() As Boolean
        Get
            Return bApplytoall
        End Get
        Set(value As Boolean)
            bApplytoall = value
        End Set
    End Property

    Public Property LabelTekst As String
        Get
            Return sLabelTekst
        End Get
        Set(value As String)
            sLabelTekst = value
            lblTekst.Text = value
        End Set
    End Property

    Public Property WindowTitle As String
        Get
            Return sWindowTitle
        End Get
        Set(value As String)
            sWindowTitle = value
            Me.Text = value
        End Set
    End Property

    Private Sub Yes_Button_Click(sender As Object, e As EventArgs) Handles Yes_Button.Click
        bApplytoall = chk_aplytoall.Checked
        Me.DialogResult = Windows.Forms.DialogResult.Yes
    End Sub

    Private Sub No_Button_Click(sender As Object, e As EventArgs) Handles No_Button.Click
        bApplytoall = chk_aplytoall.Checked
        Me.DialogResult = Windows.Forms.DialogResult.No
    End Sub

    Private Sub Cancel_Button_Click(sender As Object, e As EventArgs) Handles Cancel_Button.Click
        bApplytoall = chk_aplytoall.Checked
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub chk_aplytoall_CheckedChanged(sender As Object, e As EventArgs) Handles chk_aplytoall.CheckedChanged
        bApplytoall = chk_aplytoall.Checked
    End Sub

    Private Sub frmAlert_Load(sender As Object, e As EventArgs) Handles Me.Load
        lblTekst.Text = sLabelTekst
        Me.Text = sWindowTitle
        chk_aplytoall.Checked = bApplytoall
    End Sub
End Class