# Revit Unit Schedule Plugin - Product Requirements Document (PRD)

## Project Overview

**Project Name:** Unit Schedule Generator  
**Target Users:** Architects/BIM Engineers working on Multi-family and Mixed-use projects  
**Development Purpose:** Portfolio project to showcase Revit plugin development skills for job hunting  
**Development Goal:** Fast implementation - get working prototype ASAP

---

## Tech Stack

- **Language:** C# (.NET Framework 4.8)
- **UI Framework:** WPF (Windows Presentation Foundation)
- **Target Revit Version:** Revit 2025
- **Chart Library:** LiveCharts2 (Modern, GPU-accelerated charts via NuGet)
- **Development Environment:** VS Code + Claude Code (Development) + Visual Studio 2022 (Debugging)
- **Units:** Imperial (Square Feet) - No conversion needed
- **UI Language:** English
- **Code Comments:** Bilingual (English + Chinese)

---

## Core Feature Requirements

### 1. Unit Recognition & Data Collection
Automatically scan current Revit project for all Rooms/Spaces and collect:
- **Unit Type Identification:** Studio, 1B1B, 2B2B, 3B2B, etc.
  - Via Room Name (e.g., "Unit-A-2B2B")
  - Via Room Comments
  - Via custom parameters (if "Unit Type" parameter exists)
- **Unit Area:** Interior living space (SF)
- **Private Open Space:** Balconies, patios, terraces assigned to specific units (SF)
- **Public Open Space:** Shared amenity spaces, courtyards, rooftop decks (SF)
- **Floor Location:** Which level the unit is on

### 2. Data Statistics & Classification
Statistical Dimensions:
- **By Unit Type:** Count, total area, avg area per unit type
- **By Floor:** Distribution of units per floor
- **Open Space Analysis:**
  - Total private open space per unit type
  - Average private open space per unit
  - Total public open space in project
  - Ratio of open space to living area
- **Summary Metrics:**
  - Total units count
  - Total residential area
  - Total private open space
  - Total public open space
  - Open space percentage

### 3. User Interface Design

#### Main Window Layout (WPF) - English UI
```
┌──────────────────────────────────────────────────────────────────┐
│  Unit Schedule Generator                                     [X] │
├──────────────────────────────────────────────────────────────────┤
│  [Scan Project] [Export Excel] [Refresh]         🏢 Project: ABC │
├─────────────────────────┬────────────────────────────────────────┤
│  📊 UNIT STATISTICS     │  📈 VISUALIZATIONS                     │
│  ─────────────────────  │  ──────────────────                    │
│  Unit Type | Count | SF │                                        │
│  Studio    |  12   |... │  [Modern Donut Chart: Unit Mix]        │
│  1B1B      |  24   |... │   • Animated                           │
│  2B2B      |  36   |... │   • Gradient colors                    │
│  ─────────────────────  │   • Interactive tooltips               │
│                         │                                        │
│  🌳 OPEN SPACE          │  [Stacked Bar Chart: By Floor]         │
│  ─────────────────────  │   • Modern flat design                 │
│  Private: 2,400 SF      │   • Color-coded by unit type           │
│  Public:  1,800 SF      │                                        │
│  Ratio:   15.2%         │  [Area Comparison Chart]               │
│                         │   • Living vs Open Space               │
└─────────────────────────┴────────────────────────────────────────┘
```

