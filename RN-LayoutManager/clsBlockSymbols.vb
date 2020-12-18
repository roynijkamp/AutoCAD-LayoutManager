Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports System.IO
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Internal
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports System.Drawing
Imports Autodesk.AutoCAD.Windows.Data

Public Class clsBlockSymbols
    Public Shared Function InsertBlockFromFile(dwgFileName As String, db As Database, currentSpace As BlockTableRecord, insPoint3D As Point3d, sLayer As String, Optional annotative As Boolean = False) As ObjectId
        Dim result As ObjectId = ObjectId.Null
        Using database As Database = New Database(False, True)
            database.ReadDwgFile(dwgFileName, FileShare.Read, True, "")
            Dim mdiActiveDocument As Document = Application.DocumentManager.MdiActiveDocument
            Using transaction As Transaction = mdiActiveDocument.TransactionManager.StartTransaction()
                Dim blockNameFromInsertPathName As String = SymbolUtilityServices.GetBlockNameFromInsertPathName(dwgFileName)
                Dim objectId As ObjectId = db.Insert(blockNameFromInsertPathName, database, True)
                If objectId.IsNull() Then
                    Throw New Exception("Failed to insert block")
                End If
                If database.AnnotativeDwg() Or annotative = True Then
                    Using blockTableRecord As BlockTableRecord = CType(transaction.GetObject(objectId, 1), BlockTableRecord)
                        blockTableRecord.Annotative = AnnotativeStates.True
                    End Using
                End If
                Dim blockTableRecord2 As BlockTableRecord = CType(transaction.GetObject(objectId, 0), BlockTableRecord)
                Dim blockReference As BlockReference = New BlockReference(insPoint3D, objectId)
                result = currentSpace.AppendEntity(blockReference)
                blockReference.SetDatabaseDefaults()

                Dim arg_DC_0 As BlockReference = blockReference
                Dim scaleFactors As Scale3d = New Scale3d(1.0, 1.0, 1.0)
                arg_DC_0.ScaleFactors = scaleFactors
                Dim attributeCollection As AttributeCollection = blockReference.AttributeCollection()
                Try
                    Dim enumerator As BlockTableRecordEnumerator = blockTableRecord2.GetEnumerator()
                    While enumerator.MoveNext()
                        Dim current As ObjectId = enumerator.Current()
                        Dim entity As Entity = CType(current.GetObject(1), Entity)
                        If TypeOf entity Is AttributeDefinition Then
                            Dim attributeDefinition As AttributeDefinition = CType(entity, AttributeDefinition)
                            Dim attributeReference As AttributeReference = New AttributeReference()
                            attributeReference.SetAttributeFromBlock(attributeDefinition, blockReference.BlockTransform())
                            attributeCollection.AppendAttribute(attributeReference)
                            transaction.AddNewlyCreatedDBObject(attributeReference, True)
                        End If
                    End While
                Finally
                    'Dim enumerator As BlockTableRecordEnumerator
                    'If enumerator IsNot Nothing Then
                    'enumerator.Dispose()
                    'End If
                End Try
                If database.AnnotativeDwg() Or annotative = True Then
                    Dim objectContextManager As ObjectContextManager = db.ObjectContextManager()
                    Dim contextCollection As ObjectContextCollection = objectContextManager.GetContextCollection("ACDB_ANNOTATIONSCALES")
                    ObjectContexts.AddContext(blockReference, contextCollection.CurrentContext())
                End If
                blockReference.Layer = sLayer
                transaction.AddNewlyCreatedDBObject(blockReference, True)
                transaction.Commit()
            End Using
        End Using
        Return result
    End Function

    Public Shared Function InsertBlockFromFile(dwgFileName As String, db As Database, currentSpace As BlockTableRecord, Optional annotative As Boolean = False) As Boolean
        Using database As Database = New Database(False, True)
            database.ReadDwgFile(dwgFileName, FileShare.Read, True, "")
            Dim mdiActiveDocument As Document = Application.DocumentManager.MdiActiveDocument
            Using transaction As Transaction = mdiActiveDocument.TransactionManager.StartTransaction()
                Dim blockNameFromInsertPathName As String = SymbolUtilityServices.GetBlockNameFromInsertPathName(dwgFileName)
                Dim objectId As ObjectId = db.Insert(blockNameFromInsertPathName, database, True)
                If objectId.IsNull() Then
                    Throw New Exception("Failed to insert block")
                    InsertBlockFromFile = False
                End If
                If database.AnnotativeDwg() Or annotative = True Then
                    Using blockTableRecord As BlockTableRecord = CType(transaction.GetObject(objectId, 1), BlockTableRecord)
                        blockTableRecord.Annotative = AnnotativeStates.True
                    End Using
                End If
                transaction.Commit()
            End Using
        End Using
        InsertBlockFromFile = True
    End Function

    Public Shared Function addBlockToDB(dwgFileName As String, sBlockName As String, ByRef db As Database, ByRef transaction As Transaction, Optional bIsAnnotative As Boolean = False) As String
        Try
            Using dbSource As Database = New Database(False, True)
                dbSource.ReadDwgFile(dwgFileName, FileShare.Read, True, "")
                Dim mdiActiveDocument As Document = Application.DocumentManager.MdiActiveDocument
                Dim bIds As ObjectIdCollection = New ObjectIdCollection()
                Dim acTransMan As Autodesk.AutoCAD.DatabaseServices.TransactionManager = dbSource.TransactionManager
                Using acTrans As Transaction = acTransMan.StartTransaction()
                    Dim bt As BlockTable = acTrans.GetObject(dbSource.BlockTableId, OpenMode.ForRead, False)
                    If bt.Has(sBlockName) Then
                        bIds.Add(bt(sBlockName))
                    End If
                    acTrans.Commit()
                    If bIds.Count > 0 Then
                        'copy block from source to destination
                        Dim mapping As IdMapping = New IdMapping()
                        dbSource.WblockCloneObjects(bIds, db.BlockTableId, mapping, DuplicateRecordCloning.Replace, False)
                        dbSource.Dispose()
                        addBlockToDB = sBlockName

                    Else
                        dbSource.Dispose()
                        addBlockToDB = "ERROR"
                    End If

                End Using
            End Using
        Catch ex As System.Exception
            MsgBox("Fout bij het kopieren van het Block!" & vbCrLf & ex.Message)
            addBlockToDB = "ERROR"
        End Try
    End Function

    Public Shared Function updateAttribTekst(oEntId As ObjectId, acTrans As Transaction, dCurrHmp As Double, sWegnummer As String, Optional dRotation As Double = 0)
        Dim ent As Entity = TryCast(acTrans.GetObject(oEntId, OpenMode.ForRead), Entity)
        Dim sValue As String = ""
        If ent IsNot Nothing Then
            Dim br As BlockReference = TryCast(ent, BlockReference)
            If br IsNot Nothing Then
                'kijken of we de rotatie moten wijzigen
                'If dRotation > 0 Then
                '    br.Rotation = dRotation
                'End If
                Dim btr As BlockTableRecord = DirectCast(acTrans.GetObject(br.BlockTableRecord, OpenMode.ForRead), BlockTableRecord)
                ' Check each of the attributes...
                For Each arId As ObjectId In br.AttributeCollection
                    Dim obj As DBObject = acTrans.GetObject(arId, OpenMode.ForRead)
                    Dim ar As AttributeReference = TryCast(obj, AttributeReference)
                    If ar IsNot Nothing Then
                        'kijken of we de tag in de array kunnen vinden
                        Select Case ar.Tag.ToUpper
                            Case "KILOMETRERING"
                                ' updaten
                                sValue = CStr(Math.Round(CDec(dCurrHmp), 3))
                                Exit Select

                            Case "WEGNUMMER"
                                sValue = sWegnummer
                                Exit Select
                        End Select

                        ar.UpgradeOpen()
                        ar.TextString = sValue
                        'ar.Rotation = dRotation
                        ar.AdjustAlignment(br.Database)
                        ar.DowngradeOpen()
                    End If
                Next 'for each attribute

            End If
        End If
        Return True
    End Function

    ''' <summary>
    ''' 'Function voor updaten van block attributes
    ''' </summary>
    ''' <param name="oEntId"></param>
    ''' <param name="acTrans"></param>
    ''' <param name="sTagName"></param>
    ''' <param name="sTagValue"></param>
    ''' <returns></returns>

    Public Shared Function updateAttribTekst(oEntId As ObjectId, acTrans As Transaction, sTagName As String, sTagValue As String)
        Dim ent As Entity = TryCast(acTrans.GetObject(oEntId, OpenMode.ForRead), Entity)
        Dim sValue As String = ""
        If ent IsNot Nothing Then
            Dim br As BlockReference = TryCast(ent, BlockReference)
            If br IsNot Nothing Then
                Dim bd As BlockTableRecord = DirectCast(acTrans.GetObject(br.BlockTableRecord, OpenMode.ForRead), BlockTableRecord)
                ' Check each of the attributes...
                For Each arId As ObjectId In br.AttributeCollection
                    Dim obj As DBObject = acTrans.GetObject(arId, OpenMode.ForRead)
                    Dim ar As AttributeReference = TryCast(obj, AttributeReference)
                    If ar IsNot Nothing Then
                        'kijken of we de tag in de array kunnen vinden
                        If ar.Tag.ToUpper = sTagName.ToUpper Then
                            sValue = sTagValue
                            ar.UpgradeOpen()
                            ar.TextString = sValue
                            ar.DowngradeOpen()
                            Return True
                        End If
                    End If
                Next 'for each attribute
            End If
        End If
        Return True
    End Function

    Public Shared Function ApplyAttibutes(ByRef acLockDoc As DocumentLock, ByRef db As Database, ByRef tr As Transaction, ByVal bref As BlockReference, ByVal listTags As List(Of String), ByVal listValues As List(Of String))
        Try
            Using acLockDoc
                Dim btr As BlockTableRecord = DirectCast(tr.GetObject(bref.BlockTableRecord, OpenMode.ForRead), BlockTableRecord)

                For Each attId As ObjectId In btr
                    Dim ent As Entity = DirectCast(tr.GetObject(attId, OpenMode.ForRead), Entity)
                    If TypeOf ent Is AttributeDefinition Then
                        Dim attDef As AttributeDefinition = DirectCast(ent, AttributeDefinition)
                        Dim attRef As New AttributeReference()

                        attRef.SetAttributeFromBlock(attDef, bref.BlockTransform)
                        bref.AttributeCollection.AppendAttribute(attRef)
                        tr.AddNewlyCreatedDBObject(attRef, True)
                        If listTags.Contains(attDef.Tag) Then
                            Dim found As Integer = listTags.BinarySearch(attDef.Tag)
                            If found >= 0 Then
                                attRef.TextString = listValues(found)
                                attRef.AdjustAlignment(db)
                            End If
                        End If

                    End If
                Next
            End Using
        Catch ex As Autodesk.AutoCAD.Runtime.Exception
            MsgBox("Fout bij het kopieren van het Block!" & vbCrLf & ex.Message)

        End Try
    End Function

    Public Shared Function updateBlkAttrib(ucs As Matrix3d, tr As Transaction, br As BlockReference, atts As Dictionary(Of ObjectId, ObjectId))
        Dim _ucs As Matrix3d
        Dim _pos As Point3d
        Dim _atts As Dictionary(Of ObjectId, ObjectId)
        Dim _tr As Transaction

        _ucs = ucs
        _pos = br.Position
        _atts = atts
        _tr = tr

        'attributes updaten
        If br.AttributeCollection.Count > 0 Then
            For Each id As ObjectId In br.AttributeCollection
                Dim obj = _tr.GetObject(id, OpenMode.ForRead)
                Dim ar = TryCast(obj, AttributeReference)

                If ar IsNot Nothing Then
                    ar.UpgradeOpen()

                    ' Open the associated attribute definition

                    Dim defId = _atts(ar.ObjectId)
                    Dim obj2 = _tr.GetObject(defId, OpenMode.ForRead)
                    Dim ad = DirectCast(obj2, AttributeDefinition)

                    ' Use it to set positional information on the
                    ' reference

                    ar.SetAttributeFromBlock(ad, br.BlockTransform)
                    ar.AdjustAlignment(br.Database)
                End If
            Next
        End If
        Return True
    End Function

    Public Shared Function CopyBlockDefAndAssign(ByVal oBtrId As ObjectId, ByVal sBlockToCopy As String, ByVal sBlockNewName As String)
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
                        Dim acBlkTbl As BlockTable = DirectCast(acCurDb.BlockTableId.GetObject(OpenMode.ForWrite), BlockTable)
                        If br IsNot Nothing Then
                            Dim bd As BlockTableRecord = DirectCast(acTrans.GetObject(br.BlockTableRecord, OpenMode.ForRead), BlockTableRecord)
                            If bd.Name.ToUpper = sBlockToCopy.ToUpper Then
                                'deze moet gekopieerd en omgezet worden
                                Dim newBtr As New BlockTableRecord()
                                acBlkTbl.Add(newBtr)
                                newBtr.Name = sBlockNewName
                                newBtr.Origin = bd.Origin
                                newBtr.BlockScaling = BlockScaling.Uniform
                                newBtr.Explodable = True
                                acTrans.AddNewlyCreatedDBObject(newBtr, True)

                                Dim oIdCol As New ObjectIdCollection
                                oIdCol.Add(bd.ObjectId)

                                Dim mapId As New IdMapping

                                acCurDb.DeepCloneObjects(oIdCol, newBtr.ObjectId, mapId, False)
                                'Dim BlkRef As BlockReference = acTrans.GetObject(bd.ObjectId, OpenMode.ForWrite)
                                'Dim zMap As New IdMapping

                                'Dim newzBlkTblRec As BlockTableRecord = acCurDb.DeepCloneObjects()
                                acTrans.Commit()
                                Return True
                            End If

                        End If
                    End If

                Next 'for each object
                acTrans.Commit()
            End Using 'transaction
        End Using 'lockdock
        Return False

    End Function


    Public Shared Function CopyBlockDefAndAssign(ByVal sBlockToCopy As String, ByVal sBlockNewName As String, ByVal iIncrement As Integer)

        Dim zDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim zDb As Database = zDoc.Database
        Dim zEd As Editor = zDoc.Editor


        Using mTrans As Transaction = zDb.TransactionManager.StartTransaction()
            Try
                Dim mBlkTbl As BlockTable = mTrans.GetObject(zDb.BlockTableId, OpenMode.ForWrite)
                Dim zBlkTblRec As BlockTableRecord = mTrans.GetObject(mBlkTbl("test"), OpenMode.ForWrite)

                Dim i As Integer

                For Each zBlkTableRecId As ObjectId In zBlkTblRec.GetBlockReferenceIds(True, False)

                    Dim BlkRef As BlockReference = mTrans.GetObject(zBlkTableRecId, OpenMode.ForWrite)
                    Dim zMap As New IdMapping
                    Dim newzBlkTblRec As BlockTableRecord = zBlkTblRec.DeepClone(mBlkTbl, zMap, True)
                    'Dim newzBlkTblRec As BlockTableRecord = zBlkTblRec.DeepCloneobjects(mBlkTbl, zMap, True)

                    newzBlkTblRec.Name = zBlkTblRec.Name & i
                    BlkRef.BlockTableRecord = mBlkTbl.Add(newzBlkTblRec)
                    mTrans.AddNewlyCreatedDBObject(newzBlkTblRec, True)
                    i = i + 1

                Next

            Catch ex As SystemException
                MsgBox(ex.Message.ToString, MsgBoxStyle.Critical, "Rename block.")
            End Try

            mTrans.Commit()

        End Using

    End Function


    Public Shared Function getBlockPreview(ByVal sBlockName As String) As Drawing.Bitmap
        clsFunctions.makeLog(clsFunctions.getMyDocDir & "\Blocks.log", "Get PreviewIcon for : " & sBlockName & " ")
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
        Using acLockDoc As DocumentLock = acDoc.LockDocument()
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                Dim acBlkTbl As BlockTable = DirectCast(acCurDb.BlockTableId.GetObject(OpenMode.ForWrite), BlockTable)
                Dim acBID As ObjectId = acBlkTbl.Item(sBlockName)
                Dim acBTRBlock As BlockTableRecord = acTrans.GetObject(acBID, OpenMode.ForRead)
                clsFunctions.makeLog(clsFunctions.getMyDocDir & "\Blocks.log", "Block : " & sBlockName & " verwerkt")
                'acBTRBlock.PreviewIcon.Save(clsFunctions.getMyDocDir & "\" & sBlockName & ".bmp")
                Return acBTRBlock.PreviewIcon
            End Using
        End Using
        Return Nothing
    End Function


    Public Shared Function getBlockPreview(ByVal pcbPreview As System.Windows.Forms.PictureBox, ByVal sBlockName As String)
        'clsFunctions.makeLog(clsFunctions.getMyDocDir & "\Blocks.log", "Get PreviewIcon for : " & sBlockName & " ")
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
        Using acLockDoc As DocumentLock = acDoc.LockDocument()
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                Dim acBlkTbl As BlockTable = DirectCast(acCurDb.BlockTableId.GetObject(OpenMode.ForWrite), BlockTable)
                Dim acBID As ObjectId = acBlkTbl.Item(sBlockName)

                Dim btr = CType(acTrans.GetObject(acBID, OpenMode.ForRead), BlockTableRecord)
                If (btr.IsLayout OrElse btr.IsAnonymous) Then
                    'TODO: Warning!!! continue If
                End If

                Dim imgsrc = CMLContentSearchPreviews.GetBlockTRThumbnail(btr)
                Dim bmp = ImageSourceToGDI(CType(imgsrc, System.Windows.Media.Imaging.BitmapSource))
                'pcbPreview.Image = bmp
                pcbPreview.BackgroundImage = bmp
                Return True
            End Using
        End Using
        Return True
    End Function

    Private Shared Function ExtractThumbnails(ByVal btrId As ObjectId, ByVal tr As Transaction, ByVal bt As BlockTable)
        Dim btr = CType(tr.GetObject(btrId, OpenMode.ForRead), BlockTableRecord)
        ' Ignore layouts and anonymous blocks
        If (btr.IsLayout OrElse btr.IsAnonymous) Then
            'TODO: Warning!!! continue If
        End If

        Dim imgsrc = CMLContentSearchPreviews.GetBlockTRThumbnail(btr)
        Dim bmp = ImageSourceToGDI(CType(imgsrc, System.Windows.Media.Imaging.BitmapSource))
        Return bmp
    End Function

    Private Shared Function ExtractThumbnails(ByVal iconPath As String, ByVal tr As Transaction, ByVal bt As BlockTable) As Integer
        Dim numIcons As Integer = 0
        For Each btrId As ObjectId In bt
            Dim btr = CType(tr.GetObject(btrId, OpenMode.ForRead), BlockTableRecord)
            ' Ignore layouts and anonymous blocks
            If (btr.IsLayout OrElse btr.IsAnonymous) Then
                'TODO: Warning!!! continue If
            End If

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
            ' Increment our icon counter
            numIcons = (numIcons + 1)
        Next
        Return numIcons
    End Function

    Public Shared Function MakeHatchPreview(ByVal pcbPreview As System.Windows.Forms.PictureBox, ByVal dictHatch As Dictionary(Of String, String))
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
        Using acLockDoc As DocumentLock = acDoc.LockDocument()
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                Dim acBlkTbl As BlockTable = DirectCast(acCurDb.BlockTableId.GetObject(OpenMode.ForWrite), BlockTable)
                If acBlkTbl.Has("RNhatchpreview") Then
                    Dim acBID As ObjectId = acBlkTbl.Item("RNhatchpreview")
                    acBID.GetObject(OpenMode.ForWrite).Erase(True)
                    clsFunctions.makeLog(clsFunctions.getMyDocDir & "\HatchPreview.log", "Old block removed ")
                End If
                acTrans.Commit()
            End Using

            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                Dim acBlkTbl As BlockTable = DirectCast(acCurDb.BlockTableId.GetObject(OpenMode.ForWrite), BlockTable)
                Using acBlkTblRec As New BlockTableRecord
                    acBlkTblRec.Name = "RNhatchpreview" & dictHatch.Item("HatchPattern")
                    clsFunctions.makeLog(clsFunctions.getMyDocDir & "\HatchPreview.log", "Block: " & dictHatch.Item("HatchPattern") & " Created")
                    'kader voor de hatch
                    Using acLine As New Polyline
                        acLine.AddVertexAt(0, New Point2d(0, 0), 0, 0, 0)
                        acLine.AddVertexAt(1, New Point2d(0, 2), 0, 0, 0)
                        acLine.AddVertexAt(2, New Point2d(2, 2), 0, 0, 0)
                        acLine.AddVertexAt(3, New Point2d(2, 0), 0, 0, 0)
                        acLine.Layer = "0"
                        acLine.Closed = True
                        clsFunctions.makeLog(clsFunctions.getMyDocDir & "\HatchPreview.log", "Append LIne")
                        acBlkTblRec.AppendEntity(acLine)
                        'object id saven en gebruiken voor de hatch
                        Dim recIds As ObjectIdCollection = New ObjectIdCollection()
                        recIds.Add(acLine.ObjectId)
                        Dim myHatch As Hatch = New Hatch()
                        clsFunctions.makeLog(clsFunctions.getMyDocDir & "\HatchPreview.log", "Append Hatch")
                        acBlkTblRec.AppendEntity(myHatch)
                        myHatch.PatternScale = CDbl(dictHatch.Item("PatternScale"))
                        'myHatch.Color = clsTools.objColor(dtFilterRow.Item(12).ToString, dtFilterRow.Item(3).ToString, dtFilterRow.Item(13).ToString, dtFilterRow.Item(14).ToString)
                        'TODO Backgroundcolor
                        myHatch.Layer = dictHatch.Item("Layer")
                        clsFunctions.makeLog(clsFunctions.getMyDocDir & "\HatchPreview.log", "Get Pat Type")
                        Dim patType As HatchPatternType
                        If dictHatch.Item("HatchPatternType") = "PreDefined" Then
                            patType = HatchPatternType.PreDefined
                        ElseIf dictHatch.Item("HatchPatternType") = "CustomDefined" Then
                            patType = HatchPatternType.CustomDefined
                        Else
                            patType = HatchPatternType.UserDefined
                        End If
                        Try
                            myHatch.SetHatchPattern(patType, dictHatch.Item("HatchPattern"))
                        Catch ex As Autodesk.AutoCAD.Runtime.Exception
                            MsgBox("Fout bij het laden van het Arceer patroon!")
                        End Try
                        clsFunctions.makeLog(clsFunctions.getMyDocDir & "\HatchPreview.log", "Append Loop")
                        myHatch.AppendLoop(HatchLoopTypes.Outermost, recIds)
                        myHatch.EvaluateHatch(True)
                    End Using

                    acBlkTbl.Add(acBlkTblRec)
                    acTrans.AddNewlyCreatedDBObject(acBlkTblRec, True)
                    'acBlkTblRec.Erase(True)
                End Using
                acTrans.Commit()
                clsFunctions.makeLog(clsFunctions.getMyDocDir & "\HatchPreview.log", "Get Preview icon for: " & dictHatch.Item("HatchPattern") & " ")
                getBlockPreview(pcbPreview, "RNhatchpreview" & dictHatch.Item("HatchPattern"))
                Return True
            End Using
        End Using

    End Function

    ' Helper function to generate an Image from a BitmapSource
    Private Shared Function ImageSourceToGDI(ByVal src As System.Windows.Media.Imaging.BitmapSource) As System.Drawing.Image
        Dim ms = New MemoryStream
        Dim encoder = New System.Windows.Media.Imaging.BmpBitmapEncoder
        encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(src))
        encoder.Save(ms)
        ms.Flush()
        Return System.Drawing.Image.FromStream(ms)
    End Function


