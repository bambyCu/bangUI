
using System;
using System.Collections.Generic;
using System.Linq;

namespace BangGameLibrary
{
    public class BangGame
    {
        private readonly Deck PlayDeck;
        public readonly List<Player> Players;
        private int CurrPlayerIndex = 0;
        public bool CanPlayBang = true; 
        private AttackManager Attacks = new AttackManager();
        public event EventHandler OnUnableToPlayCard;
        public event EventHandler OnTakeCardAwai;
        public event EventHandler OnBlock;
        public event EventHandler OnDeath;
        public event EventHandler OnHealthChange;
        public event EventHandler OnCardDrawn;
        public event EventHandler OnMixedPiles;
        public event EventHandler OnCardAddedToPile;
        public event EventHandler OnNewRound;

        public BangGame(List<string> names)
        {
            Players = new List<Player>();
            List<Role> gameRoles ;
            List<Player> heroRoles;
            lock (CardInfo.AvailableCards)
            {
                PlayDeck = new Deck(CardInfo.AvailableCards);
                gameRoles = new List<Role>(CardInfo.GameSetup(names.Count));
                heroRoles = new List<Player>(CardInfo.HeroVals);
            }
            var roleIndexes = GenerateRandomizedIndexList(0, gameRoles.Count);
            var heroIndexes = GenerateRandomizedIndexList(0, heroRoles.Count);
            for (int i = 0 ; i < names.Count;i++)
            {
                var role = gameRoles[roleIndexes[i]];
                var hero = heroRoles[heroIndexes[i]];
                Players.Add(
                    new Player(names[i], hero.HeroType, role, hero.ExtraDistance, hero.SeeingDistance, hero.AttackDistance, hero.MaxHealth)
                    );
            }
            Players.ForEach(x => x.Hand.AddRange(PlayDeck.Draw(x.MaxHealth)));
            //-----------------------------------events----------------------------
            Players.ForEach(x => x.OnDied += PlayerDeath);
            Players.ForEach(x => x.OnHealthChanged += PlayerHealthChanged);
            PlayDeck.OnCardAddedToPile += CardAddedToPile;
            PlayDeck.OnDraw += CardDrawn;
            PlayDeck.OnMixPiles += PilesMixed;

            CurrentPlayer.Hand.AddRange(PlayDeck.Draw(2));//start of turn
            ;
        }

        public Player CurrentPlayer
        {
            get => Players[CurrPlayerIndex];
        }
        public int NextIndex
        {
            get
            {
                for (int i = CurrPlayerIndex; ; i += (i + 1 >= Players.Count) ? 0 : i + 1)
                {
                    if(Players[i].Health > 0)
                        return i;
                }
            }
        }

        public bool PlayerWithinRange(Player aggresor, Player victim) => Distance(aggresor, victim) <= aggresor.AttackRange;

        public int Distance(Player distanceStart, Player distanceEnd)
        {
            var startIndex = Players.IndexOf(distanceStart);
            var plusDistance = 0;
            var minusDistance = 0;
            for (int i = startIndex; distanceEnd != Players[i]; i = ((i + 1) >= Players.Count)  ? 0                 : i + 1)
                plusDistance += (Players[i].Health > 0) ? 1 : 0;
            for (int i = startIndex; distanceEnd != Players[i]; i = ((i - 1) < 0)               ? Players.Count-1   : i - 1)
                minusDistance += (Players[i].Health > 0) ? 1 : 0;
            return (minusDistance > plusDistance) ? plusDistance : minusDistance;
        }
        
        public void DiscardCard(string player, int cardsId) => DiscardCards(player, new int[] { cardsId });
        public void DiscardCard(Player player, int cardsId) => DiscardCards(player, new int[] { cardsId });
        public void DiscardCards(string player, int[] cardsId) => DiscardCards(Players.Find(x => x.Name == player), cardsId);

        public void DiscardCards(Player player, int[] cardsId)
        {
            player.Hand.ForEach(x => { if (cardsId.Contains(x.Id)) PlayDeck.CardToPile(x); } );
            player.Hand.RemoveAll(x => cardsId.Contains(x.Id));
            player.CardsOnTable.ForEach(x => { if (cardsId.Contains(x.Id)) PlayDeck.CardToPile(x); });
            player.CardsOnTable.RemoveAll(x => cardsId.Contains(x.Id));
            
        }

        private Role? IsGameFinshed()
        {
            if (Players.All(x => x.RoleType == Role.Outlaw)) 
                return Role.Outlaw;
            if (Players.All(x => x.RoleType == Role.Sherif || x.RoleType == Role.Deputy))
                return Role.Sherif;
            if (Players.All(x => x.RoleType == Role.Renegate))
                return Role.Renegate;
            return null;
        }

