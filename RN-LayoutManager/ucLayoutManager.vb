Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Windows
Imports System.Windows
Imports System.Linq
Imports System.IO
Imports System.Windows.Forms
Imports Autodesk.AutoCAD.PlottingServices
Imports System.Reflection
Imports System.ComponentModel
Imports System.Text.RegularExpressions

Public Class ucLayoutManager
    Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
    Dim acCurDb As Database = acDoc.Database
    Dim acEd As Editor = acDoc.Editor
    Dim iCheckCount As Integer = 0 'counter for checks
    'auto scroll during drag and drop
    Dim drag_drop_scroll_amount As Integer = 0
    Dim iFlowYmin As Integer = 0
    Dim iFlowYmax As Integer = 0
    Dim iMouseCurrY As Integer = 0
    Dim sScrollDirection As String = ""
    Dim bIsDragging As Boolean = False
    '/auto scroll during drag and drop
    Dim sIniDir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\RNtools"
    Dim sIniFile As String = "\layoutmanager.ini"
    Dim iniFile As clsINI
    Dim sPDFuserFolder As String
    Dim sDefaultOutputLocation As String = ""
    Dim sCurrVersion As String = Assembly.GetExecutingAssembly().GetName().Version.ToString
    Dim sCurrentLayout As String
    Dim sLayoutTemplate As String = ""
    'layout attributes replace
    Dim layAndTabTemp As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)
    Dim oBlockID As ObjectId
    Dim sBlockName As String
    Dim oModalSpace As ObjectId
    Dim oPaperSpace As ObjectId
    Dim sActiveLayout As String
    Dim sAttribNames(0 To 99) As String
    Dim sAttribValue(0 To 99) As String
    Dim bAutoNumber(0 To 99) As Boolean
    Dim dStartValue(0 To 99) As Double
    Dim dCurrValue(0 To 99) As Double
    Dim dIncrementValue(0 To 99) As Double
    'filter values
    Dim aFilters As List(Of String)
    'Active Drawing Tracking
    Private Shared m_DocData As clsMyDocData = New clsMyDocData
    Dim AcApp As Autodesk.AutoCAD.ApplicationServices.Application
    Dim _lm As LayoutManager



    '### Active Drawing Tracking
    Private Sub DocumentManager_DocumentActivated(ByVal sender As Object, ByVal e As DocumentCollectionEventArgs)
        'tekening wordt geactiveerd
        Try
            'remap vars to current document
            acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
            acCurDb = acDoc.Database
            acEd = acDoc.Editor
            _lm = LayoutManager.Current
            loadLayouts()
            'Todo: implement load saved selection
            loadFilters()
            AddHandler Me._lm.LayoutSwitched, AddressOf Me.DocumentManger_DocumentLayoutSwitched
            AddHandler acDoc.CommandEnded, New CommandEventHandler(AddressOf commandEnd)
        Catch
            'MsgBox("Probleem bij DocumentActivated")
        End Try
    End Sub
    Private Sub DocumentManager_DocumentToBeDeactivated(ByVal sender As Object, ByVal e As DocumentCollectionEventArgs)
        'switch naar een andere tekening
        Try
            'Todo: implement save selection
            'remap vars to current document
            acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
            acCurDb = acDoc.Database
            acEd = acDoc.Editor
            RemoveHandler Me._lm.LayoutSwitched, AddressOf Me.DocumentManger_DocumentLayoutSwitched
            RemoveHandler acDoc.CommandEnded, AddressOf commandEnd
            resetList()
            resetFilter()
        Catch
            'MsgBox("Probleem bij DocumentToBeDactivated")
        End Try
    End Sub

    Private Sub DocumentManger_DocumentLayoutSwitched()
        'sub layout switched
        getCurrentLayout()
        itterateList("iscurrent")
    End Sub

    Private Sub DocumentManager_DocumentLayoutsModified()
        getCurrentLayout()
        loadLayouts()
    End Sub

    Public Shared Function commandStart(ByVal o As Object, ByVal e As CommandEventArgs)
        'MsgBox(e.GlobalCommandName)
        Return True
    End Function

    Public Function commandEnd(ByVal o As Object, ByVal e As CommandEventArgs)
        'MsgBox(e.GlobalCommandName)
        Select Case e.GlobalCommandName
            Case "LAYOUT_CONTROL"
                loadLayouts()
                'Todo: implement load saved selection
                loadFilters()

        End Select
        Return True
    End Function


    Public Sub getCurrentLayout()
        Dim acLayout As Layout = Nothing
        Using acLockDoc As DocumentLock = acDoc.LockDocument
            Dim acLayoutMgr As LayoutManager = LayoutManager.Current
            sCurrentLayout = acLayoutMgr.CurrentLayout
        End Using
    End Sub


    '### /Active Drawing Tracking

    Public Sub getTrustedPaths(AITools_path As String)
        Dim str_TR As String = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("TRUSTEDPATHS")

        Dim C_Paths As String = LCase(str_TR)
        Dim splitAry() As String = C_Paths.Split(";")

        Dim Old_Path_Ary As List(Of String) = New List(Of String)
        Old_Path_Ary = splitAry.ToList()

        Dim New_paths As List(Of String) = New List(Of String)

        New_paths.Add(AITools_path)

        For Each Str As String In New_paths
            If Not Old_Path_Ary.Contains(LCase(Str)) Then
                Old_Path_Ary.Add(Str)
            End If
        Next

        Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("TRUSTEDPATHS", String.Join(";", Old_Path_Ary.ToArray()))

    End Sub

    Private Sub LayoutManager_Load(sender As Object, e As EventArgs) Handles Me.Load
        '### Active Drawing Tracking
        AddHandler Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.DocumentActivated, AddressOf Me.DocumentManager_DocumentActivated
        AddHandler Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.DocumentToBeDeactivated, AddressOf Me.DocumentManager_DocumentToBeDeactivated
        AddHandler Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.DocumentToBeDestroyed, AddressOf Me.DocumentManager_DocumentToBeDeactivated
        'set current layoutmanager
        _lm = LayoutManager.Current
        AddHandler Me._lm.LayoutSwitched, AddressOf Me.DocumentManger_DocumentLayoutSwitched


        '### /Active Drawing Tracking
        loadLayouts()
        'set active layout
        DocumentManger_DocumentLayoutSwitched()
        'tool toevoegen aan trusted path
        getTrustedPaths(clsFunctions.getCoreDir())
        'load settings
        If File.Exists(sIniDir & sIniFile) Then
            'bestand bestaat, instelingen laden
            iniFile = New clsINI(sIniDir & sIniFile)
            sLayoutTemplate = iniFile.GetString("template", "layout", sLayoutTemplate)
            If sLayoutTemplate.Length > 0 Then
                loadExternalTemplate()
            End If
            'laad extra template files
            'layout templates laden
            Dim temp As String = iniFile.GetString("template", "layouts", "")
            Dim sLayoutTemplates As List(Of String) = New List(Of String)(temp.Split(","c))
            ContextMenuTemplates.Items.Clear()
            For Each sItem As String In sLayoutTemplates
                If Not sItem = vbNullString Then
                    Dim mItem As New ToolStripMenuItem()
                    mItem.Text = sItem
                    mItem.Name = sItem
                    AddHandler mItem.Click, AddressOf selectExternalTemplate
                    ContextMenuTemplates.Items.Add(mItem)
                End If
            Next
        End If
        'selectie filters laden
        loadFilters()
        If clsFunctions.isDebugMode() Then
            'debug modus = true
            lblTitel.Text = lblTitel.Text & " [DEBUG Version]"
        End If
    End Sub



    Public Sub resetList()
        'reset check count
        iCheckCount = 0
        updateCheckLabel()
        'clear flow
        flowLayouts.Controls.Clear()
    End Sub

    ''' <summary>
    ''' 'Load layouts into list
    ''' </summary>
    Public Sub loadLayouts(Optional ByVal bLoadFromList As Boolean = False)
        'reset and clear list
        'reset check count
        iCheckCount = 0
        updateCheckLabel()
        'clear flow
        flowLayouts.Controls.Clear()
        flowLayouts.AllowDrop = True
        flowLayouts.AutoScroll = True
        flowLayouts.SetAutoScrollMargin(5, 5)
        'load current layout name
        getCurrentLayout()
        'first add model item
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = New RN_LayoutItems.RN_UCLayoutItem()
        'current layout control
        Dim myCntrlCurrent As RN_LayoutItems.RN_UCLayoutItem = New RN_LayoutItems.RN_UCLayoutItem()
        myCntrl.LayoutName = "Model"
        myCntrl.IsModel = True
        drag_drop_scroll_amount = myCntrl.Height + 40
        'hide button print and checkbox
        myCntrl.hideButtons()
        myCntrl.updateItem()
        'only handler to change view since model can't be renamed
        AddHandler myCntrl.View_Click, AddressOf ItemViewClick
        flowLayouts.Controls.Add(myCntrl)
        ' Get the layout dictionary of the current database
        Dim layAndTab As SortedDictionary(Of Integer, String) = New SortedDictionary(Of Integer, String)
        Dim layAndTabOID As SortedDictionary(Of Integer, String) = New SortedDictionary(Of Integer, String)
        Try
            Using trx As Transaction = acCurDb.TransactionManager.StartTransaction()
                Dim layDict As DBDictionary = acCurDb.LayoutDictionaryId.GetObject(OpenMode.ForRead)
                For Each entry As DBDictionaryEntry In layDict
                    Dim lay As Layout = CType(entry.Value.GetObject(OpenMode.ForRead), Layout)
                    layAndTab.Add(lay.TabOrder, lay.LayoutName)
                    'layAndTabOID.Add(lay.TabOrder, lay.ObjectId)
                    layAndTabOID.Add(lay.TabOrder, lay.Handle.ToString)
                Next
                trx.Commit()
            End Using
        Catch ex As Exception
            MsgBox("Fout bij het laden van de Layouts " & ex.Message)
        End Try
        Dim iTabIndex As Integer = 1 'model = 0
        Try
            For Each sLayoutName In layAndTab.Values
                'add name to list except Model
                If sLayoutName = "Model" Then

                Else
                    myCntrl = New RN_LayoutItems.RN_UCLayoutItem()
                    myCntrl.LayoutName = sLayoutName
                    myCntrl.updateItem()
                    myCntrl.TabIndex = iTabIndex
                    If layAndTabOID.ContainsKey(iTabIndex) Then
                        'myCntrl.LayoutID = layAndTabOID.Item(iTabIndex)
                        myCntrl.LayoutHandle = layAndTabOID.Item(iTabIndex)
                    End If
                    'add handlers to register functions for items
                    AddHandler myCntrl.View_Click, AddressOf ItemViewClick
                    AddHandler myCntrl.LayoutNameEdit_KeyDown, AddressOf renameLayout
                    AddHandler myCntrl.Plot_Click, AddressOf PlotLayout
                    AddHandler myCntrl.Plot_CheckedChanged, AddressOf PlotCheck
                    AddHandler myCntrl.MouseMove, AddressOf item_MouseMove
                    AddHandler myCntrl.DragEnter, AddressOf item_DragEnter
                    myCntrl.ContextMenuStrip = SubMenu
                    'check if this is the current layout
                    'active layout makeren
                    If myCntrl.LayoutName = sCurrentLayout Then
                        myCntrl.IsCurrent = True
                        myCntrlCurrent = myCntrl
                    Else
                        myCntrl.IsCurrent = False
                    End If
                    myCntrl.isCurrentLayout()
                    'add layout to control flow
                    If bLoadFromList Then
                        'layout list is saved in dictionary, load this list
                        If layAndTabTemp.ContainsKey(sLayoutName) Then
                            'add from list
                            myCntrl.SetCheckState(CBool(layAndTabTemp.Item(sLayoutName)))
                        Else
                            'hide item
                            myCntrl.SetCheckState(False)
                            myCntrl.Visible = False
                        End If

                    End If
                    flowLayouts.Controls.Add(myCntrl)
                    iTabIndex += 1
                End If
            Next
        Catch ex As Exception
            MsgBox("Fout bij het weergeven van de Layouts " & ex.Message)
        End Try
        scrollCurrentLayoutIntoView(myCntrlCurrent)
    End Sub

    ''' <summary>
    ''' 'Rename layout function
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <returns>true or false</returns>
    Public Function renameLayout(ByVal sender As Object, e As EventArgs)
        'get current edited list item
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)
        'check if name has changed
        If myCntrl.LayoutName = myCntrl.LayoutNameOld Then
            'no change, exit function here
            Return False
        End If
        'load layout dictionary
        Dim bUpdate As Boolean = False
        Using acLockDoc As DocumentLock = acDoc.LockDocument
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                Dim lays As DBDictionary = acTrans.GetObject(acCurDb.LayoutDictionaryId, OpenMode.ForRead)
                If lays.Contains(myCntrl.LayoutNameOld) Then
                    If lays.Contains(myCntrl.LayoutName) Then
                        MsgBox("Er bestaat al een layout met deze naam!")
                        myCntrl.LayoutName = myCntrl.LayoutNameOld
                        myCntrl.updateItem()
                        Return False
                    End If
                    bUpdate = True
                End If
                acTrans.Commit()
            End Using
            If bUpdate Then
                Try
                    Dim acLayoutMgr As LayoutManager = LayoutManager.Current
                    acLayoutMgr.RenameLayout(myCntrl.LayoutNameOld, myCntrl.LayoutName)
                    acDoc.Editor.Regen()
                Catch ex As Exception
                    MsgBox("Er ging iets fout bij het wijzigen van de layoutnaam!" & vbCrLf & ex.Message & vbCrLf & ex.Source & vbCrLf & ex.StackTrace)
                    myCntrl.LayoutName = myCntrl.LayoutNameOld
                    myCntrl.updateItem()
                    Return False
                End Try
            Else
                MsgBox("Er ging iets fout, geen layout gevonden met de naam: " & myCntrl.LayoutNameOld)
                myCntrl.LayoutName = myCntrl.LayoutNameOld
                myCntrl.updateItem()
                Return False
            End If
        End Using
        Return True
    End Function

    ''' <summary>
    ''' 'Rename layout
    ''' </summary>
    ''' <param name="sNewName"></param>
    ''' <param name="sOldName"></param>
    ''' <returns></returns>
    Public Function renameLayoutByValue(ByVal sNewName As String, ByVal sOldName As String)
        Using acLockDoc As DocumentLock = acDoc.LockDocument
            Try
                Dim acLayoutMgr As LayoutManager = LayoutManager.Current
                acLayoutMgr.RenameLayout(sOldName, sNewName)
                acDoc.Editor.Regen()
                'MsgBox("hernoemen geslaagd")
                Return True
            Catch ex As Exception
                'MsgBox("fout bij hernoemen")
                Return False
            End Try
        End Using
    End Function



    Public Function PlotCheck(ByVal sender As Object, e As EventArgs)
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)

        If myCntrl.CheckState Then
            iCheckCount += 1
        Else
            iCheckCount -= 1
        End If
        updateCheckLabel()
        Return True
    End Function

    Public Sub updateCheckLabel()
        lblCheckCount.Text = "# " & iCheckCount.ToString ' & " selected"
    End Sub

    Public Function PlotLayout(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim sOutputLocation As String = PDFoutputLocation()
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)
        Dim layouts As New List(Of Layout)()
        Try
            Using acLockDoc As DocumentLock = acDoc.LockDocument
                Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                    Dim acLayoutMgr As LayoutManager = LayoutManager.Current

                    Dim oId As ObjectId = acLayoutMgr.GetLayoutId(myCntrl.LayoutName)
                    Dim lay As Layout = acTrans.GetObject(oId, OpenMode.ForWrite)
                    layouts.Add(lay)

                    acTrans.Commit()
                    plotLayouts(SheetType.SinglePdf, layouts)
                End Using
            End Using
        Catch ex As Exception
            MsgBox("Fout bij het laden van de layout lijst" & ex.Message & vbCrLf & ex.Source)
            Return False
        End Try
        Return True
    End Function

    ''' <summary>
    ''' 'Set active layout
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Sub ItemViewClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)
        setLayoutCurrent(myCntrl.LayoutName, myCntrl.IsModel)
        itterateList("iscurrent")
    End Sub

    Public Sub setLayoutCurrent(ByVal sLayoutName As String, ByVal bIsModel As Boolean)
        Using acLockDoc As DocumentLock = acDoc.LockDocument
            Dim acLayoutMgr As LayoutManager = LayoutManager.Current
            acLayoutMgr.CurrentLayout = sLayoutName
            acDoc.Editor.Regen()
            'zoom extends when not model view
            If bIsModel = False Then
                Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                    Dim oId As ObjectId = acLayoutMgr.GetLayoutId(sLayoutName)
                    Dim lay As Layout = acTrans.GetObject(oId, OpenMode.ForWrite)
                    'lock viewports when not locked
                    For Each vpId As ObjectId In lay.GetViewports()
                        Dim vp As Viewport = DirectCast(acTrans.GetObject(vpId, OpenMode.ForWrite, False, True), Viewport)
                        vp.Locked = True
                    Next
                    acTrans.Commit()
                    Dim acadApp As Object = Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication
                    acadApp.ZoomExtents()
                    acDoc.Editor.Regen()
                End Using
            End If
        End Using
    End Sub

    Public Sub scrollCurrentLayoutIntoView(ByVal myCntrl As RN_LayoutItems.RN_UCLayoutItem)
        flowLayouts.ScrollControlIntoView(myCntrl)
    End Sub

    ''' <summary>
    ''' Move item on drag / end drag
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Sub item_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If e.Button = Forms.MouseButtons.Left Then
            Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)
            myCntrl.DoDragDrop(myCntrl, Windows.DragDropEffects.Move)
            'reset dragstate of control
            myCntrl.GetDragged = False
            myCntrl.isDragged()
            bIsDragging = False
            'save new order
            setLayoutOrder()
            lblCheckCount.Text = iCheckCount.ToString
        End If
    End Sub

    ''' <summary>
    ''' Detect Drag / start drag
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Sub item_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs)
        If e.Data.GetDataPresent(GetType(RN_LayoutItems.RN_UCLayoutItem)) Then
            e.Effect = Windows.DragDropEffects.All
            Dim myCntrlOver As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)
            Dim myCntrlSrc As RN_LayoutItems.RN_UCLayoutItem = CType(e.Data.GetData(GetType(RN_LayoutItems.RN_UCLayoutItem)), RN_LayoutItems.RN_UCLayoutItem)

            Dim iIndexOver As Integer = flowLayouts.Controls.IndexOf(myCntrlOver)
            flowLayouts.Controls.SetChildIndex(myCntrlSrc, iIndexOver)
            'auto scroll list on drag
            flowLayouts.ScrollControlIntoView(myCntrlSrc)
            'reset dragstate of control
            myCntrlSrc.GetDragged = True
            myCntrlSrc.isDragged()
            bIsDragging = True
        End If
    End Sub

    Private Sub flowLayouts_DragOver(sender As Object, e As Forms.DragEventArgs) Handles flowLayouts.DragOver
        calcMousePosition()
    End Sub

    Private Sub flowLayouts_DragLeave(sender As Object, e As EventArgs) Handles flowLayouts.DragLeave
        calcMousePosition()
    End Sub

    Private Sub calcMousePosition()
        If bIsDragging Then 'prevent autoscroll when no drag
            Dim localMousePosition As System.Drawing.Point
            localMousePosition = PointToClient(Cursor.Position)
            iMouseCurrY = localMousePosition.Y
            iFlowYmax = flowLayouts.Top + flowLayouts.Height
            iFlowYmin = flowLayouts.Top
            If iMouseCurrY < (iFlowYmin + drag_drop_scroll_amount) Then
                'scroll down
                sScrollDirection = "UP"
                If tmrAutoScroll.Enabled = False Then
                    tmrAutoScroll.Enabled = True
                End If
            ElseIf iMouseCurrY > (iFlowYmax - drag_drop_scroll_amount) Then
                'scroll up
                sScrollDirection = "DOWN"
                If tmrAutoScroll.Enabled = False Then
                    tmrAutoScroll.Enabled = True
                End If
            Else
                'no scrolling
                sScrollDirection = ""
                tmrAutoScroll.Enabled = False
            End If
        End If
    End Sub

    Private Sub tmrAutoScroll_Tick(sender As Object, e As EventArgs) Handles tmrAutoScroll.Tick
        Dim iNewScrollValue As Integer = flowLayouts.VerticalScroll.Value
        If sScrollDirection = "UP" Then
            If flowLayouts.VerticalScroll.Value > drag_drop_scroll_amount Then
                iNewScrollValue = flowLayouts.VerticalScroll.Value - drag_drop_scroll_amount
            Else
                iNewScrollValue = 0
            End If
            If iNewScrollValue > iFlowYmin And iNewScrollValue < iFlowYmax Then
                flowLayouts.VerticalScroll.Value = iNewScrollValue
            End If
        ElseIf sScrollDirection = "DOWN" Then
            If flowLayouts.VerticalScroll.Value < iFlowYmax - drag_drop_scroll_amount Then
                iNewScrollValue = flowLayouts.VerticalScroll.Value + drag_drop_scroll_amount
            Else
                iNewScrollValue = iFlowYmax
            End If
            If iNewScrollValue > iFlowYmin And iNewScrollValue < iFlowYmax Then
                flowLayouts.VerticalScroll.Value = iNewScrollValue
            End If
        End If
    End Sub

    Private Sub flowLayouts_MouseMove(sender As Object, e As MouseEventArgs) Handles flowLayouts.MouseMove
        calcMousePosition()
    End Sub

    ''' <summary>
    ''' 'Apply layout order to drawing
    ''' </summary>
    ''' <param name="sOrder"></param>
    ''' <returns></returns>
    Public Function setLayoutOrder(Optional ByVal sOrder As String = "list")
        Dim aSortedLayouts() As String
        pgbVoortgang.Visible = True
        pgbVoortgang.Value = 0
        pgbVoortgang.Maximum = flowLayouts.Controls.Count + 2
        If sOrder = "list" Then
            Try
                Using acLockDoc As DocumentLock = acDoc.LockDocument
                    Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                        Dim acLayoutMgr As LayoutManager = LayoutManager.Current
                        'First layout order should be 1 because Model is 0
                        Dim iTab As Integer = 1
                        For Each cntrl As RN_LayoutItems.RN_UCLayoutItem In flowLayouts.Controls
                            Dim oId As ObjectId = acLayoutMgr.GetLayoutId(cntrl.LayoutName)
                            Dim lay As Layout = acTrans.GetObject(oId, OpenMode.ForWrite)
                            lay.TabOrder = iTab
                            iTab += 1
                            pgbVoortgang.Value = iTab
                            pgbVoortgang.Update()
                        Next
                        acTrans.Commit()
                        acDoc.Editor.Regen()
                    End Using
                End Using
            Catch ex As Exception
                MsgBox("Fout bij het wijzigen van de volgorde!" & ex.Message & vbCrLf & ex.Source)
                Return False
            End Try
        Else
            Try
                Dim aLayouts() As String = getLayoutNames()
                If sOrder = "ASC" Then
                    aSortedLayouts = (From l In aLayouts Order By l Ascending Select l).ToArray()
                Else
                    aSortedLayouts = (From l In aLayouts Order By l Descending Select l).ToArray()
                End If
                Using acLockDoc As DocumentLock = acDoc.LockDocument
                    Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                        Dim acLayoutMgr As LayoutManager = LayoutManager.Current
                        'First layout order should be 1 because Model is 0
                        Dim iTab As Integer = 1
                        Dim sLayout As String
                        For Each sLayout In aSortedLayouts
                            Dim oId As ObjectId = acLayoutMgr.GetLayoutId(sLayout)
                            Dim lay As Layout = acTrans.GetObject(oId, OpenMode.ForWrite)
                            lay.TabOrder = iTab
                            iTab += 1
                            pgbVoortgang.Value = iTab
                            pgbVoortgang.Update()
                        Next
                        acTrans.Commit()
                        acDoc.Editor.Regen()
                    End Using
                End Using
            Catch ex As Exception
                MsgBox("Fout bij het wijzigen van de volgorde!" & ex.Message & vbCrLf & ex.Source)
                Return False
            End Try
        End If
        pgbVoortgang.Value = 0
        pgbVoortgang.Visible = False
        'reset current layout marking
        DocumentManger_DocumentLayoutSwitched()
        Return True
    End Function

    Public Function getLayoutNames()
        Dim alLayouts As ArrayList = New ArrayList()
        ' Get the layout dictionary of the current database
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
            Dim lays As DBDictionary = acTrans.GetObject(acCurDb.LayoutDictionaryId, OpenMode.ForRead)
            ' Step through and list each named layout and Model
            For Each item As DBDictionaryEntry In lays
                'add name to list except Model
                If item.Key = "Model" Then
                    'not needed
                Else
                    alLayouts.Add(item.Key)
                End If
            Next
            ' Abort the changes to the database
            acTrans.Abort()
        End Using
        Return CType(alLayouts.ToArray(GetType(String)), String())
    End Function

    Private Sub cmdSortASC_Click(sender As Object, e As EventArgs) Handles cmdSortASC.Click
        setLayoutOrder("ASC")
        loadLayouts()
    End Sub

    Private Sub cmdSortDESC_Click(sender As Object, e As EventArgs) Handles cmdSortDESC.Click
        setLayoutOrder("DESC")
        loadLayouts()
    End Sub

    Private Sub cmdSaveOrder_Click(sender As Object, e As EventArgs) Handles cmdSaveOrder.Click
        'setLayoutOrder()
    End Sub

    Public Function checkedLayouts(Optional ByVal bTrash As Boolean = False)
        Dim layouts As New List(Of Layout)()
        If iCheckCount > 0 Then
            Try
                Using acLockDoc As DocumentLock = acDoc.LockDocument
                    Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                        Dim acLayoutMgr As LayoutManager = LayoutManager.Current

                        For Each cntrl As RN_LayoutItems.RN_UCLayoutItem In flowLayouts.Controls
                            If cntrl.CheckState Then 'layout is checked, add to plot
                                Dim oId As ObjectId = acLayoutMgr.GetLayoutId(cntrl.LayoutName)
                                Dim lay As Layout = acTrans.GetObject(oId, OpenMode.ForWrite)
                                If bTrash Then
                                    'trash layout
                                    acLayoutMgr.DeleteLayout(cntrl.LayoutName)
                                End If
                                layouts.Add(lay)
                            End If
                        Next
                        acTrans.Commit()
                        Return layouts
                    End Using
                End Using
            Catch ex As Exception
                MsgBox("Fout bij het laden van de layout lijst" & ex.Message & vbCrLf & ex.Source)
                Return layouts
            End Try
        Else
            MsgBox("Er zijn geen layouts geselecteerd!")
            Return layouts
        End If
    End Function


    ''' <summary>
    ''' 'Plot multiple layouts to one multisheet PDF
    ''' </summary>
    ''' <param name="pdfSheetType"></param>
    ''' <returns></returns>
    Public Function plotLayouts(ByVal pdfSheetType As SheetType, ByVal layouts As List(Of Layout))
        Dim sOutputLocation As String = PDFoutputLocation()

        Dim db As Database = HostApplicationServices.WorkingDatabase
        Dim bgp As Short = CShort(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("BACKGROUNDPLOT"))
        Try
            Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("BACKGROUNDPLOT", 0)

            Dim filename As String
            If sOutputLocation = "" Then
                'drawing location
                filename = Path.ChangeExtension(db.Filename, "pdf")
            Else
                'user chosen location
                filename = sOutputLocation & "\" & Path.GetFileName(Path.ChangeExtension(db.Filename, "pdf"))
            End If


            Dim plotter As New plotting.MultiSheetsPdf(filename, layouts, pdfSheetType)
            plotter.Publish()

        Catch e As System.Exception
            Dim ed As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
            ed.WriteMessage(vbLf & "Error: {0}" & vbLf & "{1}", e.Message, e.StackTrace)
        Finally
            Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("BACKGROUNDPLOT", bgp)
        End Try
        Return True

    End Function



    Private Sub cmdPlotMulitSheet_Click(sender As Object, e As EventArgs) Handles cmdPlotMulitSheet.Click
        Dim layouts As New List(Of Layout)()
        layouts = checkedLayouts()
        If layouts.Count > 1 Then
            regenDrawing()
            plotLayouts(SheetType.MultiPdf, checkedLayouts())
        End If
    End Sub

    Private Sub cmdPlotSingleSheet_Click(sender As Object, e As EventArgs) Handles cmdPlotSingleSheet.Click
        Dim layouts As New List(Of Layout)()
        layouts = checkedLayouts()
        If layouts.Count > 1 Then
            regenDrawing()
            plotLayouts(SheetType.SinglePdf, checkedLayouts())
        End If
    End Sub

    Public Sub regenDrawing()
        acEd.Regen()
    End Sub

    Private Sub cmdRefreshList_Click(sender As Object, e As EventArgs) Handles cmdRefreshList.Click
        loadLayouts()
    End Sub

    Public Function itterateList(ByVal sAction As String)
        Dim CurrFound As Boolean = False
        Dim myCntrlCurrent As RN_LayoutItems.RN_UCLayoutItem
        getCurrentLayout()
        For Each myCntrl As RN_LayoutItems.RN_UCLayoutItem In flowLayouts.Controls
            If (myCntrl.IsModel = False) And (myCntrl.Visible = True) Then 'model can not be selected and item must be visible
                If myCntrl.CheckState Then 'layout is checked
                    If sAction = "hide" Then
                        myCntrl.SetCheckState(Not myCntrl.CheckState)
                        myCntrl.Visible = False
                    End If
                    If sAction = "invert" Then
                        myCntrl.SetCheckState(Not myCntrl.CheckState)
                    End If
                Else 'layout is unchecked
                    If sAction = "select" Or sAction = "invert" Then
                        myCntrl.SetCheckState(Not myCntrl.CheckState)
                    End If
                End If

                If sAction = "iscurrent" Then
                    'active layout markeren
                    If myCntrl.LayoutName = sCurrentLayout Then
                        myCntrl.IsCurrent = True
                        myCntrlCurrent = myCntrl
                        CurrFound = True
                    Else
                        myCntrl.IsCurrent = False
                    End If
                    myCntrl.isCurrentLayout()
                End If
            End If
        Next
        If sAction = "iscurrent" Then
            If CurrFound = True Then
                scrollCurrentLayoutIntoView(myCntrlCurrent)
            Else
                'layout niet gevonden, ff herladen
                loadLayouts()
            End If
        End If
            Return True
    End Function


    Private Sub cmdHideItems_Click(sender As Object, e As EventArgs) Handles cmdHideItems.Click
        itterateList("hide")
    End Sub

    Private Sub cmdShowItems_Click(sender As Object, e As EventArgs) Handles cmdShowItems.Click
        loadLayouts()
    End Sub

    Private Sub cmdSelectAll_Click(sender As Object, e As EventArgs) Handles cmdSelectAll.Click
        itterateList("select")
    End Sub

    Private Sub cmdInvertSelection_Click(sender As Object, e As EventArgs) Handles cmdInvertSelection.Click
        itterateList("invert")
    End Sub

    Private Sub cmdTrash_Click(sender As Object, e As EventArgs) Handles cmdTrash.Click
        If MsgBox("Weet u zeker dat u deze layouts wilt verwijderen?", MsgBoxStyle.Critical + MsgBoxStyle.YesNo, "Geselecteerde layouts verwijderen?") = MsgBoxResult.Yes Then
            checkedLayouts(True)
            loadLayouts()
        End If
    End Sub

    Public Sub loadSettings()
        'check of ini bestand bestaat, zo ja: instellingen laden
        If File.Exists(sIniDir & sIniFile) Then
            'bestand bestaat, instelingen laden
            iniFile = New clsINI(sIniDir & sIniFile)
            sPDFuserFolder = iniFile.GetString("publishsettings", "outputfolder", sPDFuserFolder)
            sDefaultOutputLocation = iniFile.GetString("publishsettings", "defaultoutput", sDefaultOutputLocation)
        End If
    End Sub

    Public Function PDFoutputLocation()
        loadSettings()
        Select Case sDefaultOutputLocation
            Case "drawingfolder"
                Return ""
            Case "userlocation"
                Return sPDFuserFolder
            Case "askonplot"
                Dim myFolderBrowser As FolderBrowserDialog = New FolderBrowserDialog()
                If myFolderBrowser.ShowDialog() = DialogResult.OK Then
                    Return myFolderBrowser.SelectedPath
                Else
                    Return "cancel"
                End If
            Case Else
                Return ""
        End Select
        Return ""
    End Function

    Private Sub LayoutKopierenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LayoutKopierenToolStripMenuItem.Click
        Dim sNewLayoutName As String = InputBox("Layout naam", "Layout naam", "")
        If sNewLayoutName <> "" Then
            Dim mySubMenu As ContextMenuStrip = CType(sender, ToolStripMenuItem).Owner
            Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(mySubMenu.Tag, RN_LayoutItems.RN_UCLayoutItem)
            ModifyLayout("kopieren", myCntrl, sNewLayoutName)
            loadLayouts()
            setLayoutCurrent(sNewLayoutName, False)
        End If
    End Sub

    Private Sub LayoutVerwijderenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LayoutVerwijderenToolStripMenuItem.Click
        If MsgBox("Weet u zeker dat u deze layouts wilt verwijderen?", MsgBoxStyle.Critical + MsgBoxStyle.YesNo, "Geselecteerde layouts verwijderen?") = MsgBoxResult.Yes Then
            Dim mySubMenu As ContextMenuStrip = CType(sender, ToolStripMenuItem).Owner
            Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(mySubMenu.Tag, RN_LayoutItems.RN_UCLayoutItem)
            ModifyLayout("verwijderen", myCntrl)
            loadLayouts()
        End If
    End Sub

    Public Function ModifyLayout(ByVal sAction As String, ByVal myCntrl As RN_LayoutItems.RN_UCLayoutItem, Optional ByVal sNewName As String = "")
        Try

            Using acLockDoc As DocumentLock = acDoc.LockDocument
                Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                    Dim acLayoutMgr As LayoutManager = LayoutManager.Current
                    Dim iTabs As Integer = acLayoutMgr.LayoutCount()

                    Dim oId As ObjectId = acLayoutMgr.GetLayoutId(myCntrl.LayoutName)
                    Dim lay As Layout = acTrans.GetObject(oId, OpenMode.ForWrite)

                    If sAction = "verwijderen" Then
                        'trash layout
                        acLayoutMgr.DeleteLayout(myCntrl.LayoutName)
                    ElseIf sAction = "kopieren" Then
                        'layout kopieren
                        If LayoutExists(sNewName) = False Then
                            'acLayoutMgr.CopyLayout(myCntrl.LayoutName, sNewName)
                            'Dim iTabIndex = myCntrl.TabIndex + 1
                            acLayoutMgr.CloneLayout(myCntrl.LayoutName, sNewName, iTabs)
                        End If
                    End If

                    acTrans.Commit()
                End Using
                acEd.Regen()
            End Using
        Catch ex As Exception
            MsgBox("Fout bij het " & sAction & " van de layout " & ex.Message & vbCrLf & ex.Source)
            Return False
        End Try
        Return True
    End Function

    Public Function LayoutExists(ByVal sLayName As String)
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
            Dim lays As DBDictionary = acTrans.GetObject(acCurDb.LayoutDictionaryId, OpenMode.ForRead)
            If lays.Contains(sLayName) Then
                MsgBox("Er bestaat al een layout met deze naam!" & vbCrLf & "Kies een andere naam")
                Return True
            End If
            acTrans.Commit()
        End Using
        Return False
    End Function

    Private Sub SubMenu_Opening(sender As Object, e As CancelEventArgs) Handles SubMenu.Opening
        Me.SubMenu.Tag = CType(Me.SubMenu.SourceControl, RN_LayoutItems.RN_UCLayoutItem)
        'disable rename batch if no items selected
        If iCheckCount > 1 Then
            mnuItmRenameSelection.Enabled = True
        Else
            mnuItmRenameSelection.Enabled = False
        End If
    End Sub

    Public Function selectExternalTemplate(ByVal sender As Object, ByVal e As EventArgs)
        Dim selectedItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        sLayoutTemplate = selectedItem.Text
        loadExternalTemplate()
    End Function

    Public Function loadExternalTemplate()
        If File.Exists(sLayoutTemplate) Then
            Dim acExDb As Database = New Database(False, True)
            acExDb.ReadDwgFile(sLayoutTemplate, FileOpenMode.OpenForReadAndAllShare, True, "")

            ' Get the layout dictionary of the current database
            Dim layAndTab As SortedDictionary(Of Integer, String) = New SortedDictionary(Of Integer, String)
            Dim layAndTabOID As SortedDictionary(Of Integer, ObjectId) = New SortedDictionary(Of Integer, ObjectId)
            Try
                Using trx As Transaction = acExDb.TransactionManager.StartTransaction()
                    Dim layDict As DBDictionary = acExDb.LayoutDictionaryId.GetObject(OpenMode.ForRead)
                    For Each entry As DBDictionaryEntry In layDict
                        Dim lay As Layout = CType(entry.Value.GetObject(OpenMode.ForRead), Layout)
                        layAndTab.Add(lay.TabOrder, lay.LayoutName)
                    Next
                    trx.Commit()
                End Using
            Catch ex As Exception
                MsgBox("Fout bij het laden van de Layouts uit de Template " & ex.Message)
            End Try
            Try
                cmbNewLayout.Items.Clear()
                For Each sLayoutName In layAndTab.Values
                    'add name to list except Model
                    If sLayoutName = "Model" Then

                    Else
                        cmbNewLayout.Items.Add(sLayoutName)
                    End If
                Next
                cmbNewLayout.Visible = True
                cmdAddLayout.Visible = True
            Catch ex As Exception
                MsgBox("Fout bij het weergeven van de Layouts uit de Template " & ex.Message)
            End Try
            Return True
        Else
            cmbNewLayout.Visible = False
            cmdAddLayout.Visible = False
            Return False
        End If
    End Function

    Private Sub cmdAddLayout_Click(sender As Object, e As EventArgs) Handles cmdAddLayout.Click
        If cmbNewLayout.Text.Length = 0 Then
            MsgBox("Selecteer eerst een layout!")
            Exit Sub
        End If
        Dim sNewLayoutName As String = InputBox("Layout naam", "Layout naam", cmbNewLayout.Text)
        If LayoutExists(sNewLayoutName) = False Then

            If sNewLayoutName <> "" Then
                If File.Exists(sLayoutTemplate) Then
                    Using acLockDoc As DocumentLock = acDoc.LockDocument
                        Dim acExDb As Database = New Database(False, True)
                        acExDb.ReadDwgFile(sLayoutTemplate, FileOpenMode.OpenForReadAndAllShare, True, "")
                        ' Get the layout dictionary of the current database
                        Dim layAndTab As SortedDictionary(Of Integer, String) = New SortedDictionary(Of Integer, String)
                        Dim layAndTabOID As SortedDictionary(Of Integer, ObjectId) = New SortedDictionary(Of Integer, ObjectId)
                        Try
                            Using acTransEx As Transaction = acExDb.TransactionManager.StartTransaction()
                                Dim layoutsEx As DBDictionary = acExDb.LayoutDictionaryId.GetObject(OpenMode.ForRead)

                                If layoutsEx.Contains(cmbNewLayout.Text) Then
                                    Dim layEx As Layout = layoutsEx.GetAt(cmbNewLayout.Text).GetObject(OpenMode.ForRead)
                                    Dim blkBlkRecEx As BlockTableRecord = acTransEx.GetObject(layEx.BlockTableRecordId, OpenMode.ForRead)

                                    Dim idCol As ObjectIdCollection = New ObjectIdCollection()
                                    For Each id As ObjectId In blkBlkRecEx
                                        idCol.Add(id)
                                    Next
                                    'invoegen in huidige dwg
                                    Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                                        Dim blkTbl As BlockTable = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite)

                                        Using blkBlkRec As New BlockTableRecord
                                            blkBlkRec.Name = "*Paper_Space" & CStr(layoutsEx.Count() - 1)
                                            blkTbl.Add(blkBlkRec)
                                            acTrans.AddNewlyCreatedDBObject(blkBlkRec, True)
                                            acExDb.WblockCloneObjects(idCol,
                                                                      blkBlkRec.ObjectId,
                                                                      New IdMapping(),
                                                                      DuplicateRecordCloning.Ignore,
                                                                      False)

                                            ' Create a new layout and then copy properties between drawings
                                            Dim layouts As DBDictionary =
                                                acTrans.GetObject(acCurDb.LayoutDictionaryId, OpenMode.ForWrite)

                                            Using lay As New Layout
                                                lay.LayoutName = sNewLayoutName
                                                lay.AddToLayoutDictionary(acCurDb, blkBlkRec.ObjectId)
                                                acTrans.AddNewlyCreatedDBObject(lay, True)
                                                lay.CopyFrom(layEx)

                                                Dim plSets As DBDictionary =
                                                    acTrans.GetObject(
                                                        acCurDb.PlotSettingsDictionaryId,
                                                        OpenMode.ForRead)

                                                ' Check to see if a named page setup was assigned to the layout,
                                                ' if so then copy the page setup settings
                                                If lay.PlotSettingsName <> "" Then

                                                    ' Check to see if the page setup exists
                                                    If plSets.Contains(lay.PlotSettingsName) = False Then
                                                        plSets.UpgradeOpen()

                                                        Using plSet As New PlotSettings(lay.ModelType)
                                                            plSet.PlotSettingsName = lay.PlotSettingsName
                                                            plSet.AddToPlotSettingsDictionary(acCurDb)
                                                            acTrans.AddNewlyCreatedDBObject(plSet, True)

                                                            Dim plSetsEx As DBDictionary =
                                                                acTransEx.GetObject(
                                                                    acExDb.PlotSettingsDictionaryId,
                                                                    OpenMode.ForRead)

                                                            Dim plSetEx As PlotSettings =
                                                                plSetsEx.GetAt(
                                                                    lay.PlotSettingsName).GetObject(
                                                                    OpenMode.ForRead)

                                                            plSet.CopyFrom(plSetEx)
                                                        End Using
                                                    End If
                                                End If
                                            End Using
                                        End Using
                                        ' Regen the drawing to get the layout tab to display
                                        acDoc.Editor.Regen()

                                        ' Save the changes made
                                        acTrans.Commit()
                                    End Using
                                    setLayoutCurrent(sNewLayoutName, False)
                                    loadLayouts()
                                Else
                                    MsgBox("Layout " & cmbNewLayout.Text & " kon niet worden ingevoegd!")
                                End If
                                acTransEx.Abort()
                            End Using
                        Catch ex As Exception
                            MsgBox("Fout bij het laden van de Layouts uit de Template " & ex.Message)
                        End Try
                    End Using

                Else
                    MsgBox("Kan de layout niet invoegen, Template bestand is niet gevonden!" & vbCrLf & "Het bestand: " & sLayoutTemplate & " is niet gevonden", MsgBoxStyle.Critical)
                End If
            End If
        End If
    End Sub

    Private Sub cmdBatchAttributes_Click(sender As Object, e As EventArgs) Handles cmdBatchAttributes.Click
        cmdBatchAttributes.Enabled = False
        cmdReplaceAttrib.Visible = True
        cmdReplaceAttrib.Dock = DockStyle.Fill
        cmdReplaceAttrib.BringToFront()
        cmdCancel.Visible = True
        cmdCancel.BringToFront()
        'copy visible layouts to dictionary
        layAndTabTemp = New Dictionary(Of String, Integer)
        For Each myCntr As RN_LayoutItems.RN_UCLayoutItem In flowLayouts.Controls
            If myCntr.Visible Then
                'only add visible controls
                Dim iSelected As Integer = CInt(myCntr.CheckState)
                Dim sName As String = myCntr.LayoutName
                layAndTabTemp.Add(sName, iSelected)
            End If
        Next
        flowLayouts.Controls.Clear()


        'set focus to modal space
        Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

        Using acLockDoc As DocumentLock = acDoc.LockDocument()
            '' Start a transaction
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                Try
                    Dim opts As PromptEntityOptions = New PromptEntityOptions("Selecteer een Block: ")
                    opts.SetRejectMessage("Alleen block toegestaan!")
                    opts.AddAllowedClass(GetType(BlockReference), True)
                    Dim promptSS As PromptEntityResult
                    promptSS = acEd.GetEntity(opts)

                    If promptSS.Status = PromptStatus.OK Then
                        oBlockID = promptSS.ObjectId
                        Dim blkRef As BlockReference = TryCast(acTrans.GetObject(oBlockID, OpenMode.ForRead), BlockReference)
                        Dim btr As BlockTableRecord = TryCast(acTrans.GetObject(blkRef.BlockTableRecord, OpenMode.ForRead), BlockTableRecord)
                        sBlockName = btr.Name
                        Dim colATT As Autodesk.AutoCAD.DatabaseServices.AttributeCollection = blkRef.AttributeCollection
                        For Each oAttID As ObjectId In colATT
                            'attributes doorlopen
                            Dim refATT As AttributeReference = TryCast(acTrans.GetObject(oAttID, OpenMode.ForRead), AttributeReference)
                            Dim attListItem As RN_attribute_listitem.AttributeListItem = New RN_attribute_listitem.AttributeListItem
                            attListItem.AttribName = refATT.Tag
                            flowLayouts.Controls.Add(attListItem)
                        Next
                    Else
                        'geen block geselecteerd
                    End If

                Catch ex As Autodesk.AutoCAD.Runtime.Exception
                    MsgBox(ex.Message)
                End Try
                acTrans.Commit()
            End Using
            If flowLayouts.Controls.Count < 0 Then
                'reset items
                loadLayouts(True)
                cmdBatchAttributes.Enabled = True
                cmdReplaceAttrib.Visible = False
            End If
        End Using
    End Sub


    Private Sub cmdReplaceAttrib_Click(sender As Object, e As EventArgs) Handles cmdReplaceAttrib.Click
        pgbVoortgang.Visible = True
        Dim iItmCount As Integer = 0
        For Each attListItem As RN_attribute_listitem.AttributeListItem In flowLayouts.Controls
            If attListItem.IsItemSelected Then
                'item is geselecteerd
                If attListItem.AttribNewValue.Length > 0 Then
                    'alleen verwerken indien groter dan 0
                    sAttribNames(iItmCount) = attListItem.AttribName.ToUpper
                    sAttribValue(iItmCount) = attListItem.AttribNewValue
                    bAutoNumber(iItmCount) = attListItem.AutoNumber
                    If Not attListItem.StartValue.ToString.Length = 0 Then
                        dStartValue(iItmCount) = attListItem.StartValue
                    Else
                        dStartValue(iItmCount) = 0
                    End If
                    dCurrValue(iItmCount) = dStartValue(iItmCount)
                    If Not attListItem.IncrementValue.ToString.Length = 0 Then
                        dIncrementValue(iItmCount) = attListItem.IncrementValue
                    Else
                        dIncrementValue(iItmCount) = 1
                    End If
                    iItmCount += 1
                End If
            End If
        Next
        'layouts doorlopen
        'layout lijst herladen
        loadLayouts(True)
        cmdReplaceAttrib.Visible = False
        cmdReplaceAttrib.Update()
        cmdCancel.Visible = False
        Me.Update()
        Dim iPaperSpaceCount As Integer = 0
        'LayoutWalker(iPaperSpaceCount)
        LayoutWalker2()

        'rest items
        pgbVoortgang.Visible = False
        'loadLayouts(True)
        cmdBatchAttributes.Enabled = True
        pgbVoortgang.Visible = False
        resetReplaceAttribButtons()
        MsgBox("Bijwerken van tekeninghoofd is voltooid!")
    End Sub

    Public Function getSpaceID(ByVal sSpace As String) As ObjectId
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

    Public Function LayoutWalker2()
        Dim layoutMgr As LayoutManager = LayoutManager.Current
        sActiveLayout = layoutMgr.CurrentLayout
        'check of er ook layouts geselecteerd zijn, zo ja vragen of alleen de geselecteerde layouts verwerkt moeten worden
        Dim bProcessChecked As Boolean = False
        If iCheckCount > 0 Then
            Dim sCheckResponse As MsgBoxResult = MsgBox("Wilt u de geselecteerde layouts verwerken (JA) of alle layouts in de lijst (NEE)", MsgBoxStyle.YesNoCancel)
            Select Case sCheckResponse
                Case MsgBoxResult.Yes
                    bProcessChecked = True

                Case MsgBoxResult.No
                    bProcessChecked = False

                Case MsgBoxResult.Cancel
                    Return False
            End Select
        End If
        updateProgressBar(False, iCheckCount + 1, 0)
        For Each myCntrl As RN_LayoutItems.RN_UCLayoutItem In flowLayouts.Controls
            If myCntrl.LayoutName = "Model" Then
                'modal overslaan
            Else
                'enkel de in de lijst zichtbare items verwerken
                If myCntrl.Visible Then
                    If bProcessChecked Then
                        'alleen de geselecteerde layouts
                        If myCntrl.CheckState Then
                            '## layout active zetten
                            Dim sLayoutName As String = myCntrl.LayoutName
                            setLayoutCurrent(sLayoutName, True) 'ismodal op true zodat we niet alle viewports lang hoeven
                            '## paperspace id pakken en attributes updaten
                            updateProgressBar(True)
                            UpdateAttributesInBlock(getSpaceID("paper"), sBlockName)
                        End If
                    Else
                        '## layout active zetten
                        Dim sLayoutName As String = myCntrl.LayoutName
                        setLayoutCurrent(sLayoutName, True) 'ismodal op true zodat we niet alle viewports lang hoeven
                        '## paperspace id pakken en attributes updaten
                        updateProgressBar(True)
                        UpdateAttributesInBlock(getSpaceID("paper"), sBlockName)
                    End If
                End If
            End If
        Next
        'originele layout terugzetten
        setLayoutCurrent(sActiveLayout, True)
        Return True
    End Function

    Public Function updateProgressBar(bIncrement As Boolean, Optional iMax As Integer = 0, Optional iValue As Integer = 9999999)
        If iMax > 0 Then
            pgbVoortgang.Maximum = iMax
        End If
        If iValue < 9999999 Then
            pgbVoortgang.Value = iValue
        End If
        If bIncrement Then
            If (pgbVoortgang.Value + 1) > pgbVoortgang.Maximum Then
                pgbVoortgang.Maximum = pgbVoortgang.Maximum + 2
            End If
            pgbVoortgang.Value = pgbVoortgang.Value + 1
        End If
    End Function

    Public Function LayoutWalker(ByRef iPaperSpaceCount As Integer)
        Dim doc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database
        Dim ed As Editor = doc.Editor
        Dim layoutMgr As LayoutManager = LayoutManager.Current
        Dim iLayoutCnt As Integer = 1
        Dim iLayoutCntMax As Integer = layoutMgr.LayoutCount
        sActiveLayout = layoutMgr.CurrentLayout
        pgbVoortgang.Value = 0
        pgbVoortgang.Maximum = iLayoutCntMax
        Using acLockDoc As DocumentLock = doc.LockDocument()
            Using tr As Transaction = db.TransactionManager.StartTransaction()
                Dim layoutDic As DBDictionary = TryCast(tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead, False), DBDictionary)
                For Each entry As DBDictionaryEntry In layoutDic
                    Dim layoutId As ObjectId = entry.Value
                    Dim layout As Layout = TryCast(tr.GetObject(layoutId, OpenMode.ForRead), Layout)
                    '## layout active zetten
                    layoutMgr.CurrentLayout = layout.LayoutName
                    '## paperspace id pakken en attributes updaten
                    iPaperSpaceCount = UpdateAttributesInBlock(getSpaceID("paper"), sBlockName)
                    iLayoutCnt = iLayoutCnt + 1
                Next
                'originele layout terugzetten
                layoutMgr.CurrentLayout = sActiveLayout
                tr.Commit()
            End Using
        End Using
        Return True
    End Function


    Public Function UpdateAttributesInBlock(ByVal oBtrId As ObjectId, ByVal sBlockName As String, Optional bRecursive As Boolean = False) As Integer
        Dim iChangedCount As Integer = 0
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
        Using acLockDoc As DocumentLock = acDoc.LockDocument()
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                Dim btr As BlockTableRecord = TryCast(acTrans.GetObject(oBtrId, OpenMode.ForRead), BlockTableRecord)
                If bRecursive = False Then
                    pgbVoortgang.Value = 0
                    pgbVoortgang.Maximum = btr.AcadObject.count
                End If
                For Each oEntId As ObjectId In btr
                    Dim ent As Entity = TryCast(acTrans.GetObject(oEntId, OpenMode.ForRead), Entity)
                    If ent IsNot Nothing Then
                        Dim br As BlockReference = TryCast(ent, BlockReference)
                        If br IsNot Nothing Then
                            Dim bd As BlockTableRecord = DirectCast(acTrans.GetObject(br.BlockTableRecord, OpenMode.ForRead), BlockTableRecord)
                            If bd.Name.ToUpper = sBlockName.ToUpper Then
                                ' Check each of the attributes...
                                For Each arId As ObjectId In br.AttributeCollection
                                    Dim obj As DBObject = acTrans.GetObject(arId, OpenMode.ForRead)
                                    Dim ar As AttributeReference = TryCast(obj, AttributeReference)
                                    If ar IsNot Nothing Then
                                        'kijken of we de tag in de array kunnen vinden
                                        Dim iItemIndex As Integer = Array.IndexOf(sAttribNames, ar.Tag.ToUpper)
                                        If iItemIndex >= 0 Then
                                            ar.UpgradeOpen()
                                            Dim sTmpValue As String = sAttribValue(iItemIndex)
                                            'kijke of we autonumber hebben
                                            If bAutoNumber(iItemIndex) = True Then
                                                sTmpValue = sTmpValue.Replace("[#nummer]", dCurrValue(iItemIndex).ToString)
                                                'sTmpValue = sTmpValue & " autonr"
                                                dCurrValue(iItemIndex) += dIncrementValue(iItemIndex)
                                            End If
                                            ar.TextString = sTmpValue
                                            ar.DowngradeOpen()
                                            iChangedCount += 1
                                        End If
                                    End If
                                Next 'for each attribute
                            End If
                            ' Recurse for nested blocks
                            iChangedCount += UpdateAttributesInBlock(br.BlockTableRecord, sBlockName, True)
                        End If
                    End If
                    If bRecursive = False Then
                        'check of de max van de progress nog toereikend is
                        If pgbVoortgang.Value = pgbVoortgang.Maximum Then
                            'maximum is bereikt, value verlagen
                            pgbVoortgang.Maximum = pgbVoortgang.Maximum + CInt(Math.Ceiling(Rnd() * (pgbVoortgang.Maximum - 1))) + 1
                        End If
                        pgbVoortgang.Value = pgbVoortgang.Value + 1
                    End If
                Next 'for each object
                acTrans.Commit()
            End Using 'transaction
        End Using 'lockdock
        Return iChangedCount
    End Function

    Private Sub cmdFilter_Click(sender As Object, e As EventArgs) Handles cmdFilter.Click
        'Dim frmFilterDlg As frmFilter = New frmFilter()
        'frmFilterDlg.ShowDialog()
        ContextMenuFilters.Show(cmdFilter, 0, 0)
    End Sub

    Public Sub ShowContextMenu(ByVal myObject As Object)

    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        resetReplaceAttribButtons()
        loadLayouts(True)
    End Sub

    Public Sub resetReplaceAttribButtons()
        cmdBatchAttributes.Enabled = True
        cmdReplaceAttrib.Visible = False
        cmdCancel.Visible = False
    End Sub

    Sub loadFilters(Optional ByVal sName As String = "", Optional ByVal bSetCurrent As Boolean = False)
        Using acLockDoc As DocumentLock = acDoc.LockDocument()
            Dim sCurrent As Integer = 0
            aFilters = New List(Of String)
            ContextMenuFilterList.Items.Clear()
            'load dict
            Dim dictLoad As Dictionary(Of String, List(Of String)) = clsFilterData.getDBDictionaryToDictionary(acDoc, acCurDb, acEd, "FILTERSETTINGS")
            If dictLoad Is Nothing Then
                Exit Sub
            End If
            For Each pair As KeyValuePair(Of String, List(Of String)) In dictLoad
                Dim mItem As New ToolStripMenuItem()
                mItem.Text = pair.Key.Substring(2)
                aFilters.Add(pair.Key.Substring(2))
                mItem.Name = pair.Key
                Dim custImg As Drawing.Bitmap
                If pair.Key.Substring(0, 1) = "1" Then
                    'visible items
                    custImg = My.Resources.icon_light_on_2
                Else
                    'checked items
                    custImg = My.Resources.icon_check
                End If
                mItem.Image = custImg
                AddHandler mItem.Click, AddressOf SelectFilter
                ContextMenuFilterList.Items.Add(mItem)
                If bSetCurrent Then
                    If pair.Key = sName Then
                        txtFilter.Text = pair.Key.Substring(2)
                        pcbIconFilter.BackgroundImage = custImg
                    End If
                End If
            Next
        End Using
    End Sub

    Sub resetFilter()
        ContextMenuFilterList.Items.Clear()
        txtFilter.Text = ""
        pcbIconFilter.BackgroundImage = Nothing
    End Sub


    Sub SelectFilter(ByVal sender As Object, ByVal e As EventArgs)
        Dim selectedItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        txtFilter.Text = selectedItem.Text
        pcbIconFilter.BackgroundImage = selectedItem.Image

        Dim iFilterType As Integer = CInt(selectedItem.Name.ToString.Substring(0, 1))

        'filter toepassen op lijst
        Dim dict As Dictionary(Of String, List(Of String)) = clsFilterData.getFilterFromDictionary(acDoc, acCurDb, acEd, "FILTERSETTINGS", txtFilter.Text)
        Dim aSelectedFilter As New List(Of String)
        For Each pair As KeyValuePair(Of String, List(Of String)) In dict
            For Each s As String In pair.Value
                aSelectedFilter.Add(s)
            Next
        Next
        For Each myCntrl As RN_LayoutItems.RN_UCLayoutItem In flowLayouts.Controls
            If (myCntrl.IsModel = False) Then
                Dim sObjectHandle As String = myCntrl.LayoutHandle
                Select Case iFilterType
                    Case 0 'selected items
                        If aSelectedFilter.Contains(sObjectHandle) Then
                            'item zit in het filter
                            myCntrl.Visible = True
                            myCntrl.SetCheckState(True)
                        Else
                            myCntrl.SetCheckState(False)
                        End If

                    Case 1 'visible items
                        If aSelectedFilter.Contains(sObjectHandle) Then
                            'item zit in het filter
                            myCntrl.Visible = True
                            myCntrl.SetCheckState(False)
                        Else
                            myCntrl.Visible = False
                            myCntrl.SetCheckState(False)
                        End If
                End Select
            End If
        Next
    End Sub

    Sub saveNewFilter(ByVal sType As String, ByVal sName As String)
        Using acLockDoc As DocumentLock = acDoc.LockDocument()
            'strip unwanted chars
            sName = Regex.Replace(sName, "[^A-Za-z0-9\- ]", "")
            If aFilters.Contains(sName) Then
                MsgBox("Er bestaat al een filter met deze naam, geef een andere naam op!")
                Exit Sub
            End If
            Dim dict As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))
            Dim dictLoad As Dictionary(Of String, List(Of String)) = clsFilterData.getDBDictionaryToDictionary(acDoc, acCurDb, acEd, "FILTERSETTINGS")

            If dictLoad Is Nothing Then
                dictLoad = New Dictionary(Of String, List(Of String))
            ElseIf dictLoad.Count = 0 Then
                dictLoad = New Dictionary(Of String, List(Of String))
            End If

            Dim val As List(Of String) = New List(Of String)
            Try
                For Each myCntrl As RN_LayoutItems.RN_UCLayoutItem In flowLayouts.Controls
                    If (myCntrl.IsModel = False) And (myCntrl.Visible = True) Then 'model can not be selected and item must be visible
                        Select Case sType
                            Case "selected"
                                If myCntrl.CheckState Then 'layout is checked
                                    val.Add(myCntrl.LayoutHandle)
                                End If

                            Case "visible"
                                val.Add(myCntrl.LayoutHandle)
                        End Select
                    End If
                Next
                'MsgBox("add to dict")
                dictLoad.Add(sName, val)
            Catch ex As Exception
                MsgBox("Fout bij het aanmaken van het filter!" & vbCrLf & ex.Message & ex.InnerException.ToString)
            End Try
            'save dict
            If clsFilterData.saveFilter(acDoc, acCurDb, acEd, dictLoad, "FILTERSETTINGS") Then
                MsgBox("Filter is toegeveogd!")
            Else
                MsgBox("Fout bij het toevoegen van het filter!")
            End If
            loadFilters(sName, True)
        End Using
    End Sub

    Private Sub GeselecteerdeItemsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GeselecteerdeItemsToolStripMenuItem.Click
        Dim sFilterName As String = InputBox("Filternaam", "Filternaam", "Filter" & DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss").Replace(":", "-"))
        If sFilterName.Length = 0 Then
            Exit Sub
        End If
        saveNewFilter("selected", "0-" & sFilterName)
    End Sub

    Private Sub cmdFilter_MouseDown(sender As Object, e As MouseEventArgs) Handles cmdFilter.MouseDown
        If e.Button = MouseButtons.Right Then
            loadFilters()
        End If
    End Sub

    Private Sub FilterVanZichtbareItemsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FilterVanZichtbareItemsToolStripMenuItem.Click
        Dim sFilterName As String = InputBox("Filternaam", "Filternaam", "Filter" & DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss").Replace(":", "-"))
        If sFilterName.Length = 0 Then
            Exit Sub
        End If
        saveNewFilter("visible", "1-" & sFilterName)
    End Sub

    Private Sub GeselecteerdFilterVerwijderenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GeselecteerdFilterVerwijderenToolStripMenuItem.Click
        If txtFilter.Text.Length > 0 Then
            If MsgBox("Filter " & txtFilter.Text & " zeker verwijderen?", MsgBoxStyle.Critical + MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                clsFilterData.delItemFromDictionary(acDoc, acCurDb, acEd, "FILTERSETTINGS", txtFilter.Text)
                loadFilters()
            End If
        End If
    End Sub

    Private Sub txtFilter_Click(sender As Object, e As EventArgs) Handles txtFilter.Click
        ContextMenuFilterList.Show(txtFilter, 0, 0)
    End Sub

    Private Sub mnuItmRenameSelection_Click(sender As Object, e As EventArgs) Handles mnuItmRenameSelection.Click
        'rename selected layouts
        Dim renameOptions As New frmRenameLayouts
        If renameOptions.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Dim sNewName As String = renameOptions.txtLayoutNaam.Text
            Dim dAutoNr As Double = CDbl(renameOptions.txtAutoNummer.Value)
            Dim bAutoNr As Boolean = renameOptions.chkAutoNummer.Checked
            Dim dConflictNr As Double = 0
            Dim bResolveConflict As Boolean 'true (add autonumber on conflic) / false (skip rename)
            If renameOptions.radioLayoutNameExists.Checked = True Then
                bResolveConflict = True
            Else
                bResolveConflict = False
            End If
            Try
                For Each myCntrl As RN_LayoutItems.RN_UCLayoutItem In flowLayouts.Controls
                    If (myCntrl.IsModel = False) And (myCntrl.Visible = True) And (myCntrl.CheckState = True) Then 'model can not be selected and item must be visible and checked
                        'change layout name
                        Dim sLayoutNewName As String
                        myCntrl.LayoutNameOld = myCntrl.LayoutName
                        If bAutoNr = True Then
                            sLayoutNewName = sNewName.Replace("[#nummer]", CStr(dAutoNr))
                            dAutoNr = dAutoNr + 1
                        Else
                            sLayoutNewName = sNewName
                        End If
                        'check of we wildcard replacement moeten doen
                        If sLayoutNewName.Contains("*") Then
                            sLayoutNewName = sLayoutNewName.Replace("*", myCntrl.LayoutNameOld)
                        End If
                        myCntrl.LayoutName = sLayoutNewName
                        Using acLockDoc As DocumentLock = acDoc.LockDocument
                            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                                Dim lays As DBDictionary = acTrans.GetObject(acCurDb.LayoutDictionaryId, OpenMode.ForRead)
                                If lays.Contains(myCntrl.LayoutName) Then
                                    'nieuwe naam bestaat al, kijken hoe dit conflict op te lossen
                                    If bResolveConflict = True Then
                                        'conflict oplossen door volgnummer toe te voegen
                                        myCntrl.LayoutName = myCntrl.LayoutName & "-" & CStr(dConflictNr)
                                        dConflictNr = dConflictNr + 1
                                        If lays.Contains(myCntrl.LayoutNameOld) Then
                                            'rename layout
                                            renameLayoutByValue(myCntrl.LayoutName, myCntrl.LayoutNameOld)
                                        Else
                                            'te hernoemen layout bestaat niet overslaan dus
                                            myCntrl.LayoutName = myCntrl.LayoutNameOld

                                        End If
                                    Else
                                        'niets doen rename overslaan
                                        myCntrl.LayoutName = myCntrl.LayoutNameOld
                                    End If
                                    myCntrl.updateItem()
                                Else
                                    'nieuwe naam bestaat niet, hernoemen
                                    If lays.Contains(myCntrl.LayoutNameOld) Then
                                        'rename layout
                                        renameLayoutByValue(myCntrl.LayoutName, myCntrl.LayoutNameOld)
                                    Else
                                        'te hernoemen layout bestaat niet overslaan dus
                                        myCntrl.LayoutName = myCntrl.LayoutNameOld
                                    End If
                                    myCntrl.updateItem()
                                End If
                                acTrans.Commit()
                            End Using
                        End Using
                    End If
                Next
            Catch ex As Exception
                MsgBox("Fout bij het hernoemen van de geselecteerde layouts!" & vbCrLf & ex.Message & ex.InnerException.ToString)
            End Try
        Else
            'rename is geannuleerd
            MsgBox("annuleren")
        End If
        loadLayouts()
        MsgBox("Hernoemen van de Layouts is voltooid!")
    End Sub


    Private Sub cmbNewLayout_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles cmbNewLayout.MouseDoubleClick
        'bestand bestaat, instelingen laden
        iniFile = New clsINI(sIniDir & sIniFile)
        sLayoutTemplate = iniFile.GetString("template", "layout", sLayoutTemplate)
        If sLayoutTemplate.Length > 0 Then
            loadExternalTemplate()
        End If
    End Sub
End Class
