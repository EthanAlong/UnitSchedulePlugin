using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using Autodesk.Revit.DB;
using UnitSchedulePlugin.Helpers;
using UnitSchedulePlugin.Models;
using UnitSchedulePlugin.Services;

namespace UnitSchedulePlugin.ViewModels
{
    /// <summary>
    /// Main ViewModel for the Unit Schedule Plugin window.
    /// Implements MVVM pattern with INotifyPropertyChanged.
    /// Uses pure WPF for charts (no external dependencies).
    ///
    /// 单元时间表插件窗口的主 ViewModel。
    /// 使用 INotifyPropertyChanged 实现 MVVM 模式。
    /// 使用纯 WPF 实现图表（无外部依赖）。
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields | 字段

        private readonly Document _document;
        private readonly ExportService _exportService;

        private string _projectName;
        private string _searchText;
        private string _selectedUnitType;
        private string _selectedLevel;
        private UnitInfo _selectedUnit;
        private OpenSpaceInfo _selectedOpenSpace;
        private int _selectedTabIndex;

        #endregion

        #region Properties | 属性

        /// <summary>
        /// Project name for display.
        /// 显示的项目名称。
        /// </summary>
        public string ProjectName
        {
            get => _projectName;
            set { _projectName = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Search text for filtering units.
        /// 用于过滤单元的搜索文本。
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterUnits();
            }
        }

        /// <summary>
        /// Selected unit type filter.
        /// 选定的单元类型过滤器。
        /// </summary>
        public string SelectedUnitType
        {
            get => _selectedUnitType;
            set
            {
                _selectedUnitType = value;
                OnPropertyChanged();
                FilterUnits();
            }
        }

        /// <summary>
        /// Selected level filter.
        /// 选定的楼层过滤器。
        /// </summary>
        public string SelectedLevel
        {
            get => _selectedLevel;
            set
            {
                _selectedLevel = value;
                OnPropertyChanged();
                FilterUnits();
            }
        }

