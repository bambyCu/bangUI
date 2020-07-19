using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BangGame
{
    public static class CardInfo
    {
        public static List<Card> AvailableCards;
        public static List<Player> HeroVals;

        static CardInfo()
        {
            using (StreamReader sr = new StreamReader(Path.Combine(AppContext.BaseDirectory, "") + @"Models\Decks\HeroCards.txt")) 
            {
                string line = sr.ReadToEnd();

                HeroVals =
                    JsonConvert.DeserializeAnonymousType(line, new[] { new { Hero = "", DistanceFromOthers = "", SeeingDistance = "", SeeingAttackDistance = "", MaxHealth = "" } })
                        .Select(x => new Player(x.Hero, Int32.Parse(x.DistanceFromOthers), Int32.Parse(x.SeeingDistance), Int32.Parse(x.SeeingAttackDistance), Int32.Parse(x.MaxHealth)))
                        .ToList();
            }

            using (StreamReader sr = new StreamReader(Path.Combine(AppContext.BaseDirectory, "") + @"Models\Decks\PlayingCards.txt"))
            {
                string line = sr.ReadToEnd();

                AvailableCards = JsonConvert.DeserializeObject<Card[]>(line).ToList();
            }
        }
        //CARDS FOR CHANGING ATTACK DISTANCE AND BLUE CARDS 
        private readonly static List<PlayCard> Guns = new List<PlayCard>
        {
            PlayCard.Remington,
            PlayCard.Carabine,
            PlayCard.Schofield,
            PlayCard.Volcanic,
            PlayCard.Winchester
        };

        //REST OF BLUE CARDS 
        private readonly static List<PlayCard> SpecialBlueCards = new List<PlayCard>
        {
            PlayCard.Barel,
            PlayCard.Dynamite,
            PlayCard.Mirino,
            PlayCard.Mustang,
            PlayCard.Prison
        };

        public readonly static Dictionary<PlayCard, int> GunDistanceMap = new Dictionary<PlayCard, int>
        {
            {PlayCard.Remington,2},
            {PlayCard.Carabine,3},
            {PlayCard.Schofield,1},
            {PlayCard.Volcanic,0},
            {PlayCard.Winchester,4}
        };

        private readonly static List<PlayCard> SelfApplyCards = Guns.
                                                        Concat(SpecialBlueCards).
                                                        Append(PlayCard.Beer).
                                                        ToList();


        private readonly static List<PlayCard> SpecialCases = new List<PlayCard> {
            PlayCard.Missed,
            PlayCard.Prison};

        private readonly static List<PlayCard> OnlyCardToCard = new List<PlayCard> {
            PlayCard.CatBalou,
            PlayCard.Panic};

        private readonly static List<PlayCard> AttackCards = new List<PlayCard> {
            PlayCard.Bang,
            PlayCard.Gatling,
            PlayCard.Duel,
            PlayCard.Emporio,
            PlayCard.Indians};

        public static Dictionary<int, List<Role>> GameRoles = new Dictionary<int, List<Role>>()
        {
            { 1, new List<Role>() { Role.Sherif} },
            { 2, new List<Role>() { Role.Sherif, Role.Outlaw} },
            { 3, new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw} },
            { 4, new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw, Role.Outlaw} },
            { 5, new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw, Role.Outlaw, Role.Deputy} },
            { 6, new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw, Role.Outlaw, Role.Outlaw, Role.Deputy} },
            { 7, new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw, Role.Outlaw, Role.Outlaw, Role.Deputy, Role.Deputy } }
        };

        public static Dictionary<PlayCard, PlayCard> Remedies = new Dictionary<PlayCard, PlayCard>()
        {
            { PlayCard.Bang, PlayCard.Missed},
            { PlayCard.Duel, PlayCard.Bang},
            { PlayCard.Gatling, PlayCard.Missed},
            { PlayCard.Indians, PlayCard.Bang}
        };



        public static bool IsCardBlue(Card c)
        {
            return Guns.Contains(c.Type) || SpecialBlueCards.Contains(c.Type);
        }

        public static bool IsSelfApplyCard(Card c)
        {
            return SelfApplyCards.Any(d => d == c.Type);
        }

        public static bool IsCardYellow(Card c)
        {
            return !IsCardBlue(c);
        }

        public static bool IsCardGun(Card c)
        {
            return Guns.Contains(c.Type);
        }
        public static bool IsOnlyCardToCard(Card c)
        {
            return OnlyCardToCard.Contains(c.Type);
        }
        public static bool IsAttackCard(Card p)
        {
            return AttackCards.Contains(p.Type);
        }

        public static int GunDistance(PlayCard p)
        {
            return GunDistanceMap[p];
        }

        

    }
}
