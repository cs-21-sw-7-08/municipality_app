using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MunicipalityApp.Commands
{
    public class SimpleCommand : ICommand
    {
        /*****************************************************************/
        // VARIABLES
        /*****************************************************************/
        #region Variables

        private Action action;
        public event EventHandler CanExecuteChanged;

        #endregion

        /*****************************************************************/
        // CONSTRUCTOR
        /*****************************************************************/
        #region Constructor

        public SimpleCommand(Action action)
        {
            this.action = action;
        }

        #endregion

        /*****************************************************************/
        // METHODS
        /*****************************************************************/
        #region Methods

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action?.Invoke();
        }

        #endregion
    }
    public class SimpleCommand<T> : ICommand
    {
        /*****************************************************************/
        // VARIABLES
        /*****************************************************************/
        #region Variables

        private Action<T> action;
        public event EventHandler CanExecuteChanged;

        #endregion

        /*****************************************************************/
        // CONSTRUCTOR
        /*****************************************************************/
        #region Constructor

        public SimpleCommand(Action<T> action)
        {
            this.action = action;
        }

        #endregion

        /*****************************************************************/
        // METHODS
        /*****************************************************************/
        #region Methods

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action?.Invoke((T)parameter);
        }

        #endregion
    }
}
