﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BangGame
{
    public static class CardInfo
    {
        //CARDS FOR CHANGING ATTACK DISTANCE AND BLUE CARDS 
        private static List<PlayCard> Guns = new List<PlayCard>  
        {
            PlayCard.Remington,
            PlayCard.Carabine,
            PlayCard.Schofield,
            PlayCard.Volcanic,
            PlayCard.Winchester
        };

        //REST OF BLUE CARDS 
        private static List<PlayCard> SpecialBlueCards = new List<PlayCard> 
        {
            PlayCard.Barel,
            PlayCard.Dynamite,
            PlayCard.Mirino,
            PlayCard.Mustang,
            PlayCard.Prison
        };

        private static Dictionary<PlayCard, int> GunDistanceMap = new Dictionary<PlayCard, int> 
        {
            {PlayCard.Remington,2},
            {PlayCard.Carabine,3},
            {PlayCard.Schofield,1},
            {PlayCard.Volcanic,0},
            {PlayCard.Winchester,4}
        };

        private static List<PlayCard> SelfApplyCards =  Guns.
                                                        Concat(SpecialBlueCards).
                                                        Append(PlayCard.Beer).
                                                        ToList();


        private static List<PlayCard> SpecialCases = new List<PlayCard> {
            PlayCard.Missed,
            PlayCard.Prison};

        private static List<PlayCard> OnlyCardToCard = new List<PlayCard> {
            PlayCard.CatBalou,
            PlayCard.Panic};

        private static List<PlayCard> CardWithReactionFromOthers = new List<PlayCard> {
            PlayCard.Bang,
            PlayCard.Gatling,
            PlayCard.Duel,
            PlayCard.Emporio,
            PlayCard.Indians};

        

        public static bool IsCardBlue(Card c)
        {
            return Guns.Contains(c.Type) || SpecialBlueCards.Contains(c.Type);
        }

        public static bool IsSelfApplyCard(Card c)
        {
            return SelfApplyCards.Any(d => d == c.Type);
        }

        public static bool isCardYellow(Card c)
        {
            return !IsCardBlue(c);
        }

        public static bool isCardGun(Card c)
        {
            return Guns.Contains(c.Type);
        }
        
        public static int GunDistance(PlayCard p)
        {
            return GunDistanceMap[p];

        }
        
    }
}