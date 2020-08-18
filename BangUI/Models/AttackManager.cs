using BangGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace BangUI.Models
{
    public class AttackManager
    {
        public Dictionary<string, Thread> ThreadHolder;
        public PlayCard AttackCard { get; set; }
        public Player Attacker;
        private int waitTime = 1000;
        public List<Player> Victims;
        private GameWrapper master;
        public AttackManager(GameWrapper gameWrapper)
        {
            master = gameWrapper;
        }
        public bool IsAttackFinished { get => !ThreadHolder.Any(); }

        public bool Attack(Player attacker, PlayCard cardType, Player victim)
        {
            if (!IsAttackFinished)
                return false;
            Attacker = attacker;
            AttackCard = cardType;
            Victims.Add(victim);
            ThreadHolder[victim.Name]= new Thread(() => TakeDamage(victim));
            IsAttackFinished = false;
            return true;
        }

        public bool Block(string victimName, PlayCard remedy)
        {
            //bang is not supported for now  
            if (CardInfo.AttackCardsToRemedies[AttackCard] == remedy)
            {
                ThreadHolder.Remove(victimName);
                return true;
            }
                
            return false;
        }

        private void TakeDamage(Player victim)
        {
            master.TakeDamage(victim, waitTime);
            ThreadHolder[victim.Name].Abort();
            ThreadHolder.Remove(victim.Name);
        }

    }
}