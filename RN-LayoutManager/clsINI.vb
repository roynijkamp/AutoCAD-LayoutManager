Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.InteropServices
Imports System.Text

Public Class clsINI
    Private strFilename As String

    Public ReadOnly Property FileName() As String
        Get
            Return Me.strFilename
        End Get
    End Property

    Private Declare Ansi Function GetPrivateProfileString Lib "kernel32.dll" Alias "GetPrivateProfileStringA" (<MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpApplicationName As String, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpKeyName As String, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpDefault As String, lpReturnedString As StringBuilder, nSize As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpFileName As String) As Integer

    Private Declare Ansi Function WritePrivateProfileString Lib "kernel32.dll" Alias "WritePrivateProfileStringA" (<MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpApplicationName As String, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpKeyName As String, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpString As String, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpFileName As String) As Integer

    Private Declare Ansi Function GetPrivateProfileInt Lib "kernel32.dll" Alias "GetPrivateProfileIntA" (<MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpApplicationName As String, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpKeyName As String, nDefault As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpFileName As String) As Integer

    Private Declare Ansi Function FlushPrivateProfileString Lib "kernel32.dll" Alias "WritePrivateProfileStringA" (lpApplicationName As Integer, lpKeyName As Integer, lpString As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpFileName As String) As Integer

    Public Sub New(Filename As String)
        Me.strFilename = Filename
    End Sub

    Public Function GetString(Section As String, Key As String, [Default] As String) As String
        Dim stringBuilder As StringBuilder = New StringBuilder(256)
        Dim privateProfileString As Integer = clsINI.GetPrivateProfileString(Section, Key, [Default], stringBuilder, stringBuilder.Capacity, Me.strFilename)
        Dim result As String
        If privateProfileString > 0 Then
            result = Strings.Left(stringBuilder.ToString(), privateProfileString)
        Else
            result = ""
        End If
        Return result
    End Function

    Public Function GetInteger(Section As String, Key As String, [Default] As Integer) As Integer
        Return clsINI.GetPrivateProfileInt(Section, Key, [Default], Me.strFilename)
    End Function

    Public Function GetBoolean(Section As String, Key As String, [Default] As Boolean) As Boolean
        Return clsINI.GetPrivateProfileInt(Section, Key, If((-(If(([Default] > False), 1, 0))), 1, 0), Me.strFilename) = 1
    End Function

    Public Sub WriteString(Section As String, Key As String, Value As String)
        clsINI.WritePrivateProfileString(Section, Key, Value, Me.strFilename)
        Me.Flush()
    End Sub

    Public Sub WriteInteger(Section As String, Key As String, Value As Integer)
        Me.WriteString(Section, Key, Conversions.ToString(Value))
        Me.Flush()
    End Sub

    Public Sub WriteBoolean(Section As String, Key As String, Value As Boolean)
        Me.WriteString(Section, Key, Conversions.ToString(If((-(If((Value = True), 1, 0))), 1, 0)))
        Me.Flush()
    End Sub

    Private Sub Flush()
        clsINI.FlushPrivateProfileString(0, 0, 0, Me.strFilename)
    End Sub
End Class
