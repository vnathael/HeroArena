using System.Numerics;

namespace HeroArena.Models
{
    public class Login
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public Player Player { get; set; }
    }
}