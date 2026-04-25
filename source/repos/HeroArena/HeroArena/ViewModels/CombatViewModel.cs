using HeroArena.Data;
using HeroArena.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace HeroArena.ViewModels
{
    public class CombatViewModel : ViewModelBase
    {
        private Hero _playerHero;
        private int _playerCurrentHp;
        private ObservableCollection<Spell> _playerSpells;

        private Hero _enemyHero;
        private int _enemyCurrentHp;
        private int _enemyMaxHp;
        private ObservableCollection<Spell> _enemySpells;

        private string _combatLog;
        private int _playerScore;
        private bool _isCombatOver;
        private string _resultMessage;

        private ObservableCollection<Hero> _availableHeroes;
        private Hero _selectedHero;

        public ObservableCollection<Hero> AvailableHeroes
        {
            get => _availableHeroes;
            set { _availableHeroes = value; OnPropertyChanged(); }
        }

        public Hero SelectedHero
        {
            get => _selectedHero;
            set { _selectedHero = value; OnPropertyChanged(); }
        }

        public Hero PlayerHero
        {
            get => _playerHero;
            set { _playerHero = value; OnPropertyChanged(); }
        }

        public int PlayerCurrentHp
        {
            get => _playerCurrentHp;
            set { _playerCurrentHp = value; OnPropertyChanged(); OnPropertyChanged(nameof(PlayerHpPercent)); }
        }

        public double PlayerHpPercent => PlayerHero == null ? 0 : (double)PlayerCurrentHp / PlayerHero.Health * 100;

        public ObservableCollection<Spell> PlayerSpells
        {
            get => _playerSpells;
            set { _playerSpells = value; OnPropertyChanged(); }
        }

        public Hero EnemyHero
        {
            get => _enemyHero;
            set { _enemyHero = value; OnPropertyChanged(); }
        }

        public int EnemyCurrentHp
        {
            get => _enemyCurrentHp;
            set { _enemyCurrentHp = value; OnPropertyChanged(); OnPropertyChanged(nameof(EnemyHpPercent)); }
        }

        public double EnemyHpPercent => _enemyMaxHp == 0 ? 0 : (double)EnemyCurrentHp / _enemyMaxHp * 100;

        public ObservableCollection<Spell> EnemySpells
        {
            get => _enemySpells;
            set { _enemySpells = value; OnPropertyChanged(); }
        }

        public string CombatLog
        {
            get => _combatLog;
            set { _combatLog = value; OnPropertyChanged(); }
        }

        public int PlayerScore
        {
            get => _playerScore;
            set { _playerScore = value; OnPropertyChanged(); }
        }

        public bool IsCombatOver
        {
            get => _isCombatOver;
            set { _isCombatOver = value; OnPropertyChanged(); }
        }

        public string ResultMessage
        {
            get => _resultMessage;
            set { _resultMessage = value; OnPropertyChanged(); }
        }

        public RelayCommand StartCombatCommand { get; }
        public RelayCommand UseSpellCommand { get; }
        public RelayCommand NewCombatCommand { get; }

        private Random _random = new Random();

        public CombatViewModel()
        {
            StartCombatCommand = new RelayCommand(ExecuteStartCombat);
            UseSpellCommand = new RelayCommand(ExecuteUseSpell);
            NewCombatCommand = new RelayCommand(ExecuteNewCombat);
            LoadHeroes();
        }

        private void LoadHeroes()
        {
            using var context = new AppDbContext(Properties.Settings.Default.ConnectionString);
            var heroes = context.Heroes
                .Include(h => h.HeroSpells).ThenInclude(hs => hs.Spell)
                .ToList();
            AvailableHeroes = new ObservableCollection<Hero>(heroes);
        }

        private void ExecuteStartCombat(object parameter)
        {
            if (SelectedHero == null)
            {
                MessageBox.Show("Choisissez un héros avant de commencer !");
                return;
            }

            PlayerHero = SelectedHero;
            PlayerCurrentHp = PlayerHero.Health;
            PlayerSpells = new ObservableCollection<Spell>(
                PlayerHero.HeroSpells.Select(hs => hs.Spell)
            );

            GenerateEnemy();

            CombatLog = "Le combat commence !\n";
            IsCombatOver = false;
            ResultMessage = "";
        }

        private void GenerateEnemy()
        {
            using var context = new AppDbContext(Properties.Settings.Default.ConnectionString);
            var allHeroes = context.Heroes
                .Include(h => h.HeroSpells).ThenInclude(hs => hs.Spell)
                .ToList();

            var candidates = allHeroes.Where(h => h.ID != PlayerHero.ID).ToList();
            if (!candidates.Any()) candidates = allHeroes.ToList();

            var baseEnemy = candidates[_random.Next(candidates.Count)];

            EnemyHero = new Hero
            {
                ID = baseEnemy.ID,
                Name = $" {baseEnemy.Name} (Mais plus fort)",
                Health = (int)(baseEnemy.Health * 1.1),
                HeroSpells = baseEnemy.HeroSpells.Select(hs => new HeroSpell
                {
                    HeroID = hs.HeroID,
                    SpellID = hs.SpellID,
                    Spell = new Spell
                    {
                        ID = hs.Spell.ID,
                        Name = hs.Spell.Name,
                        Damage = (int)(hs.Spell.Damage * 1.05),
                        Description = hs.Spell.Description
                    }
                }).ToList()
            };

            _enemyMaxHp = EnemyHero.Health;
            EnemyCurrentHp = EnemyHero.Health;
            EnemySpells = new ObservableCollection<Spell>(EnemyHero.HeroSpells.Select(hs => hs.Spell));
        }

        private void ExecuteUseSpell(object parameter)
        {
            if (IsCombatOver || PlayerHero == null || EnemyHero == null) return;

            var spell = parameter as Spell;
            if (spell == null) return;

            var log = new StringBuilder(CombatLog);

            EnemyCurrentHp -= spell.Damage;
            log.AppendLine($"Tu utilises {spell.Name} → -{spell.Damage} HP à l'ennemi. (Ennemi: {Math.Max(0, EnemyCurrentHp)} HP)");

            if (EnemyCurrentHp <= 0)
            {
                EnemyCurrentHp = 0;
                IsCombatOver = true;
                PlayerScore++;
                ResultMessage = "VICTOIRE ! Tu as vaincu l'ennemi !";
                log.AppendLine(ResultMessage);
                CombatLog = log.ToString();
                return;
            }

            var enemySpellList = EnemyHero.HeroSpells.Select(hs => hs.Spell).ToList();
            var enemySpell = enemySpellList[_random.Next(enemySpellList.Count)];
            PlayerCurrentHp -= enemySpell.Damage;
            log.AppendLine($"L'ennemi utilise {enemySpell.Name} → -{enemySpell.Damage} HP à toi. (Toi: {Math.Max(0, PlayerCurrentHp)} HP)");

            if (PlayerCurrentHp <= 0)
            {
                PlayerCurrentHp = 0;
                IsCombatOver = true;
                ResultMessage = "DÉFAITE ! Tu as été vaincu...";
                log.AppendLine(ResultMessage);
            }

            CombatLog = log.ToString();
        }

        private void ExecuteNewCombat(object parameter)
        {
            if (PlayerHero == null) return;
            PlayerCurrentHp = PlayerHero.Health;
            GenerateEnemy();
            CombatLog = "Nouveau combat !\n";
            IsCombatOver = false;
            ResultMessage = "";
        }
    }
}