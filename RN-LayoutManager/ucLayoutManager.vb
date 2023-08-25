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
Imports System.Drawing
Imports Autodesk.AutoCAD.Interop
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports Autodesk.AutoCAD.Internal.CommandPiper

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
    Dim sPlotPreferences As String = "\PlotPresets.json"
    Dim iniFile As clsINI
    Dim sPDFuserFolder As String
    Dim sDefaultOutputLocation As String = ""
    Dim bUseDWGname As Boolean = True
    Dim sCurrVersion As String = Assembly.GetExecutingAssembly().GetName().Version.ToString
    Dim sCurrentLayout As String = ""
    Dim sLayoutTemplate As String = ""
    Dim sLayoutTemplateFolder As String = ""
    'layout attributes replace
    Dim layAndTabTemp As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)
    Dim oBlockID As ObjectId
    Dim sBlockName As String
    Dim bIsProvFlevoland As Boolean = False
    Dim oModalSpace As ObjectId
    Dim oPaperSpace As ObjectId
    Dim sActiveLayout As String
    Dim sCurrLayout As String
    Dim sAttribNames(0 To 99) As String
    Dim sAttribValue(0 To 99) As String
    Dim bAutoNumber(0 To 99) As Boolean
    Dim dStartValue(0 To 99) As Double
    Dim dCurrValue(0 To 99) As Double
    Dim dIncrementValue(0 To 99) As Double
    Dim sLayoutNames As Dictionary(Of String, Dictionary(Of String, String)) = New Dictionary(Of String, Dictionary(Of String, String))
    Dim sVersie As String
    Dim sBestek As String
    Dim sBlad As String
    Dim bIsCentrumplan As Boolean = False

    Dim sBlockToFind As String
    Dim sAttribFile As String = ""
    'list item height based on DPI
    Dim dItemHeightMin As Double
    Dim dItemHeightMax As Double
    'plot vars
    Dim pstylemode As String
    Dim aPstyleLayouts As List(Of String) = New List(Of String)
    'filter values
    Dim aFilters As List(Of String)
    'dynamic viewport vars
    Dim vpCenter As Point3d
    Dim vpScale As String
    Dim vpCustScale As Double
    Dim vpCoordinates As ViewPortCoordinates
    Dim vpCoordinatesList As New ViewPortCoordinatesList

    'Active Drawing Tracking
    Private Shared m_DocData As clsMyDocData = New clsMyDocData
    Dim AcApp As Autodesk.AutoCAD.ApplicationServices.Application
    Dim _lm As LayoutManager

    'DEBUG vars
    Dim sDebugLog As String = clsFunctions.getMyDocDir() & "\RN-LayManDebugLog.log"

    '### Active Drawing Tracking
    Private Sub DocumentManager_DocumentActivated(ByVal sender As Object, ByVal e As DocumentCollectionEventArgs)
        'tekening wordt geactiveerd
        Try
            ' Set insertion units to meters

            clsFunctions.PrefsSetUnits()

            'remap vars to current document
            acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
            acCurDb = acDoc.Database
            acEd = acDoc.Editor
            _lm = LayoutManager.Current
            loadLayouts()
            'Todo: implement load saved selection
            loadFilters()
            'AddHandler Me._lm.LayoutSwitched, AddressOf Me.DocumentManger_DocumentLayoutSwitched
            AddHandler acDoc.CommandEnded, New CommandEventHandler(AddressOf commandEnd)
            pstylemode = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("PSTYLEMODE").ToString
        Catch
            'MsgBox("Probleem bij DocumentActivated")
        End Try
    End Sub
    Private Sub DocumentManager_DocumentToBeDeactivated(ByVal sender As Object, ByVal e As DocumentCollectionEventArgs)
        'switch naar een andere tekening
        Try
            'clsFunctions.makeLog(sIniDir & "\log.txt", "Handler activated DocumentManager_DocumentToBeDeactivated")
            'Todo: implement save selection
            'remap vars to current document
            acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
            acCurDb = acDoc.Database
            acEd = acDoc.Editor
            'RemoveHandler Me._lm.LayoutSwitched, AddressOf Me.DocumentManger_DocumentLayoutSwitched
            RemoveHandler acDoc.CommandEnded, AddressOf commandEnd
            resetList()
            resetFilter()
        Catch
            'MsgBox("Probleem bij DocumentToBeDactivated")
        End Try
    End Sub

    Private Sub DocumentManger_DocumentLayoutSwitched()
        'sub layout switched
        'clsFunctions.makeLog(sIniDir & "\log.txt", "Handler activated DocumentManger_DocumentLayoutSwitched")
        getCurrentLayout()
        itterateList("iscurrent")
    End Sub

    Private Sub DocumentManager_DocumentLayoutsModified()
        'getCurrentLayout()
        'clsFunctions.makeLog(sIniDir & "\log.txt", "Handler activated DocumentManager_DocumentLayoutsModified")
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
                'clsFunctions.makeLog(sIniDir & "\log.txt", "Handler CommandEnd LAYOUT_CONTROL")
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
        AddHandler Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.DocumentToBeDeactivated, AddressOf Me.DocumentManager_DocumentToBeDeactivated
        AddHandler Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.DocumentToBeDestroyed, AddressOf Me.DocumentManager_DocumentToBeDeactivated
        AddHandler Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.DocumentActivated, AddressOf Me.DocumentManager_DocumentActivated
        'set current layoutmanager
        '_lm = LayoutManager.Current
        'AddHandler Me._lm.LayoutSwitched, AddressOf Me.DocumentManger_DocumentLayoutSwitched


        'tool toevoegen aan trusted path
        getTrustedPaths(clsFunctions.getCoreDir())
        'load settings
        loadSettingsFromINI()
        'selectie filters laden
        loadFilters()
        loadLayouts()
        'set active layout
        DocumentManger_DocumentLayoutSwitched()
        If clsFunctions.isDebugMode() Then
            'debug modus = true
            lblTitel.Text = lblTitel.Text & " [DEBUG Version]"
            Using myGraphics As Graphics = Me.CreateGraphics()
                'Windows.MessageBox.Show(String.Format("Resolution X: {0} dpi, Resolution Y: {1} dpi", myGraphics.DpiX, myGraphics.DpiY), "Windows Resolution")
                lblTitel.Text = lblTitel.Text & " DPI x: " & myGraphics.DpiX.ToString & " y: " & myGraphics.DpiY.ToString
            End Using
        End If
        Using myGraphics As Graphics = Me.CreateGraphics()
            dItemHeightMin = myGraphics.DpiY / 2.5
            dItemHeightMax = dItemHeightMin + 100
        End Using

    End Sub

    Public Sub loadSettingsFromINI()
        If File.Exists(sIniDir & sIniFile) Then
            'bestand bestaat, instelingen laden
            iniFile = New clsINI(sIniDir & sIniFile)
            sLayoutTemplateFolder = iniFile.GetString("template", "templatefolder", sLayoutTemplateFolder)
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
        Dim layAndTab As SortedDictionary(Of Integer, String) = New SortedDictionary(Of Integer, String)
        Dim layAndTabOID As SortedDictionary(Of Integer, String) = New SortedDictionary(Of Integer, String)
        Dim layPlotStyle As SortedDictionary(Of Integer, String) = New SortedDictionary(Of Integer, String)
        Dim layPlotDevice As SortedDictionary(Of Integer, String) = New SortedDictionary(Of Integer, String)
        Dim layPlotTransparency As SortedDictionary(Of Integer, Boolean) = New SortedDictionary(Of Integer, Boolean)

        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = New RN_LayoutItems.RN_UCLayoutItem()
        'current layout control
        Dim myCntrlCurrent As RN_LayoutItems.RN_UCLayoutItem = New RN_LayoutItems.RN_UCLayoutItem()
        Try
            'reset and clear list
            'clsFunctions.makeLog(sIniDir & "\log.txt", "Load Layouts")
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
            myCntrl.LayoutName = "Model"
            myCntrl.PlotStyle = "" 'model has no plotstyle
            myCntrl.PlotDevice = "" 'model as no plotdevice
            myCntrl.IsModel = True
            drag_drop_scroll_amount = myCntrl.Height + 40
            'hide button print and checkbox
            myCntrl.hideButtons()
            myCntrl.updateItem()
            'only handler to change view since model can't be renamed
            AddHandler myCntrl.View_Click, AddressOf ItemViewClick
            flowLayouts.Controls.Add(myCntrl)
            ' Get the layout dictionary of the current database


            Using trx As Transaction = acCurDb.TransactionManager.StartTransaction()
                Dim layDict As DBDictionary = acCurDb.LayoutDictionaryId.GetObject(OpenMode.ForRead)
                For Each entry As DBDictionaryEntry In layDict
                    Dim lay As Layout = CType(entry.Value.GetObject(OpenMode.ForRead), Layout)
                    'Dim sPlotStyleTmp As String = lay.CurrentStyleSheet & " | " & lay.PlotConfigurationName
                    Dim sPlotStyleTmp As String = lay.CurrentStyleSheet & " | "
                    Dim sPlotDeviceTmp As String = lay.PlotConfigurationName
                    layAndTab.Add(lay.TabOrder, lay.LayoutName)
                    layAndTabOID.Add(lay.TabOrder, lay.Handle.ToString)
                    layPlotStyle.Add(lay.TabOrder, sPlotStyleTmp)
                    layPlotDevice.Add(lay.TabOrder, sPlotDeviceTmp)
                    layPlotTransparency.Add(lay.TabOrder, lay.PlotTransparency)
                Next
                trx.Commit()
            End Using
        Catch ex As Exception
            MsgBox("Fout bij het laden van de Layouts " & ex.Message)
        End Try
        Dim iTabIndex As Integer = 1 'model = 0
        Dim sPlotStyle As String = "PlotStyle.ctb"
        Dim sPlotDevice As String = ""
        Dim bPlotTransparency As Boolean = False
        Try
            For Each sLayoutName In layAndTab.Values
                'add name to list except Model
                If sLayoutName = "Model" Then

                Else
                    sPlotStyle = "-"
                    myCntrl = New RN_LayoutItems.RN_UCLayoutItem()
                    myCntrl.LayoutName = sLayoutName
                    myCntrl.TabIndex = iTabIndex
                    If layAndTabOID.ContainsKey(iTabIndex) Then
                        'myCntrl.LayoutID = layAndTabOID.Item(iTabIndex)
                        myCntrl.LayoutHandle = layAndTabOID.Item(iTabIndex)
                        sPlotStyle = layPlotStyle.Item(iTabIndex)
                        sPlotDevice = layPlotDevice.Item(iTabIndex)
                        bPlotTransparency = layPlotTransparency.Item(iTabIndex)
                    End If
                    myCntrl.PlotStyle = sPlotStyle
                    myCntrl.PlotDevice = sPlotDevice
                    myCntrl.PlotTransparency = bPlotTransparency
                    myCntrl.ControlWidth = flowLayouts.Width
                    myCntrl.MinHeight = dItemHeightMin  '40 'minheight for collapse
                    myCntrl.MaxHeight = dItemHeightMax  '100 'max height
                    myCntrl.updateItem()
                    'add handlers to register functions for items
                    AddHandler myCntrl.View_Click, AddressOf ItemViewClick
                    AddHandler myCntrl.LayoutNameEdit_KeyDown, AddressOf renameLayout
                    AddHandler myCntrl.Plot_Click, AddressOf PlotLayout
                    AddHandler myCntrl.Plot_CheckedChanged, AddressOf PlotCheck
                    AddHandler myCntrl.MouseMove, AddressOf item_MouseMove
                    AddHandler myCntrl.DragEnter, AddressOf item_DragEnter
                    AddHandler myCntrl.plotTransparency_Click, AddressOf ChangePlotTransparency
                    AddHandler myCntrl.Collapse_Click, AddressOf getPageSetup
                    AddHandler myCntrl.ChangePageSetup, AddressOf setPageSetup
                    AddHandler myCntrl.setPlotMediaSize, AddressOf setPlotMediaSize
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
                    'myCntrl.Width = flowLayouts.Width

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

    Public Function PlotLayout(ByVal sender As Object, ByVal e As System.EventArgs, Optional sDWF As Boolean = False, Optional myTmpCntrl As RN_LayoutItems.RN_UCLayoutItem = Nothing, Optional sOverrideDevice As String = "")
        Dim sOutputLocation As String = PDFoutputLocation()
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem
        If sDWF = True Then
            myCntrl = myTmpCntrl
        ElseIf Not sOverrideDevice = "" Then
            'plotter override choosen
            myCntrl = myTmpCntrl
        Else
            'default plot
            myCntrl = CType(sender, RN_LayoutItems.RN_UCLayoutItem)
            'use plotter from settings
        End If

        Dim layouts As New List(Of Layout)()
        Try
            Using acLockDoc As DocumentLock = acDoc.LockDocument
                Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                    Dim acLayoutMgr As LayoutManager = LayoutManager.Current

                    Dim oId As ObjectId = acLayoutMgr.GetLayoutId(myCntrl.LayoutName)
                    Dim lay As Layout = acTrans.GetObject(oId, OpenMode.ForWrite)
                    layouts.Add(lay)

                    acTrans.Commit()

                    'layouts doorlopen en titelblok attributes saven voor CENTRUMPLAN
                    'BESTEKNUMMER, BLADNUMMER, VERSIE = centrumplan (SAL-TITELBLOK-CENTERUMPLAN-V2)
                    'BESTEKNUMMER, BLADNUMMER = anacon (SAL-TITELBLOK)
                    sLayoutNames = clsLayout.getLayoutPrintNames(layouts)

                    If sDWF = False Then
                        'plot PDF
                        plotLayouts(SheetType.SinglePdf, layouts, False, "pdf", sOverrideDevice)
                    Else
                        'plot DWF
                        plotLayouts(SheetType.SingleDwf, layouts, False, "dwf")
                    End If
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
        'DEBUG logging
        clsFunctions.makeLog(sDebugLog, "function SetLayoutCurrent()", False)
        Try
            Using acLockDoc As DocumentLock = acDoc.LockDocument
                'DEBUG logging
                clsFunctions.makeLog(sDebugLog, "acDoc.LockDocument", False)
                Dim acLayoutMgr As LayoutManager = LayoutManager.Current
                acLayoutMgr.CurrentLayout = sLayoutName
                'DEBUG logging
                clsFunctions.makeLog(sDebugLog, "Get Current Layout", False)
                acDoc.Editor.Regen()
                'DEBUG logging
                clsFunctions.makeLog(sDebugLog, "Editor.Regen()", False)
                'zoom extends when not model view
                If bIsModel = False Then
                    'DEBUG logging
                    clsFunctions.makeLog(sDebugLog, "bIsModel = False", False)
                    Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                        'DEBUG logging
                        clsFunctions.makeLog(sDebugLog, "Start Transaction", False)
                        Dim oId As ObjectId = acLayoutMgr.GetLayoutId(sLayoutName)
                        Dim lay As Layout = acTrans.GetObject(oId, OpenMode.ForWrite)
                        'lock viewports when not locked
                        'DEBUG logging
                        clsFunctions.makeLog(sDebugLog, "Loop Viewports", False)
                        For Each vpId As ObjectId In lay.GetViewports()
                            Dim vp As Viewport = DirectCast(acTrans.GetObject(vpId, OpenMode.ForWrite, False, True), Viewport)
                            vp.Locked = True
                            'DEBUG logging
                            clsFunctions.makeLog(sDebugLog, "LockViewport", False)
                        Next
                        acTrans.Commit()
                        Dim acadApp As Object = Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication
                        acadApp.ZoomExtents()
                        'DEBUG logging
                        clsFunctions.makeLog(sDebugLog, "ZoomExtents()", False)
                        acDoc.Editor.Regen()
                        'DEBUG logging
                        clsFunctions.makeLog(sDebugLog, "Editor.Regen()", False)
                    End Using
                End If
            End Using
        Catch ex As Exception
            clsFunctions.LogException(ex, "setLayoutCurrent()")
            MsgBox("Fout bij het Current zetten van de layout!" & vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
        End Try
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
                                'kijken of we de layoutnaam kunnen samenstellen uit block Attributes

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
    Public Function plotLayouts(ByVal pdfSheetType As SheetType, ByVal layouts As List(Of Layout), Optional bSuppressMessage As Boolean = False, Optional sFileExt As String = "pdf", Optional sOverridePlotDevice As String = "")
        Dim sOutputLocation As String = PDFoutputLocation()

        Dim db As Database = HostApplicationServices.WorkingDatabase
        Dim bgp As Short = CShort(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("BACKGROUNDPLOT"))
        Try
            Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("BACKGROUNDPLOT", 0)
            Dim filename As String
            If sOutputLocation = "" Then
                'drawing location
                'filename = Path.ChangeExtension(db.Filename, "pdf")
                filename = Path.ChangeExtension(db.Filename, sFileExt)
            Else
                'user chosen location
                'filename = sOutputLocation & "\" & Path.GetFileName(Path.ChangeExtension(db.Filename, "pdf"))
                filename = sOutputLocation & "\" & Path.GetFileName(Path.ChangeExtension(db.Filename, sFileExt))
            End If


            Dim plotter As New plotting.MultiSheetsPdf(filename, layouts, pdfSheetType, bSuppressMessage, sOverridePlotDevice, sLayoutNames)
            plotter.Publish()

        Catch e As System.Exception
            Dim ed As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
            ed.WriteMessage(vbLf & "Error: {0}" & vbLf & "{1}", e.Message, e.StackTrace)
            MsgBox("Fout bij het plotten!" & vbCrLf & e.Message & vbCrLf & e.StackTrace)
            Return False
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

    Private Sub cmdPlotMulitSheet_MouseDown(sender As Object, e As MouseEventArgs) Handles cmdPlotMulitSheet.MouseDown
        If e.Button = MouseButtons.Right Then
            ContextMenuDWFoptions.Show(cmdPrintDWF, 0, 0)
        End If
    End Sub

    Private Sub cmdPlotSingleSheet_Click(sender As Object, e As EventArgs) Handles cmdPlotSingleSheet.Click
        Dim layouts As New List(Of Layout)()
        layouts = checkedLayouts()
        If layouts.Count > 1 Then
            'layouts doorlopen en titelblok attributes saven voor CENTRUMPLAN
            'BESTEKNUMMER, BLADNUMMER, VERSIE = centrumplan (SAL-TITELBLOK-CENTERUMPLAN-V2)
            'BESTEKNUMMER, BLADNUMMER = anacon (SAL-TITELBLOK)
            sLayoutNames = clsLayout.getLayoutPrintNames(layouts)
            regenDrawing()
            plotLayouts(SheetType.SinglePdf, checkedLayouts())
        End If
    End Sub

    Private Sub cmdPlotSingleSheet_MouseDown(sender As Object, e As MouseEventArgs) Handles cmdPlotSingleSheet.MouseDown
        If e.Button = MouseButtons.Right Then
            ContextMenuDWFoptions.Show(cmdPrintDWF, 0, 0)
        End If
    End Sub

    Private Sub cmdPrintDWF_Click(sender As Object, e As EventArgs) Handles cmdPrintDWF.Click
        Dim layouts As New List(Of Layout)()
        layouts = checkedLayouts()
        If layouts.Count > 1 Then
            regenDrawing()
            plotLayouts(SheetType.MultiDwf, checkedLayouts(), False, "dwf")
        End If
    End Sub

    Private Sub cmdPrintDWF_MouseDown(sender As Object, e As MouseEventArgs) Handles cmdPrintDWF.MouseDown
        If e.Button = MouseButtons.Right Then
            ContextMenuDWFoptions.Show(cmdPrintDWF, 0, 0)
        End If
    End Sub


    Private Sub SinglesheetDWFToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SinglesheetDWFToolStripMenuItem.Click
        Dim layouts As New List(Of Layout)()
        layouts = checkedLayouts()
        If layouts.Count > 1 Then
            regenDrawing()
            plotLayouts(SheetType.SingleDwf, checkedLayouts(), False, "dwf")
        End If
    End Sub

    Private Sub sPDFenMDWF_Click(sender As Object, e As EventArgs) Handles sPDFenMDWF.Click
        Dim layouts As New List(Of Layout)()
        layouts = checkedLayouts()
        If layouts.Count > 1 Then
            regenDrawing()
            If plotLayouts(SheetType.SinglePdf, checkedLayouts(), True) = True Then
                regenDrawing()
                plotLayouts(SheetType.MultiDwf, checkedLayouts(), False, "dwf")
            End If
        End If
    End Sub

    Private Sub mPDFenMDWF_Click(sender As Object, e As EventArgs) Handles mPDFenMDWF.Click
        Dim layouts As New List(Of Layout)()
        layouts = checkedLayouts()
        If layouts.Count > 1 Then
            regenDrawing()
            If plotLayouts(SheetType.MultiPdf, checkedLayouts(), True) = True Then
                regenDrawing()
                plotLayouts(SheetType.MultiDwf, checkedLayouts(), False, "dwf")
            End If
        End If
    End Sub


    Public Sub regenDrawing()
        acEd.Regen()
    End Sub

    Private Sub cmdRefreshList_Click(sender As Object, e As EventArgs) Handles cmdRefreshList.Click
        loadLayouts()
    End Sub

    Public Function itterateList(ByVal sAction As String)
        aPstyleLayouts = New List(Of String)
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
                    If sAction = "plotstyle" Then
                        aPstyleLayouts.Add(myCntrl.LayoutName)
                    End If
                Else 'layout is unchecked
                    If sAction = "select" Or sAction = "invert" Then
                        myCntrl.SetCheckState(Not myCntrl.CheckState)
                    End If
                End If
                'current layout markeren
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


    Private Sub PlotSinglesheetDWFToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PlotSinglesheetDWFToolStripMenuItem.Click
        'plot to DWF Singlesheet
        Dim mySubMenu As ContextMenuStrip = CType(sender, ToolStripMenuItem).Owner
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(mySubMenu.Tag, RN_LayoutItems.RN_UCLayoutItem)
        'send to plotter
        PlotLayout(sender, e, True, myCntrl)
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
        'disable change plotstyle if drawing is STB
        If pstylemode = 0 Then
            PlotstyleTableWijzigenToolStripMenuItem.Enabled = False
        Else
            PlotstyleTableWijzigenToolStripMenuItem.Enabled = True
            'submenu items toevoegen
            Dim pstyleArray As List(Of String) = New List(Of String)
            If PlotstyleTableWijzigenToolStripMenuItem.DropDownItems.Count > 0 Then
                PlotstyleTableWijzigenToolStripMenuItem.DropDownItems.Clear()
            End If
            For Each plotStyle As String In PlotSettingsValidator.Current.GetPlotStyleSheetList()
                If Not pstyleArray.Contains(plotStyle) And plotStyle.ToUpper.Contains(".CTB") Then
                    Dim mnuItm As New ToolStripMenuItem
                    mnuItm.Text = plotStyle
                    mnuItm.Tag = CType(Me.SubMenu.SourceControl, RN_LayoutItems.RN_UCLayoutItem)
                    AddHandler mnuItm.Click, AddressOf ChangePlotStyle
                    PlotstyleTableWijzigenToolStripMenuItem.DropDownItems.Add(mnuItm)
                    pstyleArray.Add(plotStyle)
                End If
            Next
        End If
        'add plot override enkel wanneer dit nog niet is toegevoegd
        If PlotterOverrideToolStripMenuItem.HasDropDownItems = True Then
            PlotterOverrideToolStripMenuItem.DropDownItems.Clear()
        End If
        Dim sSettingsFile As String = clsFunctions.getCoreDir() & sPlotPreferences
        'PlotterOverrideToolStripMenuItem
        For Each sPlotDevice As String In clsFunctions.loadPlotPresets(sSettingsFile)
            Dim mnuItm As New ToolStripMenuItem
            mnuItm.Text = sPlotDevice
            mnuItm.Tag = CType(Me.SubMenu.SourceControl, RN_LayoutItems.RN_UCLayoutItem)
            AddHandler mnuItm.Click, AddressOf OverridePlotDevice
            PlotterOverrideToolStripMenuItem.DropDownItems.Add(mnuItm)
        Next
        'add plotdevice list to submenu enkel wanneer dit nog niet is toegevoegd
        If PlotdeviceWijzigenToolStripMenuItem.HasDropDownItems = True Then
            PlotdeviceWijzigenToolStripMenuItem.DropDownItems.Clear()
        End If
        For Each plotDevice As String In PlotSettingsValidator.Current.GetPlotDeviceList()
            'skip certain devices
            If Not plotDevice.ToLower = "none" And Not plotDevice.ToLower = "fax" And Not plotDevice.ToLower.Contains("onenote") And Not plotDevice.ToLower.Contains("publishtoweb") Then
                Dim mnuItem As New ToolStripMenuItem
                mnuItem.Text = plotDevice
                mnuItem.Tag = CType(Me.SubMenu.SourceControl, RN_LayoutItems.RN_UCLayoutItem)
                AddHandler mnuItem.Click, AddressOf ChangePlotDevice
                PlotdeviceWijzigenToolStripMenuItem.DropDownItems.Add(mnuItem)
            End If
        Next

    End Sub

    Public Function ChangePlotDevice(ByVal sender As Object, ByVal e As EventArgs)
        Dim bChangeBatch As Boolean = False
        If iCheckCount > 1 Then
            If MsgBox("Wilt u de wijziging toepassen op de geselecteerde layouts?", MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                bChangeBatch = True
                itterateList("plotstyle")
            End If
        End If


        Dim mnuPltDevice As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim sOverrideDeviceName As String = mnuPltDevice.Text
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(mnuPltDevice.Tag, RN_LayoutItems.RN_UCLayoutItem)
        Dim sLayName As String = myCntrl.LayoutName

        If bChangeBatch = False Then
            Try
                Using acLockDoc As DocumentLock = acDoc.LockDocument
                    Using trx As Transaction = acCurDb.TransactionManager.StartTransaction()
                        Dim layDict As DBDictionary = acCurDb.LayoutDictionaryId.GetObject(OpenMode.ForWrite)
                        For Each entry As DBDictionaryEntry In layDict
                            Dim lay As Layout = CType(entry.Value.GetObject(OpenMode.ForWrite), Layout)
                            If sLayName = lay.LayoutName Then
                                Dim ps As PlotSettings = New PlotSettings(lay.ModelType)
                                ps.CopyFrom(lay)

                                'Dim sPlotConfig As String = lay.CurrentStyleSheet & " | " & mnuPltDevice.Text
                                'Dim sPlotDeviceTmp As String = mnuPltDevice.Text
                                'change plot device
                                Dim plotSetVal As PlotSettingsValidator = PlotSettingsValidator.Current
                                Dim plotDev = plotSetVal.GetPlotDeviceList()

                                If plotDev.Contains(sOverrideDeviceName) Then
                                    plotSetVal.RefreshLists(lay)
                                    plotSetVal.SetPlotConfigurationName(lay, sOverrideDeviceName, lay.CanonicalMediaName())
                                    trx.Commit()
                                    'myCntrl.PlotStyle = sPlotConfig
                                    myCntrl.PlotDevice = sOverrideDeviceName
                                    myCntrl.updateItem()
                                End If
                                Exit For
                            End If
                        Next
                    End Using
                End Using
            Catch ex As Exception
                MsgBox("Fout bij wijzigen van de PlotDevice " & vbCrLf & ex.Message & vbCrLf & ex.Source)
            End Try
        Else
            Try
                Using acLockDoc As DocumentLock = acDoc.LockDocument
                    Using trx As Transaction = acCurDb.TransactionManager.StartTransaction()
                        Dim layDict As DBDictionary = acCurDb.LayoutDictionaryId.GetObject(OpenMode.ForWrite)
                        For Each entry As DBDictionaryEntry In layDict
                            Dim lay As Layout = CType(entry.Value.GetObject(OpenMode.ForWrite), Layout)
                            If aPstyleLayouts.Contains(lay.LayoutName) Then
                                Dim ps As PlotSettings = New PlotSettings(lay.ModelType)
                                ps.CopyFrom(lay)

                                'Dim sPlotConfig As String = lay.CurrentStyleSheet & " | " & mnuPltDevice.Text
                                'change plot device
                                Dim plotSetVal As PlotSettingsValidator = PlotSettingsValidator.Current
                                Dim plotDev = plotSetVal.GetPlotDeviceList()

                                If plotDev.Contains(sOverrideDeviceName) Then
                                    plotSetVal.RefreshLists(lay)
                                    plotSetVal.SetPlotConfigurationName(lay, sOverrideDeviceName, lay.CanonicalMediaName())
                                    'myCntrl.PlotStyle = sPlotConfig
                                    myCntrl.PlotDevice = sOverrideDeviceName
                                    myCntrl.updateItem()
                                End If
                            End If
                        Next
                        trx.Commit()
                    End Using
                End Using
                loadLayouts()
            Catch ex As Exception
                MsgBox("Fout bij wijzigen van de PlotDevice " & ex.Message)
            End Try
        End If
        Return True


    End Function


    Public Function OverridePlotDevice(ByVal sender As Object, ByVal e As EventArgs)
        Dim mnuPltOverride As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim sOverrideDeviceName As String = mnuPltOverride.Text
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(mnuPltOverride.Tag, RN_LayoutItems.RN_UCLayoutItem)
        PlotLayout(sender, e, False, myCntrl, sOverrideDeviceName)
        Return True
    End Function

    Public Function OverridePlotDeviceLayoutSelection(ByVal sender As Object, ByVal e As EventArgs)
        Dim mnuPltOverride As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim sOverrideDeviceName As String = mnuPltOverride.Text
        'Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(mnuPltOverride.Tag, RN_LayoutItems.RN_UCLayoutItem)
        'PlotLayout(sender, e, False, myCntrl, sOverrideDeviceName)
        Dim layouts As New List(Of Layout)()
        layouts = checkedLayouts()
        If layouts.Count > 1 Then
            regenDrawing()
            plotLayouts(SheetType.SinglePdf, checkedLayouts(), False, "pdf", sOverrideDeviceName)
        End If
        Return True
    End Function

    ''' <summary>
    ''' ChangePlotStyle
    ''' </summary>
    ''' <param name="sender">ToolStripMenuItem</param>
    ''' <param name="e"></param>
    ''' <returns></returns>
    Public Function ChangePlotStyle(ByVal sender As Object, ByVal e As EventArgs)
        Dim bChangeBatch As Boolean = False
        If iCheckCount > 1 Then
            If MsgBox("Wilt u de wijziging toepassen op de geselecteerde layouts?", MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                bChangeBatch = True
                itterateList("plotstyle")
            End If
        End If

        Dim mnuPltStyle As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(mnuPltStyle.Tag, RN_LayoutItems.RN_UCLayoutItem)
        Dim sLayName As String = myCntrl.LayoutName
        If bChangeBatch = False Then
            Try
                Using acLockDoc As DocumentLock = acDoc.LockDocument
                    Using trx As Transaction = acCurDb.TransactionManager.StartTransaction()
                        Dim layDict As DBDictionary = acCurDb.LayoutDictionaryId.GetObject(OpenMode.ForWrite)
                        For Each entry As DBDictionaryEntry In layDict
                            Dim lay As Layout = CType(entry.Value.GetObject(OpenMode.ForWrite), Layout)
                            If sLayName = lay.LayoutName Then
                                Dim sPlotConfig As String = mnuPltStyle.Text & " | " '& lay.PlotConfigurationName
                                'set stylesheet
                                Dim plotSetVal As PlotSettingsValidator = PlotSettingsValidator.Current
                                plotSetVal.RefreshLists(lay)
                                plotSetVal.SetCurrentStyleSheet(lay, mnuPltStyle.Text)
                                trx.Commit()
                                myCntrl.PlotStyle = sPlotConfig
                                myCntrl.updateItem()
                                Exit For
                            End If
                        Next
                    End Using
                End Using
            Catch ex As Exception
                MsgBox("Fout bij wijzigen van de PlotStyle " & ex.Message)
            End Try
        Else
            Try
                Using acLockDoc As DocumentLock = acDoc.LockDocument
                    Using trx As Transaction = acCurDb.TransactionManager.StartTransaction()
                        Dim layDict As DBDictionary = acCurDb.LayoutDictionaryId.GetObject(OpenMode.ForWrite)
                        For Each entry As DBDictionaryEntry In layDict
                            Dim lay As Layout = CType(entry.Value.GetObject(OpenMode.ForWrite), Layout)
                            If aPstyleLayouts.Contains(lay.LayoutName) Then
                                Dim sPlotConfig As String = mnuPltStyle.Text & " | " '& lay.PlotConfigurationName
                                'set stylesheet
                                Dim plotSetVal As PlotSettingsValidator = PlotSettingsValidator.Current
                                plotSetVal.RefreshLists(lay)
                                plotSetVal.SetCurrentStyleSheet(lay, mnuPltStyle.Text)
                                myCntrl.PlotStyle = sPlotConfig
                                myCntrl.updateItem()
                            End If
                        Next
                        trx.Commit()
                    End Using
                End Using
                loadLayouts()
            Catch ex As Exception
                MsgBox("Fout bij wijzigen van de PlotStyle " & ex.Message)
            End Try
        End If
        Return True
    End Function

    Public Function ChangePlotTransparency(ByVal sender As Object, ByVal e As EventArgs)
        'Dim mnuPltStyle As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)
        Dim sLayName As String = myCntrl.LayoutName
        Try
            Using acLockDoc As DocumentLock = acDoc.LockDocument
                Using trx As Transaction = acCurDb.TransactionManager.StartTransaction()
                    Dim layDict As DBDictionary = acCurDb.LayoutDictionaryId.GetObject(OpenMode.ForWrite)
                    For Each entry As DBDictionaryEntry In layDict
                        Dim lay As Layout = CType(entry.Value.GetObject(OpenMode.ForWrite), Layout)
                        If sLayName = lay.LayoutName Then
                            Dim bPlotTransp As Boolean = Not myCntrl.PlotTransparency
                            lay.PlotTransparency = bPlotTransp
                            trx.Commit()
                            myCntrl.PlotTransparency = bPlotTransp
                            myCntrl.updateItem()
                            Exit For
                        End If
                    Next
                End Using
            End Using
            Return True
        Catch ex As Exception
            MsgBox("Fout bij wijzigen van de PlotTransparency " & ex.Message)
            Return False
        End Try
    End Function

    Public Enum PlotRotation
        ' Fields
        Degrees000 = 0
        Degrees090 = 1
        Degrees180 = 2
        Degrees270 = 3
    End Enum

    Public Function getPageSetup(ByVal sender As Object, ByVal e As EventArgs)
        'Dim mnuPltStyle As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)
        Dim sLayName As String = myCntrl.LayoutName
        Dim dtPlotMedia As System.Data.DataTable = New System.Data.DataTable
        dtPlotMedia.Columns.Add("id")
        dtPlotMedia.Columns.Add("media")
        Try
            Using acLockDoc As DocumentLock = acDoc.LockDocument
                Using trx As Transaction = acCurDb.TransactionManager.StartTransaction()
                    Dim layDict As DBDictionary = acCurDb.LayoutDictionaryId.GetObject(OpenMode.ForWrite)
                    For Each entry As DBDictionaryEntry In layDict
                        Dim lay As Layout = CType(entry.Value.GetObject(OpenMode.ForWrite), Layout)
                        If sLayName = lay.LayoutName Then
                            Dim laOrientation As PlotRotation = lay.PlotRotation
                            'MsgBox(lay.PlotRotation.ToString & " -- " & laOrientation.ToString)
                            myCntrl.ReadSettings = True 'prevent update
                            Select Case laOrientation.ToString
                                Case "Degrees000"
                                    'portrait
                                    myCntrl.PlotOrientation = "portrait"
                                Case "Degrees090"
                                    'landscape 
                                    myCntrl.PlotOrientation = "landscape"
                                Case "Degrees180"
                                    'portrait
                                    myCntrl.PlotOrientation = "portrait"
                                Case "Degrees270"
                                    'landscape
                                    myCntrl.PlotOrientation = "landscape"
                            End Select
                            myCntrl.DisplayPlotStyle = lay.ShowPlotStyles
                            'load media
                            Dim plotSetVal As PlotSettingsValidator = PlotSettingsValidator.Current
                            plotSetVal.RefreshLists(lay)
                            Dim plotSet As PlotSettings = New PlotSettings(lay.ModelType)
                            'current media
                            Dim sCurrMedName As String = lay.CanonicalMediaName

                            Dim canMedNames As Specialized.StringCollection = plotSetVal.GetCanonicalMediaNameList(plotSet)
                            Dim i As Integer = 0
                            For Each canMedName As String In canMedNames
                                Dim dtRow As System.Data.DataRow = dtPlotMedia.NewRow()
                                dtRow.Item(0) = canMedName
                                dtRow.Item(1) = plotSetVal.GetLocaleMediaName(plotSet, i)
                                dtPlotMedia.Rows.Add(dtRow)
                                If canMedName = sCurrMedName Then
                                    'set selected index
                                    myCntrl.ChoosenMediaSizeCurrent = i
                                End If
                                i = i + 1
                            Next
                            myCntrl.PlotMediaList = dtPlotMedia
                            'update control
                            myCntrl.updateItem()
                            trx.Commit()
                            Exit For
                        End If
                    Next
                End Using
            End Using
            Return True
        Catch ex As Exception
            MsgBox("Fout bij laden van de pagesetup " & ex.Message)
            Return False
        End Try
    End Function

    Public Function setPageSetup(ByVal sender As Object, ByVal e As EventArgs)
        'Dim mnuPltStyle As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)
        Dim sLayName As String = myCntrl.LayoutName
        Try
            Using acLockDoc As DocumentLock = acDoc.LockDocument
                Dim acEd As Editor = acDoc.Editor
                Using trx As Transaction = acCurDb.TransactionManager.StartTransaction()
                    Dim layDict As DBDictionary = acCurDb.LayoutDictionaryId.GetObject(OpenMode.ForWrite)
                    For Each entry As DBDictionaryEntry In layDict
                        Dim lay As Layout = CType(entry.Value.GetObject(OpenMode.ForWrite), Layout)
                        If sLayName = lay.LayoutName Then
                            Dim plotSetVal As PlotSettingsValidator = PlotSettingsValidator.Current
                            plotSetVal.RefreshLists(lay)
                            Select Case myCntrl.PlotOrientation
                                Case "portrait"
                                    plotSetVal.SetPlotRotation(lay, PlotRotation.Degrees000)
                                Case "landscape"
                                    plotSetVal.SetPlotRotation(lay, PlotRotation.Degrees090)
                            End Select
                            lay.ShowPlotStyles = myCntrl.DisplayPlotStyle
                            trx.Commit()
                            acEd.Regen()
                            Exit For
                        End If
                    Next
                End Using
            End Using
            Return True
        Catch ex As Exception
            MsgBox("Fout bij opslaan van de pagesetup " & ex.Message)
            Return False
        End Try
    End Function

    Public Function setPlotMediaSize(ByVal sender As Object, ByVal e As EventArgs)
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)
        Dim sLayName As String = myCntrl.LayoutName
        Try
            Using acLockDoc As DocumentLock = acDoc.LockDocument
                Dim acEd As Editor = acDoc.Editor
                Using trx As Transaction = acCurDb.TransactionManager.StartTransaction()
                    Dim layDict As DBDictionary = acCurDb.LayoutDictionaryId.GetObject(OpenMode.ForWrite)
                    For Each entry As DBDictionaryEntry In layDict
                        Dim lay As Layout = CType(entry.Value.GetObject(OpenMode.ForWrite), Layout)
                        If sLayName = lay.LayoutName Then
                            Dim plotSetVal As PlotSettingsValidator = PlotSettingsValidator.Current
                            plotSetVal.RefreshLists(lay)
                            Dim sChoosenMediaSize As String = myCntrl.ChoosenMediaSize
                            'check of portrait / landscape setting overeenkomt met media size, zo niet aanpassen voordat media size wordt gewijzigd.
                            'Dim sMediaSizeTMP As String() = sChoosenMediaSize.Replace("_", " ").Split("(") 'A2 (420.00 x 594.00 MM) ( H x B)
                            'Dim sMediaDimensions As String() = sMediaSizeTMP(1).ToLower.Split("x")
                            'Dim sWidth As String = clsFunctions.ParseDigits(sMediaDimensions(0))
                            'Dim sHeight As String = clsFunctions.ParseDigits(sMediaDimensions(1))
                            'Dim dHeight As Double = CDbl(sWidth)
                            'Dim dWidth As Double = CDbl(sHeight)
                            ''MsgBox(dWidth.ToString & " X " & dHeight)
                            'Dim sOrientation As String
                            'If dWidth < dHeight Then
                            '    'portrait
                            '    sOrientation = "portrait"
                            'Else
                            '    'landscape
                            '    sOrientation = "landscape"
                            'End If
                            'Select Case sOrientation
                            '    Case "portrait"
                            '        plotSetVal.SetPlotRotation(lay, PlotRotation.Degrees000)
                            '    Case "landscape"
                            '        plotSetVal.SetPlotRotation(lay, PlotRotation.Degrees090)
                            'End Select
                            plotSetVal.SetCanonicalMediaName(lay, sChoosenMediaSize)
                            trx.Commit()
                            acEd.Regen()
                            Exit For
                        End If
                    Next
                End Using
            End Using
            'getPageSetup(sender, e)
            Return True
        Catch ex As Exception
            MsgBox("Fout bij wijzigen van de Media Size " & ex.Message)
            Return False
        End Try
    End Function


    Public Function selectExternalTemplate(ByVal sender As Object, ByVal e As EventArgs)
        Dim selectedItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        sLayoutTemplate = selectedItem.Text
        loadExternalTemplate()
    End Function

    Public Function loadExternalTemplate()
        If File.Exists(sLayoutTemplateFolder & sLayoutTemplate) Then
            Dim acExDb As Database = New Database(False, True)
            acExDb.ReadDwgFile(sLayoutTemplateFolder & sLayoutTemplate, FileOpenMode.OpenForReadAndAllShare, True, "")

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
            MsgBox(sLayoutTemplateFolder & sLayoutTemplate & " niet gevonden")

            'cmbNewLayout.Visible = False
            'cmdAddLayout.Visible = False
            Return False
        End If
    End Function

    Private Sub cmdAddLayout_Click(sender As Object, e As EventArgs) Handles cmdAddLayout.Click
        insertLayout(False)
    End Sub
    Public Sub insertLayout(Optional ByVal bDynamicVP As Boolean = False, Optional sLayoutName As String = "", Optional bProcessList As Boolean = False)
        If Not bDynamicVP Then
            'DEBUG logging
            clsFunctions.makeLog(sDebugLog, "INVOEGEN LAYOUT ZONDER DYNAMISCHE PLAATSING", True)
        End If
        'DEBUG logging
        clsFunctions.makeLog(sDebugLog, "Start insertLayout(" & bDynamicVP.ToString & ", " & sLayoutName & ", " & bProcessList.ToString & ")", False)
        If cmbNewLayout.Text.Length = 0 Then
            MsgBox("Selecteer eerst een layout!")
            Exit Sub
        End If
        Dim sNewLayoutName As String
        Dim iLayoutCnt As Integer = 1
        If bDynamicVP = True Then
            'bij lijst naam automatisch
            sNewLayoutName = sLayoutName
        Else
            'bij enkele layout vragen om de naam
            sNewLayoutName = InputBox("Layout naam", "Layout naam", cmbNewLayout.Text)
            'DEBUG logging
            clsFunctions.makeLog(sDebugLog, "Geen DynViewport => InputBox: Layoutnaam " & sNewLayoutName, False)
            'handmatig viewport lijst maken
            vpCoordinates = New ViewPortCoordinates
            vpCoordinatesList = New ViewPortCoordinatesList
            vpCoordinatesList.Add(vpCoordinates)
        End If

        For Each vpViewPort As ViewPortCoordinates In vpCoordinatesList.ViewPorts
            'DEBUG logging
            clsFunctions.makeLog(sDebugLog, "Begin For Each vpViewport", False)
            Dim oLayout As ObjectId
            If bProcessList = True Then
                'in geval van lijst automatisch nummer toevoegen
                sNewLayoutName = sLayoutName & " Lay" & iLayoutCnt.ToString
                iLayoutCnt += 1
                'DEBUG logging
                clsFunctions.makeLog(sDebugLog, "bProcessList = True => " & sNewLayoutName, False)
            End If

            If LayoutExists(sNewLayoutName) = False Then
                'DEBUG logging
                clsFunctions.makeLog(sDebugLog, "LayoutExists = False", False)
                If sNewLayoutName <> "" Then
                    If File.Exists(sLayoutTemplateFolder & sLayoutTemplate) Then
                        'DEBUG logging
                        clsFunctions.makeLog(sDebugLog, "Template DWG gevonden", False)
                        Using acLockDoc As DocumentLock = acDoc.LockDocument
                            'DEBUG logging
                            clsFunctions.makeLog(sDebugLog, "acDoc.LockDocument", False)
                            Dim acExDb As Database = New Database(False, True)
                            'DEBUG logging
                            clsFunctions.makeLog(sDebugLog, "Init Empty DB", False)
                            acExDb.ReadDwgFile(sLayoutTemplateFolder & sLayoutTemplate, FileOpenMode.OpenForReadAndAllShare, True, "")
                            'DEBUG logging
                            clsFunctions.makeLog(sDebugLog, "Read Template to Empty DB", False)
                            ' Get the layout dictionary of the current database
                            Dim layAndTab As SortedDictionary(Of Integer, String) = New SortedDictionary(Of Integer, String)
                            Dim layAndTabOID As SortedDictionary(Of Integer, ObjectId) = New SortedDictionary(Of Integer, ObjectId)
                            'DEBUG logging
                            clsFunctions.makeLog(sDebugLog, "Create SorteDictionarie LayAndTab", False)
                            Try
                                'DEBUG logging
                                clsFunctions.makeLog(sDebugLog, "Begin Try", False)
                                Using acTransEx As Transaction = acExDb.TransactionManager.StartTransaction()
                                    'DEBUG logging
                                    clsFunctions.makeLog(sDebugLog, "acExDb.TransactionManager.StartTransaction()", False)
                                    Dim layoutsEx As DBDictionary = acExDb.LayoutDictionaryId.GetObject(OpenMode.ForRead)
                                    'DEBUG logging
                                    clsFunctions.makeLog(sDebugLog, "Get Layout DBDictionary", False)
                                    If layoutsEx.Contains(cmbNewLayout.Text) Then
                                        'DEBUG logging
                                        clsFunctions.makeLog(sDebugLog, "Layout gevonden", False)
                                        Dim layEx As Layout = layoutsEx.GetAt(cmbNewLayout.Text).GetObject(OpenMode.ForRead)
                                        'DEBUG logging
                                        clsFunctions.makeLog(sDebugLog, "get Layout", False)
                                        Dim blkBlkRecEx As BlockTableRecord = acTransEx.GetObject(layEx.BlockTableRecordId, OpenMode.ForRead)
                                        'DEBUG logging
                                        clsFunctions.makeLog(sDebugLog, "Set BlockTableRecord", False)

                                        Dim idCol As ObjectIdCollection = New ObjectIdCollection()
                                        For Each id As ObjectId In blkBlkRecEx
                                            idCol.Add(id)
                                            'DEBUG logging
                                            clsFunctions.makeLog(sDebugLog, "Add LayID to Collection : ID=" & id.ToString, False)
                                        Next
                                        'invoegen in huidige dwg
                                        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                                            'DEBUG logging
                                            clsFunctions.makeLog(sDebugLog, "acCurDb.TransactionManager.StartTransaction()", False)
                                            Dim blkTbl As BlockTable = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite)
                                            'DEBUG logging
                                            clsFunctions.makeLog(sDebugLog, "Open BlockTable", False)

                                            Using blkBlkRec As New BlockTableRecord
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "Using bklBlkRec", False)
                                                blkBlkRec.Name = "*Paper_Space" & CStr(layoutsEx.Count() - 1)
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "blkBlkRec.Name=" & blkBlkRec.Name.ToString, False)
                                                blkTbl.Add(blkBlkRec)
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "bklTbl.Add(blkBlkRec)", False)
                                                acTrans.AddNewlyCreatedDBObject(blkBlkRec, True)
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "acTrans.AddNewlyCreatedDBObject(blkBlkRec, True)", False)
                                                acExDb.WblockCloneObjects(idCol,
                                                                          blkBlkRec.ObjectId,
                                                                          New IdMapping(),
                                                                          DuplicateRecordCloning.Ignore,
                                                                          False)
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "acExDb.WblockCloneObject => currDB", False)
                                                ' Create a new layout and then copy properties between drawings
                                                Dim layouts As DBDictionary = acTrans.GetObject(acCurDb.LayoutDictionaryId, OpenMode.ForWrite)
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "create layouts DBDictionary", False)


                                                Using lay As New Layout
                                                    'DEBUG logging
                                                    clsFunctions.makeLog(sDebugLog, "Using lay As New Layout", False)
                                                    lay.LayoutName = sNewLayoutName
                                                    lay.AddToLayoutDictionary(acCurDb, blkBlkRec.ObjectId)
                                                    'DEBUG logging
                                                    clsFunctions.makeLog(sDebugLog, "AddToLayoutDictionary => " & sNewLayoutName, False)
                                                    acTrans.AddNewlyCreatedDBObject(lay, True)
                                                    'DEBUG logging
                                                    clsFunctions.makeLog(sDebugLog, "acTrans.AddNewlyCreatedDBObject(lay, True)", False)
                                                    lay.CopyFrom(layEx)
                                                    'DEBUG logging
                                                    clsFunctions.makeLog(sDebugLog, "Copy layout from ExDB", False)

                                                    'save object id voor viewport manipulatie
                                                    oLayout = lay.ObjectId
                                                    'DEBUG logging
                                                    clsFunctions.makeLog(sDebugLog, "Save VP ObjectID", False)

                                                    Dim plSets As DBDictionary = acTrans.GetObject(acCurDb.PlotSettingsDictionaryId, OpenMode.ForRead)
                                                    'DEBUG logging
                                                    clsFunctions.makeLog(sDebugLog, "Get Plotsetting Dictionary", False)
                                                    ' Check to see if a named page setup was assigned to the layout,
                                                    ' if so then copy the page setup settings
                                                    If lay.PlotSettingsName <> "" Then
                                                        Try
                                                            ' Check to see if the page setup exists
                                                            If plSets.Contains(lay.PlotSettingsName) = False Then
                                                                'DEBUG logging
                                                                clsFunctions.makeLog(sDebugLog, "Geen PlotSettings gevonden in Layout", False)
                                                                plSets.UpgradeOpen()
                                                                'DEBUG logging
                                                                clsFunctions.makeLog(sDebugLog, "plSets.UpgradeOpen", False)
                                                                Using plSet As New PlotSettings(lay.ModelType)
                                                                    plSet.PlotSettingsName = lay.PlotSettingsName
                                                                    'DEBUG logging
                                                                    clsFunctions.makeLog(sDebugLog, "add PlotSettings : " & lay.PlotSettingsName, False)
                                                                    plSet.AddToPlotSettingsDictionary(acCurDb)
                                                                    acTrans.AddNewlyCreatedDBObject(plSet, True)

                                                                    Dim plSetsEx As DBDictionary = acTransEx.GetObject(acExDb.PlotSettingsDictionaryId, OpenMode.ForRead)

                                                                    Dim plSetEx As PlotSettings = plSetsEx.GetAt(lay.PlotSettingsName).GetObject(OpenMode.ForRead)

                                                                    plSet.CopyFrom(plSetEx)
                                                                    'DEBUG logging
                                                                    clsFunctions.makeLog(sDebugLog, "PlotSettings CopyFrom(plSetEx)", False)
                                                                End Using
                                                            End If
                                                        Catch ex As Exception
                                                            clsFunctions.LogException(ex, "insertLayout()", "create Plotsettings")
                                                            MsgBox("Fout bij het toepassen van de Pagesetup!" & vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
                                                        End Try
                                                    End If
                                                    'set show annotations
                                                    lay.AnnoAllVisible = True
                                                    'DEBUG logging
                                                    clsFunctions.makeLog(sDebugLog, "Set show annotations to True", False)
                                                End Using
                                            End Using
                                            ' Regen the drawing to get the layout tab to display
                                            acDoc.Editor.Regen()
                                            'DEBUG logging
                                            clsFunctions.makeLog(sDebugLog, "Regen()", False)
                                            ' Save the changes made
                                            acTrans.Commit()
                                            'DEBUG logging
                                            clsFunctions.makeLog(sDebugLog, "Commit()", False)
                                        End Using
                                    Else
                                        MsgBox("Layout " & cmbNewLayout.Text & " kon niet worden ingevoegd!")
                                        Exit Sub
                                    End If
                                    acTransEx.Abort()
                                    'DEBUG logging
                                    clsFunctions.makeLog(sDebugLog, "Abort External Transaction acTransEx.Abort()", False)
                                End Using
                            Catch ex As Exception
                                clsFunctions.LogException(ex, "insertLayout()", "fout bij het laden van de layout uit de Template")
                                MsgBox("Fout bij het laden van de Layouts uit de Template " & ex.Message)
                                Exit Sub
                            End Try
                        End Using
                        setLayoutCurrent(sNewLayoutName, False)
                        'DEBUG logging
                        clsFunctions.makeLog(sDebugLog, "Reload Layout List", False)
                        loadLayouts()
                        'DEBUG logging
                        clsFunctions.makeLog(sDebugLog, "Reload Layout List OK", False)

                        Try
                            If bDynamicVP Then
                                'DEBUG logging
                                clsFunctions.makeLog(sDebugLog, "Dynamic Viewport Added, set settings", False)
                                Using acLockDoc As DocumentLock = acDoc.LockDocument
                                    'DEBUG logging
                                    clsFunctions.makeLog(sDebugLog, "acDoc.LockDocument", False)
                                    Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                                        'DEBUG logging
                                        clsFunctions.makeLog(sDebugLog, "StartTransaction()", False)
                                        Dim blkTbl As BlockTable = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite)
                                        'DEBUG logging
                                        clsFunctions.makeLog(sDebugLog, "Get BlockTable", False)
                                        Dim nwLayout As Layout = acTrans.GetObject(oLayout, OpenMode.ForWrite)
                                        'DEBUG logging
                                        clsFunctions.makeLog(sDebugLog, "Get Layout Object", False)

                                        'viewport van current layout opzoeken, unlocken, zoomen en locken
                                        Dim vpIds As ObjectIdCollection = nwLayout.GetViewports()
                                        'DEBUG logging
                                        clsFunctions.makeLog(sDebugLog, "get Viewport IDS", False)
                                        Dim iTeller As Integer = 0
                                        'Dim currVpId As ObjectId
                                        For Each vpId As ObjectId In vpIds
                                            'DEBUG logging
                                            clsFunctions.makeLog(sDebugLog, "For Each Viewport on Layout", False)
                                            'we hebben enkel het 2e vp nodig
                                            iTeller = iTeller + 1
                                            If iTeller = 2 Then
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "2e Viewport gevonden, aanpassen", False)
                                                'viewport aanpassen.
                                                'currVpId = vpId
                                                Dim currViewport As Viewport = acTrans.GetObject(vpId, OpenMode.ForWrite)
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "Open Viewport ForWrite", False)
                                                currViewport.Locked = False
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "Set Locked = False", False)
                                                'rotatie van view
                                                currViewport.ViewDirection = Vector3d.ZAxis
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "Set ViewDirection", False)
                                                currViewport.ViewTarget = New Point3d(vpViewPort.vpCenter.X, vpViewPort.vpCenter.Y, 0)
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "Set ViewTarget", False)
                                                currViewport.TwistAngle = Math.PI * 2 - vpViewPort.vpRotation
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "Set TwistAngle", False)
                                                currViewport.ViewCenter = Point2d.Origin
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "Set ViewCenter", False)
                                                'locatie van view
                                                'currViewport.ViewCenter = vpCoordinates.vpCenter
                                                'schaal van view
                                                Dim dCustScale As Double = 1000 / vpCustScale
                                                currViewport.CustomScale = dCustScale
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "Set CustomScale", False)
                                                'Dim ocm As ObjectContextManager = acCurDb.ObjectContextManager
                                                'Dim occ As ObjectContextCollection = ocm.GetContextCollection("ACDB_ANNOTAIONSCALES")
                                                'For Each objcon As AnnotationScale In occ
                                                '    If objcon.Name = "M_1:" & vpCustScale Then
                                                '        currViewport.AnnotationScale = objcon
                                                '    End If
                                                'Next

                                                'lock viewport
                                                currViewport.Locked = True
                                                'DEBUG logging
                                                clsFunctions.makeLog(sDebugLog, "Set Locked = True", False)
                                            End If
                                        Next
                                        acTrans.Commit()
                                        'DEBUG logging
                                        clsFunctions.makeLog(sDebugLog, "Commit()", False)
                                    End Using
                                End Using

                            End If
                        Catch ex As Exception
                            clsFunctions.LogException(ex, "insertLayout()", "Fout bij het aanpassen van het Viewport")
                            MsgBox("Fout bij het aanpassen van het Viewport " & ex.Message)
                            Exit Sub
                        End Try
                    Else
                        MsgBox("Kan de layout niet invoegen, Template bestand is niet gevonden!" & vbCrLf & "Het bestand: " & sLayoutTemplateFolder & sLayoutTemplate & " is niet gevonden", MsgBoxStyle.Critical)
                    End If
                End If
            End If
        Next
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
                        'check voor PFL titelblock
                        If sBlockName.ToUpper.Contains("WOS_HFD") Then
                            Dim RNmsgBox As RN_CustomAlerts.frmAlert = New RN_CustomAlerts.frmAlert
                            RNmsgBox.WindowTitle = "Titelblock van Provicie Flevoland gedetecteerd"
                            RNmsgBox.LabelTekst = "Provincie Flevoland tekening gedetecteerd, PFL instellingen toepassen?"
                            RNmsgBox.applytoall = False
                            RNmsgBox.ShowCancelButton = False
                            RNmsgBox.ShowCheckbox = False
                            Dim dlgRes As Windows.Forms.DialogResult = RNmsgBox.ShowDialog()
                            If dlgRes = Windows.Forms.DialogResult.No Then
                                bIsProvFlevoland = False
                            Else
                                bIsProvFlevoland = True
                            End If
                        End If
                        Dim colATT As Autodesk.AutoCAD.DatabaseServices.AttributeCollection = blkRef.AttributeCollection
                        For Each oAttID As ObjectId In colATT
                            'attributes doorlopen
                            Dim refATT As AttributeReference = TryCast(acTrans.GetObject(oAttID, OpenMode.ForRead), AttributeReference)
                            Dim attListItem As RN_attribute_listitem.AttributeListItem = New RN_attribute_listitem.AttributeListItem
                            attListItem.AttribName = refATT.Tag
                            attListItem.AttribCurrValue = refATT.TextString
                            flowLayouts.Controls.Add(attListItem)
                        Next
                    Else
                        'geen block geselecteerd
                        GoTo resestlistitems
                    End If

                Catch ex As Autodesk.AutoCAD.Runtime.Exception
                    MsgBox(ex.Message)
                End Try
                acTrans.Commit()
            End Using
            If flowLayouts.Controls.Count < 0 Then
