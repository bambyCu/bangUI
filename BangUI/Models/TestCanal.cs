using System;
using System.Collections.Generic;
using System.Text;

namespace BangGame
{
    public class TestCanal : IComunicationCanal
    {
        public void PickHero(ref Player p,Hero firstChoice, Hero secondChoice)
        {
            p.HeroType = firstChoice;
        }
    }
}
