using Autofac;
using Caliburn.Micro;
using Caliburn.Micro.Autofac;
using PAWSwords.Core.Crypto;
using PAWSwords.Core.Interfaces;
using PAWSwords.Core.Master;
using PAWSwords.Core.Passwords;
using PAWSwords.Core.Storage;
using PAWSwords.MasterPassword;
using PAWSwords.Passwords;
using System.Windows;

namespace PAWSwords
{
    public class Bootstrapper : AutofacBootstrapper<ShellViewModel>
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
            builder.RegisterType<PasswordsManager>().As<IPasswordsManager>().SingleInstance();
            builder.RegisterType<FileStorage>().SingleInstance();
            builder.RegisterType<AesCrypter>().SingleInstance();
            builder.RegisterType<MasterKeyManager>().As<IMasterKeyManager>().SingleInstance();

            builder.RegisterType<ShellViewModel>().SingleInstance();
            builder.RegisterType<CreateMasterPasswordViewModel>().SingleInstance();
            builder.RegisterType<EnterMasterPasswordViewModel>().SingleInstance();
            builder.RegisterType<CreatePasswordViewModel>();
        }

        protected override void ConfigureBootstrapper()
        {
            base.ConfigureBootstrapper();
            EnforceNamespaceConvention = false;
        }

        protected async override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
    
            var shell = Container.Resolve<ShellViewModel>();
            if (await Container.Resolve<IMasterKeyManager>().IsCreated())
            {
                shell.DisplayEnterMasterPasswordScreen();
            }
            else
            {
                shell.DisplayCreateMasterPasswordScreen();
            }
        }
    }
}
