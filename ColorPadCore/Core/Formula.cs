using System;
using ColorPadCore.Core.Model;

namespace ColorPadCore.Core
{
    /// <summary>
    /// The Type of Color Formula
    /// 表示配色方案的类型
    /// </summary>
    public enum FormulaType
    {
        /// <summary>
        /// 同类色
        /// </summary>
        Monochromatic,

        /// <summary>
        /// 互补色
        /// </summary>
        Complementary,

        /// <summary>
        /// 分裂互补色
        /// </summary>
        SplitComplementary,

        /// <summary>
        /// 邻近色
        /// </summary>
        Analogous,

        /// <summary>
        /// 三角色
        /// </summary>
        Triadic,

        /// <summary>
        /// 四角色
        /// </summary>
        Tetradic
    }

    public static class Formula
    {
        /// <summary>
        /// 以同类色计算色相
        /// </summary>
        /// <param name="hue">Hue</param>
        /// <returns></returns>
        public static double[] Monochromatic(double hue)
        {
            return new[] { hue };
        }

        /// <summary>
        /// 以互补色计算色相
        /// </summary>
        /// <param name="hue">Hue</param>
        /// <returns></returns>
        public static double[] Complementary(double hue)
        {
            return new[] { hue, (hue + 180d) % 360d };
        }

        /// <summary>
        /// 以分裂互补色计算色相（夹角一般取 90 - 180）
        /// </summary>
        /// <param name="hue">Hue</param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double[] SplitComplementary(double hue, double angle = 150d)
        {
            return new[] { hue, (hue + angle) % 360d, (hue + 360d - angle) % 360d };
        }

        /// <summary>
        /// 以临近色计算色相（夹角一般取 0 - 90）
        /// </summary>
        /// <param name="hue">Hue</param>
        /// <param name="angle">主色调和第二色调的色相角度差值</param>
        /// <returns></returns>
        public static double[] Analogous(double hue, double angle = 60d)
        {
            return new[] { hue, (hue + angle) % 360d, (hue + 360d - angle) % 360d };
        }

        /// <summary>
        /// 以三角色计算色相
        /// </summary>
        /// <param name="hue">Hue</param>
        /// <returns></returns>
        public static double[] Triadic(double hue)
        {
            return new[] { hue, (hue + 120d) % 360d, (hue + 240d) % 360d };
        }

        /// <summary>
        /// 以四角色计算色相（夹角一般取 0 - 90）
        /// </summary>
        /// <param name="hue">Hue</param>
        /// <param name="angle">主色调和第二色调的色相角度差值</param>
        /// <returns></returns>
        public static double[] Tetradic(double hue, double angle = 60d)
        {
            return new[] { hue, (hue + angle) % 360d, (hue + 180d) % 360d, (hue + 180d + angle) % 360d };
        }

        /// <summary>
        /// 按照指定的方式计算颜色方案
        /// </summary>
        /// <param name="hue">色相</param>
        /// <param name="type">计算方式</param>
        /// <param name="angle">主色调和第二色调的色相角度差值（仅对部分方案有效）</param>
        /// <returns></returns>
        public static double[] GetFormula(double hue, FormulaType type, double? angle = null)
        {
            switch (type)
            {
                case FormulaType.Monochromatic:
                    return Monochromatic(hue);
                case FormulaType.Complementary:
                    return Complementary(hue);
                case FormulaType.SplitComplementary:
                    return angle.HasValue
                        ? SplitComplementary(hue, Basic.GetFixRange(angle.Value, 90, 179.9))
                        : SplitComplementary(hue);
                case FormulaType.Analogous:
                    return angle.HasValue
                        ? Analogous(hue, Basic.GetFixRange(angle.Value, 1d, 90d))
                        : Analogous(hue);
                case FormulaType.Triadic:
                    return Triadic(hue);
                case FormulaType.Tetradic:
                    return angle.HasValue
                        ? Tetradic(hue, Basic.GetFixRange(angle.Value, 1d, 90d))
                        : Tetradic(hue);
                default:
                    return Array.Empty<double>();
            }
        }

        /// <summary>
        /// 按照指定的方式计算颜色方案
        /// </summary>
        /// <param name="hsb">HSB 对象</param>
        /// <param name="type">计算方式</param>
        /// <param name="angle">主色调和第二色调的色相角度差值（仅对部分方案有效）</param>
        /// <returns></returns>
        public static Hsb[] GetFormula(in Hsb hsb, FormulaType type, double? angle = null)
        {
            double[] hues = GetFormula(hsb.H, type, angle);
            var models = new Hsb[hues.Length];
            for (int i = 0; i < hues.Length; i++) models[i] = Hsb.From(hues[i], hsb.S, hsb.B);
            return models;
        }
    }
}