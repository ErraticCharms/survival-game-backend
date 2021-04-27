using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Survival_Game_Backend_Server
{
    public class ServerFunctions
    {
        public static void CreateLobby(Packet packet)
        {
            string receivedIP = packet.ReadString();
            int receivedPort = packet.ReadInt();

            int lobbyId = 0;

            while (Server.lobbies.FindIndex(l => l.id == lobbyId.ToString()) > -1)
            {
                lobbyId = new Random().Next(11111, 99999);
            }

            Lobby createdLobby = new Lobby(lobbyId.ToString(), receivedIP, receivedPort);

            Server.lobbies.Add(createdLobby);

            using (Packet _packet = new Packet())
            {
                _packet.WriteInt(4);
                _packet.WriteString(lobbyId.ToString());
                
                Server.SendData(_packet, new IPEndPoint(IPAddress.Parse(receivedIP), receivedPort));
            }

            Console.WriteLine($"Created lobby {lobbyId} with ip {receivedIP} on port {receivedPort}");
        }

        public static void JoinLobby(Packet packet)
        {     
            string lobbyId = packet.ReadString();

            Lobby lobbyToJoin = Server.lobbies.Find(l => l.id == lobbyId);

            using (Packet _packet = new Packet())
            {
                _packet.WriteInt(4);
                _packet.WriteString(lobbyId);
                _packet.WriteString(lobbyToJoin.ip.ToString());
                _packet.WriteInt(lobbyToJoin.port);

                Server.SendData(_packet, new IPEndPoint(IPAddress.Parse(packet.ReadString()), packet.ReadInt()));
            }

            Console.WriteLine($"Player joining lobby {lobbyId}");
        }

        public static void IsLobby(Packet packet)
        {
            Packet packetToSend = new Packet();

            string lobbyId = packet.ReadString();

            packetToSend.WriteBool(Server.lobbies.FindIndex(l => l.id == lobbyId) > -1);

            Server.SendData(packetToSend, new IPEndPoint(IPAddress.Parse(packet.ReadString()), packet.ReadInt()));
        }

        public static void Login(Packet packet)
        {
            Packet _packet = new Packet();
            _packet.WriteInt(1);

            string username = packet.ReadString();
            string password = packet.ReadString();
            string receivedIp = packet.ReadString();
            int receivedPort = packet.ReadInt();

            try
            {
                FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;

                FilterDefinition<BsonDocument> filter = builder.Eq("username", username) & builder.Eq("password", password);

                BsonDocument userDoc = Server.userAccountDatabase.Find(filter).FirstOrDefault();

                userDoc.Set("lastIp", receivedIp);
                userDoc.Set("lastPort", receivedPort);

                Server.userAccountDatabase.ReplaceOne(filter, userDoc);

                _packet.WriteBool(true);
            }
            catch(Exception)
            {
                _packet.WriteBool(false);
            }

            Server.SendData(_packet, new IPEndPoint(IPAddress.Parse(receivedIp), receivedPort));
        }

        public static void LoginFromGame(Packet packet)
        {
            IPAddress authAddress = IPAddress.Parse(packet.ReadString());
            int authPort = packet.ReadInt();

            Packet _packet = new Packet();
            _packet.WriteInt(1);

            try
            {
                BsonDocument userDoc;
                FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
                FilterDefinition<BsonDocument> filter = builder.Eq("lastIp", authAddress);

                userDoc = Server.userAccountDatabase.Find(filter).First();

                if (userDoc != null)
                {
                    Console.WriteLine($"User logged in from ip {authAddress}");
                }
                else
                {
                    Console.WriteLine($"Error logging in from ip {authAddress}");
                }

                _packet.WriteBool(true);
                _packet.WriteString(userDoc.GetValue("username").AsString); 
            }
            catch(Exception)
            {
                _packet.WriteBool(false);
            }

            Server.SendData(_packet, new IPEndPoint(authAddress, authPort));
        }
    }
}
