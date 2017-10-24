<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucLayoutManager
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.flowLayouts = New System.Windows.Forms.FlowLayoutPanel()
        Me.lblTitel = New System.Windows.Forms.Label()
        Me.cmdRefreshList = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.lblTitel, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.cmdRefreshList, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.flowLayouts, 0, 2)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(420, 956)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'flowLayouts
        '
        Me.flowLayouts.AutoScroll = True
        Me.flowLayouts.AutoSize = True
        Me.flowLayouts.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flowLayouts.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flowLayouts.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.flowLayouts.Location = New System.Drawing.Point(3, 63)
        Me.flowLayouts.Name = "flowLayouts"
        Me.flowLayouts.Size = New System.Drawing.Size(414, 850)
        Me.flowLayouts.TabIndex = 0
        Me.flowLayouts.WrapContents = False
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
        'cmdRefreshList
        '
        Me.cmdRefreshList.Location = New System.Drawing.Point(3, 919)
        Me.cmdRefreshList.Name = "cmdRefreshList"
        Me.cmdRefreshList.Size = New System.Drawing.Size(44, 34)
        Me.cmdRefreshList.TabIndex = 2
        Me.cmdRefreshList.Text = "REF"
        Me.cmdRefreshList.UseVisualStyleBackColor = True
        '
        'ucLayoutManager
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "ucLayoutManager"
        Me.Size = New System.Drawing.Size(420, 956)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents flowLayouts As Windows.Forms.FlowLayoutPanel
    Friend WithEvents lblTitel As Windows.Forms.Label
    Friend WithEvents cmdRefreshList As Windows.Forms.Button
End Class
