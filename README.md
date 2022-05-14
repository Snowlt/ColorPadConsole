# ColorPadConsole

A color formulas(schemes) and color model calculation tool

一个用于计算配色方案和颜色模型转换的工具

![Preview of ColorPad](https://github.com/Snowlt/ColorPadConsole/raw/master/Preview.png)



## About Project 关于项目

The project using Visual Basic.Net, which can be easily rewritten into other languages.

项目使用 Visual Basic.Net 编写，可以很容易地改写为其他语言。

Core libraries can be imported into other projects to process color-related calculations.

核心库可以在其他项目中引入，以处理颜色相关的计算。

### Core Library Major Functions 核心库主要功能：

1. Encapsulates the following color models: 

   封装以下颜色模型：

   > RGB / HSB(HSV) / HSL / CMYK / YCrCb / Lab(CIE Lab) / XYZ(CIE XYZ)

2. Convert the color model

   转换颜色模型

3. Calculate the color formula

   计算配色方案

## Using Library in Other Project 在其他项目中使用库

### Project Structure 项目结构(解决方案结构)

- ColorPadCore (.Net Standard 2.0)

  Core library for color processing and conversion.

  颜色处理、转换的核心库。

- ColorPadConsole (.Net 6.0)

  CLI part.

  命令行部分。

### Compatibility 兼容性

Since 2.0 the core library has been separated into the `ColorPadCore` module (called Project in Visual Studio). With .Net Standard 2.0, `ColorPadCore` can be used directly in any type of  .Net Framework / .Net Core / .Net  project (e.g. WinForm, console applications and WPF).

从 2.0 开始核心库已分离到 `ColorPadCore` 模块中（在Visual Studio中称为项目）。 `ColorPadCore` 使用 .Net Standard 2.0，可以直接在任何类型的 .Net Framework / .Net Core / .Net 项目中使用（例如WinForm 、控制台程序和WPF）。

### Importing Core Library 引入核心库

There are two recommended ways to do this.

1. git clone `ColorPadConsole` and import `ColorPadCore` directly into other projects
2. compile `ColorPadCore` into a DLL and introduce it in another project

有两种推荐的方式：

1. git clone `ColorPadConsole` 后直接在其他项目中引入 `ColorPadCore`
2. 将 `ColorPadCore` 编译成 DLL，在其他项目中引入

### Major Class And Namespace 主要类和命名空间

- `Core.Model.*`

  Color models

  颜色模型

- `Extend.NormalConvertBridge` / `Extend.LazyConvertBridge`

  Tools for convert color model

  颜色模型转换工具

- `Core.Converter.ModelsManager`

  Entry of process color models conversion (`ConvertBrige` is recommended as first choose)

  处理颜色模型转换的入口（推荐优先使用 `ConvertBrige`）

- `Core.Formula.Formula`

  Calculate the color formula (base on HSB)

  计算配色方案（基于HSB）



For the detail usage of functions and classes, plaese see the comments in the code or the tips given by Visual Studio.

函数和类的详细用法请见代码中的注释，或是 Visual Studio 的提示。



## Color Model (Space) Conversion 颜色模型（空间）转换

Project supports the following color models and/or color spaces

项目支持以下颜色模型和色彩空间

RGB / HSB(HSV) / HSL / CMYK / YCrCb / Lab(CIE Lab) / XYZ(CIE XYZ)

The Hex (HTML) format can be considered a kind of expression of RGB, and is automatically encapsulated in code as RGB type for processing.

Hex(HTML) 格式可以视为 RGB 的一种表示形式，在代码中将会自动封装成 RGB 类型进行处理。



## Color Formula 配色方案

**Corlor Formula** also called *Color scheme*, on some occasions.

在一些场合**配色方案**也被称为“Color scheme”。

Color scheme calculation will calculate the other color hue (Hue in HSB) based on the color matching methods commonly used in the design.

配色方案计算将根据设计中常用的配色方法，计算出其他的颜色色相（HSB中的色相）。



## Technical Information 技术资料

Some information about Color Model conversion can be found in the following locations:

一些颜色转换的资料可以在这些地方找到：

RGB & HSB: http://www.opencv.org.cn/opencvdoc/2.3.2/html/modules/imgproc/doc/miscellaneous_transformations.html

RGB & HSL: https://en.wikipedia.org/wiki/HSL_and_HSV
https://zh.wikipedia.org/wiki/HSL%E5%92%8CHSV%E8%89%B2%E5%BD%A9%E7%A9%BA%E9%97%B4

XYZ & RGB: http://www.easyrgb.com/en/math.php
