<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DDContainer
    Inherits System.Windows.Forms.ContainerControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.PushPinTimer = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'PushPinTimer
        '
        Me.PushPinTimer.Interval = 1
        '
        'DDContainer
        '
        Me.Size = New System.Drawing.Size(300, 20)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PushPinTimer As System.Windows.Forms.Timer

End Class
