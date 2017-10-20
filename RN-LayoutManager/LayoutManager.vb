Public Class LayoutManager
    Private Sub LayoutManager_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Private Sub cmdRefreshList_Click(sender As Object, e As EventArgs) Handles cmdRefreshList.Click
        loadLayouts()
    End Sub

    Public Sub loadLayouts()
        flowLayouts.Controls.Clear()
        For i = 0 To 10
            Dim myCntrl As RN_LayoutItems.RN_UCLayoutItem = New RN_LayoutItems.RN_UCLayoutItem()
            myCntrl.LayoutName = "Layout " & i.ToString
            myCntrl.updateItem()
            flowLayouts.Controls.Add(myCntrl)
        Next
    End Sub
End Class
