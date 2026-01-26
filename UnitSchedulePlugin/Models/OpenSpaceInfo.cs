using System;

namespace UnitSchedulePlugin.Models
{
    /// <summary>
    /// Enumeration of open space types.
    /// 开放空间类型枚举。
    /// </summary>
    public enum OpenSpaceType
    {
        /// <summary>Balcony - typically elevated, covered or uncovered | 阳台 - 通常是高架的，有顶或无顶</summary>
        Balcony,

        /// <summary>Patio - ground level outdoor space | 露台 - 地面层室外空间</summary>
        Patio,

        /// <summary>Terrace - roof-level or stepped outdoor space | 平台 - 屋顶层或阶梯式室外空间</summary>
        Terrace,

        /// <summary>Deck - wooden platform outdoor space | 甲板 - 木质平台室外空间</summary>
        Deck,

        /// <summary>Courtyard - enclosed outdoor space | 庭院 - 封闭的室外空间</summary>
        Courtyard,

        /// <summary>Garden - landscaped outdoor area | 花园 - 景观室外区域</summary>
        Garden,

        /// <summary>Rooftop - roof-level amenity space | 屋顶 - 屋顶层设施空间</summary>
        Rooftop,

        /// <summary>Pool area - swimming pool and surrounding deck | 泳池区 - 游泳池及周围甲板</summary>
        PoolArea,

        /// <summary>Playground - children's play area | 游乐场 - 儿童游乐区</summary>
        Playground,

        /// <summary>Plaza - public gathering space | 广场 - 公共聚集空间</summary>
        Plaza,

        /// <summary>Amenity - general amenity space | 设施 - 一般设施空间</summary>
        Amenity,

        /// <summary>Other - unclassified open space | 其他 - 未分类的开放空间</summary>
        Other
    }

    /// <summary>
    /// Enumeration for ownership/access type of open space.
    /// 开放空间的所有权/访问类型枚举。
    /// </summary>
    public enum OpenSpaceOwnership
    {
        /// <summary>Private - belongs to specific unit | 私有 - 属于特定单元</summary>
        Private,

        /// <summary>Common/Shared - accessible to all residents | 公共/共享 - 所有居民可使用</summary>
        Common,

        /// <summary>Public - accessible to general public | 公共 - 公众可使用</summary>
        Public,

        /// <summary>Semi-private - shared between few units | 半私有 - 几个单元共享</summary>
        SemiPrivate
    }

    /// <summary>
    /// Represents an open space (outdoor area) in the building project.
    /// Can be private (balcony, patio) or public (amenity, courtyard).
    ///
    /// 表示建筑项目中的开放空间（室外区域）。
    /// 可以是私有的（阳台、露台）或公共的（设施、庭院）。
    /// </summary>
    public class OpenSpaceInfo
    {
        #region Identity Properties | 身份属性

        /// <summary>
        /// Unique identifier (Revit Element Id).
        /// 唯一标识符（Revit 元素 ID）。
        /// </summary>
        public int ElementId { get; set; }

        /// <summary>
        /// Name of the open space from Revit.
        /// 来自 Revit 的开放空间名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Identifier or number of the open space.
        /// 开放空间的标识符或编号。
        /// </summary>
        public string SpaceNumber { get; set; }

        #endregion

        #region Classification Properties | 分类属性

        /// <summary>
        /// Type of open space (Balcony, Patio, Terrace, etc.).
        /// 开放空间类型（阳台、露台、平台等）。
        /// </summary>
        public OpenSpaceType SpaceType { get; set; }

        /// <summary>
        /// Ownership/access classification.
        /// 所有权/访问分类。
        /// </summary>
        public OpenSpaceOwnership Ownership { get; set; }

        /// <summary>
        /// Display name for the space type.
        /// 空间类型的显示名称。
        /// </summary>
        public string SpaceTypeName => SpaceType.ToString();

        /// <summary>
        /// Associated unit number (for private spaces).
        /// 关联的单元编号（对于私有空间）。
        /// </summary>
        public string AssociatedUnitNumber { get; set; }

        /// <summary>
        /// Associated unit's Element ID (for private spaces).
        /// 关联单元的元素 ID（对于私有空间）。
        /// </summary>
        public int? AssociatedUnitId { get; set; }

        /// <summary>
        /// Floor level where the space is located.
        /// 空间所在的楼层。
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Building identifier for multi-building projects.
        /// 多建筑项目的建筑标识符。
        /// </summary>
        public string Building { get; set; }

        #endregion

        #region Area Properties (Imperial - Square Feet) | 面积属性（英制 - 平方英尺）

        /// <summary>
        /// Total area of the open space in square feet.
        /// 开放空间的总面积（平方英尺）。
        /// </summary>
        public double Area { get; set; }

        /// <summary>
        /// Covered (roofed) portion of the area in square feet.
        /// 有顶部分的面积（平方英尺）。
        /// </summary>
        public double CoveredArea { get; set; }

