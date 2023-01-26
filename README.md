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

   配色方案计算将根据设计中常用的配色方法，计算出其他的颜色色相（HSB中的色相）。支持计算：

   > 同类色, 互补色, 分裂互补色, 邻近色, 三角色, 四角色

## 在其他项目中使用库

### 项目结构(解决方案结构)

- ColorPadCore (.Net Standard 2.0): 颜色处理、转换的核心库。

  *核心库基于 .Net Standard 2.0，可以直接在大多数 .Net Framework / .Net Core / .Net 项目中直接使用（例如WinForm
  、控制台程序和WPF）。*

- ColorPadConsole (.Net 6.0): 命令行部分。

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

### 代码主要用法

#### 创建颜色模型

通用：

```csharp
// 使用 From 从数字创建
var rgb = Rgb.From(170, 187, 204);
var grayscale = Grayscale.From(187);
var hsb = Hsb.From(210, 16.67, 80);
var hsl = Hsl.From(210, 25, 73.33);
var cmyk = Cmyk.From(17, 8, 0, 20);
var yCrCb = YCrCb.From(184, 119, 139);
var lab = Lab.From(75.11, -2.29, -10.54);
var xyz = Xyz.From(0.45247, 0.48446, 0.64093);

// 使用 FromString 从字符串解析(","分割)
var rgb = Rgb.FromString("170,187,204");
var grayscale = Grayscale.FromString("187");
var hsb = Hsb.FromString("210,16.67,80");
var hsl = Hsl.FromString("210,25,73.33");
var cmyk = Cmyk.FromString("17,8,0,20");
var yCrCb = YCrCb.FromString("184,119,139");
var lab = Lab.FromString("75.11,-2.29,-10.54");
var xyz = Xyz.FromString("0.45247,0.48446,0.64093");
```

如果要使用 RGB 的 16 进制（Hex）格式，可以使用以下方法：

```csharp
// 从 16 进制解析（不区分大小写，"#"可省略，字母和数字的长度必须为6）
var rgb1 = Rgb.FromHex("#AABBCC");
var rgb2 = Rgb.FromHex("AabBCc");
// 从 16 进制解析（同上，但支持 HTML 的 CSS 缩写样式）
var rgb3 = Rgb.FromHexEnhanced("Abc");
Console.WriteLine(rgb1.ToString()); // -> RGB: (170,187,204)
Console.WriteLine(rgb1 == rgb2);    // -> true
Console.WriteLine(rgb1 == rgb3);    // -> true

// 输出为 16 进制
Console.WriteLine(rgb3.ToHex());      // -> AABBCC
Console.WriteLine(rgb3.ToHex(false)); // -> aabbcc
```

CIE-XYZ 表示一个色彩空间的坐标，故 `Xyz.FromString` 方法不校验数字（X/Y/Z）的有效范围。

但在特定光照条件（白点值）下，X/Y/Z 应该被认为是有取值范围的，可以使用 `CieXyzHelper` 解析检查范围。
例如在 D65 光照时，X/Y/Z 的范围是：X (0 - 0.95047), Y (0 - 1.0), Z (0 - 1.08883)。

```csharp
// 以下方法会检查数字的范围： 
Xyz xyz = CieXyzHelper.ParseStringOfD65("0.95,1.0,1.0");
try {
    CieXyzHelper.ParseStringOfD65("1.0,1.0,1.0"); // 抛出异常
} catch (ArgumentException e) {
    Console.WriteLine(e);
}
```

#### 转换颜色模型

1. 使用 `ModelsManager` 转换

   `ModelsManager` 负责管理类型转换器。可以直接用 `Convert<TSource, TTarget>(TSource)` 方法匹配类型并转换。
   ```csharp
   // 1. 源
   Rgb rgb = Rgb.FromHex("#AABBCC");
   // 2. 转换并获取结果
   Hsb hsb = ModelsManager.Convert<Rgb, Hsb>(rgb);
   Xyz xyz = ModelsManager.Convert<Rgb, Xyz>(rgb);
   Lab lab = ModelsManager.Convert<Xyz, Lab>(xyz);
   ```

2. 使用 `IConvertBrige` 实现类转换

   `IConvertBrige` 可以自动将一个类型转换到其他多种类型。默认提供的实现类有：`NormalConvertBridge`, `LazyConvertBridge`
   ，支持：*Rgb / Hsb / Hsl / Cmyk / YCrCb / Lab / Xyz* 互转。

   内部是借助了 `ModelsManager.Convert` 进行转换。**如果需要一次性转换到多个类型，建议使用 `IConvertBrige`。**

   ```csharp
   // 1. 源
   Rgb rgb = Rgb.FromHex("#AABBCC");
   // 2. 创建 NormalConvertBridge
   IConvertBridge res = new NormalConvertBridge(rgb);
   // 3. 获取结果
   Hsb hsb = res.Hsb;
   Lab lab = res.Lab;
   byte grayscale = res.Grayscale; // IConvertBrige 会直接返回 byte 而非 Grayscale 类型
   ```

3. 自定义转换器

   如果想自定义类型的转换器，只需实现 `IConvertFromTo` 接口，并将实现类注册到 `ModelsManager` 中即可。

   以自定义 `Rgb` 转到 `Hsb` 为例：
   ```csharp
   // 1. 定义实现类
   public class CustomizedHsbConverter: IConvertFromTo<Rgb, Hsb>
   {
      public Hsb Convert(in Rgb source)
      {
         Hsb hsb;
         // 实现的代码
         return hsb;
      }
   }
   // 2. 注册转换器
   ModelsManager.Register<Rgb, Hsb>(new CustomizedHsbConverter());
   // 3. 使用
   ```

#### 计算配色方案

`Formula` 类中提供了主要的配色方案计算方法。也可以使用入口方法：`Formula.GetFormula(Hsb, FormulaType)`

```csharp
var hsb = Hsb.From(210, 16.67, 80);
// 计算互补色
var formulas = Formula.GetFormula(hsb, FormulaType.Complementary);
// 输出结果
foreach (var formula in formulas)
{
   Console.WriteLine(formula);
}
/* Result:
* HSB: (210,16.67,80)
* HSB: (30,16.67,80)
*/
```

计算类型请见枚举：`FormulaType`

## 在 3.0 中的重大变更

从 3.0 起，相比旧版本 API 有以下大调整：

1. 颜色模型改为只读结构（C# Struct）以改善GC性能，同时修改了创建方法
2. 改进算法的代码实现，提高性能
3. 灰度值（Grayscale）从 RGB 中分离了出来，作为单独的类型
4. `ModelsManager` 注册的转换器改为只接受 `IConvertFromTo` 接口
5. 旧版本的 `Core.Formula` 命名空间被移除，合并到 `Core` 中
6. 旧版本的 `Extend.ConvertBridge` 抽象类替换为 `IConvertBridge` 接口
7. 移除 HSL 和 HSB 之间的直接转换
8. 改变 RGB 转 YCrCb 时的取整方式

## 技术资料

一些颜色转换的资料可以在这些地方找到：

RGB & HSB: http://www.opencv.org.cn/opencvdoc/2.3.2/html/modules/imgproc/doc/miscellaneous_transformations.html

RGB & HSL: https://en.wikipedia.org/wiki/HSL_and_HSV
https://zh.wikipedia.org/wiki/HSL%E5%92%8CHSV%E8%89%B2%E5%BD%A9%E7%A9%BA%E9%97%B4

XYZ & RGB: http://www.easyrgb.com/en/math.php
