using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnitSchedulePlugin.Models;

namespace UnitSchedulePlugin.Helpers
{
    /// <summary>
    /// Helper class for identifying and classifying space types based on naming conventions.
    /// Uses pattern matching to determine unit types, room types, and open space classifications.
    ///
    /// 用于根据命名约定识别和分类空间类型的帮助类。
    /// 使用模式匹配来确定单元类型、房间类型和开放空间分类。
    /// </summary>
    public static class SpaceTypeIdentifier
    {
        #region Unit Type Patterns | 单元类型模式

        /// <summary>
        /// Patterns for identifying unit types from room names.
        /// Key: Unit type name, Value: List of regex patterns.
        ///
        /// 用于从房间名称识别单元类型的模式。
        /// 键：单元类型名称，值：正则表达式模式列表。
        /// </summary>
        private static readonly Dictionary<string, List<string>> UnitTypePatterns = new Dictionary<string, List<string>>
        {
            // Studio / 开间
            { "Studio", new List<string> { @"(?i)studio", @"(?i)bachelor", @"(?i)efficiency", @"(?i)\b0\s*br\b", @"(?i)\b0\s*bed" } },

            // 1 Bedroom / 一居室
            { "1BR", new List<string> { @"(?i)\b1\s*br\b", @"(?i)\b1\s*bed", @"(?i)one\s*bed", @"(?i)1-bed" } },

            // 2 Bedroom / 两居室
            { "2BR", new List<string> { @"(?i)\b2\s*br\b", @"(?i)\b2\s*bed", @"(?i)two\s*bed", @"(?i)2-bed" } },

            // 3 Bedroom / 三居室
            { "3BR", new List<string> { @"(?i)\b3\s*br\b", @"(?i)\b3\s*bed", @"(?i)three\s*bed", @"(?i)3-bed" } },

            // 4+ Bedroom / 四居室及以上
            { "4BR+", new List<string> { @"(?i)\b[4-9]\s*br\b", @"(?i)\b[4-9]\s*bed", @"(?i)four\s*bed", @"(?i)4-bed", @"(?i)five\s*bed" } },

            // Penthouse / 顶层公寓
            { "Penthouse", new List<string> { @"(?i)penthouse", @"(?i)pent\s*house", @"(?i)\bph\b" } },

            // Townhouse / 联排别墅
            { "Townhouse", new List<string> { @"(?i)townhouse", @"(?i)town\s*home", @"(?i)row\s*house" } },

            // Loft / 阁楼
            { "Loft", new List<string> { @"(?i)\bloft\b" } },

            // Duplex / 复式
            { "Duplex", new List<string> { @"(?i)duplex", @"(?i)bi-level" } }
        };

        #endregion

        #region Room Type Patterns | 房间类型模式

        /// <summary>
        /// Patterns for identifying individual room types.
        /// 用于识别单个房间类型的模式。
        /// </summary>
        private static readonly Dictionary<string, List<string>> RoomTypePatterns = new Dictionary<string, List<string>>
        {
            { "Bedroom", new List<string> { @"(?i)bed\s*room", @"(?i)\bbed\b", @"(?i)\bbr\b", @"(?i)master\s*bed" } },
            { "Living Room", new List<string> { @"(?i)living", @"(?i)lounge", @"(?i)family\s*room" } },
            { "Kitchen", new List<string> { @"(?i)kitchen", @"(?i)\bkit\b" } },
            { "Bathroom", new List<string> { @"(?i)bath", @"(?i)\bwc\b", @"(?i)toilet", @"(?i)powder", @"(?i)lavatory" } },
            { "Dining", new List<string> { @"(?i)dining", @"(?i)dinner" } },
            { "Closet", new List<string> { @"(?i)closet", @"(?i)wardrobe", @"(?i)storage", @"(?i)walk-in" } },
            { "Entry", new List<string> { @"(?i)entry", @"(?i)foyer", @"(?i)vestibule", @"(?i)hall" } },
            { "Laundry", new List<string> { @"(?i)laundry", @"(?i)utility" } },
            { "Office", new List<string> { @"(?i)office", @"(?i)study", @"(?i)den", @"(?i)home\s*office" } }
        };

