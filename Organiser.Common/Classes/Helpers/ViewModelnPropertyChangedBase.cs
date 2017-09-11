using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Organiser.Common.Classes
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public virtual void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public virtual void NotifyOfPropertyChange([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public virtual void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public virtual void NotifyOfPropertyChange([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public abstract class PropertyChangedViewModelBase : INotifyPropertyChanged
    {
        public ICommand OnCommandFromView { get; set; }

        public PropertyChangedViewModelBase()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
        }

        private void OnCommandFromView_Raised(object obj)
        {
            string param = obj as string;
            if (param == null) param = "";

            OnReceivedCommandFromView(param);
        }

        public abstract void OnReceivedCommandFromView(string param);

        public virtual void NotifyOfPropertyChange([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Show(string message)
        {
            MessageBox.Show(message);
        }

        public bool Ask(string message)
        {
            return MessageBox.Show(message, "Browseo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes;
        }

        public void Invoke(Action method)
        {
            Application.Current.Dispatcher.Invoke(method, System.Windows.Threading.DispatcherPriority.Background);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
