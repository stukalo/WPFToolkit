using System;
using System.Windows.Input;

namespace Xceed.Wpf.AvalonDock.Commands
{
	internal class RelayCommand : ICommand
	{
		private readonly Action<object> _execute;

		private readonly Predicate<object> _canExecute;

		public RelayCommand(Action<object> execute) : this(execute, null)
		{
		}

		public RelayCommand(Action<object> execute, Predicate<object> canExecute)
		{
			if (execute == null)
			{
				throw new ArgumentNullException("execute");
			}
			this._execute = execute;
			this._canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			if (this._canExecute == null)
			{
				return true;
			}
			return this._canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			this._execute(parameter);
		}

		public event EventHandler CanExecuteChanged
		{
			add
			{
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				CommandManager.RequerySuggested -= value;
			}
		}
	}
}