resestlistitems:
                'reset items
                loadLayouts(True)
                cmdBatchAttributes.Enabled = True
                cmdReplaceAttrib.Visible = False
            End If
        End Using
    End Sub


    Private Sub cmdReplaceAttrib_Click(sender As Object, e As EventArgs) Handles cmdReplaceAttrib.Click
        replaceAttrib()
    End Sub

    Public Sub replaceAttrib(Optional bModifyTag As Boolean = False, Optional bSaveToFile As Boolean = False)
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
                    If bSaveToFile = True Then
                        'output to file
                        clsFunctions.saveToFile(sAttribFile, attListItem.AttribName & ";" & attListItem.AttribNewValue)
                    End If
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
        If bModifyTag = False Then
            LayoutWalker2()
        Else
            LayoutWalker2(bModifyTag)
        End If

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

    Public Function LayoutWalker2(Optional bModifyTag As Boolean = False)
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
                            sCurrLayout = sLayoutName
                            setLayoutCurrent(sLayoutName, True) 'ismodal op true zodat we niet alle viewports lang hoeven
                            '## paperspace id pakken en attributes updaten
                            updateProgressBar(True)
                            UpdateAttributesInBlock(getSpaceID("paper"), sBlockName, False, bModifyTag)
                        End If
                    Else
                        '## layout active zetten
                        Dim sLayoutName As String = myCntrl.LayoutName
                        sCurrLayout = sLayoutName
                        setLayoutCurrent(sLayoutName, True) 'ismodal op true zodat we niet alle viewports lang hoeven
                        '## paperspace id pakken en attributes updaten
                        updateProgressBar(True)
                        UpdateAttributesInBlock(getSpaceID("paper"), sBlockName, False, bModifyTag)
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

    Public Function LayoutWalker(ByRef iPaperSpaceCount As Integer, Optional bBuildOutputNameList As Boolean = False)
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


    Public Function UpdateAttributesInBlock(ByVal oBtrId As ObjectId, ByVal sBlockName As String, Optional bRecursive As Boolean = False, Optional bModifyTag As Boolean = False) As Integer
        Dim iChangedCount As Integer = 0
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
        bIsCentrumplan = False

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
                            If bModifyTag = False Then
                                Dim bThisBlock As Boolean = False
                                If bIsProvFlevoland = True Then
                                    'checken of het een PFL block is
                                    If bd.Name.ToUpper.Contains("WOS_HFD") Then
                                        bThisBlock = True
                                    End If
                                Else
                                    'normaal block
                                    If bd.Name.ToUpper = sBlockName.ToUpper Then
                                        bThisBlock = True
                                    End If
                                End If
                                'If bd.Name.ToUpper = sBlockName.ToUpper Then
                                If bThisBlock = True Then
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
                                                'check of we de layoutnaam moeten invoegen
                                                If sTmpValue.Contains("[#layout]") = True Then
                                                    'layoutnaam vervangen
                                                    sTmpValue = sTmpValue.Replace("[#layout]", sCurrLayout)
                                                End If
                                                ar.TextString = sTmpValue
                                                ar.DowngradeOpen()
                                                iChangedCount += 1
                                            End If
                                        End If
                                    Next 'for each attribute
                                End If
                            Else
                                If bd.Name.ToUpper.Contains(sBlockName.ToUpper) Then
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
                                                'wijzig tag name op basis van de value
                                                'ar.TextString = sTmpValue
                                                ar.Tag = sTmpValue
                                                ar.DowngradeOpen()
                                                iChangedCount += 1
                                            End If
                                        End If
                                    Next 'for each attribute
                                End If
                            End If
                            ' Recurse for nested blocks
                            iChangedCount += UpdateAttributesInBlock(br.BlockTableRecord, sBlockName, True, bModifyTag)
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

    Private Sub ContextMenuDWFoptions_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuDWFoptions.Opening
        'OverridePlotterLayoutSelection
        'add plot override enkel wanneer dit nog niet is toegevoegd
        If OverridePlotterLayoutSelection.HasDropDownItems = True Then
            OverridePlotterLayoutSelection.DropDownItems.Clear()
        End If
        Dim sSettingsFile As String = clsFunctions.getCoreDir() & sPlotPreferences
        'PlotterOverrideToolStripMenuItem
        For Each sPlotDevice As String In clsFunctions.loadPlotPresets(sSettingsFile)
            Dim mnuItm As New ToolStripMenuItem
            mnuItm.Text = sPlotDevice
            'mnuItm.Tag = CType(Me.SubMenu.SourceControl, RN_LayoutItems.RN_UCLayoutItem)
            AddHandler mnuItm.Click, AddressOf OverridePlotDeviceLayoutSelection
            OverridePlotterLayoutSelection.DropDownItems.Add(mnuItm)
        Next
    End Sub

    Private Sub ContextMenuTemplates_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuTemplates.Opening
        'recente instellingen laden
        loadSettingsFromINI()
    End Sub

    Private Sub RenameAttributeTagsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RenameAttributeTagsToolStripMenuItem.Click
        sBlockToFind = InputBox("Blockname waar op gezocht moet worden:", "Blocknaam", sBlockName)

        If sBlockToFind = "" Then
            Exit Sub 'cancel button
        End If
        replaceAttrib(True)

    End Sub

    Private Sub SaveEditsToFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveEditsToFileToolStripMenuItem.Click
        Dim dlgSaveAs As Forms.SaveFileDialog = New Forms.SaveFileDialog
        dlgSaveAs.Filter = "Attribute Definitions|*.attdef"
        If dlgSaveAs.ShowDialog() = DialogResult.OK Then
            sAttribFile = dlgSaveAs.FileName
            If Not sAttribFile.Contains(".attdef") Then
                sAttribFile = sAttribFile & ".attdef"
            End If
            sBlockToFind = InputBox("Blockname waar op gezocht moet worden:", "Blocknaam", sBlockName)

            If sBlockToFind = "" Then
                Exit Sub 'cancel button
            End If
            replaceAttrib(True, True)
        End If
    End Sub

    Private Sub RenameAttributeTagsFromFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RenameAttributeTagsFromFileToolStripMenuItem.Click
        Dim dlgFileOpen As Forms.OpenFileDialog = New Forms.OpenFileDialog
        dlgFileOpen.Filter = "Attribute Definitions|*.attdef"
        If dlgFileOpen.ShowDialog() = DialogResult.OK Then
            Dim reader As StreamReader = My.Computer.FileSystem.OpenTextFileReader(dlgFileOpen.FileName, Encoding.Default)

            Dim line As String = Nothing
            Dim iRows As Integer = 0
            Dim iChanged As Integer = 0
            Try
                Do
                    line = reader.ReadLine
                    iRows = iRows + 1
                    If iRows >= 3 Then 'eerste 2 rijen overslaan
                        Dim sValues() As String = line.Split(";")
                        'juiste Attribute vinden
                        For Each attListItem As RN_attribute_listitem.AttributeListItem In flowLayouts.Controls
                            If attListItem.AttribName.ToUpper = sValues(0).ToUpper Then
                                attListItem.AttribCurrValue = sValues(1)
                                attListItem.AttribNewValue = sValues(1)
                                attListItem.IsItemSelected = True
                                attListItem.Update()
                                Exit For
                            End If
                        Next
                    End If
                Loop Until line Is Nothing
            Catch

            End Try

            'sBlockToFind = InputBox("Blockname waar op gezocht moet worden:", "Blocknaam", sBlockName)

            'If sBlockToFind = "" Then
            '    Exit Sub 'cancel button
            'End If
            'replaceAttrib(True, True)
        End If
    End Sub



    '    Private Sub cmdDynLayout_Click(sender As Object, e As EventArgs) Handles cmdDynLayout.Click
    '        If cmbNewLayout.Text.Length = 0 Then
    '            MsgBox("Selecteer eerst een layout!")
    '            Exit Sub
    '        End If
    '        'clsDynamicLayout.DynamicLayout()
    '        Dim cntMenu As ContextMenuStrip = New ContextMenuStrip()
    '        Dim mnuItm As ToolStripMenuItem

    '#Region "TODO"
    '        'menu dynamisch inlezen uit JSON / XML en opbouwen
    '#End Region
    '        mnuItm = New ToolStripMenuItem()
    '        mnuItm.Text = "1:50"
    '        mnuItm.Tag = "50"
    '        AddHandler mnuItm.Click, AddressOf ShowVPextendsRange
    '        cntMenu.Items.Add(mnuItm)
    '        mnuItm = New ToolStripMenuItem()
    '        mnuItm.Text = "1:100"
    '        mnuItm.Tag = "100"
    '        AddHandler mnuItm.Click, AddressOf ShowVPextendsRange
    '        cntMenu.Items.Add(mnuItm)
    '        mnuItm = New ToolStripMenuItem()
    '        mnuItm.Text = "1:200"
    '        mnuItm.Tag = "200"
    '        AddHandler mnuItm.Click, AddressOf ShowVPextendsRange
    '        cntMenu.Items.Add(mnuItm)
    '        mnuItm = New ToolStripMenuItem()
    '        mnuItm.Text = "1:500"
    '        mnuItm.Tag = "500"
    '        AddHandler mnuItm.Click, AddressOf ShowVPextendsRange
    '        cntMenu.Items.Add(mnuItm)
    '        mnuItm = New ToolStripMenuItem()
    '        mnuItm.Text = "1:1000"
    '        mnuItm.Tag = "1000"
    '        AddHandler mnuItm.Click, AddressOf ShowVPextendsRange
    '        cntMenu.Items.Add(mnuItm)
    '        'mnuItm = New ToolStripMenuItem()
    '        'mnuItm.Text = "RANGETEST"
    '        'mnuItm.Tag = "200"
    '        'AddHandler mnuItm.Click, AddressOf ShowVPextendsRange
    '        'cntMenu.Items.Add(mnuItm)

    '        cntMenu.Show(cmdDynLayout, 0, 0)

    '    End Sub

    Public Function ShowVPextends(ByVal sender As Object, ByVal e As EventArgs)
        Dim mnuItmClick As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        'layout type achterhalen en checken of het voldoet aan de eisen
        Dim sTemp As String = cmbNewLayout.Text
        Dim sVpType As String '= cmbNewLayout.Text.Substring(0, 5)
        Try
            If sTemp.Length >= 5 Then
                sVpType = sTemp.Substring(0, 5)
            Else
                sVpType = sTemp.Substring(0, 2)
                Select Case sVpType
                    Case "A0", "A1", "A2", "A3", "A4"
                        'geen orientatie beschikbaar, default LS
                        sVpType = sVpType & "_LS"
                    Case Else
                        'layout niet ondersteund voor dynamisch inserten
                        insertLayout(False)
                        Exit Function
                End Select
            End If
        Catch ex As Exception
            'ging iets fout bij het uitlezen
            insertLayout(False)
            Exit Function
        End Try

        vpScale = mnuItmClick.Text
        vpCustScale = mnuItmClick.Tag
        'vpCoordinates = clsDynamicLayout.DynamicLayout(sVpType & "-" & mnuItmClick.Text, sLayoutTemplate, sVpType)
        vpCoordinates = clsDynamicLayout.DynamicLayout(vpScale, sLayoutTemplate, sVpType)
        If vpCoordinates.vpEmpty = False Then
            'insert Layout hier
            insertLayout(True)
        End If
        Return True
    End Function

    Public Function ShowVPextendsRange(ByVal sender As Object, ByVal e As EventArgs)
        vpCoordinatesList = New ViewPortCoordinatesList

        Dim mnuItmClick As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        'Dim sVpType As String = cmbNewLayout.Text.Substring(0, 5)
        Dim sTemp As String = cmbNewLayout.Text
        Dim sVpType As String '= cmbNewLayout.Text.Substring(0, 5)

        'DEBUG logging
        clsFunctions.makeLog(sDebugLog, "START Dynamisch invoegen layout ShowVPextendsRange()", True)
        clsFunctions.makeLog(sDebugLog, "Gekozen layout: " & sTemp, False)

        Try
            If sTemp.Length >= 5 Then
                sVpType = sTemp.Substring(0, 5)
                If sTemp.Substring(2, 3) = "_LS" Or sTemp.Substring(2, 3) = "_PT" Then
                    'layout is goed
                Else
                    sVpType = sTemp.Substring(0, 2)
                    Select Case sVpType
                        Case "A0", "A1", "A2", "A3", "A4"
                            'geen orientatie beschikbaar, default LS
                            sVpType = sVpType & "_LS"
                        Case Else
                            'check of het toevalig PGL of PFL is
                            If (sTemp.Contains("PGL") Or sTemp.Contains("PFL")) Then
                                'pgl of pfl layout, gehele string gebruiken
                                sVpType = sTemp
                            Else
                                'layout niet ondersteund voor dynamisch inserten
                                insertLayout(False)
                                Return False
                                Exit Function
                            End If
                    End Select
                End If
            Else
                sVpType = sTemp.Substring(0, 2)
                Select Case sVpType
                    Case "A0", "A1", "A2", "A3", "A4"
                        'geen orientatie beschikbaar, default LS
                        sVpType = sVpType & "_LS"
                    Case Else
                        'layout niet ondersteund voor dynamisch inserten
                        insertLayout(False)
                        Return False
                        Exit Function
                End Select
            End If
        Catch ex As Exception
            'ging iets fout bij het uitlezen
            insertLayout(False)
            Return False
            Exit Function
        End Try

        vpScale = mnuItmClick.Text
        vpCustScale = mnuItmClick.Tag

        'tijdelijke override van vars voor TEST
        'vpScale = "1:200"
        'vpCustScale = "200"
        'sVpType = "A1_LS"
        Dim bLoopLayouts As Boolean = False
        vpCoordinatesList = New ViewPortCoordinatesList

        Using acLockDoc As DocumentLock = acDoc.LockDocument()
            'DEBUG logging
            clsFunctions.makeLog(sDebugLog, "acDoc.LockDocument()", False)
            Using acTrans As Transaction = acDoc.TransactionManager.StartTransaction
                'DEBUG logging
                clsFunctions.makeLog(sDebugLog, "acDoc.TransactionManager.StartTransaction", False)
                Dim acTypeValAr(0) As TypedValue
                Dim pKeyOpts As PromptKeywordOptions

                pKeyOpts = New PromptKeywordOptions(vbCrLf & " Hoeveel layouts wil je aanmaken?: ")
                pKeyOpts.Keywords.Add("Een")
                pKeyOpts.Keywords.Add("Meerdere")
                pKeyOpts.AllowNone = False
                Dim pKeyRes As PromptResult = acEd.GetKeywords(pKeyOpts)
                If pKeyRes.Status = PromptStatus.Cancel Then
                    'default een layout
                    bLoopLayouts = False
                    'DEBUG logging
                    clsFunctions.makeLog(sDebugLog, "Prompt=>cancelled, bLoopLayouts=False", False)
                    Return False
                    Exit Function
                Else
                    Select Case pKeyRes.StringResult.ToLower
                        Case "een"
                            bLoopLayouts = False
                            'DEBUG logging
                            clsFunctions.makeLog(sDebugLog, "Prompt=>choose: een, bLoopLayouts=False", False)
                        Case "meerdere"
                            bLoopLayouts = True
                        Case Else
                            'onbekende keuze, zou niet mogen gebeuren maar Just In Case
                            bLoopLayouts = False
                            'DEBUG logging
                            clsFunctions.makeLog(sDebugLog, "Prompt=>choose: Unknown, bLoopLayouts=False", False)
                            Return False
                            Exit Function
                    End Select
                End If
            End Using
        End Using

        'layouts plaatsen
        If bLoopLayouts = False Then
            'een enkele layout
            'DEBUG logging
            clsFunctions.makeLog(sDebugLog, "Start weergave ENKEL DynamicLayout(" & vpScale & ", " & sLayoutTemplate & ", " & sVpType & ")", False)
            vpCoordinates = clsDynamicLayout.DynamicLayout(vpScale, sLayoutTemplate, sVpType)
            If vpCoordinates.vpEmpty = False Then
                vpCoordinatesList.Add(vpCoordinates)
            End If
        Else
            'meerdere layouts
            'DEBUG logging
            clsFunctions.makeLog(sDebugLog, "Start weergave MEERDERE DynamicLayout(" & vpScale & ", " & sLayoutTemplate & ", " & sVpType & ")", False)
            Dim bMakeNewVP As Boolean = True
            While bMakeNewVP = True
                vpCoordinates = clsDynamicLayout.DynamicLayout(vpScale, sLayoutTemplate, sVpType)
                If vpCoordinates.vpEmpty = False Then
                    vpCoordinatesList.Add(vpCoordinates)
                Else
                    bMakeNewVP = False
                End If
            End While
        End If
        Dim sNewLayoutName As String
        sNewLayoutName = InputBox("Layout naam", "Layout naam", cmbNewLayout.Text)
        'DEBUG logging
        clsFunctions.makeLog(sDebugLog, "InputBox: Layout naam? " & sNewLayoutName, False)

        'insert layouts hier
        insertLayout(True, sNewLayoutName, bLoopLayouts)
        Return True
    End Function

    Private Sub cmdDynLayout_MouseDown(sender As Object, e As MouseEventArgs) Handles cmdDynLayout.MouseDown
        If e.Button = MouseButtons.Left Then
            If cmbNewLayout.Text.Length = 0 Then
                MsgBox("Selecteer eerst een layout!")
                Exit Sub
            End If
            Dim cntMenu As ContextMenuStrip = New ContextMenuStrip()
            Dim mnuItm As ToolStripMenuItem
            Dim sFormaat As String = cmbNewLayout.Text
            'menu dynamisch inlezen uit JSON 
            Dim sFilePath As String = clsFunctions.getCoreDir & "\viewports.json"
            Dim plVirtViewport As Autodesk.AutoCAD.DatabaseServices.Polyline = New Autodesk.AutoCAD.DatabaseServices.Polyline
            If File.Exists(sFilePath) Then
                Dim sJson As String = File.ReadAllText(sFilePath)
                Dim myJsonObject As JObject = JObject.Parse(sJson)
                Dim aPresets As JArray = myJsonObject("viewports")
                For i = 0 To aPresets.Count - 1
                    If aPresets(i).SelectToken("formaat").ToString = sFormaat Then
                        'dit is het juiste formaat blad, nu de juiste schaal vinden
                        Dim aSchalen As JArray = aPresets(i).SelectToken("schalen")
                        For x = 0 To aSchalen.Count - 1
                            mnuItm = New ToolStripMenuItem()
                            mnuItm.Text = aSchalen(x).SelectToken("schaal")
                            Dim tmpVar() As String = aSchalen(x).SelectToken("schaal").ToString.Split(":")
                            mnuItm.Tag = tmpVar(1)
                            AddHandler mnuItm.Click, AddressOf ShowVPextendsRange
                            cntMenu.Items.Add(mnuItm)
                        Next
                    End If
                Next
            End If
            cntMenu.Show(cmdDynLayout, 0, 0)
        ElseIf e.Button = MouseButtons.Right Then
            'schalen laten zien voor de geselecteerde layout
            Dim sFormaat As String = cmbNewLayout.Text
            'clsDynamicLayout.LoadViewports(sFormaat)
            clsDynamicLayout.showViewports(sFormaat)
        End If
    End Sub

End Class
