Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Windows

Public Class ucLayoutManager
    Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
    Dim acCurDb As Database = acDoc.Database
    Dim acEd As Editor = acDoc.Editor
    Private Sub LayoutManager_Load(sender As Object, e As EventArgs) Handles Me.Load
        loadLayouts()
    End Sub

    Private Sub cmdRefreshList_Click(sender As Object, e As EventArgs) Handles cmdRefreshList.Click
        loadLayouts()
    End Sub
    ''' <summary>
    ''' 'Load layouts into list
    ''' </summary>
    Public Sub loadLayouts()
        flowLayouts.Controls.Clear()
        ' Get the layout dictionary of the current database
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
            Dim lays As DBDictionary = acTrans.GetObject(acCurDb.LayoutDictionaryId, OpenMode.ForRead)

            ' Step through and list each named layout and Model
            For Each item As DBDictionaryEntry In lays
                'add name to list except Model
                If item.Key = "Model" Then
                    Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = New RN_LayoutItems.RN_UCLayoutItem()
                    myCntrl.LayoutName = item.Key
                    myCntrl.updateItem()
                    'only handler to change view since model can't be renamed
                    AddHandler myCntrl.View_Click, AddressOf ItemViewClick
                    flowLayouts.Controls.Add(myCntrl)
                Else
                    Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = New RN_LayoutItems.RN_UCLayoutItem()
                    myCntrl.LayoutName = item.Key
                    myCntrl.updateItem()
                    'add handlers to register functions for items
                    AddHandler myCntrl.View_Click, AddressOf ItemViewClick
                    AddHandler myCntrl.LayoutNameEdit_KeyDown, AddressOf renameLayout
                    AddHandler myCntrl.Plot_Click, AddressOf PlotLayout
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

    Public Function PlotLayout(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)

        Dim dItmCnt As Double = flowLayouts.Controls.Count - 1
        Dim iRandom As Integer = CInt(Math.Ceiling(Rnd() * dItmCnt)) + 1

        Dim iCurrIndex As Integer = flowLayouts.Controls.GetChildIndex(myCntrl)
        flowLayouts.Controls.SetChildIndex(myCntrl, iRandom)

        Return True
    End Function

    Public Sub ItemViewClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = CType(sender, RN_LayoutItems.RN_UCLayoutItem)
        Using acLockDoc As DocumentLock = acDoc.LockDocument
            Dim acLayoutMgr As LayoutManager = LayoutManager.Current
            acLayoutMgr.CurrentLayout = myCntrl.LayoutName
            acDoc.Editor.Regen()
        End Using
    End Sub

End Class
