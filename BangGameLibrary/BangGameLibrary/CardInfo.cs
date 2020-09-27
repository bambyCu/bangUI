using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BangGameLibrary
{
    internal static class CardInfo
    {
        private static List<Card> availableCards;
        public static List<Card> AvailableCards 
        {
            get
            {
                if (availableCards == null)
                {
                    //(StreamReader sr = new StreamReader(Path.Combine(AppContext.BaseDirectory, "") + @"Models\Decks\PlayingCards.txt"))
                    string line = Properties.Resources.PlayingCards;
                    availableCards = JsonConvert.DeserializeObject<Card[]>(line).ToList();
                    for (int i = 0; i < availableCards.Count; i++)
                        availableCards[i].Id = i;
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
                    string line = Properties.Resources.HeroCards;
                    heroVals =
                        JsonConvert.DeserializeAnonymousType(line, new[] { new { Hero = "", DistanceFromOthers = "", SeeingDistance = "", SeeingAttackDistance = "", MaxHealth = "" } })
                            .Select(x => new Player(x.Hero, Int32.Parse(x.DistanceFromOthers), Int32.Parse(x.SeeingDistance), Int32.Parse(x.SeeingAttackDistance), Int32.Parse(x.MaxHealth)))
                            .ToList();
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

        public readonly static Dictionary<PlayCard, int> GunDistanceMap = new Dictionary<PlayCard, int>
        {
            {PlayCard.Remington,2},
            {PlayCard.Carabine,3},
            {PlayCard.Schofield,1},
            {PlayCard.Volcanic,0},
            {PlayCard.Winchester,4}
        };

        public static Dictionary<PlayCard, PlayCard> AttackCardsToRemedies = new Dictionary<PlayCard, PlayCard>()
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

        public static List<Role> GameSetup(int numberOfPlayers)
        {
            switch (numberOfPlayers) {
                case 1:
                    return new List<Role>() { Role.Sherif };
                case 2:
                    return new List<Role>() { Role.Sherif, Role.Outlaw };
                case 3:
                    return new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw };
                case 4:
                    return new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw, Role.Outlaw };
                case 5:
                    return new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw, Role.Outlaw, Role.Deputy };
                case 6:
                    return new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw, Role.Outlaw, Role.Outlaw, Role.Deputy };
                case 7:
                    return new List<Role>() { Role.Sherif, Role.Renegate, Role.Outlaw, Role.Outlaw, Role.Outlaw, Role.Deputy, Role.Deputy };
            }
            throw (new Exception("unsuported number of players"));
        }

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
