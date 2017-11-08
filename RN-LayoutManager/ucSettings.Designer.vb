<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucSettings
    Inherits System.Windows.Forms.UserControl

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
        Me.lblTitel = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.radioPDFfolderAsk = New System.Windows.Forms.RadioButton()
        Me.cmdSelectUserSaveFolder = New System.Windows.Forms.Button()
        Me.txtUserSaveFolder = New System.Windows.Forms.TextBox()
        Me.radioPDFuserFolder = New System.Windows.Forms.RadioButton()
        Me.radioPDFdrawingFolder = New System.Windows.Forms.RadioButton()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitel
        '
        Me.lblTitel.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblTitel.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblTitel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitel.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.lblTitel.Location = New System.Drawing.Point(0, 0)
        Me.lblTitel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.lblTitel.Name = "lblTitel"
        Me.lblTitel.Size = New System.Drawing.Size(421, 16)
        Me.lblTitel.TabIndex = 2
        Me.lblTitel.Text = "RN Layout Manager - Instellingen"
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.radioPDFfolderAsk)
        Me.GroupBox1.Controls.Add(Me.cmdSelectUserSaveFolder)
        Me.GroupBox1.Controls.Add(Me.txtUserSaveFolder)
        Me.GroupBox1.Controls.Add(Me.radioPDFuserFolder)
        Me.GroupBox1.Controls.Add(Me.radioPDFdrawingFolder)
        Me.GroupBox1.Location = New System.Drawing.Point(3, 19)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(415, 125)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Opslag locatie PDF bestanden"
        '
        'radioPDFfolderAsk
        '
        Me.radioPDFfolderAsk.AutoSize = True
        Me.radioPDFfolderAsk.Location = New System.Drawing.Point(6, 91)
        Me.radioPDFfolderAsk.Name = "radioPDFfolderAsk"
        Me.radioPDFfolderAsk.Size = New System.Drawing.Size(260, 17)
        Me.radioPDFfolderAsk.TabIndex = 4
        Me.radioPDFfolderAsk.Text = "Vraag mij om een opslag locatie tijdens het plotten"
        Me.radioPDFfolderAsk.UseVisualStyleBackColor = True
        '
        'cmdSelectUserSaveFolder
        '
        Me.cmdSelectUserSaveFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdSelectUserSaveFolder.Enabled = False
        Me.cmdSelectUserSaveFolder.Location = New System.Drawing.Point(367, 65)
        Me.cmdSelectUserSaveFolder.Name = "cmdSelectUserSaveFolder"
        Me.cmdSelectUserSaveFolder.Size = New System.Drawing.Size(30, 20)
        Me.cmdSelectUserSaveFolder.TabIndex = 3
        Me.cmdSelectUserSaveFolder.Text = "..."
        Me.cmdSelectUserSaveFolder.UseVisualStyleBackColor = True
        '
        'txtUserSaveFolder
        '
        Me.txtUserSaveFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtUserSaveFolder.Enabled = False
        Me.txtUserSaveFolder.Location = New System.Drawing.Point(23, 65)
        Me.txtUserSaveFolder.Name = "txtUserSaveFolder"
        Me.txtUserSaveFolder.Size = New System.Drawing.Size(348, 20)
        Me.txtUserSaveFolder.TabIndex = 2
        '
        'radioPDFuserFolder
        '
        Me.radioPDFuserFolder.AutoSize = True
        Me.radioPDFuserFolder.Location = New System.Drawing.Point(6, 42)
        Me.radioPDFuserFolder.Name = "radioPDFuserFolder"
        Me.radioPDFuserFolder.Size = New System.Drawing.Size(227, 17)
        Me.radioPDFuserFolder.TabIndex = 1
        Me.radioPDFuserFolder.Text = "PDF altijd opslaan op onderstaande locatie"
        Me.radioPDFuserFolder.UseVisualStyleBackColor = True
        '
        'radioPDFdrawingFolder
        '
        Me.radioPDFdrawingFolder.AutoSize = True
        Me.radioPDFdrawingFolder.Location = New System.Drawing.Point(6, 19)
        Me.radioPDFdrawingFolder.Name = "radioPDFdrawingFolder"
        Me.radioPDFdrawingFolder.Size = New System.Drawing.Size(286, 17)
        Me.radioPDFdrawingFolder.TabIndex = 0
        Me.radioPDFdrawingFolder.Text = "PDF opslaan op de locatie van de tekening (standaard)"
        Me.radioPDFdrawingFolder.UseVisualStyleBackColor = True
        '
        'ucSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.lblTitel)
        Me.Name = "ucSettings"
        Me.Size = New System.Drawing.Size(421, 818)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lblTitel As Windows.Forms.Label
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents radioPDFfolderAsk As Windows.Forms.RadioButton
    Friend WithEvents cmdSelectUserSaveFolder As Windows.Forms.Button
    Friend WithEvents txtUserSaveFolder As Windows.Forms.TextBox
    Friend WithEvents radioPDFuserFolder As Windows.Forms.RadioButton
    Friend WithEvents radioPDFdrawingFolder As Windows.Forms.RadioButton
End Class
