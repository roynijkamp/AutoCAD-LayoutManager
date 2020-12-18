Imports System.Text.RegularExpressions
Imports System.Windows.Forms

Public Class AttributeListItem
    Inherits System.Windows.Forms.UserControl

    Private sAttribName As String = "notset"
    Private sAttribNewValue As String = ""
    Private sAttribCurrValue As String = ""
    Private bIsItemSelected As Boolean = False
    Private dStartValue As Double = 0
    Private dIncrement As Double = 1
    Private bAutoNumber As Boolean = False

    Public Property AttribName() As String
        Get
            Return lblAttName.Text
        End Get
        Set(value As String)
            lblAttName.Text = value
        End Set
    End Property

    Public Property AttribNewValue() As String
        Get
            Return txtAttValue.Text
        End Get
        Set(value As String)
            txtAttValue.Text = value
        End Set
    End Property

    Public Property AttribCurrValue() As String
        Get
            Return txtAttValue.Text
        End Get
        Set(value As String)
            txtAttValue.Text = value
        End Set
    End Property

    Public Property IsItemSelected() As Boolean
        Get
            Return chkAttribute.Checked
        End Get
        Set(value As Boolean)
            chkAttribute.Checked = value
        End Set
    End Property

    Public Property StartValue() As Double
        Get
            Return CDbl(txtStartValue.Text)
        End Get
        Set(value As Double)
            txtStartValue.Text = CStr(value)
        End Set
    End Property

    Public Property IncrementValue() As Double
        Get
            Return CDbl(txtIncrement.Text)
        End Get
        Set(value As Double)
            txtIncrement.Text = CStr(value)
        End Set
    End Property

    Public Property AutoNumber() As Boolean
        Get
            Return chkAutoNumber.Checked
        End Get
        Set(value As Boolean)
            chkAutoNumber.Checked = value
        End Set
    End Property

    Public Event chkAttrib_ChekcedChanged(sender As Object, e As EventArgs)

    Private Sub chkAttribute_CheckedChanged(sender As Object, e As EventArgs) Handles chkAttribute.CheckedChanged
        bIsItemSelected = chkAttribute.Checked
        txtAttValue.Enabled = bIsItemSelected
        txtIncrement.Enabled = bIsItemSelected
        txtStartValue.Enabled = bIsItemSelected
        chkAutoNumber.Enabled = bIsItemSelected
        RaiseEvent chkAttrib_ChekcedChanged(Me, e)
    End Sub

    Public Event chkAutoNr_CheckedChanged(sender As Object, e As EventArgs)

    Private Sub chkAutoNumber_CheckedChanged(sender As Object, e As EventArgs) Handles chkAutoNumber.CheckedChanged
        bAutoNumber = chkAutoNumber.Checked
        txtIncrement.Enabled = bAutoNumber
        txtStartValue.Enabled = bAutoNumber
        RaiseEvent chkAutoNr_CheckedChanged(Me, e)
    End Sub

    Private Sub txtStartValue_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtStartValue.KeyPress
        If Not Char.IsNumber(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub txtIncrement_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtIncrement.KeyPress
        If Not Char.IsNumber(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub txtStartValue_TextChanged(sender As Object, e As EventArgs) Handles txtStartValue.TextChanged
        Dim digitsOnly As Regex = New Regex("[^\d]")
        txtStartValue.Text = digitsOnly.Replace(txtStartValue.Text, "")
    End Sub

    Private Sub txtIncrement_TextChanged(sender As Object, e As EventArgs) Handles txtIncrement.TextChanged
        Dim digitsOnly As Regex = New Regex("[^\d]")
        txtIncrement.Text = digitsOnly.Replace(txtIncrement.Text, "")
    End Sub
End Class
