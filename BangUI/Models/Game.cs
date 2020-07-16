using System;
using System.Collections.Generic;
using System.Text;

namespace BangGame
{
    public class Game
    {
        private Deck PlayDeck = 
            new Deck(@"C:\Users\sedif\source\repos\BangGame\BangGame\Resources\PlayingCards.txt");
        private List<Player> players;
        private List<string> Names;
        Game(List<string> names, IComunicationCanal c)
        {
            Names = names;
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
