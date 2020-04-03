Imports ColorPadConsole.CPCore
Imports ColorPadConsole.CPModel

Namespace CPCore
    Module Basic
        '*
        ' Basic Functions For Conversion And Color Models
        ' 转换和颜色模型所需的基础函数
        '*

        Public Function FRound(ByRef num As Single) As Integer
            Return Int(num + 0.5F)
        End Function

        Public Function FRound(ByRef num As Double) As Integer
            Return Int(num + 0.5)
        End Function

        Public Function FIntValue(ByRef s As String) As Integer
            Dim num As Integer = 0
            Dim negative As Boolean = Left(s, 1) = "-"
            For Each c In s
                If IsNumeric(c) Then num = num * 10 + Asc(c) - 48
            Next
            Return IIf(negative, -num, num)
        End Function

        Public Sub FixRange(ByRef value As Integer, ByVal lower As Integer, ByVal upper As Integer)
            If value > upper Then
                value = upper
            ElseIf value < lower Then
                value = lower
            End If
        End Sub

        Public Sub FixRange(ByRef value As Double, ByVal lower As Double, ByVal upper As Double)
            If value > upper Then
                value = upper
            ElseIf value < lower Then
                value = lower
            End If
        End Sub

        Public Function DecimalEquals(left As Double, right As Double) As Boolean
            Return Math.Abs(left - right) < 0.00000000001
        End Function
    End Module
End Namespace

Namespace CPFormula
    ''' <summary>
    ''' The Type of Color Formula
    ''' 表示配色方案的类型
    ''' </summary>
    Public Enum FormulaType
        ''' <summary>
        ''' 同类色
        ''' </summary>
        Monochromatic
        ''' <summary>
        ''' 互补色
        ''' </summary>
        Complementary
        ''' <summary>
        ''' 分裂互补色
        ''' </summary>
        SplitComplementary
        ''' <summary>
        ''' 邻近色
        ''' </summary>
        Analogous
        ''' <summary>
        ''' 三角色
        ''' </summary>
        Tradic
        ''' <summary>
        ''' 四角色
        ''' </summary>
        Tetradic
    End Enum

    Public Module ColorFormula
        ''' <summary>
        ''' 以同类色计算色相
        ''' </summary>
        ''' <param name="hue"></param>
        ''' <returns></returns>
        Public Function Monochromatic(hue As Integer) As Integer()
            Return {hue}
        End Function

        ''' <summary>
        ''' 以互补色计算色相
        ''' </summary>
        ''' <param name="hue"></param>
        ''' <returns></returns>
        Public Function Complementary(hue As Integer) As Integer()
            Return {hue, (hue + 180) Mod 360}
        End Function

        ''' <summary>
        ''' 以分裂互补色计算色相（夹角一般取 90 - 180）
        ''' </summary>
        ''' <param name="hue"></param>
        ''' <param name="angel"></param>
        ''' <returns></returns>
        Public Function SplitComplementary(hue As Integer,
                                           Optional angel As Integer = 150) As Integer()
            Return {hue, (hue + angel) Mod 360, (hue + 360 - angel) Mod 360}
        End Function

        ''' <summary>
        ''' 以临近色计算色相（夹角一般取 0 - 90）
        ''' </summary>
        ''' <param name="hue"></param>
        ''' <param name="angel"></param>
        ''' <returns></returns>
        Public Function Analogous(hue As Integer,
                                  Optional angel As Integer = 60) As Integer()
            Return {hue, (hue + angel) Mod 360, (hue + 360 - angel) Mod 360}
        End Function

        ''' <summary>
        ''' 以三角色计算色相
        ''' </summary>
        ''' <param name="hue"></param>
        ''' <returns></returns>
        Public Function Tradic(hue As Integer) As Integer()
            Return {hue, (hue + 120) Mod 360, (hue + 240) Mod 360}
        End Function

        ''' <summary>
        ''' 以四角色计算色相（夹角一般取 0 - 90）
        ''' </summary>
        ''' <param name="hue"></param>
        ''' <param name="angel"></param>
        ''' <returns></returns>
        Public Function Tetradic(hue As Integer,
                                 Optional angel As Integer = 60) As Integer()
            Return {hue, (hue + angel) Mod 360,
                    (hue + 180) Mod 360, (hue + 180 + angel) Mod 360}
        End Function

        ''' <summary>
        ''' 按照指定的方式计算颜色方案
        ''' </summary>
        ''' <param name="hue">色相</param>
        ''' <param name="type">计算方式</param>
        ''' <param name="angel">主色调和第二色调的色相角度差值（仅对部分方案有效）</param>
        ''' <returns></returns>
        Public Function GetFormula(hue As Integer,
                                   type As FormulaType,
                                   Optional angel As Integer = -1) As Integer()
            Select Case type
                Case FormulaType.Monochromatic
                    Return Monochromatic(hue)
                Case FormulaType.Complementary
                    Return Complementary(hue)
                Case FormulaType.SplitComplementary
                    If angel < 0 Then
                        Return SplitComplementary(hue)
                    Else
                        FixRange(angel, 90, 179)
                        Return SplitComplementary(hue, angel)
                    End If
                Case FormulaType.Analogous
                    If angel < 0 Then
                        Return Analogous(hue)
                    Else
                        FixRange(angel, 1, 90)
                        Return Analogous(hue, angel)
                    End If
                Case FormulaType.Tradic
                    Return Tradic(hue)
                Case FormulaType.Tetradic
                    If angel < 0 Then
                        Return Tetradic(hue)
                    Else
                        FixRange(angel, 1, 90)
                        Return Tetradic(hue, angel)
                    End If
            End Select
            Return {}
        End Function

        ''' <summary>
        ''' 按照指定的方式计算颜色方案
        ''' </summary>
        ''' <param name="hsb">HSB 对象</param>
        ''' <param name="type">计算方式</param>
        ''' <param name="angel">主色调和第二色调的色相角度差值（仅对部分方案有效）</param>
        ''' <returns></returns>
        Public Function GetFormula(hsb As HSB,
                                   type As FormulaType,
                                   Optional angel As Integer = -1) As HSB()
            Dim formula As Integer() = GetFormula(hsb.H, type, angel)
            Return formula.Select(Function(h) New HSB(h, hsb.S, hsb.B)).ToArray()
        End Function
    End Module

End Namespace