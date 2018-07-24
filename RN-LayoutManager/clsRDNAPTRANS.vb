Imports System
Imports System.IO
Imports Microsoft.VisualBasic

Public Class clsRDNAPTRANS
    Dim PI As Double = 2.0 * Math.Asin(1.0)
    Dim PHI_AMERSFOORT_BESSEL As Double = 52.0 + 9.0 / 60.0 + 22.178 / 3600.0
    Dim LAMBDA_AMERSFOORT_BESSEL As Double = 5.0 + 23.0 / 60.0 + 15.5 / 3600.0
    Dim H_AMERSFOORT_BESSEL As Double = 0.0
    Dim A_BESSEL As Double = 6377397.155
    Dim INV_F_BESSEL As Double = 299.1528128
    Dim A_ETRS As Double = 6378137
    Dim INV_F_ETRS As Double = 298.257222101
    Dim TX_BESSEL_ETRS As Double = 593.0248
    Dim TY_BESSEL_ETRS As Double = 25.9984
    Dim TZ_BESSEL_ETRS As Double = 478.7459
    Dim ALPHA_BESSEL_ETRS As Double = 0.0000019342
    Dim BETA_BESSEL_ETRS As Double = -0.0000016677
    Dim GAMMA_BESSEL_ETRS As Double = 0.0000091019
    Dim DELTA_BESSEL_ETRS As Double = 0.0000040725
    Dim TX_ETRS_BESSEL As Double = -593.0248
    Dim TY_ETRS_BESSEL As Double = -25.9984
    Dim TZ_ETRS_BESSEL As Double = -478.7459
    Dim ALPHA_ETRS_BESSEL As Double = -0.0000019342
    Dim BETA_ETRS_BESSEL As Double = 0.0000016677
    Dim GAMMA_ETRS_BESSEL As Double = -0.0000091019
    Dim DELTA_ETRS_BESSEL As Double = -0.0000040725
    Dim SCALE_RD As Double = 0.9999079
    Dim X_AMERSFOORT_RD As Double = 155000
    Dim Y_AMERSFOORT_RD As Double = 463000
    Dim GRID_FILE_DX As String = "x2c.grd"
    Dim GRID_FILE_DY As String = "y2c.grd"
    Dim GRID_FILE_GEOID As String = "nlgeo04.grd"
    Dim PRECISION As Double = 0.0001
    Dim DEG_PRECISION As Double = PRECISION / 40000000.0 * 360
    Dim MEAN_GEOID_HEIGHT_BESSEL As Double = 0.0

    Private Function deg_sin(ByVal alpha As Double) As Double
        Return Math.Sin(alpha / 180.0 * PI)
    End Function

    Private Function deg_cos(ByVal alpha As Double) As Double
        Return Math.Cos(alpha / 180.0 * PI)
    End Function

    Private Function deg_tan(ByVal alpha As Double) As Double
        Return Math.Tan(alpha / 180.0 * PI)
    End Function

    Private Function deg_asin(ByVal a As Double) As Double
        Return (Math.Asin(a) * 180.0 / PI)
    End Function

    Private Function deg_atan(ByVal a As Double) As Double
        Return (Math.Atan(a) * 180.0 / PI)
    End Function

    Private Function atanh(ByVal a As Double) As Double
        Return (0.5 * Math.Log((1.0 + a) / (1.0 - a)))
    End Function

    Private Sub deg_min_sec2decimal(ByVal deg As Double, ByVal min As Double, ByVal sec As Double, ByRef dec_deg As Double)
        dec_deg = (deg + min / 60.0 + sec / 3600.0)
    End Sub

    Private Sub decimal2deg_min_sec(ByVal dec_deg As Double, ByRef deg As Integer, ByRef min As Integer, ByRef sec As Double)
        deg = CInt(dec_deg)
        min = CInt((dec_deg - deg) * 60.0)
        sec = ((dec_deg - deg) * 60.0 - min) * 60.0
    End Sub

    Private Sub geographic2cartesian(ByVal phi As Double, ByVal lambda As Double, ByVal h As Double, ByVal a As Double, ByVal inv_f As Double, ByRef x As Double, ByRef y As Double, ByRef z As Double)
        Dim f As Double = 1.0 / inv_f
        Dim ee As Double = f * (2.0 - f)
        Dim n As Double = a / Math.Sqrt(1.0 - ee * Math.Pow(deg_sin(phi), 2))
        x = (n + h) * deg_cos(phi) * deg_cos(lambda)
        y = (n + h) * deg_cos(phi) * deg_sin(lambda)
        z = (n * (1.0 - ee) + h) * deg_sin(phi)
    End Sub

    Private Sub cartesian2geographic(ByVal x As Double, ByVal y As Double, ByVal z As Double, ByVal a As Double, ByVal inv_f As Double, ByRef phi As Double, ByRef lambda As Double, ByRef h As Double)
        Dim f As Double = 1.0 / inv_f
        Dim ee As Double = f * (2.0 - f)
        Dim rho As Double = Math.Sqrt(x * x + y * y)
        Dim n As Double
        phi = 0
        Dim previous As Double
        Dim diff As Double = 90

        While diff > DEG_PRECISION
            previous = phi
            n = a / Math.Sqrt(1.0 - ee * Math.Pow(deg_sin(phi), 2))
            phi = deg_atan(z / rho + n * ee * deg_sin(phi) / rho)
            diff = fabs(phi - previous)
        End While

        lambda = deg_atan(y / x)
        h = rho * deg_cos(phi) + z * deg_sin(phi) - n * (1.0 - ee * Math.Pow(deg_sin(phi), 2))
    End Sub

    Private Function fabs(ByVal n As Double)
        If (n >= 0) Then
            Return n 'if positive, return without ant change
        End If

        Return (0 - n)
        'if negative, return a positive version
    End Function

    Private Sub sim_trans(ByVal x_in As Double, ByVal y_in As Double, ByVal z_in As Double, ByVal tx As Double, ByVal ty As Double, ByVal tz As Double, ByVal alpha As Double, ByVal beta As Double, ByVal gamma As Double, ByVal delta As Double, ByVal xa As Double, ByVal ya As Double, ByVal za As Double, ByRef x_out As Double, ByRef y_out As Double, ByRef z_out As Double)
        Dim a As Double = Math.Cos(gamma) * Math.Cos(beta)
        Dim b As Double = Math.Cos(gamma) * Math.Sin(beta) * Math.Sin(alpha) + Math.Sin(gamma) * Math.Cos(alpha)
        Dim c As Double = -Math.Cos(gamma) * Math.Sin(beta) * Math.Cos(alpha) + Math.Sin(gamma) * Math.Sin(alpha)
        Dim d As Double = -Math.Sin(gamma) * Math.Cos(beta)
        Dim e As Double = -Math.Sin(gamma) * Math.Sin(beta) * Math.Sin(alpha) + Math.Cos(gamma) * Math.Cos(alpha)
        Dim f As Double = Math.Sin(gamma) * Math.Sin(beta) * Math.Cos(alpha) + Math.Cos(gamma) * Math.Sin(alpha)
        Dim g As Double = Math.Sin(beta)
        Dim h As Double = -Math.Cos(beta) * Math.Sin(alpha)
        Dim i As Double = Math.Cos(beta) * Math.Cos(alpha)
        Dim x As Double = x_in - xa
        Dim y As Double = y_in - ya
        Dim z As Double = z_in - za
        x_out = (1.0 + delta) * (a * x + b * y + c * z) + tx + xa
        y_out = (1.0 + delta) * (d * x + e * y + f * z) + ty + ya
        z_out = (1.0 + delta) * (g * x + h * y + i * z) + tz + za
    End Sub

    Private Sub rd_projection(ByVal phi As Double, ByVal lambda As Double, ByRef x_rd As Double, ByRef y_rd As Double)
        Dim f As Double = 1 / INV_F_BESSEL
        Dim ee As Double = f * (2 - f)
        Dim e As Double = Math.Sqrt(ee)
        Dim eea As Double = ee / (1.0 - ee)
        Dim phi_amersfoort_sphere As Double = deg_atan(deg_tan(PHI_AMERSFOORT_BESSEL) / Math.Sqrt(1 + eea * Math.Pow(deg_cos(PHI_AMERSFOORT_BESSEL), 2)))
        Dim lambda_amersfoort_sphere As Double = LAMBDA_AMERSFOORT_BESSEL
        Dim r1 As Double = A_BESSEL * (1 - ee) / Math.Pow(Math.Sqrt(1 - ee * Math.Pow(deg_sin(PHI_AMERSFOORT_BESSEL), 2)), 3)
        Dim r2 As Double = A_BESSEL / Math.Sqrt(1.0 - ee * Math.Pow(deg_sin(PHI_AMERSFOORT_BESSEL), 2))
        Dim r_sphere As Double = Math.Sqrt(r1 * r2)
        Dim n As Double = Math.Sqrt(1 + eea * Math.Pow(deg_cos(PHI_AMERSFOORT_BESSEL), 4))
        Dim q_amersfoort As Double = atanh(deg_sin(PHI_AMERSFOORT_BESSEL)) - e * atanh(e * deg_sin(PHI_AMERSFOORT_BESSEL))
        Dim w_amersfoort As Double = Math.Log(deg_tan(45 + 0.5 * phi_amersfoort_sphere))
        Dim m As Double = w_amersfoort - n * q_amersfoort
        Dim q As Double = atanh(deg_sin(phi)) - e * atanh(e * deg_sin(phi))
        Dim w As Double = n * q + m
        Dim phi_sphere As Double = 2 * deg_atan(Math.Exp(w)) - 90
        Dim delta_lambda_sphere As Double = n * (lambda - lambda_amersfoort_sphere)
        Dim sin_half_psi_squared As Double = Math.Pow(deg_sin(0.5 * (phi_sphere - phi_amersfoort_sphere)), 2) + Math.Pow(deg_sin(0.5 * delta_lambda_sphere), 2) * deg_cos(phi_sphere) * deg_cos(phi_amersfoort_sphere)
        Dim sin_half_psi As Double = Math.Sqrt(sin_half_psi_squared)
        Dim cos_half_psi As Double = Math.Sqrt(1 - sin_half_psi_squared)
        Dim tan_half_psi As Double = sin_half_psi / cos_half_psi
        Dim sin_psi As Double = 2 * sin_half_psi * cos_half_psi
        Dim cos_psi As Double = 1 - 2 * sin_half_psi_squared
        Dim sin_alpha As Double = deg_sin(delta_lambda_sphere) * (deg_cos(phi_sphere) / sin_psi)
        Dim cos_alpha As Double = (deg_sin(phi_sphere) - deg_sin(phi_amersfoort_sphere) * cos_psi) / (deg_cos(phi_amersfoort_sphere) * sin_psi)
        Dim r As Double = 2 * SCALE_RD * r_sphere * tan_half_psi
        x_rd = r * sin_alpha + X_AMERSFOORT_RD
        y_rd = r * cos_alpha + Y_AMERSFOORT_RD
    End Sub

    Private Sub inv_rd_projection(ByVal x_rd As Double, ByVal y_rd As Double, ByRef phi As Double, ByRef lambda As Double)
        Dim f As Double = 1 / INV_F_BESSEL
        Dim ee As Double = f * (2 - f)
        Dim e As Double = Math.Sqrt(ee)
        Dim eea As Double = ee / (1.0 - ee)
        Dim phi_amersfoort_sphere As Double = deg_atan(deg_tan(PHI_AMERSFOORT_BESSEL) / Math.Sqrt(1 + eea * Math.Pow(deg_cos(PHI_AMERSFOORT_BESSEL), 2)))
        Dim r1 As Double = A_BESSEL * (1 - ee) / Math.Pow(Math.Sqrt(1 - ee * Math.Pow(deg_sin(PHI_AMERSFOORT_BESSEL), 2)), 3)
        Dim r2 As Double = A_BESSEL / Math.Sqrt(1.0 - ee * Math.Pow(deg_sin(PHI_AMERSFOORT_BESSEL), 2))
        Dim r_sphere As Double = Math.Sqrt(r1 * r2)
        Dim n As Double = Math.Sqrt(1 + eea * Math.Pow(deg_cos(PHI_AMERSFOORT_BESSEL), 4))
        Dim q_amersfoort As Double = atanh(deg_sin(PHI_AMERSFOORT_BESSEL)) - e * atanh(e * deg_sin(PHI_AMERSFOORT_BESSEL))
        Dim w_amersfoort As Double = Math.Log(deg_tan(45 + 0.5 * phi_amersfoort_sphere))
        Dim m As Double = w_amersfoort - n * q_amersfoort
        Dim r As Double = Math.Sqrt(Math.Pow(x_rd - X_AMERSFOORT_RD, 2) + Math.Pow(y_rd - Y_AMERSFOORT_RD, 2))
        Dim sin_alpha As Double = (x_rd - X_AMERSFOORT_RD) / r
        If r < PRECISION Then sin_alpha = 0
        Dim cos_alpha As Double = (y_rd - Y_AMERSFOORT_RD) / r
        If r < PRECISION Then cos_alpha = 1
        Dim psi As Double = 2 * deg_atan(r / (2 * SCALE_RD * r_sphere))
        Dim phi_sphere As Double = deg_asin(cos_alpha * deg_cos(phi_amersfoort_sphere) * deg_sin(psi) + deg_sin(phi_amersfoort_sphere) * deg_cos(psi))
        Dim delta_lambda_sphere As Double = deg_asin((sin_alpha * deg_sin(psi)) / deg_cos(phi_sphere))
        lambda = delta_lambda_sphere / n + LAMBDA_AMERSFOORT_BESSEL
        Dim w As Double = atanh(deg_sin(phi_sphere))
        Dim q As Double = (w - m) / n
        phi = 0
        Dim previous As Double
        Dim diff As Double = 90

        While diff > DEG_PRECISION
            previous = phi
            phi = 2 * deg_atan(Math.Exp(q + 0.5 * e * Math.Log((1 + e * deg_sin(phi)) / (1 - e * deg_sin(phi))))) - 90
            diff = fabs(phi - previous)
        End While
    End Sub

    Private Function read_grd_file_header(ByVal filename As String, ByRef size_x As Short, ByRef size_y As Short, ByRef min_x As Double, ByRef max_x As Double, ByRef min_y As Double, ByRef max_y As Double, ByRef min_value As Double, ByRef max_value As Double) As Integer
        '    
        '    **--------------------------------------------------------------
        '    **    Grd files are binary grid files in the format of the program Surfer(R)
        '    **--------------------------------------------------------------
        '    

        'Dim file As New fstream(filename, ios.in Or ios.binary)
        Dim file As New FileStream(filename, FileMode.Open Or FileMode.Append)

        '    
        '    **--------------------------------------------------------------
        '    **    Read file id
        '    **--------------------------------------------------------------
        '    
        Dim id As New String(New Char(4) {})
        Dim cArray As Byte()
        For i As Integer = 0 To 3
            file.Seek(i, SeekOrigin.Begin)
            cArray = System.Text.Encoding.ASCII.GetBytes(CChar(CStr(id.Chars(i))))
            file.Read(cArray, 1, 1)
        Next i
        id = StringFunctions.ChangeCharacter(id, 4, ControlChars.NullChar)
        Dim id_string As String = id

        '    
        '    **--------------------------------------------------------------
        '    **    Checks
        '    **--------------------------------------------------------------
        '    
        If file Is Nothing Then
            'cerr<< filename << " does not exist" << ControlChars.Lf
            Return -1
        End If

        If id_string <> "DSBB" Then
            'cerr<< filename << " is not a valid grd file" << ControlChars.Lf
            Return -1
        End If

        If Len(New Short()) <> 2 Then
            'cerr<< "Error: The number of bytes of a short integer in your compiler is " << Len(New Short()) << " and not 2 as in the file " << filename << ControlChars.Lf
            Return -1
        End If

        If Len(New Double()) <> 8 Then
            'cerr<< "Error: The number of bytes of a double in your compiler is " << Len(New Double()) << " and not 8 as in the file " << filename << ControlChars.Lf
            Return -1
        End If

        '    
        '    **--------------------------------------------------------------
        '    **    Read output parameters
        '    **--------------------------------------------------------------
        '    
        file.Seek(4, SeekOrigin.Begin)
        cArray = System.Text.Encoding.ASCII.GetBytes(ChrW(size_x))
        file.Read(cArray, 2, 1)

        file.Seek(6, SeekOrigin.Begin)
        cArray = System.Text.Encoding.ASCII.GetBytes(ChrW(size_y))
        file.Read(cArray, 2, 1)



        file.Seek(8, SeekOrigin.Begin)
        cArray = System.Text.Encoding.ASCII.GetBytes(ChrW(min_x))
        file.Read(cArray, 8, 1)

        file.Seek(16, SeekOrigin.Begin)
        cArray = System.Text.Encoding.ASCII.GetBytes(ChrW(max_x))
        file.Read(cArray, 8, 1)

        file.Seek(24, SeekOrigin.Begin)
        cArray = System.Text.Encoding.ASCII.GetBytes(ChrW(min_y))
        file.Read(cArray, 8, 1)

        file.Seek(32, SeekOrigin.Begin)
        cArray = System.Text.Encoding.ASCII.GetBytes(ChrW(max_y))
        file.Read(cArray, 8, 1)

        file.Seek(40, SeekOrigin.Begin)
        cArray = System.Text.Encoding.ASCII.GetBytes(ChrW(min_value))
        file.Read(cArray, 8, 1)

        file.Seek(48, SeekOrigin.Begin)
        cArray = System.Text.Encoding.ASCII.GetBytes(ChrW(max_value))
        file.Read(cArray, 8, 1)

        Return 0
    End Function



