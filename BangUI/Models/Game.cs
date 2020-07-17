using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BangGame
{
    public class Game
    {
        public Deck PlayDeck;
        public List<Player> Players;
        public List<string> Names;

        public Game(List<string> names)
        {
            if (!CardInfo.GameRoles.ContainsKey(names.Count))
            {
                //TO DO if game has wrong amount of players
            }
            var random = new Random();
            var gameRoles = new List<Role>(CardInfo.GameRoles[names.Count]);
            var heroRoles = new List<Player>(CardInfo.HeroVals);
            Players = new List<Player>();
            foreach (var i in names)
            {
                int randVal = random.Next(gameRoles.Count);
                var role = gameRoles[randVal];
                gameRoles.RemoveAt(randVal);
                randVal = random.Next(heroRoles.Count);
                var hero = heroRoles[randVal];
                heroRoles.RemoveAt(randVal);
                Players.Add(new Player(i, hero.HeroType, role, hero.DistanceFromOthers, hero.SeeingDistance, hero.SeeingAttackDistance, hero.MaxHealth));
            }
        }

        public void playSelfCard(Card c, Player p)
        {
            p.ApplySelfCard(c);
        }
    }
}
