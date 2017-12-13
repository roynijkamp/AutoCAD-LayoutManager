Imports System.IO
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Windows.Data

Public Class clsBlockThumbnail
    Public Shared Function ExtractThumbnails(ByVal iconPath As String, ByVal tr As Transaction, ByVal bt As BlockTable, ByVal sBlockName As String) As Integer
        Dim numIcons As Integer = 0
        For Each btrId As ObjectId In bt
            Dim btr = CType(tr.GetObject(btrId, OpenMode.ForRead), BlockTableRecord)
            ' Ignore layouts and anonymous blocks
            'If (btr.IsLayout OrElse btr.IsAnonymous) Then
            'TODO: Warning!!! continue If
            'End If
            If btr.Name = sBlockName Then
                ' Attempt to generate an icon, where one doesn't exist
                ' Create the output directory, if it isn't yet there
                If Not Directory.Exists(iconPath) Then
                    Directory.CreateDirectory(iconPath)
                End If

                ' Save the icon to our out directory
                Dim imgsrc = CMLContentSearchPreviews.GetBlockTRThumbnail(btr)
                Dim bmp = ImageSourceToGDI(CType(imgsrc, System.Windows.Media.Imaging.BitmapSource))
                Dim fname = (iconPath + ("\" _
                            + (btr.Name + ".bmp")))
                If File.Exists(fname) Then
                    File.Delete(fname)
                End If

                bmp.Save(fname)

                'Dim image = New clsImageForm(CType(bmp, System.Drawing.Bitmap))
                'Image.ShowDialog

                ' Increment our icon counter
                numIcons = (numIcons + 1)
                ' leave for, just one block thumbnail needed
                Exit For
            End If
        Next
        Return numIcons
    End Function

    Public Shared Function ImageSourceToGDI(ByVal src As System.Windows.Media.Imaging.BitmapSource) As System.Drawing.Image
        Dim ms = New MemoryStream
        Dim encoder = New System.Windows.Media.Imaging.BmpBitmapEncoder
        encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(src))
        encoder.Save(ms)
        ms.Flush()
        Return System.Drawing.Image.FromStream(ms)
    End Function
End Class

