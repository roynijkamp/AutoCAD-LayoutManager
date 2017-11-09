Imports System.Reflection

Public Class clsFunctions
    Public Shared Function getCoreDir() As String
        Dim strAssemblyPath As String = Assembly.GetExecutingAssembly.Location
        Dim strTmpPath() As String = strAssemblyPath.Split("\")
        Dim arrUbound As Integer = UBound(strTmpPath)
        getCoreDir = strAssemblyPath.Replace(strTmpPath(arrUbound), "")
    End Function
End Class
