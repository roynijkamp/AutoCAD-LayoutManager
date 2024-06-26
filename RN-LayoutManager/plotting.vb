﻿Imports System.IO
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
        Private sPlotPreferences As String = "\PlotPresets.json"
        Private bTrashDSD As Boolean = True
        Private iniFile As clsINI
        Private sDefaultPlottingDevice As String = "AutoCAD PDF (General Documentation).PC3"

        Private dwgFile As String, pdfFile As String, dsdFile As String, outputDir As String
        Private sheetNum As Integer
        Private layouts As IEnumerable(Of Layout)
        Private pdfSheetType As SheetType = SheetType.MultiPdf
        Private pdfOutputDIR As String
        Private sPlottingDeviceOverride As String = ""
        Private bUseDWGname As Boolean = True
        Private sLayouts As Dictionary(Of String, Dictionary(Of String, String))
        Private bUseBesteknr As Boolean = False
        Private bUseBladnr As Boolean = False
        Private bUseVersie As Boolean = False
        Private bVersieInFront As Boolean = False

        Private bSuppressMessage As Boolean

        Private Const LOG As String = "publish.log"

        Public Sub New(pdfFile As String, layouts As IEnumerable(Of Layout), pdfSheetType As SheetType, bSuppressMessage As Boolean, Optional sPlottingDeviceOverride As String = "", Optional sLayouts As Dictionary(Of String, Dictionary(Of String, String)) = Nothing)
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
            Me.sPlottingDeviceOverride = sPlottingDeviceOverride 'plotter override 
            Me.bUseDWGname = iniFile.GetBoolean("publishsettings", "usedwgname", bUseDWGname)
            Me.bUseBesteknr = iniFile.GetBoolean("publishsettings", "usebesteknummer", bUseBesteknr)
            Me.bUseBladnr = iniFile.GetBoolean("publishsettings", "usebladnummer", bUseBladnr)
            Me.bUseVersie = iniFile.GetBoolean("publishsettings", "useversie", bUseVersie)
            Me.bVersieInFront = iniFile.GetBoolean("publishsettings", "versieinfront", bVersieInFront)
            Me.sLayouts = sLayouts
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
            Dim sFileName As String = ""
            Dim sPreffix As String = ""
            Dim sSuffix As String = ""
            Dim sLayoutNaam As String = ""

            For Each layout As Layout In layouts
                Dim dsdEntry As New DsdEntry()
                sFileName = ""
                sPreffix = ""
                sSuffix = ""
                dsdEntry.DwgName = Me.dwgFile
                dsdEntry.Layout = layout.LayoutName
                dsdEntry.NpsSourceDwg = dsdEntry.DwgName

                If sLayouts.ContainsKey(layout.LayoutName) Then
                    'kijken of we de juiste attributen kunnen opbouwen
                    Dim dictAttrib As Dictionary(Of String, String) = sLayouts.Item(layout.LayoutName)
                    sLayoutNaam = layout.LayoutName
                    'If dictAttrib.ContainsKey("VERSIE") And bUseVersie = True Then
                    '    sFileName = sFileName & "V" & dictAttrib.Item("VERSIE") & " "
                    'End If
                    'attributen voor juiste bedrijf selectern
                    If dictAttrib.ContainsKey("BEDRIJF") Then
                        If dictAttrib.Item("BEDRIJF") = "ANACON" Then

                            If dictAttrib.ContainsKey("BESTEKNUMMER") And bUseBesteknr = True Then
                                sPreffix = sPreffix & dictAttrib.Item("BESTEKNUMMER") & "-"
                            End If
                            If dictAttrib.ContainsKey("BLADNUMMER") And bUseBladnr = True Then
                                sPreffix = sPreffix & dictAttrib.Item("BLADNUMMER") & " "
                            End If
                            If dictAttrib.ContainsKey("VERSIE") And bUseVersie = True Then
                                If bVersieInFront Then
                                    sPreffix = "V" & dictAttrib.Item("VERSIE") & " " & sPreffix
                                Else
                                    sSuffix = " - V" & dictAttrib.Item("VERSIE")
                                End If
                            End If
                        ElseIf dictAttrib.Item("BEDRIJF") = "PRVGLD" Then
                            If dictAttrib.ContainsKey("NL_META_TEKENINGNUMMER") And bUseBesteknr = True Then
                                sPreffix = sPreffix & dictAttrib.Item("NL_META_TEKENINGNUMMER")
                            End If
                            If dictAttrib.ContainsKey("NL_META_VERSIE") And bUseVersie = True Then
                                If bVersieInFront Then
                                    sPreffix = "V" & dictAttrib.Item("NL_META_VERSIE") & " " & sPreffix
                                Else
                                    sSuffix = " - V" & dictAttrib.Item("NL_META_VERSIE")
                                End If
                            End If
                            sLayoutNaam = "" 'layoutnaam zit al in tekeningnummer
                        Else 'onbekend bedrijf, hopen dat de standaard wat oplevert
                            If dictAttrib.ContainsKey("BESTEKNUMMER") And bUseBesteknr = True Then
                                sPreffix = sPreffix & dictAttrib.Item("BESTEKNUMMER") & "-"
                            End If
                            If dictAttrib.ContainsKey("BLADNUMMER") And bUseBladnr = True Then
                                sPreffix = sPreffix & dictAttrib.Item("BLADNUMMER") & " "
                            End If
                            If dictAttrib.ContainsKey("VERSIE") And bUseVersie = True Then
                                If bVersieInFront Then
                                    sPreffix = "V" & dictAttrib.Item("VERSIE") & " " & sPreffix
                                Else
                                    sSuffix = " - V" & dictAttrib.Item("VERSIE")
                                End If
                            End If
                        End If
                    Else
                        'onbekend bedrijf, hopen dat de standaard wat oplevert
                        If dictAttrib.ContainsKey("BESTEKNUMMER") And bUseBesteknr = True Then
                            sPreffix = sPreffix & dictAttrib.Item("BESTEKNUMMER") & "-"
                        End If
                        If dictAttrib.ContainsKey("BLADNUMMER") And bUseBladnr = True Then
                            sPreffix = sPreffix & dictAttrib.Item("BLADNUMMER") & " "
                        End If
                        If dictAttrib.ContainsKey("VERSIE") And bUseVersie = True Then
                            If bVersieInFront Then
                                sPreffix = "V" & dictAttrib.Item("VERSIE") & " " & sPreffix
                            Else
                                sSuffix = " - V" & dictAttrib.Item("VERSIE")
                            End If
                        End If
                    End If

                End If
                'filename opbouwen op basis van preffix en layoutnaam en suffix
                sFileName = sPreffix & sLayoutNaam & sSuffix

                If Me.pdfSheetType = SheetType.MultiPdf Or Me.pdfSheetType = SheetType.MultiDwf Then
                    'dsdEntry.Title = layout.LayoutName
                    dsdEntry.Title = sFileName
                    dsdEntry.Nps = layout.TabOrder.ToString()
                ElseIf bUseDWGname = True Then
                    dsdEntry.Title = Path.GetFileNameWithoutExtension(Me.dwgFile) + " - " + sFileName ' layout.LayoutName
                    dsdEntry.Nps = "Setup1"
                Else
                    'dsdEntry.Title = layout.LayoutName
                    dsdEntry.Title = sFileName
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
                        ElseIf str.Contains("PWD") Then
                            newStr = str & vbCrLf & "[PdfOptions]" 'locatie voor invoegen pdf options aanmaken
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
            Dim sSelectedPreset As String
            'kijken of er een temporarily override is voor de plotter
            If sPlottingDeviceOverride.Length = 0 Then
                sSelectedPreset = iniFile.GetString("publishsettings", "defaultplotter", sDefaultPlottingDevice)
            Else
                sSelectedPreset = sPlottingDeviceOverride
                'MsgBox("Override " & sSelectedPreset)
            End If
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
            Else
                MsgBox("Print Preset file Niet gevonden, mogelijk zijn de resultaten anders dan verwacht")
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

