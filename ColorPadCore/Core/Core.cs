using System;
using System.Collections.Generic;
using ColorPadCore.Core.Converter;
using ColorPadCore.Core.Model;

/*
== 管理颜色模型的部分 ==
*/
namespace ColorPadCore.Core
{
    /// <summary>
    /// Entry of process color models conversion and calculation
    /// 处理颜色模型转换和计算的入口
    /// </summary>
    public static class ModelsManager
    {
        private static readonly Dictionary<Type, object> RegisteredConverters = new Dictionary<Type, object>();

        /// <summary>
        /// Register a convert method (converter) to convert from one color model to another
        /// 注册一个转换方法(转换器)，用于将一个颜色模型转换为另一个颜色模型
        /// </summary>
        /// <typeparam name="TSource">Source type / 源类型</typeparam>
        /// <typeparam name="TTarget">Target type / 目标类型</typeparam>
        /// <param name="convertMethod">Convert method / 转换方法</param>
        public static void Register<TSource, TTarget>(IConvertFromTo<TSource, TTarget> convertMethod)
        {
            if (convertMethod is null)
                throw new ArgumentNullException(nameof(convertMethod));
            lock (RegisteredConverters)
                RegisteredConverters[typeof(IConvertFromTo<TSource, TTarget>)] = convertMethod;
        }

        /// <summary>
        /// Convert from source color model to target color model
        /// 将源颜色模型转换到目标颜色模型
        /// </summary>
        /// <typeparam name="TSource">Source type / 源类型</typeparam>
        /// <typeparam name="TTarget">Target type / 目标类型</typeparam>
        /// <param name="source">Source / 源</param>
        /// <returns>Color model after converted / 转换后的颜色模型</returns>
        /// <exception cref="ArgumentException">Throw when no suitable converter found / 没有找到可用的转换器时抛出</exception>
        public static TTarget Convert<TSource, TTarget>(in TSource source)
        {
            try
            {
                var converter =
                    (IConvertFromTo<TSource, TTarget>) RegisteredConverters[typeof(IConvertFromTo<TSource, TTarget>)];
                return converter.Convert(in source);
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"No Converter found for from {typeof(TSource)} to {typeof(TTarget)}");
            }
        }

        /// <summary>
        /// Check if is possible to convert from one type to another type
        /// 检查是否能将一个类型转为另一个类型
        /// </summary>
        /// <typeparam name="TSource">Source type / 源类型</typeparam>
        /// <typeparam name="TTarget">Target type / 目标类型</typeparam>
        /// <returns>true if possible / 能转换返回 true</returns>
        public static bool IsConvertable<TSource, TTarget>()
        {
            return RegisteredConverters.ContainsKey(typeof(IConvertFromTo<TSource, TTarget>));
        }

        static ModelsManager()
        {
            // 注册默认的转换算法
            Register(new GraySpaceComponentAlgorithm());
            Register<Grayscale, Rgb>(DefaultModelConverters.Instance);
            Register<Rgb, Hsb>(DefaultModelConverters.Instance);
            Register<Hsb, Rgb>(DefaultModelConverters.Instance);
            Register<Rgb, Hsl>(DefaultModelConverters.Instance);
            Register<Hsl, Rgb>(DefaultModelConverters.Instance);
            Register<Rgb, Cmyk>(DefaultModelConverters.Instance);
            Register<Cmyk, Rgb>(DefaultModelConverters.Instance);
            Register<Rgb, YCrCb>(DefaultModelConverters.Instance);
            Register<YCrCb, Rgb>(DefaultModelConverters.Instance);
            Register<Xyz, Lab>(DefaultModelConverters.Instance);
            Register<Lab, Xyz>(DefaultModelConverters.Instance);
            Register<Rgb, Xyz>(DefaultModelConverters.Instance);
            Register<Xyz, Rgb>(DefaultModelConverters.Instance);
        }
    }

    internal static class Basic
    {
        public static int FRound(double num) => (int) (num + 0.5);

        public static double GetFixRange(double value, double lower, double upper)
        {
            if (value > upper) return upper;
            return value < lower ? lower : value;
        }

        public static int GetFixRange(int value, int lower, int upper)
        {
            if (value > upper) return upper;
            return value < lower ? lower : value;
        }

        public static bool DecimalEquals(double left, double right)
        {
            return Math.Abs(left - right) < 0.000001;
        }

        /// <summary>
        /// Extract color value from a string.
        /// Return empty array when failed to parse, format incorrect or range incorrect.
        /// 从字符串提取颜色值。在解析失败，格式错误或范围错误时返回空数组。
        /// </summary>
        /// <param name="color">string</param>
        /// <returns>double[]</returns>
        public static double[] ExtractFromString(string color)
        {
            if (string.IsNullOrEmpty(color)) return Array.Empty<double>();
            var split = color.Split(',');
            var parsed = new double[split.Length];
            try
            {
                for (var i = 0; i < split.Length; i++)
                {
                    parsed[i] = double.Parse(split[i]);
                }
            }
            catch (Exception)
            {
                return Array.Empty<double>();
            }

            return parsed;
        }

        public static int[] ExtractFromStringAsInt(string color)
        {
            if (string.IsNullOrEmpty(color)) return Array.Empty<int>();
            var split = color.Split(',');
            var parsed = new int[split.Length];
            try
            {
                for (var i = 0; i < split.Length; i++)
                {
                    parsed[i] = FRound(double.Parse(split[i]));
                }
            }
            catch (Exception)
            {
                return Array.Empty<int>();
            }

            return parsed;
        }
    }
}