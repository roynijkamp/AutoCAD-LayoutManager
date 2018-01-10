Imports System.Windows.Forms
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput

Public Class frmFilter
    Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
    Dim acCurDb As Database = acDoc.Database
    Dim acEd As Editor = acDoc.Editor

    Private Sub cmdSaveFilter_Click(sender As Object, e As EventArgs) Handles cmdSaveFilter.Click
        Dim dict As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))
        Dim val As List(Of String) = New List(Of String)

        Dim dictLoad As Dictionary(Of String, List(Of String)) = clsFilterData.getDBDictionaryToDictionary(acDoc, acCurDb, acEd, "FILTERSETTINGS")
        If dictLoad Is Nothing Then
            Exit Sub
        End If



        'dict.Add("foo", New List(Of String) From {"a", "b", "c"})
        'dict.Add("bar", New List(Of String) From {"x", "y", "z"})
        'dict.Add("baz", New List(Of String) From {"this", "is", "a", "test"})
        'If cmbFilter.Text.Length > 0 Then
        '    dict.Add(cmbFilter.Text, New List(Of String) From {"custom", "filter", "added", "test"})
        'End If
        'Me.filterToDBDictionary(dict, "StringArraysDictionary")
        'save dict
        clsFilterData.filterToDBDictionary(acDoc, acCurDb, acEd, dict, "FILTERSETTINGS")
        loadFilters()
    End Sub

    Sub test()
        'Dim dict As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))
        'dict.Add("foo", New List(Of String) From {"a", "b", "c"})
        'dict.Add("bar", New List(Of String) From {"x", "y", "z"})
        'dict.Add("baz", New List(Of String) From {"this", "is", "a", "test"})
        'If cmbFilter.Text.Length > 0 Then
        '    dict.Add(cmbFilter.Text, New List(Of String) From {"custom", "filter", "added", "test"})
        'End If
        ''Me.filterToDBDictionary(dict, "StringArraysDictionary")
        ''save dict
        'clsFilterData.filterToDBDictionary(acDoc, acCurDb, acEd, dict, "FILTERSETTINGS")

        'load dict
        Dim dictLoad As Dictionary(Of String, List(Of String)) = clsFilterData.getDBDictionaryToDictionary(acDoc, acCurDb, acEd, "FILTERSETTINGS")
        If dictLoad Is Nothing Then
            TextBox1.Text = "Settings niet gevonden"
        End If
        TextBox1.Text = ""
        'Dim dictLoad As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))
        For Each pair As KeyValuePair(Of String, List(Of String)) In dictLoad
            Dim datas As String = String.Empty
            For Each s As String In pair.Value
                datas += " " & s
            Next

            'ed.WriteMessage(vbLf & "{0} ={1}", pair.Key, datas)
            TextBox1.AppendText(pair.Key.ToString & " -- " & datas & vbCrLf & "###" & vbCrLf)
        Next
    End Sub

    Sub loadFilters()
        Dim dict As Dictionary(Of String, List(Of String)) = New Dictionary(Of String, List(Of String))
        cmbFilter.Items.Clear()
        'load dict
        Dim dictLoad As Dictionary(Of String, List(Of String)) = clsFilterData.getDBDictionaryToDictionary(acDoc, acCurDb, acEd, "FILTERSETTINGS")
        If dictLoad Is Nothing Then
            Exit Sub
        End If
        For Each pair As KeyValuePair(Of String, List(Of String)) In dictLoad
            cmbFilter.Items.Add(pair.Key)
        Next

    End Sub

    Private Sub frmFilter_Load(sender As Object, e As EventArgs) Handles Me.Load
        loadFilters()
    End Sub

    Private Sub cmdDelFilter_Click(sender As Object, e As EventArgs) Handles cmdDelFilter.Click

    End Sub
End Class