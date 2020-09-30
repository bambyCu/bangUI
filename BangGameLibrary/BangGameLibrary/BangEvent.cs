using System;
using System.Collections.Generic;
using System.Text;

namespace BangGameLibrary
{
    public class BangGameEventArgs : EventArgs
    {
        public string Message { get; set; }
        public BangGameEventArgs(string message)
        {
            Message = message;
        }
    }
    public class DisputeEventArgs : EventArgs
    {
        public Player Attacker { get; set; }
        public DisputeEventArgs(Player attacker)
        {
            Attacker = attacker;
        }
    }

    public class ListOfCardsEventArgs : EventArgs
    {
        public IReadOnlyList<Card> Cards { get; set; }
        public ListOfCardsEventArgs(IReadOnlyList<Card> cards)
        {
            Cards = cards;
        }
    }


    public class PileInfoEventArgs : EventArgs
    {
        public Card Top { get; set; }
        public int PileSize { get; set; }
        public int DeckSize { get; set; }
        public PileInfoEventArgs(Card card, int pileSize, int deckSize)
        {
            Top = card;
            PileSize = pileSize;
            DeckSize = deckSize;
        }
    }



    public delegate void BangGameEventHandler(Object sender, BangGameEventArgs e);
    public delegate void TwoPlayerDisputeEventHandler(Object sender, DisputeEventArgs e);
    public delegate void CardListEventHandler(Object sender, ListOfCardsEventArgs e);
    public delegate void PileChangeEventHandler(Object sender, PileInfoEventArgs e);
}
