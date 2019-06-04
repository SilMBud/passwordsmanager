using Caliburn.Micro;
using PAWSwords.Core.Interfaces;
using PAWSwords.MasterPassword;
using PAWSwords.Passwords;
using PAWSwords.Passwords.Messages;
using System;

namespace PAWSwords
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive,
        IHandle<MasterKeyLoadedMessage>,
        IHandle<PasswordAddedMessage>,
        IHandle<DisplayCreatePasswordScreenMessage>,
        IHandle<PasswordCreationCanceledMessage>
    {
        private readonly CreateMasterPasswordViewModel _createMasterPasswordViewModel;
        private readonly EnterMasterPasswordViewModel _enterMasterPasswordViewModel;
        private readonly Func<CreatePasswordViewModel> _createPasswordViewModel;
        private readonly PasswordsViewModel _passwordsViewModel;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPasswordsManager _passwordsManager;

        public ShellViewModel(
            CreateMasterPasswordViewModel createMasterPasswordViewModel,
            EnterMasterPasswordViewModel enterMasterPasswordViewModel,
            Func<CreatePasswordViewModel> createPasswordViewModel,
            PasswordsViewModel passwordsViewModel,
            IEventAggregator eventAggregator,
            IPasswordsManager passwordsManager)
        {
            _createMasterPasswordViewModel = createMasterPasswordViewModel;
            _enterMasterPasswordViewModel = enterMasterPasswordViewModel;
            _createPasswordViewModel = createPasswordViewModel;
            _passwordsViewModel = passwordsViewModel;
            _eventAggregator = eventAggregator;
            _passwordsManager = passwordsManager;

            Items.AddRange(new Screen[] {
                _createMasterPasswordViewModel,
                _enterMasterPasswordViewModel,
                _passwordsViewModel });
        }

        protected override void OnActivate()
        {
            _eventAggregator.Subscribe(this);
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventAggregator.Unsubscribe(this);
        }

        public void DisplayEnterMasterPasswordScreen()
        {
            ActivateItem(_enterMasterPasswordViewModel);
        }

        public void DisplayCreateMasterPasswordScreen()
        {
            ActivateItem(_createMasterPasswordViewModel);
        }

        public async void Handle(MasterKeyLoadedMessage message)
        {
            var passwords = await _passwordsManager.GetAll();
            _passwordsViewModel.SetPasswords(passwords);
            ActivateItem(_passwordsViewModel);
        }

        public void Handle(PasswordAddedMessage message)
        {
            _passwordsViewModel.Add(message.AddedPassword);
            ActivateItem(_passwordsViewModel);
        }

        public void Handle(DisplayCreatePasswordScreenMessage message)
        {
            ActivateItem(_createPasswordViewModel());
        }

        public void Handle(PasswordCreationCanceledMessage message)
        {
            ActivateItem(_passwordsViewModel);
        }
    }
}
