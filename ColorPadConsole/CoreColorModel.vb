Imports ColorPadConsole.CPCore
Imports ColorPadConsole.CPModelAlgorithm
Imports ColorPadConsole.CPModel
Imports ColorPadConsole.CPModel.Interfaces

'*
' == 管理颜色模型和模型转换的部分 ==
'*

Namespace CPModel

    ''' <summary>
    ''' Aid Functions for Operate Color Model / Conversion
    ''' 操作颜色模型 / 转换的辅助函数
    ''' </summary>
    Module ColorModelTools

        '*
        ' Calculate Grayscale
        ' 计算灰阶值
        '*
        Public Function CalcGray(r As Integer, g As Integer, b As Integer) As Integer
            Return GetGray.Calc(r, g, b)
        End Function

        Public Function CalcGray(ByRef rgb As RGB) As Integer
            Return GetGray.Calc(rgb.R, rgb.G, rgb.B)
        End Function

        '*
        ' Extract color value from a string
        ' 从字符串提取颜色值
        '
        ' Functions will return Nothing when failed to parse, format incorrect or range incorrect
        ' 在解析失败，格式错误或范围错误时，函数会返回 Nothing
        '*

        Private Function ExtractFromString(ByRef color As String) As Integer()
            If String.IsNullOrEmpty(color) Then Return {}
            '处理并分割字符串
            Dim aColor() As String = color.Split(",")
            '转换为整型数组
            Dim cColor(aColor.Length - 1) As Integer
            For i = 0 To aColor.Length - 1
                cColor(i) = FIntValue(aColor(i))
            Next
            Return cColor
        End Function

        Private Function ExtractFromStringAsDouble(color As String) As Double()
            If color.Length = 0 Then Return {}
            '处理并分割字符串
            Dim aString() As String = color.Split(",")
            '转换为整型数组
            Dim aDouble(aString.Length - 1) As Double
            For i = 0 To aString.Length - 1
                Try
                    aDouble(i) = Val(aString(i))
                Catch ex As OverflowException
                    Return {}
                End Try
            Next
            Return aDouble
        End Function

        ''' <summary>
        ''' Parse out the RGB value from a string
        ''' 从一个字符串解析出 RGB 值
        ''' </summary>
        ''' <param name="color">String of RGB / RGB 字符串</param>
        ''' <returns>RGB Object / RGB 对象</returns>
        Public Function ExtractFromRGB(color As String) As RGB
            Dim rgb() As Integer = ExtractFromString(color)
            '检查格式
            If rgb.Length <> 3 Then Return Nothing
            '检查范围
            Dim R, G, B As Integer
            R = rgb(0)
            G = rgb(1)
            B = rgb(2)
            If R < 0 Or R > 255 Or G < 0 Or G > 255 Or B < 0 Or B > 255 Then
                Return Nothing
            End If
            '返回对象
            Return New RGB(R, G, B)
        End Function

        ''' <summary>
        ''' Parse out the RGB value from a Hex string
        ''' 从一个 Hex 字符串中解析出 RGB 值
        ''' </summary>
        ''' <param name="color">String of Hex / Hex 字符串</param>
        ''' <returns>RGB Object / RGB 对象</returns>
        Public Function ExtractFromHex(color As String) As RGB
            If color.Length = 0 Then Return Nothing
            '处理并分割字符串
            If color.Substring(0, 1) = "#" Then color = color.Substring(1)
            '检查格式
            If color.Length <> 6 Then
                Return Nothing
            End If
            Dim rgb() As Integer = RawHexToRGB.Calc(color)
            Return New RGB(rgb(0), rgb(1), rgb(2))
        End Function

        ''' <summary>
        ''' Parse out the RGB value from a Hex string(Support incomplete Hex and CSS style)
        ''' 从一个 Hex 字符串中解析出 RGB 值(支持不完整的 Hex 和 CSS 样式)
        ''' </summary>
        ''' <param name="color">String of Hex / Hex 字符串</param>
        ''' <returns>RGB Object / RGB 对象</returns>
        Public Function ExtractFromHexEnhanced(color As String) As RGB
            If color.Length = 0 Then Return Nothing
            '处理并分割字符串
            If color.Substring(0, 1) = "#" Then color = color.Substring(1)
            '检查格式
            If color.Length > 6 Then
                '无效的Hex
                Return Nothing
            ElseIf color.Length = 3 Then
                'CSS样式
                color = String.Format("{0}{0}{1}{1}{2}{2}", color(0), color(1), color(2))
            Else
                '小于6位长度的Hex
                color = color.PadLeft(6, "0")
            End If
            Dim rgb() As Integer = RawHexToRGB.Calc(color)
            Return New RGB(rgb(0), rgb(1), rgb(2))
        End Function

        ''' <summary>
        ''' Generate the RGB value from a string containing Grayscale
        ''' 从一个包含灰度值的字符串中生成 RGB 值
        ''' </summary>
        ''' <param name="color">String containing Grayscale / 包含灰度值的字符串</param>
        ''' <returns>RGB Object / RGB 对象</returns>
        Public Function ExtractFromGray(color As String) As RGB
            If color.Length = 0 Then Return Nothing
            Dim G As Integer = FIntValue(color)
            If G < 0 Or G > 255 Then
                Return Nothing
            End If
            Return New RGB(G, G, G)
        End Function

        ''' <summary>
        ''' Parse out the HSB value from a string
        ''' 从一个字符串解析出 HSB 值
        ''' </summary>
        ''' <param name="color">String of HSB / HSB 字符串</param>
        ''' <returns>HSB Object / HSB 对象</returns>
        Public Function ExtractFromHSB(color As String) As HSB
            Dim hsb() As Integer = ExtractFromString(color)
            '检查格式
            If hsb.Length <> 3 Then Return Nothing
            '检查范围
            Dim H, S, B As Integer
            H = hsb(0)
            S = hsb(1)
            B = hsb(2)
            If H < 0 Or H > 360 Or S < 0 Or S > 100 Or B < 0 Or B > 100 Then
                Return Nothing
            End If
            '返回对象
            Return New HSB(H, S, B)
        End Function

        ''' <summary>
        ''' Parse out the HSB value from a string
        ''' 从一个字符串解析出 HSB 值
        ''' </summary>
        ''' <param name="color">String of HSB / HSB 字符串</param>
        ''' <returns>HSB Object / HSB 对象</returns>
        Public Function ExtractFromHSL(color As String) As HSL
            Dim hsl() As Integer = ExtractFromString(color)
            '检查格式
            If hsl.Length <> 3 Then Return Nothing
            '检查范围
            Dim H, S, L As Integer
            H = hsl(0)
            S = hsl(1)
            L = hsl(2)
            If H < 0 Or H > 360 Or S < 0 Or S > 100 Or L < 0 Or L > 100 Then
                Return Nothing
            End If
            '返回对象
            Return New HSL(H, S, L)
        End Function

        ''' <summary>
        ''' Parse out the HSL value from a string
        ''' 从一个字符串解析出 HSL 值
        ''' </summary>
        ''' <param name="color">String of HSL / HSL 字符串</param>
        ''' <returns>HSL Object / HSL 对象</returns>
        Public Function ExtractFromCMYK(color As String) As CMYK
            Dim cmyk() As Integer = ExtractFromString(color)
            '检查格式
            If cmyk.Length <> 4 Then Return Nothing
            '检查范围
            Dim C, M, Y, K As Integer
            C = cmyk(0)
            M = cmyk(1)
            Y = cmyk(2)
            K = cmyk(3)
            If C < 0 Or C > 100 Or M < 0 Or M > 100 Or Y < 0 Or Y > 100 Or K < 0 Or K > 100 Then
                Return Nothing
            End If
            '返回对象
            Return New CMYK(C, M, Y, K)
        End Function

        ''' <summary>
        ''' Parse out the YCrCb value from a string
        ''' 从一个字符串解析出 YCrCb 值
        ''' </summary>
        ''' <param name="color">String of YCrCb / YCrCb 字符串</param>
        ''' <returns>YCrCb Object / YCrCb 对象</returns>
        Public Function ExtractFromYCrCb(color As String) As YCrCb
            Dim ycrcb() As Integer = ExtractFromString(color)
            '检查格式
            If ycrcb.Length <> 3 Then Return Nothing
            '检查范围
            Dim Y, Cr, Cb As Integer
            Y = ycrcb(0)
            Cr = ycrcb(1)
            Cb = ycrcb(2)
            If Y < 0 Or Y > 255 Or Cr < 0 Or Cr > 255 Or Cb < 0 Or Cb > 255 Then
                Return Nothing
            End If
            '返回对象
            Return New YCrCb(Y, Cr, Cb)
        End Function

        ''' <summary>
        ''' Parse out the XYZ value from a string(Range limited, )
        ''' 从一个字符串解析出 XYZ 值(有范围限制, 无缩放)
        ''' </summary>
        ''' <param name="color">String of XYZ / XYZ 字符串</param>
        ''' <returns>CIEXYZ Object / CIEXYZ 对象</returns>
        Public Function ExtractFromXYZ(color As String) As XYZ
            Dim xyz() As Double = ExtractFromStringAsDouble(color)
            '检查格式
            If xyz.Length <> 3 Then Return Nothing
            '检查范围
            Dim X, Y, Z As Double
            X = xyz(0)
            Y = xyz(1)
            Z = xyz(2)
            'X: 0 to 0.95047
            'Y: 0 to 1.00000
            'Z: 0 to 1.08883
            If X < 0 Or X > 0.95047 Or Y < 0 Or Y > 1.0 Or Z < 0 Or Z > 1.08883 Then
                Return Nothing
            End If
            '返回对象
            Return New XYZ(X, Y, Z)
        End Function

        ''' <summary>
        ''' Parse out the XYZ value from a string(No range limited)
        ''' 从一个字符串解析出 XYZ 值(无范围限制)
        ''' </summary>
        ''' <param name="color">String of XYZ / XYZ 字符串</param>
        ''' <returns>CIEXYZ Object / CIEXYZ 对象</returns>
        Public Function ExtractFromXYZNoCheck(color As String) As XYZ
            Dim xyz() As Double = ExtractFromStringAsDouble(color)
            If xyz.Length <> 3 Then Return Nothing
            '返回对象
            Return New XYZ(xyz(0), xyz(1), xyz(2), False)
        End Function

        ''' <summary>
        ''' Parse out the CIE-Lab value from a string
        ''' 从一个字符串解析出 CIE-Lab 值
        ''' </summary>
        ''' <param name="color">String of CIE-Lab / CIE-Lab 字符串</param>
        ''' <returns>CIELab Object / CIELab 对象</returns>
        Public Function ExtractFromLab(color As String) As Lab
            Dim lab() As Integer = ExtractFromString(color)
            '检查格式
            If lab.Length <> 3 Then Return Nothing
            '检查范围
            Dim L, a, b As Integer
            L = lab(0)
            a = lab(1)
            b = lab(2)
            If L < 0 Or L > 100 Or a < -128 Or a > 127 Or b < -128 Or b > 127 Then
                Return Nothing
            End If
            '返回对象
            Return New Lab(L, a, b)
        End Function

    End Module

    '*
    ' Interfaces Of Color Models
    ' 颜色模型的接口
    '*
    Namespace Interfaces

        Interface IToRGB

            ''' <summary>
            ''' Convert to RGB
            ''' 转换为 RGB
            ''' </summary>
            ''' <returns></returns>
            Function ToRGB() As RGB

        End Interface

        Interface IToHSB

            ''' <summary>
            ''' Convert to HSB
            ''' 转换为 HSB
            ''' </summary>
            ''' <returns></returns>
            Function ToHSB() As HSB

        End Interface

        Interface IToHSL

            ''' <summary>
            ''' Convert to HSL
            ''' 转换为 HSL
            ''' </summary>
            ''' <returns></returns>
            Function ToHSL() As HSL

        End Interface

        Interface IToCMYK

            ''' <summary>
            ''' Convert to CMYK
            ''' 转换为 CMYK
            ''' </summary>
            ''' <returns></returns>
            Function ToCMYK() As CMYK

        End Interface

        Interface IToYCrCb

            ''' <summary>
            ''' Convert to YCrCb
            ''' 转换为 YCrCb
            ''' </summary>
            ''' <returns></returns>
            Function ToYCrCb() As YCrCb

        End Interface

        Interface IToXYZ

            ''' <summary>
            ''' Convert to CIE-XYZ Space
            ''' 转换到 CIE-XYZ 空间
            ''' </summary>
            ''' <returns></returns>
            Function ToXYZ() As XYZ

        End Interface

        Interface IToLab

            ''' <summary>
            ''' Convert to CIE-Lab
            ''' 转换为 CIE-Lab
            ''' </summary>
            ''' <returns></returns>
            Function ToLab() As Lab

        End Interface

        Interface IToString

            ''' <summary>
            ''' Make a string of Color Model
            ''' 生成表示颜色模型的字符串
            ''' </summary>
            ''' <param name="separator"></param>
            ''' <returns></returns>
            Function ToString(separator As String) As String

        End Interface

    End Namespace

    '*
    ' Class of Color Model Data
    ' 颜色模型数据的类
    '*

    ''' <summary>
    ''' Class of RGB Color Model
    ''' 表示 RGB 颜色模型的类
    ''' </summary>
    Public Class RGB
        Implements IToHSB, IToHSL, IToCMYK, IToYCrCb, IToXYZ, IToLab, IToString
        Private cR, cG, cB As Integer

        ''' <summary>
        ''' Red (0 - 255)
        ''' </summary>
        ''' <returns></returns>
        Public Property R As Integer
            Get
                Return cR
            End Get
            Set(value As Integer)
                FixRange(value, 0, 255)
                cR = value
            End Set
        End Property

        ''' <summary>
        ''' Green (0 - 255)
        ''' </summary>
        ''' <returns></returns>
        Public Property G As Integer
            Get
                Return cG
            End Get
            Set(value As Integer)
                FixRange(value, 0, 255)
                cG = value
            End Set
        End Property

        ''' <summary>
        ''' Blue (0 - 255)
        ''' </summary>
        ''' <returns></returns>
        Public Property B As Integer
            Get
                Return cB
            End Get
            Set(value As Integer)
                FixRange(value, 0, 255)
                cB = value
            End Set
        End Property

        ''' <summary>
        ''' Initialize the RGB object
        ''' 初始化 RGB 对象
        ''' </summary>
        ''' <param name="r">Red (0 - 255)</param>
        ''' <param name="g">Green (0 - 255)</param>
        ''' <param name="b">Blue (0 - 255)</param>
        Public Sub New(ByVal r As Integer, ByVal g As Integer, ByVal b As Integer)
            Me.R = r
            Me.G = g
            Me.B = b
        End Sub

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj IsNot RGB Then Return False
            Dim other As RGB = CType(obj, RGB)
            Return cR = other.cR AndAlso cG = other.cG AndAlso cB = other.cB
        End Function

        Public Shared Operator =(left As RGB, right As RGB) As Boolean
            Return left.cR = right.cR AndAlso left.cG = right.cG AndAlso left.cB = right.cB
        End Operator

        Public Shared Operator <>(left As RGB, right As RGB) As Boolean
            Return left.cR <> right.cR OrElse left.cG <> right.cG OrElse left.cB <> right.cB
        End Operator

        Public Overrides Function ToString() As String
            Return String.Format("RGB: ({0},{1},{2})", R, G, B)
        End Function

        Public Overloads Function ToString(separator As String) As String _
            Implements IToString.ToString
            Return String.Format("{0}{3}{1}{3}{2}", R, G, B, separator)
        End Function

        Public Function CompareWith(r As Integer, g As Integer, b As Integer) As Boolean
            Return cR = r AndAlso cG = g AndAlso cB = b
        End Function

        Public Function ToHSB() As HSB Implements IToHSB.ToHSB
            Dim hsb() As Integer = RawRGBToHSB.Calc(cR, cG, cB)
            Return New HSB(hsb(0), hsb(1), hsb(2))
        End Function

        Public Function ToHSL() As HSL Implements IToHSL.ToHSL
            Dim hsl() As Integer = RawRGBToHSL.Calc(cR, cG, cB)
            Return New HSL(hsl(0), hsl(1), hsl(2))
        End Function

        Public Function ToCMYK() As CMYK Implements IToCMYK.ToCMYK
            Dim cmyk() As Integer = RawRGBToCMYK.Calc(cR, cG, cB)
            Return New CMYK(cmyk(0), cmyk(1), cmyk(2), cmyk(3))
        End Function

        Public Function ToYCrCb() As YCrCb Implements IToYCrCb.ToYCrCb
            Dim ycrcb() As Integer = RawRGBToYCrCb.Calc(cR, cG, cB)
            Return New YCrCb(ycrcb(0), ycrcb(1), ycrcb(2))
        End Function

        Public Function ToXYZ() As XYZ Implements IToXYZ.ToXYZ
            Dim xyz() As Double = RawRGBToXYZ.Calc(cR, cG, cB)
            'CIEXYZ 对象的属性用法上和其他颜色模型对象有区别
            Return New XYZ(xyz(0), xyz(1), xyz(2))
        End Function

        Public Function ToLab() As Lab Implements IToLab.ToLab
            Dim xyz() As Double = RawRGBToXYZ.Calc(cR, cG, cB)
            Dim lab() As Double = RawXYZToLab.Calc(xyz(0), xyz(1), xyz(2))
            Return New Lab(lab(0), lab(1), lab(2))
        End Function

    End Class

    ''' <summary>
    ''' Class of HSB / HSV Color Model
    ''' 表示 HSB / HSV 颜色模型的类
    ''' </summary>
    Public Class HSB
        Implements IToRGB, IToHSL, IToString
        Private cH, cS, cB As Integer

        ''' <summary>
        ''' Hue (0 - 360)
        ''' </summary>
        ''' <returns></returns>
        Public Property H As Integer
            Get
                Return cH
            End Get
            Set(value As Integer)
                FixRange(value, 0, 360)
                cH = value
            End Set
        End Property

        ''' <summary>
        ''' Saturation (0 - 100)
        ''' </summary>
        ''' <returns></returns>
        Public Property S As Integer
            Get
                Return cS
            End Get
            Set(value As Integer)
                FixRange(value, 0, 100)
                cS = value
            End Set
        End Property

        ''' <summary>
        ''' Brightness / Value (0 - 100)
        ''' </summary>
        ''' <returns></returns>
        Public Property B As Integer
            Get
                Return cB
            End Get
            Set(value As Integer)
                FixRange(value, 0, 100)
                cB = value
            End Set
        End Property

        ''' <summary>
        ''' Initialize the HSB / HSV object
        ''' 初始化 HSB / HSV 对象
        ''' </summary>
        ''' <param name="h">Hue (0 - 360)</param>
        ''' <param name="s">Saturation (0 - 100)</param>
        ''' <param name="b">Brightness / Value (0 - 100)</param>
        Public Sub New(ByVal h As Integer, ByVal s As Integer, ByVal b As Integer)
            Me.H = h
            Me.S = s
            Me.B = b
        End Sub

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj IsNot HSB Then Return False
            Dim other As HSB = CType(obj, HSB)
            Return cH = other.cH AndAlso cS = other.cS AndAlso cB = other.cB
        End Function

        Public Overloads Function ToString(separator As String) As String _
            Implements IToString.ToString
            Return String.Format("{0}{3}{1}{3}{2}", H, S, B, separator)
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("HSB: ({0},{1},{2})", H, S, B)
        End Function

        Public Shared Operator =(left As HSB, right As HSB) As Boolean
            Return left.cH = right.cH AndAlso left.cS = right.cS AndAlso left.cB = right.cB
        End Operator

        Public Shared Operator <>(left As HSB, right As HSB) As Boolean
            Return left.cH <> right.cH OrElse left.cS <> right.cS OrElse left.cB <> right.cB
        End Operator

        Public Function CompareWith(h As Integer, s As Integer, b As Integer) As Boolean
            Return cH = h AndAlso cS = s AndAlso cB = b
        End Function

        Public Function ToRGB() As RGB Implements IToRGB.ToRGB
            Dim rgb() As Integer = RawHSBToRGB.Calc(cH, cS, cB)
            Return New RGB(rgb(0), rgb(1), rgb(2))
        End Function

        Public Function ToHSL() As HSL Implements IToHSL.ToHSL
            Dim hsl() As Integer
            hsl = RawHSBToHSL.Calc(cH, cS, cB)
            Return New HSL(hsl(0), hsl(1), hsl(2))
        End Function

    End Class

    ''' <summary>
    ''' Class of HSL Color Model
    ''' 表示 HSL 颜色模型的类
    ''' </summary>
    Public Class HSL
        Implements IToRGB, IToHSB, IToString
        Private cH, cS, cL As Integer

        ''' <summary>
        ''' Hue (0 - 360)
        ''' </summary>
        ''' <returns></returns>
        Public Property H As Integer
            Get
                Return cH
            End Get
            Set(value As Integer)
                FixRange(value, 0, 360)
                cH = value
            End Set
        End Property

        ''' <summary>
        ''' Saturation (0 - 100)
        ''' </summary>
        ''' <returns></returns>
        Public Property S As Integer
            Get
                Return cS
            End Get
            Set(value As Integer)
                FixRange(value, 0, 100)
                cS = value
            End Set
        End Property

        ''' <summary>
        ''' Lightness (0 - 100)
        ''' </summary>
        ''' <returns></returns>
        Public Property L As Integer
            Get
                Return cL
            End Get
            Set(value As Integer)
                FixRange(value, 0, 100)
                cL = value
            End Set
        End Property

        ''' <summary>
        ''' Initialize the HSL object
        ''' 初始化 HSL 对象
        ''' </summary>
        ''' <param name="h">Hue (0 - 360)</param>
        ''' <param name="s">Saturation (0 - 100)</param>
        ''' <param name="l">Lightness (0 - 100)</param>
        Public Sub New(ByVal h As Integer, ByVal s As Integer, ByVal l As Integer)
            Me.H = h
            Me.S = s
            Me.L = l
        End Sub

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj IsNot HSL Then Return False
            Dim other As HSL = CType(obj, HSL)
            Return cH = other.cH AndAlso cS = other.cS AndAlso cL = other.cL
        End Function

        Public Shared Operator =(left As HSL, right As HSL) As Boolean
            Return left.cH = right.cH AndAlso left.cS = right.cS AndAlso left.cL = right.cL
        End Operator

        Public Shared Operator <>(left As HSL, right As HSL) As Boolean
            Return left.cH <> right.cH OrElse left.cS <> right.cS OrElse left.cL <> right.cL
        End Operator

        Public Overrides Function ToString() As String
            Return String.Format("HSL: ({0},{1},{2})", H, S, L)
        End Function

        Public Overloads Function ToString(separator As String) As String _
            Implements IToString.ToString
            Return String.Format("{0}{3}{1}{3}{2}", H, S, L, separator)
        End Function

        Public Function CompareWith(h As Integer, s As Integer, l As Integer) As Boolean
            Return cH = h AndAlso cS = s AndAlso cL = l
        End Function

        Public Function ToRGB() As RGB Implements IToRGB.ToRGB
            Dim rgb() As Integer = RawHSLToRGB.Calc(cH, cS, cL)
            Return New RGB(rgb(0), rgb(1), rgb(2))
        End Function

        Public Function ToHSB() As HSB Implements IToHSB.ToHSB
            Dim hsb() As Integer
            hsb = RawHSLToHSB.Calc(cH, cS, cL)
            Return New HSB(hsb(0), hsb(1), hsb(2))
        End Function

    End Class

    ''' <summary>
    ''' Class of CMYK Color Model
    ''' 表示 CMYK 颜色模型的类
    ''' </summary>
    Public Class CMYK
        Implements IToRGB, IToString
        Private cC, cM, cY, cK As Integer

        ''' <summary>
        ''' Cyan (0 - 100)
        ''' </summary>
        ''' <returns></returns>
        Public Property C As Integer
            Get
                Return cC
            End Get
            Set(value As Integer)
                FixRange(value, 0, 100)
                cC = value
            End Set
        End Property

        ''' <summary>
        ''' Magenta (0 - 100)
        ''' </summary>
        ''' <returns></returns>
        Public Property M As Integer
            Get
                Return cM
            End Get
            Set(value As Integer)
                FixRange(value, 0, 100)
                cM = value
            End Set
        End Property

        ''' <summary>
        ''' Yellow (0 - 100)
        ''' </summary>
        ''' <returns></returns>
        Public Property Y As Integer
            Get
                Return cY
            End Get
            Set(value As Integer)
                FixRange(value, 0, 100)
                cY = value
            End Set
        End Property

        ''' <summary>
        ''' Key / Black (0 - 100)
        ''' </summary>
        ''' <returns></returns>
        Public Property K As Integer
            Get
                Return cK
            End Get
            Set(value As Integer)
                FixRange(value, 0, 100)
                cK = value
            End Set
        End Property

        ''' <summary>
        ''' Initialize the CMYK object
        ''' 初始化 CYMK 对象
        ''' </summary>
        ''' <param name="c">Cyan (0 - 100)</param>
        ''' <param name="m">Magenta (0 - 100)</param>
        ''' <param name="y">Yellow (0 - 100)</param>
        ''' <param name="k">Key / Black (0 - 100)</param>
        Public Sub New(ByVal c As Integer, ByVal m As Integer, ByVal y As Integer, ByVal k As Integer)
            Me.C = c
            Me.M = m
            Me.Y = y
            Me.K = k
        End Sub

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj IsNot CMYK Then Return False
            Dim other As CMYK = CType(obj, CMYK)
            Return cM = other.cM AndAlso cM = other.cM AndAlso cY = other.cY AndAlso cK = other.cK
        End Function

        Public Shared Operator =(left As CMYK, right As CMYK) As Boolean
            Return left.cM = right.cM AndAlso left.cM = right.cM AndAlso left.cY = right.cY AndAlso left.cK = right.cK
        End Operator

        Public Shared Operator <>(left As CMYK, right As CMYK) As Boolean
            Return left.cM <> right.cM OrElse left.cM <> right.cM OrElse left.cY <> right.cY OrElse left.cK <> right.cK
        End Operator

        Public Overrides Function ToString() As String
            Return String.Format("CMYK: ({0},{1},{2},{3})", C, M, Y, K)
        End Function

        Public Overloads Function ToString(separator As String) As String _
            Implements IToString.ToString
            Return String.Format("{0}{4}{1}{4}{2}{4}{3}", C, M, Y, K, separator)
        End Function

        Public Function CompareWith(c As Integer, m As Integer, y As Integer, k As Integer) As Boolean
            Return cC = c AndAlso cM = m AndAlso cY = y AndAlso cK = k
        End Function

        Public Function ToRGB() As RGB Implements IToRGB.ToRGB
            Dim rgb() As Integer = RawCMYKToRGB.Calc(cC, cM, cY, cK)
            Return New RGB(rgb(0), rgb(1), rgb(2))
        End Function

    End Class

    ''' <summary>
    ''' Class of YCrCb Color Model
    ''' 表示 YCrCb 颜色模型的类
    ''' </summary>
    Public Class YCrCb
        Implements IToRGB, IToString
        Private cY, cCr, cCb As Integer

        ''' <summary>
        ''' Y (0 - 255)
        ''' </summary>
        ''' <returns></returns>
        Public Property Y As Integer
            Get
                Return cY
            End Get
            Set(value As Integer)
                FixRange(value, 0, 255)
                cY = value
            End Set
        End Property

        ''' <summary>
        ''' Cr (0 - 255)
        ''' </summary>
        ''' <returns></returns>
        Public Property Cr As Integer
            Get
                Return cCr
            End Get
            Set(value As Integer)
                FixRange(value, 0, 255)
                cCr = value
            End Set
        End Property

        ''' <summary>
        ''' Cb (0 - 255)
        ''' </summary>
        ''' <returns></returns>
        Public Property Cb As Integer
            Get
                Return cCb
            End Get
            Set(value As Integer)
                FixRange(value, 0, 255)
                cCb = value
            End Set
        End Property

        ''' <summary>
        ''' Initialize the YCrCb object
        ''' 初始化 YCrCb 对象
        ''' </summary>
        ''' <param name="y">Y (0 - 255)</param>
        ''' <param name="cr">Cr (0 - 255)</param>
        ''' <param name="cb">Cb (0 - 255)</param>
        Public Sub New(ByVal y As Integer, ByVal cr As Integer, ByVal cb As Integer)
            Me.Y = y
            Me.Cr = cr
            Me.Cb = cb
        End Sub

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj IsNot YCrCb Then Return False
            Dim other As YCrCb = CType(obj, YCrCb)
            Return cY = other.cY AndAlso cCr = other.cCr AndAlso cCb = other.cCb
        End Function

        Public Shared Operator =(left As YCrCb, right As YCrCb) As Boolean
            Return left.cY = right.cY AndAlso left.cCr = right.cCr AndAlso left.cCb = right.cCb
        End Operator

        Public Shared Operator <>(left As YCrCb, right As YCrCb) As Boolean
            Return left.cY <> right.cY OrElse left.cCr <> right.cCr OrElse left.cCb <> right.cCb
        End Operator

        Public Overrides Function ToString() As String
            Return String.Format("YCrCb: ({0},{1},{2})", Y, Cr, Cb)
        End Function

        Public Overloads Function ToString(separator As String) As String _
            Implements IToString.ToString
            Return String.Format("{0}{3}{1}{3}{2}", Y, Cr, Cb, separator)
        End Function

        Public Function CompareWith(y As Integer, cr As Integer, cb As Integer) As Boolean
            Return cY = y AndAlso cCr = cr AndAlso cCb = cb
        End Function

        Public Function ToRGB() As RGB Implements IToRGB.ToRGB
            Dim rgb() As Integer = RawYCrCbToRGB.Calc(cY, cCr, cCb)
            Return New RGB(rgb(0), rgb(1), rgb(2))
        End Function

    End Class

    ''' <summary>
    ''' Class to store the values of X, Y, Z in CIE-XYZ Color Space
    ''' 用来存储 CIE-XYZ 颜色空间 X, Y, Z 值的类
    ''' </summary>
    Public Class XYZ
        Implements IToRGB, IToLab, IToString

        'Range: ScaledX / ScaledY / ScaledZ have the following range limitation
        '       to accommodate color models convertion:
        '       ScaledX: 0 to 95.047
        '       ScaledY: 0 to 100.000
        '       ScaledZ: 0 to 108.883
        '       X / Y / Z have no range limitation
        Private cX, cY, cZ As Double

        ''' <summary>
        ''' Original value of X (No strict range limits, usually is 0 - 1.0)
        ''' X 的原始值(无严格范围限制, 通常取 0 - 1.0)
        ''' </summary>
        ''' <returns></returns>
        Public Property X As Double
            Get
                Return cX
            End Get
            Set(value As Double)
                cX = value
            End Set
        End Property

        ''' <summary>
        ''' Original value of Y (No strict range limits, usually is 0 - 1.0)
        ''' Y 的原始值(无严格范围限制, 通常取 0 - 1.0)
        ''' </summary>
        ''' <returns></returns>
        Public Property Y As Double
            Get
                Return cY
            End Get
            Set(value As Double)
                cY = value
            End Set
        End Property

        ''' <summary>
        ''' Original value of Z (No strict range limits, usually is 0 - 1.0)
        ''' Z 的原始值(无严格范围限制, 通常取 0 - 1.0)
        ''' </summary>
        ''' <returns></returns>
        Public Property Z As Double
            Get
                Return cZ
            End Get
            Set(value As Double)
                cZ = value
            End Set
        End Property

        ''' <summary>
        ''' 100 times scaled value of X (Range is 0 - 95.047)
        ''' 100倍放大后的 X 值(范围 0 - 95.047)
        ''' </summary>
        ''' <returns></returns>
        Public Property ScaledX As Double
            Get
                Return cX * 100
            End Get
            Set(value As Double)
                LimitedX = value / 100
            End Set
        End Property

        ''' <summary>
        ''' 100 times scaled value of Y (Range is 0 - 100.0)
        ''' 100倍放大后的 Y 值(范围 0 - 100.0)
        ''' </summary>
        ''' <returns></returns>
        Public Property ScaledY As Double
            Get
                Return cY * 100
            End Get
            Set(value As Double)
                LimitedY = value / 100
            End Set
        End Property

        ''' <summary>
        ''' 100 times scaled value of Z (Range is 0 - 108.883)
        ''' 100倍放大后的 Z 值(范围 0 - 108.883)
        ''' </summary>
        ''' <returns></returns>
        Public Property ScaledZ As Double
            Get
                Return cZ * 100
            End Get
            Set(value As Double)
                LimitedZ = value / 100
            End Set
        End Property

        ''' <summary>
        ''' X with limited range
        ''' </summary>
        Protected WriteOnly Property LimitedX() As Double
            Set(value As Double)
                FixRange(value, 0, 0.95047)
                cX = value
            End Set
        End Property

        ''' <summary>
        ''' Y with limited range
        ''' </summary>
        Protected WriteOnly Property LimitedY() As Double
            Set(value As Double)
                FixRange(value, 0, 1)
                cY = value
            End Set
        End Property

        ''' <summary>
        ''' Z with limited range
        ''' </summary>
        Protected WriteOnly Property LimitedZ() As Double
            Set(value As Double)
                FixRange(value, 0, 1.08883)
                cZ = value
            End Set
        End Property

        Protected Sub New()

        End Sub

        ''' <summary>
        ''' Initialize the XYZ object (range limited)
        ''' 初始化 XYZ 对象(有范围限制)
        ''' </summary>
        ''' <param name="x">X (0 - 0.95047)</param>
        ''' <param name="y">Y (0 - 1.000)</param>
        ''' <param name="z">Z (0 - 1.08883)</param>
        Public Sub New(ByVal x As Double, ByVal y As Double, ByVal z As Double)
            Me.LimitedX = x
            Me.LimitedY = y
            Me.LimitedZ = z
        End Sub

        ''' <summary>
        ''' Initialize the XYZ object with specified options
        ''' 按照指定的选项初始化 XYZ 对象
        ''' </summary>
        ''' <param name="X">X (0 - 0.95047 if check range)</param>
        ''' <param name="Y">Y (0 - 1.0 if check range)</param>
        ''' <param name="Z">Z (0 - 1.08883 if check range)</param>
        ''' <param name="limited">Check range limit or not 是否检查范围</param>
        Public Sub New(x As Double, y As Double, z As Double,
                       limited As Boolean)
            If limited Then
                Me.LimitedX = x
                Me.LimitedY = y
                Me.LimitedZ = z
            Else
                Me.X = x
                Me.Y = y
                Me.Z = z
            End If
        End Sub

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj IsNot XYZ Then Return False
            Dim other As XYZ = CType(obj, XYZ)
            Return DecimalEquals(cX, other.cX) AndAlso DecimalEquals(cY, other.cY) AndAlso DecimalEquals(cZ, other.cZ)
        End Function

        Public Shared Operator =(left As XYZ, right As XYZ) As Boolean
            Return DecimalEquals(left.cX, right.cX) AndAlso DecimalEquals(left.cY, right.cY) AndAlso DecimalEquals(left.cZ, right.cZ)
        End Operator

        Public Shared Operator <>(left As XYZ, right As XYZ) As Boolean
            Return Not DecimalEquals(left.cX, right.cX) OrElse Not DecimalEquals(left.cY, right.cY) OrElse Not DecimalEquals(left.cZ, right.cZ)
        End Operator

        Public Overrides Function ToString() As String
            Return String.Format("CIE-XYZ: ({0},{1},{2})", X, Y, Z)
        End Function

        Public Overloads Function ToString(separator As String) As String _
            Implements IToString.ToString
            Return String.Format("{0}{3}{1}{3}{2}", X, Y, Z, separator)
        End Function

        Public Function CompareWith(x As Double, y As Double, z As Double) As Boolean
            Return DecimalEquals(cX, x) AndAlso DecimalEquals(cY, y) AndAlso DecimalEquals(cZ, z)
        End Function

        Public Function CompareWithScaled(scaledX As Double, scaledY As Double, scaledZ As Double) As Boolean
            Return DecimalEquals(Me.ScaledX, scaledX) AndAlso DecimalEquals(Me.ScaledY, scaledY) AndAlso DecimalEquals(Me.ScaledZ, scaledZ)
        End Function

        Public Function ToRGB() As RGB Implements IToRGB.ToRGB
            Dim rgb() As Integer = RawXYZToRGB.Calc(cX, cY, cZ)
            Return New RGB(rgb(0), rgb(1), rgb(2))
        End Function

        Public Function ToLab() As Lab Implements IToLab.ToLab
            Dim lab() As Double = RawXYZToLab.Calc(cX, cY, cZ)
            Return New Lab(FRound(lab(0)), FRound(lab(1)), FRound(lab(2)))
        End Function

    End Class

    ''' <summary>
    ''' Class of CIE-Lab Color Model
    ''' 表示 CIE-Lab 颜色模型的类
    ''' </summary>
    Public Class Lab
        Implements IToXYZ, IToRGB, IToString

        'Range: L: 0 to 100
        '       a: -128 to 127
        '       b: -128 to 127
        Private cL, cA, cB As Integer

        ''' <summary>
        ''' L (0 - 100)
        ''' </summary>
        ''' <returns></returns>
        Public Property L As Integer
            Get
                Return cL
            End Get
            Set(value As Integer)
                FixRange(value, 0, 100)
                cL = value
            End Set
        End Property

        ''' <summary>
        ''' a (-128 - 127)
        ''' </summary>
        ''' <returns></returns>
        Public Property A As Integer
            Get
                Return cA
            End Get
            Set(value As Integer)
                FixRange(value, -128, 127)
                cA = value
            End Set
        End Property

        ''' <summary>
        ''' b (-128 - 127)
        ''' </summary>
        ''' <returns></returns>
        Public Property B As Integer
            Get
                Return cB
            End Get
            Set(value As Integer)
                FixRange(value, -128, 127)
                cB = value
            End Set
        End Property

        ''' <summary>
        ''' Initialize the CIELab object
        ''' 初始化 CIELab 对象
        ''' </summary>
        ''' <param name="l">L (0 - 100)</param>
        ''' <param name="a">a (-128 - 127)</param>
        ''' <param name="b">b (-128 - 127)</param>
        Public Sub New(ByVal l As Integer, ByVal a As Integer, ByVal b As Integer)
            Me.L = l
            Me.A = a
            Me.B = b
        End Sub

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj IsNot Lab Then Return False
            Dim other As Lab = CType(obj, Lab)
            Return cL = other.cL AndAlso cA = other.cA AndAlso cB = other.cB
        End Function

        Public Shared Operator =(left As Lab, right As Lab) As Boolean
            Return left.cL = right.cL AndAlso left.cA = right.cA AndAlso left.cB = right.cB
        End Operator

        Public Shared Operator <>(left As Lab, right As Lab) As Boolean
            Return left.cL <> right.cL OrElse left.cA <> right.cA OrElse left.cB <> right.cB
        End Operator

        Public Overrides Function ToString() As String
            Return String.Format("CIE-Lab: ({0},{1},{2})", L, A, B)
        End Function

        Public Overloads Function ToString(separator As String) As String _
            Implements IToString.ToString
            Return String.Format("{0}{3}{1}{3}{2}", L, A, B, separator)
        End Function

        Public Function CompareWith(l As Integer, a As Integer, b As Integer) As Boolean
            Return cL = l AndAlso cA = a AndAlso cB = b
        End Function

        Public Function ToXYZ() As XYZ Implements IToXYZ.ToXYZ
            Dim xyz() As Double = RawLabToXYZ.Calc(cL, cA, cB)
            Return New XYZ(xyz(0), xyz(1), xyz(2))
        End Function

        Public Function ToRGB() As RGB Implements IToRGB.ToRGB
            Dim xyz() As Double = RawLabToXYZ.Calc(cL, cA, cB)
            Dim rgb() As Integer = RawXYZToRGB.Calc(xyz(0), xyz(1), xyz(2))
            Return New RGB(rgb(0), rgb(1), rgb(2))
        End Function

    End Class

