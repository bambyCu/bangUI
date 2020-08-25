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
using System.Web.ModelBinding;
using BangUI.Models;
using Microsoft.Ajax.Utilities;

namespace SignalRTutorial
{
    [HubName("mainHub")]
    public class MyHub : Hub
    {
        //Username -> ConnectionId
        private string GroupId = "";
        private readonly static SortedDictionary<string, string> UsersToConnections =
            new SortedDictionary<string, string>();
        //ConnectionId -> Username
        private readonly static SortedDictionary<string, string> ConnectionsToUsers =
            new SortedDictionary<string, string>();

         private readonly static HashSet<string> BussyUsers =
             new HashSet<string>();

        //name -> Game
        private readonly static SortedDictionary<string, GameWrapper> CurrGames =
            new SortedDictionary<string, GameWrapper>();

        //GameID -> (userName, InvitationAccepted)
        private static SortedDictionary<string, List<(string, bool)>> GroupApprove =
            new SortedDictionary<string, List<(string, bool)>>();


        private string Name
        {
            get {
                lock (UsersToConnections)
                {
                    if (!UsersToConnections.ContainsKey(Context.ConnectionId))
                        return "haaaaaaa";
                    return UsersToConnections[Context.ConnectionId];
                } 
            }
        }

        private GameWrapper Game()
        {
            lock (CurrGames)
            {
                return CurrGames[ConnectionsToUsers[Context.ConnectionId]];
            }
        }


        private byte[] ImageTo64(string src)
        {
            if (!File.Exists(src))
                return null;
            var converter = new ImageConverter();
            var image = Image.FromFile(src);
            return (byte[])converter.ConvertTo(image, typeof(byte[]));
        }

        private void SendHandCards(Player p)
        {
            var playCards = p.Hand.Select(
                x => new { image = ImageTo64(FilesInfo.CardTypeToPath(x.Type)), id = x.Id });
            lock (UsersToConnections)
            {
                Clients.Client(UsersToConnections[p.Name]).AddHandCards(playCards.ToArray());
            }
        }

        public byte[] TypeImageByteStream(string type)
        {
            if (type == null)
                return null;
            return ImageTo64(FilesInfo.CardTypeToPath(type));
        }
        /*
        private void UpdateGame(string gameId)
        {
            
            lock (CurrGames)
            {
                foreach (var i in CurrGames[gameId].Players)
                {
                    //Clients.Client(Connections[i.Name]).DisplayImage(ImageTo64(Path.Combine(AppContext.BaseDirectory, "") + @"Content\Images\Heroes\" + i.HeroType.ToString().ToLower() + ".png"));
                    lock (UsersToConnections)
                    {
                        Clients.Client(UsersToConnections[i.Name]).SetHealth(i.Health);
                        Clients.Client(UsersToConnections[i.Name]).SetRole(i.RoleType.ToString());
                    }
                    SendHandCards(i);
                    var enemyPlayers = new List<(string, int, string)>();
                    foreach (var j in CurrGames[gameId].Players)
                    {
                        enemyPlayers.Add((j.Name, j.Health, (j.RoleType == Role.Sherif) ? "Sherif" : "NotSherif"));
                    }
                    lock (UsersToConnections)
                    {
                        Clients.Client(UsersToConnections[i.Name]).AddEnemies(enemyPlayers);
                        Clients.Client(UsersToConnections[i.Name]).SetCurrPlayer(CurrGames[gameId].CurrentPlayer().Name);
                    }
                }
            }
        }*/

        public string LogIn(string userName)
        {
            //name must be alphanumeric and shorter than 11 chars
            if (userName == null || userName.Length > 10 || userName.Length == 0 || userName.Any(x => !Char.IsLetterOrDigit(x)))
                return "incorrect";
            lock (UsersToConnections)
            {
                if (UsersToConnections.Keys.Contains(userName)) { return "taken"; }
                UsersToConnections.Add(userName, Context.ConnectionId);
            }
            Clients.All.LogIn(userName);
            return "correct";
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (Name == null) { return base.OnDisconnected(stopCalled); }
            Clients.All.Disconnect(Name);
            lock (UsersToConnections)
            {
                UsersToConnections.Remove(Name);
            }
            return base.OnDisconnected(stopCalled);
        }

