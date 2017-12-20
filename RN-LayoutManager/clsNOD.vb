Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput

Public Class clsNOD
    Public Shared Function writeFilterToNOD(ByRef acDoc As Document, ByRef acCurdDb As Database, ByRef acEd As Editor,
                                            ByVal sFilterName As String, ByVal sFilterList As String, ByVal sFilterType As String)
        'Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        'Dim acCurDb As Database = acDoc.Database
        'Dim acEd As Editor = acDoc.Editor
        Try
            Using acLockDoc As DocumentLock = acDoc.LockDocument
                Using acTrans As Transaction = acCurdDb.TransactionManager.StartTransaction()
                    Dim myNOD As DBDictionary = acTrans.GetObject(acCurdDb.NamedObjectsDictionaryId, OpenMode.ForWrite)
                    Dim myXrecord As Xrecord = New Xrecord()
                    myXrecord.Data = New ResultBuffer(New TypedValue(DxfCode.Text, sFilterList), New TypedValue(DxfCode.Text, sFilterType))
                    'data toevoegen aan NOD
                    myNOD.SetAt("RNoverlaymanager", myXrecord)
                    acTrans.AddNewlyCreatedDBObject(myXrecord, True)
                    acTrans.Commit()
                End Using
            End Using
        Catch ex As Exception

        End Try
    End Function
End Class
