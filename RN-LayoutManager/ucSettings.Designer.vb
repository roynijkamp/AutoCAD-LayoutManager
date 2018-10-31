<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucSettings
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
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
        Me.chkListboxTemplates = New System.Windows.Forms.CheckedListBox()
        Me.cmdBrowseLayoutTemplate = New System.Windows.Forms.Button()
        Me.editListTemplates = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.VerwijderenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.chkAutoLoad = New System.Windows.Forms.CheckBox()
        Me.cmdGPStest = New System.Windows.Forms.Button()
        Me.txtLat = New System.Windows.Forms.TextBox()
        Me.txtLon = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtConversion = New System.Windows.Forms.TextBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.cmdUpdate = New System.Windows.Forms.Button()
        Me.lblRegDate = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblUserEmail = New System.Windows.Forms.Label()
        Me.lblUserName = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.grpDebugOptions.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.editListTemplates.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
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
        Me.lblVersion.Location = New System.Drawing.Point(6, 795)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(280, 13)
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
        Me.cmbPlottingDevice.Location = New System.Drawing.Point(116, 14)
        Me.cmbPlottingDevice.Name = "cmbPlottingDevice"
        Me.cmbPlottingDevice.Size = New System.Drawing.Size(285, 21)
        Me.cmbPlottingDevice.TabIndex = 0
        '
        'grpDebugOptions
        '
        Me.grpDebugOptions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpDebugOptions.Controls.Add(Me.chkTrashDSD)
        Me.grpDebugOptions.Enabled = False
        Me.grpDebugOptions.Location = New System.Drawing.Point(3, 751)
        Me.grpDebugOptions.Name = "grpDebugOptions"
        Me.grpDebugOptions.Size = New System.Drawing.Size(415, 41)
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
        Me.cmdDebugOptions.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdDebugOptions.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdDebugOptions.Location = New System.Drawing.Point(3, 747)
        Me.cmdDebugOptions.Name = "cmdDebugOptions"
        Me.cmdDebugOptions.Size = New System.Drawing.Size(122, 19)
        Me.cmdDebugOptions.TabIndex = 1
        Me.cmdDebugOptions.Text = "Enable Debug Options"
        Me.cmdDebugOptions.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.chkListboxTemplates)
        Me.GroupBox3.Controls.Add(Me.cmdBrowseLayoutTemplate)
        Me.GroupBox3.Location = New System.Drawing.Point(3, 217)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(415, 200)
        Me.GroupBox3.TabIndex = 7
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Layout Template File"
        '
        'chkListboxTemplates
        '
        Me.chkListboxTemplates.FormattingEnabled = True
        Me.chkListboxTemplates.Location = New System.Drawing.Point(18, 48)
        Me.chkListboxTemplates.Name = "chkListboxTemplates"
        Me.chkListboxTemplates.Size = New System.Drawing.Size(383, 139)
        Me.chkListboxTemplates.TabIndex = 3
        '
        'cmdBrowseLayoutTemplate
        '
        Me.cmdBrowseLayoutTemplate.Location = New System.Drawing.Point(18, 22)
        Me.cmdBrowseLayoutTemplate.Name = "cmdBrowseLayoutTemplate"
        Me.cmdBrowseLayoutTemplate.Size = New System.Drawing.Size(379, 20)
        Me.cmdBrowseLayoutTemplate.TabIndex = 1
        Me.cmdBrowseLayoutTemplate.Text = "Template File toevoegen"
        Me.cmdBrowseLayoutTemplate.UseVisualStyleBackColor = True
        '
        'editListTemplates
        '
        Me.editListTemplates.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.VerwijderenToolStripMenuItem})
        Me.editListTemplates.Name = "editListTemplates"
        Me.editListTemplates.Size = New System.Drawing.Size(136, 26)
        '
        'VerwijderenToolStripMenuItem
        '
        Me.VerwijderenToolStripMenuItem.Name = "VerwijderenToolStripMenuItem"
        Me.VerwijderenToolStripMenuItem.Size = New System.Drawing.Size(135, 22)
        Me.VerwijderenToolStripMenuItem.Text = "Verwijderen"
        '
        'chkAutoLoad
        '
        Me.chkAutoLoad.AutoSize = True
        Me.chkAutoLoad.Checked = True
        Me.chkAutoLoad.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkAutoLoad.Location = New System.Drawing.Point(8, 423)
        Me.chkAutoLoad.Name = "chkAutoLoad"
        Me.chkAutoLoad.Size = New System.Drawing.Size(198, 17)
        Me.chkAutoLoad.TabIndex = 8
        Me.chkAutoLoad.Text = "Layout Manager automatisch starten"
        Me.chkAutoLoad.UseVisualStyleBackColor = True
        '
        'cmdGPStest
        '
        Me.cmdGPStest.Location = New System.Drawing.Point(3, 456)
        Me.cmdGPStest.Name = "cmdGPStest"
        Me.cmdGPStest.Size = New System.Drawing.Size(139, 36)
        Me.cmdGPStest.TabIndex = 9
        Me.cmdGPStest.Text = "Button1"
        Me.cmdGPStest.UseVisualStyleBackColor = True
        Me.cmdGPStest.Visible = False
        '
        'txtLat
        '
        Me.txtLat.Location = New System.Drawing.Point(234, 456)
        Me.txtLat.Name = "txtLat"
        Me.txtLat.Size = New System.Drawing.Size(100, 20)
        Me.txtLat.TabIndex = 10
        Me.txtLat.Visible = False
        '
        'txtLon
        '
        Me.txtLon.Location = New System.Drawing.Point(234, 486)
        Me.txtLon.Name = "txtLon"
        Me.txtLon.Size = New System.Drawing.Size(100, 20)
        Me.txtLon.TabIndex = 11
        Me.txtLon.Visible = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(176, 456)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(22, 13)
        Me.Label2.TabIndex = 12
        Me.Label2.Text = "Lat"
        Me.Label2.Visible = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(176, 486)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(25, 13)
        Me.Label3.TabIndex = 12
        Me.Label3.Text = "Lon"
        Me.Label3.Visible = False
        '
        'txtConversion
        '
        Me.txtConversion.Location = New System.Drawing.Point(160, 518)
        Me.txtConversion.Name = "txtConversion"
        Me.txtConversion.Size = New System.Drawing.Size(249, 20)
        Me.txtConversion.TabIndex = 13
        Me.txtConversion.Visible = False
        '
        'GroupBox4
        '
        Me.GroupBox4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox4.Controls.Add(Me.cmdUpdate)
        Me.GroupBox4.Controls.Add(Me.lblRegDate)
        Me.GroupBox4.Controls.Add(Me.Label4)
        Me.GroupBox4.Controls.Add(Me.lblUserEmail)
        Me.GroupBox4.Controls.Add(Me.lblUserName)
        Me.GroupBox4.Controls.Add(Me.Label5)
        Me.GroupBox4.Controls.Add(Me.Label6)
        Me.GroupBox4.Location = New System.Drawing.Point(3, 649)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(415, 90)
        Me.GroupBox4.TabIndex = 14
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Licentie Details:"
        '
        'cmdUpdate
        '
        Me.cmdUpdate.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.cmdUpdate.Location = New System.Drawing.Point(288, 63)
        Me.cmdUpdate.Name = "cmdUpdate"
        Me.cmdUpdate.Size = New System.Drawing.Size(117, 26)
        Me.cmdUpdate.TabIndex = 6
        Me.cmdUpdate.Text = "Check op Update"
        Me.cmdUpdate.UseVisualStyleBackColor = False
        '
        'lblRegDate
        '
        Me.lblRegDate.AutoSize = True
        Me.lblRegDate.Location = New System.Drawing.Point(104, 63)
        Me.lblRegDate.Name = "lblRegDate"
        Me.lblRegDate.Size = New System.Drawing.Size(29, 13)
        Me.lblRegDate.TabIndex = 5
        Me.lblRegDate.Text = "NaN"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 63)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(92, 13)
        Me.Label4.TabIndex = 4
        Me.Label4.Text = "Registratie datum:"
        '
        'lblUserEmail
        '
        Me.lblUserEmail.AutoSize = True
        Me.lblUserEmail.Location = New System.Drawing.Point(104, 44)
        Me.lblUserEmail.Name = "lblUserEmail"
        Me.lblUserEmail.Size = New System.Drawing.Size(29, 13)
        Me.lblUserEmail.TabIndex = 3
        Me.lblUserEmail.Text = "NaN"
        '
        'lblUserName
        '
        Me.lblUserName.AutoSize = True
        Me.lblUserName.Location = New System.Drawing.Point(104, 25)
        Me.lblUserName.Name = "lblUserName"
        Me.lblUserName.Size = New System.Drawing.Size(29, 13)
        Me.lblUserName.TabIndex = 2
        Me.lblUserName.Text = "NaN"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(6, 44)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(35, 13)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "Email:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(6, 25)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(56, 13)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "Gebruiker:"
        '
        'ucSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.cmdDebugOptions)
        Me.Controls.Add(Me.txtConversion)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtLon)
        Me.Controls.Add(Me.txtLat)
        Me.Controls.Add(Me.cmdGPStest)
        Me.Controls.Add(Me.chkAutoLoad)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.grpDebugOptions)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.lblVersion)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.lblTitel)
        Me.Controls.Add(Me.GroupBox4)
        Me.Name = "ucSettings"
        Me.Size = New System.Drawing.Size(421, 818)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.grpDebugOptions.ResumeLayout(False)
        Me.grpDebugOptions.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.editListTemplates.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
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
    Friend WithEvents editListTemplates As Windows.Forms.ContextMenuStrip
    Friend WithEvents VerwijderenToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkListboxTemplates As Windows.Forms.CheckedListBox
    Friend WithEvents chkAutoLoad As Windows.Forms.CheckBox
    Friend WithEvents cmdGPStest As Windows.Forms.Button
    Friend WithEvents txtLat As Windows.Forms.TextBox
    Friend WithEvents txtLon As Windows.Forms.TextBox
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents txtConversion As Windows.Forms.TextBox
    Friend WithEvents GroupBox4 As Windows.Forms.GroupBox
    Friend WithEvents lblRegDate As Windows.Forms.Label
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents lblUserEmail As Windows.Forms.Label
    Friend WithEvents lblUserName As Windows.Forms.Label
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents cmdUpdate As Windows.Forms.Button
End Class
