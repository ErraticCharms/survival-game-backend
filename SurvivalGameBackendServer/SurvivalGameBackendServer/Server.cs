using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Survival_Game_Backend_Server
{
    class Server
    {
        public static List<Lobby> lobbies = new List<Lobby>();

        private delegate void PacketHandler(Packet packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        static UdpClient serverUdp;
        static TcpListener tcpListener;

        public static MongoClient mongoClient;
        public static IMongoDatabase mainData;
        public static IMongoCollection<BsonDocument> userAccountDatabase;

        public static void Start()
        {
            StartData();

            tcpListener = new TcpListener(IPAddress.Any, 63904);
            serverUdp = new UdpClient(63902);

            serverUdp.BeginReceive(UDPReceiveCallback, null);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        }

        static void UDPReceiveCallback(IAsyncResult result)
        {            
            IPEndPoint receivedEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedData = serverUdp.EndReceive(result, ref receivedEndPoint);
            serverUdp.BeginReceive(UDPReceiveCallback, null);

            HandleData(receivedData, receivedEndPoint);
        }

        static void TCPConnectCallback(IAsyncResult result)
        {
            tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        }

        static void HandleData(byte[] data, IPEndPoint receivedEndPoint) 
        {
            Packet receivedPacket = new Packet();
            receivedPacket.WriteBytes(data);

            receivedPacket.WriteString(receivedEndPoint.Address.ToString());
            receivedPacket.WriteInt(receivedEndPoint.Port);

            int packetHandler = receivedPacket.ReadInt();

            packetHandlers[packetHandler](receivedPacket);
        }

        static void StartData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { 1, ServerFunctions.CreateLobby },
                { 2, ServerFunctions.JoinLobby },
                { 3, ServerFunctions.IsLobby },
                { 4, ServerFunctions.Login },
                { 5, ServerFunctions.LoginFromGame }
            };

            mongoClient = new MongoClient(
                "mongodb+srv://ServerAdmin:Wandmoon1@survivalgamecluster.yzv91.mongodb.net/MainData?retryWrites=true&w=majority"
            );

            mainData = mongoClient.GetDatabase("MainData");

            userAccountDatabase = mainData.GetCollection<BsonDocument>("UserAccounts");
        }

        public static void SendData(Packet packet, IPEndPoint endPoint)
        {
            packet.WriteLength();
            serverUdp.Send(packet.bytes.ToArray(), packet.length, endPoint);
        }
    }
}
