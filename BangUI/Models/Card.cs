using System;
using System.Collections.Generic;
using System.Text;

namespace BangGame
{
    public class Card
    {
        public string Name { get; set; }
        public string Num { get; set; }
        public CardColor Color { get; set; }
        public string Picture { get; set; }
        public PlayCard Type { get; set; }

        public Card(string Name, string CardNum, CardColor ColorOfCard, string CardPicture, PlayCard Type)
        {
            this.Name = Name;
            this.Num = CardNum;
            this.Color= ColorOfCard;
            this.Picture = CardPicture;
            this.Type = Type;
        }

        public override string ToString() {
            return $"name: {Name}\n" +
                    $"number: {Num}\n" +
                    $"color: {Color}\n" +
                    $"path: {Picture}\n" +
                    $"type: {Type}\n";
        }

    }
}
