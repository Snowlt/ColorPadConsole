# ColorPadConsole
A tool for calculating color formulas (schemes) and color model conversions
一个用于计算配色方案和颜色模型转换的工具

## About Project 关于项目
The project using Visual Basic.Net, which can be easily rewritten into other languages.
项目使用 Visual Basic.Net 编写，可以很容易地改写为其他语言。

The code can be used in .Net Framework WinForm and Console application directly. And support WPF, in theory(Not fully tested yet).
代码能直接用于 .Net Framework 的 WinForm 和控制台程序，理论上也能支持 WPF。

## Use Code in Other Project 在其他项目中使用代码
The core code for this project is in three files:
项目的核心代码在以下三个文件中：
CoreColorFormula.vb
CoreColorModel.vb
CoreModelAlgorithm.vb

Just simply copy the files to other VB.Net projects. Modify the imported namespace at the beginning of the file (Imports statement), then it can be used.
简单的将文件复制到其他 VB.Net 项目中，并修改文件开头导入的命名空间（Imports 语句），即可使用了。

For the usage of functions and classes, plaese see the comments in the code or the tips given by Visual Studio.
函数和类用法请见代码中的注释，或是 Visual Studio 的提示。

## Color Model (Space) Conversion 颜色模型（空间）转换
Project supports the following color models or color spaces
项目支持以下颜色模型或色彩空间
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