        #endregion

        #region Open Space Patterns | 开放空间模式

        /// <summary>
        /// Patterns for identifying open space types (private).
        /// 用于识别开放空间类型（私有）的模式。
        /// </summary>
        private static readonly Dictionary<OpenSpaceType, List<string>> PrivateOpenSpacePatterns = new Dictionary<OpenSpaceType, List<string>>
        {
            { OpenSpaceType.Balcony, new List<string> { @"(?i)balcony", @"(?i)balcon", @"(?i)\bjuliet\b" } },
            { OpenSpaceType.Patio, new List<string> { @"(?i)patio", @"(?i)yard", @"(?i)private\s*garden" } },
            { OpenSpaceType.Terrace, new List<string> { @"(?i)terrace", @"(?i)roof\s*deck", @"(?i)private\s*deck" } },
            { OpenSpaceType.Deck, new List<string> { @"(?i)\bdeck\b" } }
        };

        /// <summary>
        /// Patterns for identifying open space types (public/common).
        /// 用于识别开放空间类型（公共/共享）的模式。
        /// </summary>
        private static readonly Dictionary<OpenSpaceType, List<string>> PublicOpenSpacePatterns = new Dictionary<OpenSpaceType, List<string>>
        {
            { OpenSpaceType.Courtyard, new List<string> { @"(?i)courtyard", @"(?i)court\s*yard", @"(?i)atrium" } },
            { OpenSpaceType.Garden, new List<string> { @"(?i)garden", @"(?i)green\s*space", @"(?i)landscape" } },
            { OpenSpaceType.Rooftop, new List<string> { @"(?i)rooftop", @"(?i)roof\s*top", @"(?i)sky\s*deck", @"(?i)roof\s*amenity" } },
            { OpenSpaceType.PoolArea, new List<string> { @"(?i)pool", @"(?i)swimming", @"(?i)aquatic" } },
            { OpenSpaceType.Playground, new List<string> { @"(?i)playground", @"(?i)play\s*area", @"(?i)kids", @"(?i)children" } },
            { OpenSpaceType.Plaza, new List<string> { @"(?i)plaza", @"(?i)piazza", @"(?i)square" } },
            { OpenSpaceType.Amenity, new List<string> { @"(?i)amenity", @"(?i)common\s*area", @"(?i)recreation", @"(?i)bbq", @"(?i)outdoor\s*lounge" } }
        };

        #endregion

        #region Unit Type Identification | 单元类型识别

        /// <summary>
        /// Identifies the unit type based on the room/unit name.
        /// 根据房间/单元名称识别单元类型。
        /// </summary>
        /// <param name="name">Name of the room or unit | 房间或单元的名称</param>
        /// <returns>Identified unit type or "Unknown" | 识别的单元类型或"Unknown"</returns>
        public static string IdentifyUnitType(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Unknown";

            foreach (var typeEntry in UnitTypePatterns)
            {
                foreach (var pattern in typeEntry.Value)
                {
                    if (Regex.IsMatch(name, pattern))
                        return typeEntry.Key;
                }
            }

            return "Unknown";
        }

        /// <summary>
        /// Determines unit type based on bedroom count.
        /// 根据卧室数量确定单元类型。
        /// </summary>
        /// <param name="bedroomCount">Number of bedrooms | 卧室数量</param>
        /// <returns>Unit type string | 单元类型字符串</returns>
        public static string GetUnitTypeByBedroomCount(int bedroomCount)
        {
            switch (bedroomCount)
            {
                case 0: return "Studio";
                case 1: return "1BR";
                case 2: return "2BR";
                case 3: return "3BR";
                default: return bedroomCount >= 4 ? "4BR+" : "Unknown";
            }
        }

