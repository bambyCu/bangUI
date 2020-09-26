using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BangGameLibrary
{
    internal class AttackManager
    {
        public Dictionary<int,List<(Player,PlayCard)>> WaitingVictims = new Dictionary<int, List<(Player, PlayCard)>>();
        private int CurrentAttackNum = 0;
        public bool ReadyToAttack { get; private set; }
        private int WaitTime = 3000;

        public AttackManager()
        {
            ReadyToAttack = true;
        }
        public bool Damage(Player victim, PlayCard card) => Damage((new Player[] { victim }).ToList(), card);

        public bool Damage(List<Player> victims, PlayCard card)
        {
            if (!ReadyToAttack)
                return false;
            new Thread(() => {
                ReadyToAttack = false;
                CurrentAttackNum++;
                WaitingVictims.Add(CurrentAttackNum, new List<(Player, PlayCard)>());
                WaitingVictims[CurrentAttackNum].AddRange(victims.Select(victim => (victim, card)));
                var t = CurrentAttackNum;
                Thread.Sleep(WaitTime);
                if (t != CurrentAttackNum)
                    return;
                WaitingVictims[t].ForEach(x => x.Item1.TakeDamage());
                //WaitingVictims[t].ForEach(x => Console.WriteLine("-----------------------------------------"));
                WaitingVictims.Remove(t);
                ReadyToAttack = t == CurrentAttackNum;
            }).Start();
            return true;
        }
        public Card Block(Player victim)
        {
            lock (WaitingVictims)
            {
                if (!WaitingVictims[CurrentAttackNum].Any(x => x.Item1 == victim))
                    return null;
                var attackCardType = WaitingVictims[CurrentAttackNum].Find(x => x.Item1 == victim).Item2;
                var remedyInHand = victim.Hand.Find(x => x.Type == CardInfo.AttackCardsToRemedies[attackCardType]);
                if (remedyInHand != null)
                {
                    victim.Hand.Remove(remedyInHand);
                    WaitingVictims[CurrentAttackNum].RemoveAll(x => x.Item1 == victim);
                    ReadyToAttack = !WaitingVictims[CurrentAttackNum].Any();
                    return remedyInHand;
                }
            }
            return null;
        }

        public bool FreeBlock(Player victim)
        {
            lock (WaitingVictims)
            {
                if (WaitingVictims[CurrentAttackNum].Any(x => x.Item1 == victim))
                {
                    WaitingVictims[CurrentAttackNum].RemoveAll(x => x.Item1 == victim);
                    ReadyToAttack = !WaitingVictims[CurrentAttackNum].Any();
                    return true;
                }
            }
            return false;
        }
       
    }
}