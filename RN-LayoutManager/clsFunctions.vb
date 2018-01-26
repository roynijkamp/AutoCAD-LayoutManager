Imports System.IO
Imports System.Reflection
Imports Autodesk.AutoCAD.ApplicationServices
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

    '<summary>
    'log progress
    '</summary>
    Public Shared Sub makeLog(ByVal sLogFile As String, ByVal sMessage As String, Optional bVersionInfo As Boolean = False)
        Try
            If Not Directory.Exists(Path.GetDirectoryName(sLogFile)) Then
                Directory.CreateDirectory(Path.GetDirectoryName(sLogFile))
            End If
            Dim dtDatum As DateTime = DateTime.Now
            If Not File.Exists(sLogFile) Then
                Using sw As StreamWriter = File.CreateText(sLogFile)
                    sw.WriteLine("LOG FILE AANGEMAAKT OP: " & dtDatum.ToString("dd/MM/yyyy HH:mm:ss"))
                    sw.WriteLine("----------------------------------------------------------------")
                End Using
            End If
            Using sw As StreamWriter = File.AppendText(sLogFile)
                If bVersionInfo Then
                    sw.WriteLine("Computer: " & My.Computer.Name)
                    Dim sAcadVersion As String = clsFunctions.AcadVersion.Major.ToString & "." & clsFunctions.AcadVersion.Minor.ToString & "." & clsFunctions.AcadVersion.Revision.ToString
                    sw.WriteLine("Autodesk Software: R" & sAcadVersion)
                End If
                sw.WriteLine(dtDatum.ToString("dd/MM/yyyy HH:mm:ss") & " - " & sMessage)
            End Using
        Catch ex As Exception
            MsgBox("Fout bij maken LOG")
        End Try
    End Sub

    Public Shared ReadOnly Property AcadVersion() As Version
        Get
            Return GetType(Document).Assembly.GetName().Version
        End Get
    End Property

    Public Shared Function isDebugMode() As Boolean
        Return IsInDebugMode(Assembly.GetExecutingAssembly.Location, False)
    End Function

    Public Overloads Shared Function IsInDebugMode(ByVal FileName As String, ByVal IsAssemlbyName As Boolean) As Boolean
        Dim assembly As System.Reflection.Assembly
        If IsAssemlbyName Then
            assembly = System.Reflection.Assembly.Load(FileName)
        Else
            assembly = System.Reflection.Assembly.LoadFile(FileName)
        End If

        Return IsInDebugMode(assembly)
    End Function

    Public Overloads Shared Function IsInDebugMode(ByVal Assembly As System.Reflection.Assembly) As Boolean
        Dim attributes = Assembly.GetCustomAttributes(GetType(System.Diagnostics.DebuggableAttribute), False)
        If (attributes.Length > 0) Then
            Dim debuggable = CType(attributes(0), System.Diagnostics.DebuggableAttribute)
            If (Not (debuggable) Is Nothing) Then
                Return ((debuggable.DebuggingFlags And System.Diagnostics.DebuggableAttribute.DebuggingModes.Default) _
                            = System.Diagnostics.DebuggableAttribute.DebuggingModes.Default)
            Else
                Return False
            End If

        Else
            Return False
        End If

    End Function
End Class
