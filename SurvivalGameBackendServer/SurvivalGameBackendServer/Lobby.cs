using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

namespace Survival_Game_Backend_Server
{
    class Lobby
    {
        public string id;
        public IPAddress ip;
        public int port;
        
        public Lobby(string _id, string _ip, int _port)
        {
            id = _id;
            ip = IPAddress.Parse(_ip);
            port = _port;
        }
    }
}
