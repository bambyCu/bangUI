using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services.Description;

namespace BangUI.Models
{
    public static class FilesInfo
    {
        private static string Root = Path.Combine(AppContext.BaseDirectory, "");
        public readonly static string BasicPlayCards = Root + @"Models/Decks/PlayCards.txt";
        public readonly static string BasicHeroes = Root + @"Models/Decks/PlayCards.txt";

    }
}