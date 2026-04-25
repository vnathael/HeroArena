using HeroArena.Data;
using HeroArena.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;

namespace HeroArena.ViewModels
{
    public class HeroViewModel : ViewModelBase
    {
        private Hero _selectedHero;
        private ObservableCollection<Hero> _heroes;

        public ObservableCollection<Hero> Heroes
        {
            get => _heroes;
            set { _heroes = value; OnPropertyChanged(); }
        }

        public Hero SelectedHero
        {
            get => _selectedHero;
            set
            {
                _selectedHero = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedHeroSpells));
            }
        }

        public ObservableCollection<Spell> SelectedHeroSpells
        {
            get
            {
                if (SelectedHero == null || SelectedHero.HeroSpells == null)
                    return new ObservableCollection<Spell>();

                var spells = SelectedHero.HeroSpells
                    .Where(hs => hs.Spell != null)
                    .Select(hs => hs.Spell)
                    .ToList();

                return new ObservableCollection<Spell>(spells);
            }
        }

        public HeroViewModel()
        {
            LoadHeroes();
        }

        private void LoadHeroes()
        {
            using var context = new AppDbContext(Properties.Settings.Default.ConnectionString);
            var heroes = context.Heroes
                .Include(h => h.HeroSpells)
                .ThenInclude(hs => hs.Spell)
                .ToList();

            var distinct = heroes.GroupBy(h => h.ID).Select(g => g.First()).ToList();
            Heroes = new ObservableCollection<Hero>(distinct);
        }
    }
}