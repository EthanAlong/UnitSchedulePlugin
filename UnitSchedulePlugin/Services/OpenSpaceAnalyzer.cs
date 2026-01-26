using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using UnitSchedulePlugin.Helpers;
using UnitSchedulePlugin.Models;

namespace UnitSchedulePlugin.Services
{
    /// <summary>
    /// Service for analyzing and extracting open space information from Revit documents.
    /// Identifies and classifies private and public outdoor spaces.
    ///
    /// 用于从 Revit 文档分析和提取开放空间信息的服务。
    /// 识别和分类私有和公共室外空间。
    /// </summary>
    public class OpenSpaceAnalyzer
    {
        #region Fields | 字段

        private readonly Document _document;

        #endregion

        #region Constructor | 构造函数

        /// <summary>
        /// Initializes a new instance of the OpenSpaceAnalyzer class.
        /// 初始化 OpenSpaceAnalyzer 类的新实例。
        /// </summary>
        /// <param name="document">Revit document to analyze | 要分析的 Revit 文档</param>
        public OpenSpaceAnalyzer(Document document)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
        }

        #endregion

        #region Public Methods | 公共方法

        /// <summary>
        /// Gets all private open spaces (balconies, patios, terraces associated with units).
        /// 获取所有私有开放空间（与单元关联的阳台、露台、平台）。
        /// </summary>
        /// <returns>List of private open spaces | 私有开放空间列表</returns>
        public List<OpenSpaceInfo> GetPrivateOpenSpaces()
        {
            var privateSpaces = new List<OpenSpaceInfo>();

            try
            {
                // Scan rooms for private open spaces
                // 扫描房间以查找私有开放空间
                var rooms = new FilteredElementCollector(_document)
                    .OfCategory(BuiltInCategory.OST_Rooms)
                    .WhereElementIsNotElementType()
                    .Cast<Room>()
                    .Where(r => r.Area > 0)
                    .ToList();

                foreach (var room in rooms)
                {
                    string roomName = room.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "";

                    if (SpaceTypeIdentifier.IsPrivateOpenSpace(roomName))
                    {
                        var openSpace = CreateOpenSpaceFromRoom(room, true);
                        if (openSpace != null)
                        {
                            privateSpaces.Add(openSpace);
                        }
                    }
                }

                // Also check Area elements for private open spaces
                // 还检查 Area 元素以查找私有开放空间
                var areas = new FilteredElementCollector(_document)
                    .OfCategory(BuiltInCategory.OST_Areas)
                    .WhereElementIsNotElementType()
                    .Cast<Area>()
                    .Where(a => a.Area > 0)
                    .ToList();

                foreach (var area in areas)
                {
                    string areaName = area.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "";

                    if (SpaceTypeIdentifier.IsPrivateOpenSpace(areaName))
                    {
                        var openSpace = CreateOpenSpaceFromArea(area, true);
                        if (openSpace != null)
                        {
                            privateSpaces.Add(openSpace);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting private open spaces: {ex.Message}");
            }

            return privateSpaces.OrderBy(s => s.Level).ThenBy(s => s.AssociatedUnitNumber).ToList();
        }

        /// <summary>
        /// Gets all public/common open spaces (amenities, courtyards, plazas).
        /// 获取所有公共/共享开放空间（设施、庭院、广场）。
        /// </summary>
        /// <returns>List of public open spaces | 公共开放空间列表</returns>
        public List<OpenSpaceInfo> GetPublicOpenSpaces()
        {
            var publicSpaces = new List<OpenSpaceInfo>();

            try
            {
                // Scan rooms for public open spaces
                // 扫描房间以查找公共开放空间
                var rooms = new FilteredElementCollector(_document)
                    .OfCategory(BuiltInCategory.OST_Rooms)
                    .WhereElementIsNotElementType()
                    .Cast<Room>()
                    .Where(r => r.Area > 0)
                    .ToList();

                foreach (var room in rooms)
                {
                    string roomName = room.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "";

                    if (SpaceTypeIdentifier.IsPublicOpenSpace(roomName))
                    {
                        var openSpace = CreateOpenSpaceFromRoom(room, false);
                        if (openSpace != null)
                        {
                            publicSpaces.Add(openSpace);
                        }
                    }
                }

                // Also check Area elements for public open spaces
                // 还检查 Area 元素以查找公共开放空间
                var areas = new FilteredElementCollector(_document)
                    .OfCategory(BuiltInCategory.OST_Areas)
                    .WhereElementIsNotElementType()
                    .Cast<Area>()
                    .Where(a => a.Area > 0)
                    .ToList();

                foreach (var area in areas)
                {
                    string areaName = area.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "";

                    if (SpaceTypeIdentifier.IsPublicOpenSpace(areaName))
                    {
                        var openSpace = CreateOpenSpaceFromArea(area, false);
                        if (openSpace != null)
                        {
                            publicSpaces.Add(openSpace);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting public open spaces: {ex.Message}");
            }

            return publicSpaces.OrderBy(s => s.SpaceType).ThenBy(s => s.Name).ToList();
        }

        /// <summary>
        /// Gets all open spaces (both private and public).
        /// 获取所有开放空间（私有和公共）。
        /// </summary>
        /// <returns>List of all open spaces | 所有开放空间列表</returns>
        public List<OpenSpaceInfo> GetAllOpenSpaces()
        {
            var allSpaces = new List<OpenSpaceInfo>();
            allSpaces.AddRange(GetPrivateOpenSpaces());
            allSpaces.AddRange(GetPublicOpenSpaces());
            return allSpaces;
        }

        /// <summary>
        /// Associates private open spaces with their corresponding units.
        /// 将私有开放空间与其对应的单元关联。
        /// </summary>
        /// <param name="units">List of units | 单元列表</param>
        /// <param name="privateOpenSpaces">List of private open spaces | 私有开放空间列表</param>
        public void AssociateOpenSpacesWithUnits(List<UnitInfo> units, List<OpenSpaceInfo> privateOpenSpaces)
        {
            foreach (var openSpace in privateOpenSpaces)
            {
                if (!string.IsNullOrEmpty(openSpace.AssociatedUnitNumber))
                {
                    var unit = units.FirstOrDefault(u =>
                        u.UnitNumber == openSpace.AssociatedUnitNumber ||
                        u.UnitNumber.Contains(openSpace.AssociatedUnitNumber) ||
                        openSpace.AssociatedUnitNumber.Contains(u.UnitNumber));

                    if (unit != null)
                    {
                        // Add to unit's private open spaces list
                        // 添加到单元的私有开放空间列表
                        unit.PrivateOpenSpaces.Add(openSpace);
                        openSpace.AssociatedUnitId = unit.ElementId;

                        // Update unit area totals based on space type
                        // 根据空间类型更新单元面积总计
                        switch (openSpace.SpaceType)
                        {
                            case OpenSpaceType.Balcony:
                                unit.BalconyArea += openSpace.Area;
                                break;
                            case OpenSpaceType.Patio:
                                unit.PatioArea += openSpace.Area;
                                break;
                            case OpenSpaceType.Terrace:
                            case OpenSpaceType.Deck:
                                unit.TerraceArea += openSpace.Area;
                                break;
                        }

                        // Recalculate sellable area
                        // 重新计算可售面积
                        unit.CalculateSellableArea();
                    }
                }
            }
        }

        #endregion

        #region Private Methods | 私有方法

        /// <summary>
        /// Creates an OpenSpaceInfo object from a Room element.
        /// 从 Room 元素创建 OpenSpaceInfo 对象。
        /// </summary>
        /// <param name="room">Room element | 房间元素</param>
        /// <param name="isPrivate">Whether the space is private | 空间是否私有</param>
        /// <returns>OpenSpaceInfo object | OpenSpaceInfo 对象</returns>
        private OpenSpaceInfo CreateOpenSpaceFromRoom(Room room, bool isPrivate)
        {
            try
            {
                string roomName = room.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "Open Space";
                var (spaceType, _) = SpaceTypeIdentifier.IdentifyOpenSpaceType(roomName);

                var openSpace = new OpenSpaceInfo
                {
                    ElementId = (int)room.Id.Value,
                    Name = roomName,
                    SpaceNumber = room.Number,
                    SpaceType = spaceType,
                    Ownership = isPrivate ? OpenSpaceOwnership.Private : OpenSpaceOwnership.Common,
                    Area = room.Area, // Already in square feet | 已经是平方英尺
                    Perimeter = room.Perimeter
                };

                // Get level
                // 获取楼层
                var level = _document.GetElement(room.LevelId) as Level;
                openSpace.Level = level?.Name ?? "Unknown";

                // Determine if at grade level
                // 确定是否在地面层
                openSpace.IsAtGrade = IsAtGradeLevel(level);

                // Determine if rooftop
                // 确定是否为屋顶
                openSpace.IsRooftop = roomName.ToLower().Contains("roof") ||
                                      roomName.ToLower().Contains("top");

                // Get associated unit number for private spaces
                // 获取私有空间的关联单元编号
                if (isPrivate)
                {
                    openSpace.AssociatedUnitNumber = ExtractAssociatedUnitNumber(room);
                }

                // Get building info
                // 获取建筑信息
                var deptParam = room.get_Parameter(BuiltInParameter.ROOM_DEPARTMENT);
                string dept = deptParam?.AsString() ?? "";
                openSpace.Building = SpaceTypeIdentifier.ExtractBuildingIdentifier(dept);

                // Determine coverage (covered vs uncovered)
                // 确定覆盖情况（有顶 vs 无顶）
                DetermineCoverage(openSpace, roomName);

                return openSpace;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating open space from room: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Creates an OpenSpaceInfo object from an Area element.
        /// 从 Area 元素创建 OpenSpaceInfo 对象。
        /// </summary>
        /// <param name="area">Area element | 区域元素</param>
        /// <param name="isPrivate">Whether the space is private | 空间是否私有</param>
        /// <returns>OpenSpaceInfo object | OpenSpaceInfo 对象</returns>
        private OpenSpaceInfo CreateOpenSpaceFromArea(Area area, bool isPrivate)
        {
            try
            {
                string areaName = area.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "Open Space";
                var (spaceType, _) = SpaceTypeIdentifier.IdentifyOpenSpaceType(areaName);

                var openSpace = new OpenSpaceInfo
                {
                    ElementId = (int)area.Id.Value,
                    Name = areaName,
                    SpaceNumber = area.Number,
                    SpaceType = spaceType,
                    Ownership = isPrivate ? OpenSpaceOwnership.Private : OpenSpaceOwnership.Common,
                    Area = area.Area, // Already in square feet | 已经是平方英尺
                    Perimeter = area.Perimeter
                };

                // Get level from area scheme
                // 从区域方案获取楼层
                var levelParam = area.get_Parameter(BuiltInParameter.ROOM_LEVEL_ID);
                if (levelParam != null)
                {
                    var level = _document.GetElement(levelParam.AsElementId()) as Level;
                    openSpace.Level = level?.Name ?? "Unknown";
                    openSpace.IsAtGrade = IsAtGradeLevel(level);
                }

                // Get associated unit number for private spaces
                // 获取私有空间的关联单元编号
                if (isPrivate)
                {
                    openSpace.AssociatedUnitNumber = SpaceTypeIdentifier.ExtractUnitNumber(areaName);
                }

                // Determine coverage
                // 确定覆盖情况
                DetermineCoverage(openSpace, areaName);

                return openSpace;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating open space from area: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Extracts the associated unit number from a room's parameters.
        /// 从房间参数中提取关联的单元编号。
        /// </summary>
        /// <param name="room">Room element | 房间元素</param>
        /// <returns>Unit number or null | 单元编号或 null</returns>
        private string ExtractAssociatedUnitNumber(Room room)
        {
            // Try custom parameter
            // 尝试自定义参数
            var unitParam = room.LookupParameter("Unit Number") ??
                           room.LookupParameter("Associated Unit") ??
                           room.LookupParameter("Parent Unit");

            if (unitParam != null && unitParam.HasValue)
            {
                string value = unitParam.AsString();
                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            // Try department parameter
            // 尝试部门参数
            var deptParam = room.get_Parameter(BuiltInParameter.ROOM_DEPARTMENT);
            if (deptParam != null && deptParam.HasValue)
            {
                string dept = deptParam.AsString();
                string unitNumber = SpaceTypeIdentifier.ExtractUnitNumber(dept);
                if (!string.IsNullOrEmpty(unitNumber))
                    return unitNumber;
            }

            // Try extracting from room name or number
            // 尝试从房间名称或编号中提取
            string roomName = room.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "";
            string fromName = SpaceTypeIdentifier.ExtractUnitNumber(roomName);
            if (!string.IsNullOrEmpty(fromName))
                return fromName;

            // Try room number
            // 尝试房间编号
            string roomNumber = room.Number;
            return SpaceTypeIdentifier.ExtractUnitNumber(roomNumber);
        }

        /// <summary>
        /// Determines if a level is at grade (ground level).
        /// 确定楼层是否在地面层。
        /// </summary>
        /// <param name="level">Level to check | 要检查的楼层</param>
        /// <returns>True if at grade | 如果在地面层则为 true</returns>
        private bool IsAtGradeLevel(Level level)
        {
            if (level == null)
                return false;

            string levelName = level.Name.ToLower();

            // Check common ground level naming conventions
            // 检查常见的地面层命名约定
            return levelName.Contains("ground") ||
                   levelName.Contains("grade") ||
                   levelName.Contains("level 1") ||
                   levelName.Contains("l1") ||
                   levelName.Contains("first") ||
                   levelName.Contains("lobby") ||
                   level.Elevation < 5; // Within 5 feet of 0 elevation | 在 0 标高 5 英尺以内
        }

        /// <summary>
        /// Determines coverage status of an open space.
        /// 确定开放空间的覆盖状态。
        /// </summary>
        /// <param name="openSpace">Open space to update | 要更新的开放空间</param>
        /// <param name="name">Space name for analysis | 用于分析的空间名称</param>
        private void DetermineCoverage(OpenSpaceInfo openSpace, string name)
        {
            string lowerName = name.ToLower();

            // Determine if covered based on naming
            // 根据命名确定是否有顶
            openSpace.IsCovered = lowerName.Contains("covered") ||
                                  lowerName.Contains("roof") ||
                                  (openSpace.SpaceType == OpenSpaceType.Balcony && !lowerName.Contains("open"));

            // If covered, assume full coverage unless specified
            // 如果有顶，除非另有说明，否则假设完全覆盖
            if (openSpace.IsCovered)
            {
                openSpace.CoveredArea = openSpace.Area;
            }
            else if (lowerName.Contains("partial"))
            {
                // Partial coverage - estimate 50%
                // 部分覆盖 - 估计 50%
                openSpace.CoveredArea = openSpace.Area * 0.5;
            }
            else
            {
                openSpace.CoveredArea = 0;
            }

            // Adjust sellable area multiplier based on coverage
            // 根据覆盖情况调整可售面积乘数
            if (openSpace.IsCovered)
            {
                openSpace.SellableAreaMultiplier = 0.5; // 50% for covered | 有顶为 50%
            }
            else
            {
                openSpace.SellableAreaMultiplier = 0.25; // 25% for uncovered | 无顶为 25%
            }
        }

        #endregion
    }
}
