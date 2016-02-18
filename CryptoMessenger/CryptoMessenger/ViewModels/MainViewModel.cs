using System.Collections.ObjectModel;

namespace CryptoMessenger.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        public ObservableCollection<ClientViewModel> BooksList { get; set; } 

        public MainViewModel()
        {
            
        }
    }
}
