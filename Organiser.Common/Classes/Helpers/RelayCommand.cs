using System;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace Organiser.Common.Classes
{                         
    [DataContract]
    public class RelayCommand : ICommand
    {
        [field: NonSerialized]
        private Action<object> _action;
        private ICommand onCommandFromView;

        public RelayCommand(Action<object> action)
        {
            _action = action;
        }

        public RelayCommand(ICommand onCommandFromView)
        {
            this.onCommandFromView = onCommandFromView;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        [field: NonSerialized]
        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                _action(parameter);
            }
            else
            {
                _action("Param cannot be null");
            }
        }

        #endregion
    }
}
