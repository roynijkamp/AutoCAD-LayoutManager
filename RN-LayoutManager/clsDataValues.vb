Imports Autodesk.AutoCAD.Geometry

Public Class clsDataValues

End Class

Public Class ViewPortCoordinates
    Public Property vpEmpty As Boolean
    Public Property vpCenter As Point2d
    Public Property vpExtMin As Point2d
    Public Property vpExtMax As Point2d
    Public Property vpRotation As Double
End Class

Public Class ViewPortCoordinatesList
    Public Property ViewPorts As New List(Of ViewPortCoordinates)

    Public Sub Add(ViewPort As ViewPortCoordinates)
        ViewPorts.Add(ViewPort)
    End Sub
End Class

'Public Class ViewPortTest
'    Public Property VpTest As New List(Of ViewPortCoordinates)

'    Public Sub Add(vp As ViewPortCoordinates)
'        VpTest.Add(vp)
'    End Sub
'End Class