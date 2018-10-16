<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRegister
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmRegister))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtEmailadres = New System.Windows.Forms.TextBox()
        Me.cmdRegister = New System.Windows.Forms.Button()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.Label1.Location = New System.Drawing.Point(12, 73)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(136, 25)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Emailadres:"
        '
        'txtEmailadres
        '
        Me.txtEmailadres.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEmailadres.Location = New System.Drawing.Point(154, 70)
        Me.txtEmailadres.Name = "txtEmailadres"
        Me.txtEmailadres.Size = New System.Drawing.Size(367, 31)
        Me.txtEmailadres.TabIndex = 1
        '
        'cmdRegister
        '
        Me.cmdRegister.Location = New System.Drawing.Point(418, 349)
        Me.cmdRegister.Name = "cmdRegister"
        Me.cmdRegister.Size = New System.Drawing.Size(111, 31)
        Me.cmdRegister.TabIndex = 2
        Me.cmdRegister.Text = "Registreer"
        Me.cmdRegister.UseVisualStyleBackColor = True
        '
        'cmdCancel
        '
        Me.cmdCancel.Location = New System.Drawing.Point(305, 349)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(111, 31)
        Me.cmdCancel.TabIndex = 3
        Me.cmdCancel.Text = "Annuleren"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'frmRegister
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.Clouds
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(660, 384)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdRegister)
        Me.Controls.Add(Me.txtEmailadres)
        Me.Controls.Add(Me.Label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmRegister"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "RN Open Data Manager - ©Roy Nijkamp "
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents txtEmailadres As Windows.Forms.TextBox
    Friend WithEvents cmdRegister As Windows.Forms.Button
    Friend WithEvents cmdCancel As Windows.Forms.Button
End Class
