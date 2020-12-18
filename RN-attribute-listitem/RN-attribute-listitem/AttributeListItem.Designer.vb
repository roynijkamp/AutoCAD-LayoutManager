<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class AttributeListItem
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AttributeListItem))
        Me.lblAttName = New System.Windows.Forms.Label()
        Me.txtAttValue = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.chkAttribute = New System.Windows.Forms.CheckBox()
        Me.chkAutoNumber = New System.Windows.Forms.CheckBox()
        Me.txtStartValue = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtIncrement = New System.Windows.Forms.TextBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblAttName
        '
        Me.lblAttName.AutoSize = True
        Me.lblAttName.Location = New System.Drawing.Point(68, 11)
        Me.lblAttName.Name = "lblAttName"
        Me.lblAttName.Size = New System.Drawing.Size(58, 13)
        Me.lblAttName.TabIndex = 1
        Me.lblAttName.Text = "lblAttName"
        '
        'txtAttValue
        '
        Me.txtAttValue.Enabled = False
        Me.txtAttValue.Location = New System.Drawing.Point(62, 27)
        Me.txtAttValue.Name = "txtAttValue"
        Me.txtAttValue.Size = New System.Drawing.Size(310, 20)
        Me.txtAttValue.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(22, 31)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(37, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Value:"
        '
        'chkAttribute
        '
        Me.chkAttribute.AutoSize = True
        Me.chkAttribute.Location = New System.Drawing.Point(3, 9)
        Me.chkAttribute.Name = "chkAttribute"
        Me.chkAttribute.Size = New System.Drawing.Size(68, 17)
        Me.chkAttribute.TabIndex = 4
        Me.chkAttribute.Text = "Attribute:"
        Me.chkAttribute.UseVisualStyleBackColor = True
        '
        'chkAutoNumber
        '
        Me.chkAutoNumber.AutoSize = True
        Me.chkAutoNumber.Enabled = False
        Me.chkAutoNumber.Location = New System.Drawing.Point(5, 51)
        Me.chkAutoNumber.Name = "chkAutoNumber"
        Me.chkAutoNumber.Size = New System.Drawing.Size(90, 17)
        Me.chkAutoNumber.TabIndex = 5
        Me.chkAutoNumber.Text = "Auto Nummer"
        Me.chkAutoNumber.UseVisualStyleBackColor = True
        '
        'txtStartValue
        '
        Me.txtStartValue.Enabled = False
        Me.txtStartValue.Location = New System.Drawing.Point(174, 49)
        Me.txtStartValue.Name = "txtStartValue"
        Me.txtStartValue.Size = New System.Drawing.Size(43, 20)
        Me.txtStartValue.TabIndex = 6
        Me.txtStartValue.Text = "0"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(101, 52)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(70, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Start waarde:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(227, 52)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(74, 13)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Ophogen met:"
        '
        'txtIncrement
        '
        Me.txtIncrement.Enabled = False
        Me.txtIncrement.Location = New System.Drawing.Point(307, 49)
        Me.txtIncrement.Name = "txtIncrement"
        Me.txtIncrement.Size = New System.Drawing.Size(47, 20)
        Me.txtIncrement.TabIndex = 9
        Me.txtIncrement.Text = "1"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.PictureBox1.Image = Global.RN_attribute_listitem.My.Resources.Resources.info_icon
        Me.PictureBox1.Location = New System.Drawing.Point(360, 49)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(19, 20)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 10
        Me.PictureBox1.TabStop = False
        Me.ToolTip1.SetToolTip(Me.PictureBox1, resources.GetString("PictureBox1.ToolTip"))
        '
        'AttributeListItem
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.txtIncrement)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtStartValue)
        Me.Controls.Add(Me.chkAutoNumber)
        Me.Controls.Add(Me.chkAttribute)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtAttValue)
        Me.Controls.Add(Me.lblAttName)
        Me.Name = "AttributeListItem"
        Me.Size = New System.Drawing.Size(391, 84)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblAttName As System.Windows.Forms.Label
    Friend WithEvents txtAttValue As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents chkAttribute As System.Windows.Forms.CheckBox
    Friend WithEvents chkAutoNumber As System.Windows.Forms.CheckBox
    Friend WithEvents txtStartValue As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtIncrement As System.Windows.Forms.TextBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
End Class
