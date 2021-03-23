using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Survival_Game_Backend_Server
{
    class Server
    {
        public static List<int> currentLobbyPorts = new List<int>();
        public static List<string> currentLobbyAddresses = new List<string>();
        public static List<string> currentLobbyHosts = new List<string>();

        private delegate void PacketHandler(Packet packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        static UdpClient serverUdp;

        public static void Start()
        {
            StartData();
            
            serverUdp = new UdpClient(63902);

            serverUdp.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine("Began receive");
        }

        static void UDPReceiveCallback(IAsyncResult result)
        {
            IPEndPoint receivedEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedData = serverUdp.EndReceive(result, ref receivedEndPoint);
            serverUdp.BeginReceive(UDPReceiveCallback, null);

            HandleData(receivedData);
        }

        static void HandleData(byte[] data) 
        {
            Packet receivedPacket = new Packet();
            receivedPacket.WriteBytes(data);

            int packetHandler = receivedPacket.ReadInt();

            packetHandlers[packetHandler](receivedPacket);
        }

        static void StartData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { 1, ServerFunctions.CreateLobby },
                { 2, ServerFunctions.JoinLobby }
            };
        }

        public static void SendData(Packet packet, IPEndPoint endPoint)
        {
            packet.WriteLength();
            serverUdp.Send(packet.bytes.ToArray(), packet.length, endPoint);
        }
    }
}
