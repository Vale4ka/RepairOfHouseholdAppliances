using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RepairOfHouseholdAppliances.Core
{
    public class ParCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<object> _execute;
        public ParCommand(Action<object> execute)
        {
            this._execute = execute;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
