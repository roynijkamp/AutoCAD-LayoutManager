<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucLayoutManager
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.lblTitel = New System.Windows.Forms.Label()
        Me.flowLayouts = New System.Windows.Forms.FlowLayoutPanel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.cmdTrash = New System.Windows.Forms.Button()
        Me.cmdInvertSelection = New System.Windows.Forms.Button()
        Me.cmdSelectAll = New System.Windows.Forms.Button()
        Me.cmdHideItems = New System.Windows.Forms.Button()
        Me.cmdShowItems = New System.Windows.Forms.Button()
        Me.cmdPlotSingleSheet = New System.Windows.Forms.Button()
        Me.cmdPlotMulitSheet = New System.Windows.Forms.Button()
        Me.cmdSortDESC = New System.Windows.Forms.Button()
        Me.cmdSortASC = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.cmdFilter = New System.Windows.Forms.Button()
        Me.pgbVoortgang = New System.Windows.Forms.ProgressBar()
        Me.cmdSaveOrder = New System.Windows.Forms.Button()
        Me.lblCheckCount = New System.Windows.Forms.Label()
        Me.cmdRefreshList = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SubMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.LayoutKopierenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.LayoutVerwijderenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tmrAutoScroll = New System.Windows.Forms.Timer(Me.components)
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SubMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.lblTitel, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.flowLayouts, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBox1, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBox2, 0, 3)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(420, 582)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'lblTitel
        '
        Me.lblTitel.AutoSize = True
        Me.lblTitel.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblTitel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTitel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitel.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.lblTitel.Location = New System.Drawing.Point(3, 3)
        Me.lblTitel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.lblTitel.Name = "lblTitel"
        Me.lblTitel.Size = New System.Drawing.Size(414, 17)
        Me.lblTitel.TabIndex = 1
        Me.lblTitel.Text = "RN Layout Manager"
        '
        'flowLayouts
        '
        Me.flowLayouts.AutoScroll = True
        Me.flowLayouts.AutoSize = True
        Me.flowLayouts.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flowLayouts.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flowLayouts.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.flowLayouts.Location = New System.Drawing.Point(3, 73)
        Me.flowLayouts.Name = "flowLayouts"
        Me.flowLayouts.Size = New System.Drawing.Size(414, 456)
        Me.flowLayouts.TabIndex = 0
        Me.flowLayouts.WrapContents = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.cmdTrash)
        Me.GroupBox1.Controls.Add(Me.cmdInvertSelection)
        Me.GroupBox1.Controls.Add(Me.cmdSelectAll)
        Me.GroupBox1.Controls.Add(Me.cmdHideItems)
        Me.GroupBox1.Controls.Add(Me.cmdShowItems)
        Me.GroupBox1.Controls.Add(Me.cmdPlotSingleSheet)
        Me.GroupBox1.Controls.Add(Me.cmdPlotMulitSheet)
        Me.GroupBox1.Controls.Add(Me.cmdSortDESC)
        Me.GroupBox1.Controls.Add(Me.cmdSortASC)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(0, 20)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox1.Size = New System.Drawing.Size(420, 50)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        '
        'cmdTrash
        '
        Me.cmdTrash.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_trash
        Me.cmdTrash.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdTrash.Location = New System.Drawing.Point(206, 11)
        Me.cmdTrash.Name = "cmdTrash"
        Me.cmdTrash.Size = New System.Drawing.Size(28, 28)
        Me.cmdTrash.TabIndex = 13
        Me.ToolTip1.SetToolTip(Me.cmdTrash, "Verwijder geselecteerde layouts")
        Me.cmdTrash.UseVisualStyleBackColor = True
        '
        'cmdInvertSelection
        '
        Me.cmdInvertSelection.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_selection_invert
        Me.cmdInvertSelection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdInvertSelection.Location = New System.Drawing.Point(95, 11)
        Me.cmdInvertSelection.Name = "cmdInvertSelection"
        Me.cmdInvertSelection.Size = New System.Drawing.Size(28, 28)
        Me.cmdInvertSelection.TabIndex = 12
        Me.ToolTip1.SetToolTip(Me.cmdInvertSelection, "Selectie omkeren")
        Me.cmdInvertSelection.UseVisualStyleBackColor = True
        '
        'cmdSelectAll
        '
        Me.cmdSelectAll.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_selection_all
        Me.cmdSelectAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdSelectAll.Location = New System.Drawing.Point(65, 11)
        Me.cmdSelectAll.Name = "cmdSelectAll"
        Me.cmdSelectAll.Size = New System.Drawing.Size(28, 28)
        Me.cmdSelectAll.TabIndex = 11
        Me.ToolTip1.SetToolTip(Me.cmdSelectAll, "Selecteer alle layouts")
        Me.cmdSelectAll.UseVisualStyleBackColor = True
        '
        'cmdHideItems
        '
        Me.cmdHideItems.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_light_off_2
        Me.cmdHideItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdHideItems.Location = New System.Drawing.Point(172, 11)
        Me.cmdHideItems.Name = "cmdHideItems"
        Me.cmdHideItems.Size = New System.Drawing.Size(28, 28)
        Me.cmdHideItems.TabIndex = 10
        Me.ToolTip1.SetToolTip(Me.cmdHideItems, "Geslecteerde items uit")
        Me.cmdHideItems.UseVisualStyleBackColor = True
        '
        'cmdShowItems
        '
        Me.cmdShowItems.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_light_on_2
        Me.cmdShowItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdShowItems.Location = New System.Drawing.Point(138, 11)
        Me.cmdShowItems.Name = "cmdShowItems"
        Me.cmdShowItems.Size = New System.Drawing.Size(28, 28)
        Me.cmdShowItems.TabIndex = 9
        Me.ToolTip1.SetToolTip(Me.cmdShowItems, "Alle items aan")
        Me.cmdShowItems.UseVisualStyleBackColor = True
        '
        'cmdPlotSingleSheet
        '
        Me.cmdPlotSingleSheet.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdPlotSingleSheet.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_PDF
        Me.cmdPlotSingleSheet.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdPlotSingleSheet.Location = New System.Drawing.Point(343, 11)
        Me.cmdPlotSingleSheet.Name = "cmdPlotSingleSheet"
        Me.cmdPlotSingleSheet.Size = New System.Drawing.Size(28, 28)
        Me.cmdPlotSingleSheet.TabIndex = 8
        Me.ToolTip1.SetToolTip(Me.cmdPlotSingleSheet, "Geslecteerde layouts afdrukken naar Singlesheet PDF")
        Me.cmdPlotSingleSheet.UseVisualStyleBackColor = True
        '
        'cmdPlotMulitSheet
        '
        Me.cmdPlotMulitSheet.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdPlotMulitSheet.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_PDF_multi
        Me.cmdPlotMulitSheet.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdPlotMulitSheet.Location = New System.Drawing.Point(381, 11)
        Me.cmdPlotMulitSheet.Name = "cmdPlotMulitSheet"
        Me.cmdPlotMulitSheet.Size = New System.Drawing.Size(28, 28)
        Me.cmdPlotMulitSheet.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.cmdPlotMulitSheet, "Geslecteerde layouts afdrukken naar Multisheet PDF")
        Me.cmdPlotMulitSheet.UseVisualStyleBackColor = True
        '
        'cmdSortDESC
        '
        Me.cmdSortDESC.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_sort_descending
        Me.cmdSortDESC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdSortDESC.Location = New System.Drawing.Point(34, 11)
        Me.cmdSortDESC.Name = "cmdSortDESC"
        Me.cmdSortDESC.Size = New System.Drawing.Size(28, 28)
        Me.cmdSortDESC.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.cmdSortDESC, "Sorteren van Z-A")
        Me.cmdSortDESC.UseVisualStyleBackColor = True
        '
        'cmdSortASC
        '
        Me.cmdSortASC.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_sort_ascending
        Me.cmdSortASC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdSortASC.Location = New System.Drawing.Point(6, 11)
        Me.cmdSortASC.Name = "cmdSortASC"
        Me.cmdSortASC.Size = New System.Drawing.Size(28, 28)
        Me.cmdSortASC.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.cmdSortASC, "Sorteren van A-Z")
        Me.cmdSortASC.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.cmdFilter)
        Me.GroupBox2.Controls.Add(Me.pgbVoortgang)
        Me.GroupBox2.Controls.Add(Me.cmdSaveOrder)
        Me.GroupBox2.Controls.Add(Me.lblCheckCount)
        Me.GroupBox2.Controls.Add(Me.cmdRefreshList)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox2.Location = New System.Drawing.Point(0, 532)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(0)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox2.Size = New System.Drawing.Size(420, 50)
        Me.GroupBox2.TabIndex = 4
        Me.GroupBox2.TabStop = False
        '
        'cmdFilter
        '
        Me.cmdFilter.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_filter
        Me.cmdFilter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdFilter.Location = New System.Drawing.Point(44, 16)
        Me.cmdFilter.Name = "cmdFilter"
        Me.cmdFilter.Size = New System.Drawing.Size(28, 28)
        Me.cmdFilter.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.cmdFilter, "Weergave filter instellen")
        Me.cmdFilter.UseVisualStyleBackColor = True
        '
        'pgbVoortgang
        '
        Me.pgbVoortgang.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pgbVoortgang.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.pgbVoortgang.Location = New System.Drawing.Point(381, 11)
        Me.pgbVoortgang.Name = "pgbVoortgang"
        Me.pgbVoortgang.Size = New System.Drawing.Size(36, 18)
        Me.pgbVoortgang.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.pgbVoortgang.TabIndex = 5
        Me.pgbVoortgang.Visible = False
        '
        'cmdSaveOrder
        '
        Me.cmdSaveOrder.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdSaveOrder.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_save
        Me.cmdSaveOrder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdSaveOrder.Location = New System.Drawing.Point(341, 16)
        Me.cmdSaveOrder.Name = "cmdSaveOrder"
        Me.cmdSaveOrder.Size = New System.Drawing.Size(28, 28)
        Me.cmdSaveOrder.TabIndex = 7
        Me.ToolTip1.SetToolTip(Me.cmdSaveOrder, "Layout volgorde opslaan in DWG")
        Me.cmdSaveOrder.UseVisualStyleBackColor = True
        '
        'lblCheckCount
        '
        Me.lblCheckCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblCheckCount.AutoSize = True
        Me.lblCheckCount.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCheckCount.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblCheckCount.Location = New System.Drawing.Point(389, 32)
        Me.lblCheckCount.Name = "lblCheckCount"
        Me.lblCheckCount.Size = New System.Drawing.Size(26, 13)
        Me.lblCheckCount.TabIndex = 4
        Me.lblCheckCount.Text = "# 0"
        Me.lblCheckCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cmdRefreshList
        '
        Me.cmdRefreshList.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_refresh
        Me.cmdRefreshList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdRefreshList.Location = New System.Drawing.Point(4, 16)
        Me.cmdRefreshList.Name = "cmdRefreshList"
        Me.cmdRefreshList.Size = New System.Drawing.Size(28, 28)
        Me.cmdRefreshList.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.cmdRefreshList, "Herlaad layout lijst")
        Me.cmdRefreshList.UseVisualStyleBackColor = True
        '
        'ToolTip1
        '
        Me.ToolTip1.IsBalloon = True
        '
        'SubMenu
        '
        Me.SubMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LayoutKopierenToolStripMenuItem, Me.ToolStripMenuItem1, Me.LayoutVerwijderenToolStripMenuItem})
        Me.SubMenu.Name = "SubMenu"
        Me.SubMenu.Size = New System.Drawing.Size(175, 54)
        '
        'LayoutKopierenToolStripMenuItem
        '
        Me.LayoutKopierenToolStripMenuItem.Name = "LayoutKopierenToolStripMenuItem"
        Me.LayoutKopierenToolStripMenuItem.Size = New System.Drawing.Size(174, 22)
        Me.LayoutKopierenToolStripMenuItem.Text = "Layout kopieren"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(171, 6)
        '
        'LayoutVerwijderenToolStripMenuItem
        '
        Me.LayoutVerwijderenToolStripMenuItem.Name = "LayoutVerwijderenToolStripMenuItem"
        Me.LayoutVerwijderenToolStripMenuItem.Size = New System.Drawing.Size(174, 22)
        Me.LayoutVerwijderenToolStripMenuItem.Text = "Layout verwijderen"
        '
        'tmrAutoScroll
        '
        Me.tmrAutoScroll.Interval = 200
        '
        'ucLayoutManager
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "ucLayoutManager"
        Me.Size = New System.Drawing.Size(420, 582)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.SubMenu.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents flowLayouts As Windows.Forms.FlowLayoutPanel
    Friend WithEvents lblTitel As Windows.Forms.Label
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents cmdSortDESC As Windows.Forms.Button
    Friend WithEvents cmdSortASC As Windows.Forms.Button
    Friend WithEvents cmdPlotMulitSheet As Windows.Forms.Button
    Friend WithEvents GroupBox2 As Windows.Forms.GroupBox
    Friend WithEvents cmdRefreshList As Windows.Forms.Button
    Friend WithEvents lblCheckCount As Windows.Forms.Label
    Friend WithEvents cmdSaveOrder As Windows.Forms.Button
    Friend WithEvents ToolTip1 As Windows.Forms.ToolTip
    Friend WithEvents pgbVoortgang As Windows.Forms.ProgressBar
    Friend WithEvents cmdPlotSingleSheet As Windows.Forms.Button
    Friend WithEvents cmdFilter As Windows.Forms.Button
    Friend WithEvents cmdShowItems As Windows.Forms.Button
    Friend WithEvents cmdHideItems As Windows.Forms.Button
    Friend WithEvents cmdSelectAll As Windows.Forms.Button
    Friend WithEvents cmdInvertSelection As Windows.Forms.Button
    Friend WithEvents cmdTrash As Windows.Forms.Button
    Friend WithEvents SubMenu As Windows.Forms.ContextMenuStrip
    Friend WithEvents LayoutKopierenToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As Windows.Forms.ToolStripSeparator
    Friend WithEvents LayoutVerwijderenToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents tmrAutoScroll As Windows.Forms.Timer
End Class
