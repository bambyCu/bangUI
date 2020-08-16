using Microsoft.Ajax.Utilities;
using Microsoft.Owin.Security.Provider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;

namespace BangGame
{
    public class Deck
    {
        public List<Card> CardsInDeck = new List<Card>();

        public List<Card> CardsInPile = new List<Card>();

        public Deck(List<Card> cardsToDeck)
        {
            CardsInDeck = cardsToDeck;
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
            CardsInDeck.RemoveAt(0);
            return temp;
        }

        public List<Card> Draw(int amount) => 
            Enumerable.Range(0, amount).Select(x => Draw()).ToList();

        //function ignores null
        public void CardToPile(Card c)
        {
            if (c != null)
                CardsInPile.Add(c);
        }

        public Card TopOfPile()
        {
            if (!CardsInPile.Any()) 
                return null; 
            var temp = CardsInPile.Last();
            CardsInPile.Remove(temp);
            return temp;
        }
        private void MixPiles()
        {
            if (CardsInPile.Count <= 1) 
                return;
            CardsInDeck.AddRange(CardsInPile);
            CardsInPile.Clear();
            Shuffle();
        }
    }
        
}
