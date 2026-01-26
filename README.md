# Unit Schedule Plugin for Revit

<p align="center">
  <img src="https://img.shields.io/badge/Revit-2025-blue?style=for-the-badge&logo=autodesk" alt="Revit 2025"/>
  <img src="https://img.shields.io/badge/.NET-8.0-purple?style=for-the-badge&logo=dotnet" alt=".NET 8.0"/>
  <img src="https://img.shields.io/badge/WPF-Modern_UI-green?style=for-the-badge" alt="WPF"/>
  <img src="https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge" alt="MIT License"/>
</p>

A professional Revit plugin for generating comprehensive unit schedules with area analysis, open space tracking, and interactive visualizations. Perfect for residential and mixed-use building projects.

## Features

### Core Functionality
- **Unit Scanning** - Automatically detects and catalogs all residential units from Revit rooms/areas
- **Area Calculations** - Gross area, net area, and sellable area in Square Feet (Imperial)
- **Unit Type Detection** - Smart pattern matching identifies Studio, 1BR, 2BR, 3BR, Penthouse, etc.

### Open Space Tracking
- **Private Open Spaces** - Balconies, patios, terraces associated with individual units
- **Public Open Spaces** - Amenities, courtyards, rooftops, pool areas, playgrounds
- **Coverage Analysis** - Tracks covered vs. uncovered areas for sellable area calculations

### Dashboard & Visualization
- **Statistics Cards** - Total units, gross/net area, efficiency ratios at a glance
- **Interactive Charts** - Unit mix distribution, bedroom mix, area by type
- **Professional UI** - Modern WPF design with Deep Blue (#1E3A8A), Teal (#14B8A6), Amber (#F59E0B) color scheme

### Data Management
- **Filtering & Search** - Filter by unit type, level, or search by unit number/name
- **Multiple Exports** - CSV for Excel, comprehensive reports in TXT format
- **Real-time Refresh** - Update data without restarting the plugin

## Screenshots

| Dashboard | Unit Schedule |
|-----------|---------------|
| Statistics cards and charts overview | Filterable unit list with all details |

| Private Open Spaces | Export Options |
|---------------------|----------------|
| Balconies, patios, terraces by unit | CSV and report export functionality |

## Installation

### Requirements
- Autodesk Revit 2025 or later
- Windows 10/11 (64-bit)

### Steps

1. **Download** the latest release from [Releases](https://github.com/EthanAlong/UnitSchedulePlugin/releases)

2. **Copy files** to your Revit Addins folder:
   ```
   %APPDATA%\Autodesk\Revit\Addins\2025\
   ```
   - `UnitSchedulePlugin.dll`
   - `UnitSchedulePlugin.addin`

3. **Restart Revit**

4. Find the plugin under **Add-Ins** tab → **Unit Schedule** panel

### Building from Source

```bash
# Clone the repository
git clone https://github.com/EthanAlong/UnitSchedulePlugin.git

# Open in Visual Studio 2022
# Or build with .NET CLI
cd UnitSchedulePlugin
dotnet build -c Release
```

## Usage

1. Open a Revit project with rooms/areas defined
2. Navigate to **Add-Ins** → **Unit Schedule**
3. Click **Unit Schedule** button
4. Explore the dashboard, filter units, and export data

### Room Naming Conventions

For best results, use consistent naming patterns:

| Pattern | Detected As |
|---------|-------------|
| `Unit 101 - Living`, `101-LR` | Unit 101 |
| `Studio`, `Bachelor` | Studio |
| `1BR`, `1 Bedroom`, `One Bed` | 1 Bedroom |
| `Balcony`, `Patio`, `Terrace` | Private Open Space |
| `Courtyard`, `Amenity`, `Pool` | Public Open Space |

## Project Structure

```
UnitSchedulePlugin/
├── Command.cs                    # Revit IExternalCommand entry point
├── Models/
│   ├── UnitInfo.cs              # Unit data model
│   ├── OpenSpaceInfo.cs         # Open space model
│   └── ProjectStatistics.cs     # Aggregated statistics
├── Helpers/
│   ├── SpaceTypeIdentifier.cs   # Pattern matching for space types
│   └── ColorPalette.cs          # Professional color scheme
├── Services/
│   ├── UnitScanner.cs           # Scans Revit for units
│   ├── OpenSpaceAnalyzer.cs     # Analyzes open spaces
│   └── ExportService.cs         # CSV and report exports
├── ViewModels/
│   ├── MainViewModel.cs         # MVVM ViewModel
│   └── RelayCommand.cs          # ICommand implementation
└── Views/
    ├── MainWindow.xaml          # WPF UI (Pure WPF, no dependencies)
    └── MainWindow.xaml.cs       # Code-behind
```

## Technical Details

| Aspect | Details |
|--------|---------|
| **Target Framework** | .NET 8.0 Windows |
| **Revit API** | 2025 |
| **UI Framework** | WPF (Pure, no external dependencies) |
| **Architecture** | MVVM Pattern |
| **Units** | Imperial (Square Feet) |
| **Language** | English UI, Bilingual Comments (EN + 中文) |

## API Reference

### UnitInfo Properties
```csharp
public class UnitInfo
{
    public string UnitNumber { get; set; }      // Unit identifier
    public string UnitType { get; set; }        // Studio, 1BR, 2BR, etc.
    public double GrossArea { get; set; }       // Total area (SF)
    public double NetArea { get; set; }         // Usable area (SF)
    public double SellableArea { get; set; }    // Marketable area (SF)
    public double BalconyArea { get; set; }     // Private balcony (SF)
    public double PatioArea { get; set; }       // Private patio (SF)
    public double TerraceArea { get; set; }     // Private terrace (SF)
    public int BedroomCount { get; set; }       // Number of bedrooms
    public double Efficiency { get; set; }      // Net/Gross ratio (%)
}
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built with [Autodesk Revit API](https://www.revitapidocs.com/)
- UI inspired by modern design systems
- Developed with assistance from Claude AI

---

<p align="center">
  Made with ❤️ for the AEC Industry
</p>
