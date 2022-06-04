Imports ColorPadCore.Core.Model

Namespace Extend
    Public Enum ColorType
        RGB
        Hex
        Gray
        HSB
        HSL
        CMYK
        YCrCb
        XYZ
        Lab
    End Enum


    Public MustInherit Class ConvertBridge

        Protected InnerRgb As Rgb
        Protected InnerHsb As Hsb
        Protected InnerHsl As Hsl
        Protected InnerCmyk As Cmyk
        Protected InnerYcrcb As YCrCb
        Protected InnerXyz As Xyz
        Protected InnerLab As Lab

        Public Overridable ReadOnly Property Rgb As Rgb
            Get
                Return InnerRgb
            End Get
        End Property

        Public MustOverride ReadOnly Property Hsb As Hsb

        Public MustOverride ReadOnly Property Hsl As Hsl

        Public MustOverride ReadOnly Property Cmyk As Cmyk

        Public MustOverride ReadOnly Property YCrCb As YCrCb

        Public MustOverride ReadOnly Property Xyz As Xyz

        Public MustOverride ReadOnly Property Lab As Lab

        Protected Sub New()
        End Sub

        Public Sub New(bridge As ConvertBridge)
            With bridge
                InnerRgb = .Rgb
                InnerHsb = .Hsb
                InnerHsl = .Hsl
                InnerCmyk = .Cmyk
                InnerYcrcb = .YCrCb
                InnerXyz = .Xyz
                InnerLab = .Lab
            End With
        End Sub

        Public Shared Narrowing Operator CType(obj As ConvertBridge) As Rgb
            Return obj.Rgb
        End Operator

        Public Overridable Function ToHex() As String
            Return Rgb.ToHex(True)
        End Function

        Public Overridable Function ToHex(upper As Boolean) As String
            Return Rgb.ToHex(upper)
        End Function

        Public Overridable Function GetGray() As Integer
            With Rgb
                Return Core.ModelsManager.GrayscaleAlgorithm.Calc(.R, .G, .B)
            End With
        End Function

        Protected Sub SetRgbFrom(Of TSource)(source As TSource)
            InnerRgb = Core.ModelsManager.Convert(Of TSource, Rgb)(source)
        End Sub

        Protected Sub SetIfNull(Of TField)(ByRef target As TField)
            If target Is Nothing Then target = Core.ModelsManager.Convert(Of Rgb, TField)(InnerRgb)
        End Sub

        Protected Sub SetIfNull(Of TSourceField, TTargetField)(source As TSourceField, ByRef target As TTargetField)
            If target Is Nothing Then target = Core.ModelsManager.Convert(Of TSourceField, TTargetField)(source)
        End Sub

    End Class


    ''' <summary>
    ''' 提供各个颜色模型相互转换的扩展类
    ''' </summary>
    Public Class NormalConvertBridge
        Inherits ConvertBridge

        Public Overrides ReadOnly Property Hsb As Hsb
            Get
                Return InnerHsb
            End Get
        End Property

        Public Overrides ReadOnly Property Hsl As Hsl
            Get
                Return InnerHsl
            End Get
        End Property

        Public Overrides ReadOnly Property Cmyk As Cmyk
            Get
                Return InnerCmyk
            End Get
        End Property

        Public Overrides ReadOnly Property YCrCb As YCrCb
            Get
                Return InnerYcrcb
            End Get
        End Property

        Public Overrides ReadOnly Property Xyz As Xyz
            Get
                Return InnerXyz
            End Get
        End Property

        Public Overrides ReadOnly Property Lab As Lab
            Get
                Return InnerLab
            End Get
        End Property

        Public Sub New(model As Rgb)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            InnerRgb = model
            InitConvertPossibleField()
        End Sub

        Public Sub New(model As Hsb)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            InnerHsb = model
            SetRgbFrom(model)
            InitConvertPossibleField()
        End Sub

        Public Sub New(model As Hsl)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            InnerHsl = model
            SetRgbFrom(model)
            InitConvertPossibleField()
        End Sub

        Public Sub New(model As Cmyk)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            InnerCmyk = model
            SetRgbFrom(model)
            InitConvertPossibleField()
        End Sub

        Public Sub New(model As YCrCb)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            InnerYcrcb = model
            SetRgbFrom(model)
            InitConvertPossibleField()
        End Sub

        Public Sub New(model As Xyz)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            InnerXyz = model
            SetRgbFrom(model)
            InitConvertPossibleField()
        End Sub

        Public Sub New(model As Lab)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            InnerLab = model
            InnerXyz = Core.ModelsManager.Convert(Of Lab, Xyz)(Lab)
            SetRgbFrom(InnerXyz)
            InitConvertPossibleField()
        End Sub

        Protected Overridable Sub InitConvertPossibleField()
            SetIfNull(InnerHsb)
            SetIfNull(InnerHsl)
            SetIfNull(InnerCmyk)
            SetIfNull(InnerYcrcb)
            SetIfNull(InnerXyz)
            SetIfNull(InnerXyz, InnerLab)
        End Sub

        Public Shared Function FromRgb(r As Integer, g As Integer, b As Integer) As NormalConvertBridge
            Return New NormalConvertBridge(New Rgb(r, g, b))
        End Function
    End Class

    ''' <summary>
    ''' 提供各个颜色模型互相转换的扩展类，带懒加载
    ''' </summary>
    Public Class LazyConvertBridge
        Inherits ConvertBridge

        ''' <summary>
        ''' 默认的空占位对象
        ''' </summary>
        Public Shared ReadOnly EmptyBridge As ConvertBridge = New LazyConvertBridge(New Rgb(0, 0, 0))

        Public Overrides ReadOnly Property Hsb As Hsb
            Get
                SetIfNull(InnerHsb)
                Return InnerHsb
            End Get
        End Property

        Public Overrides ReadOnly Property Hsl As Hsl
            Get
                SetIfNull(InnerHsl)
                Return InnerHsl
            End Get
        End Property

        Public Overrides ReadOnly Property Cmyk As Cmyk
            Get
                SetIfNull(InnerCmyk)
                Return InnerCmyk
            End Get
        End Property

        Public Overrides ReadOnly Property YCrCb As YCrCb
            Get
                SetIfNull(InnerYcrcb)
                Return InnerYcrcb
            End Get
        End Property

        Public Overrides ReadOnly Property Xyz As Xyz
            Get
                SetIfNull(InnerXyz)
                Return InnerXyz
            End Get
        End Property

        Public Overrides ReadOnly Property Lab As Lab
            Get
                If InnerLab Is Nothing Then
                    SetIfNull(InnerXyz)
                    SetIfNull(InnerXyz, InnerLab)
                End If
                Return InnerLab
            End Get
        End Property

        Public Sub New(model As Rgb)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            InnerRgb = model
        End Sub

        Public Sub New(model As Hsb)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            InnerHsb = model
            SetRgbFrom(model)
        End Sub

        Public Sub New(model As Hsl)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            InnerHsl = model
            SetRgbFrom(model)
        End Sub

        Public Sub New(model As Cmyk)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            InnerCmyk = model
            SetRgbFrom(model)
        End Sub

        Public Sub New(model As YCrCb)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            InnerYcrcb = model
            SetRgbFrom(model)
        End Sub

        Public Sub New(model As Xyz)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            InnerXyz = model
            SetRgbFrom(model)
        End Sub

        Public Sub New(model As Lab)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            InnerLab = model
            InnerXyz = Core.ModelsManager.Convert(Of Lab, Xyz)(Lab)
            SetRgbFrom(InnerXyz)
        End Sub

        Public Shared Shadows Function FromRgb(r As Integer, g As Integer, b As Integer) As LazyConvertBridge
            Return New LazyConvertBridge(New Rgb(r, g, b))
        End Function
    End Class
End Namespace