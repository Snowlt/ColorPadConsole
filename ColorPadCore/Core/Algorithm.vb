Imports ColorPadCore.Core.Model

'*
' == 管理颜色模型 / 空间转换算法的部分 ==
'*
Namespace Core

    ''' <summary>
    ''' Entry of process color models conversion and calculation
    ''' 处理颜色模型转换和计算的入口
    ''' </summary>
    Public Module ModelsManager
        Private ReadOnly RegisteredConvertors As New Dictionary(Of Type, Object)

        Public ReadOnly Property DefaultConvertAlgorithm() As New Converter.DefaultModelConverters
        Public Property GrayscaleAlgorithm() As Algorithm.Grayscale.IGrayscaleAlgorithm = New Algorithm.Grayscale.GrayComponentAlgorithm()

        ''' <summary>
        ''' Register a convert method (convertor) to convert from one color model to another
        ''' 注册一个转换方法(转换器)，用于将一个颜色模型转换为另一个颜色模型
        ''' </summary>
        ''' <typeparam name="TSource">Source type / 源类型</typeparam>
        ''' <typeparam name="TTarget">Target type / 目标类型</typeparam>
        ''' <param name="convertMethod">Convert method / 转换方法</param>
        Public Sub Register(Of TSource, TTarget)(convertMethod As Func(Of TSource, TTarget))
            If convertMethod Is Nothing Then Throw New ArgumentNullException(NameOf(convertMethod))
            SyncLock RegisteredConvertors
                RegisteredConvertors.Item(GetType(Converter.IConvertFromTo(Of TSource, TTarget))) = convertMethod
            End SyncLock
        End Sub

        ''' <summary>
        ''' Convert from source color model to target color model
        ''' 将源颜色模型转换到目标颜色模型
        ''' </summary>
        ''' <typeparam name="TSource">Source type / 源类型</typeparam>
        ''' <typeparam name="TTarget">Target type / 目标类型</typeparam>
        ''' <param name="source">Source / 源</param>
        ''' <exception cref="ArgumentException">Throw when no suitable convertor found / 没有找到可用的转换器时抛出</exception>
        ''' <returns>Color model after converted / 转换后的颜色模型</returns>
        Public Function Convert(Of TSource, TTarget)(source As TSource) As TTarget
            Try
                Dim convertor As Func(Of TSource, TTarget) =
                    RegisteredConvertors.Item(GetType(Converter.IConvertFromTo(Of TSource, TTarget)))
                Return convertor.Invoke(source)
            Catch ex As KeyNotFoundException
                Throw New ArgumentException(String.Format("No Convertor found for from {0} to {1}",
                                                      GetType(TSource), GetType(TTarget)))
            End Try
        End Function

        ''' <summary>
        ''' Check if is possible to convert from one type to another type
        ''' 检查是否能将一个类型转为另一个类型
        ''' </summary>
        ''' <typeparam name="TSource">Source type / 源类型</typeparam>
        ''' <typeparam name="TTarget">Target type / 目标类型</typeparam>
        ''' <returns>True if possiable / 能转换返回 True</returns>
        Public Function IsConvertable(Of TSource, TTarget)() As Boolean
            Return RegisteredConvertors.ContainsKey(GetType(Converter.IConvertFromTo(Of TSource, TTarget)))
        End Function

        Sub New()
            ' 注册默认的转换算法
            Register(Of Rgb, Hsb)(AddressOf DefaultConvertAlgorithm.RGBToHSB)
            Register(Of Hsb, Rgb)(AddressOf DefaultConvertAlgorithm.HSBToRGB)
            Register(Of Rgb, Hsl)(AddressOf DefaultConvertAlgorithm.RGBToHSL)
            Register(Of Hsl, Rgb)(AddressOf DefaultConvertAlgorithm.HSLToRGB)
            Register(Of Hsb, Hsl)(AddressOf DefaultConvertAlgorithm.HSBToHSL)
            Register(Of Hsl, Hsb)(AddressOf DefaultConvertAlgorithm.HSLToHSB)
            Register(Of Rgb, Cmyk)(AddressOf DefaultConvertAlgorithm.RGBToCMYK)
            Register(Of Cmyk, Rgb)(AddressOf DefaultConvertAlgorithm.CMYKToRGB)
            Register(Of Rgb, YCrCb)(AddressOf DefaultConvertAlgorithm.RGBToYCrCb)
            Register(Of YCrCb, Rgb)(AddressOf DefaultConvertAlgorithm.YCrCbToRGB)
            Register(Of Xyz, Lab)(AddressOf DefaultConvertAlgorithm.XYZToLab)
            Register(Of Lab, Xyz)(AddressOf DefaultConvertAlgorithm.LabToXYZ)
            Register(Of Rgb, Xyz)(AddressOf DefaultConvertAlgorithm.RGBToXYZ)
            Register(Of Xyz, Rgb)(AddressOf DefaultConvertAlgorithm.XYZToRGB)
        End Sub
    End Module

    Namespace Converter
        Public Interface IConvertFromTo(Of In TSource, Out TTarget)

            Function Convert(source As TSource) As TTarget
        End Interface


        Public Class DefaultModelConverters
            Public Function RGBToHSB(rgb As Rgb) As Hsb
                Dim max, min As Integer
                Dim h, s, v As Double
                max = Math.Max(Math.Max(rgb.R, rgb.G), rgb.B)
                min = Math.Min(Math.Min(rgb.R, rgb.G), rgb.B)
                v = max / 255
                If max <> 0 Then
                    s = (max - min) / max
                Else
                    s = 0
                End If
                If DecimalEquals(s, 0) Then
                    h = 0
                Else
                    Select Case max
                        Case rgb.R
                            h = 60 * (rgb.G - rgb.B) / (max - min)
                            If h < 0 Then h += 360
                        Case rgb.G
                            h = 120 + 60 * (rgb.B - rgb.R) / (max - min)
                        Case rgb.B
                            h = 240 + 60 * (rgb.R - rgb.G) / (max - min)
                    End Select
                End If
                Return New Hsb(h, s * 100, v * 100)
            End Function

            Public Function HSBToRGB(hsb As Hsb) As Rgb
                Dim r, g, b, h, s, v As Double
                h = hsb.H Mod 360
                s = hsb.S / 100
                v = hsb.B / 100
                Dim i As Integer
                Dim f, p, q, t As Double
                i = Math.Truncate(h / 60) Mod 6
                f = (h / 60) - i
                p = v * (1 - s)
                q = v * (1 - f * s)
                t = v * (1 - (1 - f) * s)
                Select Case i
                    Case 0
                        r = v
                        g = t
                        b = p
                    Case 1
                        r = q
                        g = v
                        b = p
                    Case 2
                        r = p
                        g = v
                        b = t
                    Case 3
                        r = p
                        g = q
                        b = v
                    Case 4
                        r = t
                        g = p
                        b = v
                    Case 5
                        r = v
                        g = p
                        b = q
                End Select
                Return New Rgb(FRound(r * 255), FRound(g * 255), FRound(b * 255))
            End Function

            'HSL - RGB
            Public Function RGBToHSL(rgb As Rgb) As Hsl
                Dim max, min As Integer
                Dim h, s, l As Double
                max = Math.Max(Math.Max(rgb.R, rgb.G), rgb.B)
                min = Math.Min(Math.Min(rgb.R, rgb.G), rgb.B)
                l = (max + min) / 255 / 2
                If max = min OrElse DecimalEquals(l, 0) Then
                    s = 0
                ElseIf l <= 0.5 Then
                    s = (max - min) / (max + min)
                Else
                    s = (max - min) / (510 - (max + min))
                End If
                If max = min Then
                    h = 0
                Else
                    Select Case max
                        Case rgb.R
                            h = 60 * (rgb.G - rgb.B) / (max - min)
                            If h < 0 Then h += 360
                        Case rgb.G
                            h = 120 + 60 * (rgb.B - rgb.R) / (max - min)
                        Case rgb.B
                            h = 240 + 60 * (rgb.R - rgb.G) / (max - min)
                    End Select
                End If
                Return New Hsl(h, s * 100, l * 100)
            End Function

            Public Function HSLToRGB(hsl As Hsl) As Rgb
                Dim hue = hsl.H, saturation = hsl.S, lightness = hsl.L
                Dim cRgb(2), tRgb(2) As Integer
                Dim sRgb(2) As Double
                Dim q, p As Double
                If DecimalEquals(saturation, 0) Then
                    For i = 0 To cRgb.Length - 1
                        cRgb(i) = lightness * 255 / 100
                    Next
                Else
                    If lightness <= 50 Then
                        q = lightness * (100 + saturation) / 10000
                    Else
                        q = (lightness + saturation) / 100 - (lightness * saturation) / 10000
                    End If
                    p = 2 * lightness / 100 - q
                    tRgb(0) = hue + 120
                    tRgb(1) = hue
                    tRgb(2) = hue - 120
                    For i = 0 To tRgb.Length - 1
                        If tRgb(i) < 0 Then
                            tRgb(i) += 360
                        ElseIf tRgb(i) > 360 Then
                            tRgb(i) -= 360
                        End If
                        If tRgb(i) < 60 Then
                            sRgb(i) = p + (q - p) * (6 * tRgb(i) / 360)
                        ElseIf tRgb(i) < 180 Then
                            sRgb(i) = q
                        ElseIf tRgb(i) < 240 Then
                            sRgb(i) = p + (q - p) * (6 * (240 - tRgb(i)) / 360)
                        Else
                            sRgb(i) = p
                        End If
                        cRgb(i) = FRound(sRgb(i) * 255)
                    Next
                End If
                Return New Rgb(cRgb(0), cRgb(1), cRgb(2))
            End Function

            'HSB/HSV - HSL
            Public Function HSBToHSL(hsb As Hsb) As Hsl
                Dim s, l As Double
                l = hsb.B * (200 - hsb.S) / 200
                If DecimalEquals(l, 0) OrElse DecimalEquals(l, 100) Then
                    s = 0
                Else
                    s = FRound((hsb.B - l) / Math.Min(l, 100 - l) * 100)
                End If
                Return New Hsl(hsb.H, s, l)
            End Function

            Public Function HSLToHSB(hsl As Hsl) As Hsb
                Dim s, b As Double
                b = hsl.L + s * Math.Min(hsl.L, 100 - hsl.L) / 100
                If DecimalEquals(b, 0) Then
                    s = 0
                Else
                    s = FRound(200 - 200 * hsl.L / b)
                End If
                Return New Hsb(hsl.H, s, b)
            End Function

            'CMYK - RGB
            Public Function RGBToCMYK(rgb As Rgb) As Cmyk
                Dim c, m, y, k As Integer
                'RGB转CMYK
                c = 255 - rgb.R
                m = 255 - rgb.G
                y = 255 - rgb.B
                k = Math.Min(Math.Min(c, m), y)
                'CMYK色彩修正
                If k = 255 Then
                    c = FRound(c / 255 * 100)
                    m = FRound(m / 255 * 100)
                    y = FRound(y / 255 * 100)
                    k = 100
                Else
                    c = FRound((c - k) / (255 - k) * 100)
                    m = FRound((m - k) / (255 - k) * 100)
                    y = FRound((y - k) / (255 - k) * 100)
                    k = FRound(k / 255 * 100)
                End If
                Return New Cmyk(c, m, y, k)
            End Function

            Public Function CMYKToRGB(cmyk As Cmyk) As Rgb
                Dim r, g, b As Integer
                r = 225 * (100 - cmyk.C) * (100 - cmyk.K) / 10000
                g = 225 * (100 - cmyk.M) * (100 - cmyk.K) / 10000
                b = 225 * (100 - cmyk.Y) * (100 - cmyk.K) / 10000
                Return New Rgb(r, g, b)
            End Function

            'YCrCb - RGB
            Public Function RGBToYCrCb(rgb As Rgb) As YCrCb
                Const delta As Integer = 128
                Dim y, cr, cb As Double
                y = (rgb.R * 299 + rgb.G * 587 + rgb.B * 114) / 1000
                cr = (500000 * rgb.R - 418688 * rgb.G - 81312 * rgb.B) / 1000000 + delta
                cb = (-168736 * rgb.R - 331264 * rgb.G + 500000 * rgb.B) / 1000000 + delta
                Return New YCrCb(FRound(y), FRound(cr), FRound(cb))
            End Function

            Public Function YCrCbToRGB(yCrCb As YCrCb) As Rgb
                Const delta As Integer = 128
                Dim r, g, b As Double
                r = yCrCb.Y + 1.402 * (yCrCb.Cr - delta)
                g = yCrCb.Y - 0.344136 * (yCrCb.Cb - delta) - 0.714136 * (yCrCb.Cr - delta)
                b = yCrCb.Y + 1.772 * (yCrCb.Cb - delta)
                Return New Rgb(FRound(r), FRound(g), FRound(b))
            End Function

            'XYZ - RGB
            Public Function RGBToXYZ(rgb As Rgb) As Xyz
                'Observer = 2°, Illuminant = D65
                Dim x, y, z, cR, cG, cB As Double
                'Gamma calculation for RGB
                '  Original Gamma formula:
                '  n > 0.04045 ? (n + 0.055) / 1.055 ^ 2.4 : n / 12.92
                If rgb.R > 10 Then
                    cR = ((rgb.R / 255 + 0.055) / 1.055) ^ 2.4
                Else
                    cR = rgb.R * 10 / 32946
                End If
                If rgb.G > 10 Then
                    cG = ((rgb.G / 255 + 0.055) / 1.055) ^ 2.4
                Else
                    cG = rgb.G * 10 / 32946
                End If
                If rgb.B > 10 Then
                    cB = ((rgb.B / 255 + 0.055) / 1.055) ^ 2.4
                Else
                    cB = rgb.B * 10 / 32946
                End If
                'XYZ calculation
                x = cR * 0.4124 + cG * 0.3576 + cB * 0.1805
                y = cR * 0.2126 + cG * 0.7152 + cB * 0.0722
                z = cR * 0.0193 + cG * 0.1192 + cB * 0.9505
                Return New Xyz(x, y, z)
            End Function

            Public Function XYZToRGB(xyz As Xyz) As Rgb
                'Observer = 2°, Illuminant = D65
                Dim cR, cG, cB As Double
                cR = xyz.X * 3.2406 - xyz.Y * 1.5372 - xyz.Z * 0.4986
                cG = xyz.X * -0.9689 + xyz.Y * 1.8758 + xyz.Z * 0.0415
                cB = xyz.X * 0.0557 - xyz.Y * 0.204 + xyz.Z * 1.057
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
                Return New Rgb(FRound(cR * 255), FRound(cG * 255), FRound(cB * 255))
            End Function

            'CIE-Lab - XYZ
            Public Function XYZToLab(xyz As Xyz) As Lab
                Dim x = xyz.X, y = xyz.Y, z = xyz.Z
                Dim fX, fY, fZ, cL, cA, cB As Double
                x /= 0.950456
                z /= 1.088754
                If x > 0.008856 Then
                    fX = x ^ 0.333333
                Else
                    fX = 7.787 * x + 0.137931
                End If
                If y > 0.008856 Then
                    fY = y ^ 0.333333
                Else
                    fY = 7.787 * y + 0.137931
                End If
                If z > 0.008856 Then
                    fZ = z ^ 0.333333
                Else
                    fZ = 7.787 * z + 0.137931
                End If
                '计算CIE-Lab
                If y > 0.008856 Then
                    cL = 116 * fY - 16
                Else
                    cL = 903.3 * y
                End If
                cA = 500 * (fX - fY)
                cB = 200 * (fY - fZ)
                Return New Lab(cL, cA, cB)
            End Function

            Public Function LabToXYZ(lab As Lab) As Xyz
                Dim l = lab.L, a = lab.A, b = lab.B
                Dim x, y, z, fX, fY, fZ As Double
                'Y and f(Y)
                If l > 7.99959 Then
                    'Calculate f(Y) first
                    fY = (l + 16) / 116
                    If fY > 0.2068927 Then
                        y = fY ^ 3
                    Else
                        y = (fY - 0.137931) / 7.787
                    End If
                Else
                    'Calculate Y first
                    y = l / 903.3
                    If y > 0.008856 Then
                        fY = y ^ 0.333333
                    Else
                        fY = 7.787 * y + 0.137931
                    End If
                End If
                'f(X) and f(Z)
                fX = a / 500 + fY
                fZ = fY - b / 200
                'X and Z
                If fX > 0.2068927 Then
                    x = fX ^ 3
                Else
                    x = (fX - 0.137931) / 7.787
                End If
                If fZ > 0.2068927 Then
                    z = fZ ^ 3
                Else
                    z = (fZ - 0.137931) / 7.787
                End If
                x *= 0.950456
                z *= 1.088754
                Return New Xyz(x, y, z)
            End Function
        End Class

    End Namespace


    Namespace Algorithm

        Namespace Grayscale

            Public Interface IGrayscaleAlgorithm
                Function Calc(r As Integer, g As Integer, b As Integer) As Integer
            End Interface

            '*
            ' Grayscale calculations for color
            ' 颜色的灰度值计算
            '*

            Public Class GrayAverageAlgorithm
                Implements IGrayscaleAlgorithm

                ''' <summary>
                ''' Calculate Grayscale of RGB by means of average value
                ''' 使用平均值方式计算 RGB 的灰度值
                ''' </summary>
                ''' <param name="r">Red (0 - 255)</param>
                ''' <param name="g">Green (0 - 255)</param>
                ''' <param name="b">Blue (0 - 255)</param>
                ''' <returns>Grayscale</returns>
                Public Function GrayByAverage(r As Integer, g As Integer, b As Integer) As Integer _
            Implements Grayscale.IGrayscaleAlgorithm.Calc
                    Return FRound((r + g + b) / 3)
                End Function
            End Class

            Public Class GrayComponentAlgorithm
                Implements IGrayscaleAlgorithm

                ''' <summary>
                ''' Calculate Grayscale of RGB by sRGB space component algorithm
                ''' 使用 sRGB 分量方式计算 RGB 的灰度值
                ''' </summary>
                ''' <param name="r">Red (0 - 255)</param>
                ''' <param name="g">Green (0 - 255)</param>
                ''' <param name="b">Blue (0 - 255)</param>
                ''' <returns>Grayscale</returns>
                Public Function GrayBySpaceComponent(r As Integer, g As Integer, b As Integer) As Integer _
            Implements Grayscale.IGrayscaleAlgorithm.Calc
                    Return FRound((r * 299 + g * 587 + b * 114) / 1000)
                End Function
            End Class
        End Namespace

    End Namespace

End Namespace
