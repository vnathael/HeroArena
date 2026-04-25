using HeroArena.ViewModels;
using System.Windows;

namespace HeroArena.Views
{
    public partial class RegisterView : Window
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as RegisterViewModel;
            if (vm != null)
            {
                vm.RegisterCommand.Execute((PasswordBox.Password, ConfirmBox.Password));
            }
        }
    }
}