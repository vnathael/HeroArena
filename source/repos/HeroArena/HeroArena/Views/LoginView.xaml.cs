using HeroArena.ViewModels;
using System.Windows;

namespace HeroArena.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as LoginViewModel;
            if (vm != null)
            {
                vm.LoginCommand.Execute(PasswordBox.Password);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerView = new RegisterView();
            registerView.Show();
            this.Close();
        }
    }
}