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
        private Deck PlayDeck =
            new Deck(@"C:\Users\sedif\source\repos\BangGame\BangGame\Resources\PlayingCards.txt");
        private List<Player> Players;
        private List<string> Names;
        
        Game(List<string> names, IComunicationCanal c)
        {
            if (!CardInfo.GameRoles.ContainsKey(names.Count))
            {
                //TO DO if game has wrong amount of players
            }

            
            //System.Console.WriteLine(k[0].Hero);


            
            var k = JsonConvert.DeserializeObject<Card[]>(d);
            System.Console.WriteLine(k[k.Count() - 1].Name);
            var random = new Random();
            var gameRoles = new List<Role>(GameRoles[names.Count]);
            var heroRoles = Enum.GetValues(typeof(Hero)).Cast<Hero>().ToList();
            foreach(var i in names)
            {
                int randVal = random.Next(gameRoles.Count);
                var role = gameRoles[randVal];
                gameRoles.RemoveAt(randVal);
                randVal = random.Next(heroRoles.Count);
                var hero = heroRoles[randVal];
                heroRoles.RemoveAt(randVal);
                Players.Add(new Player(i, gameRoles[random.Next(gameRoles.Count)]));
            }
        }

        public void Setup()
        {
            foreach( string s in Names)
            {
                players.Add(new Player(s));
            }
        }

        public void playSelfCard(Card c, Player p)
        {
            p.ApplySelfCard(c);
        }

    }
}
