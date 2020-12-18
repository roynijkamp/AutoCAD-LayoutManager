<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAddUserSearchPath
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAddUserSearchPath))
        Me.lstUserPaths = New System.Windows.Forms.ListBox()
        Me.submenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BewerkenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.VerwijderenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ItemOmhoogPlaatsenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ItemOmlaagPlaatsenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.cmdSave = New System.Windows.Forms.Button()
        Me.cmdSelectPath = New System.Windows.Forms.Button()
        Me.txtSelectedPath = New System.Windows.Forms.TextBox()
        Me.cmdClose = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.submenu.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lstUserPaths
        '
        Me.lstUserPaths.ContextMenuStrip = Me.submenu
        Me.lstUserPaths.FormattingEnabled = True
        Me.lstUserPaths.Location = New System.Drawing.Point(12, 16)
        Me.lstUserPaths.Name = "lstUserPaths"
        Me.lstUserPaths.Size = New System.Drawing.Size(809, 290)
        Me.lstUserPaths.TabIndex = 0
        '
        'submenu
        '
        Me.submenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BewerkenToolStripMenuItem, Me.ToolStripMenuItem1, Me.VerwijderenToolStripMenuItem, Me.ItemOmhoogPlaatsenToolStripMenuItem, Me.ItemOmlaagPlaatsenToolStripMenuItem})
        Me.submenu.Name = "submenu"
        Me.submenu.Size = New System.Drawing.Size(195, 98)
        '
        'BewerkenToolStripMenuItem
        '
        Me.BewerkenToolStripMenuItem.Name = "BewerkenToolStripMenuItem"
        Me.BewerkenToolStripMenuItem.Size = New System.Drawing.Size(194, 22)
        Me.BewerkenToolStripMenuItem.Text = "Bewerken"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(191, 6)
        '
        'VerwijderenToolStripMenuItem
        '
        Me.VerwijderenToolStripMenuItem.Name = "VerwijderenToolStripMenuItem"
        Me.VerwijderenToolStripMenuItem.Size = New System.Drawing.Size(194, 22)
        Me.VerwijderenToolStripMenuItem.Text = "Verwijderen"
        '
        'ItemOmhoogPlaatsenToolStripMenuItem
        '
        Me.ItemOmhoogPlaatsenToolStripMenuItem.Name = "ItemOmhoogPlaatsenToolStripMenuItem"
        Me.ItemOmhoogPlaatsenToolStripMenuItem.Size = New System.Drawing.Size(194, 22)
        Me.ItemOmhoogPlaatsenToolStripMenuItem.Text = "Item omhoog plaatsen"
        '
        'ItemOmlaagPlaatsenToolStripMenuItem
        '
        Me.ItemOmlaagPlaatsenToolStripMenuItem.Name = "ItemOmlaagPlaatsenToolStripMenuItem"
        Me.ItemOmlaagPlaatsenToolStripMenuItem.Size = New System.Drawing.Size(194, 22)
        Me.ItemOmlaagPlaatsenToolStripMenuItem.Text = "Item omlaag plaatsen"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.cmdSave)
        Me.GroupBox1.Controls.Add(Me.cmdSelectPath)
        Me.GroupBox1.Controls.Add(Me.txtSelectedPath)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 312)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(804, 73)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Edit Search Path"
        '
        'cmdSave
        '
        Me.cmdSave.Location = New System.Drawing.Point(6, 45)
        Me.cmdSave.Name = "cmdSave"
        Me.cmdSave.Size = New System.Drawing.Size(131, 23)
        Me.cmdSave.TabIndex = 2
        Me.cmdSave.Text = "Pad opslaan"
        Me.cmdSave.UseVisualStyleBackColor = True
        '
        'cmdSelectPath
        '
        Me.cmdSelectPath.Location = New System.Drawing.Point(6, 19)
        Me.cmdSelectPath.Name = "cmdSelectPath"
        Me.cmdSelectPath.Size = New System.Drawing.Size(54, 24)
        Me.cmdSelectPath.TabIndex = 1
        Me.cmdSelectPath.Text = "Open"
        Me.cmdSelectPath.UseVisualStyleBackColor = True
        '
        'txtSelectedPath
        '
        Me.txtSelectedPath.Location = New System.Drawing.Point(66, 19)
        Me.txtSelectedPath.Name = "txtSelectedPath"
        Me.txtSelectedPath.Size = New System.Drawing.Size(732, 20)
        Me.txtSelectedPath.TabIndex = 0
        '
        'cmdClose
        '
        Me.cmdClose.Location = New System.Drawing.Point(675, 391)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(146, 32)
        Me.cmdClose.TabIndex = 2
        Me.cmdClose.Text = "Sluiten"
        Me.cmdClose.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(38, 394)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(159, 29)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "Read AutoCAD Paths"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(406, 397)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(159, 29)
        Me.Button2.TabIndex = 4
        Me.Button2.Text = "Save AutoCAD Paths"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'frmAddUserSearchPath
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(828, 435)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.cmdClose)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.lstUserPaths)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(844, 474)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(844, 474)
        Me.Name = "frmAddUserSearchPath"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Add Path To AutoCAD Options"
        Me.submenu.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lstUserPaths As Windows.Forms.ListBox
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents cmdClose As Windows.Forms.Button
    Friend WithEvents cmdSelectPath As Windows.Forms.Button
    Friend WithEvents txtSelectedPath As Windows.Forms.TextBox
    Friend WithEvents cmdSave As Windows.Forms.Button
    Friend WithEvents submenu As Windows.Forms.ContextMenuStrip
    Friend WithEvents BewerkenToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As Windows.Forms.ToolStripSeparator
    Friend WithEvents VerwijderenToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ItemOmhoogPlaatsenToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ItemOmlaagPlaatsenToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents Button1 As Windows.Forms.Button
    Friend WithEvents Button2 As Windows.Forms.Button
End Class
