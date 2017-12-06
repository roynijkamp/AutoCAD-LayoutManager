Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput

Public Class clsNOD
    Public Shared Function writeToNOD(ByRef acDoc As Document, ByRef acCurdDb As Database, ByRef acEd As Editor)
        'Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        'Dim acCurDb As Database = acDoc.Database
        'Dim acEd As Editor = acDoc.Editor
        Try
            Using acLockDoc As DocumentLock = acDoc.LockDocument
                Using acTrans As Transaction = acCurdDb.TransactionManager.StartTransaction()
                    Dim myNOD As DBDictionary = acTrans.GetObject(acCurdDb.NamedObjectsDictionaryId, OpenMode.ForWrite)
                    Dim myXrecord As Xrecord = New Xrecord()

                End Using
            End Using
        Catch ex As Exception

        End Try
    End Function
End Class
