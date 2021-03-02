Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput

Public Class clsLayout
    Shared sData As Dictionary(Of String, String)

    Public Shared Function getLayoutPrintNames(ByVal layouts As List(Of Layout)) As Dictionary(Of String, Dictionary(Of String, String))
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

        Dim dictLay As New Dictionary(Of String, Dictionary(Of String, String))

        Using acLockDoc As DocumentLock = acDoc.LockDocument
            Dim acLayoutMgr As LayoutManager = LayoutManager.Current
            For Each sLay As Layout In layouts
                acLayoutMgr.CurrentLayout = sLay.LayoutName
                sData = New Dictionary(Of String, String)
                getAttributesInBlock(getSpaceID("paper"), "SAL-TITELBLOK")

                dictLay.Add(sLay.LayoutName, sData)
            Next
        End Using
        Return dictLay
    End Function

    Public Shared Function getSpaceID(ByVal sSpace As String) As ObjectId
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
        Using acLockDoc As DocumentLock = acDoc.LockDocument()
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                Dim bt As BlockTable = DirectCast(acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead), BlockTable)
                'modal en paperspace ids
                Select Case sSpace
                    Case "modal"
                        Return bt(BlockTableRecord.ModelSpace)
                    Case "paper"
                        Return bt(BlockTableRecord.PaperSpace)
                End Select
            End Using 'transaction
        End Using 'lock dock
        Dim oEmpty As ObjectId = Nothing
        Return oEmpty
    End Function

    Public Shared Function getAttributesInBlock(ByVal oBtrId As ObjectId, ByVal sBlockName As String)

        Dim sTmp As String
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

        Using acLockDoc As DocumentLock = acDoc.LockDocument()
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                Dim btr As BlockTableRecord = TryCast(acTrans.GetObject(oBtrId, OpenMode.ForRead), BlockTableRecord)
                For Each oEntId As ObjectId In btr
                    Dim ent As Entity = TryCast(acTrans.GetObject(oEntId, OpenMode.ForRead), Entity)
                    If ent IsNot Nothing Then
                        Dim br As BlockReference = TryCast(ent, BlockReference)
                        If br IsNot Nothing Then
                            Dim sCurrName As String = ""
                            If br.IsDynamicBlock Then
                                Dim tmp As BlockTableRecord = br.DynamicBlockTableRecord.GetObject(OpenMode.ForRead)
                                sCurrName = tmp.Name
                            Else
                                Dim bd As BlockTableRecord = DirectCast(acTrans.GetObject(br.BlockTableRecord, OpenMode.ForRead), BlockTableRecord)
                                sCurrName = bd.Name
                            End If

                            'If bd.Name.ToUpper.Contains(sBlockName.ToUpper) Then
                            If sCurrName.ToUpper.Contains(sBlockName.ToUpper) Then
                                'dit block uitpluizen om bestandsnaam te genereren
                                For Each arId As ObjectId In br.AttributeCollection
                                    Dim obj As DBObject = acTrans.GetObject(arId, OpenMode.ForRead)
                                    Dim ar As AttributeReference = TryCast(obj, AttributeReference)
                                    If ar IsNot Nothing Then

                                        'ar.TextString = sTmpValue
                                        sData.Add(ar.Tag.ToUpper, ar.TextString)
                                    End If
                                Next 'for each attribute
                                Exit For
                            End If
                            'recurse for nested blocks
                            getAttributesInBlock(br.BlockTableRecord, sBlockName)
                        End If
                    End If
                Next 'for each object
                acTrans.Commit()
            End Using 'transaction
        End Using 'lockdock
        Return sData
    End Function
End Class
