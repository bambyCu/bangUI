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

        //GroupID -> (userName, InvitationAccepted)
        private static SortedDictionary<string, List<(string, bool)>> GroupApprove =
            new SortedDictionary<string, List<(string, bool)>>();

        


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
            string name = Connections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (name == null) { return base.OnDisconnected(stopCalled); }
            lock (Connections)
            {
                Connections.Remove(name);
            }
            Clients.All.Disconnect(name);
            return base.OnDisconnected(stopCalled);
        }

        public string[] GetUsers()
        {
            return Connections.Keys.ToArray();
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
            var name = Connections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (!GroupApprove.ContainsKey(GroupName))
            {
                return;
            }
            if (Decision)
            {
                lock (GroupApprove)
                {
                    var index = GroupApprove[GroupName].FindIndex(x => x.Item1 == name);
                    GroupApprove[GroupName][index] = (GroupApprove[GroupName][index].Item1, true);
                    if (GroupApprove[GroupName].All(x => x.Item2))
                    {
                        var names = GroupApprove[GroupName].Select(x => x.Item1);
                        GroupApprove.Remove(GroupName);
                        string keyVal = "66";
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
                        ImageConverter converter = new ImageConverter();
                        List<Player> p;
                        lock (CurrGames)
                        {
                            p = CurrGames[keyVal].Players;
                        }
                        foreach (var i in p)
                        {
                            var image = Image.FromFile(Path.Combine(AppContext.BaseDirectory, "") + @"Content\Images\heroes\" + i.HeroType.ToString().ToLower() + ".png");
                            Clients.All.LogIn();
                            Clients.Client(Connections[i.Name]).DisplayImage((byte[])converter.ConvertTo(image, typeof(byte[])));
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
                        Clients.Client(Connections[s]).InvitationRefused(name);
                    }
                }
            }
        }
        public string Date()
        {
            return DateTime.Now.ToString();
        }
    }
}