        /// <summary>
        /// Currently selected unit in the list.
        /// 列表中当前选定的单元。
        /// </summary>
        public UnitInfo SelectedUnit
        {
            get => _selectedUnit;
            set { _selectedUnit = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Currently selected open space.
        /// 当前选定的开放空间。
        /// </summary>
        public OpenSpaceInfo SelectedOpenSpace
        {
            get => _selectedOpenSpace;
            set { _selectedOpenSpace = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Selected tab index.
        /// 选定的标签页索引。
        /// </summary>
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set { _selectedTabIndex = value; OnPropertyChanged(); }
        }

        #endregion

        #region Collections | 集合

        /// <summary>
        /// All units in the project.
        /// 项目中的所有单元。
        /// </summary>
        public ObservableCollection<UnitInfo> Units { get; set; } = new ObservableCollection<UnitInfo>();

        /// <summary>
        /// Filtered units for display.
        /// 用于显示的过滤后单元。
        /// </summary>
        public ObservableCollection<UnitInfo> FilteredUnits { get; set; } = new ObservableCollection<UnitInfo>();

        /// <summary>
        /// Private open spaces.
        /// 私有开放空间。
        /// </summary>
        public ObservableCollection<OpenSpaceInfo> PrivateOpenSpaces { get; set; } = new ObservableCollection<OpenSpaceInfo>();

        /// <summary>
        /// Public open spaces.
        /// 公共开放空间。
        /// </summary>
        public ObservableCollection<OpenSpaceInfo> PublicOpenSpaces { get; set; } = new ObservableCollection<OpenSpaceInfo>();

        /// <summary>
        /// Available unit types for filtering.
        /// 可用于过滤的单元类型。
        /// </summary>
        public ObservableCollection<string> UnitTypes { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Available levels for filtering.
        /// 可用于过滤的楼层。
        /// </summary>
        public ObservableCollection<string> Levels { get; set; } = new ObservableCollection<string>();

        #endregion

        #region Statistics | 统计数据

        /// <summary>
        /// Project statistics.
        /// 项目统计数据。
        /// </summary>
        public ProjectStatistics Statistics { get; set; } = new ProjectStatistics();

        #endregion

        #region Chart Data (Pure WPF) | 图表数据（纯 WPF）

        /// <summary>
        /// Data items for unit mix chart.
        /// 单元组合图表的数据项。
        /// </summary>
        public ObservableCollection<ChartItem> UnitMixChartData { get; set; } = new ObservableCollection<ChartItem>();

        /// <summary>
        /// Data items for bedroom mix chart.
        /// 卧室组合图表的数据项。
        /// </summary>
        public ObservableCollection<ChartItem> BedroomMixChartData { get; set; } = new ObservableCollection<ChartItem>();

        /// <summary>
        /// Data items for area by type chart.
        /// 按类型划分面积图表的数据项。
        /// </summary>
        public ObservableCollection<ChartItem> AreaByTypeChartData { get; set; } = new ObservableCollection<ChartItem>();

        /// <summary>
        /// Data items for open space chart.
        /// 开放空间图表的数据项。
        /// </summary>
        public ObservableCollection<ChartItem> OpenSpaceChartData { get; set; } = new ObservableCollection<ChartItem>();

        #endregion

        #region Commands | 命令

        public ICommand ExportUnitsCommand { get; }
        public ICommand ExportAllCommand { get; }
        public ICommand ExportReportCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand CloseCommand { get; }

        #endregion

        #region Constructor | 构造函数

        /// <summary>
        /// Initializes the MainViewModel.
        /// 初始化 MainViewModel。
        /// </summary>
        /// <param name="document">Revit document | Revit 文档</param>
        public MainViewModel(Document document)
        {
            _document = document;
            _exportService = new ExportService();

            // Initialize commands | 初始化命令
            ExportUnitsCommand = new RelayCommand(ExportUnits);
            ExportAllCommand = new RelayCommand(ExportAll);
            ExportReportCommand = new RelayCommand(ExportReport);
            RefreshCommand = new RelayCommand(RefreshData);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
            CloseCommand = new RelayCommand(CloseWindow);

            // Add default filter options | 添加默认过滤器选项
            UnitTypes.Add("All Types");
            Levels.Add("All Levels");
            _selectedUnitType = "All Types";
            _selectedLevel = "All Levels";
        }

        #endregion

        #region Public Methods | 公共方法

        /// <summary>
        /// Calculates all statistics and updates charts.
        /// 计算所有统计数据并更新图表。
        /// </summary>
        public void CalculateStatistics()
        {
            // Calculate statistics | 计算统计数据
            Statistics.Calculate(Units, PrivateOpenSpaces, PublicOpenSpaces);
            OnPropertyChanged(nameof(Statistics));

            // Update filter options | 更新过滤器选项
            UpdateFilterOptions();

            // Initialize filtered units | 初始化过滤后的单元
            FilterUnits();

            // Create chart data | 创建图表数据
            CreateChartData();
        }

        #endregion

        #region Private Methods | 私有方法

        /// <summary>
        /// Updates the filter dropdown options.
        /// 更新过滤器下拉选项。
        /// </summary>
        private void UpdateFilterOptions()
        {
            UnitTypes.Clear();
            UnitTypes.Add("All Types");
            foreach (var type in Statistics.UnitCountByType.Keys.OrderBy(k => k))
            {
                UnitTypes.Add(type);
            }

            Levels.Clear();
            Levels.Add("All Levels");
            foreach (var level in Units.Select(u => u.Level).Distinct().OrderBy(l => l))
            {
                if (!string.IsNullOrEmpty(level))
                    Levels.Add(level);
            }
        }

        /// <summary>
        /// Filters units based on search and filter criteria.
        /// 根据搜索和过滤条件过滤单元。
        /// </summary>
        private void FilterUnits()
        {
            FilteredUnits.Clear();

            var filtered = Units.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string search = SearchText.ToLower();
                filtered = filtered.Where(u =>
                    (u.UnitNumber?.ToLower().Contains(search) ?? false) ||
                    (u.Name?.ToLower().Contains(search) ?? false) ||
                    (u.UnitType?.ToLower().Contains(search) ?? false));
            }

            if (!string.IsNullOrEmpty(SelectedUnitType) && SelectedUnitType != "All Types")
            {
                filtered = filtered.Where(u => u.UnitType == SelectedUnitType);
            }

            if (!string.IsNullOrEmpty(SelectedLevel) && SelectedLevel != "All Levels")
            {
                filtered = filtered.Where(u => u.Level == SelectedLevel);
            }

            foreach (var unit in filtered.OrderBy(u => u.Level).ThenBy(u => u.UnitNumber))
            {
                FilteredUnits.Add(unit);
            }
        }

        /// <summary>
        /// Creates chart data for WPF visualization.
        /// 为 WPF 可视化创建图表数据。
        /// </summary>
        private void CreateChartData()
        {
            CreateUnitMixChartData();
            CreateBedroomMixChartData();
            CreateAreaByTypeChartData();
            CreateOpenSpaceChartData();
        }

        private void CreateUnitMixChartData()
        {
            UnitMixChartData.Clear();
            int colorIndex = 0;
            double total = Statistics.UnitCountByType.Values.Sum();

            foreach (var kvp in Statistics.UnitCountByType.OrderByDescending(x => x.Value))
            {
                var color = ColorPalette.GetChartColor(colorIndex);
                double percentage = total > 0 ? (kvp.Value / total) * 100 : 0;
                UnitMixChartData.Add(new ChartItem
                {
                    Label = kvp.Key,
                    Value = kvp.Value,
                    Percentage = percentage,
                    Color = new SolidColorBrush(color),
                    DisplayText = $"{kvp.Key}: {kvp.Value} ({percentage:F1}%)"
                });
                colorIndex++;
            }
        }

        private void CreateBedroomMixChartData()
        {
            BedroomMixChartData.Clear();
            int colorIndex = 0;
            double total = Statistics.UnitCountByBedrooms.Values.Sum();

            foreach (var kvp in Statistics.UnitCountByBedrooms.OrderBy(x => x.Key))
            {
                string label = kvp.Key == 0 ? "Studio" : $"{kvp.Key} BR";
                var color = ColorPalette.GetChartColor(colorIndex);
                double percentage = total > 0 ? (kvp.Value / total) * 100 : 0;
                BedroomMixChartData.Add(new ChartItem
                {
                    Label = label,
                    Value = kvp.Value,
                    Percentage = percentage,
                    Color = new SolidColorBrush(color),
                    DisplayText = $"{label}: {kvp.Value} ({percentage:F1}%)"
                });
                colorIndex++;
            }
        }

        private void CreateAreaByTypeChartData()
        {
            AreaByTypeChartData.Clear();
            int colorIndex = 0;
            double maxArea = Statistics.GrossAreaByType.Values.Any() ? Statistics.GrossAreaByType.Values.Max() : 1;

            foreach (var kvp in Statistics.GrossAreaByType.OrderByDescending(x => x.Value))
            {
                var color = ColorPalette.GetChartColor(colorIndex);
                double percentage = maxArea > 0 ? (kvp.Value / maxArea) * 100 : 0;
                AreaByTypeChartData.Add(new ChartItem
                {
                    Label = kvp.Key,
                    Value = kvp.Value,
                    Percentage = percentage,
                    Color = new SolidColorBrush(color),
                    DisplayText = $"{kvp.Key}: {kvp.Value:N0} SF"
                });
                colorIndex++;
            }
        }

        private void CreateOpenSpaceChartData()
        {
            OpenSpaceChartData.Clear();
            double total = Statistics.TotalPrivateOpenSpace + Statistics.TotalPublicOpenSpace;
            if (total == 0) total = 1;

            var deepBlue = ColorPalette.DeepBlue;
            var teal = ColorPalette.Teal;
            var amber = ColorPalette.Amber;

            if (Statistics.TotalBalconyArea > 0)
            {
                double pct = (Statistics.TotalBalconyArea / total) * 100;
                OpenSpaceChartData.Add(new ChartItem
                {
                    Label = "Balconies",
                    Value = Statistics.TotalBalconyArea,
                    Percentage = pct,
                    Color = new SolidColorBrush(deepBlue),
                    DisplayText = $"Balconies: {Statistics.TotalBalconyArea:N0} SF ({pct:F1}%)"
                });
            }

            if (Statistics.TotalPatioArea > 0)
            {
                double pct = (Statistics.TotalPatioArea / total) * 100;
                OpenSpaceChartData.Add(new ChartItem
                {
                    Label = "Patios",
                    Value = Statistics.TotalPatioArea,
                    Percentage = pct,
                    Color = new SolidColorBrush(teal),
                    DisplayText = $"Patios: {Statistics.TotalPatioArea:N0} SF ({pct:F1}%)"
                });
            }

            if (Statistics.TotalTerraceArea > 0)
            {
                double pct = (Statistics.TotalTerraceArea / total) * 100;
                OpenSpaceChartData.Add(new ChartItem
                {
                    Label = "Terraces",
                    Value = Statistics.TotalTerraceArea,
                    Percentage = pct,
                    Color = new SolidColorBrush(amber),
                    DisplayText = $"Terraces: {Statistics.TotalTerraceArea:N0} SF ({pct:F1}%)"
                });
            }

            if (Statistics.TotalPublicOpenSpace > 0)
            {
                double pct = (Statistics.TotalPublicOpenSpace / total) * 100;
                var color = ColorPalette.GetChartColor(3);
                OpenSpaceChartData.Add(new ChartItem
                {
                    Label = "Public Amenities",
                    Value = Statistics.TotalPublicOpenSpace,
                    Percentage = pct,
                    Color = new SolidColorBrush(color),
                    DisplayText = $"Public: {Statistics.TotalPublicOpenSpace:N0} SF ({pct:F1}%)"
                });
            }
        }

        #endregion

        #region Command Handlers | 命令处理程序

        private void ExportUnits(object parameter)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                DefaultExt = ".csv",
                FileName = $"{ProjectName}_Units"
            };

            if (dialog.ShowDialog() == true)
            {
                _exportService.ExportUnitsToCSV(FilteredUnits, dialog.FileName);
                System.Windows.MessageBox.Show("Units exported successfully!\n单元导出成功！",
                    "Export Complete", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        private void ExportAll(object parameter)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                DefaultExt = ".csv",
                FileName = $"{ProjectName}_Complete"
            };

            if (dialog.ShowDialog() == true)
            {
                _exportService.ExportAllToCSV(Statistics, Units, PrivateOpenSpaces, PublicOpenSpaces, ProjectName, dialog.FileName);
                System.Windows.MessageBox.Show("All data exported successfully!\n所有数据导出成功！",
                    "Export Complete", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        private void ExportReport(object parameter)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                DefaultExt = ".txt",
                FileName = $"{ProjectName}_Report"
            };

            if (dialog.ShowDialog() == true)
            {
                _exportService.ExportSummaryReport(Statistics, Units, PrivateOpenSpaces, PublicOpenSpaces, ProjectName, dialog.FileName);
                System.Windows.MessageBox.Show("Report exported successfully!\n报告导出成功！",
                    "Export Complete", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        private void RefreshData(object parameter)
        {
            var unitScanner = new UnitScanner(_document);
            var openSpaceAnalyzer = new OpenSpaceAnalyzer(_document);

            Units.Clear();
            foreach (var unit in unitScanner.ScanAllUnits())
            {
                Units.Add(unit);
            }

            PrivateOpenSpaces.Clear();
            foreach (var space in openSpaceAnalyzer.GetPrivateOpenSpaces())
            {
                PrivateOpenSpaces.Add(space);
            }

            PublicOpenSpaces.Clear();
            foreach (var space in openSpaceAnalyzer.GetPublicOpenSpaces())
            {
                PublicOpenSpaces.Add(space);
            }

            openSpaceAnalyzer.AssociateOpenSpacesWithUnits(Units.ToList(), PrivateOpenSpaces.ToList());
            CalculateStatistics();
        }

        private void ClearFilters(object parameter)
        {
            SearchText = string.Empty;
            SelectedUnitType = "All Types";
            SelectedLevel = "All Levels";
        }

        private void CloseWindow(object parameter)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Events | 事件

        public event EventHandler CloseRequested;

        #endregion

        #region INotifyPropertyChanged | INotifyPropertyChanged 实现

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    /// Simple chart data item for WPF visualization.
    /// 用于 WPF 可视化的简单图表数据项。
    /// </summary>
    public class ChartItem : INotifyPropertyChanged
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public double Percentage { get; set; }
        public SolidColorBrush Color { get; set; }
        public string DisplayText { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
