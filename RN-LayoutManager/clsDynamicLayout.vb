Imports System.IO
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.GraphicsInterface
Imports Newtonsoft.Json.Linq
Imports MgdAcApplication = Autodesk.AutoCAD.ApplicationServices.Application

Public Class clsDynamicLayout
    Inherits DrawJig

    Private mBase As Point3d
    Private mLocation As Point3d
    Private mRotation As Double
    Private sAction As String
    Private mEntities As List(Of Entity)

#Region "Constructors"
    Public Sub New(basePt As Point3d)
        mBase = basePt.TransformBy(UCS)
        mEntities = New List(Of Entity)()
        'sAction = "move"
    End Sub

#End Region

#Region "Properties"

    Public Property Base() As Point3d
        Get
            Return mLocation
        End Get
        Set
            mLocation = Value
        End Set
    End Property

    Public Property Location() As Point3d
        Get
            Return mLocation
        End Get
        Set
            mLocation = Value
        End Set
    End Property

    Public Property Rotation() As Double
        Get
            Return mRotation
        End Get
        Set
            mRotation = Value
        End Set
    End Property

    Public Property Action() As String
        Get
            Return sAction
        End Get
        Set
            sAction = Value
        End Set
    End Property

    Public ReadOnly Property UCS() As Matrix3d
        Get
            Return MgdAcApplication.DocumentManager.MdiActiveDocument.Editor.CurrentUserCoordinateSystem
        End Get
    End Property

#End Region

#Region "Methods"

    Public Sub AddEntity(ent As Entity)
        mEntities.Add(ent)
    End Sub

    Public Sub TransformEntities()
        Dim mat As Matrix3d = Matrix3d.Displacement(mBase.GetVectorTo(mLocation))

        For Each ent As Entity In mEntities
            ent.TransformBy(mat)
        Next
    End Sub

#End Region

    Protected Overrides Function WorldDraw(draw As WorldDraw) As Boolean
        Dim mat As Matrix3d = Matrix3d.Displacement(mBase.GetVectorTo(mLocation))
        Dim geo As WorldGeometry = draw.Geometry
        If geo IsNot Nothing Then
            geo.PushModelTransform(mat)

            For Each ent As Entity In mEntities
                geo.Draw(ent)
            Next

            geo.PopModelTransform()
        End If

        Return True
    End Function

    Protected Overrides Function Sampler(prompts As JigPrompts) As SamplerStatus

        Dim prOptions1 As New JigPromptPointOptions(vbLf & "Selecteer locatie voor het viewport:")
        prOptions1.UseBasePoint = False

        Dim prResult1 As PromptPointResult = prompts.AcquirePoint(prOptions1)
        If prResult1.Status = PromptStatus.Cancel OrElse prResult1.Status = PromptStatus.[Error] Then
            Return SamplerStatus.Cancel
        End If

        If Not mLocation.IsEqualTo(prResult1.Value, New Tolerance(0.000000001, 0.000000001)) Then
            mLocation = prResult1.Value
            Return SamplerStatus.OK
        Else
            Return SamplerStatus.NoChange
        End If

    End Function

#Region "Rotate Jigg"
    Class RotateJig
        Inherits EntityJig

        Private m_baseAngle, m_deltaAngle As Double
        Private m_rotationPoint As Point3d
        Private m_ucs As Matrix3d

        Public Sub New(ByVal ent As Entity, ByVal rotationPoint As Point3d, ByVal baseAngle As Double, ByVal ucs As Matrix3d)
            MyBase.New(TryCast(ent.Clone(), Entity))
            m_rotationPoint = rotationPoint
            m_baseAngle = baseAngle
            m_ucs = ucs
        End Sub

        Protected Overrides Function Sampler(ByVal jp As JigPrompts) As SamplerStatus
            Dim jo As JigPromptAngleOptions = New JigPromptAngleOptions(vbLf & "Geef de rotatie van het viewport: ")
            jo.BasePoint = m_rotationPoint
            jo.UseBasePoint = True
            Dim pdr As PromptDoubleResult = jp.AcquireAngle(jo)

            If pdr.Status = PromptStatus.OK Then

                If m_baseAngle = pdr.Value Then
                    Return SamplerStatus.NoChange
                Else
                    m_deltaAngle = pdr.Value
                    Return SamplerStatus.OK
                End If
            End If

            Return SamplerStatus.Cancel
        End Function

        Protected Overrides Function Update() As Boolean
            If m_deltaAngle > Tolerance.[Global].EqualPoint Then
                Dim trans As Matrix3d = Matrix3d.Rotation(m_deltaAngle - m_baseAngle, m_ucs.CoordinateSystem3d.Zaxis, m_rotationPoint)
                Entity.TransformBy(trans)
                m_baseAngle = m_deltaAngle
                m_deltaAngle = 0.0
            End If

            Return True
        End Function

        Public Function GetEntity() As Entity
            Return Entity
        End Function

        Public Function GetRotation() As Double
            Return m_baseAngle + m_deltaAngle
        End Function
    End Class
