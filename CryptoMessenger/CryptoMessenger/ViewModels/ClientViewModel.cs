using CryptoMessenger.Models;

namespace CryptoMessenger.ViewModels
{
	class ClientViewModel : ViewModelBase
	{
		// model
		private Client client;

		public ClientViewModel(Client client)
		{
			this.client = client;
		}

	}
}