        public void ApplyCardToPlayer(Card card, Player victim)
        {
            if (card.Type == PlayCard.Missed)
            {
                OnUnableToPlayCard?.Invoke(this, EventArgs.Empty);
                return;
            }
            if (CardInfo.IsSelfApplyCard(card))
            {
                var tempCard = CurrentPlayer.ApplySelfCard(card);
                CurrentPlayer.Hand.Remove(card);
                PlayDeck.CardToPile(tempCard);
                return;
            }
            if (CardInfo.IsAttackCard(card) && !Attacks.ReadyToAttack)
            {
                OnUnableToPlayCard?.Invoke(this, EventArgs.Empty);
                return;
            }
            if (card.Type == PlayCard.Prison && victim.CardsOnTable.Any(x => x.Type == PlayCard.Prison))
            {
                OnUnableToPlayCard?.Invoke(this, EventArgs.Empty);
                return;
            }

            switch (card.Type)
            {
                case PlayCard.Dynamite:
                    victim.CardsOnTable.Add(card);
                    break;
                case PlayCard.Prison:
                    victim.CardsOnTable.Add(card);
                    break;
                case PlayCard.Bang:
                    if (CanPlayBang && PlayerWithinRange(CurrentPlayer, victim))
                        Attacks.Damage(victim, card.Type);
                    break;
                case PlayCard.Gatling:
                    Attacks.Damage(victim, card.Type);
                    break;
                case PlayCard.Duel:
                    Attacks.Damage(victim, card.Type);
                    break;
                case PlayCard.Indians:
                    Attacks.Damage(Players.Where(x => x.Health > 0).ToList(), card.Type);
                    break;
                case PlayCard.WellsFargo:
                    CurrentPlayer.Hand.AddRange(PlayDeck.Draw(3)); ;
                    break;
                case PlayCard.Diligenza:
                    CurrentPlayer.Hand.AddRange(PlayDeck.Draw(2));
                    break;
                case PlayCard.Saloon:
                    Players.ForEach(x => x.IncreaseHeaht());
                    break;
            }
            CurrentPlayer.Hand.Remove(card);
            PlayDeck.CardToPile(card);
        }

        public void NextTurn()
        {
            OnNewRound?.Invoke(this, EventArgs.Empty);
            CurrPlayerIndex = NextIndex;
            CurrentPlayer.Hand.AddRange(PlayDeck.Draw(2));
            StartTurnEvaluateCards();
        }

        private void StartTurnEvaluateCards()
        {
            var dynamite = CurrentPlayer.CardsOnTable.Find(x => x.Type == PlayCard.Dynamite);
            if (dynamite != null)
            {
                var drawn = PlayDeck.DrawForEffect();
                if (drawn.Color == CardColor.spade && (new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" }).Contains(drawn.Num))
                {
                    CurrentPlayer.TakeDamage(3);
                    CurrentPlayer.CardsOnTable.Remove(dynamite);
                    PlayDeck.CardToPile(dynamite);
                    if (CurrentPlayer.Health != 0)
                    {
                        NextTurn();
                        return;
                    }
                }
                else
                {
                    Players[NextIndex].CardsOnTable.Add(dynamite);
                }
            }
            var prison = CurrentPlayer.CardsOnTable.Find(x => x.Type == PlayCard.Prison);
            if (prison != null)
            {
                CurrentPlayer.CardsOnTable.Remove(prison);
                PlayDeck.CardToPile(prison);
                var drawn = PlayDeck.DrawForEffect();
                if (drawn.Color == CardColor.heart)
                {
                    NextTurn();
                    return;
                }
            }
        }

        public void ApplyCardToCard(Card myCard, Player victim, Card victimCard)
        {
            if (Distance(CurrentPlayer, victim) > CurrentPlayer.SeeingDistance)
            {
                OnUnableToPlayCard?.Invoke(this, EventArgs.Empty);
                return;
            }
            OnTakeCardAwai?.Invoke(this, EventArgs.Empty);
            if (myCard.Type == PlayCard.CatBalou)
            {
                DiscardCard(victim, victimCard.Id);
            }
            if (myCard.Type == PlayCard.Panic)
            {
                CurrentPlayer.Hand.Add(victimCard);
                if (victim.Hand.Contains(victimCard))
                    victim.Hand.Remove(victimCard);
                else
                    victim.CardsOnTable.Remove(victimCard);

            }
        }

        public bool BlockAttackByBarel(Player victim)
        {
            if(PlayDeck.DrawForEffect().Color == CardColor.heart)
            {
                OnBlock(this, EventArgs.Empty);
                Attacks.FreeBlock(victim);
                return true;
            }
            return false;
        }

        public void BlockAttackByCard(Player victim)
        {
            var temp = Attacks.Block(victim);
            if (temp != null)
            {
                OnBlock?.Invoke(this, EventArgs.Empty);
                PlayDeck.CardToPile(temp);
            }
        }

        private List<int> GenerateRandomizedIndexList(int startIndex, int endIndex)
        {
            Random rng = new Random();
            var list = Enumerable.Range(startIndex, endIndex).ToList();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        //debug functons 
        public override string ToString()
        {
            string stats = "";
            Players.ForEach(x => stats += "\n" + x.ToString() + Distances(x));
            return stats;
        }

        public string Distances(Player p)
        {
            string temp = "";
            Players.ForEach(x => temp += x.Name + " " + Distance(x, p) + " attackable : " + PlayerWithinRange(x,p) +  "\n");
            return temp;
        }

        private void PlayerDeath(Object p, EventArgs e) => OnDeath?.Invoke(p, e);
        private void PlayerHealthChanged(Object p, EventArgs e) => OnHealthChange?.Invoke(p, e);

        private void CardAddedToPile(Object p, EventArgs e) => OnCardAddedToPile?.Invoke(p, e);

        private void CardDrawn(Object p, EventArgs e) => OnCardDrawn?.Invoke(p, e);

        private void PilesMixed(Object p, EventArgs e) => OnMixedPiles?.Invoke(p, e);
        
    }

    
}