End Class
'----------------------------------------------------------------------------------------
'	Copyright © 2006 - 2018 Tangible Software Solutions Inc.
'	This module can be used by anyone provided that the copyright notice remains intact.
'
'	This module provides the ability to replicate various classic C string functions
'	which don't have exact equivalents in the .NET Framework.
'----------------------------------------------------------------------------------------
Friend Module StringFunctions
    '------------------------------------------------------------------------------------
    '	This method allows replacing a single character in a string, to help convert
    '	C++ code where a single character in a character array is replaced.
    '------------------------------------------------------------------------------------
    Friend Function ChangeCharacter(ByVal sourceString As String, ByVal charIndex As Integer, ByVal changeChar As Char) As String
        Return If(charIndex > 0, sourceString.Substring(0, charIndex), "") _
            & changeChar.ToString() & If(charIndex < sourceString.Length - 1, sourceString.Substring(charIndex + 1), "")
    End Function

    '------------------------------------------------------------------------------------
    '	This method replicates the classic C string function 'isxdigit' (and 'iswxdigit').
    '------------------------------------------------------------------------------------
    Friend Function IsXDigit(ByVal character As Char) As Boolean
        If Char.IsDigit(character) Then
            Return True
        ElseIf "ABCDEFabcdef".IndexOf(character) > -1 Then
            Return True
        Else
            Return False
        End If
    End Function

    '------------------------------------------------------------------------------------
    '	This method replicates the classic C string function 'strchr' (and 'wcschr').
    '------------------------------------------------------------------------------------
    Friend Function StrChr(ByVal stringToSearch As String, ByVal charToFind As Char) As String
        Dim index As Integer = stringToSearch.IndexOf(charToFind)
        If index > -1 Then
            Return stringToSearch.Substring(index)
        Else
            Return Nothing
        End If
    End Function

    '------------------------------------------------------------------------------------
    '	This method replicates the classic C string function 'strrchr' (and 'wcsrchr').
    '------------------------------------------------------------------------------------
    Friend Function StrRChr(ByVal stringToSearch As String, ByVal charToFind As Char) As String
        Dim index As Integer = stringToSearch.LastIndexOf(charToFind)
        If index > -1 Then
            Return stringToSearch.Substring(index)
        Else
            Return Nothing
        End If
    End Function

    '------------------------------------------------------------------------------------
    '	This method replicates the classic C string function 'strstr' (and 'wcsstr').
    '------------------------------------------------------------------------------------
    Friend Function StrStr(ByVal stringToSearch As String, ByVal stringToFind As String) As String
        Dim index As Integer = stringToSearch.IndexOf(stringToFind)
        If index > -1 Then
            Return stringToSearch.Substring(index)
        Else
            Return Nothing
        End If
    End Function

    '------------------------------------------------------------------------------------
    '	This method replicates the classic C string function 'strtok' (and 'wcstok').
    '	Note that the .NET string 'Split' method cannot be used to replicate 'strtok' since
    '	it doesn't allow changing the delimiters between each token retrieval.
    '------------------------------------------------------------------------------------
    Private activeString As String
    Private activePosition As Integer
    Friend Function StrTok(ByVal stringToTokenize As String, ByVal delimiters As String) As String
        If Not stringToTokenize Is Nothing Then
            activeString = stringToTokenize
            activePosition = -1
        End If

        'the stringToTokenize was never set:
        If activeString Is Nothing Then
            Return Nothing
        End If

        'all tokens have already been extracted:
        If activePosition = activeString.Length Then
            Return Nothing
        End If

        'bypass delimiters:
        activePosition += 1
        Do While activePosition < activeString.Length AndAlso delimiters.IndexOf(activeString(activePosition)) > -1
            activePosition += 1
        Loop

        'only delimiters were left, so return null:
        If activePosition = activeString.Length Then
            Return Nothing
        End If

        'get starting position of string to return:
        Dim startingPosition As Integer = activePosition

        'read until next delimiter:
        Do
            activePosition += 1
        Loop While activePosition < activeString.Length AndAlso delimiters.IndexOf(activeString.Chars(activePosition)) = -1

        Return activeString.Substring(startingPosition, activePosition - startingPosition)
    End Function
End Module
