using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfWebBrowser.ViewModel.Core
{
    class DelegateCommand : ICommand
    {
        private readonly Action<object> _Execute;
        private readonly Predicate<object> _CanExecute;
        
        public DelegateCommand(Action<object> execute) : this(execute, null) { }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _Execute = execute ?? throw new Exception("execute 不能为空");
            _CanExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 判断命令是否可以执行
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter) => _CanExecute?.Invoke(parameter) ?? true;

        /// <summary>
        /// 执行命令操作
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) => _Execute(parameter);

    }
}
