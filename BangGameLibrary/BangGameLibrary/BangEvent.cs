using System;
using System.Collections.Generic;
using System.Text;

namespace BangGameLibrary
{
    public class BangGameEventArgs : EventArgs
    {
        public string Message { get; set; }
        public BangGameEventArgs(string message)
        {
            Message = message;
        }
    }

    public delegate void BangGameEventHandler(Object sender, BangGameEventArgs e);
}
