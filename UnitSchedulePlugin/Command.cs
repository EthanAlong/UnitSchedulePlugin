using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using UnitSchedulePlugin.Services;
using UnitSchedulePlugin.ViewModels;
using UnitSchedulePlugin.Views;

namespace UnitSchedulePlugin
{
    /// <summary>
    /// Main entry point for the Revit Unit Schedule Plugin.
    /// Implements IExternalCommand interface for Revit add-in integration.
    ///
    /// Revit 单元时间表插件的主入口点。
    /// 实现 IExternalCommand 接口以集成 Revit 插件。
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        /// <summary>
        /// Executes the external command when triggered from Revit.
        /// 当从 Revit 触发时执行外部命令。
        /// </summary>
        /// <param name="commandData">Contains reference to Application and View | 包含对应用程序和视图的引用</param>
        /// <param name="message">Error message to display if command fails | 命令失败时显示的错误消息</param>
        /// <param name="elements">Elements to highlight if command fails | 命令失败时要高亮显示的元素</param>
        /// <returns>Result indicating success, failure, or cancellation | 表示成功、失败或取消的结果</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Get the current Revit application and document
                // 获取当前的 Revit 应用程序和文档
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;

                // Validate that a document is open
                // 验证是否打开了文档
                if (doc == null)
                {
                    TaskDialog.Show("Unit Schedule Plugin",
                        "Please open a Revit project before running this command.\n" +
                        "请在运行此命令之前打开 Revit 项目。");
                    return Result.Cancelled;
                }

                // Initialize services for scanning and analysis
                // 初始化扫描和分析服务
                var unitScanner = new UnitScanner(doc);
                var openSpaceAnalyzer = new OpenSpaceAnalyzer(doc);

                // Scan all units and open spaces in the project
                // 扫描项目中的所有单元和开放空间
                var units = unitScanner.ScanAllUnits();
                var privateOpenSpaces = openSpaceAnalyzer.GetPrivateOpenSpaces();
                var publicOpenSpaces = openSpaceAnalyzer.GetPublicOpenSpaces();

                // Create and configure the main ViewModel
                // 创建并配置主 ViewModel
                var viewModel = new MainViewModel(doc)
                {
                    ProjectName = doc.Title,
                    Units = new System.Collections.ObjectModel.ObservableCollection<Models.UnitInfo>(units),
                    PrivateOpenSpaces = new System.Collections.ObjectModel.ObservableCollection<Models.OpenSpaceInfo>(privateOpenSpaces),
                    PublicOpenSpaces = new System.Collections.ObjectModel.ObservableCollection<Models.OpenSpaceInfo>(publicOpenSpaces)
                };

                // Calculate project statistics
                // 计算项目统计数据
                viewModel.CalculateStatistics();

                // Create and show the main window
                // 创建并显示主窗口
                var mainWindow = new MainWindow(viewModel);
                mainWindow.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                // Log the exception and display error message
                // 记录异常并显示错误消息
                message = $"An error occurred: {ex.Message}\n发生错误：{ex.Message}";

                TaskDialog errorDialog = new TaskDialog("Unit Schedule Plugin - Error");
                errorDialog.MainInstruction = "An unexpected error occurred.";
                errorDialog.MainContent = $"{ex.Message}\n\nStack Trace:\n{ex.StackTrace}";
                errorDialog.Show();

                return Result.Failed;
            }
        }
    }

    /// <summary>
    /// Application class for plugin initialization on Revit startup.
    /// 插件在 Revit 启动时初始化的应用程序类。
    /// </summary>
    public class Application : IExternalApplication
    {
        /// <summary>
        /// Called when Revit starts up.
        /// 在 Revit 启动时调用。
        /// </summary>
        public Result OnStartup(UIControlledApplication application)
        {
            // Create ribbon panel for the plugin
            // 为插件创建功能区面板
            try
            {
                RibbonPanel ribbonPanel = application.CreateRibbonPanel("Unit Schedule");

                // Get the path to this assembly
                // 获取此程序集的路径
                string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                // Create push button for the command
                // 为命令创建按钮
                PushButtonData buttonData = new PushButtonData(
                    "UnitSchedule",
                    "Unit\nSchedule",
                    assemblyPath,
                    "UnitSchedulePlugin.Command");

                buttonData.ToolTip = "Generate unit schedule with area analysis and charts.\n生成带有面积分析和图表的单元时间表。";
                buttonData.LongDescription = "Scans all rooms and areas in the project to generate a comprehensive unit schedule " +
                    "with statistics, charts, and export capabilities.\n" +
                    "扫描项目中的所有房间和区域，生成带有统计数据、图表和导出功能的综合单元时间表。";

                ribbonPanel.AddItem(buttonData);

                return Result.Succeeded;
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }

        /// <summary>
        /// Called when Revit shuts down.
        /// 在 Revit 关闭时调用。
        /// </summary>
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
