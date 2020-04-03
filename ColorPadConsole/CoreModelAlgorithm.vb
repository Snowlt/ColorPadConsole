Imports ColorPadConsole.CPCore
Imports ColorPadConsole.CPModelAlgorithm.Interfaces

'*
' == 管理颜色模型 / 空间转换算法的部分 ==
'*
Namespace CPModelAlgorithm

    Namespace Interfaces

        Public Interface IGetGray

            Function Calc(r As Integer, g As Integer, b As Integer) As Integer

        End Interface

        Public Interface IHexToRGB

            Function Calc(ByRef hex As String) As Integer()

        End Interface

        Public Interface IRGBToHSB

            Function Calc(r As Integer, g As Integer, b As Integer) As Integer()

        End Interface

        Public Interface IHSBToRGB

            Function Calc(h As Integer, s As Integer, b As Integer) As Integer()

        End Interface

        Public Interface IRGBToHSL

            Function Calc(r As Integer, g As Integer, b As Integer) As Integer()

        End Interface

        Public Interface IHSLToRGB

            Function Calc(h As Integer, s As Integer, l As Integer) As Integer()

        End Interface

        Public Interface IHSBToHSL

            Function Calc(h As Integer, s As Integer, b As Integer) As Integer()

        End Interface

        Public Interface IHSLToHSB

            Function Calc(h As Integer, s As Integer, l As Integer) As Integer()

        End Interface

        Public Interface IRGBToCMYK

            Function Calc(r As Integer, g As Integer, b As Integer) As Integer()

        End Interface

        Public Interface ICMYKToRGB

            Function Calc(c As Integer, m As Integer, y As Integer, k As Integer) As Integer()

        End Interface

        Public Interface IRGBToYCrCb

            Function Calc(r As Integer, g As Integer, b As Integer) As Integer()

        End Interface

        Public Interface IYCrCbToRGB

            Function Calc(y As Integer, cr As Integer, cb As Integer) As Integer()

        End Interface

        Public Interface IXYZToLab

            Function Calc(x As Double, y As Double, z As Double) As Double()

        End Interface

        Public Interface ILabToXYZ

            Function Calc(l As Double, a As Double, b As Double) As Double()

        End Interface

        Public Interface IRGBToXYZ

            Function Calc(r As Integer, g As Integer, b As Integer) As Double()

        End Interface

        Public Interface IXYZToRGB

            Function Calc(x As Double, y As Double, z As Double) As Integer()

        End Interface

    End Namespace

    '*
    ' Grayscale calculations for color
    ' 颜色的灰度值计算
    '*

    Class GrayAverage
        Implements IGetGray

        ''' <summary>
        ''' Calculate Grayscale of RGB by means of average value
        ''' 使用平均值方式计算 RGB 的灰度值
        ''' </summary>
        ''' <param name="r">Red (0 - 255)</param>
        ''' <param name="g">Green (0 - 255)</param>
        ''' <param name="b">Blue (0 - 255)</param>
        ''' <returns>Grayscale</returns>
        Public Function GrayByAverage(r As Integer, g As Integer, b As Integer) As Integer Implements IGetGray.Calc
            Return FRound((r + g + b) / 3)
        End Function

    End Class

    Class GrayDefault
        Implements IGetGray

        ''' <summary>
        ''' Calculate Grayscale of RGB by sRGB space component algorithm
        ''' 使用 sRGB 分量方式计算 RGB 的灰度值
        ''' </summary>
        ''' <param name="r">Red (0 - 255)</param>
        ''' <param name="g">Green (0 - 255)</param>
        ''' <param name="b">Blue (0 - 255)</param>
        ''' <returns>Grayscale</returns>
        Public Function GrayBySpaceComponent(r As Integer, g As Integer, b As Integer) As Integer Implements IGetGray.Calc
            Return FRound((r * 299 + g * 587 + b * 114) / 1000)
        End Function

    End Class

    '*
    ' Convert Color Model From One To Others
    ' 将一个颜色模型转为另一个模型
    '
    ' No check for the range of data value(Not suggest for external use)
    ' 不会检查数据范围(不建议在外部使用)
    '*

    Public Class DefaultConv
        Implements IHexToRGB, IRGBToHSB, IHSBToRGB, IRGBToHSL, IHSLToRGB,
                       IHSBToHSL, IHSLToHSB, IRGBToCMYK, ICMYKToRGB, IRGBToYCrCb, IYCrCbToRGB,
                       IXYZToLab, ILabToXYZ, IRGBToXYZ, IXYZToRGB

        'HSB/HSV - RGB
        Public Function RawRGBToHSB(ByVal R As Integer, ByVal G As Integer, ByVal B As Integer) As Integer() _
                Implements IRGBToHSB.Calc
            Dim max, min As Integer
            Dim H, S, V As Single
            max = Math.Max(Math.Max(R, G), B)
            min = Math.Min(Math.Min(R, G), B)
            V = max / 255
            If max <> 0 Then
                S = (max - min) / max
            Else
                S = 0
            End If
            If S = 0 Then
                H = 0
            Else
                Select Case max
                    Case R
                        H = 60 * (G - B) / (max - min)
                        If H < 0 Then H += 360
                    Case G
                        H = 120 + 60 * (B - R) / (max - min)
                    Case B
                        H = 240 + 60 * (R - G) / (max - min)
                End Select
            End If
            H = FRound(H)
            S = FRound(S * 100)
            V = FRound(V * 100)
            Return {H, S, V}
        End Function

        Public Function RawHSBToRGB(Hue As Integer, Saturation As Integer, Brightness As Integer) As Integer() _
                Implements IHSBToRGB.Calc
            Dim R, G, B, H, S, V As Single
            H = Hue Mod 360
            S = Saturation / 100
            V = Brightness / 100
            Dim i As Integer
            Dim f, p, q, t As Single
            i = (Int(H / 60) Mod 6)
            f = (H / 60) - i
            p = V * (1 - S)
            q = V * (1 - f * S)
            t = V * (1 - (1 - f) * S)
            Select Case i
                Case 0
                    R = V
                    G = t
                    B = p
                Case 1
                    R = q
                    G = V
                    B = p
                Case 2
                    R = p
                    G = V
                    B = t
                Case 3
                    R = p
                    G = q
                    B = V
                Case 4
                    R = t
                    G = p
                    B = V
                Case 5
                    R = V
                    G = p
                    B = q
            End Select
            Return {FRound(R * 255), FRound(G * 255), FRound(B * 255)}
        End Function

        'HSL - RGB
        Public Function RawRGBToHSL(ByVal R As Integer, ByVal G As Integer, ByVal B As Integer) As Integer() _
                Implements IRGBToHSL.Calc
            Dim max, min As Integer
            Dim H, S, L As Single
            max = Math.Max(Math.Max(R, G), B)
            min = Math.Min(Math.Min(R, G), B)
            L = (max + min) / 255 / 2
            If L = 0 Or max = min Then
                S = 0
            ElseIf L <= 0.5 Then
                S = (max - min) / (max + min)
            Else
                S = (max - min) / (510 - (max + min))
            End If
            If max = min Then
                H = 0
            Else
                Select Case max
                    Case R
                        H = 60 * (G - B) / (max - min)
                        If H < 0 Then H += 360
                    Case G
                        H = 120 + 60 * (B - R) / (max - min)
                    Case B
                        H = 240 + 60 * (R - G) / (max - min)
                End Select
            End If
            H = FRound(H)
            S = FRound(S * 100)
            L = FRound(L * 100)
            Return {H, S, L}
        End Function

        Public Function RawHSLToRGB(Hue As Integer, Saturation As Integer, Lightness As Integer) As Integer() _
                Implements IHSLToRGB.Calc
            Dim cRGB(2), tRGB(2) As Integer
            Dim sRGB(2) As Single
            Dim q, p As Single
            If Saturation = 0 Then
                For i = 0 To cRGB.Length - 1
                    cRGB(i) = Lightness * 255 / 100
                Next
            Else
                If Lightness <= 50 Then
                    q = Lightness * (100 + Saturation) / 10000
                Else
                    q = (Lightness + Saturation) / 100 - (Lightness * Saturation) / 10000
                End If
                p = 2 * Lightness / 100 - q
                tRGB(0) = Hue + 120
                tRGB(1) = Hue
                tRGB(2) = Hue - 120
                For i = 0 To tRGB.Length - 1
                    If tRGB(i) < 0 Then
                        tRGB(i) += 360
                    ElseIf tRGB(i) > 360 Then
                        tRGB(i) -= 360
                    End If
                    If tRGB(i) < 60 Then
                        sRGB(i) = p + (q - p) * (6 * tRGB(i) / 360)
                    ElseIf tRGB(i) < 180 Then
                        sRGB(i) = q
                    ElseIf tRGB(i) < 240 Then
                        sRGB(i) = p + (q - p) * (6 * (240 - tRGB(i)) / 360)
                    Else
                        sRGB(i) = p
                    End If
                    cRGB(i) = FRound(sRGB(i) * 255)
                Next
            End If
            Return cRGB
        End Function

        'HSB/HSV - HSL
        Public Function RawHSBToHSL(ByVal H As Integer, ByVal S As Integer, ByVal B As Integer) As Integer() _
                Implements IHSBToHSL.Calc
            Dim L As Integer
            L = FRound(B * (200 - S) / 200)
            If L = 0 Or L = 100 Then
                S = 0
            Else
                S = FRound((B - L) / Math.Min(L, 100 - L) * 100)
            End If
            Return {H, S, L}
        End Function

        Public Function RawHSLToHSB(ByVal H As Integer, ByVal S As Integer, ByVal L As Integer) As Integer() _
                Implements IHSLToHSB.Calc
            Dim V As Integer
            V = FRound(L + S * Math.Min(L, 100 - L) / 100)
            If V = 0 Then
                S = 0
            Else
                S = FRound(200 - 200 * L / V)
            End If
            Return {H, S, V}
        End Function

        'CMYK - RGB
        Public Function RawRGBToCMYK(ByVal R As Integer, ByVal G As Integer, ByVal B As Integer) As Integer() _
                Implements IRGBToCMYK.Calc
            Dim C, M, Y, K As Integer
            'RGB转CMYK
            C = 255 - R
            M = 255 - G
            Y = 255 - B
            K = Math.Min(Math.Min(C, M), Y)
            'CMYK色彩修正
            If K = 255 Then
                C = FRound(C / 255 * 100)
                M = FRound(M / 255 * 100)
                Y = FRound(Y / 255 * 100)
                K = 100
            Else
                C = FRound((C - K) / (255 - K) * 100)
                M = FRound((M - K) / (255 - K) * 100)
                Y = FRound((Y - K) / (255 - K) * 100)
                K = FRound(K / 255 * 100)
            End If
            Return {C, M, Y, K}
        End Function

        Public Function RawCMYKToRGB(C As Integer, M As Integer, Y As Integer, K As Integer) As Integer() _
                Implements ICMYKToRGB.Calc
            Dim R, G, B As Integer
            R = 225 * (100 - C) * (100 - K) / 10000
            G = 225 * (100 - M) * (100 - K) / 10000
            B = 225 * (100 - Y) * (100 - K) / 10000
            Return {R, G, B}
        End Function

        'YCrCb - RGB
        Public Function RawRGBToYCrCb(ByVal R As Integer, ByVal G As Integer, ByVal B As Integer) As Integer() _
                Implements IRGBToYCrCb.Calc
            Const delta As Integer = 128
            Dim Y, Cr, Cb As Single
            Y = (R * 299 + G * 587 + B * 114) / 1000
            Cr = (500000 * R - 418688 * G - 81312 * B) / 1000000 + delta
            Cb = (-168736 * R - 331264 * G + 500000 * B) / 1000000 + delta
            Return {FRound(Y), FRound(Cr), FRound(Cb)}
        End Function

        Public Function RawYCrCbToRGB(ByVal Y As Integer, ByVal Cr As Integer, ByVal Cb As Integer) As Integer() _
                Implements IYCrCbToRGB.Calc
            Const delta As Integer = 128
            Dim R, G, B As Single
            R = Y + 1.402 * (Cr - delta)
            G = Y - 0.344136 * (Cb - delta) - 0.714136 * (Cr - delta)
            B = Y + 1.772 * (Cb - delta)
            Return {FRound(R), FRound(G), FRound(B)}
        End Function

        'XYZ - RGB
        Public Function RawRGBToXYZ(ByVal R As Integer, ByVal G As Integer, ByVal B As Integer) As Double() _
                Implements IRGBToXYZ.Calc
            'Observer = 2°, Illuminant = D65
            Dim X, Y, Z, cR, cG, cB As Double
            'Gamma calculation for RGB
            '  Original Gamma formula:
            '  n > 0.04045 ? (n + 0.055) / 1.055 ^ 2.4 : n / 12.92
            If R > 10 Then
                cR = ((R / 255 + 0.055) / 1.055) ^ 2.4
            Else
                cR = R * 10 / 32946
            End If
            If G > 10 Then
                cG = ((G / 255 + 0.055) / 1.055) ^ 2.4
            Else
                cG = G * 10 / 32946
            End If
            If B > 10 Then
                cB = ((B / 255 + 0.055) / 1.055) ^ 2.4
            Else
                cB = B * 10 / 32946
            End If
            'XYZ calculation
            X = cR * 0.4124 + cG * 0.3576 + cB * 0.1805
            Y = cR * 0.2126 + cG * 0.7152 + cB * 0.0722
            Z = cR * 0.0193 + cG * 0.1192 + cB * 0.9505
            Return {X, Y, Z}
        End Function

        Public Function RawXYZToRGB(ByVal X As Double, ByVal Y As Double, ByVal Z As Double) As Integer() _
                Implements IXYZToRGB.Calc
            'Observer = 2°, Illuminant = D65
            Dim cR, cG, cB As Double
            cR = X * 3.2406 - Y * 1.5372 - Z * 0.4986
            cG = X * -0.9689 + Y * 1.8758 + Z * 0.0415
            cB = X * 0.0557 - Y * 0.204 + Z * 1.057
            'Reverse Gamma calculation
            If cR > 0.0031308 Then
                cR = cR ^ 0.4166667 * 1.055 - 0.055
            Else
                cR *= 12.92
            End If
            If cG > 0.0031308 Then
                cG = cG ^ 0.4166667 * 1.055 - 0.055
            Else
                cG *= 12.92
            End If
            If cB > 0.0031308 Then
                cB = cB ^ 0.4166667 * 1.055 - 0.055
            Else
                cB *= 12.92
            End If
            Return {FRound(cR * 255), FRound(cG * 255), FRound(cB * 255)}
        End Function

        'CIE-Lab - XYZ
        Public Function RawXYZToLab(ByVal X As Double, ByVal Y As Double, ByVal Z As Double) As Double() _
                Implements IXYZToLab.Calc
            Dim fX, fY, fZ, cL, cA, cB As Double
            X /= 0.950456
            Z /= 1.088754
            If X > 0.008856 Then
                fX = X ^ 0.333333
            Else
                fX = 7.787 * X + 0.137931
            End If
            If Y > 0.008856 Then
                fY = Y ^ 0.333333
            Else
                fY = 7.787 * Y + 0.137931
            End If
            If Z > 0.008856 Then
                fZ = Z ^ 0.333333
            Else
                fZ = 7.787 * Z + 0.137931
            End If
            '计算CIE-Lab
            If Y > 0.008856 Then
                cL = 116 * fY - 16
            Else
                cL = 903.3 * Y
            End If
            cA = 500 * (fX - fY)
            cB = 200 * (fY - fZ)
            Return {cL, cA, cB}
        End Function

        Public Function RawLabToXYZ(ByVal L As Double, ByVal a As Double, ByVal b As Double) As Double() _
                Implements ILabToXYZ.Calc
            Dim X, Y, Z, fX, fY, fZ As Double
            'Y and f(Y)
            If L > 7.99959 Then
                'Calculate f(Y) first
                fY = (L + 16) / 116
                If fY > 0.2068927 Then
                    Y = fY ^ 3
                Else
                    Y = (fY - 0.137931) / 7.787
                End If
            Else
                'Calculate Y first
                Y = L / 903.3
                If Y > 0.008856 Then
                    fY = Y ^ 0.333333
                Else
                    fY = 7.787 * Y + 0.137931
                End If
            End If
            'f(X) and f(Z)
            fX = a / 500 + fY
            fZ = fY - b / 200
            'X and Z
            If fX > 0.2068927 Then
                X = fX ^ 3
            Else
                X = (fX - 0.137931) / 7.787
            End If
            If fZ > 0.2068927 Then
                Z = fZ ^ 3
            Else
                Z = (fZ - 0.137931) / 7.787
            End If
            X *= 0.950456
            Z *= 1.088754
            Return {X, Y, Z}
        End Function

        'Hex and RGB
        Public Function RawHexToRGB(ByRef hex As String) As Integer() Implements IHexToRGB.Calc
            '返回按数组返回值
            Dim rgb(3) As Integer
            rgb(0) = Val("&H" & hex.Substring(0, 2))
            rgb(1) = Val("&H" & hex.Substring(2, 2))
            rgb(2) = Val("&H" & hex.Substring(4, 2))
            Return rgb
        End Function

    End Class

    ''' <summary>
    ''' Manage the Invoked Algorithm of Conversion
    ''' 管理转换时调用的算法
    ''' </summary>
    Module ConvertorManager
        Public ReadOnly Property DefModelConv() As DefaultConv = New DefaultConv
        Public Property GetGray() As IGetGray = New GrayDefault()
        Public Property RawHexToRGB() As IHexToRGB = DefModelConv
        Public Property RawRGBToHSB As IRGBToHSB = DefModelConv
        Public Property RawHSBToRGB As IHSBToRGB = DefModelConv
        Public Property RawRGBToHSL As IRGBToHSL = DefModelConv
        Public Property RawHSLToRGB As IHSLToRGB = DefModelConv
        Public Property RawHSBToHSL As IHSBToHSL = DefModelConv
        Public Property RawHSLToHSB As IHSLToHSB = DefModelConv
        Public Property RawRGBToCMYK As IRGBToCMYK = DefModelConv
        Public Property RawCMYKToRGB As ICMYKToRGB = DefModelConv
        Public Property RawRGBToYCrCb As IRGBToYCrCb = DefModelConv
        Public Property RawYCrCbToRGB As IYCrCbToRGB = DefModelConv
        Public Property RawXYZToLab As IXYZToLab = DefModelConv
        Public Property RawLabToXYZ As ILabToXYZ = DefModelConv
        Public Property RawRGBToXYZ As IRGBToXYZ = DefModelConv
        Public Property RawXYZToRGB As IXYZToRGB = DefModelConv

    End Module

End Namespace