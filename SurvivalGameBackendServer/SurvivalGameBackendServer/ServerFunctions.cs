using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

namespace Survival_Game_Backend_Server
{
    public class ServerFunctions
    {
        public static void CreateLobby(Packet packet)
        {
            string receivedIP = packet.ReadString();
            int receivedPort = packet.ReadInt();

            Server.currentLobbyPorts.Add(receivedPort);
            Server.currentLobbyAddresses.Add(receivedIP);
            Server.currentLobbyHosts.Add("1234567890");

            Console.WriteLine($"Created new lobby with ip {receivedIP} on port {receivedPort}");
        }

        public static void JoinLobby(Packet packet)
        {
            string lobbyId = packet.ReadString();

            Packet packetToSend = new Packet();
            packetToSend.WriteInt(4);
            packetToSend.WriteString(Server.currentLobbyAddresses[Server.currentLobbyHosts.IndexOf(lobbyId)]);
            packetToSend.WriteInt(Server.currentLobbyPorts[Server.currentLobbyHosts.IndexOf(lobbyId)]);

            Server.SendData(packetToSend, new IPEndPoint(IPAddress.Parse(packet.ReadString()), packet.ReadInt()));

            Console.WriteLine($"Player joining lobby {lobbyId}");
        }
    }
}
