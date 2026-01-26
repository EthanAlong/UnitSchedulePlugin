using System;
using System.Windows;
using UnitSchedulePlugin.ViewModels;

namespace UnitSchedulePlugin.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Code-behind for the main plugin window.
    ///
    /// MainWindow.xaml 的交互逻辑
    /// 主插件窗口的代码后置。
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields | 字段

        private readonly MainViewModel _viewModel;

        #endregion

        #region Constructor | 构造函数

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// 初始化 MainWindow 类的新实例。
        /// </summary>
        /// <param name="viewModel">The ViewModel to bind to | 要绑定的 ViewModel</param>
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            // Set the DataContext for data binding
            // 设置数据绑定的 DataContext
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = _viewModel;

            // Subscribe to close request event
            // 订阅关闭请求事件
            _viewModel.CloseRequested += OnCloseRequested;

            // Set window owner to Revit main window for proper modal behavior
            // 将窗口所有者设置为 Revit 主窗口以实现正确的模态行为
            SetRevitAsOwner();
        }

        #endregion

        #region Private Methods | 私有方法

        /// <summary>
        /// Sets Revit's main window as the owner of this window.
        /// 将 Revit 的主窗口设置为此窗口的所有者。
        /// </summary>
        private void SetRevitAsOwner()
        {
            try
            {
                // Get Revit's main window handle
                // 获取 Revit 主窗口句柄
                var revitProcess = System.Diagnostics.Process.GetCurrentProcess();
                var revitHandle = revitProcess.MainWindowHandle;

                if (revitHandle != IntPtr.Zero)
                {
                    // Set this window's owner using interop
                    // 使用互操作设置此窗口的所有者
                    var helper = new System.Windows.Interop.WindowInteropHelper(this)
                    {
                        Owner = revitHandle
                    };
                }
            }
            catch (Exception ex)
            {
                // Log but don't throw - window will still work without owner
                // 记录但不抛出 - 窗口在没有所有者的情况下仍然可以工作
                System.Diagnostics.Debug.WriteLine($"Could not set Revit as owner: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the close request from the ViewModel.
        /// 处理来自 ViewModel 的关闭请求。
        /// </summary>
        private void OnCloseRequested(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Event Handlers | 事件处理程序

        /// <summary>
        /// Handles the window closing event.
        /// 处理窗口关闭事件。
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            // Unsubscribe from events to prevent memory leaks
            // 取消订阅事件以防止内存泄漏
            if (_viewModel != null)
            {
                _viewModel.CloseRequested -= OnCloseRequested;
            }

            base.OnClosed(e);
        }

        #endregion
    }
}
