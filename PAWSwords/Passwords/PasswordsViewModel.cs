using Caliburn.Micro;
using PAWSwords.Core.Interfaces;
using PAWSwords.Core.Passwords;
using PAWSwords.Passwords.Messages;
using System.Collections.Generic;
using System.Linq;

namespace PAWSwords.Passwords
{
    public class PasswordsViewModel : Screen
	{
		private readonly IPasswordsManager _passwordsManager;
		private readonly IEventAggregator _eventAggregator;
        private readonly IMasterKeyManager _masterKeyManager;

        public PasswordsViewModel(IPasswordsManager passwordsManager, IEventAggregator eventAggregator, IMasterKeyManager masterKeyManager)
		{
			_passwordsManager = passwordsManager;
			_eventAggregator = eventAggregator;
            _masterKeyManager = masterKeyManager;
        }

		public BindableCollection<PasswordViewModel> Passwords { get; } = new BindableCollection<PasswordViewModel>();

		protected override void OnDeactivate(bool close)
		{
			base.OnDeactivate(close);
		}

		public void SetPasswords(IList<LazyPasswordEntry> passwords)
		{
			Passwords.AddRange(passwords.Select(CreatePasswordViewModel));
		}

		public void Add()
		{
			_eventAggregator.PublishOnUIThread(new DisplayCreatePasswordScreenMessage());
		}

		public void Add(PasswordEntry password)
		{
			Passwords.Add(CreatePasswordViewModel(password));
		}

		private PasswordViewModel CreatePasswordViewModel(LazyPasswordEntry password)
		{
			var vm = new PasswordViewModel(password, _masterKeyManager);
			vm.RemoveRequested += OnRemoveRequested;
			return vm;
		}

        private PasswordViewModel CreatePasswordViewModel(PasswordEntry password)
        {
            var vm = new PasswordViewModel(password);
            vm.RemoveRequested += OnRemoveRequested;
            return vm;
        }

        private void OnRemoveRequested(object sender, RemovePasswordEventArgs e)
		{
			_passwordsManager.Remove(e.PasswordId);
			Passwords.Remove((PasswordViewModel)sender);
		}
	}
}
