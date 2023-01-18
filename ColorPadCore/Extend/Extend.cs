using ColorPadCore.Core.Model;
using static ColorPadCore.Core.ModelsManager;

namespace ColorPadCore.Extend
{
    public enum ColorType
    {
        Rgb,
        Hex,
        Grayscale,
        Hsb,
        Hsl,
        Cmyk,
        YCrCb,
        Xyz,
        Lab
    }

    public interface IConvertBridge
    {
        Rgb Rgb { get; }

        Hsb Hsb { get; }

        Hsl Hsl { get; }

        Cmyk Cmyk { get; }

        YCrCb YCrCb { get; }

        Xyz Xyz { get; }

        Lab Lab { get; }

        byte Grayscale { get; }

        /// <summary>
        /// Create Hex / HTML string of RGB
        /// 生成 RGB 的 16进制 / HTML 格式字符串
        /// </summary>
        /// <param name="upper">Upper case / 大写</param>
        /// <returns>Hex of RGB</returns>
        string ToHex(bool upper = true);
    }

    /// <summary>
    /// 提供各个颜色模型相互转换的扩展类
    /// </summary>
    public class NormalConvertBridge : IConvertBridge
    {
        private readonly Rgb _rgb;

        public Rgb Rgb => _rgb;

        public Hsb Hsb { get; }
        public Hsl Hsl { get; }
        public Cmyk Cmyk { get; }
        public YCrCb YCrCb { get; }
        public Xyz Xyz { get; }
        public Lab Lab { get; }
        public byte Grayscale { get; }
        public string ToHex(bool upper = true) => _rgb.ToHex(upper);

        private NormalConvertBridge(Rgb model, ColorType avoidType)
        {
            _rgb = model;
            Grayscale = Convert<Rgb, Grayscale>(in model).Value;
            if (avoidType != ColorType.Hsb) Hsb = Convert<Rgb, Hsb>(in model);
            if (avoidType != ColorType.Hsl) Hsl = Convert<Rgb, Hsl>(in model);
            if (avoidType != ColorType.Cmyk) Cmyk = Convert<Rgb, Cmyk>(in model);
            if (avoidType != ColorType.YCrCb) YCrCb = Convert<Rgb, YCrCb>(in model);
            if (avoidType != ColorType.Xyz)
            {
                Xyz = Convert<Rgb, Xyz>(in model);
                Lab = Convert<Xyz, Lab>(Xyz);
            }
        }

        public NormalConvertBridge(Rgb rgb) : this(rgb, ColorType.Rgb)
        {
        }

        public NormalConvertBridge(Hsb hsb) : this(Convert<Hsb, Rgb>(in hsb), ColorType.Hsb) => Hsb = hsb;

        public NormalConvertBridge(Hsl hsl) : this(Convert<Hsl, Rgb>(in hsl), ColorType.Hsl) => Hsl = hsl;

        public NormalConvertBridge(Cmyk cmyk) : this(Convert<Cmyk, Rgb>(in cmyk), ColorType.Cmyk) => Cmyk = cmyk;

        public NormalConvertBridge(YCrCb yCrCb) : this(Convert<YCrCb, Rgb>(in yCrCb), ColorType.YCrCb)
            => YCrCb = yCrCb;

        public NormalConvertBridge(Xyz xyz) : this(Convert<Xyz, Rgb>(in xyz), ColorType.Xyz)
        {
            Xyz = xyz;
            Lab = Convert<Xyz, Lab>(Xyz);
        }

        public NormalConvertBridge(Lab lab)
        {
            Lab = lab;
            Xyz = Convert<Lab, Xyz>(in lab);
            _rgb = Convert<Xyz, Rgb>(Xyz);
            Grayscale = Convert<Rgb, Grayscale>(in _rgb).Value;
            Hsb = Convert<Rgb, Hsb>(in _rgb);
            Hsl = Convert<Rgb, Hsl>(in _rgb);
            Cmyk = Convert<Rgb, Cmyk>(in _rgb);
            YCrCb = Convert<Rgb, YCrCb>(in _rgb);
        }

        public NormalConvertBridge(IConvertBridge bridge)
        {
            _rgb = bridge.Rgb;
            Grayscale = bridge.Grayscale;
            Hsb = bridge.Hsb;
            Hsl = bridge.Hsl;
            Cmyk = bridge.Cmyk;
            YCrCb = bridge.YCrCb;
            Xyz = bridge.Xyz;
            Lab = bridge.Lab;
        }
    }

    /// <summary>
    /// 提供各个颜色模型互相转换的扩展类，转换时会尽量使用懒加载
    /// </summary>
    public class LazyConvertBridge : IConvertBridge
    {
        private readonly Rgb _rgb;
        private Hsb _hsb;
        private Hsl _hsl;
        private Cmyk _cmyk;
        private YCrCb _yCrCb;
        private Xyz _xyz;
        private Lab _lab;
        private bool _initHsb, _initHsl, _initCmyk, _initYCrCb, _initXyz, _initLab;

