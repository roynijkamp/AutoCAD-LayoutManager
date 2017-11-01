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

Public Class ucLayoutManager
    Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
    Dim acCurDb As Database = acDoc.Database
    Dim acEd As Editor = acDoc.Editor
    Dim iCheckCount As Integer = 0 'counter for checks
    Dim drag_drop_scroll_amount As Integer = 0
    Private Sub LayoutManager_Load(sender As Object, e As EventArgs) Handles Me.Load
        loadLayouts()
    End Sub

    Private Sub cmdRefreshList_Click(sender As Object, e As EventArgs)
        loadLayouts()
    End Sub
    ''' <summary>
    ''' 'Load layouts into list
    ''' </summary>
    Public Sub loadLayouts()
        'reset check count
        iCheckCount = 0
        updateCheckLabel()
        'clear flow
        flowLayouts.Controls.Clear()
        flowLayouts.AllowDrop = True
        'first add model item
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = New RN_LayoutItems.RN_UCLayoutItem()
        myCntrl.LayoutName = "Model"
        myCntrl.IsModel = True
        drag_drop_scroll_amount = myCntrl.Height
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
        lblCheckCount.Text = "# " & iCheckCount.ToString & " selected"
    End Sub

    Public Function PlotLayout(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)

        plotSingleLayout(myCntrl.LayoutName)

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
                Dim acadApp As Object = Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication
                acadApp.ZoomExtents()
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

    Public Function checkedLayouts()
        Dim layouts As New List(Of Layout)()
        Try
            Using acLockDoc As DocumentLock = acDoc.LockDocument
                Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                    Dim acLayoutMgr As LayoutManager = LayoutManager.Current

                    For Each cntrl As RN_LayoutItems.RN_UCLayoutItem In flowLayouts.Controls
                        If cntrl.CheckState Then 'layout is checked, add to plot
                            Dim oId As ObjectId = acLayoutMgr.GetLayoutId(cntrl.LayoutName)
                            Dim lay As Layout = acTrans.GetObject(oId, OpenMode.ForWrite)
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
    End Function


    ''' <summary>
    ''' 'Plot multiple layouts to one multisheet PDF
    ''' </summary>
    ''' <param name="pdfSheetType"></param>
    ''' <returns></returns>
    Public Function plotLayouts(ByVal pdfSheetType As SheetType)
        If iCheckCount > 0 Then
            Dim db As Database = HostApplicationServices.WorkingDatabase
            Dim bgp As Short = CShort(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("BACKGROUNDPLOT"))
            Try
                Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("BACKGROUNDPLOT", 0)

                Dim layouts As New List(Of Layout)()
                layouts = checkedLayouts()

                Dim filename As String = Path.ChangeExtension(db.Filename, "pdf")

                Dim plotter As New plotting.MultiSheetsPdf(filename, layouts, pdfSheetType)
                    plotter.Publish()




            Catch e As System.Exception
                Dim ed As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
                ed.WriteMessage(vbLf & "Error: {0}" & vbLf & "{1}", e.Message, e.StackTrace)
            Finally
                Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("BACKGROUNDPLOT", bgp)
            End Try
            Return True
        Else
            MsgBox("Er zijn geen layouts geselecteerd!")
            Return False
        End If
    End Function


    Public Function plotSingleLayout(ByVal sLayoutName As String)
        If iCheckCount > 0 Then
            Dim db As Database = HostApplicationServices.WorkingDatabase
            Dim bgp As Short = CShort(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("BACKGROUNDPLOT"))
            Try
                Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("BACKGROUNDPLOT", 0)

            Catch e As System.Exception
                Dim ed As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
                ed.WriteMessage(vbLf & "Error: {0}" & vbLf & "{1}", e.Message, e.StackTrace)
            Finally
                Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("BACKGROUNDPLOT", bgp)
            End Try
            Return True
        Else
            MsgBox("Er zijn geen layouts geselecteerd!")
            Return False
        End If
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
        plotLayouts(SheetType.MultiPdf)
    End Sub

    Private Sub cmdPlotSingleSheet_Click(sender As Object, e As EventArgs) Handles cmdPlotSingleSheet.Click
        plotLayouts(SheetType.SinglePdf)
    End Sub
End Class
