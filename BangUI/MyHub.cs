using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalRTutorial
{
    [HubName("mainHub")]
    public class MyHub : Hub
    {
        static private HashSet<string> Logged = new HashSet<string>();
        public void Announce(string input)
        {
            Clients.All.Announce(input);
        }
        public bool LogIn(string username)
        {
            if (Logged.Contains(username)) { return false; }
            Logged.Add(username);
            Clients.All.LogIn(username);
            return true;
        }

        public string[] GetUsers()
        {
            return Logged.ToArray();
        }
        public string Date()
        {
            return DateTime.Now.ToString();
        }
    }
}