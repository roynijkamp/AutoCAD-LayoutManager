<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAlert
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
        Me.lblTekst = New System.Windows.Forms.Label()
        Me.chk_aplytoall = New System.Windows.Forms.CheckBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Yes_Button = New System.Windows.Forms.Button()
        Me.No_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTekst
        '
        Me.lblTekst.AutoSize = True
        Me.lblTekst.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTekst.Location = New System.Drawing.Point(50, 20)
        Me.lblTekst.Name = "lblTekst"
        Me.lblTekst.Size = New System.Drawing.Size(374, 16)
        Me.lblTekst.TabIndex = 8
        Me.lblTekst.Text = "Wilt u de wijzigingen aan de geopende documenten opslaan?"
        '
        'chk_aplytoall
        '
        Me.chk_aplytoall.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chk_aplytoall.AutoSize = True
        Me.chk_aplytoall.Location = New System.Drawing.Point(178, 45)
        Me.chk_aplytoall.Name = "chk_aplytoall"
        Me.chk_aplytoall.Size = New System.Drawing.Size(225, 17)
        Me.chk_aplytoall.TabIndex = 7
        Me.chk_aplytoall.Text = "Mijn keuze toepassen op alle documenten"
        Me.chk_aplytoall.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.Controls.Add(Me.Yes_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.No_Button, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 2, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(178, 68)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(282, 29)
        Me.TableLayoutPanel1.TabIndex = 6
        '
        'Yes_Button
        '
        Me.Yes_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Yes_Button.Location = New System.Drawing.Point(13, 3)
        Me.Yes_Button.Name = "Yes_Button"
        Me.Yes_Button.Size = New System.Drawing.Size(67, 23)
        Me.Yes_Button.TabIndex = 0
        Me.Yes_Button.Text = "Ja"
        '
        'No_Button
        '
        Me.No_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.No_Button.Location = New System.Drawing.Point(107, 3)
        Me.No_Button.Name = "No_Button"
        Me.No_Button.Size = New System.Drawing.Size(67, 23)
        Me.No_Button.TabIndex = 0
        Me.No_Button.Text = "Nee"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(201, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Annuleren"
        '
        'frmAlert
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(472, 109)
        Me.Controls.Add(Me.lblTekst)
        Me.Controls.Add(Me.chk_aplytoall)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAlert"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmAlert"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTekst As Windows.Forms.Label
    Friend WithEvents chk_aplytoall As Windows.Forms.CheckBox
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Yes_Button As Windows.Forms.Button
    Friend WithEvents No_Button As Windows.Forms.Button
    Friend WithEvents Cancel_Button As Windows.Forms.Button
End Class
