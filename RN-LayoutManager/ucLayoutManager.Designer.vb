﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
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
        Me.cmdPrintDWF = New System.Windows.Forms.Button()
        Me.pcbIconFilter = New System.Windows.Forms.PictureBox()
        Me.txtFilter = New System.Windows.Forms.TextBox()
        Me.cmdBatchAttributes = New System.Windows.Forms.Button()
        Me.cmdTrash = New System.Windows.Forms.Button()
        Me.cmdInvertSelection = New System.Windows.Forms.Button()
        Me.cmdFilter = New System.Windows.Forms.Button()
        Me.cmdSelectAll = New System.Windows.Forms.Button()
        Me.cmdHideItems = New System.Windows.Forms.Button()
        Me.cmdShowItems = New System.Windows.Forms.Button()
        Me.cmdPlotSingleSheet = New System.Windows.Forms.Button()
        Me.cmdPlotMulitSheet = New System.Windows.Forms.Button()
        Me.cmdSortDESC = New System.Windows.Forms.Button()
        Me.cmdSortASC = New System.Windows.Forms.Button()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.cmdReplaceAttrib = New System.Windows.Forms.Button()
        Me.cmdAddLayout = New System.Windows.Forms.Button()
        Me.cmbNewLayout = New System.Windows.Forms.ComboBox()
        Me.ContextMenuTemplates = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.pgbVoortgang = New System.Windows.Forms.ProgressBar()
        Me.cmdSaveOrder = New System.Windows.Forms.Button()
        Me.lblCheckCount = New System.Windows.Forms.Label()
        Me.cmdRefreshList = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SubMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.PlotSinglesheetDWFToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LayoutKopierenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.LayoutVerwijderenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuItmRenameSelection = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.PlotstyleTableWijzigenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PlotterOverrideToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tmrAutoScroll = New System.Windows.Forms.Timer(Me.components)
        Me.ContextMenuFilters = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.GeselecteerdeItemsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FilterVanZichtbareItemsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.GeselecteerdFilterVerwijderenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuFilterList = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ContextMenuDWFoptions = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SinglesheetDWFToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.sPDFenMDWF = New System.Windows.Forms.ToolStripMenuItem()
        Me.mPDFenMDWF = New System.Windows.Forms.ToolStripMenuItem()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.pcbIconFilter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        Me.SubMenu.SuspendLayout()
        Me.ContextMenuFilters.SuspendLayout()
        Me.ContextMenuDWFoptions.SuspendLayout()
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
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
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
        Me.flowLayouts.Location = New System.Drawing.Point(3, 83)
        Me.flowLayouts.Name = "flowLayouts"
        Me.flowLayouts.Size = New System.Drawing.Size(414, 446)
        Me.flowLayouts.TabIndex = 0
        Me.flowLayouts.WrapContents = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.cmdPrintDWF)
        Me.GroupBox1.Controls.Add(Me.pcbIconFilter)
        Me.GroupBox1.Controls.Add(Me.txtFilter)
        Me.GroupBox1.Controls.Add(Me.cmdBatchAttributes)
        Me.GroupBox1.Controls.Add(Me.cmdTrash)
        Me.GroupBox1.Controls.Add(Me.cmdInvertSelection)
        Me.GroupBox1.Controls.Add(Me.cmdFilter)
        Me.GroupBox1.Controls.Add(Me.cmdSelectAll)
        Me.GroupBox1.Controls.Add(Me.cmdHideItems)
        Me.GroupBox1.Controls.Add(Me.cmdShowItems)
        Me.GroupBox1.Controls.Add(Me.cmdPlotSingleSheet)
        Me.GroupBox1.Controls.Add(Me.cmdPlotMulitSheet)
        Me.GroupBox1.Controls.Add(Me.cmdSortDESC)
        Me.GroupBox1.Controls.Add(Me.cmdSortASC)
        Me.GroupBox1.Controls.Add(Me.cmdCancel)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(0, 20)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox1.Size = New System.Drawing.Size(420, 60)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        '
        'cmdPrintDWF
        '
        Me.cmdPrintDWF.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdPrintDWF.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_DWF
        Me.cmdPrintDWF.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdPrintDWF.Location = New System.Drawing.Point(319, 30)
        Me.cmdPrintDWF.Name = "cmdPrintDWF"
        Me.cmdPrintDWF.Size = New System.Drawing.Size(28, 28)
        Me.cmdPrintDWF.TabIndex = 19
        Me.ToolTip1.SetToolTip(Me.cmdPrintDWF, "Geselecteerde layouts afdrukken naar DWF")
        Me.cmdPrintDWF.UseVisualStyleBackColor = True
        '
        'pcbIconFilter
        '
        Me.pcbIconFilter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pcbIconFilter.Location = New System.Drawing.Point(43, 5)
        Me.pcbIconFilter.Name = "pcbIconFilter"
        Me.pcbIconFilter.Size = New System.Drawing.Size(22, 20)
        Me.pcbIconFilter.TabIndex = 18
        Me.pcbIconFilter.TabStop = False
        '
        'txtFilter
        '
        Me.txtFilter.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.txtFilter.Location = New System.Drawing.Point(65, 5)
        Me.txtFilter.Name = "txtFilter"
        Me.txtFilter.ReadOnly = True
        Me.txtFilter.Size = New System.Drawing.Size(203, 20)
        Me.txtFilter.TabIndex = 17
        '
        'cmdBatchAttributes
        '
        Me.cmdBatchAttributes.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_attrib_eddit
        Me.cmdBatchAttributes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdBatchAttributes.Location = New System.Drawing.Point(240, 30)
        Me.cmdBatchAttributes.Name = "cmdBatchAttributes"
        Me.cmdBatchAttributes.Size = New System.Drawing.Size(28, 28)
        Me.cmdBatchAttributes.TabIndex = 14
        Me.ToolTip1.SetToolTip(Me.cmdBatchAttributes, "Hernoemen van Tekeninghoofd")
        Me.cmdBatchAttributes.UseVisualStyleBackColor = True
        '
        'cmdTrash
        '
        Me.cmdTrash.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_trash
        Me.cmdTrash.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdTrash.Location = New System.Drawing.Point(206, 30)
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
        Me.cmdInvertSelection.Location = New System.Drawing.Point(95, 30)
        Me.cmdInvertSelection.Name = "cmdInvertSelection"
        Me.cmdInvertSelection.Size = New System.Drawing.Size(28, 28)
        Me.cmdInvertSelection.TabIndex = 12
        Me.ToolTip1.SetToolTip(Me.cmdInvertSelection, "Selectie omkeren")
        Me.cmdInvertSelection.UseVisualStyleBackColor = True
        '
        'cmdFilter
        '
        Me.cmdFilter.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_filter
        Me.cmdFilter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdFilter.Location = New System.Drawing.Point(6, 0)
        Me.cmdFilter.Name = "cmdFilter"
        Me.cmdFilter.Size = New System.Drawing.Size(28, 28)
        Me.cmdFilter.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.cmdFilter, "Weergave filter instellen")
        Me.cmdFilter.UseVisualStyleBackColor = True
        '
        'cmdSelectAll
        '
        Me.cmdSelectAll.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_selection_all
        Me.cmdSelectAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdSelectAll.Location = New System.Drawing.Point(65, 30)
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
        Me.cmdHideItems.Location = New System.Drawing.Point(172, 30)
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
        Me.cmdShowItems.Location = New System.Drawing.Point(138, 30)
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
        Me.cmdPlotSingleSheet.Location = New System.Drawing.Point(350, 30)
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
        Me.cmdPlotMulitSheet.Location = New System.Drawing.Point(381, 30)
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
        Me.cmdSortDESC.Location = New System.Drawing.Point(34, 30)
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
        Me.cmdSortASC.Location = New System.Drawing.Point(6, 30)
        Me.cmdSortASC.Name = "cmdSortASC"
        Me.cmdSortASC.Size = New System.Drawing.Size(28, 28)
        Me.cmdSortASC.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.cmdSortASC, "Sorteren van A-Z")
        Me.cmdSortASC.UseVisualStyleBackColor = True
        '
        'cmdCancel
        '
        Me.cmdCancel.Location = New System.Drawing.Point(3, 30)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(230, 28)
        Me.cmdCancel.TabIndex = 16
        Me.cmdCancel.Text = "Annuleren"
        Me.cmdCancel.UseVisualStyleBackColor = True
        Me.cmdCancel.Visible = False
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.cmdReplaceAttrib)
        Me.GroupBox2.Controls.Add(Me.cmdAddLayout)
        Me.GroupBox2.Controls.Add(Me.cmbNewLayout)
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
        'cmdReplaceAttrib
        '
        Me.cmdReplaceAttrib.Location = New System.Drawing.Point(227, 19)
        Me.cmdReplaceAttrib.Name = "cmdReplaceAttrib"
        Me.cmdReplaceAttrib.Size = New System.Drawing.Size(41, 24)
        Me.cmdReplaceAttrib.TabIndex = 10
        Me.cmdReplaceAttrib.Text = "Geselecteerde attributes vervangen"
        Me.cmdReplaceAttrib.UseVisualStyleBackColor = True
        Me.cmdReplaceAttrib.Visible = False
        '
        'cmdAddLayout
        '
        Me.cmdAddLayout.BackgroundImage = Global.RN_LayoutManager.My.Resources.Resources.icon_add
        Me.cmdAddLayout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdAddLayout.Location = New System.Drawing.Point(149, 17)
        Me.cmdAddLayout.Name = "cmdAddLayout"
        Me.cmdAddLayout.Size = New System.Drawing.Size(28, 28)
        Me.cmdAddLayout.TabIndex = 9
        Me.ToolTip1.SetToolTip(Me.cmdAddLayout, "Geselecteerde Layout toevoegen")
        Me.cmdAddLayout.UseVisualStyleBackColor = True
        '
        'cmbNewLayout
        '
        Me.cmbNewLayout.ContextMenuStrip = Me.ContextMenuTemplates
        Me.cmbNewLayout.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbNewLayout.FormattingEnabled = True
        Me.cmbNewLayout.Location = New System.Drawing.Point(38, 21)
        Me.cmbNewLayout.Name = "cmbNewLayout"
        Me.cmbNewLayout.Size = New System.Drawing.Size(107, 21)
        Me.cmbNewLayout.TabIndex = 8
        '
        'ContextMenuTemplates
        '
        Me.ContextMenuTemplates.Name = "ContextMenuTemplates"
        Me.ContextMenuTemplates.Size = New System.Drawing.Size(61, 4)
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
        Me.cmdSaveOrder.Visible = False
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
        Me.SubMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PlotSinglesheetDWFToolStripMenuItem, Me.LayoutKopierenToolStripMenuItem, Me.ToolStripMenuItem1, Me.LayoutVerwijderenToolStripMenuItem, Me.ToolStripMenuItem2, Me.mnuItmRenameSelection, Me.ToolStripMenuItem4, Me.PlotstyleTableWijzigenToolStripMenuItem, Me.PlotterOverrideToolStripMenuItem})
        Me.SubMenu.Name = "SubMenu"
        Me.SubMenu.Size = New System.Drawing.Size(195, 176)
        '
        'PlotSinglesheetDWFToolStripMenuItem
        '
        Me.PlotSinglesheetDWFToolStripMenuItem.Image = Global.RN_LayoutManager.My.Resources.Resources.icon_DWF
        Me.PlotSinglesheetDWFToolStripMenuItem.Name = "PlotSinglesheetDWFToolStripMenuItem"
        Me.PlotSinglesheetDWFToolStripMenuItem.Size = New System.Drawing.Size(194, 22)
        Me.PlotSinglesheetDWFToolStripMenuItem.Text = "Plot Singlesheet DWF"
        '
        'LayoutKopierenToolStripMenuItem
        '
        Me.LayoutKopierenToolStripMenuItem.Image = Global.RN_LayoutManager.My.Resources.Resources.icon_copy
        Me.LayoutKopierenToolStripMenuItem.Name = "LayoutKopierenToolStripMenuItem"
        Me.LayoutKopierenToolStripMenuItem.Size = New System.Drawing.Size(194, 22)
        Me.LayoutKopierenToolStripMenuItem.Text = "Layout kopieren"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(191, 6)
        '
        'LayoutVerwijderenToolStripMenuItem
        '
        Me.LayoutVerwijderenToolStripMenuItem.Image = Global.RN_LayoutManager.My.Resources.Resources.icon_trash
        Me.LayoutVerwijderenToolStripMenuItem.Name = "LayoutVerwijderenToolStripMenuItem"
        Me.LayoutVerwijderenToolStripMenuItem.Size = New System.Drawing.Size(194, 22)
        Me.LayoutVerwijderenToolStripMenuItem.Text = "Layout verwijderen"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(191, 6)
        '
        'mnuItmRenameSelection
        '
        Me.mnuItmRenameSelection.Image = Global.RN_LayoutManager.My.Resources.Resources.icon_attrib_eddit
        Me.mnuItmRenameSelection.Name = "mnuItmRenameSelection"
        Me.mnuItmRenameSelection.Size = New System.Drawing.Size(194, 22)
        Me.mnuItmRenameSelection.Text = "Selectie Hernoemen"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(191, 6)
        '
        'PlotstyleTableWijzigenToolStripMenuItem
        '
        Me.PlotstyleTableWijzigenToolStripMenuItem.Image = Global.RN_LayoutManager.My.Resources.Resources.icon_penstyle
        Me.PlotstyleTableWijzigenToolStripMenuItem.Name = "PlotstyleTableWijzigenToolStripMenuItem"
        Me.PlotstyleTableWijzigenToolStripMenuItem.Size = New System.Drawing.Size(194, 22)
        Me.PlotstyleTableWijzigenToolStripMenuItem.Text = "Plotstyle table wijzigen"
        '
        'PlotterOverrideToolStripMenuItem
        '
        Me.PlotterOverrideToolStripMenuItem.Image = Global.RN_LayoutManager.My.Resources.Resources.icon_print
        Me.PlotterOverrideToolStripMenuItem.Name = "PlotterOverrideToolStripMenuItem"
        Me.PlotterOverrideToolStripMenuItem.Size = New System.Drawing.Size(194, 22)
        Me.PlotterOverrideToolStripMenuItem.Text = "Plotter Override"
        '
        'tmrAutoScroll
        '
        Me.tmrAutoScroll.Interval = 200
        '
        'ContextMenuFilters
        '
        Me.ContextMenuFilters.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GeselecteerdeItemsToolStripMenuItem, Me.FilterVanZichtbareItemsToolStripMenuItem, Me.ToolStripMenuItem3, Me.GeselecteerdFilterVerwijderenToolStripMenuItem})
        Me.ContextMenuFilters.Name = "ContextMenuStrip1"
        Me.ContextMenuFilters.Size = New System.Drawing.Size(233, 76)
        '
        'GeselecteerdeItemsToolStripMenuItem
        '
        Me.GeselecteerdeItemsToolStripMenuItem.Image = Global.RN_LayoutManager.My.Resources.Resources.icon_check
        Me.GeselecteerdeItemsToolStripMenuItem.Name = "GeselecteerdeItemsToolStripMenuItem"
        Me.GeselecteerdeItemsToolStripMenuItem.Size = New System.Drawing.Size(232, 22)
        Me.GeselecteerdeItemsToolStripMenuItem.Text = "Filter van Geselecteerde items"
        '
        'FilterVanZichtbareItemsToolStripMenuItem
        '
        Me.FilterVanZichtbareItemsToolStripMenuItem.Image = Global.RN_LayoutManager.My.Resources.Resources.icon_light_on_2
        Me.FilterVanZichtbareItemsToolStripMenuItem.Name = "FilterVanZichtbareItemsToolStripMenuItem"
        Me.FilterVanZichtbareItemsToolStripMenuItem.Size = New System.Drawing.Size(232, 22)
        Me.FilterVanZichtbareItemsToolStripMenuItem.Text = "Filter van Zichtbare items"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(229, 6)
        '
        'GeselecteerdFilterVerwijderenToolStripMenuItem
        '
        Me.GeselecteerdFilterVerwijderenToolStripMenuItem.Image = Global.RN_LayoutManager.My.Resources.Resources.icon_trash
        Me.GeselecteerdFilterVerwijderenToolStripMenuItem.Name = "GeselecteerdFilterVerwijderenToolStripMenuItem"
        Me.GeselecteerdFilterVerwijderenToolStripMenuItem.Size = New System.Drawing.Size(232, 22)
        Me.GeselecteerdFilterVerwijderenToolStripMenuItem.Text = "Geselecteerd filter verwijderen"
        '
        'ContextMenuFilterList
        '
        Me.ContextMenuFilterList.Name = "ContextMenuFilterList"
        Me.ContextMenuFilterList.Size = New System.Drawing.Size(61, 4)
        '
        'ContextMenuDWFoptions
        '
        Me.ContextMenuDWFoptions.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SinglesheetDWFToolStripMenuItem, Me.sPDFenMDWF, Me.mPDFenMDWF})
        Me.ContextMenuDWFoptions.Name = "ContextMenuStrip1"
        Me.ContextMenuDWFoptions.Size = New System.Drawing.Size(257, 70)
        '
        'SinglesheetDWFToolStripMenuItem
        '
        Me.SinglesheetDWFToolStripMenuItem.Name = "SinglesheetDWFToolStripMenuItem"
        Me.SinglesheetDWFToolStripMenuItem.Size = New System.Drawing.Size(256, 22)
        Me.SinglesheetDWFToolStripMenuItem.Text = "Singlesheet DWF"
        '
        'sPDFenMDWF
        '
        Me.sPDFenMDWF.Name = "sPDFenMDWF"
        Me.sPDFenMDWF.Size = New System.Drawing.Size(256, 22)
        Me.sPDFenMDWF.Text = "Singlesheet PDF + Multisheet DWF"
        '
        'mPDFenMDWF
        '
        Me.mPDFenMDWF.Name = "mPDFenMDWF"
        Me.mPDFenMDWF.Size = New System.Drawing.Size(256, 22)
        Me.mPDFenMDWF.Text = "Multisheet PDF + Multisheet DWF"
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
        Me.GroupBox1.PerformLayout()
        CType(Me.pcbIconFilter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.SubMenu.ResumeLayout(False)
        Me.ContextMenuFilters.ResumeLayout(False)
        Me.ContextMenuDWFoptions.ResumeLayout(False)
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
    Friend WithEvents cmbNewLayout As Windows.Forms.ComboBox
    Friend WithEvents cmdAddLayout As Windows.Forms.Button
    Friend WithEvents cmdBatchAttributes As Windows.Forms.Button
    Friend WithEvents cmdReplaceAttrib As Windows.Forms.Button
    Friend WithEvents ToolStripMenuItem2 As Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuItmRenameSelection As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuFilters As Windows.Forms.ContextMenuStrip
    Friend WithEvents GeselecteerdeItemsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents FilterVanZichtbareItemsToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As Windows.Forms.ToolStripSeparator
    Friend WithEvents GeselecteerdFilterVerwijderenToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents cmdCancel As Windows.Forms.Button
    Friend WithEvents ContextMenuFilterList As Windows.Forms.ContextMenuStrip
    Friend WithEvents pcbIconFilter As Windows.Forms.PictureBox
    Friend WithEvents txtFilter As Windows.Forms.TextBox
    Friend WithEvents ContextMenuTemplates As Windows.Forms.ContextMenuStrip
    Friend WithEvents cmdPrintDWF As Windows.Forms.Button
    Friend WithEvents ContextMenuDWFoptions As Windows.Forms.ContextMenuStrip
    Friend WithEvents sPDFenMDWF As Windows.Forms.ToolStripMenuItem
    Friend WithEvents mPDFenMDWF As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As Windows.Forms.ToolStripSeparator
    Friend WithEvents PlotstyleTableWijzigenToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SinglesheetDWFToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents PlotSinglesheetDWFToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents PlotterOverrideToolStripMenuItem As Windows.Forms.ToolStripMenuItem
End Class
