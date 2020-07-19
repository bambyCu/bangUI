using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using BangGame;
using System.Drawing;
using System.IO;
using Microsoft.Ajax.Utilities;

namespace SignalRTutorial
{
    [HubName("mainHub")]
    public class MyHub : Hub
    {
        //Username -> ConnectionId
        private readonly static Dictionary<string, string> Connections =
            new Dictionary<string, string>();

        private readonly static HashSet<string> BussyUsers =
            new HashSet<string>();

        private readonly static SortedDictionary<string, Game> CurrGames =
            new SortedDictionary<string, Game>();

        //GameID -> (userName, InvitationAccepted)
        private static SortedDictionary<string, List<(string, bool)>> GroupApprove =
            new SortedDictionary<string, List<(string, bool)>>();


        private string Name()
        {
            lock (Connections)
            {
                return Connections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            }
        }


        private byte[] ImageTo64(string src)
        {
            var converter = new ImageConverter();
            var image = Image.FromFile(src);
            return (byte[])converter.ConvertTo(image, typeof(byte[]));
        }

        private string GameID()
        {
            lock (CurrGames)
            {
                return CurrGames.First(x => x.Value.Names.Contains(Name())).Key;
            }
        }

        private void SendHandCards(Player p)
        {
            var playCards = new List<(byte[], string)>();
            foreach (var j in p.Hand)
            {
                playCards.Add((ImageTo64(Path.Combine(AppContext.BaseDirectory, "") + @"Content\Images\PlayCards\" + j.Type.ToString().ToLower() + ".png"), j.ID()));
            }
            lock (Connections)
            {
                Clients.Client(Connections[p.Name]).AddHandCards(playCards.ToArray());
            }
        }

        private void UpdateGame(string gameId)
        {
            
            lock (CurrGames)
            {
                foreach (var i in CurrGames[gameId].Players)
                {
                    //Clients.Client(Connections[i.Name]).DisplayImage(ImageTo64(Path.Combine(AppContext.BaseDirectory, "") + @"Content\Images\Heroes\" + i.HeroType.ToString().ToLower() + ".png"));
                    lock (Connections)
                    {
                        Clients.Client(Connections[i.Name]).SetHealth(i.Health);
                        Clients.Client(Connections[i.Name]).SetRole(i.RoleType.ToString());
                    }
                    SendHandCards(i);
                    var enemyPlayers = new List<(string, int, string)>();
                    foreach (var j in CurrGames[gameId].Players)
                    {
                        enemyPlayers.Add((j.Name, j.Health, (j.RoleType == Role.Sherif) ? "Sherif" : "NotSherif"));
                    }
                    lock (Connections)
                    {
                        Clients.Client(Connections[i.Name]).AddEnemies(enemyPlayers);
                        Clients.Client(Connections[i.Name]).SetCurrPlayer(CurrGames[gameId].GetCurrentTurnPlayer().Name);
                    }
                }
            }
        }

        public bool LogIn(string userName)
        {
            lock (Connections)
            {
                if (Connections.Keys.Contains(userName)) { return false; }
                Connections.Add(userName, Context.ConnectionId);
            }
            Clients.All.LogIn(userName);
            return true;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (Name() == null) { return base.OnDisconnected(stopCalled); }
            Clients.All.Disconnect(Name());
            lock (Connections)
            {
                Connections.Remove(Name());
            }
            return base.OnDisconnected(stopCalled);
        }

        public bool ApplyGameIdCardTo(string gameId, string cardId, string target)
        {
            lock (CurrGames)
            {
                if (CurrGames[gameId].GetCurrentTurnPlayer().Name != Name())
                {
                    lock (Connections)
                    {
                        Clients.Client(Connections[Name()]).Message("it is not your turn");
                    }
                    return false;
                }
                if (!CurrGames.ContainsKey(gameId)) { return false; }
                if (!CurrGames[gameId].Names.Contains(target)) { return false; }
                CurrGames[gameId].applyCard(Name(), cardId, target);
                
                var mess = CurrGames[gameId].LastMessage;
                if (mess != "")
                {
                    lock (Connections)
                    {
                        Clients.Client(Connections[target]).addToMessageList(mess);
                    }
                }
            }
            UpdateGame(gameId);
            
            return true;
        }

        
        public void Discard(string cardId)
        {
            lock (CurrGames)
            {
                var game = CurrGames[GameID()];
                game.DiscardCard(Name(), cardId);
            }
        }
        public void EndTurn()
        {
            
            lock (CurrGames[GameID()])
            {
                var game = CurrGames[GameID()];

                var player = game.GetCurrentTurnPlayer();
                if (Name() != player.Name)
                {
                    lock (Connections)
                    {
                        Clients.Client(Connections[Name()]).Message("it is not your turn");
                    }
                    return;
                }
                if (player.Hand.Count > player.Health)
                {
                    Clients.Client(Connections[Name()]).Message("too many cards in hand");
                    return;
                }
                game.NextTurn();
                SendHandCards(game.GetCurrentTurnPlayer());
                foreach (var i in game.Names)
                {
                    Clients.Client(Connections[i]).SetCurrPlayer(game.GetCurrentTurnPlayer().Name);
                }
            }
        }
        public string[] GetUsers()
        {
            lock (Connections)
            {
                return Connections.Keys.ToArray();
            }
        }

        public bool GameInvitation(String[] users)
        {
            if (!users.Any()) 
            { 
                return false; 
            }
            lock (BussyUsers)
            {
                var bussyUsers = BussyUsers.Where(x => users.Contains(x));
                if (bussyUsers.Any())
                {
                    return false;
                }
                //BussyUsers.UnionWith(users.ToHashSet());
            }
            List<bool> temp = new List<bool>();

            string keyVal = "66";
            var random = new Random();
            lock (GroupApprove)
            {
                while (GroupApprove.Keys.Contains(keyVal))
                {
                    //TO DO make better namer of groups, that goes above 1000
                    keyVal = random.Next(1000).ToString();
                }
                GroupApprove.Add(keyVal, users.Select(x => (x, false)).ToList());
            }
            lock (Connections)
            {
                foreach ( string s in users)
                {
                    Clients.Client(Connections[s]).Invitation(keyVal, users);
                }
            }
            return true;
        }

        public void GetInVal(string GroupName, bool Decision) 
        {
            if (!GroupApprove.ContainsKey(GroupName))
            {
                return;
            }
            if (Decision)
            {
                lock (GroupApprove)
                {
                    var index = GroupApprove[GroupName].FindIndex(x => x.Item1 == Name());
                    GroupApprove[GroupName][index] = (GroupApprove[GroupName][index].Item1, true);
                    if (GroupApprove[GroupName].All(x => x.Item2))
                    {
                        var names = GroupApprove[GroupName].Select(x => x.Item1);
                        GroupApprove.Remove(GroupName);
                        var keyVal = "66";
                        var random = new Random();
                        lock (CurrGames)
                        {
                            while (CurrGames.Keys.Contains(keyVal))
                            {
                                //TO DO make better namer of groups, that goes above 1000
                                keyVal = random.Next(1000).ToString();
                            }
                            CurrGames.Add(keyVal, new Game(names.ToList()));
                        }
                        List<Player> p;
                        lock (CurrGames)
                        {
                            p = CurrGames[keyVal].Players;
                        
                            foreach (var i in p)
                            {
                                lock (Connections)
                                {
                                    Clients.Client(Connections[i.Name]).SetGameId(keyVal);
                                    Clients.Client(Connections[i.Name]).DisplayImage(ImageTo64(Path.Combine(AppContext.BaseDirectory, "") + @"Content\Images\Heroes\" + i.HeroType.ToString().ToLower() + ".png"));
                                    Clients.Client(Connections[i.Name]).SetHealth(i.Health);
                                    Clients.Client(Connections[i.Name]).SetRole(i.RoleType.ToString());
                                }
                                SendHandCards(i);
                                var enemyPlayers = new List<(string, int, string)>();
                                foreach ( var j in p)
                                {
                                    enemyPlayers.Add((j.Name, j.Health, (j.RoleType == Role.Sherif) ? "Sherif" : "NotSherif"));
                                }
                                lock (Connections)
                                {
                                    Clients.Client(Connections[i.Name]).AddEnemies(enemyPlayers);
                                }
                                var player = i;
                                var tempImage = ImageTo64(Path.Combine(AppContext.BaseDirectory, "") + @"Content\Images\Heroes\" + player.HeroType.ToString().ToLower() + ".png");
                                List<(byte[], string)> images = new List<(byte[], string)>();
                                foreach (var j in player.CardsOnTable)
                                {
                                    images.Add((
                                        ImageTo64(
                                            Path.Combine(AppContext.BaseDirectory, "")
                                            + @"Content\Images\PlayCards\"
                                            + j.Type.ToString().ToLower()
                                            + ".png")
                                        , j.ID())
                                        );
                                }
                                lock (Connections)
                                {
                                    Clients.Client(Connections[i.Name]).SetGameView(tempImage, Name(), player.HeroType.ToString(), player.Health, images, player.Hand.Count);
                                    Clients.Client(Connections[i.Name]).SetCurrPlayer(CurrGames[keyVal].GetCurrentTurnPlayer().Name);
                                }
                            }
                        }
                    }
                    
                }
            }
            else
            {
                IEnumerable<string> invited;
                lock (GroupApprove)
                {
                    invited = GroupApprove[GroupName].Select(x => x.Item1);
                    GroupApprove.Remove(GroupName);
                }
                lock (Connections) {
                    foreach (var s in invited)
                    {
                        Clients.Client(Connections[s]).InvitationRefused(Name());
                    }
                }
            }
        }

        public void GetInfoUser(string game, string name)
        {
            Player player;
            lock (CurrGames)
            {
                if (!CurrGames.ContainsKey(game))
                {
                    Clients.Client(Context.ConnectionId).Message("game you are calling doesnt exist");
                    return;
                }
                player = CurrGames[game].Players.Find(x => x.Name == name);
                if (player == null)
                {
                    Clients.Client(Context.ConnectionId).Message("you are not part of game");
                    return;
                }
            }
            var tempImage = ImageTo64(Path.Combine(AppContext.BaseDirectory, "") + @"Content\Images\Heroes\" + player.HeroType.ToString().ToLower() + ".png");
            List<(byte[],string)> images = new List<(byte[],string)>() ;
            foreach (var i in player.CardsOnTable)
            {
                images.Add((
                    ImageTo64(
                        Path.Combine(AppContext.BaseDirectory, "")
                        + @"Content\Images\PlayCards\"
                        + i.Type.ToString().ToLower()
                        + ".png")
                    ,i.ID())
                    );
            }
            Clients
                .Client(Context.ConnectionId)
                .SetGameView(tempImage, name, player.HeroType.ToString(), player.Health, images,player.Hand.Count);
        }

        public string Date()
        {
            return DateTime.Now.ToString();
        }
    }
}