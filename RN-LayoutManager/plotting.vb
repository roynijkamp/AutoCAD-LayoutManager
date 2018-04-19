Imports System.IO
Imports System.Linq
Imports System.Text
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.PlottingServices
Imports Autodesk.AutoCAD.Publishing
Imports Newtonsoft.Json.Linq

Public Class plotting
    Public Class MultiSheetsPdf
        Private sIniDir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\RNtools"
        Private sIniFile As String = "\layoutmanager.ini"
        Private sPlotPreferences As String = "PlotPresets.json"
        Private bTrashDSD As Boolean = True
        Private iniFile As clsINI
        Private sDefaultPlottingDevice As String = "AutoCAD PDF (General Documentation).PC3"

        Private dwgFile As String, pdfFile As String, dsdFile As String, outputDir As String
        Private sheetNum As Integer
        Private layouts As IEnumerable(Of Layout)
        Private pdfSheetType As SheetType = SheetType.MultiPdf
        Private pdfOutputDIR As String

        Private bSuppressMessage As Boolean

        Private Const LOG As String = "publish.log"

        Public Sub New(pdfFile As String, layouts As IEnumerable(Of Layout), pdfSheetType As SheetType, bSuppressMessage As Boolean)
            'get settings from INI
            iniFile = New clsINI(sIniDir & sIniFile)
            bTrashDSD = iniFile.GetBoolean("debugoptions", "trashdsd", bTrashDSD)

            Dim db As Database = HostApplicationServices.WorkingDatabase
            Me.dwgFile = db.Filename
            Me.pdfFile = pdfFile

            Me.outputDir = Path.GetDirectoryName(Me.pdfFile)

            Me.dsdFile = Path.ChangeExtension(Me.pdfFile, "dsd")
            Me.layouts = layouts
            Me.pdfSheetType = pdfSheetType
            Me.bSuppressMessage = bSuppressMessage
        End Sub

        Public Sub Publish()
            If TryCreateDSD() Then
                Dim publisher As Publisher = Autodesk.AutoCAD.ApplicationServices.Application.Publisher
                Dim plotDlg As New PlotProgressDialog(False, Me.sheetNum, True)
                publisher.PublishDsd(Me.dsdFile, plotDlg)
                plotDlg.Destroy()
                If bTrashDSD Then
                    File.Delete(Me.dsdFile)
                End If
                If bSuppressMessage = False Then
                    MsgBox("Plot bestanden aanmaken is voltooid!")
                End If
            Else
                MsgBox("Fout bij het maken van de plot bestanden / DSD file")
            End If
        End Sub


        Private Function TryCreateDSD() As Boolean
            Using dsd As New DsdData()
                Using dsdEntries As DsdEntryCollection = CreateDsdEntryCollection(Me.layouts)
                    If dsdEntries Is Nothing OrElse dsdEntries.Count <= 0 Then
                        Return False
                    End If

                    If Not Directory.Exists(Me.outputDir) Then
                        Directory.CreateDirectory(Me.outputDir)
                    End If

                    Me.sheetNum = dsdEntries.Count

                    dsd.SetDsdEntryCollection(dsdEntries)

                    dsd.SetUnrecognizedData("PwdProtectPublishedDWF", "FALSE")
                    dsd.SetUnrecognizedData("PromptForPwd", "FALSE")
                    dsd.SheetType = Me.pdfSheetType

                    dsd.NoOfCopies = 1
                    dsd.DestinationName = Me.pdfFile
                    'dsd.DestinationName = Me.outputDir & Me.pdfFile

                    dsd.IsHomogeneous = False
                    dsd.LogFilePath = Path.Combine(Me.outputDir, LOG)

                    PostProcessDSDnew(dsd)
                    Return True
                End Using
            End Using
        End Function

        Private Function CreateDsdEntryCollection(layouts As IEnumerable(Of Layout)) As DsdEntryCollection
            Dim entries As New DsdEntryCollection()

            For Each layout As Layout In layouts
                Dim dsdEntry As New DsdEntry()
                dsdEntry.DwgName = Me.dwgFile
                dsdEntry.Layout = layout.LayoutName
                dsdEntry.NpsSourceDwg = dsdEntry.DwgName
                If Me.pdfSheetType = SheetType.MultiPdf Or Me.pdfSheetType = SheetType.MultiDwf Then
                    dsdEntry.Title = layout.LayoutName
                    dsdEntry.Nps = layout.TabOrder.ToString()
                Else
                    dsdEntry.Title = Path.GetFileNameWithoutExtension(Me.dwgFile) + "-" + layout.LayoutName
                    dsdEntry.Nps = "Setup1"
                End If
                entries.Add(dsdEntry)
            Next
            Return entries
        End Function

        Private Sub PostProcessDSDnew(dsd As DsdData)
            Dim str As String, newStr As String
            Dim tmpFile As String = Path.Combine(Me.outputDir, "temp.dsd")

            dsd.WriteDsd(tmpFile)

            Using reader As New StreamReader(tmpFile, Encoding.[Default])
                Using writer As New StreamWriter(Me.dsdFile, False, Encoding.[Default])
                    While Not reader.EndOfStream
                        str = reader.ReadLine()
                        If str.Contains("Has3DDWF") Then
                            newStr = "Has3DDWF=0"
                        ElseIf str.Contains("OriginalSheetPath") Then
                            newStr = Convert.ToString("OriginalSheetPath=") & Me.dwgFile
                            'ElseIf str.Contains("Type") Then
                            '    If Me.pdfSheetType = SheetType.MultiPdf Then
                            '        'Multi Sheet PDF
                            '        newStr = "Type=6"
                            '    Else
                            '        'Single Sheet PDF
                            '        newStr = "Type=5"
                            '    End If

                        ElseIf str.Contains("OUT") Then
                            newStr = Convert.ToString("OUT=") & Me.outputDir
                            'ElseIf str.Contains("IncludeLayer") Then
                            '    newStr = "IncludeLayer=TRUE"
                        ElseIf str.Contains("PromptForDwfName") Then
                            newStr = "PromptForDwfName=FALSE"
                        ElseIf str.Contains("LogFilePath") Then
                            newStr = "LogFilePath=" + Path.Combine(Me.outputDir, LOG)
                        Else
                            newStr = str
                        End If
                        writer.WriteLine(newStr)
                    End While
                End Using
            End Using
            'delete tempfile
            File.Delete(tmpFile)


            'selected plot preset in options
            iniFile = New clsINI(sIniDir & sIniFile)
            Dim sSelectedPreset = iniFile.GetString("publishsettings", "defaultplotter", sDefaultPlottingDevice)
            'define settings
            Dim sPdfType As String = "5" 'default single sheet PDF
            If Me.pdfSheetType = SheetType.MultiPdf Then
                'Multi Sheet PDF
                sPdfType = "6"
            ElseIf Me.pdfSheetType = SheetType.SinglePdf Then
                'Single Sheet PDF
                sPdfType = "5"
            ElseIf Me.pdfSheetType = SheetType.MultiDwf Then
                sPdfType = "1"
                sSelectedPreset = "DWF"
            ElseIf Me.pdfSheetType = SheetType.SingleDwf Then
                sPdfType = "0"
                sSelectedPreset = "DWF"
            End If

            'write dsd file
            'dsd.WriteDsd(Me.dsdFile)

            'load DSD file for modify
            iniFile = New clsINI(Me.dsdFile)

            'read settings
            Dim sPresetFile As String = clsFunctions.getCoreDir() & sPlotPreferences
            If File.Exists(sPresetFile) Then
                Dim sJson As String = File.ReadAllText(sPresetFile)
                Dim myJsonObject As JObject = JObject.Parse(sJson)
                Dim aPresets As JArray = myJsonObject("presets")
                For i = 0 To aPresets.Count - 1
                    Dim sPreset As String = aPresets(i).SelectToken("preset").ToString
                    If sPreset = sSelectedPreset Then
                        Try
                            'deze settings moeten we inlezen en verwerken
                            'eerste pdf type invullen
                            iniFile.WriteString("Target", "Type", sPdfType)
                            Dim aPdfOptions As JArray = aPresets(i).SelectToken("PdfOptions")
                            For x = 0 To aPdfOptions.Count - 1
                                Dim sKey As String = aPdfOptions(x).SelectToken("item").ToString
                                Dim sValue As String = aPdfOptions(x).SelectToken("waarde").ToString
                                iniFile.WriteString("PdfOptions", sKey, sValue)
                            Next
                            aPdfOptions = aPresets(i).SelectToken("SheetSet_Properties")
                            For x = 0 To aPdfOptions.Count - 1
                                Dim sKey As String = aPdfOptions(x).SelectToken("item").ToString
                                Dim sValue As String = aPdfOptions(x).SelectToken("waarde").ToString
                                iniFile.WriteString("SheetSet Properties", sKey, sValue)
                            Next
                        Catch ex As Exception
                            MsgBox("Fout bij het schrijven in de DSD file" & vbCrLf & ex.Message & vbCrLf & ex.Source & vbCrLf & ex.StackTrace)
                        End Try

                    End If
                Next
            End If
        End Sub

        Private Sub PostProcessDSD(dsd As DsdData)
            Dim str As String, newStr As String
            Dim tmpFile As String = Path.Combine(Me.outputDir, "temp.dsd")

            dsd.WriteDsd(tmpFile)

            Using reader As New StreamReader(tmpFile, Encoding.[Default])
                Using writer As New StreamWriter(Me.dsdFile, False, Encoding.[Default])
                    While Not reader.EndOfStream
                        str = reader.ReadLine()
                        If str.Contains("Has3DDWF") Then
                            newStr = "Has3DDWF=0"
                        ElseIf str.Contains("OriginalSheetPath") Then
                            newStr = Convert.ToString("OriginalSheetPath=") & Me.dwgFile
                        ElseIf str.Contains("Type") Then
                            If Me.pdfSheetType = SheetType.MultiPdf Then
                                'Multi Sheet PDF
                                newStr = "Type=6"
                            Else
                                'Single Sheet PDF
                                newStr = "Type=5"
                            End If

                        ElseIf str.Contains("OUT") Then
                            newStr = Convert.ToString("OUT=") & Me.outputDir
                        ElseIf str.Contains("IncludeLayer") Then
                            newStr = "IncludeLayer=TRUE"
                        ElseIf str.Contains("PromptForDwfName") Then
                            newStr = "PromptForDwfName=FALSE"
                        ElseIf str.Contains("LogFilePath") Then
                            newStr = "LogFilePath=" + Path.Combine(Me.outputDir, LOG)
                        Else
                            newStr = str
                        End If
                        writer.WriteLine(newStr)
                    End While
                End Using
            End Using

            File.Delete(tmpFile)

        End Sub
    End Class


End Class
