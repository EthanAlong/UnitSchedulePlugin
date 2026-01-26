using System;
using System.Windows.Input;

namespace UnitSchedulePlugin.ViewModels
{
    /// <summary>
    /// Implementation of ICommand for MVVM pattern.
    /// Allows binding commands to UI elements in WPF.
    ///
    /// MVVM 模式的 ICommand 实现。
    /// 允许在 WPF 中将命令绑定到 UI 元素。
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields | 字段

        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        #endregion

        #region Constructors | 构造函数

        /// <summary>
        /// Creates a new command that can always execute.
        /// 创建一个始终可以执行的新命令。
        /// </summary>
        /// <param name="execute">The execution logic | 执行逻辑</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command with execution and can-execute logic.
        /// 创建具有执行和可执行逻辑的新命令。
        /// </summary>
        /// <param name="execute">The execution logic | 执行逻辑</param>
        /// <param name="canExecute">The can-execute logic | 可执行逻辑</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Implementation | ICommand 实现

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// 确定命令是否可以在其当前状态下执行。
        /// </summary>
        /// <param name="parameter">Data used by the command | 命令使用的数据</param>
        /// <returns>True if command can execute | 如果命令可以执行则为 true</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Executes the command.
        /// 执行命令。
        /// </summary>
        /// <param name="parameter">Data used by the command | 命令使用的数据</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Event raised when CanExecute changes.
        /// 当 CanExecute 更改时引发的事件。
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion

        #region Public Methods | 公共方法

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// 引发 CanExecuteChanged 事件。
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion
    }

    /// <summary>
    /// Generic implementation of RelayCommand with typed parameter.
    /// 带有类型化参数的 RelayCommand 泛型实现。
    /// </summary>
    /// <typeparam name="T">Type of command parameter | 命令参数类型</typeparam>
    public class RelayCommand<T> : ICommand
    {
        #region Fields | 字段

        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        #endregion

        #region Constructors | 构造函数

        /// <summary>
        /// Creates a new command that can always execute.
        /// 创建一个始终可以执行的新命令。
        /// </summary>
        /// <param name="execute">The execution logic | 执行逻辑</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command with execution and can-execute logic.
        /// 创建具有执行和可执行逻辑的新命令。
        /// </summary>
        /// <param name="execute">The execution logic | 执行逻辑</param>
        /// <param name="canExecute">The can-execute logic | 可执行逻辑</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Implementation | ICommand 实现

        /// <summary>
        /// Determines whether the command can execute.
        /// 确定命令是否可以执行。
        /// </summary>
        public bool CanExecute(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
                return _canExecute == null;

            return _canExecute == null || _canExecute((T)parameter);
        }

        /// <summary>
        /// Executes the command.
        /// 执行命令。
        /// </summary>
        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        /// <summary>
        /// Event raised when CanExecute changes.
        /// 当 CanExecute 更改时引发的事件。
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion
    }
}
