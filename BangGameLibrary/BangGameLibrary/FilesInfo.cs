using BangGameLibrary;
using System;
using System.IO;
using System.Reflection;

namespace BangGameLibrary
{
    internal static class FilesInfo
    {
        private readonly static string workingDirectory = Environment.CurrentDirectory;
        private readonly static string  Root = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        public readonly static string BasicPlayCards = Root + @"\Decks\PlayingCards.txt";
        public readonly static string BasicHeroes = Root + @"\Decks\HeroCards.txt";    
    }
}