End Namespace

Namespace CPExtend

    Namespace DotNet
        ''' <summary>
        ''' 提供转换到 .Net 的 Color 结构相关函数
        ''' </summary>
        Module ColorConvertor
            Public Function ToColor(rgb As RGB) As Color
                Return Color.FromArgb(255, rgb.R, rgb.G, rgb.B)
            End Function

            Public Function ToColor(toRGB As CPModel.Interfaces.IToRGB) As Color
                Dim rgb As RGB = toRGB.ToRGB
                Return Color.FromArgb(255, rgb.R, rgb.G, rgb.B)
            End Function
        End Module
    End Namespace

    Public Interface IToColor

        ''' <summary>
        ''' 转换到 VisualBasic.Net 的 Color 结构
        ''' </summary>
        ''' <returns></returns>
        Function ToColor() As Color
    End Interface

    ''' <summary>
    ''' 提供各个颜色模型相互转换的扩展类
    ''' </summary>
    Public Class ConvertBridge
        Implements IToColor
        Protected _rgb As RGB
        Protected _hsb As HSB
        Protected _hsl As HSL
        Protected _cmyk As CMYK
        Protected _ycrcb As YCrCb
        Protected _xyz As XYZ
        Protected _lab As Lab

        Protected Sub New()

        End Sub

        Public Sub New(colorModel As Object)
            DoConvert(colorModel)
            '自动补全剩余的颜色模型
            If _hsb Is Nothing Then _hsb = _rgb.ToHSB
            If _hsl Is Nothing Then _hsl = _rgb.ToHSL
            If _cmyk Is Nothing Then _cmyk = _rgb.ToCMYK
            If _ycrcb Is Nothing Then _ycrcb = _rgb.ToYCrCb
            If _xyz Is Nothing Then _xyz = _rgb.ToXYZ
            If _lab Is Nothing Then _lab = _xyz.ToLab
        End Sub

        Protected Overridable Sub DoConvert(ByRef cm As Object)
            '判断并确定转换的来源，手动转换出需要的模型
            Select Case cm.GetType
                Case GetType(RGB)
                    _rgb = cm
                Case GetType(HSB)
                    _hsb = cm
                    _rgb = _hsb.ToRGB
                Case GetType(HSL)
                    _hsl = cm
                    _rgb = _hsl.ToRGB
                Case GetType(CMYK)
                    _cmyk = cm
                    _rgb = _cmyk.ToRGB
                Case GetType(YCrCb)
                    _ycrcb = cm
                    _rgb = _ycrcb.ToRGB
                Case GetType(Lab)
                    _lab = cm
                    _xyz = _lab.ToXYZ
                    _rgb = _xyz.ToRGB
                Case GetType(XYZ)
                    _xyz = cm
                    _rgb = _xyz.ToRGB
                    _lab = _xyz.ToLab
                Case GetType(Color)
                    With CType(cm, Color)
                        _rgb = New RGB(.R, .G, .B)
                    End With
                Case Else
                    If TypeOf cm Is ConvertBridge Then
                        With CType(cm, ConvertBridge)
                            _rgb = .RGB
                            _hsb = .HSB
                            _hsl = .HSL
                            _cmyk = .CMYK
                            _ycrcb = .YCrCb
                            _xyz = .XYZ
                            _lab = .Lab
                        End With
                    Else
                        Throw New ArgumentException("Illegal Argument Type")
                    End If
            End Select
        End Sub

        Public Overridable Function ToColor() As Color Implements IToColor.ToColor
            With _rgb
                Return Color.FromArgb(255, .R, .G, .B)
            End With
        End Function

        Public Overridable Function ToHex() As String
            With _rgb
                Return String.Format("{0:X2}{1:X2}{2:X2}", .R, .G, .B)
            End With
        End Function

        Public Overridable ReadOnly Property RGB As RGB
            Get
                Return _rgb
            End Get
        End Property

        Public Overridable ReadOnly Property HSB As HSB
            Get
                Return _hsb
            End Get
        End Property

        Public Overridable ReadOnly Property HSL As HSL
            Get
                Return _hsl
            End Get
        End Property

        Public Overridable ReadOnly Property CMYK As CMYK
            Get
                Return _cmyk
            End Get
        End Property

        Public Overridable ReadOnly Property YCrCb As YCrCb
            Get
                Return _ycrcb
            End Get
        End Property

        Public Overridable ReadOnly Property XYZ As XYZ
            Get
                Return _xyz
            End Get
        End Property

        Public Overridable ReadOnly Property Lab As Lab
            Get
                Return _lab
            End Get
        End Property

        Public Shared Narrowing Operator CType(obj As ConvertBridge) As RGB
            Return obj.RGB
        End Operator

        Public Shared Narrowing Operator CType(obj As ConvertBridge) As Color
            Return obj.ToColor()
        End Operator

        Public Shared Function FromRGB(r As Integer, g As Integer, b As Integer) As ConvertBridge
            Return New ConvertBridge(New RGB(r, g, b))
        End Function

        Public Shared Function FromColor(ByRef color As Color) As ConvertBridge
            Return New ConvertBridge(color)
        End Function
    End Class

    ''' <summary>
    ''' 提供各个颜色模型互相转换的扩展类，带懒加载
    ''' </summary>
    Public Class ConvertBridgeLazy
        Inherits ConvertBridge


        Public Sub New(colorModel As Object)
            MyBase.DoConvert(colorModel)
        End Sub


        Public Overrides ReadOnly Property HSB As HSB
            Get
                If _hsb Is Nothing Then _hsb = _rgb.ToHSB
                Return _hsb
            End Get
        End Property

        Public Overrides ReadOnly Property HSL As HSL
            Get
                If _hsl Is Nothing Then _hsl = _rgb.ToHSL
                Return _hsl
            End Get
        End Property

        Public Overrides ReadOnly Property CMYK As CMYK
            Get
                If _cmyk Is Nothing Then _cmyk = _rgb.ToCMYK
                Return _cmyk
            End Get
        End Property

        Public Overrides ReadOnly Property YCrCb As YCrCb
            Get
                If _ycrcb Is Nothing Then _ycrcb = _rgb.ToYCrCb
                Return _ycrcb
            End Get
        End Property

        Public Overrides ReadOnly Property XYZ As XYZ
            Get
                If _xyz Is Nothing Then _xyz = _rgb.ToXYZ
                Return _xyz
            End Get
        End Property

        Public Overrides ReadOnly Property Lab As Lab
            Get
                If _lab Is Nothing Then
                    If _xyz Is Nothing Then _xyz = _rgb.ToXYZ
                    _lab = _xyz.ToLab
                End If
                Return _lab
            End Get
        End Property

        Public Shared Shadows Function FromRGB(r As Integer, g As Integer, b As Integer) As ConvertBridgeLazy
            Return New ConvertBridgeLazy(New RGB(r, g, b))
        End Function

        Public Shared Shadows Function FromColor(ByRef color As Color) As ConvertBridgeLazy
            Return New ConvertBridgeLazy(color)
        End Function
    End Class

End Namespace
