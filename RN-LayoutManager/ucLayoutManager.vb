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

Public Class ucLayoutManager
    Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
    Dim acCurDb As Database = acDoc.Database
    Dim acEd As Editor = acDoc.Editor
    Dim iCheckCount As Integer = 0 'counter for checks
    Dim drag_drop_scroll_amount As Integer = 0
    Dim sIniDir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\RNtools"
    Dim sIniFile As String = "\layoutmanager.ini"
    Dim iniFile As clsINI
    Dim sPDFuserFolder As String
    Dim sDefaultOutputLocation As String = ""
    Dim sCurrVersion As String = Assembly.GetExecutingAssembly().GetName().Version.ToString
    Dim sCurrentLayout As String
    'Active Drawing Tracking
    Private Shared m_DocData As clsMyDocData = New clsMyDocData
    Dim AcApp As Autodesk.AutoCAD.ApplicationServices.Application



    '### Active Drawing Tracking
    Private Sub DocumentManager_DocumentActivated(ByVal sender As Object, ByVal e As DocumentCollectionEventArgs)
        'display the current active document

        'If Not m_DocData Is Nothing Then
        'm_Container2.txtADwg.Text = m_DocData.Current.Stuff
        Try
            'remap vars to current document
            acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
            acCurDb = acDoc.Database
            acEd = acDoc.Editor
            loadLayouts()
            'Todo: implement load saved selection
        Catch
            'MsgBox("Probleem bij DocumentActivated")
        End Try
    End Sub
    Private Sub DocumentManager_DocumentToBeDeactivated(ByVal sender As Object, ByVal e As DocumentCollectionEventArgs)
        'store the current contents
        'If Not m_DocData Is Nothing Then
        'm_DocData.Current.Stuff = m_Container2.txtADwg.Text
        Try
            'Todo: implement save selection
            'remap vars to current document
            acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
            acCurDb = acDoc.Database
            acEd = acDoc.Editor
            resetList()
        Catch
            'MsgBox("Probleem bij DocumentToBeDactivated")
        End Try
    End Sub

    Private Sub DocumentManger_DocumentLayoutSwitched()
        'sub layout switched
        Dim acLayout As Layout = Nothing
        Using acLockDoc As DocumentLock = acDoc.LockDocument
            Dim acLayoutMgr As LayoutManager = LayoutManager.Current
            sCurrentLayout = acLayoutMgr.CurrentLayout
        End Using
        itterateList("iscurrent")
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
        AddHandler Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LayoutSwitched, AddressOf Me.DocumentManger_DocumentLayoutSwitched
        '### /Active Drawing Tracking
        loadLayouts()
        'set active layout
        DocumentManger_DocumentLayoutSwitched()
        'tool toevoegen aan trusted path
        getTrustedPaths(clsFunctions.getCoreDir())
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
    Public Sub loadLayouts()
        'reset and clear list
        'reset check count
        iCheckCount = 0
        updateCheckLabel()
        'clear flow
        flowLayouts.Controls.Clear()
        flowLayouts.AllowDrop = True
        flowLayouts.AutoScroll = True
        flowLayouts.SetAutoScrollMargin(5, 5)
        'first add model item
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = New RN_LayoutItems.RN_UCLayoutItem()
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
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
            Dim lays As DBDictionary = acTrans.GetObject(acCurDb.LayoutDictionaryId, OpenMode.ForRead)

            ' Step through and list each named layout and Model
            For Each item As DBDictionaryEntry In lays
                'add name to list except Model
                If item.Key = "Model" Then

                Else
                    myCntrl = New RN_LayoutItems.RN_UCLayoutItem()
                    myCntrl.LayoutName = item.Key
                    myCntrl.updateItem()
                    'add handlers to register functions for items
                    AddHandler myCntrl.View_Click, AddressOf ItemViewClick
                    AddHandler myCntrl.LayoutNameEdit_KeyDown, AddressOf renameLayout
                    AddHandler myCntrl.Plot_Click, AddressOf PlotLayout
                    AddHandler myCntrl.Plot_CheckedChanged, AddressOf PlotCheck
                    AddHandler myCntrl.MouseMove, AddressOf item_MouseMove
                    AddHandler myCntrl.DragEnter, AddressOf item_DragEnter
                    myCntrl.ContextMenuStrip = SubMenu
                    flowLayouts.Controls.Add(myCntrl)
                End If
            Next
            ' Abort the changes to the database
            acTrans.Abort()
        End Using
    End Sub
    ''' <summary>
    ''' 'Rename layout function
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <returns></returns>
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
        Using acLockDoc As DocumentLock = acDoc.LockDocument

            Dim acLayoutMgr As LayoutManager = LayoutManager.Current
            acLayoutMgr.CurrentLayout = myCntrl.LayoutName
            acDoc.Editor.Regen()
            'zoom extends when not model view
            If myCntrl.IsModel = False Then
                Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                    Dim oId As ObjectId = acLayoutMgr.GetLayoutId(myCntrl.LayoutName)
                    Dim lay As Layout = acTrans.GetObject(oId, OpenMode.ForWrite)
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

    ''' <summary>
    ''' Move item on drag
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
            'save new order
            setLayoutOrder()
        End If
    End Sub

    ''' <summary>
    ''' Detect Drag
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
        End If
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
        setLayoutOrder()
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


    Private Sub flowLayouts_DragLeave(sender As Object, e As EventArgs) Handles flowLayouts.DragLeave
        Dim iBegY As Integer = Me.flowLayouts.FindForm().PointToClient(Me.flowLayouts.Parent.PointToScreen(Me.flowLayouts.Location)).Y
        Dim iFlowBoundY As Integer = Me.flowLayouts.Height + iBegY
        Dim iMouseY As Integer = Me.flowLayouts.FindForm().PointToClient(MousePosition).Y

        While iMouseY >= iFlowBoundY
            flowLayouts.VerticalScroll.Value = flowLayouts.VerticalScroll.Value + drag_drop_scroll_amount
            iMouseY = flowLayouts.FindForm().PointToClient(MousePosition).Y
            flowLayouts.Refresh()
        End While

        While iMouseY <= iBegY
            flowLayouts.VerticalScroll.Value = flowLayouts.VerticalScroll.Value - drag_drop_scroll_amount
            iMouseY = flowLayouts.FindForm().PointToClient(MousePosition).Y
            flowLayouts.Refresh()
        End While
    End Sub

    Private Sub cmdPlotMulitSheet_Click(sender As Object, e As EventArgs) Handles cmdPlotMulitSheet.Click
        Dim layouts As New List(Of Layout)()
        layouts = checkedLayouts()
        If layouts.Count > 1 Then
            plotLayouts(SheetType.MultiPdf, checkedLayouts())
        End If
    End Sub

    Private Sub cmdPlotSingleSheet_Click(sender As Object, e As EventArgs) Handles cmdPlotSingleSheet.Click
        Dim layouts As New List(Of Layout)()
        layouts = checkedLayouts()
        If layouts.Count > 1 Then
            plotLayouts(SheetType.SinglePdf, checkedLayouts())
        End If
    End Sub

    Private Sub cmdRefreshList_Click(sender As Object, e As EventArgs) Handles cmdRefreshList.Click
        loadLayouts()
    End Sub

    Public Function itterateList(ByVal sAction As String)
        For Each myCntrl As RN_LayoutItems.RN_UCLayoutItem In flowLayouts.Controls
            If (myCntrl.IsModel = False) And (myCntrl.Visible = True) Then 'model can not be selected and item must be visible
                If myCntrl.CheckState Then 'layout is checked
                    If sAction = "hide" Then
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
                    'active layout makeren
                    If myCntrl.LayoutName = sCurrentLayout Then
                        myCntrl.IsCurrent = True
                    Else
                        myCntrl.IsCurrent = False
                    End If
                    myCntrl.isCurrentLayout()
                End If
            End If
        Next
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
            Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)
            ModifyLayout("kopieren", myCntrl, sNewLayoutName)
            loadLayouts()
        End If
    End Sub

    Private Sub LayoutVerwijderenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LayoutVerwijderenToolStripMenuItem.Click
        If MsgBox("Weet u zeker dat u deze layouts wilt verwijderen?", MsgBoxStyle.Critical + MsgBoxStyle.YesNo, "Geselecteerde layouts verwijderen?") = MsgBoxResult.Yes Then
            Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)
            ModifyLayout("verwijderen", myCntrl)
            loadLayouts()
        End If
    End Sub

    Public Function ModifyLayout(ByVal sAction As String, ByVal myCntrl As RN_LayoutItems.RN_UCLayoutItem, Optional ByVal sNewName As String = "")
        Try

            Using acLockDoc As DocumentLock = acDoc.LockDocument
                Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                    Dim acLayoutMgr As LayoutManager = LayoutManager.Current


                    Dim oId As ObjectId = acLayoutMgr.GetLayoutId(myCntrl.LayoutName)
                    Dim lay As Layout = acTrans.GetObject(oId, OpenMode.ForWrite)

                    If sAction = "verwijderen" Then
                        'trash layout
                        acLayoutMgr.DeleteLayout(myCntrl.LayoutName)
                    ElseIf sAction = "kopieren" Then
                        'layout kopieren
                        If LayoutExists(myCntrl.LayoutName) = False Then
                            acLayoutMgr.CopyLayout(myCntrl.LayoutName, sNewName)
                        End If
                    End If

                    acTrans.Commit()
                End Using
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
                MsgBox("Er bestaat al een layout met deze naam!")
                Return True
            End If
            acTrans.Commit()
        End Using
        Return False
    End Function

End Class
