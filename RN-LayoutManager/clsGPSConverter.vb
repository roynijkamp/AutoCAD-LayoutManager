Public Class clsGPSConverter
    Private Shared cArray As Char() = New Char() {"C"c, "D"c, "E"c, "F"c, "G"c, "H"c,
    "J"c, "K"c, "L"c, "M"c, "N"c, "P"c,
    "Q"c, "R"c, "S"c, "T"c, "U"c, "V"c,
    "W"c, "X"c}


    Private Shared Function FutmYzone(lat As Double) As Short
        Dim [string] As String
        If lat < 84 And lat >= 72 Then
            [string] = CStr(clsGPSConverter.cArray(19))
        Else
            [string] = CStr(clsGPSConverter.cArray(CInt(Math.Round(Conversion.Fix(lat + 80 / 8)))))
        End If
        If lat >= 84 Or lat < -80 Then
            [string] = "*"
        End If
        Return CShort(Strings.Asc([string]))

    End Function

    Private Shared Function CalculateESquared(a As Double, b As Double) As Object
        Return (a * a - b * b) / (a * a)
    End Function

    Private Shared Function CalculateE2Squared(a As Double, b As Double) As Object
        Return (a * a - b * b) / (b * b)
    End Function


    Private Shared Function denom(es As Double, sphi As Double) As Object
        Dim num As Double = Math.Sin(sphi)
        Return Math.Pow(1 - es * (num * num), 0.5)
    End Function

    Private Shared Function sphsr(a As Double, es As Double, sphi As Double) As Object
        Dim num As Double = CDbl(clsGPSConverter.denom(es, sphi))
        Return a * (1 - es) / (num * num * num)
    End Function

    Private Shared Function sphsn(a As Double, es As Double, sphi As Double) As Object
        Dim num As Double = Math.Sin(sphi)
        Return a / Math.Pow(1 - es * (num * num), 0.5)
    End Function

    Private Shared Function sphtmd(ap As Double, bp As Double, cp As Double, dp As Double, ep As Double, sphi As Double) As Object
        Return ap * sphi - bp * Math.Sin(2 * sphi) + cp * Math.Sin(4 * sphi) - dp * Math.Sin(6 * sphi) + ep * Math.Sin(8 * sphi)
    End Function


    Public Shared Function LatLonToUTM(ByVal a As Double, ByVal f As Double, ByVal Lat As Double, ByVal Lon As Double) As Double()
        Dim num As Integer
        Dim result As Double()
        Dim num23 As Integer
        Try
            num = 2
            Dim num2 As Double
            If Lon <= 0.0 Then
                num2 = 30.0 + Conversion.Fix(Lon / 6.0)
            Else
                num2 = 31.0 + Conversion.Fix(Lon / 6.0)
            End If
            Dim num3 As Double = CDec(clsGPSConverter.FutmYzone(Lat))
            Dim num4 As Double = Lat * 0.017453292500000002
            Dim num5 As Double = Lon * 0.017453292500000002
            Dim num6 As Double = 1.0 / f
            Dim num7 As Double = a * (num6 - 1.0) / num6
            Dim es As Double = CDbl(clsGPSConverter.CalculateESquared(a, num7))
            Dim arg_29F_0 As Double = CDbl(clsGPSConverter.CalculateE2Squared(a, num7))
            Dim num8 As Double = (a - num7) / (a + num7)
            Dim ap As Double = a * (1.0 - num8 + 5.0 * (num8 * num8 - num8 * num8 * num8) / 4.0 + 81.0 * (num8 * num8 * num8 * num8 - num8 * num8 * num8 * num8 * num8) / 64.0)
            Dim bp As Double = 3.0 * a * (num8 - num8 * num8 + 7.0 * (num8 * num8 * num8 - num8 * num8 * num8 * num8) / 8.0 + 55.0 * (num8 * num8 * num8 * num8 * num8) / 64.0) / 2.0
            Dim cp As Double = 15.0 * a * (num8 * num8 - num8 * num8 * num8 + 3.0 * (num8 * num8 * num8 * num8 - num8 * num8 * num8 * num8 * num8) / 4.0) / 16.0
            Dim dp As Double = 35.0 * a * (num8 * num8 * num8 - num8 * num8 * num8 * num8 + 11.0 * (num8 * num8 * num8 * num8 * num8) / 16.0) / 48.0
            Dim ep As Double = 315.0 * a * (num8 * num8 * num8 * num8 - num8 * num8 * num8 * num8 * num8) / 512.0
            Dim num9 As Double = (num2 * 6.0 - 183.0) * 0.017453292500000002
            Dim num10 As Double = num5 - num9
            Dim num11 As Double = Math.Sin(num4)
            Dim num12 As Double = Math.Cos(num4)
            Dim num13 As Double = num11 / num12
            Dim num14 As Double = arg_29F_0 * (num12 * num12)
            Dim arg_2D3_0 As Double = CDbl(clsGPSConverter.sphsn(a, es, num4))
            Dim num15 As Double = CDbl(clsGPSConverter.sphtmd(ap, bp, cp, dp, ep, num4)) * 0.9996
            Dim num16 As Double = arg_2D3_0 * num11 * num12 * 0.9996 / 2.0
            Dim num17 As Double = arg_2D3_0 * num11 * (num12 * num12 * num12) * 0.9996 * (5.0 - num13 * num13 + 9.0 * num14 + 4.0 * (num14 * num14)) / 24.0
            Dim num18 As Double
            If num4 < 0.0 Then
                num18 = 10000000.0
            Else
                num18 = 0.0
            End If
            Dim num19 As Double = num18 + num15 + num10 * num10 * num16 + num10 * num10 * num10 * num10 * num17 + num10 * num10 * num10 * num10 * num10 * num10 + 0.5
            Dim num20 As Double = arg_2D3_0 * num12 * 0.9996
            Dim num21 As Double = arg_2D3_0 * (num12 * num12 * num12) * (1.0 - num13 * num13 + num14) / 6.0
            Dim num22 As Double = 500000.0 + num10 * num20 + num10 * num10 * num10 * num21 + 0.5
            If num19 >= 9999999.0 Then
                num19 = 9999999.0
            End If
            result = New Double() {num2, num22, num3, num19}
        Catch ex As Exception
            MsgBox(ex.Message)
            result = New Double() {0, 0, 0, 0}
        End Try


        Return result
    End Function

    Public Shared Function UTMToLatLon(a As Double, f As Double, UtmXZone As Integer, Easting As Double, NorthHemisphere As Boolean, Northing As Double) As Double()
        Dim num As Double = 1.0 / f
        Dim num2 As Double = a * (num - 1.0) / num
        Dim es As Double = CDbl(clsGPSConverter.CalculateESquared(a, num2))
        Dim num3 As Double = CDbl(clsGPSConverter.CalculateE2Squared(a, num2))
        Dim num4 As Double = (a - num2) / (a + num2)
        Dim ap As Double = a * (1.0 - num4 + 5.0 * (num4 * num4 - num4 * num4 * num4) / 4.0 + 81.0 * (num4 * num4 * num4 * num4 - num4 * num4 * num4 * num4 * num4) / 64.0)
        Dim bp As Double = 3.0 * a * (num4 - num4 * num4 + 7.0 * (num4 * num4 * num4 - num4 * num4 * num4 * num4) / 8.0 + 55.0 * (num4 * num4 * num4 * num4 * num4) / 64.0) / 2.0
        Dim cp As Double = 15.0 * a * (num4 * num4 - num4 * num4 * num4 + 3.0 * (num4 * num4 * num4 * num4 - num4 * num4 * num4 * num4 * num4) / 4.0) / 16.0
        Dim dp As Double = 35.0 * a * (num4 * num4 * num4 - num4 * num4 * num4 * num4 + 11.0 * (num4 * num4 * num4 * num4 * num4) / 16.0) / 48.0
        Dim ep As Double = 315.0 * a * (num4 * num4 * num4 * num4 - num4 * num4 * num4 * num4 * num4) / 512.0
        Dim num5 As Double
        If Not NorthHemisphere Then
            num5 = 10000000.0
        Else
            num5 = 0.0
        End If
        Dim num6 As Double = (Northing - num5) / 0.9996
        Dim num7 As Double = CDbl(clsGPSConverter.sphsr(a, es, 0.0))
        Dim num8 As Double = num6 / num7
        Dim num9 As Integer = 0
        Dim num10 As Double
        Do
            num10 = CDbl(clsGPSConverter.sphtmd(ap, bp, cp, dp, ep, num8))
            num7 = CDbl(clsGPSConverter.sphsr(a, es, num8))
            num8 += (num6 - num10) / num7
            ' The following expression was wrapped in a checked-statement
            num9 += 1
        Loop While num9 <= 4
        num7 = CDbl(clsGPSConverter.sphsr(a, es, num8))
        Dim num11 As Double = CDbl(clsGPSConverter.sphsn(a, es, num8))
        Dim arg_2A3_0 As Double = Math.Sin(num8)
        Dim num12 As Double = Math.Cos(num8)
        Dim num13 As Double = arg_2A3_0 / num12
        Dim num14 As Double = num3 * (num12 * num12)
        Dim num15 As Double = Easting - 500000.0
        num10 = num13 / (2.0 * num7 * num11 * 0.99920016000000011)
        Dim num16 As Double = num13 * (5.0 + 3.0 * (num13 * num13) + num14 - 4.0 * (num14 * num14) - 9.0 * (num13 * num13) * num14) / (24.0 * num7 * (num11 * num11 * num11) * 0.99840095974402587)
        Dim num17 As Double = num8 - num15 * num15 * num10 + num15 * num15 * num15 * num15 * num16
        Dim num18 As Double = 1.0 / (num11 * num12 * 0.9996)
        Dim num19 As Double = (1.0 + 2.0 * (num13 * num13) + num14) / (6.0 * (num11 * num11 * num11) * num12 * 0.99880047993600019)
        Dim num20 As Double = num15 * num18 - num15 * num15 * num15 * num19
        ' The following expression was wrapped in a checked-expression
        Dim num21 As Double = (CDec((UtmXZone * 6)) - 183.0) * 0.017453292500000002 + num20
        num21 *= 57.295779578552292
        num17 *= 57.295779578552292
        Return New Double() {num17, num21}
    End Function



    Public Shared Function WGS84LatLonToUTM(Lat As Double, Lon As Double) As Double()
        Return clsGPSConverter.LatLonToUTM(6378137, 0.003352810665, Lat, Lon)
    End Function

    Public Shared Function WGS84UTMTOLatLon(UtmXZone As Short, Easting As Double, NorthHemisphere As Boolean, Northing As Double) As Double()
        Return clsGPSConverter.UTMToLatLon(6378137, 0.003352810665, CInt(UtmXZone), Easting, NorthHemisphere, Northing)
    End Function


    Public Shared Function ChangeDatum(Lat As Double, Lon As Double, From_a As Double, From_f As Double, To_a As Double, To_f As Double, dx As Double, dy As Double, dz As Double) As Double()
        Lat = Lat / 180.0 * 3.1415926535897931
        Lon = Lon / 180.0 * 3.1415926535897931
        Dim num As Double = Math.Sin(Lat)
        Dim num2 As Double = Math.Cos(Lat)
        Dim num3 As Double = Math.Sin(Lon)
        Dim num4 As Double = Math.Cos(Lon)
        Dim num5 As Double = num * num
        Dim num6 As Double = 1.0 / (1.0 - From_f)
        Dim num7 As Double = 2.0 * From_f - Math.Pow(From_f, 2.0)
        Dim num8 As Double = From_a / Math.Sqrt(1.0 - num7 * num5)
        Dim num9 As Double = From_a * (1.0 - num7) / Math.Pow(1.0 - num7 * num5, 1.5)
        Dim array As Double() = New Double(2) {}
        Dim num10 As Double = To_a - From_a
        Dim num11 As Double = To_f - From_f
        Dim num12 As Double = (-dx * num * num4 - dy * num * num3 + dz * num2 + num10 * (num8 * num7 * num * num2 / From_a) + num11 * (num9 * num6 + num8 / num6) * num * num2) / (num9 + 0.0)
        Dim num13 As Double = (-dx * num3 + dy * num4) / ((num8 + 0.0) * num2)
        Dim arg_140_0 As Double = From_a / num8
        Dim arg_14C_0 As Double = num11 * num8 * num5 / num6
        array(0) = Lat + num12
        array(1) = Lon + num13
        array(0) = array(0) / 3.1415926535897931 * 180.0
        array(1) = array(1) / 3.1415926535897931 * 180.0
        Return array
    End Function
End Class
