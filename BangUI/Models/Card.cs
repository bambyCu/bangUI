using System;
using System.Collections.Generic;
using System.Text;

namespace BangGame
{
    public class Card
    {
        public string Num { get; set; }
        public CardColor Color { get; set; }
        public PlayCard Type { get; set; }
        public int Id { get; set; }

        public Card(string Num, CardColor Color, PlayCard Type) => 
            (this.Num, this.Color, this.Type) = (Num, Color, Type);
    }
}
