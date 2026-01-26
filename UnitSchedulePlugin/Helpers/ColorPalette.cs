using System.Collections.Generic;
using System.Windows.Media;

namespace UnitSchedulePlugin.Helpers
{
    /// <summary>
    /// Professional color palette for the plugin UI and charts.
    /// Defines consistent colors across the application.
    ///
    /// 插件 UI 和图表的专业配色方案。
    /// 在整个应用程序中定义一致的颜色。
    /// </summary>
    public static class ColorPalette
    {
        #region Primary Colors | 主色

        /// <summary>
        /// Deep Blue - Primary brand color (#1E3A8A).
        /// 深蓝色 - 主品牌色。
        /// </summary>
        public static Color DeepBlue => Color.FromRgb(30, 58, 138);

        /// <summary>
        /// Deep Blue hex string for XAML.
        /// 用于 XAML 的深蓝色十六进制字符串。
        /// </summary>
        public const string DeepBlueHex = "#1E3A8A";

        /// <summary>
        /// Teal - Secondary accent color (#14B8A6).
        /// 青色 - 次要强调色。
        /// </summary>
        public static Color Teal => Color.FromRgb(20, 184, 166);

        /// <summary>
        /// Teal hex string for XAML.
        /// 用于 XAML 的青色十六进制字符串。
        /// </summary>
        public const string TealHex = "#14B8A6";

        /// <summary>
        /// Amber - Highlight/warning color (#F59E0B).
        /// 琥珀色 - 高亮/警告色。
        /// </summary>
        public static Color Amber => Color.FromRgb(245, 158, 11);

        /// <summary>
        /// Amber hex string for XAML.
        /// 用于 XAML 的琥珀色十六进制字符串。
        /// </summary>
        public const string AmberHex = "#F59E0B";

        #endregion

        #region Extended Chart Colors | 扩展图表颜色

        /// <summary>
        /// Full chart color palette for multiple series.
        /// 用于多个系列的完整图表配色板。
        /// </summary>
        public static readonly List<Color> ChartColors = new List<Color>
        {
            Color.FromRgb(30, 58, 138),   // Deep Blue | 深蓝色
            Color.FromRgb(20, 184, 166),  // Teal | 青色
            Color.FromRgb(245, 158, 11),  // Amber | 琥珀色
            Color.FromRgb(99, 102, 241),  // Indigo | 靛蓝色
            Color.FromRgb(236, 72, 153),  // Pink | 粉色
            Color.FromRgb(34, 197, 94),   // Green | 绿色
            Color.FromRgb(168, 85, 247),  // Purple | 紫色
            Color.FromRgb(239, 68, 68),   // Red | 红色
            Color.FromRgb(14, 165, 233),  // Sky Blue | 天蓝色
            Color.FromRgb(251, 146, 60),  // Orange | 橙色
        };

        /// <summary>
        /// Hex strings for chart colors.
        /// 图表颜色的十六进制字符串。
        /// </summary>
        public static readonly List<string> ChartColorHexes = new List<string>
        {
            "#1E3A8A", // Deep Blue
            "#14B8A6", // Teal
            "#F59E0B", // Amber
            "#6366F1", // Indigo
            "#EC4899", // Pink
            "#22C55E", // Green
            "#A855F7", // Purple
            "#EF4444", // Red
            "#0EA5E9", // Sky Blue
            "#FB923C", // Orange
        };

        #endregion

        #region UI Colors | UI 颜色

        /// <summary>
        /// Background color for main window.
        /// 主窗口的背景色。
        /// </summary>
        public static Color WindowBackground => Color.FromRgb(249, 250, 251);
        public const string WindowBackgroundHex = "#F9FAFB";

        /// <summary>
        /// Card/panel background color.
        /// 卡片/面板的背景色。
        /// </summary>
        public static Color CardBackground => Color.FromRgb(255, 255, 255);
        public const string CardBackgroundHex = "#FFFFFF";

        /// <summary>
        /// Border color for cards and panels.
        /// 卡片和面板的边框颜色。
        /// </summary>
        public static Color BorderColor => Color.FromRgb(229, 231, 235);
        public const string BorderColorHex = "#E5E7EB";

        /// <summary>
        /// Primary text color.
        /// 主要文本颜色。
        /// </summary>
        public static Color TextPrimary => Color.FromRgb(17, 24, 39);
        public const string TextPrimaryHex = "#111827";

        /// <summary>
        /// Secondary text color.
        /// 次要文本颜色。
        /// </summary>
        public static Color TextSecondary => Color.FromRgb(107, 114, 128);
        public const string TextSecondaryHex = "#6B7280";

        /// <summary>
        /// Light text color (for dark backgrounds).
        /// 浅色文本（用于深色背景）。
        /// </summary>
        public static Color TextLight => Color.FromRgb(255, 255, 255);
        public const string TextLightHex = "#FFFFFF";

        /// <summary>
        /// Hover state color.
        /// 悬停状态颜色。
        /// </summary>
        public static Color HoverColor => Color.FromRgb(243, 244, 246);
        public const string HoverColorHex = "#F3F4F6";

