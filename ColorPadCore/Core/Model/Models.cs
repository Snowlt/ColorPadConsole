using System;
using System.Globalization;
using System.Text;

namespace ColorPadCore.Core.Model
{
    /*
     Interfaces Of Color Models
     颜色模型的接口
    */
    public interface IColorModel
    {
        /// <summary>
        /// Make a string of Color Model
        /// 生成表示颜色模型的字符串
        /// </summary>
        /// <param name="separator">分隔符</param>
        /// <returns>表示颜色的字符串</returns>
        string ToString(string separator);
    }

    /*
     Struct of Color Model Data
     颜色模型数据的结构
    */

    /// <summary>
    /// Represents RGB Color Model
    /// 表示 RGB 颜色模型
    /// </summary>
    public readonly struct Rgb : IColorModel, IEquatable<Rgb>
    {
        /// <summary>
        /// Pure white 纯白色
        /// <para>RGB(255, 255, 255)</para>
        /// </summary>
        public static readonly Rgb White = new Rgb(255, 255, 255);

        /// <summary>
        /// Pure black 纯黑色
        /// <para>RGB(0, 0, 0)</para>
        /// </summary>
        public static readonly Rgb Black = new Rgb(0);

        /// <summary>
        /// Red (0 - 255)
        /// </summary>
        /// <returns></returns>
        public byte R { get; }

        /// <summary>
        /// Green (0 - 255)
        /// </summary>
        /// <returns></returns>
        public byte G { get; }

        /// <summary>
        /// Blue (0 - 255)
        /// </summary>
        /// <returns></returns>
        public byte B { get; }

        private Rgb(int r, int g, int b)
        {
            R = (byte) r;
            G = (byte) g;
            B = (byte) b;
        }

        private Rgb(int value)
        {
            R = (byte) (value >> 16 & 255);
            G = (byte) (value >> 8 & 255);
            B = (byte) (value & 255);
        }

        internal Rgb(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public bool CompareWith(int r, int g, int b)
        {
            return Equals(new Rgb(r, g, b));
        }

        /// <summary>
        /// Create Hex / HTML string of RGB
        /// 生成 RGB 的 16进制 / HTML 格式字符串
        /// </summary>
        /// <returns>RGB Hex</returns>
        public string ToHex(bool upper = true)
        {
            return ToInteger().ToString(upper ? "X6" : "x6");
        }

        public bool Equals(Rgb other)
        {
            return R == other.R && G == other.G && B == other.B;
        }

        public override bool Equals(object obj)
        {
            return obj is Rgb other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ToInteger();
        }

        public static bool operator ==(Rgb left, Rgb right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Rgb left, Rgb right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"RGB: ({R},{G},{B})";
        }

        public string ToString(string separator)
        {
            return string.Format("{0}{3}{1}{3}{2}", R, G, B, separator);
        }

        /// <summary>
        /// Get the integer value of RGB, equals to decimal value of hex()
        /// </summary>
        /// <returns>Integer value</returns>
        private int ToInteger()
        {
            return R << 16 | G << 8 | B;
        }

        /// <summary>
        /// Create the RGB model
        /// 创建 RGB 模型
        /// </summary>
        /// <param name="r">Red (0 - 255)</param>
        /// <param name="g">Green (0 - 255)</param>
        /// <param name="b">Blue (0 - 255)</param>
        /// <returns>RGB</returns>
        /// <exception cref="ArgumentOutOfRangeException">Value out of range / 数值超出范围</exception>
        public static Rgb From(int r, int g, int b)
        {
            CheckRange(r, g, b);
            return new Rgb(r, g, b);
        }

        /// <summary>
        /// Parse out the RGB value from a string
        /// 从一个字符串解析出 RGB 值
        /// </summary>
        /// <param name="color">String of RGB / RGB 字符串</param>
        /// <returns>RGB</returns>
        /// <exception cref="ArgumentException">Unable to parse / 无法解析</exception>
        public static Rgb FromString(string color)
        {
            var cm = Basic.ExtractFromStringAsInt(color);
            if (cm.Length != 3) throw new ArgumentException();
            CheckRange(cm[0], cm[1], cm[2]);
            return new Rgb(cm[0], cm[1], cm[2]);
        }

        /// <summary>
        /// Parse out the RGB value from a Hex string
        /// 从一个 Hex 字符串中解析出 RGB 值
        /// </summary>
        /// <param name="color">String of Hex / Hex 字符串</param>
        /// <returns>RGB</returns>
        /// <exception cref="ArgumentException">Unable to parse / 无法解析</exception>
        public static Rgb FromHex(string color)
        {
            if (string.IsNullOrEmpty(color)) throw new ArgumentException();
            // 处理并分割字符串
            if (color[0] == '#') color = color.Substring(1);
            // 检查格式
            if (color.Length != 6) throw new ArgumentException();
            try
            {
                return new Rgb(int.Parse(color, NumberStyles.HexNumber));
            }
            catch (Exception)
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Parse out the RGB value from a Hex string(Support incomplete Hex and CSS style)
        /// 从一个 Hex 字符串中解析出 RGB 值(支持不完整的 Hex 和 CSS 样式)
        /// </summary>
        /// <param name="color">String of Hex / Hex 字符串</param>
        /// <returns>RGB</returns>
        /// <exception cref="ArgumentException">Unable to parse / 无法解析</exception>
        public static Rgb FromHexEnhanced(string color)
        {
            if (string.IsNullOrEmpty(color)) throw new ArgumentException();
            // 处理并分割字符串
            if (color[0] == '#') color = color.Substring(1);
            // 检查格式
            if (color.Length > 6) throw new ArgumentException();
            if (color.Length == 3)
            {
                // CSS样式
                color = new StringBuilder().Append(color[0]).Append(color[0]).Append(color[1]).Append(color[1])
                    .Append(color[2]).Append(color[2]).ToString();
            }

            try
            {
                return new Rgb(int.Parse(color, NumberStyles.HexNumber));
            }
            catch (Exception)
            {
                throw new ArgumentException();
            }
        }

        private static void CheckRange(int r, int g, int b)
        {
            if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Represents Grayscale of RGB
    /// 表示 RGB 的灰度值
    /// <seealso cref="Rgb"/>
    /// </summary>
    public readonly struct Grayscale : IColorModel, IEquatable<Grayscale>
    {
        /// <summary>
        /// Value (0 - 255)
        /// </summary>
        /// <returns></returns>
        public byte Value { get; }

        private Grayscale(int g) => Value = (byte) g;

        public Rgb ToRgb() => new Rgb(Value, Value, Value);

        public bool Equals(Grayscale other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Grayscale other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public static bool operator ==(Grayscale left, Grayscale right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Grayscale left, Grayscale right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"Grayscale: {Value}";
        }

        public string ToString(string separator)
        {
            return Value.ToString();
        }

        /// <summary>
        /// Create the Grayscale model
        /// 创建 Grayscale 模型
        /// </summary>
        /// <param name="g">Grayscale value (0 - 255)</param>
        /// <returns>Grayscale</returns>
        /// <exception cref="ArgumentOutOfRangeException">Value out of range / 数值超出范围</exception>
        public static Grayscale From(int g)
        {
            CheckRange(g);
            return new Grayscale(g);
        }

        /// <summary>
        /// Parse the Grayscale value from string
        /// 从字符串中解析出灰度值
        /// </summary>
        /// <param name="color">String containing Grayscale / 包含灰度值的字符串</param>
        /// <returns>Grayscale</returns>
        /// <exception cref="ArgumentException">Unable to parse / 无法解析</exception>
        /// <exception cref="ArgumentOutOfRangeException">Value out of range / 数值超出范围</exception>
        public static Grayscale FromString(string color)
        {
            var cm = Basic.ExtractFromStringAsInt(color);
            if (cm.Length != 1) throw new ArgumentException();
            return From(cm[0]);
        }

        private static void CheckRange(int g)
        {
            if (g < 0 || g > 255) throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Represents HSB / HSV Color Model
    /// 表示 HSB / HSV 颜色模型
    /// </summary>
    public readonly struct Hsb : IColorModel, IEquatable<Hsb>
    {
        /// <summary>
        /// Hue (0 - 360)
        /// </summary>
        /// <returns></returns>
        public double H { get; }

        /// <summary>
        /// Saturation (0 - 100)
        /// </summary>
        /// <returns></returns>
        public double S { get; }

        /// <summary>
        /// Brightness / Value (0 - 100)
        /// </summary>
        /// <returns></returns>
        public double B { get; }

        private Hsb(double h, double s, double b)
        {
            H = h;
            S = s;
            B = b;
        }

        public bool CompareWith(double h, double s, double b)
        {
            return Equals(new Hsb(h, s, b));
        }

        public bool Equals(Hsb other)
        {
            return Basic.DecimalEquals(H, other.H) && Basic.DecimalEquals(S, other.S) &&
                   Basic.DecimalEquals(B, other.B);
        }

        public override bool Equals(object obj)
        {
            return obj is Hsb other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = H.GetHashCode();
                hashCode = (hashCode * 397) ^ S.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Hsb left, Hsb right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Hsb left, Hsb right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"HSB: ({ToString(",")})";
        }

        public string ToString(string separator)
        {
            return string.Format("{0:0.##}{3}{1:0.##}{3}{2:0.##}", H, S, B, separator);
        }

        /// <summary>
        /// Create the HSB / HSV model
        /// 创建 HSB / HSV 模型
        /// </summary>
        /// <param name="h">Hue (0 - 360)</param>
        /// <param name="s">Saturation (0 - 100)</param>
        /// <param name="b">Brightness / Value (0 - 100)</param>
        /// <returns>HSB</returns>
        /// <exception cref="ArgumentOutOfRangeException">Value out of range / 数值超出范围</exception>
        public static Hsb From(double h, double s, double b)
        {
            CheckRange(h, s, b);
            return new Hsb(h, s, b);
        }

        /// <summary>
        /// Parse out the HSB value from a string
        /// 从一个字符串解析出 HSB 值
        /// </summary>
        /// <param name="color">String of HSB / HSB 字符串</param>
        /// <returns>HSB</returns>
        /// <exception cref="ArgumentException">Unable to parse / 无法解析</exception>
        /// <exception cref="ArgumentOutOfRangeException">Value out of range / 数值超出范围</exception>
        public static Hsb FromString(string color)
        {
            double[] cm = Basic.ExtractFromString(color);
            // 检查格式
            if (cm.Length != 3) throw new ArgumentException();
            // 检查范围
            double h = cm[0];
            double s = cm[1];
            double b = cm[2];
            CheckRange(h, s, b);
            // 返回对象
            return new Hsb(h, s, b);
        }

        private static void CheckRange(double h, double s, double b)
        {
            if (h < 0d || h > 360d || s < 0d || s > 100d || b < 0d || b > 100d)
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Represents HSL Color Model
    /// 表示 HSL 颜色模型
    /// </summary>
    public readonly struct Hsl : IColorModel, IEquatable<Hsl>
    {
        /// <summary>
        /// Hue (0 - 360)
        /// </summary>
        /// <returns></returns>
        public double H { get; }

        /// <summary>
        /// Saturation (0 - 100)
        /// </summary>
        /// <returns></returns>
        public double S { get; }

        /// <summary>
        /// Lightness (0 - 100)
        /// </summary>
        /// <returns></returns>
        public double L { get; }

        private Hsl(double h, double s, double l)
        {
            H = h;
            S = s;
            L = l;
        }

        public bool CompareWith(double h, double s, double l)
        {
            return Equals(new Hsl(h, s, l));
        }

        public bool Equals(Hsl other)
        {
            return Basic.DecimalEquals(H, other.H) && Basic.DecimalEquals(S, other.S) &&
                   Basic.DecimalEquals(L, other.L);
        }

        public override bool Equals(object obj)
        {
            return obj is Hsl other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = H.GetHashCode();
                hashCode = (hashCode * 397) ^ S.GetHashCode();
                hashCode = (hashCode * 397) ^ L.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Hsl left, Hsl right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Hsl left, Hsl right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"HSL: ({ToString(",")})";
        }

        public string ToString(string separator)
        {
            return string.Format("{0:0.##}{3}{1:0.##}{3}{2:0.##}", H, S, L, separator);
        }

        /// <summary>
        /// Create the HSL model
        /// 创建 HSL 模型
        /// </summary>
        /// <param name="h">Hue (0 - 360)</param>
        /// <param name="s">Saturation (0 - 100)</param>
        /// <param name="l">Lightness (0 - 100)</param>
        /// <returns>HSB</returns>
        /// <exception cref="ArgumentOutOfRangeException">Value out of range / 数值超出范围</exception>
        public static Hsl From(double h, double s, double l)
        {
            CheckRange(h, s, l);
            return new Hsl(h, s, l);
        }

        /// <summary>
        /// Parse out the HSL value from a string
        /// 从一个字符串解析出 HSL 值
        /// </summary>
        /// <param name="color">String of HSB / HSB 字符串</param>
        /// <returns>HSB</returns>
        /// <exception cref="ArgumentException">Unable to parse / 无法解析</exception>
        /// <exception cref="ArgumentOutOfRangeException">Value out of range / 数值超出范围</exception>
        public static Hsl FromString(string color)
        {
            double[] cm = Basic.ExtractFromString(color);
            // 检查格式
            if (cm.Length != 3) throw new ArgumentException();
            // 检查范围
            double h = cm[0];
            double s = cm[1];
            double l = cm[2];
            CheckRange(h, s, l);
            // 返回对象
            return new Hsl(h, s, l);
        }

        private static void CheckRange(double h, double s, double l)
        {
            if (h < 0d || h > 360d || s < 0d || s > 100d || l < 0d || l > 100d)
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Represents CMYK Color Model
    /// 表示 CMYK 颜色模型
    /// </summary>
    public readonly struct Cmyk : IColorModel, IEquatable<Cmyk>
    {
        /// <summary>
        /// Cyan (0 - 100)
        /// </summary>
        /// <returns></returns>
        public byte C { get; }

        /// <summary>
        /// Magenta (0 - 100)
        /// </summary>
        /// <returns></returns>
        public byte M { get; }

        /// <summary>
        /// Yellow (0 - 100)
        /// </summary>
        /// <returns></returns>
        public byte Y { get; }

        /// <summary>
        /// Key / Black (0 - 100)
        /// </summary>
        /// <returns></returns>
        public byte K { get; }

        private Cmyk(int c, int m, int y, int k)
        {
            C = (byte) c;
            M = (byte) m;
            Y = (byte) y;
            K = (byte) k;
        }

        public bool CompareWith(int c, int m, int y, int k)
        {
            return Equals(new Cmyk(c, m, y, k));
        }

        public bool Equals(Cmyk other)
        {
            return C == other.C && M == other.M && Y == other.Y && K == other.K;
        }

        public override bool Equals(object obj)
        {
            return obj is Cmyk other && Equals(other);
        }

        public override int GetHashCode()
        {
            return C << 24 | M << 16 | Y << 8 | K;
        }

        public static bool operator ==(Cmyk left, Cmyk right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Cmyk left, Cmyk right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"CMYK: ({C},{M},{Y},{K})";
        }

        public string ToString(string separator)
        {
            return string.Format("{0}{4}{1}{4}{2}{4}{3}", C, M, Y, K, separator);
        }

        /// <summary>
        /// Create the CMYK model
        /// 创建 CMYK 模型
        /// </summary>
        /// <param name="c">Cyan (0 - 100)</param>
        /// <param name="m">Magenta (0 - 100)</param>
        /// <param name="y">Yellow (0 - 100)</param>
        /// <param name="k">Key / Black (0 - 100)</param>
        /// <returns>CMYK</returns>
        /// <exception cref="ArgumentOutOfRangeException">Value out of range / 数值超出范围</exception>
        public static Cmyk From(int c, int m, int y, int k)
        {
            CheckRange(c, m, y, k);
            return new Cmyk(c, m, y, k);
        }

        /// <summary>
        /// Parse out the CMYK value from a string
        /// 从一个字符串解析出 CMYK 值
        /// </summary>
        /// <param name="color">String of CMYK / CMYK 字符串</param>
        /// <returns>CMYK Object / CMYK 对象</returns>
        public static Cmyk FromString(string color)
        {
            var cm = Basic.ExtractFromStringAsInt(color);
            if (cm.Length != 4) throw new ArgumentException();
            CheckRange(cm[0], cm[1], cm[2], cm[3]);
            return new Cmyk(cm[0], cm[1], cm[2], cm[3]);
        }

        private static void CheckRange(int c, int m, int y, int k)
        {
            if (c < 0 || c > 100 || m < 0 || m > 100 || y < 0 || y > 100 || k < 0 || k > 100)
                throw new ArgumentOutOfRangeException();
        }
    }


    /// <summary>
    /// Represents YCrCb Color Model
    /// 表示 YCrCb 颜色模型
    /// </summary>
    public readonly struct YCrCb : IColorModel, IEquatable<YCrCb>
    {
        /// <summary>
        /// Y (0 - 255)
        /// </summary>
        /// <returns></returns>
        public byte Y { get; }

        /// <summary>
        /// Cr (0 - 255)
        /// </summary>
        /// <returns></returns>
        public byte Cr { get; }

        /// <summary>
        /// Cb (0 - 255)
        /// </summary>
        /// <returns></returns>
        public byte Cb { get; }

        private YCrCb(int r, int g, int b)
        {
            Y = (byte) r;
            Cr = (byte) g;
            Cb = (byte) b;
        }

        public bool CompareWith(int y, int cr, int cb)
        {
            return Equals(new YCrCb(y, cr, cb));
        }

        public bool Equals(YCrCb other)
        {
            return Y == other.Y && Cr == other.Cr && Cb == other.Cb;
        }

        public override bool Equals(object obj)
        {
            return obj is YCrCb other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Y << 16 | Cr << 8 | Cb;
        }

        public static bool operator ==(YCrCb left, YCrCb right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(YCrCb left, YCrCb right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"YCrCb: ({Y},{Cr},{Cb})";
        }

        public string ToString(string separator)
        {
            return string.Format("{0}{3}{1}{3}{2}", Y, Cr, Cb, separator);
        }

        /// <summary>
        /// Create the YCrCb model
        /// 创建 YCrCb 模型
        /// </summary>
        /// <param name="y">Y (0 - 255)</param>
        /// <param name="cr">Cr (0 - 255)</param>
        /// <param name="cb">Cb (0 - 255)</param>
        /// <returns>YCrCb</returns>
        /// <exception cref="ArgumentOutOfRangeException">Value out of range / 数值超出范围</exception>
        public static YCrCb From(int y, int cr, int cb)
        {
            CheckRange(y, cr, cb);
            return new YCrCb(y, cr, cb);
        }

        /// <summary>
        /// Parse out the YCrCb value from a string
        /// 从一个字符串解析出 YCrCb 值
        /// </summary>
        /// <param name="color">String of YCrCb / YCrCb 字符串</param>
        /// <returns>YCrCb</returns>
        /// <exception cref="ArgumentException">Unable to parse / 无法解析</exception>
        public static YCrCb FromString(string color)
        {
            var cm = Basic.ExtractFromStringAsInt(color);
            if (cm.Length != 3) throw new ArgumentException();
            CheckRange(cm[0], cm[1], cm[2]);
            return new YCrCb(cm[0], cm[1], cm[2]);
        }

        private static void CheckRange(int y, int cr, int cb)
        {
            if (y < 0 || y > 255 || cr < 0 || cr > 255 || cb < 0 || cb > 255)
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Represents CIE-Lab Color Model
    /// 表示 CIE-Lab 颜色模型
    /// </summary>
    public readonly struct Lab : IColorModel, IEquatable<Lab>
    {
        /// <summary>
        /// L (0 - 100)
        /// </summary>
        /// <returns></returns>
        public double L { get; }

        /// <summary>
        /// a (-128 - 127)
        /// </summary>
        /// <returns></returns>
        public double A { get; }

        /// <summary>
        /// b (-128 - 127)
        /// </summary>
        /// <returns></returns>
        public double B { get; }


        private Lab(double l, double a, double b)
        {
            L = l;
            A = a;
            B = b;
        }

        public bool Equals(Lab other)
        {
            return Basic.DecimalEquals(L, other.L) && Basic.DecimalEquals(A, other.A) &&
                   Basic.DecimalEquals(B, other.B);
        }

        public bool CompareWith(int l, int a, int b)
        {
            return Equals(new Lab(l, a, b));
        }

        public override bool Equals(object obj)
        {
            return obj is Lab other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = L.GetHashCode();
                hashCode = (hashCode * 397) ^ A.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Lab left, Lab right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Lab left, Lab right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"CIE-Lab: ({ToString(",")})";
        }

        public string ToString(string separator)
        {
            return string.Format("{0:0.##}{3}{1:0.##}{3}{2:0.##}", L, A, B, separator);
        }

        /// <summary>
        /// Create the CIE-Lab model
        /// 创建 CIE-Lab 模型
        /// </summary>
        /// <param name="l">l (0 - 100)</param>
        /// <param name="a">a (-128 - 127)</param>
        /// <param name="b">b (-128 - 127)</param>
        /// <returns>CIE-Lab</returns>
        /// <exception cref="ArgumentOutOfRangeException">Value out of range / 数值超出范围</exception>
        public static Lab From(double l, double a, double b)
        {
            CheckRange(l, a, b);
            return new Lab(l, a, b);
        }

        /// <summary>
        /// Parse out the CIE-Lab value from a string
        /// 从一个字符串解析出 CIE-Lab 值
        /// </summary>
        /// <param name="color">String of CIE-Lab / CIE-Lab 字符串</param>
        /// <returns>CIE-Lab</returns>
        public static Lab FromString(string color)
        {
            double[] cm = Basic.ExtractFromString(color);
            // 检查格式
            if (cm.Length != 3) throw new ArgumentException();
            double l = cm[0];
            double a = cm[1];
            double b = cm[2];
            CheckRange(l, a, b);
            return new Lab(l, a, b);
        }

        private static void CheckRange(double l, double a, double b)
        {
            if (l < 0d || l > 100d || a < -128d || a > 127d || b < -128d || b > 127d)
                throw new ArgumentOutOfRangeException();
        }
    }


    /// <summary>
    /// Represents the values of X, Y, Z in CIE-XYZ Color Space. Usually X, Y, Z ranges from about 0 to 1.0
    /// <para>表示 CIE-XYZ 颜色空间 X, Y, Z 的值。通常 X, Y, Z 的范围大约是 0 到 1.0</para>
    /// </summary>
    public readonly struct Xyz : IColorModel, IEquatable<Xyz>
    {
        /// <summary>
        /// Value of X
        /// X 的值
        /// </summary>
        /// <returns></returns>
        public double X { get; }

        /// <summary>
        /// Value of Y
        /// Y 的值
        /// </summary>
        /// <returns></returns>
        public double Y { get; }

        /// <summary>
        /// Value of Z
        /// Z 的值
        /// </summary>
        /// <returns></returns>
        public double Z { get; }

        private Xyz(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool CompareWith(double x, double y, double z)
        {
            return Equals(new Xyz(x, y, z));
        }

        public bool Equals(Xyz other)
        {
            return Basic.DecimalEquals(X, other.X) && Basic.DecimalEquals(Y, other.Y) &&
                   Basic.DecimalEquals(Z, other.Z);
        }

        public override bool Equals(object obj)
        {
            return obj is Xyz other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Xyz left, Xyz right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Xyz left, Xyz right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"CIE-XYZ: ({ToString(",")})";
        }

        public string ToString(string separator)
        {
            return string.Format("{0:0.#####}{3}{1:0.#####}{3}{2:0.#####}", X, Y, Z, separator);
        }

        /// <summary>
        /// Create the XYZ struct(No range limited)
        /// 创建 XYZ 结构(无范围限制)
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        /// <returns>CIE-XYZ</returns>
        public static Xyz From(double x, double y, double z)
        {
            return new Xyz(x, y, z);
        }

        /// <summary>
        /// Parse out the XYZ value from a string(No range limited)
        /// 从一个字符串解析出 XYZ 值(无范围限制)
        /// </summary>
        /// <param name="color">String of XYZ / XYZ 字符串</param>
        /// <returns>CIE-XYZ</returns>
        /// <exception cref="ArgumentException">Unable to parse / 无法解析</exception>
        public static Xyz FromString(string color)
        {
            double[] xyz = Basic.ExtractFromString(color);
            if (xyz.Length != 3) throw new ArgumentException();
            return new Xyz(xyz[0], xyz[1], xyz[2]);
        }
    }

    /// <summary>
    /// CIE-XYZ Color Space Helper
    /// CIE-XYZ 颜色空间辅助工具
    /// <seealso cref="Xyz"/>
    /// </summary>
    public static class CieXyzHelper
    {
        /// <summary>
        /// Check if X, Y, Z values are within the valid range of D65 illuminant
        /// 检查 X, Y, Z 的值是否在 D65 光源的有效范围内
        /// <para>X: 0 - 0.95047, Y: 0 - 1.0, Z: 0 - 1.08883</para>
        /// </summary>
        /// <param name="value">CIE-XYZ</param>
        /// <returns>true if in valid range / 在有效范围内则返回 true</returns>
        public static bool CheckRangeOfD65(Xyz value)
        {
            return !(value.X < 0d || value.X > 0.95047d || value.Y < 0d || value.Y > 1.0d ||
                     value.Z < 0d || value.Z > 1.08883d);
        }

        /// <summary>
        /// Parse out the XYZ value from a string, which within the valid range of D65 illuminant
        /// 从一个字符串解析出在 D65 光源有效范围内的 XYZ 值
        /// <para>X (0 - 0.95047), Y (0 - 1.0), Z (0 - 1.08883)</para>
        /// </summary>
        /// <param name="color">String of XYZ / XYZ 字符串</param>
        /// <returns>CIE-XYZ(D65)</returns>
        /// <exception cref="ArgumentException">Unable to parse / 无法解析</exception>
        /// <exception cref="ArgumentOutOfRangeException">Value out of range / 数值超出范围</exception>
        public static Xyz ParseStringOfD65(string color)
        {
            var value = Xyz.FromString(color);
            if (!CheckRangeOfD65(value)) throw new ArgumentOutOfRangeException();
            return value;
        }
    }
}