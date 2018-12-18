Imports System.IO
Imports System.Reflection
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Interop
Imports Newtonsoft.Json.Linq

Public Class clsFunctions
    Public Shared Function getCoreDir() As String
        Dim strAssemblyPath As String = Assembly.GetExecutingAssembly.Location
        Dim strTmpPath() As String = strAssemblyPath.Split("\")
        Dim arrUbound As Integer = UBound(strTmpPath)
        getCoreDir = strAssemblyPath.Replace(strTmpPath(arrUbound), "").TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
    End Function

    Public Shared Function ParseDigits(ByVal strRawValue As String) As String
        Dim strDigits As String = ""
        If strRawValue = Nothing Then Return strDigits


        For Each c As Char In strRawValue.ToCharArray()
            If IsNumeric(c) Then
                strDigits &= c
            ElseIf c = "." Then
                strDigits &= c
            End If
        Next c


        ' return the number string, or "" if no numbers were in the string.
        Return strDigits
    End Function
    ''' <summary>
    ''' 'Set insertion units
    ''' </summary>
    Public Shared Sub PrefsSetUnits()
        '' Set insertion units to meters

        '' Access the Preferences object
        Dim acPrefComObj As AcadPreferences = Autodesk.AutoCAD.ApplicationServices.Application.Preferences

        '' Disable the scroll bars
        'acPrefComObj.Display.DisplayScrollBars = False
        acPrefComObj.User.ADCInsertUnitsDefaultSource = Common.AcInsertUnits.acInsertUnitsMeters
        acPrefComObj.User.ADCInsertUnitsDefaultTarget = Common.AcInsertUnits.acInsertUnitsMeters
    End Sub


    Public Shared Sub switchToModalspace()
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument

        Using acLockDoc As DocumentLock = acDoc.LockDocument
            Dim acLayoutMgr As LayoutManager = LayoutManager.Current
            acLayoutMgr.CurrentLayout = "Model"
            'acDoc.Editor.Regen()
        End Using

    End Sub

    ''' <summary>
    ''' Get plot presets
    ''' </summary>
    ''' <param name="sPresetFile"></param>
    ''' <param name="dtPlotPresets"></param>
    ''' <param name="iSelectedIndex"></param>
    ''' <param name="sCurrentPreset"></param>
    ''' <returns></returns>
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

    Public Shared Function loadPlotPresets(ByVal sPresetFile As String) As List(Of String)
        'bestaand bestand inladen
        Dim sPlotDevices As List(Of String) = New List(Of String)

        If File.Exists(sPresetFile) Then
            Dim sJson As String = File.ReadAllText(sPresetFile)
            Dim myJsonObject As JObject = JObject.Parse(sJson)
            Dim aPresets As JArray = myJsonObject("presets")
            For i = 0 To aPresets.Count - 1
                Dim sPreset As String = aPresets(i).SelectToken("preset").ToString
                If Not sPreset.ToLower = "dwf" Then
                    sPlotDevices.Add(sPreset)
                End If
            Next
            loadPlotPresets = sPlotDevices
        Else
            loadPlotPresets = sPlotDevices
        End If
    End Function


    ''' <summary>
    ''' Get Current plotstyle
    ''' </summary>
    ''' <returns>integer 0 = STB, 1 = CTB</returns>
    Public Shared Function getPlotStyleTable() As Integer
        'plotstyletable achterhalen en instellen
        '1 = CTB
        '0 = STB
        Dim oCurrPlotStyleTable As System.Object = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("PSTYLEMODE")
        getPlotStyleTable = CInt(oCurrPlotStyleTable.ToString)
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


    Public Shared Function startCommandMonitor() As Boolean
        'To avoid ambiguity, we have to use the full type here.
        Dim doc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim db As Database = HostApplicationServices.WorkingDatabase()
        'AddHandler db.ObjectOpenedForModify, New ObjectEventHandler(AddressOf objOpenedForMod)
        AddHandler doc.CommandWillStart, New CommandEventHandler(AddressOf commandStart)
        AddHandler doc.CommandEnded, New CommandEventHandler(AddressOf commandEnd)
        'modFunctions.writeLOGtxt("Command monitor started!")
        startCommandMonitor = True
    End Function

    Public Shared Function endCommandMonitor() As Boolean
        Dim doc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim db As Database = HostApplicationServices.WorkingDatabase()

        RemoveHandler doc.CommandWillStart, AddressOf commandStart
        RemoveHandler doc.CommandEnded, AddressOf commandEnd
        endCommandMonitor = True
    End Function

    Public Shared Function commandStart(ByVal o As Object, ByVal e As CommandEventArgs)
        'MsgBox(e.GlobalCommandName)
    End Function

    Public Shared Function commandEnd(ByVal o As Object, ByVal e As CommandEventArgs)
        'MsgBox(e.GlobalCommandName)
    End Function

    Public Shared Function getActiveDrawings(Optional boolBlank As Boolean = False, Optional bSuppressMessage As Boolean = False)
        'functie om het aantal actieve tekeningen weer te geven
        Dim AcadDocs As DocumentCollection = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager
        'melding weergeven indien geen actieve tekening gevonden
        If AcadDocs.Count = 0 Then
            If bSuppressMessage = False Then
                MsgBox("Er is geen document geopend!" & vbCrLf & "Open eerst een document voordat u verder gaat.", MsgBoxStyle.Exclamation + vbOKOnly)
            End If
            Return AcadDocs.Count
        Else
            'kijken of we moeten checken of een Lege tekening actief is
            If boolBlank = True Then
                Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
                If acDoc.IsNamedDrawing = False Then
                    'lege tekening niet opgeslagen
                    Return False
                Else
                    Return True
                End If
            End If
            Return AcadDocs.Count
        End If
    End Function


End Class
