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

   提供对以下颜色模型的封装：

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
   used in the design.

## Using Library in Other Project

### Project Structure

- ColorPadCore (.Net Standard 2.0): Core library for color processing and conversion.


- ColorPadConsole (.Net 6.0): CLI part.

### Compatibility

Since 2.0 the core library has been separated into the `ColorPadCore` module (called Project in Visual Studio). With
.Net Standard 2.0, `ColorPadCore` can be used directly in any type of .Net Framework / .Net Core / .Net project (e.g.
WinForm, console applications and WPF).

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

## Breaking Changes in 3.0

1. Changed color model to readonly structure (C# Struct) to improve GC performance
2. Changed the code implementation of the algorithm and improve performance
3. Disabled the constructor of the color model, instead, use `From(...) ` method, and may throws exception
4. Grayscale has been separated from the RGB as an independent type
5. `ModelsManager` registered converters now accept only the `IConvertFromTo` interface
6. `Core.Formula` namespace has been removed from the old version and merged into `Core`
7. `ConvertBridge` abstract class from the old version was replaced with `IConvertBridge` interface
8. Removed direct conversion between HSL and HSB
9. Changed the rounding method when converting RGB to YCrCb

## Technical Information

Some information about Color Model conversion can be found in the following locations:

RGB & HSB: http://www.opencv.org.cn/opencvdoc/2.3.2/html/modules/imgproc/doc/miscellaneous_transformations.html

RGB & HSL: https://en.wikipedia.org/wiki/HSL_and_HSV
https://zh.wikipedia.org/wiki/HSL%E5%92%8CHSV%E8%89%B2%E5%BD%A9%E7%A9%BA%E9%97%B4

XYZ & RGB: http://www.easyrgb.com/en/math.php
