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
    /// Service for scanning and extracting unit information from Revit documents.
    /// Analyzes rooms and areas to build comprehensive unit schedules.
    ///
    /// 用于从 Revit 文档扫描和提取单元信息的服务。
    /// 分析房间和区域以构建全面的单元时间表。
    /// </summary>
    public class UnitScanner
    {
        #region Fields | 字段

        private readonly Document _document;
        private readonly Dictionary<string, List<Room>> _roomsByUnit;

        #endregion

        #region Constructor | 构造函数

        /// <summary>
        /// Initializes a new instance of the UnitScanner class.
        /// 初始化 UnitScanner 类的新实例。
        /// </summary>
        /// <param name="document">Revit document to scan | 要扫描的 Revit 文档</param>
        public UnitScanner(Document document)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _roomsByUnit = new Dictionary<string, List<Room>>();
        }

        #endregion

        #region Public Methods | 公共方法

        /// <summary>
        /// Scans all units in the document and returns unit information.
        /// 扫描文档中的所有单元并返回单元信息。
        /// </summary>
        /// <returns>List of unit information | 单元信息列表</returns>
        public List<UnitInfo> ScanAllUnits()
        {
            var units = new List<UnitInfo>();

            try
            {
                // Collect all rooms from the document
                // 从文档中收集所有房间
                var rooms = new FilteredElementCollector(_document)
                    .OfCategory(BuiltInCategory.OST_Rooms)
                    .WhereElementIsNotElementType()
                    .Cast<Room>()
                    .Where(r => r.Area > 0) // Only placed rooms with area | 仅放置的有面积的房间
                    .ToList();

                // Group rooms by unit number
                // 按单元编号对房间进行分组
                GroupRoomsByUnit(rooms);

                // Process each unit group
                // 处理每个单元组
                foreach (var unitGroup in _roomsByUnit)
                {
                    var unitInfo = ProcessUnitGroup(unitGroup.Key, unitGroup.Value);
                    if (unitInfo != null)
                    {
                        units.Add(unitInfo);
                    }
                }

                // Also check for Area elements (for gross area calculations)
                // 还检查 Area 元素（用于建筑面积计算）
                ProcessAreaElements(units);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error scanning units: {ex.Message}");
            }

            return units.OrderBy(u => u.Level).ThenBy(u => u.UnitNumber).ToList();
        }

        /// <summary>
        /// Gets detailed information about a specific unit.
        /// 获取特定单元的详细信息。
        /// </summary>
        /// <param name="unitNumber">Unit number to query | 要查询的单元编号</param>
        /// <returns>Unit information or null | 单元信息或 null</returns>
        public UnitInfo GetUnitDetails(string unitNumber)
        {
            if (!_roomsByUnit.ContainsKey(unitNumber))
                return null;

            return ProcessUnitGroup(unitNumber, _roomsByUnit[unitNumber]);
        }

        #endregion

        #region Private Methods | 私有方法

        /// <summary>
        /// Groups rooms by their unit number for processing.
        /// 按单元编号对房间进行分组以进行处理。
        /// </summary>
        /// <param name="rooms">List of rooms to group | 要分组的房间列表</param>
        private void GroupRoomsByUnit(List<Room> rooms)
        {
            _roomsByUnit.Clear();

            foreach (var room in rooms)
            {
                // Skip non-residential spaces
                // 跳过非住宅空间
                string roomName = room.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "";
                if (!SpaceTypeIdentifier.IsResidentialSpace(roomName))
                    continue;

                // Skip open spaces (handled separately)
                // 跳过开放空间（单独处理）
                if (SpaceTypeIdentifier.IsOpenSpace(roomName))
                    continue;

                // Extract unit number from room
                // 从房间提取单元编号
                string unitNumber = ExtractUnitNumberFromRoom(room);

                if (string.IsNullOrEmpty(unitNumber))
                {
                    // Try to create a unit number from room number
                    // 尝试从房间编号创建单元编号
                    unitNumber = room.Number;
                }

                if (!string.IsNullOrEmpty(unitNumber))
                {
                    if (!_roomsByUnit.ContainsKey(unitNumber))
                    {
                        _roomsByUnit[unitNumber] = new List<Room>();
                    }
                    _roomsByUnit[unitNumber].Add(room);
                }
            }
        }

        /// <summary>
        /// Extracts the unit number from room parameters or naming conventions.
        /// 从房间参数或命名约定中提取单元编号。
        /// </summary>
        /// <param name="room">Room element | 房间元素</param>
        /// <returns>Unit number or null | 单元编号或 null</returns>
        private string ExtractUnitNumberFromRoom(Room room)
        {
            // Try custom "Unit Number" parameter first
            // 首先尝试自定义"单元编号"参数
            var unitParam = room.LookupParameter("Unit Number") ??
                           room.LookupParameter("Unit_Number") ??
                           room.LookupParameter("UnitNumber") ??
                           room.LookupParameter("Apartment Number");

            if (unitParam != null && unitParam.HasValue)
            {
                string value = unitParam.AsString();
                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            // Try Department parameter
            // 尝试部门参数
            var deptParam = room.get_Parameter(BuiltInParameter.ROOM_DEPARTMENT);
            if (deptParam != null && deptParam.HasValue)
            {
                string dept = deptParam.AsString();
                string extracted = SpaceTypeIdentifier.ExtractUnitNumber(dept);
                if (!string.IsNullOrEmpty(extracted))
                    return extracted;
            }

            // Try extracting from room name
            // 尝试从房间名称中提取
            string roomName = room.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString();
            string fromName = SpaceTypeIdentifier.ExtractUnitNumber(roomName);
            if (!string.IsNullOrEmpty(fromName))
                return fromName;

            // Try room number prefix
            // 尝试房间编号前缀
            string roomNumber = room.Number;
            if (!string.IsNullOrEmpty(roomNumber) && roomNumber.Length >= 3)
            {
                // Extract first 3-4 digits as unit number (e.g., "101A" from "101A-LR")
                // 提取前 3-4 位数字作为单元编号（例如，从"101A-LR"中提取"101A"）
                var match = System.Text.RegularExpressions.Regex.Match(roomNumber, @"^(\d{3,4}[A-Za-z]?)");
                if (match.Success)
                    return match.Groups[1].Value;
            }

            return null;
        }

        /// <summary>
        /// Processes a group of rooms belonging to a single unit.
        /// 处理属于单个单元的一组房间。
        /// </summary>
        /// <param name="unitNumber">Unit number | 单元编号</param>
        /// <param name="rooms">Rooms in the unit | 单元中的房间</param>
        /// <returns>Processed unit information | 处理后的单元信息</returns>
        private UnitInfo ProcessUnitGroup(string unitNumber, List<Room> rooms)
        {
            if (rooms == null || !rooms.Any())
                return null;

            var firstRoom = rooms.First();
            var unit = new UnitInfo
            {
                ElementId = (int)firstRoom.Id.Value,
                UnitNumber = unitNumber,
                Name = GetUnitName(rooms)
            };

            // Calculate areas (Revit returns area in square feet by default)
            // 计算面积（Revit 默认以平方英尺返回面积）
            unit.NetArea = rooms.Sum(r => r.Area);
            unit.GrossArea = unit.NetArea * 1.15; // Approximate gross (walls ~15%) | 近似总面积（墙体约 15%）

            // Get level information
            // 获取楼层信息
            var level = _document.GetElement(firstRoom.LevelId) as Level;
            unit.Level = level?.Name ?? "Unknown";

            // Get building/department info
            // 获取建筑/部门信息
            var deptParam = firstRoom.get_Parameter(BuiltInParameter.ROOM_DEPARTMENT);
            unit.Department = deptParam?.AsString();
            unit.Building = SpaceTypeIdentifier.ExtractBuildingIdentifier(unit.Department ?? unit.UnitNumber);

            // Count bedrooms and bathrooms
            // 统计卧室和浴室数量
            unit.BedroomCount = rooms.Count(r =>
            {
                string name = r.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "";
                return SpaceTypeIdentifier.IsBedroom(name);
            });

            unit.BathroomCount = rooms.Count(r =>
            {
                string name = r.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "";
                return SpaceTypeIdentifier.IsBathroom(name);
            });

            // Determine unit type
            // 确定单元类型
            unit.UnitType = DetermineUnitType(rooms, unit.BedroomCount);

            // Add room details
            // 添加房间详情
            foreach (var room in rooms)
            {
                string roomName = room.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "Room";
                unit.Rooms.Add(new RoomDetail
                {
                    ElementId = (int)room.Id.Value,
                    Name = roomName,
                    RoomType = SpaceTypeIdentifier.IdentifyRoomType(roomName),
                    Area = room.Area
                });
            }

            // Get phase
            // 获取阶段
            var phaseParam = firstRoom.get_Parameter(BuiltInParameter.ROOM_PHASE);
            if (phaseParam != null)
            {
                var phase = _document.GetElement(phaseParam.AsElementId()) as Phase;
                unit.Phase = phase?.Name;
            }

            // Calculate sellable area
            // 计算可售面积
            unit.CalculateSellableArea();

            return unit;
        }

        /// <summary>
        /// Gets a display name for the unit from its rooms.
        /// 从房间获取单元的显示名称。
        /// </summary>
        /// <param name="rooms">Rooms in the unit | 单元中的房间</param>
        /// <returns>Unit display name | 单元显示名称</returns>
        private string GetUnitName(List<Room> rooms)
        {
            // Look for a "main" room name or use first room
            // 查找"主"房间名称或使用第一个房间
            var livingRoom = rooms.FirstOrDefault(r =>
            {
                string name = r.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "";
                return name.ToLower().Contains("living") || name.ToLower().Contains("main");
            });

            if (livingRoom != null)
            {
                var deptParam = livingRoom.get_Parameter(BuiltInParameter.ROOM_DEPARTMENT);
                if (deptParam != null && deptParam.HasValue)
                    return deptParam.AsString();
            }

            return rooms.First().get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "Unit";
        }

        /// <summary>
        /// Determines the unit type based on rooms and bedroom count.
        /// 根据房间和卧室数量确定单元类型。
        /// </summary>
        /// <param name="rooms">Rooms in the unit | 单元中的房间</param>
        /// <param name="bedroomCount">Number of bedrooms | 卧室数量</param>
        /// <returns>Unit type string | 单元类型字符串</returns>
        private string DetermineUnitType(List<Room> rooms, int bedroomCount)
        {
            // First check room names for explicit type
            // 首先检查房间名称中的显式类型
            foreach (var room in rooms)
            {
                string roomName = room.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "";
                string identifiedType = SpaceTypeIdentifier.IdentifyUnitType(roomName);
                if (identifiedType != "Unknown")
                    return identifiedType;

                // Check department parameter
                // 检查部门参数
                string dept = room.get_Parameter(BuiltInParameter.ROOM_DEPARTMENT)?.AsString() ?? "";
                identifiedType = SpaceTypeIdentifier.IdentifyUnitType(dept);
                if (identifiedType != "Unknown")
                    return identifiedType;
            }

            // Fall back to bedroom count
            // 回退到卧室数量
            return SpaceTypeIdentifier.GetUnitTypeByBedroomCount(bedroomCount);
        }

        /// <summary>
        /// Processes Area elements for additional gross area information.
        /// 处理 Area 元素以获取额外的建筑面积信息。
        /// </summary>
        /// <param name="units">List of units to update | 要更新的单元列表</param>
        private void ProcessAreaElements(List<UnitInfo> units)
        {
            try
            {
                // Collect area elements
                // 收集区域元素
                var areas = new FilteredElementCollector(_document)
                    .OfCategory(BuiltInCategory.OST_Areas)
                    .WhereElementIsNotElementType()
                    .Cast<Area>()
                    .Where(a => a.Area > 0)
                    .ToList();

                foreach (var area in areas)
                {
                    string areaName = area.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString() ?? "";
                    string unitNumber = SpaceTypeIdentifier.ExtractUnitNumber(areaName);

                    if (!string.IsNullOrEmpty(unitNumber))
                    {
                        var unit = units.FirstOrDefault(u => u.UnitNumber == unitNumber);
                        if (unit != null)
                        {
                            // Update gross area from Area element
                            // 从 Area 元素更新建筑面积
                            unit.GrossArea = area.Area;
                            unit.CalculateSellableArea();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error processing areas: {ex.Message}");
            }
        }

        #endregion
    }
}
