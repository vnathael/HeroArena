using System.Collections.Generic;

namespace HeroArena.Models
{
    public class Hero
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }
        public string ImageURL { get; set; }
        public ICollection<HeroSpell> HeroSpells { get; set; }
        public ICollection<PlayerHero> PlayerHeroes { get; set; }
    }
}