        /// <summary>
        /// Uncovered (open-air) portion of the area in square feet.
        /// 无顶部分的面积（平方英尺）。
        /// </summary>
        public double UncoveredArea => Area - CoveredArea;

        /// <summary>
        /// Perimeter of the open space in feet.
        /// 开放空间的周长（英尺）。
        /// </summary>
        public double Perimeter { get; set; }

        #endregion

        #region Physical Properties | 物理属性

        /// <summary>
        /// Indicates if the space is covered/roofed.
        /// 指示空间是否有顶/有屋顶。
        /// </summary>
        public bool IsCovered { get; set; }

        /// <summary>
        /// Indicates if the space is at ground level.
        /// 指示空间是否在地面层。
        /// </summary>
        public bool IsAtGrade { get; set; }

        /// <summary>
        /// Indicates if the space is on the rooftop.
        /// 指示空间是否在屋顶。
        /// </summary>
        public bool IsRooftop { get; set; }

        /// <summary>
        /// Height above grade in feet (if elevated).
        /// 高于地面的高度（英尺）（如果是高架的）。
        /// </summary>
        public double HeightAboveGrade { get; set; }

        /// <summary>
        /// Orientation/facing direction (N, S, E, W, NE, etc.).
        /// 朝向/面向方向（北、南、东、西、东北等）。
        /// </summary>
        public string Orientation { get; set; }

        #endregion

        #region Computed Properties | 计算属性

        /// <summary>
        /// Display text for lists and reports.
        /// 用于列表和报告的显示文本。
        /// </summary>
        public string DisplayName => !string.IsNullOrEmpty(SpaceNumber)
            ? $"{SpaceNumber} - {SpaceTypeName}"
            : $"{Name} ({SpaceTypeName})";

        /// <summary>
        /// Formatted area with unit suffix.
        /// 带单位后缀的格式化面积。
        /// </summary>
        public string FormattedArea => $"{Area:N0} SF";

        /// <summary>
        /// Formatted covered area with unit suffix.
        /// 带单位后缀的格式化有顶面积。
        /// </summary>
        public string FormattedCoveredArea => $"{CoveredArea:N0} SF";

        /// <summary>
        /// Coverage description for display.
        /// 用于显示的覆盖描述。
        /// </summary>
        public string CoverageDescription => IsCovered ? "Covered" : "Open";

        /// <summary>
        /// Ownership description for display.
        /// 用于显示的所有权描述。
        /// </summary>
        public string OwnershipDescription => Ownership.ToString();

        /// <summary>
        /// Determines if this is a private open space (belongs to a unit).
        /// 确定这是否是私有开放空间（属于某个单元）。
        /// </summary>
        public bool IsPrivate => Ownership == OpenSpaceOwnership.Private ||
                                  Ownership == OpenSpaceOwnership.SemiPrivate;

        /// <summary>
        /// Determines if this is a public/common open space.
        /// 确定这是否是公共/共享开放空间。
        /// </summary>
        public bool IsPublic => Ownership == OpenSpaceOwnership.Common ||
                                 Ownership == OpenSpaceOwnership.Public;

        #endregion

        #region Sellable Area Calculation | 可售面积计算

        /// <summary>
        /// Multiplier for sellable area calculation (varies by jurisdiction).
        /// Covered: typically 50%, Uncovered: typically 25%.
        ///
        /// 可售面积计算乘数（因地区而异）。
        /// 有顶：通常 50%，无顶：通常 25%。
        /// </summary>
        public double SellableAreaMultiplier { get; set; } = 0.5;

        /// <summary>
        /// Calculated sellable area contribution in square feet.
        /// 计算的可售面积贡献（平方英尺）。
        /// </summary>
        public double SellableAreaContribution => Area * SellableAreaMultiplier;

        /// <summary>
        /// Formatted sellable area contribution.
        /// 格式化的可售面积贡献。
        /// </summary>
        public string FormattedSellableContribution => $"{SellableAreaContribution:N0} SF";

        #endregion

        #region Constructors | 构造函数

        /// <summary>
        /// Default constructor.
        /// 默认构造函数。
        /// </summary>
        public OpenSpaceInfo()
        {
        }

        /// <summary>
        /// Constructor with essential properties.
        /// 带有基本属性的构造函数。
        /// </summary>
        /// <param name="elementId">Revit element ID | Revit 元素 ID</param>
        /// <param name="name">Space name | 空间名称</param>
        /// <param name="spaceType">Type of open space | 开放空间类型</param>
        /// <param name="ownership">Ownership type | 所有权类型</param>
        public OpenSpaceInfo(int elementId, string name, OpenSpaceType spaceType, OpenSpaceOwnership ownership)
        {
            ElementId = elementId;
            Name = name;
            SpaceType = spaceType;
            Ownership = ownership;
        }

        #endregion

        #region Methods | 方法

        /// <summary>
        /// Returns a string representation of the open space.
        /// 返回开放空间的字符串表示。
        /// </summary>
        public override string ToString()
        {
            return $"{DisplayName} - {FormattedArea} ({OwnershipDescription})";
        }

        #endregion
    }
}