        /*public bool ApplyGameIdCardTo(string gameId, string cardId, string target)
        {
            string mess;
            lock (CurrGames)
            {
                if (CurrGames[gameId].CurrentPlayer.Name != Name())
                {
                    lock (UsersToConnections)
                    {
                        //Clients.Client(Connections[Name()]).Message("it is not your turn");
                        //handled on client
                    }
                    return false;
                }
                if (!CurrGames.ContainsKey(gameId)) { return false; }
                if (!CurrGames[gameId].Names.Contains(target)) { return false; }
                CurrGames[gameId].applyCard(Name(), cardId, target);
                mess = CurrGames[gameId].LastMessage;
            }
            
            if (mess != "")
            {
                lock (UsersToConnections)
                {
                    foreach (Player i in CurrGames[gameId].Players)
                    {
                        Clients.Client(UsersToConnections[i.Name]).addToMessageList(mess);
                    }
                }
            }
            UpdateGame(gameId);
            
            return true;
        }*/

        
        public void Discard(int cardId)
        {
            lock (CurrGames)
            {
                var game = CurrGames[ConnectionsToUsers[Context.ConnectionId]];
                game.DiscardCard(Name, cardId);
            }
        }
        /*public void EndTurn()
        {
            
            lock (CurrGames[GameID()])
            {
                var game = CurrGames[GameID()];

                var player = game.CurrentPlayer();
                if (Name() != player.Name)
                {
                    lock (UsersToConnections)
                    {
                        Clients.Client(UsersToConnections[Name()]).Message("it is not your turn");
                    }
                    return;
                }
                if (player.Hand.Count > player.Health)
                {
                    Clients.Client(UsersToConnections[Name()]).Message("too many cards in hand");
                    return;
                }
                var playersAlive = game.Players.Count;
                game.NextTurn();
                if (game.Players.Count != playersAlive)//if dynamite has been activated and someone died 
                {
                    var outcome = game.IsGameFinshed();
                    foreach (var i in game.Names)
                    {
                        Clients.Client(UsersToConnections[i]).addToMessageList(player.Name + " has just been kiled by dynamite");
                        if (outcome != null)
                        {
                            Clients.Client(UsersToConnections[i]).addToMessageList(outcome + " team has just won");
                        }
                    }

                    UpdateGame(GameID());
                    return;
                }
                SendHandCards(game.CurrentPlayer());
                foreach (var i in game.Names)
                {
                    Clients.Client(UsersToConnections[i]).SetCurrPlayer(game.CurrentPlayer().Name);
                }
            }
        }*/
        public string[] GetUsers()
        {
            lock (UsersToConnections)
            {
                return UsersToConnections.Keys.ToArray();
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
                if (BussyUsers.Any(x => users.Contains(x)))
                    return false;
                //BussyUsers.UnionWith(users.ToHashSet());
            }
            lock (GroupApprove)
            {
                var playerList = users.Select(x => (x, false)).ToList();
                users.ForEach(x => GroupApprove.Add(x, playerList));
            }
            lock (UsersToConnections)
            {
                users
                    .ForEach(x => Clients.Client(UsersToConnections[x]).Invitation(users));
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
                    var index = GroupApprove[GroupName].FindIndex(x => x.Item1 == Name);
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
                            CurrGames.Add(keyVal, new GameWrapper( this, new Game(names.ToList())));
                        }
                        List<Player> p;
                        lock (CurrGames)
                        {
                            p = CurrGames[keyVal].Players;
                        
                            foreach (var i in p)
                            {
                                lock (UsersToConnections)
                                {
                                    Clients.Client(UsersToConnections[i.Name]).SetGameId(keyVal);
                                    Clients.Client(UsersToConnections[i.Name]).DisplayImage(ImageTo64(Path.Combine(AppContext.BaseDirectory, "") + @"Content\Images\Heroes\" + i.HeroType.ToString().ToLower() + ".png"));
                                    Clients.Client(UsersToConnections[i.Name]).SetHealth(i.Health);
                                    Clients.Client(UsersToConnections[i.Name]).SetRole(i.RoleType.ToString());
                                }
                                SendHandCards(i);
                                var enemyPlayers = new List<(string, int, string)>();
                                foreach ( var j in p)
                                {
                                    enemyPlayers.Add((j.Name, j.Health, (j.RoleType == Role.Sherif) ? "Sherif" : "NotSherif"));
                                }
                                lock (UsersToConnections)
                                {
                                    Clients.Client(UsersToConnections[i.Name]).AddEnemies(enemyPlayers);
                                }
                                var player = i;
                                var tempImage = ImageTo64(Path.Combine(AppContext.BaseDirectory, "") + @"Content\Images\Heroes\" + player.HeroType.ToString().ToLower() + ".png");
                                List<(byte[], int)> images = new List<(byte[], int)>();
                                foreach (var j in player.CardsOnTable)
                                {
                                    images.Add((
                                        ImageTo64(
                                            Path.Combine(AppContext.BaseDirectory, "")
                                            + @"Content\Images\PlayCards\"
                                            + j.Type.ToString().ToLower()
                                            + ".png")
                                        , j.Id)
                                        );
                                }
                                lock (UsersToConnections)
                                {
                                    Clients.Client(UsersToConnections[i.Name]).SetGameView(tempImage, Name, player.HeroType.ToString(), player.Health, images, player.Hand.Count);
                                    Clients.Client(UsersToConnections[i.Name]).SetCurrPlayer(CurrGames[keyVal].CurrentPlayer.Name);
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
                lock (UsersToConnections) {
                    foreach (var s in invited)
                    {
                        Clients.Client(UsersToConnections[s]).InvitationRefused(Name);
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
            List<(byte[],int)> images = new List<(byte[],int)>() ;
            foreach (var i in player.CardsOnTable)
            {
                images.Add((
                    ImageTo64(
                        Path.Combine(AppContext.BaseDirectory, "")
                        + @"Content\Images\PlayCards\"
                        + i.Type.ToString().ToLower()
                        + ".png")
                    ,i.Id)
                    );
            }
            Clients
                .Client(Context.ConnectionId)
                .SetGameView(tempImage, name, player.HeroType.ToString(), player.Health, images,player.Hand.Count);
        }
    }
}