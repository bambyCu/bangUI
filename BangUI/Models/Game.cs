using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace BangGame
{
    public class Game
    {
        public Deck PlayDeck;
        public List<Player> Players;
        public List<string> Names;
        private int CurrTurn = 0;
        public string LastMessage = "";
        private bool canPlayBang = true; 

        public Game(List<string> names)
        {
            Names = names;
            lock (CardInfo.AvailableCards)
            {
                PlayDeck = new Deck(CardInfo.AvailableCards);
            }
            if (!CardInfo.GameRoles.ContainsKey(names.Count))
            {
                //TO DO if game has wrong amount of players
            }
            var random = new Random();
            var gameRoles = new List<Role>(CardInfo.GameRoles[names.Count]);
            var heroRoles = new List<Player>(CardInfo.HeroVals);
            Players = new List<Player>();
            foreach (var i in names)
            {
                int randVal = random.Next(gameRoles.Count);
                var role = gameRoles[randVal];
                gameRoles.RemoveAt(randVal);
                randVal = random.Next(heroRoles.Count);
                var hero = heroRoles[randVal];
                heroRoles.RemoveAt(randVal);
                var nPlayer = new Player(i, hero.HeroType, role, hero.DistanceFromOthers, hero.SeeingDistance, hero.SeeingAttackDistance, hero.MaxHealth);
                Players.Add(nPlayer);
                for(int j = 0; j < nPlayer.MaxHealth;j++)
                {
                    nPlayer.Hand.Add(PlayDeck.Draw());
                }
            }
            GetCurrentTurnPlayer().Hand.Add(PlayDeck.Draw());
            GetCurrentTurnPlayer().Hand.Add(PlayDeck.Draw());
        }

        private Card DrawForEffect()
        {
            var card = PlayDeck.Draw();
            PlayDeck.CardToPile(card);
            return card;
        } 
        private int Distance(Player distanceStart, Player distanceEnd)
        {
            var startInex = Players.FindIndex(x => distanceStart == x);
            var endInex = Players.FindIndex(x => distanceEnd == x);
            var distance = endInex - startInex;
            return (distance < 0) ? -distance : distance;
        }

        public Player GetCurrentTurnPlayer()
        {
            return Players[CurrTurn];
        }
        public void DiscardCard(string player, string cardsId)
        {
            var playerInstance = Players.Find(x => x.Name == player);
            DiscardCard(playerInstance, cardsId);
        }
        public void DiscardCard(Player player, string cardsId)
        {
            var card = player.Hand.Find(x => x.ID() == cardsId);
            if (card == null)
            {
                card = player.CardsOnTable.Find(x => x.ID() == cardsId);
                if (card == null) { return; }
                player.CardsOnTable.Remove(card);
            }
            else
            {
                player.Hand.Remove(card);
            }
            PlayDeck.CardToPile(card);
        }
        public void DiscardCards(string player, string[] cardsId)
        {
            DiscardCards(player, cardsId);
        }

        public void DiscardCards(Player player, string[] cardsId)
        {
            foreach(var i in cardsId)
            {
                DiscardCard(player, i);
            }
        }

        public Role? IsGameFinshed()
        {
            if (Players.All(x => x.RoleType == Role.Outlaw)) 
            {
                return Role.Outlaw;
            }
            if (Players.All(x => x.RoleType == Role.Sherif || x.RoleType == Role.Deputy))
            {
                return Role.Sherif;
            }

            if (Players.All(x => x.RoleType == Role.Renegate))
            {
                return Role.Renegate;
            }
            return null;
        }

        public Player NextTurn()
        {
            CurrTurn = (CurrTurn + 1) % Players.Count;
            Card temp;
            if ((temp = GetCurrentTurnPlayer().CardsOnTable.Find(x => x.Type == PlayCard.Dynamite)) != null)
            {
                var cardVal = DrawForEffect();
                var num = (cardVal.Num == "A") ? 1 :
                    (cardVal.Num == "K") ? 13 :
                    (cardVal.Num == "A") ? 12 : 
                    (cardVal.Num == "A") ? 11 : Int32.Parse(cardVal.Num);
                if (cardVal.Color == CardColor.spade && (num <= 9 && num >= 2))
                {
                    GetCurrentTurnPlayer().Health -= 3;
                    evaluateDead();
                    PlayDeck.CardToPile(temp);
                    return NextTurn();
                }
                else
                {
                    Players[(CurrTurn + 1) % Players.Count].CardsOnTable.Add(temp);
                    GetCurrentTurnPlayer().CardsOnTable.Remove(temp);
                }
            }
            if ((temp = GetCurrentTurnPlayer().CardsOnTable.Find(x => x.Type == PlayCard.Prison)) != null)
            {
                var cardVal = DrawForEffect();
                GetCurrentTurnPlayer().CardsOnTable.Remove(temp);
                PlayDeck.CardToPile(temp);
                if (cardVal.Color == CardColor.heart)
                {
                    GetCurrentTurnPlayer().CardsOnTable.Remove(temp);
                    PlayDeck.CardToPile(temp);
                    return NextTurn();
                }
                
            }
            canPlayBang = true;
            Players[CurrTurn].Hand.Add(PlayDeck.Draw());
            Players[CurrTurn].Hand.Add(PlayDeck.Draw());
            return Players[CurrTurn];
        }

        private void evaluateDead()
        {
            Players = Players.Where(x => x.Health > 0).ToList();
        }

        public void applyCard(string  applicator, string card, string victim)
        {
            LastMessage = "";
            var apInstance = Players.Find(x => x.Name == applicator);
            var cardInstance = apInstance.Hand.Find(x => x.ID() == card);
            var vicInstance = Players.Find(x => x.Name == victim);
            if (cardInstance.Type == PlayCard.Missed)
            {
                //apInstance.Hand.Remove(cardInstance);
                //PlayDeck.CardToPile(cardInstance);
                return;
            }
            else if (CardInfo.IsSelfApplyCard(cardInstance))
            {
                LastMessage = applicator + " applied " + cardInstance.Type;
                var tempCard = apInstance.ApplySelfCard(cardInstance);
                apInstance.Hand.Remove(cardInstance);
                PlayDeck.CardToPile(tempCard);
                return;
            }
            if (cardInstance.Type == PlayCard.Prison || PlayCard.Dynamite == cardInstance.Type)
            {
                LastMessage = applicator + " applied " + cardInstance.Type + " to " + victim;
                vicInstance.CardsOnTable.Add(cardInstance);
            }
            else if (cardInstance.Type == PlayCard.WellsFargo)
            {
                LastMessage = applicator + " applied " + cardInstance.Type;
                apInstance.Hand.Add(PlayDeck.Draw());
                apInstance.Hand.Add(PlayDeck.Draw());
                apInstance.Hand.Add(PlayDeck.Draw());
            }
            else if (PlayCard.Diligenza == cardInstance.Type)
            {
                LastMessage = applicator + " applied " + cardInstance.Type;
                apInstance.Hand.Add(PlayDeck.Draw());
                apInstance.Hand.Add(PlayDeck.Draw());
            }
            else if (CardInfo.IsAttackCard(cardInstance))
            {
                if (cardInstance.Type == PlayCard.Bang)
                {
                    if (!canPlayBang)
                    {
                        LastMessage = applicator + " cant play bang";
                        return;
                    }
                    if (vicInstance.CardsOnTable.Any(x => x.Type == PlayCard.Barel) &&
                        DrawForEffect().Color == CardColor.heart)
                    {
                        apInstance.Hand.Remove(cardInstance);
                        PlayDeck.CardToPile(cardInstance);
                        LastMessage = applicator + " applied " + cardInstance.Type + " to " + victim + "but Barel took it";
                        return;
                    }
                    if (Distance(vicInstance, apInstance) + vicInstance.DistanceFromOthers - apInstance.SeeingAttackDistance - apInstance.SeeingDistance > 0)
                    {
                        LastMessage = applicator + "is too far from " + victim + " for " + cardInstance.Type;
                        return;
                    }
                    canPlayBang = false || apInstance.CardsOnTable.Any(x => x.Type == PlayCard.Volcanic);
                    LastMessage = applicator + " applied " + cardInstance.Type + " to " + victim;
                    var discartedMiss = vicInstance.TakeBangDamage();
                    PlayDeck.CardToPile(discartedMiss);
                }
                if (cardInstance.Type == PlayCard.Gatling)
                {
                    LastMessage = applicator + " applied " + cardInstance.Type ;
                    foreach (var i in Players)
                    {
                        if (i == GetCurrentTurnPlayer())
                        {
                            continue;
                        }
                        var discartedMiss = i.TakeBangDamage();
                        PlayDeck.CardToPile(discartedMiss);
                    }
                }
                if (cardInstance.Type == PlayCard.Duel)
                {
                    Card temp;
                    while (true)
                    {
                        temp = vicInstance.TakeIndiandDamage();
                        PlayDeck.CardToPile(temp);
                        if (temp == null)
                        {
                            LastMessage = applicator + " applied " + cardInstance.Type + " to " 
                                + victim + ", " + victim + "won";
                            break;
                        }
                        temp = apInstance.TakeIndiandDamage();
                        PlayDeck.CardToPile(temp);
                        if (temp == null)
                        {
                            LastMessage = applicator + " applied " + cardInstance.Type + " to " 
                                + victim + ", " + applicator + "won";
                            break;
                        }
                    }
                }
                if (cardInstance.Type == PlayCard.Indians)
                {
                    foreach (var i in Players)
                    {
                        if (i == GetCurrentTurnPlayer()) 
                        {
                            continue;
                        }
                        LastMessage = applicator + " applied " + cardInstance.Type;
                        var discartedMiss = i.TakeIndiandDamage();
                        PlayDeck.CardToPile(discartedMiss);
                    }
                }
            }
            apInstance.Hand.Remove(cardInstance);
            PlayDeck.CardToPile(cardInstance);
            evaluateDead();
        }



        public PlayCard Attack(Player player, Card card)
        {
            return card.Type;
        }

        public void ApplyPlayerToPlayer(Player applier, Card c, Player victim)
        {
            
        }


    }
}
