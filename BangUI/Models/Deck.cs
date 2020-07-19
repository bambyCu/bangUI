using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BangGame
{
    public class Deck
    {
        public List<Card> CardsInDeck = new List<Card>();

        public List<Card> CardsInPile = new List<Card>();

        public Deck(List<Card> cardsInDeck)
        {
            CardsInDeck = cardsInDeck;
            Shuffle();
        }
        //this code has been donated from StackOverflow
        public void Shuffle()
        {
            int n = CardsInDeck.Count;
            Random rng = new Random();
            Card value;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                value = CardsInDeck[k];
                CardsInDeck[k] = CardsInDeck[n];
                CardsInDeck[n] = value;
            }
        }

        public Card Draw()
        {
            if (!CardsInDeck.Any())
            {
                MixPiles();
            }
            var temp = CardsInDeck[0];
            CardsInDeck.Remove(temp);
            return temp;
        }

        //function ignores null
        public void CardToPile(Card c)
        {
            if (c == null) { return; }
            CardsInPile.Add(c);
        }

        public Card TopOfPile()
        {
            if (!CardsInPile.Any()) { return null; }
            var temp = CardsInPile.Last();
            CardsInPile.Remove(temp);
            return temp;
        }
        private void MixPiles()
        {
            if (CardsInPile.Count <= 1) { return; }
            for(int i = 0; i < CardsInPile.Count-1; i++)
            {
                CardsInDeck.Add(CardsInPile[0]);
                CardsInPile.RemoveAt(0);
            }
            Shuffle();
        }
    }
        
}
