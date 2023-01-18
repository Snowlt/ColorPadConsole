using System;
using ColorPadCore.Core.Model;

/*
== 管理颜色模型 / 空间转换算法的部分 ==
*/
namespace ColorPadCore.Core.Converter
{
    public interface IConvertFromTo<TSource, out TTarget>
    {
        TTarget Convert(in TSource source);
    }

    /// <summary>
    /// Calculate Grayscale of RGB by sRGB space component algorithm
    /// 使用 sRGB 分量方式计算 RGB 的灰度值
    /// </summary>
    public class GraySpaceComponentAlgorithm : IConvertFromTo<Rgb, Grayscale>
    {
        public Grayscale Convert(in Rgb rgb) =>
            Grayscale.From((rgb.R * 299 + rgb.G * 587 + rgb.B * 114 + 500) / 1000);
    }

    /// <summary>
    /// Calculate Grayscale of RGB by means of average value
    /// 使用平均值方式计算 RGB 的灰度值
    /// </summary>
    public class GrayAverageAlgorithm : IConvertFromTo<Rgb, Grayscale>
    {
        public Grayscale Convert(in Rgb rgb) =>
            Grayscale.From(((rgb.R + rgb.G + rgb.B) * 10 / 3 + 5) / 10);
    }

    /// <summary>
    /// Default implementation of color model conversion algorithm. Use <see cref="Instance"/> to get instance.
    /// <para>颜色模型转换算法的默认实现。使用 <see cref="Instance"/> 获取实例。</para>  
    /// <seealso cref="IConvertFromTo{TSource,TTarget}"/>
    /// </summary>
    public class DefaultModelConverters : IConvertFromTo<Grayscale, Rgb>,
        IConvertFromTo<Rgb, Hsb>, IConvertFromTo<Hsb, Rgb>,
        IConvertFromTo<Rgb, Cmyk>, IConvertFromTo<Cmyk, Rgb>,
        IConvertFromTo<Rgb, Hsl>, IConvertFromTo<Hsl, Rgb>,
        IConvertFromTo<Rgb, YCrCb>, IConvertFromTo<YCrCb, Rgb>,
        IConvertFromTo<Rgb, Xyz>, IConvertFromTo<Xyz, Rgb>,
        IConvertFromTo<Xyz, Lab>, IConvertFromTo<Lab, Xyz>
    {
        public static readonly DefaultModelConverters Instance = new DefaultModelConverters();

        // Grayscale -> RGB
        public Rgb Convert(in Grayscale source) => source.ToRgb();

        // HSB - RGB
        Hsb IConvertFromTo<Rgb, Hsb>.Convert(in Rgb rgb)
        {
            var max = Math.Max(Math.Max(rgb.R, rgb.G), rgb.B);
            var min = Math.Min(Math.Min(rgb.R, rgb.G), rgb.B);
            double v = max / 255d;
            double s = max != 0 ? (max - min) / (double)max : 0d;
            double h;
            if (Basic.DecimalEquals(s, 0d)) h = 0d;
            else
            {
                if (max == rgb.R)
                {
                    h = 60 * (rgb.G - rgb.B) / (double)(max - min);
                    if (h < 0d) h += 360d;
                }
                else if (max == rgb.G) h = 120d + 60 * (rgb.B - rgb.R) / (double)(max - min);
                // equals to case: max == rgb.B
                else h = 240d + 60 * (rgb.R - rgb.G) / (double)(max - min);
            }

            return Hsb.From(h, s * 100d, v * 100d);
        }

        public Rgb Convert(in Hsb hsb)
        {
            double r = 0, g = 0, b = 0;
            double h = hsb.H % 360d;
            double s = hsb.S / 100d;
            double v = hsb.B / 100d;
            int i = (int)(h / 60d) % 6;
            double f = h / 60d - i;
            double p = v * (1d - s);
            double q = v * (1d - f * s);
            double t = v * (1d - (1d - f) * s);
            switch (i)
            {
                case 0:
                    {
                        r = v;
                        g = t;
                        b = p;
                        break;
                    }
                case 1:
                    {
                        r = q;
                        g = v;
                        b = p;
                        break;
                    }
                case 2:
                    {
                        r = p;
                        g = v;
                        b = t;
                        break;
                    }
                case 3:
                    {
                        r = p;
                        g = q;
                        b = v;
                        break;
                    }
                case 4:
                    {
                        r = t;
                        g = p;
                        b = v;
                        break;
                    }
                case 5:
                    {
                        r = v;
                        g = p;
                        b = q;
                        break;
                    }
            }

            return Rgb.From(Basic.FRound(r * 255), Basic.FRound(g * 255), Basic.FRound(b * 255));
        }

        // HSL - RGB
        Hsl IConvertFromTo<Rgb, Hsl>.Convert(in Rgb rgb)
        {
            var max = Math.Max(Math.Max(rgb.R, rgb.G), rgb.B);
            var min = Math.Min(Math.Min(rgb.R, rgb.G), rgb.B);
            double l = (max + min) / 255d / 2d;
            double h, s;
            if (max == min || Basic.DecimalEquals(l, 0d)) s = 0d;
            else if (l <= 0.5) s = (max - min) / (double)(max + min);
            else s = (max - min) / (double)(510 - (max + min));
            if (max == min)
                h = 0d;
            else
            {
                if (max == rgb.R)
                {
                    h = 60 * (rgb.G - rgb.B) / (double)(max - min);
                    if (h < 0d) h += 360d;
                }
                else if (max == rgb.G) h = 120d + 60 * (rgb.B - rgb.R) / (double)(max - min);
                // equals to case: max == rgb.B
                else h = 240 + 60 * (rgb.R - rgb.G) / (double)(max - min);
            }

            return Hsl.From(h, s * 100d, l * 100d);
        }

        public Rgb Convert(in Hsl hsl)
        {
            double hue = hsl.H;
            double saturation = hsl.S;
            double lightness = hsl.L;
            if (Basic.DecimalEquals(saturation, 0d))
            {
                var g = Basic.FRound(lightness * 255d / 100d);
                return Rgb.From(g, g, g);
            }

            double q = lightness <= 50d
                ? lightness * (100d + saturation) / 10000d
                : (lightness + saturation) / 100d - lightness * saturation / 10000d;
            double p = 2d * lightness / 100d - q;
            int tr = Basic.FRound(hue + 120);
            int tg = Basic.FRound(hue);
            int tb = Basic.FRound(hue - 120);

            int Calc(int v)
            {
                if (v < 0) v += 360;
                else if (v > 360) v -= 360;
                double sv;
                if (v < 60) sv = p + (q - p) * (6 * v / 360d);
                else if (v < 180) sv = q;
                else if (v < 240) sv = p + (q - p) * (6 * (240 - v) / 360d);
                else sv = p;
                return Basic.FRound(sv * 255d);
            }

            return Rgb.From(Calc(tr), Calc(tg), Calc(tb));
        }

        // CMYK - RGB
        Cmyk IConvertFromTo<Rgb, Cmyk>.Convert(in Rgb rgb)
        {
            // RGB转CMYK
            int c = 255 - rgb.R;
            int m = 255 - rgb.G;
            int y = 255 - rgb.B;
            int k = Math.Min(Math.Min(c, m), y);
            // CMYK色彩修正
            if (k == 255)
            {
                c = Basic.FRound(c / 255d * 100d);
                m = Basic.FRound(m / 255d * 100d);
                y = Basic.FRound(y / 255d * 100d);
                k = 100;
            }
            else
            {
                c = Basic.FRound((c - k) / (double)(255 - k) * 100d);
                m = Basic.FRound((m - k) / (double)(255 - k) * 100d);
                y = Basic.FRound((y - k) / (double)(255 - k) * 100d);
                k = Basic.FRound(k / 255d * 100d);
            }

            return Cmyk.From(c, m, y, k);
        }

        public Rgb Convert(in Cmyk cmyk)
        {
            var r = Basic.FRound(225 * (100 - cmyk.C) * (100 - cmyk.K) / 10000d);
            var g = Basic.FRound(225 * (100 - cmyk.M) * (100 - cmyk.K) / 10000d);
            var b = Basic.FRound(225 * (100 - cmyk.Y) * (100 - cmyk.K) / 10000d);
            return Rgb.From(r, g, b);
        }

        // YCrCb - RGB
        YCrCb IConvertFromTo<Rgb, YCrCb>.Convert(in Rgb rgb)
        {
            const int delta = 128;
            // use int to accelerate
            var y = (rgb.R * 299 + rgb.G * 587 + rgb.B * 114 + 500) / 1000;
            var cr = (500000 * rgb.R - 418688 * rgb.G - 81312 * rgb.B + 500000) / 1000000 + delta;
            var cb = (-168736 * rgb.R - 331264 * rgb.G + 500000 * rgb.B + 500000) / 1000000 + delta;
            return YCrCb.From(y, Basic.GetFixRange(cr, 0, 255), Basic.GetFixRange(cb, 0, 255));
        }

        public Rgb Convert(in YCrCb yCrCb)
        {
            const int delta = 128;
            var r = yCrCb.Y + 1.402d * (yCrCb.Cr - delta);
            var g = yCrCb.Y - 0.344136d * (yCrCb.Cb - delta) - 0.714136d * (yCrCb.Cr - delta);
            var b = yCrCb.Y + 1.772d * (yCrCb.Cb - delta);
            return Rgb.From(Basic.GetFixRange(Basic.FRound(r), 0, 255),
                Basic.GetFixRange(Basic.FRound(g), 0, 255),
                Basic.GetFixRange(Basic.FRound(b), 0, 255));
        }

        // XYZ - RGB
        Xyz IConvertFromTo<Rgb, Xyz>.Convert(in Rgb rgb)
        {
            // Observer = 2°, Illuminant = D65
            // Gamma calculation for RGB
            // Original Gamma formula:
            // n > 0.04045 ? (n + 0.055) / 1.055 ^ 2.4 : n / 12.92
            double cR = rgb.R > 10 ? Math.Pow((rgb.R / 255d + 0.055) / 1.055, 2.4) : rgb.R * 10 / 32946d;
            double cG = rgb.G > 10 ? Math.Pow((rgb.G / 255d + 0.055) / 1.055, 2.4) : rgb.G * 10 / 32946d;
            double cB = rgb.B > 10 ? Math.Pow((rgb.B / 255d + 0.055) / 1.055, 2.4) : rgb.B * 10 / 32946d;
            // XYZ calculation
            var x = cR * 0.4124 + cG * 0.3576 + cB * 0.1805;
            var y = cR * 0.2126 + cG * 0.7152 + cB * 0.0722;
            var z = cR * 0.0193 + cG * 0.1192 + cB * 0.9505;
            return Xyz.From(x, y, z);
        }

        Rgb IConvertFromTo<Xyz, Rgb>.Convert(in Xyz xyz)
        {
            // Observer = 2°, Illuminant = D65
            double cR = xyz.X * 3.2406d - xyz.Y * 1.5372d - xyz.Z * 0.4986d;
            double cG = xyz.X * -0.9689d + xyz.Y * 1.8758d + xyz.Z * 0.0415d;
            double cB = xyz.X * 0.0557d - xyz.Y * 0.204d + xyz.Z * 1.057d;
            // Reverse Gamma calculation
            if (cR > 0.0031308) cR = Math.Pow(cR, 0.4166667) * 1.055 - 0.055;
            else cR *= 12.92;

            if (cG > 0.0031308) cG = Math.Pow(cG, 0.4166667) * 1.055 - 0.055;
            else cG *= 12.92;

            if (cB > 0.0031308) cB = Math.Pow(cB, 0.4166667) * 1.055 - 0.055;
            else cB *= 12.92;

            return Rgb.From(Basic.GetFixRange(Basic.FRound(cR * 255), 0, 255),
                Basic.GetFixRange(Basic.FRound(cG * 255), 0, 255),
                Basic.GetFixRange(Basic.FRound(cB * 255), 0, 255));
        }

        // CIE-Lab - XYZ
        Lab IConvertFromTo<Xyz, Lab>.Convert(in Xyz xyz)
        {
            double x = xyz.X / 0.950456;
            double y = xyz.Y;
            double z = xyz.Z / 1.088754;
            double fX = x > 0.008856 ? Math.Pow(x, 0.333333) : 7.787 * x + 0.137931;
            double fY = y > 0.008856 ? Math.Pow(y, 0.333333) : 7.787 * y + 0.137931;
            double fZ = z > 0.008856 ? Math.Pow(z, 0.333333) : 7.787 * z + 0.137931;
            // Calculate CIE-Lab
            double cL = y > 0.008856 ? 116 * fY - 16 : 903.3 * y;
            double cA = 500d * (fX - fY);
            double cB = 200d * (fY - fZ);
            return Lab.From(cL, cA, cB);
        }

        public Xyz Convert(in Lab lab)
        {
            double l = lab.L;
            double a = lab.A;
            double b = lab.B;
            double y, fY;
            // Y and f(Y)
            if (l > 7.99959)
            {
                // Calculate f(Y) first
                fY = (l + 16d) / 116d;
                y = fY > 0.2068927 ? Math.Pow(fY, 3) : (fY - 0.137931) / 7.787;
            }
            else
            {
                // Calculate Y first
                y = l / 903.3;
                fY = y > 0.008856 ? Math.Pow(y, 0.333333) : 7.787 * y + 0.137931;
            }

            // f(X) and f(Z)
            double fX = a / 500d + fY;
            double fZ = fY - b / 200d;
            // X and Z
            double x = fX > 0.2068927 ? Math.Pow(fX, 3) : (fX - 0.137931) / 7.787;
            double z = fZ > 0.2068927 ? Math.Pow(fZ, 3) : (fZ - 0.137931) / 7.787;
            x *= 0.950456;
            z *= 1.088754;
            return Xyz.From(x, y, z);
        }
    }
}