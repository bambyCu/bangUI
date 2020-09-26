using System;
using System.Collections.Generic;
using System.Linq;

namespace BangGameLibrary
{
    internal class Deck
    {
        private List<Card> CardsInDeck = new List<Card>();
        private List<Card> CardsInPile = new List<Card>();
        public event EventHandler OnDraw;
        public event EventHandler OnMixPiles;
        public event EventHandler OnCardAddedToPile;

        public Deck(List<Card> cardsToDeck)
        {
            if(cardsToDeck.Count == 0)
                throw (new Exception("Deck has no input cards for set up in constructor"));
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

        public void PutCardOnDeck(Card c) => CardsInDeck.Insert(0, c);

        private Card Drawing()
        {
            if (!CardsInDeck.Any())
                MixPiles();
            var temp = CardsInDeck[0];
            CardsInDeck.RemoveAt(0);
            return temp;
        }

        public  Card Draw()
        {
            OnDraw?.Invoke(this, EventArgs.Empty);
            return Drawing();
        }

        public List<Card> Draw(int amount)
        {
            OnDraw?.Invoke(this, EventArgs.Empty);
            return Enumerable.Range(0, amount).Select(x => Drawing()).ToList();
        }
            
        public Card DrawForEffect()
        {
            var card = Draw();
            CardToPile(card);
            return card;
        }

        //function ignores null
        public void CardToPile(Card c)
        {
            OnCardAddedToPile?.Invoke(this, EventArgs.Empty);
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

        public Card SeeTopOfPile()
        {
            if (!CardsInPile.Any()) 
                return null; 
            return CardsInPile.Last();
        }


        private void MixPiles()
        {
            OnMixPiles?.Invoke(this, EventArgs.Empty);
            if (CardsInPile.Count <= 1) 
                return;
            CardsInDeck.AddRange(CardsInPile);
            CardsInPile.Clear();
            Shuffle();
        }
    }
        
}
