Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Text
Imports Newtonsoft.Json.Linq

Public Class clsRegister
    Shared sComputerName As String = My.Computer.Name
    Shared sCurrVersion As String = Assembly.GetExecutingAssembly().GetName().Version.ToString
    Shared sLicName As String = "RNLAYMAN.LCF"
    Shared sUpdateFile As String = "RNUPD.LCU"

    Public Shared Function PHP(ByVal url As String, ByVal method As String, ByVal data As String)
        Try
            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(data)
            Dim request As System.Net.WebRequest = System.Net.WebRequest.Create(url)
            request.Method = method
            request.ContentType = "application/xml"
            request.ContentLength = byteArray.Length
            request.GetRequestStream().Write(byteArray, 0, byteArray.Length)

            Dim response As WebResponse = request.GetResponse()
            Dim responseFromServer As String = (New StreamReader(response.GetResponseStream())).ReadToEnd()

            response.Close()
            Return (responseFromServer)
        Catch ex As System.Exception
            Dim error1 As String = ErrorToString()
            If error1 = "Invalid URI: The format of the URI could not be determined." Then
                Return "ERROR! Must have HTTP:// before the URL."
            Else
                'MsgBox(error1)
                Return error1
            End If
            'Return ("ERROR")
            Return "PHP error"
        End Try
    End Function

    Public Shared Function checkUpdates(sUserEmail As String, sRequestType As String, sUserID As String)
        Try
            Dim tdes As New clsTripleDES("royNijkamp@My3Dkey")
            Dim strUpdateURL As String = "https://www.roynijkamp.nl/software/register/"

            Dim strXMLbase As String = "<xml><user><requestsoort>" & sRequestType & "</requestsoort><email>" _
                                    & sUserEmail & "</email><userid>" _
                                    & sUserID & "</userid><computername>" _
                                    & sComputerName & "</computername><curr_version>" _
                                    & sCurrVersion & "</curr_version><appname>RNLAYOUTMANAGER</appname></user></xml>"
            Dim strXML As String = tdes.Encrypt(strXMLbase)
            'MsgBox(strXMLbase)
            'InputBox(strXML, "test", strXML)
            Dim resp As String = PHP(strUpdateURL & "index.php", "POST", strXML)

            Return resp
        Catch ex As Exception
            MsgBox("Fout bij het checken op updates!" & vbCrLf & ex.Message)
            Return "error: " & ex.Message
        End Try
    End Function

    Public Shared Function startUpdate(ByVal sURL As String, ByVal sAppName As String, ByVal sAppPath As String)
        Dim sTempDir = My.Computer.FileSystem.SpecialDirectories.Temp

        If File.Exists(sTempDir & "/RN-AutoUpdater/RN-AutoUpdater.exe") Then
            'er bestaat al een kopie, deze dus eerst verwijderen
            File.Delete(sTempDir & "/RN-AutoUpdater/RN-AutoUpdater.exe")
        End If
        Dim sCoreDir As String = clsFunctions.getCoreDir()
        My.Computer.FileSystem.CopyFile(sCoreDir & "/RN-SmartUpdate.exe", sTempDir & "/RN-AutoUpdater/RN-AutoUpdater.exe")

        Dim installProc As New ProcessStartInfo
        installProc.FileName = sTempDir & "/RN-AutoUpdater/RN-AutoUpdater.exe"
        Dim strArguments As String = ""

        strArguments = sURL & " " & sAppName & " " & sAppPath
        installProc.Arguments = strArguments
        Process.Start(installProc)
        Return True
    End Function

    Public Shared Function createUpdateFile(sUserEmail As String, sRequestType As String, sUserID As String)
        Dim tdes As New clsTripleDES("royNijkamp@My3Dkey")
        Dim strXMLbase As String = "<xml><user><requestsoort>" & sRequestType & "</requestsoort><email>" _
                                    & sUserEmail & "</email><userid>" _
                                    & sUserID & "</userid><computername>" _
                                    & sComputerName & "</computername><curr_version>" _
                                    & sCurrVersion & "</curr_version><appname>RNLAYOUTMANAGER</appname></user></xml>"
        Dim strXML As String = tdes.Encrypt(strXMLbase)
        Dim sCoreDir As String = clsFunctions.getCoreDir()
        If IO.File.Exists(sCoreDir & "\" & sUpdateFile) Then

            My.Computer.FileSystem.DeleteFile(sCoreDir & "\" & sUpdateFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        End If
        Dim strmWriter As StreamWriter

        strmWriter = File.CreateText(sCoreDir & "\" & sUpdateFile)
        strmWriter.WriteLine(strXML)
        strmWriter.Flush()
        strmWriter.Close()
        Return True
    End Function

    Public Shared Function createLicense(sUserEmail As String, sRequestType As String, sUserID As String)
        Dim tdes As New clsTripleDES("royNijkamp@My3Dkey")
        Dim strUpdateURL As String = "https://www.roynijkamp.nl/software/register/"
        Dim sCoreDir As String = clsFunctions.getCoreDir()
        '// vewachte paramters in XML
        '// $user->requestsoort
        '// $user->id
        '// $user->email
        '// $user->computername
        '// $user->curr_version
        '// $user->appname

        Dim strXMLbase As String = "<xml><user><requestsoort>" & sRequestType & "</requestsoort><email>" _
                                    & sUserEmail & "</email><userid>" _
                                    & sUserID & "</userid><computername>" _
                                    & sComputerName & "</computername><curr_version>" _
                                    & sCurrVersion & "</curr_version><appname>RNLAYOUTMANAGER</appname></user></xml>"
        Dim strXML As String = tdes.Encrypt(strXMLbase)
        clsFunctions.makeLog(sCoreDir & "\" & sLicName & ".log", "Send XML " & strXMLbase)
        clsFunctions.makeLog(sCoreDir & "\" & sLicName & ".log", "Encrypt XML " & strXML)

        Dim resp As String = PHP(strUpdateURL & "index.php", "POST", strXML)
        clsFunctions.makeLog(sCoreDir & "\" & sLicName & ".log", "Server Response " & resp)
        If resp.Contains("error") Then
            MsgBox("Het was niet mogelijk om een KEY aan te maken" & vbCrLf & resp)
            Return False
        Else
            If IO.File.Exists(sCoreDir & "\" & sLicName) Then
                My.Computer.FileSystem.DeleteFile(sCoreDir & "\" & sLicName, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            End If

            clsFunctions.makeLog(sCoreDir & "\" & sLicName & ".log", "Write License - Filename: " & sCoreDir & "\" & sLicName)
            Dim strmWriter As StreamWriter

            strmWriter = File.CreateText(sCoreDir & "\" & sLicName)
            strmWriter.WriteLine(resp)
            clsFunctions.makeLog(sCoreDir & "\" & sLicName & ".log", "Contents " & resp)
            strmWriter.Flush()
            strmWriter.Close()
            Return True
        End If
    End Function

    Public Shared Function CheckLicense(ByVal sLicFile As String)
        Dim tdes As New clsTripleDES("royNijkamp@My3Dkey")
        Dim sCoreDir As String = clsFunctions.getCoreDir()
        clsFunctions.makeLog(sLicFile & ".log", "Check License " & sLicFile)
        If IO.File.Exists(sCoreDir & "\" & sLicName) Then
            Try
                Dim sLic As String = My.Computer.FileSystem.ReadAllText(sCoreDir & "\" & sLicName)
                clsFunctions.makeLog(sLicFile & ".log", "Lic loaded into String " & sLic)
                Dim sLicenseDecrypt As String = tdes.Decrypt(sLic)
                clsFunctions.makeLog(sLicFile & ".log", "Lic Decrypted ")
                clsFunctions.makeLog(sLicFile & ".log", sLicenseDecrypt)
                Dim myObject As JObject = JObject.Parse(sLicenseDecrypt)
                Dim aUserDet As JArray = myObject("details")
                Dim sComputernameLic As String = aUserDet(0).SelectToken("computername").ToString
                Dim sUserName As String = aUserDet(0).SelectToken("name").ToString

                Dim sUserEmail As String = aUserDet(0).SelectToken("email").ToString

                Dim sRegDate As String = aUserDet(0).SelectToken("regdate").ToString

                clsFunctions.makeLog(sLicFile & ".log", "computername " & sComputernameLic)
                clsFunctions.makeLog(sLicFile & ".log", "UserName " & sUserName)
                clsFunctions.makeLog(sLicFile & ".log", "UserEmail " & sUserEmail)
                clsFunctions.makeLog(sLicFile & ".log", "RegDate " & sRegDate)

                If sComputerName = sComputernameLic Then
                    Return True
                Else
                    My.Computer.FileSystem.DeleteFile(sCoreDir & "\" & sLicName, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    MsgBox("Licentiebestand is niet geldig voor deze computer!" & vbCrLf & "Registreert u opnieuw!")
                    clsFunctions.makeLog(sLicFile & ".log", "Lic niet geldig voor deze pc " & sComputerName & " != " & sComputernameLic)
                    Return False
                End If
            Catch ex As System.Exception
                MsgBox("Fout bij het lezen van de licentie!" & vbCrLf & ex.Message)
                Return False
            End Try
        Else
            MsgBox("Licentiebestand niet gevonden")
            Return False
        End If
    End Function
End Class
