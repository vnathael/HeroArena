using HeroArena.Data;
using HeroArena.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;

namespace HeroArena.ViewModels
{
    public class SpellViewModel : ViewModelBase
    {
        private ObservableCollection<Spell> _allSpells;
        private ObservableCollection<Spell> _filteredSpells;
        private ObservableCollection<Hero> _heroes;
        private Hero _selectedHeroFilter;
        private Spell _selectedSpell;

        public ObservableCollection<Spell> FilteredSpells
        {
            get => _filteredSpells;
            set { _filteredSpells = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Hero> Heroes
        {
            get => _heroes;
            set { _heroes = value; OnPropertyChanged(); }
        }
        public Hero SelectedHeroFilter
        {
            get => _selectedHeroFilter;
            set
            {
                _selectedHeroFilter = value;
                OnPropertyChanged();
                FilterSpells();
            }
        }

        public Spell SelectedSpell
        {
            get => _selectedSpell;
            set { _selectedSpell = value; OnPropertyChanged(); }
        }

        public SpellViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            using var context = new AppDbContext(Properties.Settings.Default.ConnectionString);

            _allSpells = new ObservableCollection<Spell>(
                context.Spells.Include(s => s.HeroSpells).ThenInclude(hs => hs.Hero).ToList()
            );
            var heroList = context.Heroes.ToList();
            Heroes = new ObservableCollection<Hero>(heroList);
            Heroes.Insert(0, new Hero { ID = 0, Name = "Tous les héros" });

            FilteredSpells = new ObservableCollection<Spell>(_allSpells);
        }

        private void FilterSpells()
        {
            if (SelectedHeroFilter == null || SelectedHeroFilter.ID == 0)
            {
                FilteredSpells = new ObservableCollection<Spell>(_allSpells);
                return;
            }

            var filtered = _allSpells.Where(s =>
                s.HeroSpells.Any(hs => hs.HeroID == SelectedHeroFilter.ID)
            );
            FilteredSpells = new ObservableCollection<Spell>(filtered);
        }
    }
}