
using System;
using System.Collections.Generic;
using System.Text;

namespace BangGame
{
    public class Player
    {
        public string Name { get; set; }
        public Hero HeroType { get; set; }
        public Role RoleType { get; set; }
        public List<Card> Hand = new List<Card>();
        public List<Card> CardsOnTable = new List<Card>();
        public int DistanceFromOthers { get; set; }
        public int SeeingDistance { get; set; }
        public int SeeingAttackDistance { get; set; }
        public int MaxHealth { get; set; }
        public int Lives { get; set; }

        public Player(string name)
        {
            Name = name;
        }

        public Player(int Lives, int DistanceFromOthers, int SeeingDistance, int SeeingAttackDistance, string HeroType)
        {
            this.HeroType = (Hero)Enum.Parse(typeof(Hero), HeroType, true);
            this.DistanceFromOthers = DistanceFromOthers;
            this.Lives = Lives;
            this.MaxHealth = Lives;
            this.SeeingAttackDistance = SeeingAttackDistance;
            this.SeeingDistance = SeeingDistance;
        }


        //all functions that return Card return object that is to be placed into used pile
        private Card RemoveBlueCard(Card c)
        {
            if (CardInfo.isCardYellow(c) || c == null) { return null; }
            Card Temp;
            if ((Temp = CardsOnTable.Find(f => f.Type == c.Type)) == null) { return null; }
            if (Temp.Type == PlayCard.Mirino)
            {
                SeeingDistance -= 1;
                CardsOnTable.Remove(Temp);
                return Temp;
            }
            if (Temp.Type == PlayCard.Mustang)
            {
                DistanceFromOthers -= 1;
                CardsOnTable.Remove(Temp);
                return Temp;
            }
            if (CardInfo.isCardGun(Temp))
            {
                SeeingAttackDistance -= CardInfo.GunDistance(Temp.Type);
                return Temp;
            }
            CardsOnTable.Remove(Temp);
            return Temp;
        }

        //can apply ane blue card 
        private Card ApplyBlueCard(Card c)
        {
            if (CardInfo.isCardYellow(c) || c == null) { return null; }
            if (CardsOnTable.Find(f => CardInfo.isCardGun(f)) != null){//if same card is at play, just throw new card into pile
                return c;
            }
            Card Temp;
            if (CardInfo.isCardGun(c) && (Temp = CardsOnTable.Find(f => CardInfo.isCardGun(f))) != null)
            {
                SeeingAttackDistance -= CardInfo.GunDistance(Temp.Type);
                CardsOnTable.Remove(Temp);
                SeeingAttackDistance += CardInfo.GunDistance(c.Type);
                CardsOnTable.Add(c);
                return Temp;
            }
            if (CardInfo.isCardGun(c) && CardsOnTable.Find(f => CardInfo.isCardGun(f)) == null) { SeeingAttackDistance += CardInfo.GunDistance(c.Type);}
            if(c.Type == PlayCard.Mirino) { SeeingDistance++; } 
            if(c.Type == PlayCard.Mustang) { DistanceFromOthers++; }
            CardsOnTable.Add(c);
            return null;
        }

        public Card ApplySelfCard(Card c)
        {
            if (!CardInfo.IsSelfApplyCard(c))
            {
                throw (new Exception("Card that needs to be managed by Game class is sent to player"));
            }
            if (CardInfo.IsCardBlue(c)) { return ApplyBlueCard(c); }
            if (c.Type == PlayCard.Beer)
            {
                if (Lives < MaxHealth) { Lives++; }
                return c;
            }
            throw (new Exception("It seems that CardInfo is not correctly setup"));
            //if this exception has been thrown CardInfo has been incorrectly changed
            //return null;
        }

    }
}
