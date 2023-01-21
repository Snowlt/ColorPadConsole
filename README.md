# ColorPadConsole

A color formulas(schemes) and color model calculation tool

一个用于计算配色方案和颜色模型转换的工具

![Preview of ColorPad](https://github.com/Snowlt/ColorPadConsole/raw/master/Preview.png)

## About Project 关于项目

Project provides functions for color model conversion and handling color-related calculation. Core library can be
imported into other .Net projects to use API.

项目提供了颜色模型转换、处理颜色相关计算的功能。 核心库可以在其他 .Net 项目中引入以使用 API。

---
English version instruction: [README_EN.md](README_EN.md)

---

### 核心库主要功能

1. 提供对颜色模型（类型）的封装

   > RGB / Hex / 灰度值 / HSB(HSV) / HSL / CMYK / YCrCb / Lab(CIE Lab) / XYZ(CIE XYZ)

    - *Hex(HTML) 格式可以视为 RGB 的一种表示形式，在代码中将会自动封装成 RGB 类型进行处理。*

    - *灰度值(Grayscale)是一类特殊的 RGB，等效于红绿蓝数值相同的 RGB。*

2. 转换颜色模型
    - RGB 和 灰度值 / HSB(HSV) / HSL / CMYK / YCrCb / XYZ 互转
    - Lab 和 XYZ 互转

3. 计算配色方案

   在一些场合**配色方案**也被称为“Color scheme”。

   配色方案计算将根据设计中常用的配色方法，计算出其他的颜色色相（HSB中的色相）。

## 在其他项目中使用库

### 项目结构(解决方案结构)

- ColorPadCore (.Net Standard 2.0): 颜色处理、转换的核心库。

- ColorPadConsole (.Net 6.0): 命令行部分。

### 兼容性

从 2.0 开始核心库已分离到 `ColorPadCore` 模块中（在Visual Studio中称为项目）。 `ColorPadCore` 使用 .Net Standard
2.0，可以直接在任何类型的 .Net Framework / .Net Core / .Net 项目中使用（例如WinForm 、控制台程序和WPF）。

### 引入核心库

有两种推荐的方式：

1. git clone `ColorPadConsole` 后直接在其他项目中引入 `ColorPadCore`
2. 将 `ColorPadCore` 编译成 DLL，在其他项目中引入

### 主要类和命名空间

- `Core.Model.*`: 颜色模型

- `Extend.NormalConvertBridge` / `Extend.LazyConvertBridge`: 颜色模型转换工具

- `Core.ModelsManager`: 转换颜色模型和注册转换器的入口（推荐优先使用 `*ConvertBrige`）

- `Core.Formula`: 计算配色方案的入口（基于HSB）

函数和类的详细用法请见代码中的 XML 注释文档，或是 Visual Studio 的提示。

## 在 3.0 中的重大变更

1. 颜色模型改为只读结构（C# Struct）以改善GC性能
2. 改进算法的代码实现，提高性能
3. 禁用颜色模型的构造函数，改为用 `From(...)` 方法创建，同时引入了异常抛出
4. 灰度值（Grayscale）从旧版 RGB 中分离了出来，作为单独的类型
5. `ModelsManager` 注册的转换器改为只接受 `IConvertFromTo` 接口
6. 旧版本的 `Core.Formula` 命名空间被移除，合并到 `Core` 中
7. 旧版本的 `Extend.ConvertBridge` 抽象类替换为 `IConvertBridge` 接口
8. 移除 HSL 和 HSB 之间的直接转换
9. 改变 RGB 转 YCrCb 时的取整方式

## 技术资料

一些颜色转换的资料可以在这些地方找到：

RGB & HSB: http://www.opencv.org.cn/opencvdoc/2.3.2/html/modules/imgproc/doc/miscellaneous_transformations.html

RGB & HSL: https://en.wikipedia.org/wiki/HSL_and_HSV
https://zh.wikipedia.org/wiki/HSL%E5%92%8CHSV%E8%89%B2%E5%BD%A9%E7%A9%BA%E9%97%B4

XYZ & RGB: http://www.easyrgb.com/en/math.php
