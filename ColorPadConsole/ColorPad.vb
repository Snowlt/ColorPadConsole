Imports ColorPadConsole.CPExtend
Imports ColorPadConsole.CPFormula
Imports ColorPadConsole.CPModel

Module ColorPad

    Sub Main()
        Console.Title = "ColorPad Console V1.0"
        Console.WriteLine()
        Console.WriteLine("    ====================    ")
        Console.WriteLine("      ColorPad Console      ")
        Console.WriteLine("    ====================    ")
        Console.WriteLine()
        Console.WriteLine("++++++++++++++++++++++++++++")
        Console.WriteLine()
        While True
            Console.WriteLine("Supported Color Model or Color Space: ")
            Console.WriteLine("    RGB / Hex / HSB / HSV / HSL / ")
            Console.WriteLine("    CMYK / YCrCb / Lab / XYZ ")
            Console.WriteLine("Use 'exit' to quit, 'clear' to clear screen, above to do calculation. ")
            Console.WriteLine()
            Console.Write("Please input Command: ")
            Dim input As String = Console.ReadLine().Trim().ToLower()
            If input = "exit" Then
                Exit While
            ElseIf input = "clear" Then
                Console.Clear()
            Else
                Dim func As Func(Of String, Object) = GetColorModelFunc(input)
                If func Is Nothing Then
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine("Unknown command: " + input)
                    Console.ForegroundColor = ConsoleColor.Gray
                    Console.WriteLine()
                    Continue While
                Else
                    Console.Write("Please input value of color(separated by ',' if necessary): ")
                    Dim model As Object = func(Console.ReadLine().Trim())
                    If model Is Nothing Then
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.WriteLine("Inputted value or format is incorrect")
                        Console.ForegroundColor = ConsoleColor.Gray
                        Console.WriteLine()
                    Else
                        Dim brige As New ConvertBridge(model)
                        Console.WriteLine()
                        Console.WriteLine("====================")
                        Console.WriteLine(" Color Information: ")
                        Console.ForegroundColor = ConsoleColor.DarkYellow
                        Console.WriteLine("    " + brige.RGB.ToString())
                        Console.WriteLine("    Hex(HTML): #" + brige.ToHex())
                        Console.WriteLine("    HSB(HSV): " + brige.HSB.ToString(","))
                        Console.WriteLine("    " + brige.HSL.ToString())
                        Console.WriteLine("    " + brige.CMYK.ToString())
                        Console.WriteLine("    " + brige.YCrCb.ToString())
                        Console.WriteLine("    " + brige.Lab.ToString())
                        Console.WriteLine("    XYZ: ({0:0.000},{1:0.000},{2:0.000})", brige.XYZ.X, brige.XYZ.Y, brige.XYZ.Z)
                        Console.ForegroundColor = ConsoleColor.Gray
                        Console.WriteLine("--------------------")
                        Console.WriteLine(" Color Formula: ")
                        'Use lambada to print the results of formula
                        Dim lambadaPrint = Sub(hsb) Console.WriteLine("        " + hsb.ToString())
                        Console.ForegroundColor = ConsoleColor.DarkYellow
                        Console.WriteLine("    Monochromatic: ")
                        Array.ForEach(GetFormula(brige.HSB, FormulaType.Monochromatic), lambadaPrint)
                        Console.WriteLine("    Complementary: ")
                        Array.ForEach(GetFormula(brige.HSB, FormulaType.Complementary), lambadaPrint)
                        Console.WriteLine("    Split Complementary: ")
                        Array.ForEach(GetFormula(brige.HSB, FormulaType.SplitComplementary), lambadaPrint)
                        Console.WriteLine("    Analogous: ")
                        Array.ForEach(GetFormula(brige.HSB, FormulaType.Analogous), lambadaPrint)
                        Console.WriteLine("    Tradic: ")
                        Array.ForEach(GetFormula(brige.HSB, FormulaType.Tradic), lambadaPrint)
                        Console.WriteLine("    Tetradic: ")
                        Array.ForEach(GetFormula(brige.HSB, FormulaType.Tetradic), lambadaPrint)
                        Console.ForegroundColor = ConsoleColor.Gray
                        Console.WriteLine("====================")
                        Console.WriteLine()
                    End If
                End If
            End If
        End While
    End Sub

    Public Function GetColorModelFunc(keyword As String) As Func(Of String, Object)
        Select Case keyword
            Case "rgb"
                Return AddressOf ExtractFromRGB
            Case "hex"
                Return AddressOf ExtractFromHexEnhanced
            Case "hsb"
                Return AddressOf ExtractFromHSB
            Case "hsv"
                Return AddressOf ExtractFromHSB
            Case "hsl"
                Return AddressOf ExtractFromHSL
            Case "cmyk"
                Return AddressOf ExtractFromCMYK
            Case "ycrcb"
                Return AddressOf ExtractFromYCrCb
            Case "lab"
                Return AddressOf ExtractFromLab
            Case "xyz"
                Return AddressOf ExtractFromXYZ
        End Select
        Return Nothing
    End Function

End Module

''' <summary>
''' Replace System.Drawing.Color of .Net Framework WinForm in the console application
''' 在控制台程序中替代 .Net Framework 窗体程序中的 System.Drawing.Color
''' 没有太多实际的作用
''' </summary>
Public Structure Color
    Public Property R() As Integer
    Public Property G() As Integer
    Public Property B() As Integer
    Public Property A() As Integer

    Public Shared Function FromArgb(alpha As Integer, red As Integer, green As Integer, blue As Integer) As Color
        Return New Color With {
                .A = alpha,
                .R = red,
                .G = green,
                .B = blue
            }
    End Function

    Public Shared Function FromArgb(red As Integer, green As Integer, blue As Integer) As Color
        Return FromArgb(255, red, green, blue)
    End Function

End Structure