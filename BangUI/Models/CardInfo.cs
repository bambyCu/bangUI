using BangUI.Models;
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
        private static List<Card> availableCards;
        public static List<Card> AvailableCards 
        {
            get
            {
                if (availableCards == null)
                {
                    //(StreamReader sr = new StreamReader(Path.Combine(AppContext.BaseDirectory, "") + @"Models\Decks\PlayingCards.txt"))
                    using (StreamReader sr = new StreamReader(FilesInfo.BasicPlayCards))
                    {
                        string line = sr.ReadToEnd();
                        availableCards = JsonConvert.DeserializeObject<Card[]>(line).ToList();
                    }
                }
                return availableCards;
            }    
        }
        private static List<Player> heroVals;
        public static List<Player> HeroVals
        {
            get
            {
                if(heroVals == null)
                {
                    using (StreamReader sr = new StreamReader(FilesInfo.BasicHeroes))
                    {
                        string line = sr.ReadToEnd();
                        heroVals =
                            JsonConvert.DeserializeAnonymousType(line, new[] { new { Hero = "", DistanceFromOthers = "", SeeingDistance = "", SeeingAttackDistance = "", MaxHealth = "" } })
                                .Select(x => new Player(x.Hero, Int32.Parse(x.DistanceFromOthers), Int32.Parse(x.SeeingDistance), Int32.Parse(x.SeeingAttackDistance), Int32.Parse(x.MaxHealth)))
                                .ToList();
                    }
                }
                return heroVals;
            }
        }


        private static List<PlayCard> SpecialBlueCards = new List<PlayCard>
        {
            PlayCard.Barel,
            PlayCard.Mirino,
            PlayCard.Mustang,
        };

        private readonly static Dictionary<PlayCard, int> GunDistanceMap = new Dictionary<PlayCard, int>
        {
            {PlayCard.Remington,2},
            {PlayCard.Carabine,3},
            {PlayCard.Schofield,1},
            {PlayCard.Volcanic,0},
            {PlayCard.Winchester,4}
        };

        private static Dictionary<PlayCard, PlayCard> AttackCardsToRemedies = new Dictionary<PlayCard, PlayCard>()
        {
            { PlayCard.Bang, PlayCard.Missed},
            { PlayCard.Duel, PlayCard.Bang},
            { PlayCard.Gatling, PlayCard.Missed},
            { PlayCard.Indians, PlayCard.Bang}
        };


        private readonly static List<PlayCard> SelfApplyCards = GunDistanceMap.Keys.
                                                        Concat(SpecialBlueCards).
                                                        Append(PlayCard.Beer).
                                                        ToList();

        private readonly static List<PlayCard> OnlyCardToCard = new List<PlayCard> {
            PlayCard.CatBalou,
            PlayCard.Panic};

        public static List<Role> GameSetup(int numberOfPlayers) =>
            numberOfPlayers switch
        {
            1 => new List<Role>() { Role.Sherif},
            2 => new List<Role>() { Role.Sherif, Role.Outlaw} ,
            3 => new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw} ,
            4 => new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw, Role.Outlaw} ,
            5 => new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw, Role.Outlaw, Role.Deputy} ,
            6 => new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw, Role.Outlaw, Role.Outlaw, Role.Deputy} ,
            7 => new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw, Role.Outlaw, Role.Outlaw, Role.Deputy, Role.Deputy },
            _ => throw (new Exception("unsuported number of players"))
        };

        public static bool IsCardBlue(Card c)
        {
            return GunDistanceMap.ContainsKey(c.Type) || SpecialBlueCards.Contains(c.Type);
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
            return GunDistanceMap.ContainsKey(c.Type);
        }
        public static bool IsOnlyCardToCard(Card c)
        {
            return OnlyCardToCard.Contains(c.Type);
        }
        public static bool IsAttackCard(Card p)
        {
            return AttackCardsToRemedies.ContainsKey(p.Type);
        }

        public static int GunDistance(PlayCard p)
        {
            return GunDistanceMap[p];
        }

        

    }
}
