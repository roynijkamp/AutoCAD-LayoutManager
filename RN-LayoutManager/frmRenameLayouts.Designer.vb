<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRenameLayouts
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtLayoutNaam = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.radioSkipRename = New System.Windows.Forms.RadioButton()
        Me.radioLayoutNameExists = New System.Windows.Forms.RadioButton()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.cmdRename = New System.Windows.Forms.Button()
        Me.chkAutoNummer = New System.Windows.Forms.CheckBox()
        Me.txtAutoNummer = New System.Windows.Forms.NumericUpDown()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        CType(Me.txtAutoNummer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(18, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(73, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Layout Naam:"
        '
        'txtLayoutNaam
        '
        Me.txtLayoutNaam.Location = New System.Drawing.Point(97, 5)
        Me.txtLayoutNaam.Name = "txtLayoutNaam"
        Me.txtLayoutNaam.Size = New System.Drawing.Size(404, 20)
        Me.txtLayoutNaam.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(19, 47)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(74, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Auto Nummer:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(182, 47)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(321, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Gebruik [#nummer] in de layoutnaam om automatisch te nummeren"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.radioSkipRename)
        Me.GroupBox1.Controls.Add(Me.radioLayoutNameExists)
        Me.GroupBox1.Location = New System.Drawing.Point(22, 70)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(317, 64)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Conflict oplossing"
        '
        'radioSkipRename
        '
        Me.radioSkipRename.AutoSize = True
        Me.radioSkipRename.Location = New System.Drawing.Point(17, 41)
        Me.radioSkipRename.Name = "radioSkipRename"
        Me.radioSkipRename.Size = New System.Drawing.Size(133, 17)
        Me.radioSkipRename.TabIndex = 1
        Me.radioSkipRename.Text = "Layout niet hernoemen"
        Me.radioSkipRename.UseVisualStyleBackColor = True
        '
        'radioLayoutNameExists
        '
        Me.radioLayoutNameExists.AutoSize = True
        Me.radioLayoutNameExists.Checked = True
        Me.radioLayoutNameExists.Location = New System.Drawing.Point(17, 18)
        Me.radioLayoutNameExists.Name = "radioLayoutNameExists"
        Me.radioLayoutNameExists.Size = New System.Drawing.Size(277, 17)
        Me.radioLayoutNameExists.TabIndex = 0
        Me.radioLayoutNameExists.TabStop = True
        Me.radioLayoutNameExists.Text = "Indien layoutnaam al bestaat, volgnummer toevoegen"
        Me.radioLayoutNameExists.UseVisualStyleBackColor = True
        '
        'cmdCancel
        '
        Me.cmdCancel.Location = New System.Drawing.Point(345, 111)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(75, 23)
        Me.cmdCancel.TabIndex = 6
        Me.cmdCancel.Text = "Annuleren"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'cmdRename
        '
        Me.cmdRename.Location = New System.Drawing.Point(426, 111)
        Me.cmdRename.Name = "cmdRename"
        Me.cmdRename.Size = New System.Drawing.Size(75, 23)
        Me.cmdRename.TabIndex = 7
        Me.cmdRename.Text = "Hernoem"
        Me.cmdRename.UseVisualStyleBackColor = True
        '
        'chkAutoNummer
        '
        Me.chkAutoNummer.AutoSize = True
        Me.chkAutoNummer.Location = New System.Drawing.Point(97, 48)
        Me.chkAutoNummer.Name = "chkAutoNummer"
        Me.chkAutoNummer.Size = New System.Drawing.Size(15, 14)
        Me.chkAutoNummer.TabIndex = 8
        Me.chkAutoNummer.UseVisualStyleBackColor = True
        '
        'txtAutoNummer
        '
        Me.txtAutoNummer.Location = New System.Drawing.Point(116, 44)
        Me.txtAutoNummer.Name = "txtAutoNummer"
        Me.txtAutoNummer.Size = New System.Drawing.Size(56, 20)
        Me.txtAutoNummer.TabIndex = 9
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Label4.Location = New System.Drawing.Point(94, 28)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(211, 13)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "Wildcards: * = bestaande naam overnemen"
        '
        'frmRenameLayouts
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(515, 144)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txtAutoNummer)
        Me.Controls.Add(Me.chkAutoNummer)
        Me.Controls.Add(Me.cmdRename)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtLayoutNaam)
        Me.Controls.Add(Me.Label1)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(531, 183)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(531, 183)
        Me.Name = "frmRenameLayouts"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Hernoem Layouts"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.txtAutoNummer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtLayoutNaam As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents radioSkipRename As System.Windows.Forms.RadioButton
    Friend WithEvents radioLayoutNameExists As System.Windows.Forms.RadioButton
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents cmdRename As System.Windows.Forms.Button
    Friend WithEvents chkAutoNummer As System.Windows.Forms.CheckBox
    Friend WithEvents txtAutoNummer As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label4 As System.Windows.Forms.Label
End Class
