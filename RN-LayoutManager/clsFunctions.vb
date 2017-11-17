Imports System.IO
Imports System.Reflection
Imports Newtonsoft.Json.Linq

Public Class clsFunctions
    Public Shared Function getCoreDir() As String
        Dim strAssemblyPath As String = Assembly.GetExecutingAssembly.Location
        Dim strTmpPath() As String = strAssemblyPath.Split("\")
        Dim arrUbound As Integer = UBound(strTmpPath)
        getCoreDir = strAssemblyPath.Replace(strTmpPath(arrUbound), "")
    End Function


    Public Shared Function loadPlotPresets(ByVal sPresetFile As String, ByRef dtPlotPresets As System.Data.DataTable, ByRef iSelectedIndex As Integer, ByVal sCurrentPreset As String)
        'bestaand bestand inladen
        dtPlotPresets.Columns.Add("id")
        dtPlotPresets.Columns.Add("preset")
        If File.Exists(sPresetFile) Then
            Dim sJson As String = File.ReadAllText(sPresetFile)
            Dim myJsonObject As JObject = JObject.Parse(sJson)
            Dim aPresets As JArray = myJsonObject("presets")
            For i = 0 To aPresets.Count - 1
                Dim sPreset As String = aPresets(i).SelectToken("preset").ToString
                If sPreset = sCurrentPreset Then
                    iSelectedIndex = i
                End If

                Dim dtRow As DataRow = dtPlotPresets.NewRow()
                dtRow(0) = i
                dtRow(1) = sPreset
                dtPlotPresets.Rows.Add(dtRow)
            Next
            Return True
        Else
            Return False
        End If
    End Function

    ' dtPlotPresets.Columns.Add("id")
    '        dtRasterServicesZoom.Columns.Add("zoomlevel")
    '        dtRasterServicesZoom.Columns.Add("unitsperpixel")

    '        Dim sJsonResponse As String = File.ReadAllText(sDataSet & sRasterJSON)
    'Dim myObject As JObject = JObject.Parse(sJsonResponse)
    'Dim aItems As JArray = myObject("items")

    'Dim aZoomLevel As JArray = aItems(iId).SelectToken("zoomlevels")
    'For i As Integer = 0 To aZoomLevel.Count - 1
    'Dim dtRow As DataRow = dtRasterServicesZoom.NewRow()
    'Dim sZoomLevel As String = aZoomLevel(i).SelectToken("level")
    'Dim sUnitsPerPixel As String = aZoomLevel(i).SelectToken("units-per-pixel")
    '            dtRow(0) = sZoomLevel
    '            dtRow(1) = sZoomLevel
    '            dtRow(2) = sUnitsPerPixel
    '            dtRasterServicesZoom.Rows.Add(dtRow)
    '        Next
End Class
