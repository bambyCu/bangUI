using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;

namespace SignalRTutorial
{
    [HubName("mainHub")]
    public class MyHub : Hub
    {
        private readonly static SortedDictionary<string, string> Connections =
            new SortedDictionary<string, string>();
        private readonly static SortedDictionary<string, string> Names =
            new SortedDictionary<string, string>();

        public bool LogIn(string userName)
        {
            lock (Connections)
            {
                if (Connections.Keys.Contains(userName)) { return false; }
                Connections.Add(userName, Context.ConnectionId);
            }
            lock (Names)
            {
                Names.Add(Context.User.Identity.Name, userName);
            }
            Clients.All.LogIn(userName);
            return true;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name;
            lock (Names)
            {
                if (!Names.ContainsKey(Context.User.Identity.Name)) { return null; }
                name = Names[Context.User.Identity.Name];
            }
            
            Clients.All.Disconected(Names[Context.User.Identity.Name]);

            lock (Names)
            {
                Names.Remove(Context.User.Identity.Name);
            }
            lock (Connections)
            {
                Connections.Remove(name);
            }
            Clients.All.Disconnect();
            return base.OnDisconnected(stopCalled);
        }

        public string[] GetUsers()
        {
            return Connections.Keys.ToArray();
        }
        public string Date()
        {
            return DateTime.Now.ToString();
        }
    }
}