        #endregion

        #region Room Type Identification | 房间类型识别

        /// <summary>
        /// Identifies the room type based on the room name.
        /// 根据房间名称识别房间类型。
        /// </summary>
        /// <param name="name">Name of the room | 房间名称</param>
        /// <returns>Identified room type or "Other" | 识别的房间类型或"Other"</returns>
        public static string IdentifyRoomType(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Other";

            foreach (var typeEntry in RoomTypePatterns)
            {
                foreach (var pattern in typeEntry.Value)
                {
                    if (Regex.IsMatch(name, pattern))
                        return typeEntry.Key;
                }
            }

            return "Other";
        }

        /// <summary>
        /// Determines if a room name indicates a bedroom.
        /// 确定房间名称是否表示卧室。
        /// </summary>
        /// <param name="name">Room name | 房间名称</param>
        /// <returns>True if bedroom | 如果是卧室则为 true</returns>
        public static bool IsBedroom(string name)
        {
            return IdentifyRoomType(name) == "Bedroom";
        }

        /// <summary>
        /// Determines if a room name indicates a bathroom.
        /// 确定房间名称是否表示浴室。
        /// </summary>
        /// <param name="name">Room name | 房间名称</param>
        /// <returns>True if bathroom | 如果是浴室则为 true</returns>
        public static bool IsBathroom(string name)
        {
            return IdentifyRoomType(name) == "Bathroom";
        }

        #endregion

        #region Open Space Identification | 开放空间识别

        /// <summary>
        /// Identifies the open space type based on the space name.
        /// 根据空间名称识别开放空间类型。
        /// </summary>
        /// <param name="name">Name of the space | 空间名称</param>
        /// <returns>Tuple of (OpenSpaceType, IsPrivate) | （开放空间类型，是否私有）元组</returns>
        public static (OpenSpaceType Type, bool IsPrivate) IdentifyOpenSpaceType(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return (OpenSpaceType.Other, false);

            // Check private patterns first | 首先检查私有模式
            foreach (var typeEntry in PrivateOpenSpacePatterns)
            {
                foreach (var pattern in typeEntry.Value)
                {
                    if (Regex.IsMatch(name, pattern))
                        return (typeEntry.Key, true);
                }
            }

            // Check public patterns | 检查公共模式
            foreach (var typeEntry in PublicOpenSpacePatterns)
            {
                foreach (var pattern in typeEntry.Value)
                {
                    if (Regex.IsMatch(name, pattern))
                        return (typeEntry.Key, false);
                }
            }

            return (OpenSpaceType.Other, false);
        }