        /// <summary>
        /// Selected/active state color.
        /// 选中/活动状态颜色。
        /// </summary>
        public static Color SelectedColor => Color.FromRgb(219, 234, 254);
        public const string SelectedColorHex = "#DBEAFE";

        #endregion

        #region Status Colors | 状态颜色

        /// <summary>
        /// Success/positive color.
        /// 成功/积极颜色。
        /// </summary>
        public static Color Success => Color.FromRgb(34, 197, 94);
        public const string SuccessHex = "#22C55E";

        /// <summary>
        /// Warning color.
        /// 警告颜色。
        /// </summary>
        public static Color Warning => Color.FromRgb(245, 158, 11);
        public const string WarningHex = "#F59E0B";

        /// <summary>
        /// Error/danger color.
        /// 错误/危险颜色。
        /// </summary>
        public static Color Error => Color.FromRgb(239, 68, 68);
        public const string ErrorHex = "#EF4444";

        /// <summary>
        /// Info color.
        /// 信息颜色。
        /// </summary>
        public static Color Info => Color.FromRgb(59, 130, 246);
        public const string InfoHex = "#3B82F6";

        #endregion

        #region Gradient Definitions | 渐变定义

        /// <summary>
        /// Creates a linear gradient brush from Deep Blue to Teal.
        /// 创建从深蓝色到青色的线性渐变画刷。
        /// </summary>
        public static LinearGradientBrush PrimaryGradient
        {
            get
            {
                var brush = new LinearGradientBrush();
                brush.StartPoint = new System.Windows.Point(0, 0);
                brush.EndPoint = new System.Windows.Point(1, 1);
                brush.GradientStops.Add(new GradientStop(DeepBlue, 0));
                brush.GradientStops.Add(new GradientStop(Teal, 1));
                return brush;
            }
        }

        /// <summary>
        /// Creates a header gradient brush.
        /// 创建标题渐变画刷。
        /// </summary>
        public static LinearGradientBrush HeaderGradient
        {
            get
            {
                var brush = new LinearGradientBrush();
                brush.StartPoint = new System.Windows.Point(0, 0);
                brush.EndPoint = new System.Windows.Point(1, 0);
                brush.GradientStops.Add(new GradientStop(DeepBlue, 0));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(37, 99, 235), 1));
                return brush;
            }
        }

        #endregion

        #region Helper Methods | 辅助方法

        /// <summary>
        /// Gets a chart color by index (cycles through palette).
        /// 按索引获取图表颜色（循环遍历调色板）。
        /// </summary>
        /// <param name="index">Color index | 颜色索引</param>
        /// <returns>Color from palette | 调色板中的颜色</returns>
        public static Color GetChartColor(int index)
        {
            return ChartColors[index % ChartColors.Count];
        }

        /// <summary>
        /// Gets a chart color hex by index (cycles through palette).
        /// 按索引获取图表颜色十六进制值（循环遍历调色板）。
        /// </summary>
        /// <param name="index">Color index | 颜色索引</param>
        /// <returns>Hex string from palette | 调色板中的十六进制字符串</returns>
        public static string GetChartColorHex(int index)
        {
            return ChartColorHexes[index % ChartColorHexes.Count];
        }

        /// <summary>
        /// Creates a SolidColorBrush from a Color.
        /// 从颜色创建 SolidColorBrush。
        /// </summary>
        /// <param name="color">Source color | 源颜色</param>
        /// <returns>SolidColorBrush</returns>
        public static SolidColorBrush ToBrush(Color color)
        {
            return new SolidColorBrush(color);
        }

        /// <summary>
        /// Converts hex string to Color.
        /// 将十六进制字符串转换为颜色。
        /// </summary>
        /// <param name="hex">Hex color string (e.g., "#1E3A8A") | 十六进制颜色字符串</param>
        /// <returns>Color object | 颜色对象</returns>
        public static Color FromHex(string hex)
        {
            hex = hex.TrimStart('#');
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return Color.FromRgb(r, g, b);
        }

        /// <summary>
        /// Creates a lighter version of a color.
        /// 创建颜色的较浅版本。
        /// </summary>
        /// <param name="color">Original color | 原始颜色</param>
        /// <param name="factor">Lightening factor (0-1) | 变浅因子（0-1）</param>
        /// <returns>Lightened color | 变浅后的颜色</returns>
        public static Color Lighten(Color color, double factor = 0.3)
        {
            return Color.FromRgb(
                (byte)(color.R + (255 - color.R) * factor),
                (byte)(color.G + (255 - color.G) * factor),
                (byte)(color.B + (255 - color.B) * factor));
        }

        /// <summary>
        /// Creates a darker version of a color.
        /// 创建颜色的较深版本。
        /// </summary>
        /// <param name="color">Original color | 原始颜色</param>
        /// <param name="factor">Darkening factor (0-1) | 变深因子（0-1）</param>
        /// <returns>Darkened color | 变深后的颜色</returns>
        public static Color Darken(Color color, double factor = 0.3)
        {
            return Color.FromRgb(
                (byte)(color.R * (1 - factor)),
                (byte)(color.G * (1 - factor)),
                (byte)(color.B * (1 - factor)));
        }

        #endregion
    }
}