#End Region


    Public Shared jigger As clsDynamicLayout
    Public Shared Function DynamicLayout(ByRef sSchaal As String, ByRef sTemplate As String, ByRef sFormaat As String, Optional dStartRotation As Double = 0.0) As ViewPortCoordinates
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = acDoc.Editor

        Dim vpCenter As Point3d = New Point3d()
        Dim vpCoordinates As New ViewPortCoordinates
        vpCoordinates.vpEmpty = True
        vpCoordinates.vpCenter = New Point2d()
        vpCoordinates.vpExtMax = New Point2d()
        vpCoordinates.vpExtMin = New Point2d()
        vpCoordinates.vpRotation = 0.0

        'set focus to dwg
        Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

        Dim acBlkTbl As BlockTable
        Dim acBlkTblRec As BlockTableRecord

        Using acLockDoc As DocumentLock = acDoc.LockDocument()
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)
                acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)

                'viewport basepoint
                Dim ptBasePnt As Point3d

                'viewport extends
                Dim plVirtViewport As Autodesk.AutoCAD.DatabaseServices.Polyline = New Autodesk.AutoCAD.DatabaseServices.Polyline

                'inladen van viewports uit JSON
                'Formaat -> schaal
                plVirtViewport = LoadViewport(sFormaat, sSchaal, ptBasePnt)

                acBlkTblRec.AppendEntity(plVirtViewport)

                acTrans.AddNewlyCreatedDBObject(plVirtViewport, True)
                acTrans.Commit()
                Dim oPlId As ObjectId = plVirtViewport.ObjectId

                Try
                    'Dim ptBasePnt As Point3d = New Point3d(0, 0, 0)
                    jigger = New clsDynamicLayout(ptBasePnt)
                    Using tr As Transaction = acCurDb.TransactionManager.StartTransaction()
                        acBlkTbl = tr.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)
                        acBlkTblRec = tr.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)
                        'For Each id As ObjectId In selRes.Value.GetObjectIds()
                        Dim ent As Entity = DirectCast(tr.GetObject(oPlId, OpenMode.ForWrite), Entity)
                        jigger.AddEntity(ent)


                        Dim dRotation As Double = 0.0
                        Dim jigRes As PromptResult = MgdAcApplication.DocumentManager.MdiActiveDocument.Editor.Drag(jigger)
                        If jigRes.Status = PromptStatus.OK Then
                            jigger.TransformEntities()
                            vpCoordinates.vpCenter = New Point2d(jigger.Location.X, jigger.Location.Y)
                            'vp markeren als niet meer leeg
                            vpCoordinates.vpEmpty = False

                            'rotatie van viewport
                            ptBasePnt = jigger.Location
                            Dim ucs As Matrix3d = acEd.CurrentUserCoordinateSystem
                            'Dim jiggerRotate As RotateJig = New RotateJig(ent, ptBasePnt, 0.0, ucs)
                            Dim jiggerRotate As RotateJig = New RotateJig(ent, ptBasePnt, dStartRotation, ucs)
                            Dim jigResRotate As PromptResult = acEd.Drag(jiggerRotate)
                            If jigResRotate.Status = PromptStatus.OK Then
                                dRotation = jiggerRotate.GetRotation()
                                jiggerRotate.GetEntity().Dispose()
                                Dim trans As Matrix3d = Matrix3d.Rotation(dRotation, ucs.CoordinateSystem3d.Zaxis, ptBasePnt)
                                vpCoordinates.vpRotation = dRotation
                                ent.UpgradeOpen()
                                ent.TransformBy(trans)
                            End If
                            tr.Commit()
                        Else
                            ent.Erase()
                            'tr.Abort()
                            tr.Commit()
                            vpCoordinates.vpEmpty = True
                        End If
                    End Using
                Catch ex As System.Exception
                    MgdAcApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.ToString())
                End Try
                DynamicLayout = vpCoordinates
            End Using
        End Using
    End Function

    Public Shared Function showViewports(ByVal sFormaat As String)
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = acDoc.Editor
        Dim acBlkTbl As BlockTable
        Dim acBlkTblRec As BlockTableRecord
        'set focus to dwg
        Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
        Dim sBlockName As String = "Viewportoverizcht_" & sFormaat

        Try
            Using acLockDoc As DocumentLock = acDoc.LockDocument()
                Dim ptBasePnt As Point3d = New Point3d(0, 0, 0)
                jigger = New clsDynamicLayout(ptBasePnt)
                Using tr As Transaction = acCurDb.TransactionManager.StartTransaction()
                    acBlkTbl = tr.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)
                    acBlkTblRec = tr.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)
                    Dim lstEnts As List(Of Entity) = New List(Of Entity)
                    Dim blkID As ObjectId
                    'TODO: check of block al bestaat, zoja deze inserten en gebruiken inplaats van opnieuw maken
                    If acBlkTbl.Has(sBlockName) Then
                        'block bestaat
                        'blkID = acBlkTbl(sBlockName)
                    Else
                        'viewports aanmaken
                        blkID = LoadViewports(sFormaat)
                    End If

                    Dim btr As BlockTableRecord = tr.GetObject(acBlkTbl(sBlockName), OpenMode.ForRead)
                    Dim br As New BlockReference(New Point3d(), btr.ObjectId)

                    Dim sLayer As String = "X-XX-AL-VIEWPORTS-S"
                    clsFunctions.makeLayer(sLayer, "index", "10")

                    br.Layer = sLayer
                    acBlkTblRec.AppendEntity(br)
                    tr.AddNewlyCreatedDBObject(br, True)
                    ' Instantiate our map between attribute references
                    ' and their definitions
                    Dim atts = New Dictionary(Of ObjectId, ObjectId)()

                    If btr.HasAttributeDefinitions Then
                        For Each id As ObjectId In btr
                            Dim obj = tr.GetObject(id, OpenMode.ForRead)
                            Dim ad = TryCast(obj, AttributeDefinition)

                            If ad IsNot Nothing AndAlso Not ad.Constant Then
                                Dim ar = New AttributeReference()
                                ar.SetAttributeFromBlock(ad, br.BlockTransform)
                                ar.TextString = ad.TextString
                                Dim arId = br.AttributeCollection.AppendAttribute(ar)
                                tr.AddNewlyCreatedDBObject(ar, True)
                                atts.Add(arId, ad.ObjectId)
                            End If
                        Next
                    End If
                    '' Run the jig
                    Dim jig = New BlockJig(acEd.CurrentUserCoordinateSystem, tr, br, atts)
                    If jig.Run(New ObjectId, New Point3d, True) <> PromptStatus.OK Then
                        'Jig is afgebroken
                        Return True
                    End If

                    tr.Commit()
                End Using
            End Using
        Catch ex As System.Exception
            MgdAcApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.ToString())
        End Try
    End Function

    Public Shared Function LoadViewport(ByVal sFormaat As String, ByVal sSchaal As String, ByRef ptBasePnt As Point3d) As Autodesk.AutoCAD.DatabaseServices.Polyline
        Dim sFilePath As String = clsFunctions.getCoreDir & "\viewports.json"
        Dim plVirtViewport As Autodesk.AutoCAD.DatabaseServices.Polyline = New Autodesk.AutoCAD.DatabaseServices.Polyline
        Dim sLayer As String = "X-XX-AL-TEKENBLAD_BLADINDELING-G"
        If clsFunctions.makeLayer(sLayer, "rgb", "255,0,0") = True Then
            'ok
        Else
            sLayer = "0"
        End If
        plVirtViewport.Layer = sLayer

        If File.Exists(sFilePath) Then
            Dim sJson As String = File.ReadAllText(sFilePath)
            Dim myJsonObject As JObject = JObject.Parse(sJson)
            Dim aPresets As JArray = myJsonObject("viewports")
            For i = 0 To aPresets.Count - 1
                If aPresets(i).SelectToken("formaat").ToString = sFormaat Then
                    'dit is het juiste formaat blad, nu de juiste schaal vinden
                    Dim aSchalen As JArray = aPresets(i).SelectToken("schalen")

                    For x = 0 To aSchalen.Count - 1
                        If aSchalen(x).SelectToken("schaal") = sSchaal Then
                            'juiste schaal gevonden, polyline uitwerken
                            Dim aVertices As String() = aSchalen(x).SelectToken("vertices").ToString.Split(";")
                            For y = 0 To aVertices.Length - 1
                                Dim sCoords As String() = aVertices(y).Split(",")
                                plVirtViewport.AddVertexAt(y, New Point2d(CDbl(sCoords(0)), CDbl(sCoords(1))), 0, 0, 0)
                            Next
                            plVirtViewport.Closed = True
                            Dim plExtends As Extents3d = plVirtViewport.GeometricExtents
                            Dim ptMax As Point3d = plExtends.MaxPoint
                            Dim ptMin As Point3d = plExtends.MinPoint
                            ptBasePnt = New Point3d((ptMax.X + ptMin.X) / 2, (ptMax.Y + ptMin.Y) / 2, 0)
                            LoadViewport = plVirtViewport
                            Exit Function
                        End If
                    Next
                End If
            Next
        End If
        MsgBox("Gekozen papierformaat niet gevonden, Standaard 1:200 A1_LS wordt getoond")
        'A1 1:200 layout weergeven, geen geschikte layout in Json gevonden
        plVirtViewport.AddVertexAt(0, New Point2d(-82.1, 57.4), 0, 0, 0)
        plVirtViewport.AddVertexAt(1, New Point2d(82.1, 57.4), 0, 0, 0)
        plVirtViewport.AddVertexAt(2, New Point2d(82.1, -38.587), 0, 0, 0)
        plVirtViewport.AddVertexAt(3, New Point2d(42.086, -38.587), 0, 0, 0)
        plVirtViewport.AddVertexAt(4, New Point2d(42.1, -57.4), 0, 0, 0)
        plVirtViewport.AddVertexAt(5, New Point2d(-82.1, -57.4), 0, 0, 0)
        plVirtViewport.Closed = True
        LoadViewport = plVirtViewport
    End Function

    Public Shared Function LoadViewports(ByVal sFormaat As String) As ObjectId
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = acDoc.Editor
        Dim acBlkTbl As BlockTable
        Dim acBlkTblRec As BlockTableRecord

        Dim sFilePath As String = clsFunctions.getCoreDir & "\viewports.json"
        Dim plVirtViewport As Autodesk.AutoCAD.DatabaseServices.Polyline = New Autodesk.AutoCAD.DatabaseServices.Polyline
        Dim ptBasePnt As Point3d = New Point3d()
        Dim lstVirtViewports As List(Of Entity) = New List(Of Entity)
        Dim colObjecIDS As ObjectIdCollection = New ObjectIdCollection
        Dim blkID As ObjectId

        If File.Exists(sFilePath) Then
            Dim sJson As String = File.ReadAllText(sFilePath)
            Dim myJsonObject As JObject = JObject.Parse(sJson)
            Dim aPresets As JArray = myJsonObject("viewports")
            For i = 0 To aPresets.Count - 1
                If aPresets(i).SelectToken("formaat").ToString = sFormaat Then
                    'dit is het juiste formaat blad, nu de juiste schaal vinden
                    Dim aSchalen As JArray = aPresets(i).SelectToken("schalen")


                    Using acLockDoc As DocumentLock = acDoc.LockDocument()
                        For x = 0 To aSchalen.Count - 1

                            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)
                                acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)
                                plVirtViewport = New Autodesk.AutoCAD.DatabaseServices.Polyline
                                'juiste schaal gevonden, polyline uitwerken
                                Dim aVertices As String() = aSchalen(x).SelectToken("vertices").ToString.Split(";")
                                For y = 0 To aVertices.Length - 1
                                    Dim sCoords As String() = aVertices(y).Split(",")
                                    plVirtViewport.AddVertexAt(y, New Point2d(CDbl(sCoords(0)), CDbl(sCoords(1))), 0, 0, 0)
                                Next
                                plVirtViewport.Closed = True
                                Dim plExtends As Extents3d = plVirtViewport.GeometricExtents
                                Dim ptMax As Point3d = plExtends.MaxPoint
                                Dim ptMin As Point3d = plExtends.MinPoint
                                ptBasePnt = New Point3d((ptMax.X + ptMin.X) / 2, (ptMax.Y + ptMin.Y) / 2, 0)
                                'verplaats viewport naar 0,0
                                Dim acVec3d As Vector3d = ptBasePnt.GetVectorTo(New Point3d(0, 0, 0))
                                plVirtViewport.TransformBy(Matrix3d.Displacement(acVec3d))
                                plVirtViewport.Layer = 0
                                acBlkTblRec.AppendEntity(plVirtViewport)
                                acTrans.AddNewlyCreatedDBObject(plVirtViewport, True)
                                'get entity from object and save
                                Dim ent As Entity = DirectCast(acTrans.GetObject(plVirtViewport.ObjectId, OpenMode.ForWrite), Entity)
                                lstVirtViewports.Add(ent)
                                colObjecIDS.Add(plVirtViewport.ObjectId)
                                'add textlabel to viewport
                                plExtends = plVirtViewport.GeometricExtents 'nieuwe min / max bepalen voor text label
                                Dim mText As MText = New MText()
                                mText.Contents = sFormaat & " - " & aSchalen(x).SelectToken("schaal").ToString
                                mText.TextHeight = 5.5
                                mText.Attachment = AttachmentPoint.BottomLeft
                                mText.Location = plExtends.MinPoint
                                mText.Layer = 0
                                acBlkTblRec.AppendEntity(mText)
                                acTrans.AddNewlyCreatedDBObject(mText, True)
                                ent = DirectCast(acTrans.GetObject(mText.ObjectId, OpenMode.ForWrite), Entity)
                                lstVirtViewports.Add(ent)
                                colObjecIDS.Add(mText.ObjectId)
                                acTrans.Commit()
                            End Using
                        Next
                    End Using
                    'block maken van de elementen
                    blkID = buildBlockFromObjectIdCollection(colObjecIDS, sFormaat, lstVirtViewports)
                End If
            Next
        End If
        Return blkID
    End Function

    Public Shared Function buildBlockFromObjectIdCollection(ByVal colObjID As ObjectIdCollection, ByVal sFormaat As String, ByVal lstEnts As List(Of Entity)) As ObjectId
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = acDoc.Editor
        Dim acBlkTbl As BlockTable
        Dim acBlkTblRec As BlockTableRecord

        Dim blkEnt As Entity
        Dim sBlockName As String = "Viewportoverizcht_" & sFormaat
        Dim blkId As ObjectId

        Using acLockDoc As DocumentLock = acDoc.LockDocument()
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)
                acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)

                If acBlkTbl.Has(sBlockName) Then
                    'block bestaat al en eigenlijk zouden we hier niet moeten uitkomen
                    blkId = acBlkTbl(sBlockName)
                    Return blkId
                    Exit Function
                Else
                    acBlkTbl.UpgradeOpen()
                    Dim blkReccord As BlockTableRecord = New BlockTableRecord()
                    acBlkTbl.Add(blkReccord)
                    blkReccord.Name = sBlockName
                    blkReccord.Origin = New Point3d(0, 0, 0)
                    blkReccord.BlockScaling = BlockScaling.Uniform
                    blkReccord.Explodable = True
                    acTrans.AddNewlyCreatedDBObject(blkReccord, True)
                    'objecten toevoegen aan het block
                    Dim mapping As IdMapping = New IdMapping
                    acCurDb.DeepCloneObjects(colObjID, blkReccord.ObjectId, mapping, False)
                    Dim coll As New ObjectIdCollection()
                    For Each pair As IdPair In mapping
                        If pair.IsPrimary Then
                            Dim ent As Entity = DirectCast(acTrans.GetObject(pair.Value, OpenMode.ForWrite), Entity)
                            If ent IsNot Nothing Then
                                ent.Layer = "0"
                                ent.ColorIndex = 0
                                If (TypeOf (ent) Is AttributeDefinition) Then
                                    Dim att As AttributeDefinition = DirectCast(ent, AttributeDefinition)
                                    att.LockPositionInBlock = True
                                End If
                                coll.Add(ent.ObjectId)
                            End If
                        End If
                    Next
                    blkReccord.AssumeOwnershipOf(coll)

                    'originele objecten verwijderen
                    For Each ent As Entity In lstEnts
                        ent.Erase()
                    Next
                    blkId = blkReccord.ObjectId
                    acTrans.Commit()

                End If
            End Using
        End Using
        Return blkId
    End Function

    Public Shared Function insertBlock(ByVal sBlockName As String) As Entity
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = acDoc.Editor
        Dim acBlkTbl As BlockTable
        Dim acBlkTblRec As BlockTableRecord

        Dim blkEnt As Entity = Nothing

        Using acLockDoc As DocumentLock = acDoc.LockDocument()
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)
                acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)
                If Not acBlkTbl.Has(sBlockName) Then
                    acEd.WriteMessage(vbLf & "Block '{0}' not found.", sBlockName)
                    MsgBox("Block NIET gevonden!")
                    Return Nothing
                    Exit Function
                End If
                'insert block
                Dim blkRecId As ObjectId = ObjectId.Null
                blkRecId = acBlkTbl(sBlockName)

                Dim acBlkRef As BlockReference
                acBlkRef = New BlockReference(New Point3d(0, 0, 0), blkRecId)
                'blkEnt = DirectCast(acTrans.GetObject(acBlkRef.ObjectId, OpenMode.ForWrite), Entity)
                acBlkTblRec.AppendEntity(acBlkRef)
                acTrans.AddNewlyCreatedDBObject(acBlkRef, True)
                acTrans.Commit()
            End Using
        End Using
        Return blkEnt
    End Function

    Public Shared Function buildVpCoords()
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = acDoc.Editor

        'set focus to dwg
        Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

        Dim acBlkTbl As BlockTable
        Dim acBlkTblRec As BlockTableRecord

        Using acLockDoc As DocumentLock = acDoc.LockDocument()
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)
                acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)
                Dim per As PromptEntityResult
                Dim peo As PromptEntityOptions = New PromptEntityOptions("Selecteer een polyline voor Viewport coordinates.")
                Dim oPolyline As ObjectId
                peo.SetRejectMessage("Alleen een polyline wordt momenteel ondersteund!")
                peo.AddAllowedClass(GetType(Autodesk.AutoCAD.DatabaseServices.Polyline), False)

                per = acEd.GetEntity(peo)

                If per.Status = PromptStatus.OK Then
                    'as geselecteerd
                    oPolyline = per.ObjectId
                Else
                    Return False
                    Exit Function
                End If
                Dim plVp As Autodesk.AutoCAD.DatabaseServices.Polyline = CType(acTrans.GetObject(oPolyline, OpenMode.ForWrite), Autodesk.AutoCAD.DatabaseServices.Polyline)
                Dim strCoords As String = ""
                For i = 0 To plVp.NumberOfVertices - 1
                    Dim pt As Point2d = plVp.GetPoint2dAt(i)
                    strCoords = strCoords & pt.X.ToString & "," & pt.Y.ToString & ";"
                Next
                strCoords = strCoords.Substring(0, strCoords.Length - 1) 'laatste ; verwijderen
                Dim sLayout As String = InputBox("Layout", "Layout", "")
                Dim sScale As String = InputBox("Schaal", "Schaal", "")
                clsFunctions.makeLog(clsFunctions.getMyDocDir & "\viewports\viewports.txt", sLayout & " -- " &  sScale)
                clsFunctions.makeLog(clsFunctions.getMyDocDir & "\viewports\viewports.txt", "{ ""schaal"":""" & sScale & """, ""vertices"":""" & strCoords & """}")
            End Using
        End Using
        MsgBox("Opgeslagen")
        Return True
    End Function
End Class

Public Class RotateView

End Class