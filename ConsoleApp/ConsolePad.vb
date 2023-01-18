Imports ColorPadCore.Core
Imports ColorPadCore.Core.Model
Imports ColorPadCore.Extend

Module ConsolePad
    Private const Separator as String = ","

    Dim ReadOnly Version As String = Reflection.Assembly.GetEntryAssembly.GetName.Version.ToString

    Sub Main(args As String())
        Console.Title = "ColorPad Console V" + Version
        Console.WriteLine("++++++++++++++++++++++++++++")
        Console.WriteLine()
        Console.WriteLine("    ====================    ")
        Console.WriteLine("      ColorPad Console      ")
        Console.WriteLine("    ====================    ")
        Console.WriteLine()
        Console.WriteLine(" > Ver " + Version)
        Console.WriteLine("++++++++++++++++++++++++++++")
        Console.WriteLine()
        Dim defaultColor = Console.ForegroundColor
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
            Dim func As Func(Of String, IConvertBridge) = GetConvertBridge(input)
            If func Is Nothing Then
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("Unknown command: " + input)
                Console.ForegroundColor = defaultColor
                Console.WriteLine()
                Continue While
            End If
            Console.Write("Please input value of color(separated by ',' if necessary): ")
            Dim bridge As IConvertBridge
            Try
                bridge = func(Console.ReadLine().Trim())
            Catch ex As ArgumentNullException
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("Inputted value or format is incorrect")
                Console.ForegroundColor = defaultColor
                Console.WriteLine()
                Continue While
            End Try
            Console.WriteLine()
            Console.WriteLine("====================")
            Console.WriteLine(" Color Information: ")
            Console.ForegroundColor = ConsoleColor.DarkYellow
            Console.WriteLine("    " + bridge.Rgb.ToString())
            Console.WriteLine("    Hex(HTML): #" + bridge.ToHex())
            Console.WriteLine("    HSB(HSV): ({0})", bridge.Hsb.ToString(Separator))
            Console.WriteLine("    HSL: ({0})", bridge.Hsl.ToString(Separator))
            Console.WriteLine("    " + bridge.Cmyk.ToString())
            Console.WriteLine("    " + bridge.YCrCb.ToString())
            Console.WriteLine("    Lab: ({0})", bridge.Lab.ToString(Separator))
            Console.WriteLine("    XYZ: ({0})", bridge.Xyz.ToString(Separator))
            Console.ForegroundColor = defaultColor
            Console.WriteLine("--------------------")
            Console.WriteLine(" Color Formula(HSB): ")
            'Use lambada to print the results of formula
            Dim lambadaPrint = Sub(hsb As Hsb) Console.WriteLine("        " + hsb.ToString(Separator))
            Console.ForegroundColor = ConsoleColor.DarkYellow
            For Each type in [Enum].GetValues(GetType(FormulaType))
                Dim typeName = [Enum].GetName(GetType(FormulaType), type)
                If type = FormulaType.SplitComplementary then typeName = "Split Complementary"
                Console.WriteLine($"    {typeName}: ")
                Array.ForEach(Formula.GetFormula(bridge.Hsb, type), lambadaPrint)
            Next
            Console.ForegroundColor = defaultColor
            Console.WriteLine("====================")
            Console.WriteLine()
        End While
    End Sub

    Private Function GetConvertBridge(type As String) As Func(Of String, IConvertBridge)
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
                Return Function(s) New NormalConvertBridge(CieXyzHelper.ParseStringOfD65(s))
        End Select
        Return Nothing
    End Function
End Module