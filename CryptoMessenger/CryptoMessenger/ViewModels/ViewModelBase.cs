using System.ComponentModel;

namespace CryptoMessenger.ViewModels
{
    /// <summary>
    /// Provides common functionality for ViewModel classes
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

    }
}