        /// <summary>
        /// Determines if a space name indicates a private open space.
        /// 确定空间名称是否表示私有开放空间。
        /// </summary>
        /// <param name="name">Space name | 空间名称</param>
        /// <returns>True if private open space | 如果是私有开放空间则为 true</returns>
        public static bool IsPrivateOpenSpace(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            foreach (var patterns in PrivateOpenSpacePatterns.Values)
            {
                foreach (var pattern in patterns)
                {
                    if (Regex.IsMatch(name, pattern))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if a space name indicates a public/common open space.
        /// 确定空间名称是否表示公共/共享开放空间。
        /// </summary>
        /// <param name="name">Space name | 空间名称</param>
        /// <returns>True if public open space | 如果是公共开放空间则为 true</returns>
        public static bool IsPublicOpenSpace(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            foreach (var patterns in PublicOpenSpacePatterns.Values)
            {
                foreach (var pattern in patterns)
                {
                    if (Regex.IsMatch(name, pattern))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if a space name indicates any type of open space.
        /// 确定空间名称是否表示任何类型的开放空间。
        /// </summary>
        /// <param name="name">Space name | 空间名称</param>
        /// <returns>True if open space | 如果是开放空间则为 true</returns>
        public static bool IsOpenSpace(string name)
        {
            return IsPrivateOpenSpace(name) || IsPublicOpenSpace(name);
        }

        #endregion

        #region Unit Number Extraction | 单元编号提取

        /// <summary>
        /// Extracts unit number from a room or space name.
        /// Common formats: "Unit 101", "101-Living", "A-201", etc.
        ///
        /// 从房间或空间名称中提取单元编号。
        /// 常见格式："Unit 101"、"101-Living"、"A-201"等。
        /// </summary>
        /// <param name="name">Name containing unit number | 包含单元编号的名称</param>
        /// <returns>Extracted unit number or null | 提取的单元编号或 null</returns>
        public static string ExtractUnitNumber(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            // Pattern: "Unit XXX" or "UNIT XXX" | 模式："Unit XXX" 或 "UNIT XXX"
            var unitMatch = Regex.Match(name, @"(?i)unit\s*[#]?\s*([A-Za-z]?\d+[A-Za-z]?)");
            if (unitMatch.Success)
                return unitMatch.Groups[1].Value;

            // Pattern: Leading number with optional letter prefix "A-101", "101", "B2-301"
            // 模式：带可选字母前缀的前导数字 "A-101"、"101"、"B2-301"
            var leadingMatch = Regex.Match(name, @"^([A-Za-z]?\d*[-]?\d+)");
            if (leadingMatch.Success && leadingMatch.Groups[1].Value.Any(char.IsDigit))
                return leadingMatch.Groups[1].Value;

            // Pattern: Number in parentheses "(101)" | 模式：括号中的数字"(101)"
            var parenMatch = Regex.Match(name, @"\(([A-Za-z]?\d+[A-Za-z]?)\)");
            if (parenMatch.Success)
                return parenMatch.Groups[1].Value;

            return null;
        }

        /// <summary>
        /// Extracts building identifier from a name or parameter.
        /// 从名称或参数中提取建筑标识符。
        /// </summary>
        /// <param name="name">Name potentially containing building ID | 可能包含建筑 ID 的名称</param>
        /// <returns>Building identifier or null | 建筑标识符或 null</returns>
        public static string ExtractBuildingIdentifier(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            // Pattern: "Building A", "Tower 1", "Block B" | 模式："Building A"、"Tower 1"、"Block B"
            var buildingMatch = Regex.Match(name, @"(?i)(?:building|tower|block)\s*([A-Za-z0-9]+)");
            if (buildingMatch.Success)
                return buildingMatch.Groups[1].Value;

            // Pattern: Single letter prefix before number "A-101" -> "A"
            // 模式：数字前的单字母前缀 "A-101" -> "A"
            var prefixMatch = Regex.Match(name, @"^([A-Za-z])[-]?\d");
            if (prefixMatch.Success)
                return prefixMatch.Groups[1].Value;

            return null;
        }

        #endregion

        #region Validation | 验证

        /// <summary>
        /// Validates if a name represents a residential unit (not circulation, mechanical, etc.).
        /// 验证名称是否表示住宅单元（而非交通、机械等空间）。
        /// </summary>
        /// <param name="name">Space name | 空间名称</param>
        /// <returns>True if likely residential unit | 如果可能是住宅单元则为 true</returns>
        public static bool IsResidentialSpace(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Exclude patterns for non-residential spaces
            // 排除非住宅空间的模式
            var excludePatterns = new[]
            {
                @"(?i)corridor", @"(?i)hallway", @"(?i)stair", @"(?i)elevator",
                @"(?i)mechanical", @"(?i)electrical", @"(?i)telecom", @"(?i)janitor",
                @"(?i)trash", @"(?i)garbage", @"(?i)service", @"(?i)shaft",
                @"(?i)parking", @"(?i)garage", @"(?i)lobby", @"(?i)reception",
                @"(?i)mail\s*room", @"(?i)loading", @"(?i)dock"
            };

            foreach (var pattern in excludePatterns)
            {
                if (Regex.IsMatch(name, pattern))
                    return false;
            }

            return true;
        }

        #endregion
    }
}
