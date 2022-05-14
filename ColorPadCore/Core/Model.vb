'*
' == 管理颜色模型的部分 ==
'*
Imports System.Text

Namespace Core

    Module Basic
        '*
        ' Basic Functions For Conversion And Color Models
        ' 转换和颜色模型所需的基础函数
        '*

        Public Function FRound(ByRef num As Double) As Integer
            Return Math.Truncate(num + 0.5)
        End Function

        Public Function FIntValue(ByRef s As String) As Integer
            Dim num As Integer = 0
            Dim negative As Boolean = (s(0) = "-")
            For Each c In s
                If Char.IsNumber(c) Then num = num * 10 + AscW(c) - 48
            Next
            Return If(negative, -num, num)
        End Function

        Public Sub FixRange(ByRef value As Integer, lower As Integer, upper As Integer)
            If value > upper Then
                value = upper
            ElseIf value < lower Then
                value = lower
            End If
        End Sub

        Public Sub FixRange(ByRef value As Double, lower As Double, upper As Double)
            If value > upper Then
                value = upper
            ElseIf value < lower Then
                value = lower
            End If
        End Sub

        Public Function GetFixRange(value As Double, lower As Double, upper As Double)
            If value > upper Then
                value = upper
            ElseIf value < lower Then
                value = lower
            End If
            Return value
        End Function

        Public Function DecimalEquals(left As Double, right As Double) As Boolean
            Return Math.Abs(left - right) < 0.000001
        End Function


        '*
        ' Extract color value from a string
        ' 从字符串提取颜色值
        '
        ' Functions will return Nothing when failed to parse, format incorrect or range incorrect
        ' 在解析失败，格式错误或范围错误时，函数会返回 Nothing
        '*

        Public Function ExtractFromString(color As String) As Double()
            If String.IsNullOrEmpty(color) Then Return Array.Empty(Of Double)()
            '处理并分割字符串
            Dim aString() As String = color.Split(",")
            '转换为整型数组
            Dim aDouble(aString.Length - 1) As Double
            For i = 0 To aString.Length - 1
                Try
                    aDouble(i) = Double.Parse(aString(i))
                Catch ex As Exception
                    Return Array.Empty(Of Double)()
                End Try
            Next
            Return aDouble
        End Function

    End Module

    Namespace Model

        '*
        ' Interfaces Of Color Models
        ' 颜色模型的接口
        '*
        Interface IColorModel

            ''' <summary>
            ''' Make a string of Color Model
            ''' 生成表示颜色模型的字符串
            ''' </summary>
            ''' <param name="separator">分隔符</param>
            ''' <returns>表示颜色的字符串</returns>
            Function ToString(separator As String) As String

        End Interface

        '*
        ' Class of Color Model Data
        ' 颜色模型数据的类
        '*

        ''' <summary>
        ''' Class of RGB Color Model
        ''' 表示 RGB 颜色模型的类
        ''' </summary>
        Public Class Rgb
            Implements IColorModel
            Private _r, _g, _b As Integer

            ''' <summary>
            ''' Red (0 - 255)
            ''' </summary>
            ''' <returns></returns>
            Public Property R As Integer
                Get
                    Return _r
                End Get
                Set
                    FixRange(Value, 0, 255)
                    _r = Value
                End Set
            End Property

            ''' <summary>
            ''' Green (0 - 255)
            ''' </summary>
            ''' <returns></returns>
            Public Property G As Integer
                Get
                    Return _g
                End Get
                Set
                    FixRange(Value, 0, 255)
                    _g = Value
                End Set
            End Property

            ''' <summary>
            ''' Blue (0 - 255)
            ''' </summary>
            ''' <returns></returns>
            Public Property B As Integer
                Get
                    Return _b
                End Get
                Set
                    FixRange(Value, 0, 255)
                    _b = Value
                End Set
            End Property

            ''' <summary>
            ''' Initialize the RGB object
            ''' 初始化 RGB 对象
            ''' </summary>
            ''' <param name="r">Red (0 - 255)</param>
            ''' <param name="g">Green (0 - 255)</param>
            ''' <param name="b">Blue (0 - 255)</param>
            Public Sub New(r As Integer, g As Integer, b As Integer)
                Me.R = r
                Me.G = g
                Me.B = b
            End Sub   
            
            Protected Friend Sub New(value As Integer)
                Me.R = (value >> 16) And 255
                Me.G = (value >> 8) And 255
                Me.B = value And 255
            End Sub

            Public Overrides Function Equals(obj As Object) As Boolean
                If TypeOf obj IsNot Rgb Then Return False
                Dim other As Rgb = CType(obj, Rgb)
                Return _r = other._r AndAlso _g = other._g AndAlso _b = other._b
            End Function

            Public Shared Operator =(left As Rgb, right As Rgb) As Boolean
                Return Equals(left, right)
            End Operator

            Public Shared Operator <>(left As Rgb, right As Rgb) As Boolean
                Return Not Equals(left, right)
            End Operator

            Public Overrides Function GetHashCode() As Integer
                Return ToInteger()
            End Function

            Public Overrides Function ToString() As String
                Return String.Format("RGB: ({0},{1},{2})", R, G, B)
            End Function

            Public Overridable Overloads Function ToString(separator As String) As String _
            Implements IColorModel.ToString
                Return String.Format("{0}{3}{1}{3}{2}", R, G, B, separator)
            End Function

            Public Overridable Function CompareWith(r As Integer, g As Integer, b As Integer) As Boolean
                Return _r = r AndAlso _g = g AndAlso _b = b
            End Function

            Public Overridable Function ToHex() As String
                Return ToInteger().ToString("X6")
            End Function
            
            Protected Friend Function ToInteger() As Integer
                Return R << 16 Or G << 8 Or B
            End Function

            ''' <summary>
            ''' Parse out the RGB value from a string
            ''' 从一个字符串解析出 RGB 值
            ''' </summary>
            ''' <param name="color">String of RGB / RGB 字符串</param>
            ''' <returns>RGB Object / RGB 对象</returns>
            Public Shared Function FromString(color As String) As Rgb
                Dim rgb As Integer() = ExtractFromString(color).Select(Function(d) CInt(d)).ToArray()
                '检查格式
                If rgb.Length <> 3 Then Return Nothing
                '检查范围
                Dim r = rgb(0), g = rgb(1), b = rgb(2)
                If r < 0 Or r > 255 Or g < 0 Or g > 255 Or b < 0 Or b > 255 Then
                    Return Nothing
                End If
                '返回对象
                Return New Rgb(r, g, b)
            End Function

            ''' <summary>
            ''' Parse out the RGB value from a Hex string
            ''' 从一个 Hex 字符串中解析出 RGB 值
            ''' </summary>
            ''' <param name="color">String of Hex / Hex 字符串</param>
            ''' <returns>RGB Object / RGB 对象</returns>
            Public Shared Function FromHex(color As String) As Rgb
                If String.IsNullOrEmpty(color) Then Return Nothing
                '处理并分割字符串
                If color.Substring(0, 1) = "#" Then color = color.Substring(1)
                '检查格式
                If color.Length <> 6 Then Return Nothing
                Try
                    Dim v As Integer = Integer.Parse(color, Globalization.NumberStyles.HexNumber)
                    Return New Rgb(v)
                Catch ex As Exception
                    Return Nothing
                End Try
            End Function

            ''' <summary>
            ''' Parse out the RGB value from a Hex string(Support incomplete Hex and CSS style)
            ''' 从一个 Hex 字符串中解析出 RGB 值(支持不完整的 Hex 和 CSS 样式)
            ''' </summary>
            ''' <param name="color">String of Hex / Hex 字符串</param>
            ''' <returns>RGB Object / RGB 对象</returns>
            Public Shared Function FromHexEnhanced(color As String) As Rgb
                If String.IsNullOrEmpty(color) Then Return Nothing
                '处理并分割字符串
                If color.Substring(0, 1) = "#" Then color = color.Substring(1)
                '检查格式
                If color.Length > 6 Then
                    '无效的Hex
                    Return Nothing
                ElseIf color.Length = 3 Then
                    'CSS样式
                    color = New StringBuilder().Append(color(0)).Append(color(0)) _
                        .Append(color(1)).Append(color(1)).Append(color(2)).Append(color(2)).ToString()
                End If
                Try
                    Dim v As Integer = Integer.Parse(color, Globalization.NumberStyles.HexNumber)
                    Return New Rgb(v)
                Catch ex As Exception
                    Return Nothing
                End Try
            End Function

            ''' <summary>
            ''' Generate the RGB value from a string containing Grayscale
            ''' 从一个包含灰度值的字符串中生成 RGB 值
            ''' </summary>
            ''' <param name="color">String containing Grayscale / 包含灰度值的字符串</param>
            ''' <returns>RGB Object / RGB 对象</returns>
            Public Shared Function FromGray(color As String) As Rgb
                If String.IsNullOrEmpty(color) Then Return Nothing
                Dim g As Integer = FIntValue(color)
                If g < 0 Or g > 255 Then Return Nothing
                Return New Rgb(g, g, g)
            End Function

        End Class

        ''' <summary>
        ''' Class of HSB / HSV Color Model
        ''' 表示 HSB / HSV 颜色模型的类
        ''' </summary>
        Public Class Hsb
            Implements IColorModel
            Private _h, _s, _b As Double

            ''' <summary>
            ''' Hue (0 - 360)
            ''' </summary>
            ''' <returns></returns>
            Public Property H As Double
                Get
                    Return _h
                End Get
                Set
                    FixRange(Value, 0, 360)
                    _h = Value
                End Set
            End Property

            ''' <summary>
            ''' Saturation (0 - 100)
            ''' </summary>
            ''' <returns></returns>
            Public Property S As Double
                Get
                    Return _s
                End Get
                Set
                    FixRange(Value, 0, 100)
                    _s = Value
                End Set
            End Property

            ''' <summary>
            ''' Brightness / Value (0 - 100)
            ''' </summary>
            ''' <returns></returns>
            Public Property B As Double
                Get
                    Return _b
                End Get
                Set
                    FixRange(Value, 0, 100)
                    _b = Value
                End Set
            End Property

            ''' <summary>
            ''' Initialize the HSB / HSV object
            ''' 初始化 HSB / HSV 对象
            ''' </summary>
            ''' <param name="h">Hue (0 - 360)</param>
            ''' <param name="s">Saturation (0 - 100)</param>
            ''' <param name="b">Brightness / Value (0 - 100)</param>
            Public Sub New(h As Double, s As Double, b As Double)
                Me.H = h
                Me.S = s
                Me.B = b
            End Sub

            Public Overrides Function Equals(obj As Object) As Boolean
                If TypeOf obj IsNot Hsb Then Return False
                Dim other As Hsb = CType(obj, Hsb)
                Return DecimalEquals(_h, other._h) AndAlso DecimalEquals(_s, other._s) AndAlso DecimalEquals(_b, other._b)
            End Function

            Public Shared Operator =(left As Hsb, right As Hsb) As Boolean
                Return Equals(left, right)
            End Operator

            Public Shared Operator <>(left As Hsb, right As Hsb) As Boolean
                Return Not Equals(left, right)
            End Operator

            Public Overrides Function GetHashCode() As Integer
                Return H << 16 Or S << 8 Or B
            End Function

            Public Overloads Function ToString(separator As String) As String _
            Implements IColorModel.ToString
                Return String.Format("{0:0.##}{3}{1:0.##}{3}{2:0.##}", H, S, B, separator)
            End Function

            Public Overrides Function ToString() As String
                Return String.Format("HSB: ({0},{1},{2})", H, S, B)
            End Function

            Public Function CompareWith(h As Integer, s As Integer, b As Integer) As Boolean
                Return DecimalEquals(_h, h) AndAlso DecimalEquals(_s, s) AndAlso DecimalEquals(_b, b)
            End Function

            ''' <summary>
            ''' Parse out the HSB value from a string
            ''' 从一个字符串解析出 HSB 值
            ''' </summary>
            ''' <param name="color">String of HSB / HSB 字符串</param>
            ''' <returns>HSB Object / HSB 对象</returns>
            Public Shared Function FromString(color As String) As Hsb
                Dim hsb() As Double = ExtractFromString(color)
                '检查格式
                If hsb.Length <> 3 Then Return Nothing
                '检查范围
                Dim h = hsb(0), s = hsb(1), b = hsb(2)
                If h < 0 Or h > 360 Or s < 0 Or s > 100 Or b < 0 Or b > 100 Then
                    Return Nothing
                End If
                '返回对象
                Return New Hsb(h, s, b)
            End Function

        End Class

        ''' <summary>
        ''' Class of HSL Color Model
        ''' 表示 HSL 颜色模型的类
        ''' </summary>
        Public Class Hsl
            Implements IColorModel
            Private _h, _s, _l As Double

            ''' <summary>
            ''' Hue (0 - 360)
            ''' </summary>
            ''' <returns></returns>
            Public Property H As Double
                Get
                    Return _h
                End Get
                Set
                    FixRange(Value, 0, 360)
                    _h = Value
                End Set
            End Property

            ''' <summary>
            ''' Saturation (0 - 100)
            ''' </summary>
            ''' <returns></returns>
            Public Property S As Double
                Get
                    Return _s
                End Get
                Set
                    FixRange(Value, 0, 100)
                    _s = Value
                End Set
            End Property

            ''' <summary>
            ''' Lightness (0 - 100)
            ''' </summary>
            ''' <returns></returns>
            Public Property L As Double
                Get
                    Return _l
                End Get
                Set
                    FixRange(Value, 0, 100)
                    _l = Value
                End Set
            End Property

            ''' <summary>
            ''' Initialize the HSL object
            ''' 初始化 HSL 对象
            ''' </summary>
            ''' <param name="h">Hue (0 - 360)</param>
            ''' <param name="s">Saturation (0 - 100)</param>
            ''' <param name="l">Lightness (0 - 100)</param>
            Public Sub New(h As Double, s As Double, l As Double)
                Me.H = h
                Me.S = s
                Me.L = l
            End Sub

            Public Overrides Function Equals(obj As Object) As Boolean
                If TypeOf obj IsNot Hsl Then Return False
                Dim other As Hsl = CType(obj, Hsl)
                Return DecimalEquals(_h, other._h) AndAlso DecimalEquals(_s, other._s) AndAlso DecimalEquals(_l, other._l)
            End Function

            Public Shared Operator =(left As Hsl, right As Hsl) As Boolean
                Return Equals(left, right)
            End Operator

            Public Shared Operator <>(left As Hsl, right As Hsl) As Boolean
                Return Not Equals(left, right)
            End Operator

            Public Overrides Function GetHashCode() As Integer
                Return H << 16 Or S << 8 Or L
            End Function

            Public Overrides Function ToString() As String
                Return String.Format("HSL: ({0},{1},{2})", H, S, L)
            End Function

            Public Overloads Function ToString(separator As String) As String _
            Implements IColorModel.ToString
                Return String.Format("{0:0.##}{3}{1:0.##}{3}{2:0.##}", H, S, L, separator)
            End Function

            Public Function CompareWith(h As Integer, s As Integer, l As Integer) As Boolean
                Return DecimalEquals(_h, h) AndAlso DecimalEquals(_s, s) AndAlso DecimalEquals(_l, l)
            End Function

            ''' <summary>
            ''' Parse out the HSL value from a string
            ''' 从一个字符串解析出 HSL 值
            ''' </summary>
            ''' <param name="color">String of HSL / HSL 字符串</param>
            ''' <returns>HSL Object / HSL 对象</returns>
            Public Shared Function FromString(color As String) As Hsl
                Dim hsl() As Double = ExtractFromString(color)
                '检查格式
                If hsl.Length <> 3 Then Return Nothing
                '检查范围
                Dim h = hsl(0), s = hsl(1), l = hsl(2)
                If h < 0 Or h > 360 Or s < 0 Or s > 100 Or l < 0 Or l > 100 Then
                    Return Nothing
                End If
                '返回对象
                Return New Hsl(h, s, l)
            End Function

        End Class

        ''' <summary>
        ''' Class of CMYK Color Model
        ''' 表示 CMYK 颜色模型的类
        ''' </summary>
        Public Class Cmyk
            Implements IColorModel
            Private _c, _m, _y, _k As Integer

            ''' <summary>
            ''' Cyan (0 - 100)
            ''' </summary>
            ''' <returns></returns>
            Public Property C As Integer
                Get
                    Return _c
                End Get
                Set
                    FixRange(Value, 0, 100)
                    _c = Value
                End Set
            End Property

            ''' <summary>
            ''' Magenta (0 - 100)
            ''' </summary>
            ''' <returns></returns>
            Public Property M As Integer
                Get
                    Return _m
                End Get
                Set
                    FixRange(Value, 0, 100)
                    _m = Value
                End Set
            End Property

            ''' <summary>
            ''' Yellow (0 - 100)
            ''' </summary>
            ''' <returns></returns>
            Public Property Y As Integer
                Get
                    Return _y
                End Get
                Set
                    FixRange(Value, 0, 100)
                    _y = Value
                End Set
            End Property

            ''' <summary>
            ''' Key / Black (0 - 100)
            ''' </summary>
            ''' <returns></returns>
            Public Property K As Integer
                Get
                    Return _k
                End Get
                Set
                    FixRange(Value, 0, 100)
                    _k = Value
                End Set
            End Property

            ''' <summary>
            ''' Initialize the CMYK object
            ''' 初始化 CMYK 对象
            ''' </summary>
            ''' <param name="c">Cyan (0 - 100)</param>
            ''' <param name="m">Magenta (0 - 100)</param>
            ''' <param name="y">Yellow (0 - 100)</param>
            ''' <param name="k">Key / Black (0 - 100)</param>
            Public Sub New(c As Integer, m As Integer, y As Integer, k As Integer)
                Me.C = c
                Me.M = m
                Me.Y = y
                Me.K = k
            End Sub

            Public Overrides Function Equals(obj As Object) As Boolean
                If TypeOf obj IsNot Cmyk Then Return False
                Dim other As Cmyk = CType(obj, Cmyk)
                Return _m = other._m AndAlso _m = other._m AndAlso _y = other._y AndAlso _k = other._k
            End Function

            Public Shared Operator =(left As Cmyk, right As Cmyk) As Boolean
                Return Equals(left, right)
            End Operator

            Public Shared Operator <>(left As Cmyk, right As Cmyk) As Boolean
                Return Not Equals(left, right)
            End Operator

            Public Overrides Function GetHashCode() As Integer
                Return C << 24 Or M << 16 Or Y << 8 Or K
            End Function

            Public Overrides Function ToString() As String
                Return String.Format("CMYK: ({0},{1},{2},{3})", C, M, Y, K)
            End Function

            Public Overloads Function ToString(separator As String) As String _
            Implements IColorModel.ToString
                Return String.Format("{0}{4}{1}{4}{2}{4}{3}", C, M, Y, K, separator)
            End Function

            Public Function CompareWith(c As Integer, m As Integer, y As Integer, k As Integer) As Boolean
                Return _c = c AndAlso _m = m AndAlso _y = y AndAlso _k = k
            End Function

            ''' <summary>
            ''' Parse out the CMYK value from a string
            ''' 从一个字符串解析出 CMYK 值
            ''' </summary>
            ''' <param name="color">String of CMYK / CMYK 字符串</param>
            ''' <returns>CMYK Object / CMYK 对象</returns>
            Public Shared Function FromString(color As String) As Cmyk
                Dim cmyk As Integer() = ExtractFromString(color).Select(Function(d) CInt(d)).ToArray
                '检查格式
                If cmyk.Length <> 4 Then Return Nothing
                '检查范围
                Dim c, m, y, k As Integer
                c = cmyk(0)
                m = cmyk(1)
                y = cmyk(2)
                k = cmyk(3)
                If c < 0 Or c > 100 Or m < 0 Or m > 100 Or y < 0 Or y > 100 Or k < 0 Or k > 100 Then
                    Return Nothing
                End If
                '返回对象
                Return New Cmyk(c, m, y, k)
            End Function

        End Class

        ''' <summary>
        ''' Class of YCrCb Color Model
        ''' 表示 YCrCb 颜色模型的类
        ''' </summary>
        Public Class YCrCb
            Implements IColorModel
            Private _y, _cr, _cb As Integer

            ''' <summary>
            ''' Y (0 - 255)
            ''' </summary>
            ''' <returns></returns>
            Public Property Y As Integer
                Get
                    Return _y
                End Get
                Set
                    FixRange(Value, 0, 255)
                    _y = Value
                End Set
            End Property

            ''' <summary>
            ''' Cr (0 - 255)
            ''' </summary>
            ''' <returns></returns>
            Public Property Cr As Integer
                Get
                    Return _cr
                End Get
                Set
                    FixRange(Value, 0, 255)
                    _cr = Value
                End Set
            End Property

            ''' <summary>
            ''' Cb (0 - 255)
            ''' </summary>
            ''' <returns></returns>
            Public Property Cb As Integer
                Get
                    Return _cb
                End Get
                Set
                    FixRange(Value, 0, 255)
                    _cb = Value
                End Set
            End Property

            ''' <summary>
            ''' Initialize the YCrCb object
            ''' 初始化 YCrCb 对象
            ''' </summary>
            ''' <param name="y">Y (0 - 255)</param>
            ''' <param name="cr">Cr (0 - 255)</param>
            ''' <param name="cb">Cb (0 - 255)</param>
            Public Sub New(y As Integer, cr As Integer, cb As Integer)
                Me.Y = y
                Me.Cr = cr
                Me.Cb = cb
            End Sub

            Public Overrides Function Equals(obj As Object) As Boolean
                If TypeOf obj IsNot YCrCb Then Return False
                Dim other As YCrCb = CType(obj, YCrCb)
                Return _y = other._y AndAlso _cr = other._cr AndAlso _cb = other._cb
            End Function

            Public Shared Operator =(left As YCrCb, right As YCrCb) As Boolean
                Return Equals(left, right)
            End Operator

            Public Shared Operator <>(left As YCrCb, right As YCrCb) As Boolean
                Return Not Equals(left, right)
            End Operator

            Public Overrides Function GetHashCode() As Integer
                Return Y << 16 Or Cr << 8 Or Cb
            End Function

            Public Overrides Function ToString() As String
                Return String.Format("YCrCb: ({0},{1},{2})", Y, Cr, Cb)
            End Function

            Public Overloads Function ToString(separator As String) As String _
            Implements IColorModel.ToString
                Return String.Format("{0}{3}{1}{3}{2}", Y, Cr, Cb, separator)
            End Function

            Public Function CompareWith(y As Integer, cr As Integer, cb As Integer) As Boolean
                Return _y = y AndAlso _cr = cr AndAlso _cb = cb
            End Function

            ''' <summary>
            ''' Parse out the YCrCb value from a string
            ''' 从一个字符串解析出 YCrCb 值
            ''' </summary>
            ''' <param name="color">String of YCrCb / YCrCb 字符串</param>
            ''' <returns>YCrCb Object / YCrCb 对象</returns>
            Public Shared Function FromString(color As String) As YCrCb
                Dim yCrCb As Integer() = ExtractFromString(color).Select(Function(d) CInt(d)).ToArray
                '检查格式
                If yCrCb.Length <> 3 Then Return Nothing
                '检查范围
                Dim y, cr, cb As Integer
                y = yCrCb(0)
                cr = yCrCb(1)
                cb = yCrCb(2)
                If y < 0 Or y > 255 Or cr < 0 Or cr > 255 Or cb < 0 Or cb > 255 Then
                    Return Nothing
                End If
                '返回对象
                Return New YCrCb(y, cr, cb)
            End Function

        End Class

        ''' <summary>
        ''' Class to store the values of X, Y, Z in CIE-XYZ Color Space
        ''' 用来存储 CIE-XYZ 颜色空间 X, Y, Z 值的类
        ''' </summary>
        Public Class Xyz
            Implements IColorModel

            Private _x, _y, _z As Double

            ''' <summary>
            ''' Value of X
            ''' X 的值
            ''' </summary>
            ''' <returns></returns>
            Public Property X As Double
                Get
                    Return _x
                End Get
                Set
                    OnSetXValue(Value)
                    _x = Value
                End Set
            End Property

            ''' <summary>
            ''' Value of Y
            ''' Y 的值
            ''' </summary>
            ''' <returns></returns>
            Public Property Y As Double
                Get
                    Return _y
                End Get
                Set
                    OnSetYValue(Value)
                    _y = Value
                End Set
            End Property

            ''' <summary>
            ''' Value of Z
            ''' Z 的值
            ''' </summary>
            ''' <returns></returns>
            Public Property Z As Double
                Get
                    Return _z
                End Get
                Set
                    OnSetZValue(Value)
                    _z = Value
                End Set
            End Property

            Protected Sub New()
            End Sub

            ''' <summary>
            ''' Initialize the XYZ object
            ''' 初始化 XYZ 对象
            ''' </summary>
            ''' <param name="x">X</param>
            ''' <param name="y">Y</param>
            ''' <param name="z">Z</param>
            Public Sub New(x As Double, y As Double, z As Double)
                Me.X = x
                Me.Y = y
                Me.Z = z
            End Sub

            ''' <summary>
            ''' Initialize the new XYZ object from existed one
            ''' 从现有的对象初始化新 XYZ 对象
            ''' </summary>
            ''' <param name="xyz">Existed XYZ Object / 现有的对象</param>
            Public Sub New(xyz As Xyz)
                Me.New(xyz.X, xyz.Y, xyz.Z)
            End Sub

            Protected Overridable Sub OnSetXValue(ByRef value As Double)
            End Sub

            Protected Overridable Sub OnSetYValue(ByRef value As Double)
            End Sub

            Protected Overridable Sub OnSetZValue(ByRef value As Double)
            End Sub

            Public Overrides Function Equals(obj As Object) As Boolean
                If TypeOf obj IsNot Xyz Then Return False
                Dim other As Xyz = CType(obj, Xyz)
                Return DecimalEquals(_x, other._x) AndAlso DecimalEquals(_y, other._y) AndAlso DecimalEquals(_z, other._z)
            End Function

            Public Shared Operator =(left As Xyz, right As Xyz) As Boolean
                Return Equals(left, right)
            End Operator

            Public Shared Operator <>(left As Xyz, right As Xyz) As Boolean
                Return Not Equals(left, right)
            End Operator

            Public Overrides Function GetHashCode() As Integer
                'X: 10 bit, Y: 10 bit, Z: 11 bit
                Return CInt(X * 1000) << 21 Or CInt(Y * 1000) << 11 Or CInt(Z * 1000)
            End Function

            Public Overrides Function ToString() As String
                Return String.Format("CIE-XYZ: ({0},{1},{2})", X, Y, Z)
            End Function

            Public Overloads Function ToString(separator As String) As String _
            Implements IColorModel.ToString
                Return String.Format("{0:0.#####}{3}{1:0.#####}{3}{2:0.#####}", X, Y, Z, separator)
            End Function

            Public Function CompareWith(x As Double, y As Double, z As Double) As Boolean
                Return DecimalEquals(_x, x) AndAlso DecimalEquals(_y, y) AndAlso DecimalEquals(_z, z)
            End Function

            ''' <summary>
            ''' Parse out the XYZ value from a string(No range limited)
            ''' 从一个字符串解析出 XYZ 值(无范围限制)
            ''' </summary>
            ''' <param name="color">String of XYZ / XYZ 字符串</param>
            ''' <returns>CIE-XYZ Object / CIE-XYZ 对象</returns>
            Public Shared Function FromString(color As String) As Xyz
                Dim xyz() As Double = ExtractFromString(color)
                '返回对象
                Return New Xyz(xyz(0), xyz(1), xyz(2))
            End Function

        End Class

        ''' <summary>
        ''' CIE-XYZ Color Space (Illuminant = D65)
        ''' CIE-XYZ 颜色空间(Illuminant = D65)
        ''' </summary>
        Public Class CieXyzD65
            Inherits Xyz

            ''' <summary>
            ''' Initialize the XYZ object (range limited)
            ''' 初始化 XYZ 对象(有范围限制)
            ''' </summary>
            ''' <param name="x">X (0 - 0.95047)</param>
            ''' <param name="y">Y (0 - 1.000)</param>
            ''' <param name="z">Z (0 - 1.08883)</param>
            Public Sub New(x As Double, y As Double, z As Double)
                MyBase.New(x, y, z)
            End Sub

            Protected Overrides Sub OnSetXValue(ByRef value As Double)
                FixRange(value, 0, 0.95047)
            End Sub

            Protected Overrides Sub OnSetYValue(ByRef value As Double)
                FixRange(value, 0, 1)
            End Sub

            Protected Overrides Sub OnSetZValue(ByRef value As Double)
                FixRange(value, 0, 1.08883)
            End Sub

            ''' <summary>
            ''' Parse out the XYZ value from a string(Range limited)
            ''' 从一个字符串解析出 XYZ 值(有范围限制)
            ''' X (0 - 0.95047), Y (0 - 1.0), Z (0 - 1.08883)
            ''' </summary>
            ''' <param name="color">String of XYZ / XYZ 字符串</param>
            ''' <returns>CIE XYZ D65 Object / CIE XYZ D65 对象</returns>
            Public Overloads Shared Function FromString(color As String) As CieXyzD65
                Dim xyz() As Double = ExtractFromString(color)
                '检查格式
                If xyz.Length <> 3 Then Return Nothing
                '检查范围
                Dim x = xyz(0), y = xyz(1), z = xyz(2)
                'X: 0 to 0.95047
                'Y: 0 to 1.00000
                'Z: 0 to 1.08883
                If x < 0 Or x > 0.95047 Or y < 0 Or y > 1.0 Or z < 0 Or z > 1.08883 Then
                    Return Nothing
                End If
                '返回对象
                Return New Xyz(x, y, z)
            End Function

        End Class

        ''' <summary>
        ''' Class of CIE-Lab Color Model
        ''' 表示 CIE-Lab 颜色模型的类
        ''' </summary>
        Public Class Lab
            Implements IColorModel

            'Range: L: 0 to 100
            '       a: -128 to 127
            '       b: -128 to 127
            Private _l, _a, _b As Double

            ''' <summary>
            ''' L (0 - 100)
            ''' </summary>
            ''' <returns></returns>
            Public Property L As Double
                Get
                    Return _l
                End Get
                Set
                    FixRange(Value, 0, 100)
                    _l = Value
                End Set
            End Property

            ''' <summary>
            ''' a (-128 - 127)
            ''' </summary>
            ''' <returns></returns>
            Public Property A As Double
                Get
                    Return _a
                End Get
                Set
                    FixRange(Value, -128, 127)
                    _a = Value
                End Set
            End Property

            ''' <summary>
            ''' b (-128 - 127)
            ''' </summary>
            ''' <returns></returns>
            Public Property B As Double
                Get
                    Return _b
                End Get
                Set
                    FixRange(Value, -128, 127)
                    _b = Value
                End Set
            End Property

            ''' <summary>
            ''' Initialize the Lab object
            ''' 初始化 Lab 对象
            ''' </summary>
            ''' <param name="l">L (0 - 100)</param>
            ''' <param name="a">a (-128 - 127)</param>
            ''' <param name="b">b (-128 - 127)</param>
            Public Sub New(l As Double, a As Double, b As Double)
                Me.L = l
                Me.A = a
                Me.B = b
            End Sub

            Public Overrides Function Equals(obj As Object) As Boolean
                If TypeOf obj IsNot Lab Then Return False
                Dim other As Lab = CType(obj, Lab)
                Return DecimalEquals(_l, other._l) AndAlso DecimalEquals(_a, other._a) AndAlso DecimalEquals(_b, other._b)
            End Function

            Public Shared Operator =(left As Lab, right As Lab) As Boolean
                Return Equals(left, right)
            End Operator

            Public Shared Operator <>(left As Lab, right As Lab) As Boolean
                Return Not Equals(left, right)
            End Operator

            Public Overrides Function GetHashCode() As Integer
                Return L << 16 Or (A + 128) << 8 Or (B + 128)
            End Function

            Public Overrides Function ToString() As String
                Return String.Format("CIE-Lab: ({0},{1},{2})", L, A, B)
            End Function

            Public Overloads Function ToString(separator As String) As String _
            Implements IColorModel.ToString
                Return String.Format("{0:0.##}{3}{1:0.##}{3}{2:0.##}", L, A, B, separator)
            End Function

            Public Function CompareWith(l As Integer, a As Integer, b As Integer) As Boolean
                Return DecimalEquals(_l, l) AndAlso DecimalEquals(_a, a) AndAlso DecimalEquals(_b, b)
            End Function

            ''' <summary>
            ''' Parse out the CIE-Lab value from a string
            ''' 从一个字符串解析出 CIE-Lab 值
            ''' </summary>
            ''' <param name="color">String of CIE-Lab / CIE-Lab 字符串</param>
            ''' <returns>CIELab Object / CIELab 对象</returns>
            Public Shared Function FromString(color As String) As Lab
                Dim lab() As Double = ExtractFromString(color)
                '检查格式
                If lab.Length <> 3 Then Return Nothing
                '检查范围
                Dim l = lab(0), a = lab(1), b = lab(2)
                If l < 0 Or l > 100 Or a < -128 Or a > 127 Or b < -128 Or b > 127 Then
                    Return Nothing
                End If
                '返回对象
                Return New Lab(l, a, b)
            End Function

        End Class

    End Namespace

End Namespace
