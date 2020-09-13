using Antlr.Runtime.Misc;
using BangGame;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web;

namespace BangUI.Models
{
    public class GameWrapper
    {
        private Hub IOHub;
        private Game BangGame;
        private AttackManager Attacks;
        public Player CurrentPlayer => BangGame.CurrentPlayer;
        public GameWrapper(Hub IOHub, Game BangGame) =>
            (this.IOHub, this.BangGame) = (IOHub, BangGame);
        public List<Player> Players
        {
            get => BangGame.Players;
        }

        private void StartPhaseOne()
        {
            CurrentPlayer.Hand.AddRange(BangGame.PlayDeck.Draw(2));
        }

        private bool DrawForEffect(System.Func<Card, bool> funct) =>
            funct(BangGame.DrawForEffect());
        public void DiscardCard(string player, int id) =>
            BangGame.DiscardCard(player, id);

        public void nextTurn()
        {
            BangGame.NewRound();
            if (CurrentPlayer.HasOnTable(PlayCard.Dynamite) &&  
                DrawForEffect( x =>(x.Color, true) == (CardColor.spade, int.TryParse(x.Num, out _))))
            {
                CurrentPlayer.TakeDamage(3);
                if (!CurrentPlayer.IsAlive)
                {
                    nextTurn();
                    //give all cards to vulcher
                    return;
                }
            }
            if (CurrentPlayer.HasOnTable(PlayCard.Prison) &&
                DrawForEffect(x => x.Color != CardColor.heart))
            {
                nextTurn();
                return;
            }
            StartPhaseOne();
        }
        

        public void applyCard(string applicator, int card, string victim)
        {
            var apInstance = BangGame.Players.Find(x => x.Name == applicator);
            var vicInstance = BangGame.Players.Find(x => x.Name == victim);
            var cardInstance = apInstance.Hand.Find(x => x.Id == card);
            if (apInstance == CurrentPlayer)
                applyCard(apInstance, cardInstance, vicInstance);
        }
        public void applyCard(Player attacker, Card card, Player victim)
        {
            if (!victim.IsAlive || !attacker.IsAlive)
                //TODO
                return;
            //TODO
            //if(card.Type == Miss)
            if ( !CardInfo.IsAttackCard(card))
            {
                BangGame.applyCard(card, victim);
            }
            switch (card.Type)
            {
                case PlayCard.Bang:
                    if ((true , true) != ( BangGame.canPlayBang, BangGame.AttackDistance(CurrentPlayer, victim) <= 0))
                        Attacks.Attack(attacker, card.Type, victim);
                    break;
                case PlayCard.Gatling:
                    Attacks.Attack(attacker, card.Type, victim);
                    break;
                case PlayCard.Duel:
                    Attacks.Attack(attacker, card.Type, victim);
                    break;
                case PlayCard.Indians:
                    Attacks.Attack(attacker, card.Type, victim);
                    break;
            }
            attacker.Hand.Remove(card);
            BangGame.PlayDeck.CardToPile(card);
        }
        //this is not exactly necessary but may be needed for game extentions

        internal void TakeDamage(Player victim, int waitTime)
        {
            Thread.Sleep(waitTime);
            //IOHub sends message
            lock (victim)
            {
                victim.TakeDamage();
            }
        }

        public Player getPlayer(string name) =>
            BangGame.Players.Find(x => x.Name == name);

    }
}