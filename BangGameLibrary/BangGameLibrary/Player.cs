
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangGameLibrary
{
    public class Player
    {
        public string Name { get; private set; }
        public Hero HeroType { get; private set; }
        public Role RoleType { get; private set; }
        public List<Card> Hand = new List<Card>();
        public List<Card> CardsOnTable = new List<Card>();
        public int ExtraDistance { get; private set; }
        public int SeeingDistance { get; private set; }
        public int AttackDistance { get; private set; }
        public int AttackRange { get => AttackDistance + SeeingDistance; }
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }
        public event EventHandler OnHealthChanged;
        public event EventHandler OnDied;

        public Player(string name, Hero heroType, Role roleType, int distanceFromOthers, int seeingDistance, int seeingAttackDistance, int maxHealt)
        {
            Name = name;
            HeroType = heroType;
            RoleType = roleType;
            ExtraDistance = distanceFromOthers;
            SeeingDistance = seeingDistance;
            AttackDistance = seeingAttackDistance;
            if (RoleType == Role.Sherif) { maxHealt++; }
            MaxHealth = maxHealt;
            Health = maxHealt;
        }

        public Player(string heroType, int distanceFromOthers, int seeingDistance, int seeingAttackDistance, int maxHealt)
        {
            HeroType = (Hero)Enum.Parse(typeof(Hero), heroType, true);
            ExtraDistance = distanceFromOthers;
            SeeingDistance = seeingDistance;
            AttackDistance = seeingAttackDistance;
            MaxHealth = maxHealt;
        }

        
        //all functions that return Card return object that is to be placed into used pile
        private Card RemoveBlueCard(Card c)
        {
            Card Temp = CardsOnTable.Find(f => f.Type == c.Type) ?? throw new ArgumentException("invalid argument");
            if (CardInfo.IsCardGun(Temp))
                AttackDistance -= CardInfo.GunDistance(Temp.Type);
            if (Temp.Type == PlayCard.Mustang)
                ExtraDistance -= 1;
            if (Temp.Type == PlayCard.Mirino)
                SeeingDistance -= 1;
            CardsOnTable.Remove(Temp);
            return Temp;
        }

        //can apply ane blue card 
        private Card ApplyBlueCard(Card c)
        {
            if (CardsOnTable.Find(f => f.Type  == c.Type) != null)//if same card type is at play, just throw new card into pile
                return c;
            Card temp = null;
            if (c.Type == PlayCard.Mirino) { SeeingDistance++; }
            else if (c.Type == PlayCard.Mustang) { ExtraDistance++; }
            else if (CardInfo.IsCardGun(c))//gun needs to be replaced 
            {
                temp = CardsOnTable.Find(f => CardInfo.IsCardGun(f));
                if (temp != null)
                {
                    AttackDistance -= CardInfo.GunDistance(temp.Type);
                    AttackDistance += CardInfo.GunDistance(c.Type);
                    CardsOnTable.Remove(temp);
                }
                else
                {
                    AttackDistance += CardInfo.GunDistance(c.Type);
                }
            }
            CardsOnTable.Add(c);
            return temp;
        }

        public Card ApplySelfCard(Card c)
        {
            if (!CardInfo.IsSelfApplyCard(c))
                throw (new Exception("Card that needs to be managed by Game class is sent to player"));
            if (CardInfo.IsCardBlue(c)) 
                return ApplyBlueCard(c); 
            if (c.Type == PlayCard.Beer)
            {
                if (Health < MaxHealth) { Health++; }
                return c;
            }
            throw (new Exception("It seems that CardInfo is not correctly setup"));
            //if this exception has been thrown CardInfo has been incorrectly changed
            //return null;
        }
        public bool HasOnTable(PlayCard cardType) =>
            CardsOnTable.Any(x => x.Type == cardType);
        
        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health <= 0)
                OnDied?.Invoke(this, EventArgs.Empty);
            OnHealthChanged?.Invoke(this, EventArgs.Empty);
        }

        public void TakeDamage() => TakeDamage(1);

        public void IncreaseHeaht() => IncreaseHealthBy(1);

        public void IncreaseHealthBy(int i)
        {
            Health += (Health <= 0) ? 0 : i;
            OnHealthChanged?.Invoke(this, EventArgs.Empty);
        }



        public override string ToString()
        {
            return "Name: " + Name + "\n" +
                "Role: " + RoleType + "\n" +
                "Hero: " + HeroType + "\n" + 
                "Max health: " +MaxHealth + "\n" +
                "Health: " + Health + "\n" +
                "Seeing distance: " + SeeingDistance + "\n" +
                "Attack distance: " + AttackDistance + "\n" +
                "Distance from others: " + ExtraDistance + "\n";
        }

    }
}
