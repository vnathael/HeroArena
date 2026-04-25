using BCrypt.Net;
using HeroArena.Data;
using HeroArena.Models;
using System.Linq;
using System.Windows;

namespace HeroArena.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private string _errorMessage;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public RelayCommand LoginCommand { get; }
        public RelayCommand OpenSettingsCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin);
            OpenSettingsCommand = new RelayCommand(ExecuteOpenSettings);
        }

        private void ExecuteLogin(object parameter)
        {
            string password = parameter as string;

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
            {
                ErrorMessage = "Veuillez remplir tous les champs.";
                return;
            }

            try
            {
                using var context = new AppDbContext(Properties.Settings.Default.ConnectionString);
                var login = context.Logins.FirstOrDefault(l => l.Username == Username);

                if (login == null)
                {
                    ErrorMessage = "Utilisateur introuvable.";
                    return;
                }
                bool isValid = BCrypt.Net.BCrypt.Verify(password, login.PasswordHash);

                if (!isValid)
                {
                    ErrorMessage = "Mot de passe incorrect.";
                    return;
                }
                var mainWindow = new Views.MainWindow(login);
                mainWindow.Show();
                Application.Current.Windows[0].Close();
            }
            catch
            {
                ErrorMessage = "Erreur de connexion à la base de données.";
            }
        }

        private void ExecuteOpenSettings(object parameter)
        {
            var settingsWindow = new Views.SettingsView();
            settingsWindow.ShowDialog();
        }
    }
}