Namespace Core

    Namespace Formula
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
            Triadic
            ''' <summary>
            ''' 四角色
            ''' </summary>
            Tetradic
        End Enum

        Public Module Formula
            ''' <summary>
            ''' 以同类色计算色相
            ''' </summary>
            ''' <param name="hue"></param>
            ''' <returns></returns>
            Public Function Monochromatic(hue As Double) As Double()
                Return {hue}
            End Function

            ''' <summary>
            ''' 以互补色计算色相
            ''' </summary>
            ''' <param name="hue"></param>
            ''' <returns></returns>
            Public Function Complementary(hue As Double) As Double()
                Return {hue, (hue + 180) Mod 360}
            End Function

            ''' <summary>
            ''' 以分裂互补色计算色相（夹角一般取 90 - 180）
            ''' </summary>
            ''' <param name="hue"></param>
            ''' <param name="angle"></param>
            ''' <returns></returns>
            Public Function SplitComplementary(hue As Double,
                                               Optional angle As Double = 150) As Double()
                Return {hue, (hue + angle) Mod 360, (hue + 360 - angle) Mod 360}
            End Function

            ''' <summary>
            ''' 以临近色计算色相（夹角一般取 0 - 90）
            ''' </summary>
            ''' <param name="hue"></param>
            ''' <param name="angle"></param>
            ''' <returns></returns>
            Public Function Analogous(hue As Double,
                                      Optional angle As Double = 60) As Double()
                Return {hue, (hue + angle) Mod 360, (hue + 360 - angle) Mod 360}
            End Function

            ''' <summary>
            ''' 以三角色计算色相
            ''' </summary>
            ''' <param name="hue"></param>
            ''' <returns></returns>
            Public Function Triadic(hue As Double) As Double()
                Return {hue, (hue + 120) Mod 360, (hue + 240) Mod 360}
            End Function

            ''' <summary>
            ''' 以四角色计算色相（夹角一般取 0 - 90）
            ''' </summary>
            ''' <param name="hue"></param>
            ''' <param name="angle"></param>
            ''' <returns></returns>
            Public Function Tetradic(hue As Double,
                                     Optional angle As Double = 60) As Double()
                Return {hue, (hue + angle) Mod 360,
                        (hue + 180) Mod 360, (hue + 180 + angle) Mod 360}
            End Function

            ''' <summary>
            ''' 按照指定的方式计算颜色方案
            ''' </summary>
            ''' <param name="hue">色相</param>
            ''' <param name="type">计算方式</param>
            ''' <param name="angle">主色调和第二色调的色相角度差值（仅对部分方案有效）</param>
            ''' <returns></returns>
            Public Function GetFormula(hue As Double,
                                       type As FormulaType,
                                       Optional angle As Double? = Nothing) As Double()
                Select Case type
                    Case FormulaType.Monochromatic
                        Return Monochromatic(hue)
                    Case FormulaType.Complementary
                        Return Complementary(hue)
                    Case FormulaType.SplitComplementary
                        If angle.HasValue Then
                            Return SplitComplementary(hue, Core.GetFixRange(angle.Value, 90, 179))
                        Else
                            Return SplitComplementary(hue)
                        End If
                    Case FormulaType.Analogous
                        If angle.HasValue Then
                            Return Analogous(hue, Core.GetFixRange(angle.Value, 1, 90))
                        Else
                            Return Analogous(hue)
                        End If
                    Case FormulaType.Triadic
                        Return Triadic(hue)
                    Case FormulaType.Tetradic
                        If angle.HasValue Then
                            Return Tetradic(hue, Core.GetFixRange(angle.Value, 1, 90))
                        Else
                            Return Tetradic(hue)
                        End If
                End Select
                Return Array.Empty(Of Double)()
            End Function

            ''' <summary>
            ''' 按照指定的方式计算颜色方案
            ''' </summary>
            ''' <param name="hsb">HSB 对象</param>
            ''' <param name="type">计算方式</param>
            ''' <param name="angle">主色调和第二色调的色相角度差值（仅对部分方案有效）</param>
            ''' <returns></returns>
            Public Function GetFormula(hsb As Core.Model.Hsb,
                                       type As FormulaType,
                                       Optional angle As Double? = Nothing) As Core.Model.Hsb()
                Dim formulas = GetFormula(hsb.H, type, angle)
                Return formulas.Select(Function(h) New Core.Model.Hsb(h, hsb.S, hsb.B)).ToArray()
            End Function
        End Module

    End Namespace
End Namespace