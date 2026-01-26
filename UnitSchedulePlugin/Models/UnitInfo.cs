using System;
using System.Collections.Generic;

namespace UnitSchedulePlugin.Models
{
    /// <summary>
    /// Represents a residential or commercial unit in the building.
    /// Contains all relevant information for scheduling and analysis.
    ///
    /// 表示建筑物中的住宅或商业单元。
    /// 包含用于调度和分析的所有相关信息。
    /// </summary>
    public class UnitInfo
    {
        #region Identity Properties | 身份属性

        /// <summary>
        /// Unique identifier for the unit (Revit Element Id).
        /// 单元的唯一标识符（Revit 元素 ID）。
        /// </summary>
        public int ElementId { get; set; }

        /// <summary>
        /// Unit number or identifier (e.g., "101", "A-201").
        /// 单元编号或标识符（例如 "101"、"A-201"）。
        /// </summary>
        public string UnitNumber { get; set; }

        /// <summary>
        /// Name of the unit from Revit (Room Name).
        /// 来自 Revit 的单元名称（房间名称）。
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region Classification Properties | 分类属性

        /// <summary>
        /// Type of unit (e.g., "Studio", "1BR", "2BR", "3BR", "Penthouse").
        /// 单元类型（例如 "开间"、"一居室"、"两居室"、"三居室"、"顶层公寓"）。
        /// </summary>
        public string UnitType { get; set; }

        /// <summary>
        /// Department or section the unit belongs to.
        /// 单元所属的部门或区域。
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Building or tower identifier in multi-building projects.
        /// 多建筑项目中的建筑或塔楼标识符。
        /// </summary>
        public string Building { get; set; }

        /// <summary>
        /// Floor level where the unit is located.
        /// 单元所在的楼层。
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Phase of construction when this unit is scheduled.
        /// 此单元计划的施工阶段。
        /// </summary>
        public string Phase { get; set; }

        #endregion

        #region Area Properties (Imperial - Square Feet) | 面积属性（英制 - 平方英尺）

        /// <summary>
        /// Gross interior area of the unit in square feet.
        /// Includes all interior spaces without deductions.
        /// 单元的室内总面积（平方英尺）。
        /// 包括所有室内空间，无扣除。
        /// </summary>
        public double GrossArea { get; set; }

        /// <summary>
        /// Net interior area in square feet.
        /// Excludes walls, columns, and other structural elements.
        /// 净室内面积（平方英尺）。
        /// 不包括墙壁、柱子和其他结构元素。
        /// </summary>
        public double NetArea { get; set; }

        /// <summary>
        /// Total area of private balconies in square feet.
        /// 私人阳台总面积（平方英尺）。
        /// </summary>
        public double BalconyArea { get; set; }

        /// <summary>
        /// Total area of private patios in square feet.
        /// 私人露台总面积（平方英尺）。
        /// </summary>
        public double PatioArea { get; set; }

        /// <summary>
        /// Total area of private terraces in square feet.
        /// 私人平台总面积（平方英尺）。
        /// </summary>
        public double TerraceArea { get; set; }

        /// <summary>
        /// Total private open space area in square feet.
        /// Sum of balcony, patio, and terrace areas.
        /// 私人开放空间总面积（平方英尺）。
        /// 阳台、露台和平台面积之和。
        /// </summary>
        public double TotalPrivateOpenSpace => BalconyArea + PatioArea + TerraceArea;

        /// <summary>
        /// Calculated sellable/leasable area in square feet.
        /// Based on local regulations (typically Net + partial outdoor).
        /// 计算的可售/可租面积（平方英尺）。
        /// 基于当地法规（通常为净面积 + 部分室外面积）。
        /// </summary>
        public double SellableArea { get; set; }

        #endregion

        #region Room Details | 房间详情

        /// <summary>
        /// Number of bedrooms in the unit.
        /// 单元中的卧室数量。
        /// </summary>
        public int BedroomCount { get; set; }

        /// <summary>
        /// Number of bathrooms in the unit.
        /// 单元中的浴室数量。
        /// </summary>
        public double BathroomCount { get; set; }

        /// <summary>
        /// List of individual rooms within the unit.
        /// 单元内各个房间的列表。
        /// </summary>
        public List<RoomDetail> Rooms { get; set; } = new List<RoomDetail>();

        /// <summary>
        /// List of associated private open spaces.
        /// 关联的私人开放空间列表。
        /// </summary>
        public List<OpenSpaceInfo> PrivateOpenSpaces { get; set; } = new List<OpenSpaceInfo>();

