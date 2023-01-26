# ColorPadConsole

A color formulas(schemes) and color model calculation tool

一个用于计算配色方案和颜色模型转换的工具

![Preview of ColorPad](https://github.com/Snowlt/ColorPadConsole/raw/master/Preview.png)

## About Project 关于项目

Project provides functions for color model conversion and handling color-related calculation. Core library can be
imported into other .Net projects to use API.

项目提供了颜色模型转换、处理颜色相关计算的功能。 核心库可以在其他 .Net 项目中引入以使用 API。

---
中文说明请见: [README.md](README.md)

---

### Core Library Major Functions

1. Provides the encapsulation of the color models(types):

   > RGB / Hex / Grayscale / HSB(HSV) / HSL / CMYK / YCrCb / Lab(CIE Lab) / XYZ(CIE XYZ)

    - *The Hex (HTML) format can be considered a kind of expression of RGB, and is automatically encapsulated in code as
      RGB type for processing.*

    - *Grayscale is a special kind of RGB. Equivalent to RGB with the same red, green and blue values.*

2. Converts the color model
    - RGB to Grayscale / HSB(HSV) / HSL / CMYK / YCrCb / XYZ and backward
    - Lab to XYZ and backward

3. Calculates the color formula

   **Color Formula** also called *Color scheme*, on some occasions.

   Color scheme calculation will calculate the other color hue (Hue in HSB) based on the color matching methods commonly
   used in the design. Supports:

   > Monochromatic, Complementary, SplitComplementary, Analogous, Triadic, Tetradic

## Using Library in Other Project

### Project Structure

- ColorPadCore (.Net Standard 2.0): Core library for color processing and conversion.

  *The core library based on
  .Net Standard 2.0, can be used directly in most of .Net Framework / .Net Core / .Net project (e.g. WinForm, console
  applications and WPF).*

- ColorPadConsole (.Net 6.0): CLI part.

### Importing Core Library

There are two recommended ways to do this.

1. git clone `ColorPadConsole` and import `ColorPadCore` directly into other projects
2. compile `ColorPadCore` into a DLL and import it in another project

### Major Class And Namespace

- `Core.Model.*`: Color models


- `Extend.NormalConvertBridge` / `Extend.LazyConvertBridge`: Tools for convert color models


- `Core.ModelsManager`: Entry of convert color models and register converter (`*ConvertBrige` is recommended as first
  choose)

- `Core.Formula`:Entry of calculate the color formula (base on HSB)

For the detail usage of functions and classes, please see the comments in the code or the tips given by Visual Studio.

### Code usage examples

#### Creating color models

Common:

```csharp
// Use "From" to create from numbers
var rgb = Rgb.From(170, 187, 204);
var grayscale = Grayscale.From(187);
var hsb = Hsb.From(210, 16.67, 80);
var hsl = Hsl.From(210, 25, 73.33);
var cmyk = Cmyk.From(17, 8, 0, 20);
var yCrCb = YCrCb.From(184, 119, 139);
var lab = Lab.From(75.11, -2.29, -10.54);
var xyz = Xyz.From(0.45247, 0.48446, 0.64093);

// Use "FromString" to parse from string(separated by ",")
var rgb = Rgb.FromString("170,187,204");
var grayscale = Grayscale.FromString("187");
var hsb = Hsb.FromString("210,16.67,80");
var hsl = Hsl.FromString("210,25,73.33");
var cmyk = Cmyk.FromString("17,8,0,20");
var yCrCb = YCrCb.FromString("184,119,139");
var lab = Lab.FromString("75.11,-2.29,-10.54");
var xyz = Xyz.FromString("0.45247,0.48446,0.64093");
```

the following methods can be considered for using the Hex format of RGB:

```csharp
// Parse Hex. Case-insensitive, "#" can be omitted, letters and numbers must be 6 in length
var rgb1 = Rgb.FromHex("#AABBCC");
var rgb2 = Rgb.FromHex("AabBCc");
// Parse Hex. Same as above, but support shorten CSS style in HTML
var rgb3 = Rgb.FromHexEnhanced("Abc");
Console.WriteLine(rgb1.ToString()); // -> RGB: (170,187,204)
Console.WriteLine(rgb1 == rgb2);    // -> true
Console.WriteLine(rgb1 == rgb3);    // -> true

// Output Hex
Console.WriteLine(rgb3.ToHex());      // -> AABBCC
Console.WriteLine(rgb3.ToHex(false)); // -> aabbcc
```

CIE-XYZ represents the coordinate in color space, so the `Xyz.FromString` method does not check the valid range of
digits(X/Y/Z).

However, under specified illumination(white point), X/Y/Z should be considered to have a value range, and the range can
be checked using `CieXyzHelper`.
For example, in D65, the ranges of X/Y/Z are: X (0 - 0.95047), Y (0 - 1.0), Z (0 - 1.08883)

```csharp
// Check the range of numbers:
Xyz xyz = CieXyzHelper.ParseStringOfD65("0.95,1.0,1.0");
try {
    CieXyzHelper.ParseStringOfD65("1.0,1.0,1.0"); // throws exception
} catch (ArgumentException e) {
    Console.WriteLine(e);
}
```

#### Converting Between Color Models

1. `ModelsManager`

   `ModelsManager` manages all type converters. `Convert<TSource, TTarget>(TSource)` method can be used to match and
   convert.

   ```csharp
   // 1. Source
   Rgb rgb = Rgb.FromHex("#AABBCC");
   // 2. Convert and then get
   Hsb hsb = ModelsManager.Convert<Rgb, Hsb>(rgb);
   Xyz xyz = ModelsManager.Convert<Rgb, Xyz>(rgb);
   Lab lab = ModelsManager.Convert<Xyz, Lab>(xyz);
   ```

2. Implementation of `IConvertBrige`

   `IConvertBrige` can automatically convert one type to multiple others. Default implementations
   are：`NormalConvertBridge`, `LazyConvertBridge`, which supports: *Rgb / Hsb / Hsl / Cmyk / YCrCb /
   Lab / Xyz* conversion.

   `IConvertBrige` using `ModelsManager.Convert` inside.

   **`IConvertBrige` is recommended for converting to multiple types at once.**

   ```csharp
   // 1. Source
   Rgb rgb = Rgb.FromHex("#AABBCC");
   // 2. Create NormalConvertBridge
   IConvertBridge res = new NormalConvertBridge(rgb);
   // 3. Get
   Hsb hsb = res.Hsb;
   Lab lab = res.Lab;
   byte grayscale = res.Grayscale; // IConvertBrige returns byte instead of Grayscale
   ```

3. Customizing converter

   To customize type converter, simply implement the `IConvertFromTo` interface and register the implementation
   into `ModelsManager`.

   Take convert `rgb` to `hsb` as an example:
   ```csharp
   // 1. Implement interface
   public class CustomizedHsbConverter: IConvertFromTo<Rgb, Hsb>
   {
      public Hsb Convert(in Rgb source)
      {
         Hsb hsb;
         // Implementation here
         return hsb;
      }
   }
   // 2. Register converter
   ModelsManager.Register<Rgb, Hsb>(new CustomizedHsbConverter());
   // 3. Use
   ```

#### Calculating Color Formula

Class `Formula` provides the main method of calculating color formulas. Also can
use `Formula.GetFormula(Hsb, FormulaType)` as the entry method.

```csharp
var hsb = Hsb.From(210, 16.67, 80);
// Calculate Complementary
var formulas = Formula.GetFormula(hsb, FormulaType.Complementary);
// Output
foreach (var formula in formulas)
{
   Console.WriteLine(formula);
}
/* Result:
* HSB: (210,16.67,80)
* HSB: (30,16.67,80)
*/
```

Please see enum: `FormulaType` for the calculation type.

## Breaking Changes in 3.0

Since 3.0, there are the following major changes compared to older versions of the API:

1. Changed color model to readonly structure (C# Struct) to improve GC performance, and changed creating ways
2. Changed the code implementation of the algorithm and improve performance
3. Grayscale has been separated from the RGB as an independent type
4. `ModelsManager` registered converters now accept only the `IConvertFromTo` interface
5. `Core.Formula` namespace has been removed from the old version and merged into `Core`
6. `ConvertBridge` abstract class from the old version was replaced with `IConvertBridge` interface
7. Removed direct conversion between HSL and HSB
8. Changed the rounding method when converting RGB to YCrCb

## Technical Information

Some information about Color Model conversion can be found in the following locations:

RGB & HSB: http://www.opencv.org.cn/opencvdoc/2.3.2/html/modules/imgproc/doc/miscellaneous_transformations.html

RGB & HSL: https://en.wikipedia.org/wiki/HSL_and_HSV
https://zh.wikipedia.org/wiki/HSL%E5%92%8CHSV%E8%89%B2%E5%BD%A9%E7%A9%BA%E9%97%B4

XYZ & RGB: http://www.easyrgb.com/en/math.php
