
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
        private readonly List<Card> _hand = new List<Card>();
        private readonly List<Card> _cardsOnTable = new List<Card>();
        public IReadOnlyList<Card> Hand { get => _hand; }
        public IReadOnlyList<Card> CardsOnTable { get => _cardsOnTable; }
        public int ExtraDistance { get; private set; }
        public int SeeingDistance { get; private set; }
        public int AttackDistance { get; private set; }
        public int AttackRange { get => AttackDistance + SeeingDistance; }
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }
        public event EventHandler OnTakeDamage;
        public event EventHandler OnIncreaseHealth;
        public event EventHandler OnDied;
        public event EventHandler OnCardAddedToHand;
        public event EventHandler OnCardAddedToTable;
        public event EventHandler OnSuzyLafayetteHasNoCards;
        public event EventHandler OnBartCasidyTakenDamage;



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
            Card card = SeeCardFromTable(c.Id) ?? throw new ArgumentException("invalid argument");
            if (CardInfo.IsCardGun(card))
                AttackDistance -= CardInfo.GunDistance(card.Type);
            if (card.Type == PlayCard.Mustang)
                ExtraDistance -= 1;
            if (card.Type == PlayCard.Mirino)
                SeeingDistance -= 1;
            _cardsOnTable.Remove(card);
            return card;
        }

        //can apply ane blue card 
        private Card ApplyBlueCard(Card c)
        {
            if (_cardsOnTable.Find(f => f.Type  == c.Type) != null)//if same card type is at play, just throw new card into pile
                return c;
            Card temp = null;
            if (c.Type == PlayCard.Mirino) { SeeingDistance++; }
            else if (c.Type == PlayCard.Mustang) { ExtraDistance++; }
            else if (CardInfo.IsCardGun(c))//gun needs to be replaced 
            {
                temp = _cardsOnTable.Find(f => CardInfo.IsCardGun(f));
                if (temp != null)
                {
                    AttackDistance -= CardInfo.GunDistance(temp.Type);
                    AttackDistance += CardInfo.GunDistance(c.Type);
                    _cardsOnTable.Remove(temp);
                }
                else
                {
                    AttackDistance += CardInfo.GunDistance(c.Type);
                }
            }
            _cardsOnTable.Add(c);
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
            if (HeroType == Hero.BartCassidy)
                OnBartCasidyTakenDamage?.Invoke(this, EventArgs.Empty);
            OnTakeDamage?.Invoke(this, EventArgs.Empty);
        }

        public void TakeDamage() => TakeDamage(1);

        public void IncreaseHeaht() => IncreaseHealthBy(1);

        public void IncreaseHealthBy(int i)
        {
            Health += (Health <= 0) ? 0 : i;
            OnIncreaseHealth?.Invoke(this, EventArgs.Empty);
        }

        public List<Card> SeeCards(IEnumerable<int> ids) => SeeCardsFromHand(ids).Concat(SeeCardsFromTable(ids)).ToList();

        public Card SeeCardFromHand(int id) => _hand.Find(x => x.Id == id);

        public Card SeeCardFromTable(int id) => _cardsOnTable.Find(x => x.Id == id);

        public List<Card> SeeCardsFromHand(IEnumerable<int> ids) => _hand.FindAll(x => ids.Contains(x.Id));

        public List<Card> SeeCardsFromTable(IEnumerable<int> ids) => _cardsOnTable.FindAll(x => ids.Contains(x.Id));

        public Card GetCard(int id) => (SeeCardFromTable(id) != null) ? GetCardFromTable(id) : GetCardFromHand(id);
        public List<Card> GetCards(int[] ids) => GetCardsFromTable(ids).Concat(GetCardsFromHand(ids)).ToList();

        public Card GetCardFromHand(int id) => GetCardsFromHand(new int[] { id })?[0];
        public Card GetCardFromHand(PlayCard type) => GetCardsFromHand(new PlayCard[] { type })?[0];
        public Card GetCardFromTable(int id) => GetCardsFromTable(new int[] { id })?[0];
        public Card GetCardFromTable(PlayCard type) => GetCardsFromTable(new PlayCard[] { type })?[0];
        public List<Card> GetCardsFromHand(IEnumerable<int> ids)
        {
            var cards = _hand.FindAll(x => ids.Contains(x.Id));
            _hand.RemoveAll(x => ids.Contains(x.Id));
            if (!_hand.Any())
                OnSuzyLafayetteHasNoCards?.Invoke(this, EventArgs.Empty);
            return cards;
        }
        public List<Card> GetCardsFromHand(IEnumerable<PlayCard> types)
        {
            var cards = _hand.FindAll(x => types.Contains(x.Type));
            _hand.RemoveAll(x => types.Contains(x.Type));
            return cards;
        }

        public List<Card> GetCardsFromTable(IEnumerable<int> ids)
        {
            List<Card> output = new List<Card>();
            foreach (Card i in _cardsOnTable)
            {
                if (ids.Contains(i.Id))
                    output.Add(RemoveBlueCard(i));
            }
            return output;
        }
        public List<Card> GetCardsFromTable(IEnumerable<PlayCard> types)
        {
            var cards = _cardsOnTable.FindAll(x => types.Contains(x.Type));
            _cardsOnTable.RemoveAll(x => types.Contains(x.Type));
            return cards;
        }

        public void AddCardOnHand(Card card) => AddCardsOnTable(new Card[] { card });
        public void AddCardsOnHand(IEnumerable<Card> cards)
        {
            OnCardAddedToHand?.Invoke(this, EventArgs.Empty);
            _hand.AddRange(cards);
        }
        public void AddCardOnTable(Card card) => AddCardsOnTable(new Card[] { card });
        public void AddCardsOnTable(IEnumerable<Card> cards)
        {
            OnCardAddedToTable?.Invoke(this, EventArgs.Empty);
            _cardsOnTable.AddRange(cards);
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