        #endregion

        #region Status Properties | 状态属性

        /// <summary>
        /// Indicates if the unit is marked as occupied.
        /// 指示单元是否标记为已占用。
        /// </summary>
        public bool IsOccupied { get; set; }

        /// <summary>
        /// Current status of the unit (e.g., "Available", "Sold", "Reserved").
        /// 单元的当前状态（例如 "可用"、"已售"、"预留"）。
        /// </summary>
        public string Status { get; set; } = "Available";

        /// <summary>
        /// Additional comments or notes about the unit.
        /// 关于单元的其他评论或备注。
        /// </summary>
        public string Comments { get; set; }

        #endregion

        #region Computed Properties | 计算属性

        /// <summary>
        /// Display text for the unit in lists and reports.
        /// 在列表和报告中显示的单元文本。
        /// </summary>
        public string DisplayName => $"{UnitNumber} - {UnitType}";

        /// <summary>
        /// Formatted gross area with unit suffix.
        /// 带单位后缀的格式化总面积。
        /// </summary>
        public string FormattedGrossArea => $"{GrossArea:N0} SF";

        /// <summary>
        /// Formatted net area with unit suffix.
        /// 带单位后缀的格式化净面积。
        /// </summary>
        public string FormattedNetArea => $"{NetArea:N0} SF";

        /// <summary>
        /// Formatted total private open space with unit suffix.
        /// 带单位后缀的格式化私人开放空间总面积。
        /// </summary>
        public string FormattedPrivateOpenSpace => $"{TotalPrivateOpenSpace:N0} SF";

        /// <summary>
        /// Efficiency ratio (Net Area / Gross Area).
        /// 效率比（净面积/总面积）。
        /// </summary>
        public double Efficiency => GrossArea > 0 ? NetArea / GrossArea * 100 : 0;

        /// <summary>
        /// Formatted efficiency percentage.
        /// 格式化的效率百分比。
        /// </summary>
        public string FormattedEfficiency => $"{Efficiency:F1}%";

        #endregion

        #region Constructors | 构造函数

        /// <summary>
        /// Default constructor.
        /// 默认构造函数。
        /// </summary>
        public UnitInfo()
        {
        }

        /// <summary>
        /// Constructor with essential properties.
        /// 带有基本属性的构造函数。
        /// </summary>
        /// <param name="elementId">Revit element ID | Revit 元素 ID</param>
        /// <param name="unitNumber">Unit number | 单元编号</param>
        /// <param name="unitType">Unit type classification | 单元类型分类</param>
        public UnitInfo(int elementId, string unitNumber, string unitType)
        {
            ElementId = elementId;
            UnitNumber = unitNumber;
            UnitType = unitType;
        }

        #endregion

        #region Methods | 方法

        /// <summary>
        /// Calculates the sellable area based on standard rules.
        /// Typically: Net Area + 50% of covered outdoor + 25% of uncovered outdoor.
        ///
        /// 根据标准规则计算可售面积。
        /// 通常：净面积 + 50% 有顶室外面积 + 25% 无顶室外面积。
        /// </summary>
        public void CalculateSellableArea()
        {
            // Standard calculation: Net + 50% of balcony/terrace
            // 标准计算：净面积 + 50% 阳台/平台面积
            SellableArea = NetArea + (TotalPrivateOpenSpace * 0.5);
        }

        /// <summary>
        /// Returns a string representation of the unit.
        /// 返回单元的字符串表示。
        /// </summary>
        public override string ToString()
        {
            return $"{UnitNumber} ({UnitType}) - {FormattedGrossArea}";
        }

        #endregion
    }

    /// <summary>
    /// Represents details of an individual room within a unit.
    /// 表示单元内单个房间的详细信息。
    /// </summary>
    public class RoomDetail
    {
        /// <summary>
        /// Revit Element ID of the room.
        /// 房间的 Revit 元素 ID。
        /// </summary>
        public int ElementId { get; set; }

        /// <summary>
        /// Name of the room (e.g., "Living Room", "Bedroom 1").
        /// 房间名称（例如 "客厅"、"卧室1"）。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Room type classification.
        /// 房间类型分类。
        /// </summary>
        public string RoomType { get; set; }

        /// <summary>
        /// Area of the room in square feet.
        /// 房间面积（平方英尺）。
        /// </summary>
        public double Area { get; set; }

        /// <summary>
        /// Formatted area display string.
        /// 格式化的面积显示字符串。
        /// </summary>
        public string FormattedArea => $"{Area:N0} SF";
    }
}
