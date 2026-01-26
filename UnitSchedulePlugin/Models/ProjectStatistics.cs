using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitSchedulePlugin.Models
{
    /// <summary>
    /// Contains aggregated statistics for the entire project.
    /// Used for dashboard displays and summary reports.
    ///
    /// 包含整个项目的汇总统计数据。
    /// 用于仪表板显示和摘要报告。
    /// </summary>
    public class ProjectStatistics
    {
        #region Unit Counts | 单元数量

        /// <summary>
        /// Total number of units in the project.
        /// 项目中的单元总数。
        /// </summary>
        public int TotalUnits { get; set; }

        /// <summary>
        /// Number of units by type (e.g., "Studio": 10, "1BR": 25).
        /// 按类型划分的单元数量（例如，"开间"：10，"一居室"：25）。
        /// </summary>
        public Dictionary<string, int> UnitCountByType { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Number of units by building/tower.
        /// 按建筑/塔楼划分的单元数量。
        /// </summary>
        public Dictionary<string, int> UnitCountByBuilding { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Number of units by floor level.
        /// 按楼层划分的单元数量。
        /// </summary>
        public Dictionary<string, int> UnitCountByLevel { get; set; } = new Dictionary<string, int>();

        #endregion

        #region Area Statistics (Imperial - Square Feet) | 面积统计（英制 - 平方英尺）

        /// <summary>
        /// Total gross area of all units in square feet.
        /// 所有单元的总建筑面积（平方英尺）。
        /// </summary>
        public double TotalGrossArea { get; set; }

        /// <summary>
        /// Total net area of all units in square feet.
        /// 所有单元的总净面积（平方英尺）。
        /// </summary>
        public double TotalNetArea { get; set; }

        /// <summary>
        /// Total sellable area in square feet.
        /// 总可售面积（平方英尺）。
        /// </summary>
        public double TotalSellableArea { get; set; }

        /// <summary>
        /// Average gross area per unit in square feet.
        /// 每单元平均建筑面积（平方英尺）。
        /// </summary>
        public double AverageGrossArea => TotalUnits > 0 ? TotalGrossArea / TotalUnits : 0;

        /// <summary>
        /// Average net area per unit in square feet.
        /// 每单元平均净面积（平方英尺）。
        /// </summary>
        public double AverageNetArea => TotalUnits > 0 ? TotalNetArea / TotalUnits : 0;

        /// <summary>
        /// Minimum unit gross area in square feet.
        /// 最小单元建筑面积（平方英尺）。
        /// </summary>
        public double MinGrossArea { get; set; }

        /// <summary>
        /// Maximum unit gross area in square feet.
        /// 最大单元建筑面积（平方英尺）。
        /// </summary>
        public double MaxGrossArea { get; set; }

        /// <summary>
        /// Gross area by unit type.
        /// 按单元类型划分的建筑面积。
        /// </summary>
        public Dictionary<string, double> GrossAreaByType { get; set; } = new Dictionary<string, double>();

        /// <summary>
        /// Average gross area by unit type.
        /// 按单元类型划分的平均建筑面积。
        /// </summary>
        public Dictionary<string, double> AverageAreaByType { get; set; } = new Dictionary<string, double>();

        #endregion

        #region Open Space Statistics | 开放空间统计

        /// <summary>
        /// Total private open space area in square feet.
        /// 私人开放空间总面积（平方英尺）。
        /// </summary>
        public double TotalPrivateOpenSpace { get; set; }

        /// <summary>
        /// Total public/common open space area in square feet.
        /// 公共/共享开放空间总面积（平方英尺）。
        /// </summary>
        public double TotalPublicOpenSpace { get; set; }

        /// <summary>
        /// Total balcony area in square feet.
        /// 阳台总面积（平方英尺）。
        /// </summary>
        public double TotalBalconyArea { get; set; }

        /// <summary>
        /// Total patio area in square feet.
        /// 露台总面积（平方英尺）。
        /// </summary>
        public double TotalPatioArea { get; set; }

        /// <summary>
        /// Total terrace area in square feet.
        /// 平台总面积（平方英尺）。
        /// </summary>
        public double TotalTerraceArea { get; set; }

        /// <summary>
        /// Number of private open spaces.
        /// 私人开放空间数量。
        /// </summary>
        public int PrivateOpenSpaceCount { get; set; }

        /// <summary>
        /// Number of public open spaces.
        /// 公共开放空间数量。
        /// </summary>
        public int PublicOpenSpaceCount { get; set; }

        /// <summary>
        /// Private open space area by type.
        /// 按类型划分的私人开放空间面积。
        /// </summary>
        public Dictionary<string, double> PrivateOpenSpaceByType { get; set; } = new Dictionary<string, double>();

        /// <summary>
        /// Public open space area by type.
        /// 按类型划分的公共开放空间面积。
        /// </summary>
        public Dictionary<string, double> PublicOpenSpaceByType { get; set; } = new Dictionary<string, double>();

        #endregion

        #region Efficiency Metrics | 效率指标

        /// <summary>
        /// Overall project efficiency (Net/Gross ratio as percentage).
        /// 项目整体效率（净/总比率，以百分比表示）。
        /// </summary>
        public double OverallEfficiency => TotalGrossArea > 0 ? (TotalNetArea / TotalGrossArea) * 100 : 0;

        /// <summary>
        /// Average efficiency across all units.
        /// 所有单元的平均效率。
        /// </summary>
        public double AverageEfficiency { get; set; }

        /// <summary>
        /// Minimum efficiency among units.
        /// 单元中的最低效率。
        /// </summary>
        public double MinEfficiency { get; set; }

        /// <summary>
        /// Maximum efficiency among units.
        /// 单元中的最高效率。
        /// </summary>
        public double MaxEfficiency { get; set; }

        /// <summary>
        /// Open space ratio (Total open space / Total gross area).
        /// 开放空间比率（总开放空间/总建筑面积）。
        /// </summary>
        public double OpenSpaceRatio => TotalGrossArea > 0
            ? ((TotalPrivateOpenSpace + TotalPublicOpenSpace) / TotalGrossArea) * 100
            : 0;

        #endregion

        #region Bedroom Mix | 卧室组合

        /// <summary>
        /// Count of units by number of bedrooms.
        /// 按卧室数量划分的单元数量。
        /// </summary>
        public Dictionary<int, int> UnitCountByBedrooms { get; set; } = new Dictionary<int, int>();

        /// <summary>
        /// Percentage distribution by bedroom count.
        /// 按卧室数量的百分比分布。
        /// </summary>
        public Dictionary<int, double> BedroomMixPercentage { get; set; } = new Dictionary<int, double>();

        #endregion

        #region Formatted Display Properties | 格式化显示属性

        /// <summary>
        /// Formatted total gross area.
        /// 格式化的总建筑面积。
        /// </summary>
        public string FormattedTotalGrossArea => $"{TotalGrossArea:N0} SF";

        /// <summary>
        /// Formatted total net area.
        /// 格式化的总净面积。
        /// </summary>
        public string FormattedTotalNetArea => $"{TotalNetArea:N0} SF";

        /// <summary>
        /// Formatted total sellable area.
        /// 格式化的总可售面积。
        /// </summary>
        public string FormattedTotalSellableArea => $"{TotalSellableArea:N0} SF";

        /// <summary>
        /// Formatted average gross area.
        /// 格式化的平均建筑面积。
        /// </summary>
        public string FormattedAverageGrossArea => $"{AverageGrossArea:N0} SF";

        /// <summary>
        /// Formatted average net area.
        /// 格式化的平均净面积。
        /// </summary>
        public string FormattedAverageNetArea => $"{AverageNetArea:N0} SF";

        /// <summary>
        /// Formatted overall efficiency.
        /// 格式化的整体效率。
        /// </summary>
        public string FormattedEfficiency => $"{OverallEfficiency:F1}%";

        /// <summary>
        /// Formatted total private open space.
        /// 格式化的私人开放空间总面积。
        /// </summary>
        public string FormattedTotalPrivateOpenSpace => $"{TotalPrivateOpenSpace:N0} SF";

        /// <summary>
        /// Formatted total public open space.
        /// 格式化的公共开放空间总面积。
        /// </summary>
        public string FormattedTotalPublicOpenSpace => $"{TotalPublicOpenSpace:N0} SF";

        /// <summary>
        /// Formatted open space ratio.
        /// 格式化的开放空间比率。
        /// </summary>
        public string FormattedOpenSpaceRatio => $"{OpenSpaceRatio:F1}%";

        #endregion

        #region Methods | 方法

        /// <summary>
        /// Calculates all statistics from a list of units and open spaces.
        /// 从单元和开放空间列表计算所有统计数据。
        /// </summary>
        /// <param name="units">List of unit information | 单元信息列表</param>
        /// <param name="privateOpenSpaces">List of private open spaces | 私人开放空间列表</param>
        /// <param name="publicOpenSpaces">List of public open spaces | 公共开放空间列表</param>
        public void Calculate(
            IEnumerable<UnitInfo> units,
            IEnumerable<OpenSpaceInfo> privateOpenSpaces,
            IEnumerable<OpenSpaceInfo> publicOpenSpaces)
        {
            var unitList = units?.ToList() ?? new List<UnitInfo>();
            var privateList = privateOpenSpaces?.ToList() ?? new List<OpenSpaceInfo>();
            var publicList = publicOpenSpaces?.ToList() ?? new List<OpenSpaceInfo>();

            // Unit counts | 单元数量
            TotalUnits = unitList.Count;

            // Area totals | 面积总计
            TotalGrossArea = unitList.Sum(u => u.GrossArea);
            TotalNetArea = unitList.Sum(u => u.NetArea);
            TotalSellableArea = unitList.Sum(u => u.SellableArea);

            if (unitList.Any())
            {
                MinGrossArea = unitList.Min(u => u.GrossArea);
                MaxGrossArea = unitList.Max(u => u.GrossArea);
                AverageEfficiency = unitList.Average(u => u.Efficiency);
                MinEfficiency = unitList.Min(u => u.Efficiency);
                MaxEfficiency = unitList.Max(u => u.Efficiency);
            }

            // Group by unit type | 按单元类型分组
            UnitCountByType = unitList
                .GroupBy(u => u.UnitType ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());

            GrossAreaByType = unitList
                .GroupBy(u => u.UnitType ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Sum(u => u.GrossArea));

            AverageAreaByType = unitList
                .GroupBy(u => u.UnitType ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Average(u => u.GrossArea));

            // Group by building | 按建筑分组
            UnitCountByBuilding = unitList
                .Where(u => !string.IsNullOrEmpty(u.Building))
                .GroupBy(u => u.Building)
                .ToDictionary(g => g.Key, g => g.Count());

            // Group by level | 按楼层分组
            UnitCountByLevel = unitList
                .Where(u => !string.IsNullOrEmpty(u.Level))
                .GroupBy(u => u.Level)
                .ToDictionary(g => g.Key, g => g.Count());

            // Bedroom mix | 卧室组合
            UnitCountByBedrooms = unitList
                .GroupBy(u => u.BedroomCount)
                .ToDictionary(g => g.Key, g => g.Count());

            BedroomMixPercentage = UnitCountByBedrooms
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => TotalUnits > 0 ? (double)kvp.Value / TotalUnits * 100 : 0);

            // Private open space statistics | 私人开放空间统计
            PrivateOpenSpaceCount = privateList.Count;
            TotalPrivateOpenSpace = privateList.Sum(s => s.Area);
            TotalBalconyArea = privateList.Where(s => s.SpaceType == OpenSpaceType.Balcony).Sum(s => s.Area);
            TotalPatioArea = privateList.Where(s => s.SpaceType == OpenSpaceType.Patio).Sum(s => s.Area);
            TotalTerraceArea = privateList.Where(s => s.SpaceType == OpenSpaceType.Terrace).Sum(s => s.Area);

            PrivateOpenSpaceByType = privateList
                .GroupBy(s => s.SpaceType.ToString())
                .ToDictionary(g => g.Key, g => g.Sum(s => s.Area));

            // Public open space statistics | 公共开放空间统计
            PublicOpenSpaceCount = publicList.Count;
            TotalPublicOpenSpace = publicList.Sum(s => s.Area);

            PublicOpenSpaceByType = publicList
                .GroupBy(s => s.SpaceType.ToString())
                .ToDictionary(g => g.Key, g => g.Sum(s => s.Area));
        }

        /// <summary>
        /// Gets a summary text for display or export.
        /// 获取用于显示或导出的摘要文本。
        /// </summary>
        /// <returns>Formatted summary string | 格式化的摘要字符串</returns>
        public string GetSummaryText()
        {
            return $@"PROJECT STATISTICS SUMMARY
==========================
Total Units: {TotalUnits}
Total Gross Area: {FormattedTotalGrossArea}
Total Net Area: {FormattedTotalNetArea}
Total Sellable Area: {FormattedTotalSellableArea}
Average Unit Size: {FormattedAverageGrossArea}
Overall Efficiency: {FormattedEfficiency}

OPEN SPACE SUMMARY
------------------
Private Open Space: {FormattedTotalPrivateOpenSpace}
Public Open Space: {FormattedTotalPublicOpenSpace}
Open Space Ratio: {FormattedOpenSpaceRatio}

UNIT MIX
--------
{string.Join(Environment.NewLine, UnitCountByType.Select(kvp => $"{kvp.Key}: {kvp.Value} units"))}
";
        }

        #endregion
    }
}
