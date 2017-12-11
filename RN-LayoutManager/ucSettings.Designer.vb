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
        Me.lblVersion = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmbPlottingDevice = New System.Windows.Forms.ComboBox()
        Me.grpDebugOptions = New System.Windows.Forms.GroupBox()
        Me.chkTrashDSD = New System.Windows.Forms.CheckBox()
        Me.cmdDebugOptions = New System.Windows.Forms.Button()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.cmdBrowseLayoutTemplate = New System.Windows.Forms.Button()
        Me.txtLayoutTemplate = New System.Windows.Forms.TextBox()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.grpDebugOptions.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
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
        'lblVersion
        '
        Me.lblVersion.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblVersion.AutoSize = True
        Me.lblVersion.Location = New System.Drawing.Point(6, 795)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(36, 13)
        Me.lblVersion.TabIndex = 4
        Me.lblVersion.Text = "Versie"
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.cmbPlottingDevice)
        Me.GroupBox2.Location = New System.Drawing.Point(3, 150)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(415, 58)
        Me.GroupBox2.TabIndex = 5
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Standaard Plotter"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 17)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(91, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Standaard plotter:"
        '
        'cmbPlottingDevice
        '
        Me.cmbPlottingDevice.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmbPlottingDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPlottingDevice.FormattingEnabled = True
        Me.cmbPlottingDevice.Location = New System.Drawing.Point(97, 14)
        Me.cmbPlottingDevice.Name = "cmbPlottingDevice"
        Me.cmbPlottingDevice.Size = New System.Drawing.Size(304, 21)
        Me.cmbPlottingDevice.TabIndex = 0
        '
        'grpDebugOptions
        '
        Me.grpDebugOptions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpDebugOptions.Controls.Add(Me.chkTrashDSD)
        Me.grpDebugOptions.Enabled = False
        Me.grpDebugOptions.Location = New System.Drawing.Point(3, 692)
        Me.grpDebugOptions.Name = "grpDebugOptions"
        Me.grpDebugOptions.Size = New System.Drawing.Size(415, 100)
        Me.grpDebugOptions.TabIndex = 6
        Me.grpDebugOptions.TabStop = False
        '
        'chkTrashDSD
        '
        Me.chkTrashDSD.AutoSize = True
        Me.chkTrashDSD.Location = New System.Drawing.Point(5, 19)
        Me.chkTrashDSD.Name = "chkTrashDSD"
        Me.chkTrashDSD.Size = New System.Drawing.Size(126, 17)
        Me.chkTrashDSD.TabIndex = 0
        Me.chkTrashDSD.Text = "DSD File Verwijderen"
        Me.chkTrashDSD.UseVisualStyleBackColor = True
        '
        'cmdDebugOptions
        '
        Me.cmdDebugOptions.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdDebugOptions.Location = New System.Drawing.Point(9, 686)
        Me.cmdDebugOptions.Name = "cmdDebugOptions"
        Me.cmdDebugOptions.Size = New System.Drawing.Size(122, 19)
        Me.cmdDebugOptions.TabIndex = 1
        Me.cmdDebugOptions.Text = "Enable Debug Options"
        Me.cmdDebugOptions.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.cmdBrowseLayoutTemplate)
        Me.GroupBox3.Controls.Add(Me.txtLayoutTemplate)
        Me.GroupBox3.Location = New System.Drawing.Point(3, 217)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(415, 58)
        Me.GroupBox3.TabIndex = 7
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Layout Template File"
        '
        'cmdBrowseLayoutTemplate
        '
        Me.cmdBrowseLayoutTemplate.Location = New System.Drawing.Point(367, 22)
        Me.cmdBrowseLayoutTemplate.Name = "cmdBrowseLayoutTemplate"
        Me.cmdBrowseLayoutTemplate.Size = New System.Drawing.Size(30, 20)
        Me.cmdBrowseLayoutTemplate.TabIndex = 1
        Me.cmdBrowseLayoutTemplate.Text = "..."
        Me.cmdBrowseLayoutTemplate.UseVisualStyleBackColor = True
        '
        'txtLayoutTemplate
        '
        Me.txtLayoutTemplate.Location = New System.Drawing.Point(12, 22)
        Me.txtLayoutTemplate.Name = "txtLayoutTemplate"
        Me.txtLayoutTemplate.Size = New System.Drawing.Size(355, 20)
        Me.txtLayoutTemplate.TabIndex = 0
        '
        'ucSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.cmdDebugOptions)
        Me.Controls.Add(Me.grpDebugOptions)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.lblVersion)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.lblTitel)
        Me.Name = "ucSettings"
        Me.Size = New System.Drawing.Size(421, 818)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.grpDebugOptions.ResumeLayout(False)
        Me.grpDebugOptions.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitel As Windows.Forms.Label
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents radioPDFfolderAsk As Windows.Forms.RadioButton
    Friend WithEvents cmdSelectUserSaveFolder As Windows.Forms.Button
    Friend WithEvents txtUserSaveFolder As Windows.Forms.TextBox
    Friend WithEvents radioPDFuserFolder As Windows.Forms.RadioButton
    Friend WithEvents radioPDFdrawingFolder As Windows.Forms.RadioButton
    Friend WithEvents lblVersion As Windows.Forms.Label
    Friend WithEvents GroupBox2 As Windows.Forms.GroupBox
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents cmbPlottingDevice As Windows.Forms.ComboBox
    Friend WithEvents grpDebugOptions As Windows.Forms.GroupBox
    Friend WithEvents chkTrashDSD As Windows.Forms.CheckBox
    Friend WithEvents cmdDebugOptions As Windows.Forms.Button
    Friend WithEvents GroupBox3 As Windows.Forms.GroupBox
    Friend WithEvents cmdBrowseLayoutTemplate As Windows.Forms.Button
    Friend WithEvents txtLayoutTemplate As Windows.Forms.TextBox
End Class
