using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnitSchedulePlugin.Models;

namespace UnitSchedulePlugin.Services
{
    /// <summary>
    /// Service for exporting unit schedule data to various formats.
    /// Supports CSV, Excel-compatible CSV, and text reports.
    ///
    /// 用于将单元时间表数据导出为各种格式的服务。
    /// 支持 CSV、Excel 兼容的 CSV 和文本报告。
    /// </summary>
    public class ExportService
    {
        #region CSV Export | CSV 导出

        /// <summary>
        /// Exports unit data to CSV format.
        /// 将单元数据导出为 CSV 格式。
        /// </summary>
        /// <param name="units">Units to export | 要导出的单元</param>
        /// <param name="filePath">Output file path | 输出文件路径</param>
        public void ExportUnitsToCSV(IEnumerable<UnitInfo> units, string filePath)
        {
            var sb = new StringBuilder();

            // Header row | 标题行
            sb.AppendLine("Unit Number,Unit Type,Level,Building,Gross Area (SF),Net Area (SF)," +
                         "Sellable Area (SF),Balcony (SF),Patio (SF),Terrace (SF)," +
                         "Total Private Open Space (SF),Bedrooms,Bathrooms,Efficiency (%),Status");

            // Data rows | 数据行
            foreach (var unit in units)
            {
                sb.AppendLine($"\"{EscapeCSV(unit.UnitNumber)}\"," +
                             $"\"{EscapeCSV(unit.UnitType)}\"," +
                             $"\"{EscapeCSV(unit.Level)}\"," +
                             $"\"{EscapeCSV(unit.Building)}\"," +
                             $"{unit.GrossArea:F0}," +
                             $"{unit.NetArea:F0}," +
                             $"{unit.SellableArea:F0}," +
                             $"{unit.BalconyArea:F0}," +
                             $"{unit.PatioArea:F0}," +
                             $"{unit.TerraceArea:F0}," +
                             $"{unit.TotalPrivateOpenSpace:F0}," +
                             $"{unit.BedroomCount}," +
                             $"{unit.BathroomCount:F1}," +
                             $"{unit.Efficiency:F1}," +
                             $"\"{EscapeCSV(unit.Status)}\"");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Exports private open spaces to CSV format.
        /// 将私有开放空间导出为 CSV 格式。
        /// </summary>
        /// <param name="openSpaces">Open spaces to export | 要导出的开放空间</param>
        /// <param name="filePath">Output file path | 输出文件路径</param>
        public void ExportPrivateOpenSpacesToCSV(IEnumerable<OpenSpaceInfo> openSpaces, string filePath)
        {
            var sb = new StringBuilder();

            // Header row | 标题行
            sb.AppendLine("Space Number,Name,Type,Associated Unit,Level,Building," +
                         "Area (SF),Covered Area (SF),Coverage,Sellable Contribution (SF)");

            // Data rows | 数据行
            foreach (var space in openSpaces)
            {
                sb.AppendLine($"\"{EscapeCSV(space.SpaceNumber)}\"," +
                             $"\"{EscapeCSV(space.Name)}\"," +
                             $"\"{space.SpaceTypeName}\"," +
                             $"\"{EscapeCSV(space.AssociatedUnitNumber)}\"," +
                             $"\"{EscapeCSV(space.Level)}\"," +
                             $"\"{EscapeCSV(space.Building)}\"," +
                             $"{space.Area:F0}," +
                             $"{space.CoveredArea:F0}," +
                             $"\"{space.CoverageDescription}\"," +
                             $"{space.SellableAreaContribution:F0}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Exports public open spaces to CSV format.
        /// 将公共开放空间导出为 CSV 格式。
        /// </summary>
        /// <param name="openSpaces">Open spaces to export | 要导出的开放空间</param>
        /// <param name="filePath">Output file path | 输出文件路径</param>
        public void ExportPublicOpenSpacesToCSV(IEnumerable<OpenSpaceInfo> openSpaces, string filePath)
        {
            var sb = new StringBuilder();

            // Header row | 标题行
            sb.AppendLine("Space Number,Name,Type,Level,Building,Area (SF)," +
                         "Covered Area (SF),Coverage,Is Rooftop,Is At Grade");

            // Data rows | 数据行
            foreach (var space in openSpaces)
            {
                sb.AppendLine($"\"{EscapeCSV(space.SpaceNumber)}\"," +
                             $"\"{EscapeCSV(space.Name)}\"," +
                             $"\"{space.SpaceTypeName}\"," +
                             $"\"{EscapeCSV(space.Level)}\"," +
                             $"\"{EscapeCSV(space.Building)}\"," +
                             $"{space.Area:F0}," +
                             $"{space.CoveredArea:F0}," +
                             $"\"{space.CoverageDescription}\"," +
                             $"{(space.IsRooftop ? "Yes" : "No")}," +
                             $"{(space.IsAtGrade ? "Yes" : "No")}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        #endregion

        #region Summary Report Export | 摘要报告导出

        /// <summary>
        /// Exports a comprehensive summary report.
        /// 导出全面的摘要报告。
        /// </summary>
        /// <param name="statistics">Project statistics | 项目统计数据</param>
        /// <param name="units">Unit list | 单元列表</param>
        /// <param name="privateOpenSpaces">Private open spaces | 私有开放空间</param>
        /// <param name="publicOpenSpaces">Public open spaces | 公共开放空间</param>
        /// <param name="projectName">Project name | 项目名称</param>
        /// <param name="filePath">Output file path | 输出文件路径</param>
        public void ExportSummaryReport(
            ProjectStatistics statistics,
            IEnumerable<UnitInfo> units,
            IEnumerable<OpenSpaceInfo> privateOpenSpaces,
            IEnumerable<OpenSpaceInfo> publicOpenSpaces,
            string projectName,
            string filePath)
        {
            var sb = new StringBuilder();

            // Report Header | 报告标题
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("                    UNIT SCHEDULE REPORT");
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine();
            sb.AppendLine($"Project: {projectName}");
            sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            // Executive Summary | 执行摘要
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("                    EXECUTIVE SUMMARY");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine($"  Total Units:              {statistics.TotalUnits}");
            sb.AppendLine($"  Total Gross Area:         {statistics.FormattedTotalGrossArea}");
            sb.AppendLine($"  Total Net Area:           {statistics.FormattedTotalNetArea}");
            sb.AppendLine($"  Total Sellable Area:      {statistics.FormattedTotalSellableArea}");
            sb.AppendLine($"  Average Unit Size:        {statistics.FormattedAverageGrossArea}");
            sb.AppendLine($"  Overall Efficiency:       {statistics.FormattedEfficiency}");
            sb.AppendLine();

            // Unit Mix | 单元组合
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("                        UNIT MIX");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("  Type                Count       Total Area       Avg Area");
            sb.AppendLine("  ────────────────────────────────────────────────────────────");

            foreach (var type in statistics.UnitCountByType.OrderBy(x => x.Key))
            {
                double totalArea = statistics.GrossAreaByType.ContainsKey(type.Key)
                    ? statistics.GrossAreaByType[type.Key] : 0;
                double avgArea = statistics.AverageAreaByType.ContainsKey(type.Key)
                    ? statistics.AverageAreaByType[type.Key] : 0;

                sb.AppendLine($"  {type.Key,-16}  {type.Value,6}       {totalArea,10:N0} SF   {avgArea,8:N0} SF");
            }
            sb.AppendLine();

            // Open Space Summary | 开放空间摘要
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("                    OPEN SPACE SUMMARY");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("  Private Open Space");
            sb.AppendLine("  ───────────────────");
            sb.AppendLine($"    Total Area:             {statistics.FormattedTotalPrivateOpenSpace}");
            sb.AppendLine($"    Number of Spaces:       {statistics.PrivateOpenSpaceCount}");
            sb.AppendLine($"    Balcony Area:           {statistics.TotalBalconyArea:N0} SF");
            sb.AppendLine($"    Patio Area:             {statistics.TotalPatioArea:N0} SF");
            sb.AppendLine($"    Terrace Area:           {statistics.TotalTerraceArea:N0} SF");
            sb.AppendLine();
            sb.AppendLine("  Public Open Space");
            sb.AppendLine("  ───────────────────");
            sb.AppendLine($"    Total Area:             {statistics.FormattedTotalPublicOpenSpace}");
            sb.AppendLine($"    Number of Spaces:       {statistics.PublicOpenSpaceCount}");
            sb.AppendLine();

            if (statistics.PublicOpenSpaceByType.Any())
            {
                sb.AppendLine("    By Type:");
                foreach (var type in statistics.PublicOpenSpaceByType.OrderByDescending(x => x.Value))
                {
                    sb.AppendLine($"      {type.Key,-20}  {type.Value:N0} SF");
                }
                sb.AppendLine();
            }

            sb.AppendLine($"  Open Space Ratio:         {statistics.FormattedOpenSpaceRatio}");
            sb.AppendLine();

            // Bedroom Mix | 卧室组合
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("                      BEDROOM MIX");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("  Bedrooms          Count         Percentage");
            sb.AppendLine("  ────────────────────────────────────────────");

            foreach (var bedroom in statistics.UnitCountByBedrooms.OrderBy(x => x.Key))
            {
                string bedroomLabel = bedroom.Key == 0 ? "Studio" : $"{bedroom.Key} BR";
                double percentage = statistics.BedroomMixPercentage.ContainsKey(bedroom.Key)
                    ? statistics.BedroomMixPercentage[bedroom.Key] : 0;

                sb.AppendLine($"  {bedroomLabel,-16}  {bedroom.Value,6}         {percentage,6:F1}%");
            }
            sb.AppendLine();

            // Unit Details Table | 单元详情表
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine("                      UNIT DETAILS");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("Unit       Type       Level      Gross SF    Net SF     Outdoor SF  Eff%");
            sb.AppendLine("───────────────────────────────────────────────────────────────────────────");

            foreach (var unit in units.OrderBy(u => u.Level).ThenBy(u => u.UnitNumber))
            {
                sb.AppendLine($"{unit.UnitNumber,-10} {unit.UnitType,-10} {unit.Level,-10} " +
                             $"{unit.GrossArea,10:N0} {unit.NetArea,10:N0} " +
                             $"{unit.TotalPrivateOpenSpace,10:N0}  {unit.Efficiency,5:F1}");
            }
            sb.AppendLine();

            // Footer | 页脚
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine("                      END OF REPORT");
            sb.AppendLine("═══════════════════════════════════════════════════════════════");

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        #endregion

        #region Combined Export | 组合导出

        /// <summary>
        /// Exports all data to a single Excel-ready CSV file with multiple sections.
        /// 将所有数据导出到具有多个部分的单个 Excel 就绪 CSV 文件。
        /// </summary>
        /// <param name="statistics">Project statistics | 项目统计数据</param>
        /// <param name="units">Unit list | 单元列表</param>
        /// <param name="privateOpenSpaces">Private open spaces | 私有开放空间</param>
        /// <param name="publicOpenSpaces">Public open spaces | 公共开放空间</param>
        /// <param name="projectName">Project name | 项目名称</param>
        /// <param name="filePath">Output file path | 输出文件路径</param>
        public void ExportAllToCSV(
            ProjectStatistics statistics,
            IEnumerable<UnitInfo> units,
            IEnumerable<OpenSpaceInfo> privateOpenSpaces,
            IEnumerable<OpenSpaceInfo> publicOpenSpaces,
            string projectName,
            string filePath)
        {
            var sb = new StringBuilder();

            // Project Info Section | 项目信息部分
            sb.AppendLine("PROJECT SUMMARY");
            sb.AppendLine($"Project Name,{EscapeCSV(projectName)}");
            sb.AppendLine($"Generated Date,{DateTime.Now:yyyy-MM-dd}");
            sb.AppendLine($"Total Units,{statistics.TotalUnits}");
            sb.AppendLine($"Total Gross Area (SF),{statistics.TotalGrossArea:F0}");
            sb.AppendLine($"Total Net Area (SF),{statistics.TotalNetArea:F0}");
            sb.AppendLine($"Total Sellable Area (SF),{statistics.TotalSellableArea:F0}");
            sb.AppendLine($"Total Private Open Space (SF),{statistics.TotalPrivateOpenSpace:F0}");
            sb.AppendLine($"Total Public Open Space (SF),{statistics.TotalPublicOpenSpace:F0}");
            sb.AppendLine($"Overall Efficiency (%),{statistics.OverallEfficiency:F1}");
            sb.AppendLine();
            sb.AppendLine();

            // Units Section | 单元部分
            sb.AppendLine("UNIT SCHEDULE");
            sb.AppendLine("Unit Number,Unit Type,Level,Building,Gross Area (SF),Net Area (SF)," +
                         "Sellable Area (SF),Balcony (SF),Patio (SF),Terrace (SF)," +
                         "Total Private Open Space (SF),Bedrooms,Bathrooms,Efficiency (%),Status");

            foreach (var unit in units.OrderBy(u => u.Level).ThenBy(u => u.UnitNumber))
            {
                sb.AppendLine($"\"{EscapeCSV(unit.UnitNumber)}\"," +
                             $"\"{EscapeCSV(unit.UnitType)}\"," +
                             $"\"{EscapeCSV(unit.Level)}\"," +
                             $"\"{EscapeCSV(unit.Building)}\"," +
                             $"{unit.GrossArea:F0}," +
                             $"{unit.NetArea:F0}," +
                             $"{unit.SellableArea:F0}," +
                             $"{unit.BalconyArea:F0}," +
                             $"{unit.PatioArea:F0}," +
                             $"{unit.TerraceArea:F0}," +
                             $"{unit.TotalPrivateOpenSpace:F0}," +
                             $"{unit.BedroomCount}," +
                             $"{unit.BathroomCount:F1}," +
                             $"{unit.Efficiency:F1}," +
                             $"\"{EscapeCSV(unit.Status)}\"");
            }
            sb.AppendLine();
            sb.AppendLine();

            // Private Open Spaces Section | 私有开放空间部分
            sb.AppendLine("PRIVATE OPEN SPACES");
            sb.AppendLine("Space Number,Name,Type,Associated Unit,Level,Area (SF),Covered Area (SF),Coverage");

            foreach (var space in privateOpenSpaces.OrderBy(s => s.Level).ThenBy(s => s.AssociatedUnitNumber))
            {
                sb.AppendLine($"\"{EscapeCSV(space.SpaceNumber)}\"," +
                             $"\"{EscapeCSV(space.Name)}\"," +
                             $"\"{space.SpaceTypeName}\"," +
                             $"\"{EscapeCSV(space.AssociatedUnitNumber)}\"," +
                             $"\"{EscapeCSV(space.Level)}\"," +
                             $"{space.Area:F0}," +
                             $"{space.CoveredArea:F0}," +
                             $"\"{space.CoverageDescription}\"");
            }
            sb.AppendLine();
            sb.AppendLine();

            // Public Open Spaces Section | 公共开放空间部分
            sb.AppendLine("PUBLIC OPEN SPACES");
            sb.AppendLine("Space Number,Name,Type,Level,Area (SF),Covered Area (SF),Coverage,Is Rooftop");

            foreach (var space in publicOpenSpaces.OrderBy(s => s.SpaceType).ThenBy(s => s.Name))
            {
                sb.AppendLine($"\"{EscapeCSV(space.SpaceNumber)}\"," +
                             $"\"{EscapeCSV(space.Name)}\"," +
                             $"\"{space.SpaceTypeName}\"," +
                             $"\"{EscapeCSV(space.Level)}\"," +
                             $"{space.Area:F0}," +
                             $"{space.CoveredArea:F0}," +
                             $"\"{space.CoverageDescription}\"," +
                             $"{(space.IsRooftop ? "Yes" : "No")}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        #endregion

        #region Helper Methods | 辅助方法

        /// <summary>
        /// Escapes a string for CSV format.
        /// 为 CSV 格式转义字符串。
        /// </summary>
        /// <param name="value">Value to escape | 要转义的值</param>
        /// <returns>Escaped string | 转义后的字符串</returns>
        private string EscapeCSV(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // Escape double quotes by doubling them
            // 通过加倍来转义双引号
            return value.Replace("\"", "\"\"");
        }

        #endregion
    }
}
