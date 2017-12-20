Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput




Public Class clsFilterData

    '    Public Sub Test()
    '        Dim dict As Dictionary(Of String, List) = New Dictionary(Of String, List)
    '        dict.Add("foo", New List(Of String)() {"a", "b", "c"})
    '        dict.Add("bar", New List(Of String)() {"x", "y", "z"})
    '        dict.Add("baz", New List(Of String)() {"this", "is", "a", "test"})
    '    Me.filterToDBDictionary(dict, "StringArraysDictionary")
    'End Sub

    Public Shared Sub filterToDBDictionary(ByRef acDoc As Document, ByRef acCurdDb As Database, ByRef acEd As Editor,
                                     ByVal dict As Dictionary(Of String, List(Of String)), ByVal dictName As String)
        'Dim doc As Document = AcAp.DocumentManager.MdiActiveDocument
        'Dim db As Database = doc.Database
        'Dim tr As Transaction = db.TransactionManager.StartTransaction
        Using acLockDoc As DocumentLock = acDoc.LockDocument
            Using acTrans As Transaction = acCurdDb.TransactionManager.StartTransaction()
                Dim nod As DBDictionary = CType(acTrans.GetObject(acCurdDb.NamedObjectsDictionaryId, OpenMode.ForRead), DBDictionary)
                Dim dbDict As DBDictionary
                Try
                    dbDict = CType(acTrans.GetObject(nod.GetAt(dictName), OpenMode.ForWrite), DBDictionary)
                    For Each entry As DBDictionaryEntry In dbDict
                        dbDict.Remove(entry.Key)
                    Next
                Catch ex As System.Exception
                    nod.UpgradeOpen()
                    dbDict = New DBDictionary
                    nod.SetAt(dictName, dbDict)
                    acTrans.AddNewlyCreatedDBObject(dbDict, True)
                End Try

                For Each entry As KeyValuePair(Of String, List(Of String)) In dict
                    Dim xrec As Xrecord = New Xrecord
                    Dim datas As ResultBuffer = New ResultBuffer
                    For Each s As String In entry.Value
                        datas.Add(New TypedValue(1, s))
                    Next
                    xrec.Data = datas
                    dbDict.SetAt(entry.Key, xrec)
                    acTrans.AddNewlyCreatedDBObject(xrec, True)
                Next
                acTrans.Commit()
            End Using
        End Using

    End Sub

    Public Shared Function getDBDictionaryToDictionary(ByRef acDoc As Document, ByRef acCurdDb As Database, ByRef acEd As Editor,
                                                 ByVal dictName As String) As Dictionary(Of String, List(Of String))
        '       Dim dict As Dictionary(Of String, List(Of String)) = DBDictionaryToDictionary("StringArraysDictionary")
        'If dict Is Nothing Then
        '        ed.WriteMessage(vbLf & "The 'StringArraysDictionary' dictionary can'tbe found.")
        '        Return
        '    End If

        '    For Each pair As KeyValuePair(Of String, List(Of String)) In dict
        '        Dim datas As String = String.Empty
        '        For Each s As String In pair.Value
        '            datas += " " & s
        '        Next

        '        ed.WriteMessage(vbLf & "{0} ={1}", pair.Key, datas)
        '    Next                                      
        Using acLockDoc As DocumentLock = acDoc.LockDocument
            Using acTrans As Transaction = acCurdDb.TransactionManager.StartTransaction()
                Dim nod As DBDictionary = CType(acTrans.GetObject(acCurdDb.NamedObjectsDictionaryId, OpenMode.ForRead), DBDictionary)
                Try
                    Dim dbDict As DBDictionary = CType(acTrans.GetObject(nod.GetAt(dictName), OpenMode.ForRead), DBDictionary)
                    Dim result As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))()
                    For Each entry As DBDictionaryEntry In dbDict
                        Dim xrec As Xrecord = TryCast(acTrans.GetObject(entry.Value, OpenMode.ForRead), Xrecord)
                        If xrec Is Nothing Then Continue For
                        Dim strs As List(Of String) = New List(Of String)()
                        Dim datas As ResultBuffer = xrec.Data
                        If datas Is Nothing Then Continue For
                        For Each tv As TypedValue In datas
                            If tv.TypeCode = 1 Then
                                strs.Add(CStr(tv.Value))
                            End If
                        Next

                        result.Add(entry.Key, strs)
                    Next

                    Return result
                Catch
                    Return Nothing
                End Try
            End Using
        End Using
    End Function
End Class

