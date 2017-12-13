Imports System.Drawing
Imports System.Windows.Forms

Public Class clsImageForm
    Inherits Form

    Public Sub New(ByVal ToShow As Bitmap)
        MyBase.New
        Me.Height = 800
        Me.Width = 800
        Dim pictureBox1 = New PictureBox
        Dim buttonOK = New Button
        Me.Controls.Add(pictureBox1)
        Me.Controls.Add(buttonOK)
        pictureBox1.BackgroundImageLayout = ImageLayout.Stretch
        pictureBox1.BackgroundImage = ToShow
    End Sub
End Class
