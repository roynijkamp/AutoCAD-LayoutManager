<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmFilter
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.cmbFilter = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmdSaveFilter = New System.Windows.Forms.Button()
        Me.cmdDelFilter = New System.Windows.Forms.Button()
        Me.radioVisibleItems = New System.Windows.Forms.RadioButton()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.cmdNieuw = New System.Windows.Forms.Button()
        Me.radioSaveAsSelection = New System.Windows.Forms.RadioButton()
        Me.SuspendLayout()
        '
        'cmbFilter
        '
        Me.cmbFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbFilter.FormattingEnabled = True
        Me.cmbFilter.Location = New System.Drawing.Point(47, 6)
        Me.cmbFilter.Name = "cmbFilter"
        Me.cmbFilter.Size = New System.Drawing.Size(346, 21)
        Me.cmbFilter.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(29, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Filter"
        '
        'cmdSaveFilter
        '
        Me.cmdSaveFilter.Location = New System.Drawing.Point(411, 58)
        Me.cmdSaveFilter.Name = "cmdSaveFilter"
        Me.cmdSaveFilter.Size = New System.Drawing.Size(73, 21)
        Me.cmdSaveFilter.TabIndex = 3
        Me.cmdSaveFilter.Text = "Opslaan"
        Me.cmdSaveFilter.UseVisualStyleBackColor = True
        '
        'cmdDelFilter
        '
        Me.cmdDelFilter.Location = New System.Drawing.Point(411, 5)
        Me.cmdDelFilter.Name = "cmdDelFilter"
        Me.cmdDelFilter.Size = New System.Drawing.Size(73, 21)
        Me.cmdDelFilter.TabIndex = 4
        Me.cmdDelFilter.Text = "Verwijderen"
        Me.cmdDelFilter.UseVisualStyleBackColor = True
        '
        'radioVisibleItems
        '
        Me.radioVisibleItems.AutoSize = True
        Me.radioVisibleItems.Enabled = False
        Me.radioVisibleItems.Location = New System.Drawing.Point(47, 39)
        Me.radioVisibleItems.Name = "radioVisibleItems"
        Me.radioVisibleItems.Size = New System.Drawing.Size(170, 17)
        Me.radioVisibleItems.TabIndex = 5
        Me.radioVisibleItems.TabStop = True
        Me.radioVisibleItems.Text = "Zichtbare items opslaan als lijst"
        Me.radioVisibleItems.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(25, 199)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TextBox1.Size = New System.Drawing.Size(435, 206)
        Me.TextBox1.TabIndex = 6
        '
        'cmdNieuw
        '
        Me.cmdNieuw.Location = New System.Drawing.Point(411, 35)
        Me.cmdNieuw.Name = "cmdNieuw"
        Me.cmdNieuw.Size = New System.Drawing.Size(73, 21)
        Me.cmdNieuw.TabIndex = 7
        Me.cmdNieuw.Text = "Nieuw"
        Me.cmdNieuw.UseVisualStyleBackColor = True
        '
        'radioSaveAsSelection
        '
        Me.radioSaveAsSelection.AutoSize = True
        Me.radioSaveAsSelection.Location = New System.Drawing.Point(47, 62)
        Me.radioSaveAsSelection.Name = "radioSaveAsSelection"
        Me.radioSaveAsSelection.Size = New System.Drawing.Size(216, 17)
        Me.radioSaveAsSelection.TabIndex = 8
        Me.radioSaveAsSelection.TabStop = True
        Me.radioSaveAsSelection.Text = "Geselecteerde items opslaan als selectie"
        Me.radioSaveAsSelection.UseVisualStyleBackColor = True
        '
        'frmFilter
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(490, 422)
        Me.Controls.Add(Me.radioSaveAsSelection)
        Me.Controls.Add(Me.cmdNieuw)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.radioVisibleItems)
        Me.Controls.Add(Me.cmdDelFilter)
        Me.Controls.Add(Me.cmdSaveFilter)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cmbFilter)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmFilter"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "RN LayoutManager - Filter Settings"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmbFilter As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cmdSaveFilter As System.Windows.Forms.Button
    Friend WithEvents cmdDelFilter As System.Windows.Forms.Button
    Friend WithEvents radioVisibleItems As System.Windows.Forms.RadioButton
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents cmdNieuw As System.Windows.Forms.Button
    Friend WithEvents radioSaveAsSelection As System.Windows.Forms.RadioButton
End Class
