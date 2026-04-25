using BCrypt.Net;
using HeroArena.Data;
using HeroArena.Models;
using System.Linq;
using System.Windows;

namespace HeroArena.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _connectionString;
        private string _statusMessage;

        public string ConnectionString
        {
            get => _connectionString;
            set { _connectionString = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand InitDataCommand { get; }

        public SettingsViewModel()
        {
            ConnectionString = Properties.Settings.Default.ConnectionString;
            SaveCommand = new RelayCommand(ExecuteSave);
            InitDataCommand = new RelayCommand(ExecuteInitData);
        }

        private void ExecuteSave(object parameter)
        {
            Properties.Settings.Default.ConnectionString = ConnectionString;
            Properties.Settings.Default.Save();
            StatusMessage = "Paramètres sauvegardés.";
        }

        private void ExecuteInitData(object parameter)
        {
            try
            {
                using var context = new AppDbContext(ConnectionString);

                if (context.Logins.Any() || context.Heroes.Any() || context.Spells.Any())
                {
                    StatusMessage = "Des données existent déjà.";
                    return;
                }

                var spell1 = new Spell { Name = "Coup franc", Damage = 45, Description = "Vous balance un ballon en pleine gueule, pour une fois, ça touche" };
                var spell2 = new Spell { Name = "Jeu d'acteur en français", Damage = 35, Description = "Un jeu d'acteur si parfait que ça pique les yeux." };
                var spell3 = new Spell { Name = "Salut ! C'est Franck le boeuf", Damage = 55, Description = "Intimide les enemis et les pousse à se blesser" };
                var spell4 = new Spell { Name = "Vendezvotrevoiture.fr", Damage = 70, Description = "Vend votre voiture de force. Un super deal." };

                var spell5 = new Spell { Name = "Triple Tung", Damage = 60, Description = "Attaque trois fois à coup de batte." };
                var spell6 = new Spell { Name = "Sahur Shield", Damage = 10, Description = "Se protège et contre-attaque." };
                var spell7 = new Spell { Name = "Anti-Brainrot", Damage = 50, Description = "Attaque tous les ennemis autour (et particulièrement Brr Brr parapim)." };
                var spell8 = new Spell { Name = "Homerun", Damage = 80, Description = "Une frappe mortelle sur une cible affaiblie." };

                var spell9 = new Spell { Name = "Mauvaise Malice", Damage = 55, Description = "Votre enemi marchait de manière innocente. Et c'est là qu'il le vis : Larry, le malicieux. 'Oh non s'il te plait, ne me donne pas ta malice malicieusement malicieuse...' mais Larry ne perds pas un instant. Il lui jette un sort rempi de mauvaise malice. 'Oh non ! Je me suis tranformé ! Je suis devenu.... Le fripon.' " };
                var spell10 = new Spell { Name = "le rire", Damage = 30, Description = "Si jamais tu croises Larry, cours vite ! Si jamais tu vois Larry en mode malicieux, cours pour ta vie ! Mais si tu l'entends rire... Ahahaha... Disons juste que... C'est déjà trop tard." };
                var spell11 = new Spell { Name = "Toucher Nocturne", Damage = 20, Description = "Il va vous toucher la nuit." };
                var spell12 = new Spell { Name = "Le gang", Damage = 65, Description = "Vous vous baladiez tranquillou bilou dans le parc, pensant à votre liste de courses que vous alliez acheter, quand vous avez vu l'impensabilité impensable, c'était... Larry. Il n'était pas tout seul, mais avec son gang : Larry le malicieux lui même, Jacky le fourbe et James le Bourlingueur." };

                context.Spells.AddRange(spell1, spell2, spell3, spell4,
                                        spell5, spell6, spell7, spell8,
                                        spell9, spell10, spell11, spell12);
                var mage = new Hero { Name = "Franck Leboeuf", Health = 800, ImageURL = "" };
                var warrior = new Hero { Name = "Tung Tung Tung Sahur", Health = 1200, ImageURL = "" };
                var assassin = new Hero { Name = "Larry Le Malicieux", Health = 900, ImageURL = "" };

                context.Heroes.AddRange(mage, warrior, assassin);

                context.SaveChanges();

                context.HeroSpells.AddRange(
                    new HeroSpell { HeroID = mage.ID, SpellID = spell1.ID },
                    new HeroSpell { HeroID = mage.ID, SpellID = spell2.ID },
                    new HeroSpell { HeroID = mage.ID, SpellID = spell3.ID },
                    new HeroSpell { HeroID = mage.ID, SpellID = spell4.ID },

                    new HeroSpell { HeroID = warrior.ID, SpellID = spell5.ID },
                    new HeroSpell { HeroID = warrior.ID, SpellID = spell6.ID },
                    new HeroSpell { HeroID = warrior.ID, SpellID = spell7.ID },
                    new HeroSpell { HeroID = warrior.ID, SpellID = spell8.ID },

                    new HeroSpell { HeroID = assassin.ID, SpellID = spell9.ID },
                    new HeroSpell { HeroID = assassin.ID, SpellID = spell10.ID },
                    new HeroSpell { HeroID = assassin.ID, SpellID = spell11.ID },
                    new HeroSpell { HeroID = assassin.ID, SpellID = spell12.ID }
                );

                context.SaveChanges();

                var loginAdmin = new Login
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
                };
                context.Logins.Add(loginAdmin);
                context.SaveChanges();

                var player = new Player { Name = "Joueur Admin", LoginID = loginAdmin.ID };
                context.Players.Add(player);
                context.SaveChanges();

                StatusMessage = "Données initialisées ! (admin / admin123)";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Erreur : {ex.Message}";
            }
        }
    }
}