using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FGridUdp
{
    public struct Received
    {
        public IPEndPoint Sender;
        public string Message;
    }

    abstract class UdpBase
    {
        protected UdpClient Client;

        protected UdpBase()
        {
            Client = new UdpClient();
        }


        public async Task<Received> Receive()
        {
            var result = await Client.ReceiveAsync();
            return new Received()
            {
                Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
                Sender = result.RemoteEndPoint
            };
        }

        public struct FGridPingPacket
        {
            public byte version;
            public ulong timestamp;
        }

        public byte[] serializePing(FGridPingPacket input)
        {
            byte[] output = new byte[1 + 8];
            byte[] timestamp = BitConverter.GetBytes(input.timestamp);
            output[0] = input.version;
            Array.Copy(timestamp, 0, output, 1, 8);
            return output;
        }

        public FGridPingPacket deserializePing(byte[] input)
        {
            FGridPingPacket output = new FGridPingPacket();

            output.version = input[0];
            output.timestamp = BitConverter.ToUInt64(input, 1);
            return output;
        }

        public struct FGridDMXPacket
        {
            public byte version;
            public string hostname; //16
            public string key; //16
            public ulong packetId;
            public ulong timestamp;
            public byte universe;
            public byte[] dmxData; //512
        }

        public byte[] serializeDMX(FGridDMXPacket input)
        {
            byte[] output = new byte[1 + 16 + 16 + 8 + 8 + 1 + 512];
            byte[] hostname = Encoding.ASCII.GetBytes(input.hostname);
            byte[] key = Encoding.ASCII.GetBytes(input.key);
            byte[] packetId = BitConverter.GetBytes(input.packetId);
            byte[] timestamp = BitConverter.GetBytes(input.timestamp);

            output[0] = input.version;
            Array.Copy(hostname, 0, output, 1, 16);
            Array.Copy(key, 0, output, 1 + 16, 16);
            Array.Copy(packetId, 0, output, 1 + 16 + 16, 8);
            Array.Copy(timestamp, 0, output, 1 + 16 + 16 + 8, 8);
            output[1 + 16 + 16 + 8 + 8] = input.universe;
            Array.Copy(input.dmxData, 0, output, 1 + 16 + 16 + 8 + 8 + 1, 512);

            return output;
        }

        public FGridDMXPacket deserializeDMX(byte[] input)
        {
            FGridDMXPacket output = new FGridDMXPacket();

            output.version = input[0];
            output.hostname = input.Skip(1).Take(16).ToArray().ToString();
            output.key = input.Skip(1 + 16).Take(16).ToArray().ToString();
            output.packetId = BitConverter.ToUInt64(input, 1 + 16 + 16);
            output.timestamp = BitConverter.ToUInt64(input, 1 + 16 + 16 + 8);
            output.universe = input[1 + 16 + 16 + 8 + 8];
            output.dmxData = input.Skip(1 + 16 + 16 + 8 + 8 + 1).Take(512).ToArray();

            return output;
        }
    }

    //Server
    class UdpListener : UdpBase
    {
        private IPEndPoint _listenOn;

        public UdpListener() : this(new IPEndPoint(IPAddress.Any, 32123))
        {
        }

        public UdpListener(IPEndPoint endpoint)
        {
            _listenOn = endpoint;
            Client = new UdpClient(_listenOn);
        }

        public void Reply(string message, IPEndPoint endpoint)
        {
            var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length, endpoint);
        }

    }

    //Client
    class UdpUser : UdpBase
    {
        private UdpUser() { }

        public static UdpUser ConnectTo(string hostname, int port)
        {
            var connection = new UdpUser();
            connection.Client.Connect(hostname, port);
            return connection;
        }

        public void Send(string message)
        {
            var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length);
        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            //create a new server
            var server = new UdpListener();

            //start listening for messages and copy the messages back to the client
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var received = await server.Receive();
                    server.Reply("copy " + received.Message, received.Sender);
                    if (received.Message == "quit")
                        break;
                }
            });

            //create a new client
            var client = UdpUser.ConnectTo("127.0.0.1", 32123);

            //wait for reply messages from server and send them to console 
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        var received = await client.Receive();
                        Console.WriteLine(received.Message);
                        if (received.Message.Contains("quit"))
                            break;
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex);
                    }
                }
            });

            //type ahead :-)
            string read;
            do
            {
                read = Console.ReadLine();
                client.Send(read);
            } while (read != "quit");
        }
    }
}