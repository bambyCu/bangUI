using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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
        public bool canPlayBang = true; 

        public Game(List<string> names)
        {
            Names = names;
            Players = new List<Player>();
            lock (CardInfo.AvailableCards)
            {
                PlayDeck = new Deck(CardInfo.AvailableCards);
            }
            var random = new Random();
            var gameRoles = new List<Role>(CardInfo.GameSetup(names.Count));
            var heroRoles = new List<Player>(CardInfo.HeroVals);
            foreach (var i in names)
            {
                var role = gameRoles[random.Next(gameRoles.Count)];
                gameRoles.Remove(role);
                var hero = heroRoles[random.Next(gameRoles.Count)];
                heroRoles.Remove(hero);
                Players.Add(
                    new Player(i, hero.HeroType, role, hero.DistanceFromOthers, hero.SeeingDistance, hero.SeeingAttackDistance, hero.MaxHealth)
                    );
            }
            Players.ForEach(x => x.Hand.AddRange(PlayDeck.Draw(x.MaxHealth)));
            //start of turn
            CurrentPlayer.Hand.AddRange(PlayDeck.Draw(2));
        }

        public Card DrawForEffect()
        {
            var card = PlayDeck.Draw();
            PlayDeck.CardToPile(card);
            return card;
        } 

        public int AttackDistance(Player distanceStart, Player distanceEnd)
        {
            var distance = 0;
            var found = false;
            for(int i = 0; ; i++)
            {
                if (Players[i] == distanceStart || Players[i] == distanceEnd)
                {
                    if (found)
                        return distance + 
                            distanceEnd.DistanceFromOthers - 
                            distanceStart.SeeingAttackDistance - 
                            distanceStart.SeeingDistance;
                    found = true;
                    distance++;
                }
            }
        }
        public void NewRound()
        {
            CurrTurn = (1 + CurrTurn) % Players.Count;
        }
        public void PassCardOnTableToNext(PlayCard cardType)
        {
            var card = CurrentPlayer.CardsOnTable.Find(x => x.Type == cardType);
            if (card == null)
                return;
            CurrentPlayer.CardsOnTable.Remove(card);
            NextPlayer().CardsOnTable.Add(card);
        }
        public Player NextPlayer()
        {
            return Players[(CurrTurn + 1) % Players.Count]; 
        }
        public Player CurrentPlayer
        {
            get => Players[CurrTurn];
        }
        public void DiscardCard(string player, int cardsId)
        {
            var playerInstance = Players.Find(x => x.Name == player);
            DiscardCard(playerInstance, cardsId);
        }
        public void DiscardCard(Player player, int cardsId)
        {
            var card = player.Hand.Find(x => x.Id == cardsId);
            if (card == null)
            {
                card = player.CardsOnTable.Find(x => x.Id == cardsId);
                if (card == null) 
                    throw (new Exception("player does not own the card "));
                player.CardsOnTable.Remove(card);
            }
            else
            {
                player.Hand.Remove(card);
            }
            PlayDeck.CardToPile(card);
        }
        public void DiscardCards(string player, int[] cardsId)
        {
            DiscardCards(player, cardsId);
        }

        public void DiscardCards(Player player, int[] cardsId)
        {
            foreach(var i in cardsId)
            {
                DiscardCard(player, i);
            }
        }

        public Role? IsGameFinshed()
        {
            if (Players.All(x => x.RoleType == Role.Outlaw)) 
                return Role.Outlaw;
            if (Players.All(x => x.RoleType == Role.Sherif || x.RoleType == Role.Deputy))
                return Role.Sherif;
            if (Players.All(x => x.RoleType == Role.Renegate))
                return Role.Renegate;
            return null;
        }

        public void ApplyCard(Card card, Player victim)
        {
            if (card.Type == PlayCard.Missed)
            {
                //apInstance.Hand.Remove(cardInstance);
                //PlayDeck.CardToPile(cardInstance);
                return;
            }
            else if (CardInfo.IsSelfApplyCard(card))
            {
                var tempCard = CurrentPlayer.ApplySelfCard(card);
                CurrentPlayer.Hand.Remove(card);
                PlayDeck.CardToPile(tempCard);
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
                case PlayCard.WellsFargo:
                    CurrentPlayer.Hand.AddRange(PlayDeck.Draw(3)); ;
                    break;
                case PlayCard.Diligenza:
                    CurrentPlayer.Hand.AddRange(PlayDeck.Draw(2));
                    break;
                case PlayCard.Bang:
                    /*if ((true , true) != ( canPlayBang, AttackDistance(CurrentPlayer, victim) <= 0))
                        return;
                    if (victim.CardsOnTable.Any(x => x.Type == PlayCard.Barel) &&
                        DrawForEffect().Color == CardColor.heart)
                    {
                        CurrentPlayer.Hand.Remove(card);
                        PlayDeck.CardToPile(card);
                        return;
                    }
                    
                    canPlayBang = false || CurrentPlayer.CardsOnTable.Any(x => x.Type == PlayCard.Volcanic);
                    var discartedMiss = victim.TakeBangDamage();
                    PlayDeck.CardToPile(discartedMiss);*/
                    break;
                case PlayCard.Gatling:
                    /*foreach (var i in Players)
                    {
                        if (i == CurrentPlayer())
                        {
                            continue;
                        }
                        var discartedMiss = i.TakeBangDamage();
                        PlayDeck.CardToPile(discartedMiss);
                    }*/
                    break;
                case PlayCard.Duel:
                    /*
                     Card temp;
                    while (true)
                    {
                        temp = victim.TakeIndiandDamage();
                        PlayDeck.CardToPile(temp);
                        if (temp == null)
                        {
                            LastMessage = applicator + " applied " + card.Type + " to "
                                + victim + ", " + victim + "won";
                            break;
                        }
                        temp = apInstance.TakeIndiandDamage();
                        PlayDeck.CardToPile(temp);
                        if (temp == null)
                        {
                            LastMessage = applicator + " applied " + card.Type + " to "
                                + victim + ", " + applicator + "won";
                            break;
                        }
                    }
                    */
                    break;
                case PlayCard.Indians:
                    /*
                     foreach (var i in Players)
                    {
                        if (i == CurrentPlayer())
                        {
                            continue;
                        }
                        LastMessage = applicator + " applied " + card.Type;
                        var discartedMiss = i.TakeIndiandDamage();
                        PlayDeck.CardToPile(discartedMiss);
                    }
                     */
                    break;
            }
            CurrentPlayer.Hand.Remove(card);
            PlayDeck.CardToPile(card);
        }
        //this is not exactly necessary but may be needed for game extentions



    }
}
