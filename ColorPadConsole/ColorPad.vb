Imports ColorPadCore.Core.Formula
Imports ColorPadCore.Core.Model
Imports ColorPadCore.Extend

Module ColorPad

    Sub Main(args As String())
        Console.Title = "ColorPad Console V" + Reflection.Assembly.GetEntryAssembly.GetName.Version.ToString
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
                Continue While
            End If
            Dim func As Func(Of String, ConvertBridge) = GetConvertBridge(input)
            If func Is Nothing Then
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("Unknown command: " + input)
                Console.ForegroundColor = ConsoleColor.Gray
                Console.WriteLine()
                Continue While
            End If
            Console.Write("Please input value of color(separated by ',' if necessary): ")
            Dim brige As ConvertBridge
            Try
                brige = func(Console.ReadLine().Trim())
            Catch ex As ArgumentNullException
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("Inputted value or format is incorrect")
                Console.ForegroundColor = ConsoleColor.Gray
                Console.WriteLine()
                Continue While
            End Try
            Console.WriteLine()
            Console.WriteLine("====================")
            Console.WriteLine(" Color Information: ")
            Console.ForegroundColor = ConsoleColor.DarkYellow
            Console.WriteLine("    " + brige.Rgb.ToString())
            Console.WriteLine("    Hex(HTML): #" + brige.ToHex())
            Console.WriteLine("    HSB(HSV): " + brige.Hsb.ToString(","))
            Console.WriteLine("    " + brige.Hsl.ToString())
            Console.WriteLine("    " + brige.Cmyk.ToString())
            Console.WriteLine("    " + brige.YCrCb.ToString())
            Console.WriteLine("    " + brige.Lab.ToString())
            Console.WriteLine("    XYZ: ({0:0.000},{1:0.000},{2:0.000})", brige.Xyz.X, brige.Xyz.Y, brige.Xyz.Z)
            Console.ForegroundColor = ConsoleColor.Gray
            Console.WriteLine("--------------------")
            Console.WriteLine(" Color Formula: ")
            'Use lambada to print the results of formula
            Dim lambadaPrint = Sub(hsb) Console.WriteLine("        " + hsb.ToString())
            Console.ForegroundColor = ConsoleColor.DarkYellow
            Console.WriteLine("    Monochromatic: ")
            Array.ForEach(GetFormula(brige.Hsb, FormulaType.Monochromatic), lambadaPrint)
            Console.WriteLine("    Complementary: ")
            Array.ForEach(GetFormula(brige.Hsb, FormulaType.Complementary), lambadaPrint)
            Console.WriteLine("    Split Complementary: ")
            Array.ForEach(GetFormula(brige.Hsb, FormulaType.SplitComplementary), lambadaPrint)
            Console.WriteLine("    Analogous: ")
            Array.ForEach(GetFormula(brige.Hsb, FormulaType.Analogous), lambadaPrint)
            Console.WriteLine("    Tradic: ")
            Array.ForEach(GetFormula(brige.Hsb, FormulaType.Triadic), lambadaPrint)
            Console.WriteLine("    Tetradic: ")
            Array.ForEach(GetFormula(brige.Hsb, FormulaType.Tetradic), lambadaPrint)
            Console.ForegroundColor = ConsoleColor.Gray
            Console.WriteLine("====================")
            Console.WriteLine()
        End While
    End Sub

    Public Function GetConvertBridge(type As String) As Func(Of String, ConvertBridge)
        type = type.ToLower
        Select Case type
            Case "rgb"
                Return Function(s) New NormalConvertBridge(Rgb.FromString(s))
            Case "hex"
                Return Function(s) New NormalConvertBridge(Rgb.FromHexEnhanced(s))
            Case "hsb"
                Return Function(s) New NormalConvertBridge(Hsb.FromString(s))
            Case "hsv"
                Return Function(s) New NormalConvertBridge(Hsl.FromString(s))
            Case "hsl"
                Return Function(s) New NormalConvertBridge(Hsl.FromString(s))
            Case "cmyk"
                Return Function(s) New NormalConvertBridge(Cmyk.FromString(s))
            Case "ycrcb"
                Return Function(s) New NormalConvertBridge(YCrCb.FromString(s))
            Case "lab"
                Return Function(s) New NormalConvertBridge(Lab.FromString(s))
            Case "xyz"
                Return Function(s) New NormalConvertBridge(CieXyzD65.FromString(s))
        End Select
        Return Nothing
    End Function

End Module