End Class


Class BlockJig
    Inherits EntityJig
    ' Member variables

    Private _ucs As Matrix3d
    Private _pos As Point3d
    Private _atts As Dictionary(Of ObjectId, ObjectId)
    Private _tr As Transaction

    Private mRotation As Double = 0

    Private bRotate As Boolean = True

    Public iCurrJiggFunction As Integer = 0

    ' Constructor

    Public Sub New(ucs As Matrix3d, tr As Transaction, br As BlockReference, atts As Dictionary(Of ObjectId, ObjectId))
        MyBase.New(br)
        _ucs = ucs
        _pos = br.Position
        _atts = atts
        _tr = tr
    End Sub

    Protected Overrides Function Update() As Boolean
        Dim br = DirectCast(Entity, BlockReference)
        Select Case iCurrJiggFunction
            Case 0

                ' Transform to the current UCS

                br.Position = _pos.TransformBy(_ucs)


                Exit Select
            Case 1
                br.Rotation = mRotation

                Exit Select

            Case 3
                br.Erase()
                Exit Select
        End Select
        'attributes updaten
        If br.AttributeCollection.Count > 0 Then
            For Each id As ObjectId In br.AttributeCollection
                Dim obj = _tr.GetObject(id, OpenMode.ForRead)
                Dim ar = TryCast(obj, AttributeReference)

                If ar IsNot Nothing Then
                    ar.UpgradeOpen()

                    ' Open the associated attribute definition

                    Dim defId = _atts(ar.ObjectId)
                    Dim obj2 = _tr.GetObject(defId, OpenMode.ForRead)
                    Dim ad = DirectCast(obj2, AttributeDefinition)

                    ' Use it to set positional information on the
                    ' reference

                    ar.SetAttributeFromBlock(ad, br.BlockTransform)
                    ar.AdjustAlignment(br.Database)
                End If
            Next
        End If
        Return True
    End Function

    Protected Overrides Function Sampler(prompts As JigPrompts) As SamplerStatus
        Select Case iCurrJiggFunction
            Case 0
                Dim opts = New JigPromptPointOptions(vbLf & "Selecteer insertion point:")
                opts.BasePoint = Point3d.Origin
                opts.UserInputControls = UserInputControls.NoZeroResponseAccepted

                Dim ppr = prompts.AcquirePoint(opts)
                If ppr.Status = PromptStatus.Cancel Then
                    Dim br = DirectCast(Entity, BlockReference)
                    br.Erase()
                    Exit Select
                End If
                Dim ucsPt = ppr.Value.TransformBy(_ucs.Inverse())
                If _pos = ucsPt Then
                    Return SamplerStatus.NoChange
                End If

                _pos = ucsPt

                Exit Select
            Case 1
                Dim prOptions1 As New JigPromptAngleOptions(vbLf & "Block rotatie:")
                prOptions1.BasePoint = TryCast(Entity, BlockReference).Position
                prOptions1.UseBasePoint = True
                Dim prResult1 As PromptDoubleResult = prompts.AcquireAngle(prOptions1)
                If prResult1.Status = PromptStatus.Cancel Then
                    Return SamplerStatus.Cancel
                End If

                If prResult1.Value.Equals(mRotation) Then
                    Return SamplerStatus.NoChange
                Else
                    mRotation = prResult1.Value
                    Return SamplerStatus.OK
                End If

                Exit Select

            Case 2
                'geen rotatie
                Return SamplerStatus.OK
            Case Else
                Return SamplerStatus.Cancel
        End Select


        Return SamplerStatus.OK
    End Function

    Public Function Run(ByRef oObjId As ObjectId, ByRef pInsertion As Point3d, Optional bRotate As Boolean = True) As PromptStatus
        Dim doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        bRotate = bRotate 'wel of niet roteren na plaatsen
        If doc.Editor.Drag(Me).Status = PromptStatus.Keyword Then

        Else
            'insertion point
            Dim br = DirectCast(Entity, BlockReference)
            pInsertion = br.Position
            oObjId = br.ObjectId
            'overschakelen op rotatie
            If bRotate Then
                iCurrJiggFunction = 1
            Else
                iCurrJiggFunction = 2 'geen rotatie
            End If
        End If
        If doc Is Nothing Then
            Return PromptStatus.[Error]
        End If

        Return doc.Editor.Drag(Me).Status
    End Function
End Class