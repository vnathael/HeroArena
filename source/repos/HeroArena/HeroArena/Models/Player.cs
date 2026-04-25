using System.Collections.Generic;

namespace HeroArena.Models
{
    public class Player
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? LoginID { get; set; }
        public Login Login { get; set; }
        public ICollection<PlayerHero> PlayerHeroes { get; set; }
    }
}