#### UI Design Requirements
- **Color Scheme (Modern & Professional):**
  - Primary: Deep Blue (#1E3A8A) - Professional corporate feel
  - Accent 1: Teal (#14B8A6) - For open space metrics
  - Accent 2: Amber (#F59E0B) - For unit counts
  - Success: Green (#10B981) - For completion states
  - Background: Soft Gray (#F8FAFC)
  - Cards: White (#FFFFFF) with subtle shadows
  
- **Visual Effects:**
  - Frosted glass effect (acrylic) on panels
  - Rounded corners (8px radius)
  - Soft drop shadows
  - Smooth animations on hover/click
  - Gradient backgrounds for chart areas
  
- **Typography:**
  - Headers: Segoe UI Semibold, 16-18pt
  - Body: Segoe UI Regular, 12-14pt
  - Metrics: Segoe UI Bold, 20-24pt
  
- **Responsive:**
  - Minimum size: 1000x700
  - Resizable with adaptive layout
  - Grid system for proper spacing

### 4. Chart Visualizations (Modern Design)

#### Chart 1: Unit Mix Donut Chart
- Modern donut chart (not traditional pie chart)
- Animated entry effect
- Gradient fills for each segment
- Interactive tooltips showing:
  - Unit type name
  - Count and percentage
  - Total area
  - Avg private open space
- Color palette: Vibrant gradients

#### Chart 2: Floor Distribution Stacked Bar
- Horizontal stacked bars
- Each floor shows unit type breakdown
- Color-coded by unit type
- Labels show unit counts
- Smooth animations

#### Chart 3: Area Comparison Chart
- Side-by-side bar comparison per unit type
- Shows: Living Area vs Private Open Space
- Uses pattern fills or gradients
- Clear legend

#### Chart 4: Open Space Metrics Gauge (Optional)
- Radial gauge showing open space ratio
- Target line at 15%
- Color changes based on achievement

### 5. Export Functionality
- Export to Excel (.xlsx) format
- Includes:
  - Summary metrics sheet
  - Detailed unit breakdown
  - Open space analysis
  - Charts as images (optional)
  - Auto-formatted with color coding

---

## Technical Implementation Details

### Revit API Core Logic
```csharp
// 核心功能实现 / Core functionality implementation
1. Get all Rooms: FilteredElementCollector
2. Extract unit info: Room.get_Parameter()
3. Calculate area: Room.Area (already in Square Feet - no conversion needed!)
4. Floor info: Room.Level.Name
5. Identify open space:
   - Check Room Category or Name contains "Balcony", "Patio", "Terrace" → Private
   - Check Room Category or Name contains "Amenity", "Courtyard", "Rooftop" → Public
   - Or use custom parameter "Space Type"
```

### Space Type Identification Rules
```csharp
// 开放空间识别规则 / Open space identification rules
- Private Open Space:
  - Room Name contains: "Balcony", "Patio", "Terrace", "Deck"
  - Room Category: "Outdoor" or custom
  - Adjacent to specific unit (check bounding box)
  
- Public Open Space:
  - Room Name contains: "Amenity", "Courtyard", "Rooftop", "Garden", "Lobby"
  - Shared spaces not assigned to individual units
  
- Living Space:
  - All other rooms within unit boundaries
```

### Code Comments Requirement
**CRITICAL:** All code must have bilingual comments:
```csharp
// Example format 示例格式:

// Scan all rooms in the project 扫描项目中所有房间
FilteredElementCollector collector = new FilteredElementCollector(doc)
    .OfCategory(BuiltInCategory.OST_Rooms)
    .WhereElementIsNotElementType();

// Calculate total area 计算总面积
double totalArea = 0; // Square feet 平方英尺
foreach (Room room in rooms)
{
    totalArea += room.Area; // Already in SF 已经是平方英尺单位
}
```

### WPF Data Binding (MVVM Pattern)
- Use ObservableCollection for dynamic updates
- INotifyPropertyChanged for property changes
- Commands for button actions
- Data templates for list items

### Chart Library Integration
- Use **LiveCharts2** (NOT LiveCharts v0.9)
- Install via NuGet: `Install-Package LiveChartsCore.SkiaSharpView.WPF`
- Modern, GPU-accelerated rendering
- Built-in animations and interactions

---

## File Structure

```
UnitSchedulePlugin/
├── UnitSchedulePlugin.csproj
├── Command.cs                      # Revit External Command entry point
├── Application.cs                  # Revit External Application (optional)
├── MainWindow.xaml                 # WPF main window UI (English)
├── MainWindow.xaml.cs              # WPF code-behind
├── Models/
│   ├── UnitInfo.cs                # Unit information data model
│   ├── OpenSpaceInfo.cs           # Open space data model (NEW)
│   └── ProjectStatistics.cs       # Overall project statistics (NEW)
├── ViewModels/
│   └── MainViewModel.cs           # MVVM ViewModel with bilingual comments
├── Services/
│   ├── UnitScanner.cs             # Scans Revit project for units
│   ├── OpenSpaceAnalyzer.cs       # Analyzes open spaces (NEW)
│   └── ExcelExporter.cs           # Excel export service
├── Helpers/
│   └── SpaceTypeIdentifier.cs     # Helper to identify space types (NEW)
├── Resources/
│   ├── Icons/                     # Icon resources
│   └── Styles.xaml                # WPF styles and colors (NEW)
└── UnitSchedulePlugin.addin       # Revit plugin manifest file
```

---

## .addin File Configuration

```xml
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
  <AddIn Type="Command">
    <Text>Unit Schedule Generator</Text>
    <Description>Automatically generate unit schedules with open space analysis for multi-family projects</Description>
    <Assembly>UnitSchedulePlugin.dll</Assembly>
    <FullClassName>UnitSchedulePlugin.Command</FullClassName>
    <ClientId>12345678-1234-1234-1234-123456789ABC</ClientId>
    <VendorId>YourCompany</VendorId>
    <VendorDescription>Your Name - BIM Developer</VendorDescription>
  </AddIn>
</RevitAddIns>
```

**Note:** Generate a new GUID for ClientId using: `[System.Guid]::NewGuid()` in PowerShell

---

## Development Priority (Fast Implementation)

### 🚀 Phase 1: Minimal Working Version (Get it running FIRST!)
**Goal:** See something working in Revit immediately

1. ✅ Create Visual Studio project with references
2. Create Command.cs - implement IExternalCommand
3. Create basic WPF window (just a button and text)
4. Test plugin loads in Revit and opens window
5. **Stop and verify** - Can you click your plugin button in Revit?

### 🎯 Phase 2: Core Data Collection
**Goal:** Get real data from Revit

1. Implement UnitScanner service
   - Scan all Rooms
   - Extract unit type from Room Name/Comments
   - Calculate area (already in SF)
   - Group by unit type
2. Display data in simple DataGrid
3. **Stop and verify** - Can you see your unit data in the window?

### 🎨 Phase 3: Add Open Space Analysis
**Goal:** Add the differentiating feature

1. Create OpenSpaceAnalyzer service
2. Identify private open spaces (balconies, patios)
3. Identify public open spaces (amenities)
4. Add columns to DataGrid for open space
5. **Stop and verify** - Does open space data show correctly?

### 📊 Phase 4: Add Charts (Modern UI)
**Goal:** Make it visually impressive

1. Install LiveCharts2 via NuGet
2. Add donut chart for unit mix
3. Add stacked bar chart for floor distribution
4. Apply color scheme
5. **Stop and verify** - Do charts look good and animate?

### 💾 Phase 5: Export to Excel
**Goal:** Make it practical

1. Install EPPlus or ClosedXML
2. Implement Excel export
3. Format the output
4. **Done!**

---

## Success Criteria (Must-Haves)

✅ Loads successfully in Revit 2025  
✅ Scans and identifies at least 3 unit types  
✅ Calculates private open space per unit  
✅ Calculates total public open space  
✅ Shows data in English UI  
✅ Modern charts with smooth animations  
✅ Professional color scheme applied  
✅ Can export to Excel  
✅ Code has bilingual comments  
✅ No major bugs, handles common errors  

---

## Time Estimate

- **Absolute minimum (MVP):** 3-4 hours (basic stats + simple UI)
- **Full featured version:** 8-10 hours (everything including modern charts)
- **Working in focused sessions:** Should be done in 1-2 days

---

## Debugging Setup

### Visual Studio Debug Configuration
1. Right-click project → Properties
2. Debug tab
3. Start Action: **Start external program**
4. Path: `C:\Program Files\Autodesk\Revit 2025\Revit.exe`
5. Working directory: (leave empty or set to project output)

### Plugin Installation Location
Copy `.addin` file and compiled `.dll` to:
```
C:\ProgramData\Autodesk\Revit\Addins\2025\
```

**Tip:** Set up post-build event to auto-copy files:
```
xcopy "$(TargetPath)" "C:\ProgramData\Autodesk\Revit\Addins\2025\" /Y
xcopy "$(ProjectDir)$(ProjectName).addin" "C:\ProgramData\Autodesk\Revit\Addins\2025\" /Y
```

---

## Showcase Highlights (for Job Interviews)

When presenting this project, emphasize:

1. **Understanding of Multi-Family Workflow:**
   - "I noticed that tracking open space is critical for multi-family projects due to zoning requirements and amenity ratios"
   - "Private balconies directly affect unit valuations"
   - Shows you understand the business, not just the tech

2. **Technical Skills Demonstrated:**
   - Proficient with Revit API (FilteredElementCollector, Parameters, Areas)
   - Modern WPF development with MVVM pattern
   - Data visualization and chart implementation
   - Excel integration for deliverables
   - Clean code architecture with separation of concerns

3. **User Experience Focus:**
   - Professional English UI suitable for client presentations
   - Modern, animated charts that make data engaging
   - Color-coded information for quick comprehension
   - Export functionality for easy sharing

4. **Code Quality:**
   - Bilingual comments show consideration for diverse teams
   - Structured project organization
   - Error handling for edge cases
   - Uses imperial units (practical for US market)

5. **Problem-Solving Approach:**
   - Identified a real pain point in multi-family projects
   - Built a practical solution, not just a demo
   - Thought about the full workflow (scan → analyze → export)

---

## Optional Enhancements (If You Have Extra Time)

- Filter by specific floors
- Customizable unit type naming rules
- PDF report generation with charts
- Integration with Revit Schedules (auto-create/update)
- Historical data comparison between project versions
- Calculate open space per capita ratios
- Highlight units that don't meet minimum open space requirements

---

## Technical Challenges & Solutions

### 1. Unit Identification
**Challenge:** Different projects use different naming conventions  
**Solution:** Support multiple detection methods (Name, Comments, Parameters)  
**Code pattern:**
```csharp
// Try multiple sources 尝试多个数据源
string unitType = GetParameter(room, "Unit Type") 
                  ?? ParseFromName(room.Name) 
                  ?? ParseFromComments(room.Comments);
```

### 2. Open Space Association
**Challenge:** Linking private open spaces to specific units  
**Solution:** Use spatial proximity or shared parameters  
**Code approach:**
```csharp
// Check if balcony is adjacent to unit 检查阳台是否与单元相邻
// Compare bounding boxes or use room separation lines
```

### 3. Units - Already Imperial!
**No Challenge:** Revit API in US returns values in feet  
**Just remember:**
```csharp
double areaInSF = room.Area; // Already square feet! 已经是平方英尺
// No conversion needed! 不需要转换
```

### 4. WPF Threading with Revit
**Challenge:** WPF runs on UI thread, Revit API calls must be on Revit thread  
**Solution:** Collect all data in Command.Execute(), then pass to WPF window  
```csharp
// In Command.Execute() - Revit thread
var data = CollectAllData(doc);

// Open window and pass data
var window = new MainWindow(data);
window.Show();
```

### 5. Modern Charts Performance
**Challenge:** Many data points can slow down charts  
**Solution:** Use LiveCharts2 (GPU-accelerated), limit to reasonable data sets  
**Tip:** For 100+ units, use simplified views or pagination

---

## Reference Resources

- **Revit API Docs:** https://www.revitapidocs.com/
- **LiveCharts2 Docs:** https://livecharts.dev/ (Use v2, NOT v0.9!)
- **WPF Tutorial:** Microsoft Learn - WPF Documentation
- **EPPlus (Excel):** https://github.com/EPPlusSoftware/EPPlus
- **ClosedXML (Alternative):** https://github.com/ClosedXML/ClosedXML

---

**Ready to start coding with Claude Code? Use this PRD and let's build this plugin FAST! 🚀**

---

## Quick Start Command for Claude Code

Once you have this PRD, run in VS Code terminal:
```bash
claude
```

Then paste this:
```
I need you to build a complete Revit plugin based on PRD_RevitUnitSchedulePlugin.md.

CRITICAL REQUIREMENTS:
1. English UI labels and text
2. Bilingual code comments (English + Chinese)
3. Imperial units (Square Feet) - no conversion
4. Modern charts using LiveCharts2
5. Include private and public open space analysis

Generate in this order:
1. Command.cs - Entry point
2. Models (UnitInfo.cs, OpenSpaceInfo.cs, ProjectStatistics.cs)
3. Services (UnitScanner.cs, OpenSpaceAnalyzer.cs)
4. MainViewModel.cs
5. MainWindow.xaml (with modern color scheme)
6. MainWindow.xaml.cs
7. UnitSchedulePlugin.addin

Make it look professional and modern! 让它看起来专业现代！
```