        /// <summary>
        /// 默认的空占位对象
        /// </summary>
        public static readonly LazyConvertBridge EmptyBridge = new LazyConvertBridge(Rgb.White);

        public Rgb Rgb => _rgb;

        public byte Grayscale { get; }

        public Hsb Hsb
        {
            get
            {
                if (_initHsb) return _hsb;
                _hsb = Convert<Rgb, Hsb>(in _rgb);
                _initHsb = true;
                return _hsb;
            }
        }

        public Hsl Hsl
        {
            get
            {
                if (_initHsl) return _hsl;
                _hsl = Convert<Rgb, Hsl>(in _rgb);
                _initHsl = true;
                return _hsl;
            }
        }

        public Cmyk Cmyk
        {
            get
            {
                if (_initCmyk) return _cmyk;
                _cmyk = Convert<Rgb, Cmyk>(in _rgb);
                _initCmyk = true;
                return _cmyk;
            }
        }

        public YCrCb YCrCb
        {
            get
            {
                if (_initYCrCb) return _yCrCb;
                _yCrCb = Convert<Rgb, YCrCb>(in _rgb);
                _initYCrCb = true;
                return _yCrCb;
            }
        }

        public Xyz Xyz
        {
            get
            {
                if (_initXyz) return _xyz;
                _xyz = Convert<Rgb, Xyz>(in _rgb);
                _initXyz = true;
                return _xyz;
            }
        }

        public Lab Lab
        {
            get
            {
                if (_initLab) return _lab;
                if (!_initXyz)
                {
                    _xyz = Convert<Rgb, Xyz>(in _rgb);
                    _initXyz = true;
                }

                _lab = Convert<Xyz, Lab>(Xyz);
                _initLab = true;
                return _lab;
            }
        }

        public string ToHex(bool upper = true) => _rgb.ToHex(upper);

        public LazyConvertBridge(Rgb model)
        {
            _rgb = model;
            Grayscale = GetGray();
        }

        public LazyConvertBridge(Hsb model)
        {
            _hsb = model;
            _initHsb = true;
            _rgb = Convert<Hsb, Rgb>(in model);
            Grayscale = GetGray();
        }

        public LazyConvertBridge(Hsl model)
        {
            _hsl = model;
            _initHsl = true;
            _rgb = Convert<Hsl, Rgb>(in model);
            Grayscale = GetGray();
        }

        public LazyConvertBridge(Cmyk model)
        {
            _cmyk = model;
            _initCmyk = true;
            _rgb = Convert<Cmyk, Rgb>(in model);
            Grayscale = GetGray();
        }

        public LazyConvertBridge(YCrCb model)
        {
            _yCrCb = model;
            _initYCrCb = true;
            _rgb = Convert<YCrCb, Rgb>(in model);
            Grayscale = GetGray();
        }

        public LazyConvertBridge(Xyz model)
        {
            _xyz = model;
            _initXyz = true;
            _rgb = Convert<Xyz, Rgb>(in model);
            Grayscale = GetGray();
        }

        public LazyConvertBridge(Lab model)
        {
            _lab = model;
            _initLab = true;
            _xyz = Convert<Lab, Xyz>(in model);
            _initXyz = true;
            _rgb = Convert<Xyz, Rgb>(in _xyz);
            Grayscale = GetGray();
        }

        public LazyConvertBridge(LazyConvertBridge l)
        {
            _rgb = l.Rgb;
            Grayscale = l.Grayscale;
            if (l._initHsb)
            {
                _hsb = l._hsb;
                _initHsb = true;
            }

            if (l._initHsl)
            {
                _hsl = l._hsl;
                _initHsl = true;
            }

            if (l._initCmyk)
            {
                _cmyk = l._cmyk;
                _initCmyk = true;
            }

            if (l._initYCrCb)
            {
                _yCrCb = l._yCrCb;
                _initYCrCb = true;
            }

            if (l._initXyz)
            {
                _xyz = l._xyz;
                _initXyz = true;
            }

            if (l._initLab)
            {
                _lab = l._lab;
                _initLab = true;
            }
        }

        public LazyConvertBridge(IConvertBridge bridge)
        {
            _rgb = bridge.Rgb;
            Grayscale = bridge.Grayscale;
            _hsb = bridge.Hsb;
            _initHsb = true;
            _hsl = bridge.Hsl;
            _initHsl = true;
            _cmyk = bridge.Cmyk;
            _initCmyk = true;
            _yCrCb = bridge.YCrCb;
            _initYCrCb = true;
            _xyz = bridge.Xyz;
            _initXyz = true;
            _lab = bridge.Lab;
            _initLab = true;
        }

        public static LazyConvertBridge FromRgb(int r, int g, int b)
        {
            return new LazyConvertBridge(Rgb.From(r, g, b));
        }

        private byte GetGray() => Convert<Rgb, Grayscale>(in _rgb).Value;
    }
}