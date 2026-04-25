using BCrypt.Net;
using HeroArena.Data;
using HeroArena.Models;
using System.Linq;
using System.Windows;

namespace HeroArena.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private string _username;
        private string _errorMessage;
        private string _successMessage;

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

        public string SuccessMessage
        {
            get => _successMessage;
            set { _successMessage = value; OnPropertyChanged(); }
        }

        public RelayCommand RegisterCommand { get; }
        public RelayCommand BackToLoginCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(ExecuteRegister);
            BackToLoginCommand = new RelayCommand(ExecuteBackToLogin);
        }

        private void ExecuteRegister(object parameter)
        {
            var passwords = parameter as (string password, string confirm)?;
            if (passwords == null) return;

            string password = passwords.Value.password;
            string confirm = passwords.Value.confirm;

            ErrorMessage = "";
            SuccessMessage = "";

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Le nom d'utilisateur est obligatoire.";
                return;
            }

            if (Username.Length < 3)
            {
                ErrorMessage = "Le nom d'utilisateur doit faire au moins 3 caractères.";
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Le mot de passe est obligatoire.";
                return;
            }

            if (password.Length < 6)
            {
                ErrorMessage = "Le mot de passe doit faire au moins 6 caractères.";
                return;
            }

            if (password != confirm)
            {
                ErrorMessage = "Les mots de passe ne correspondent pas.";
                return;
            }

            try
            {
                using var context = new AppDbContext(Properties.Settings.Default.ConnectionString);

                if (context.Logins.Any(l => l.Username == Username))
                {
                    ErrorMessage = "Ce nom d'utilisateur est déjà pris.";
                    return;
                }

                var login = new Login
                {
                    Username = Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
                };
                context.Logins.Add(login);
                context.SaveChanges();

                var player = new Player
                {
                    Name = Username,
                    LoginID = login.ID
                };
                context.Players.Add(player);
                context.SaveChanges();

                SuccessMessage = $"Compte '{Username}' créé ! Vous pouvez vous connecter.";
                Username = "";
            }
            catch
            {
                ErrorMessage = "Erreur de connexion à la base de données.";
            }
        }

        private void ExecuteBackToLogin(object parameter)
        {
            var loginView = new Views.LoginView();
            loginView.Show();
            Application.Current.Windows
                .OfType<Views.RegisterView>()
                .FirstOrDefault()?.Close();
        }
    }
}